using SudokuServer.GameLogic;
using SudokuServer.Models;
using System;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SudokuServer.Network
{
    public class ClientSession
    {
        private bool _isDisposed = false;
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public TcpClient Client { get; }
        public NetworkStream Stream { get; }
        public StreamReader Reader { get; }
        public StreamWriter Writer { get; }
        public string Username { get; set; } = "Guest";
        public int Score { get; set; } = 0;
        public bool IsReady { get; set; } = false;
        public CancellationTokenSource Cts { get; } = new CancellationTokenSource();
        private readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1, 1);

        public ClientSession(TcpClient client)
        {
            Client = client;
            Stream = client.GetStream();
            Reader = new StreamReader(Stream, Encoding.UTF8);
            Writer = new StreamWriter(Stream, Encoding.UTF8) { AutoFlush = true };
        }

        public async Task SendAsync(object obj)
        {
            try
            {
                if (Cts.IsCancellationRequested || _isDisposed) return;

                await _writeLock.WaitAsync(Cts.Token);
                try
                {
                    string json = JsonSerializer.Serialize(obj);
                    await Writer.WriteLineAsync(json);
                }
                finally
                {
                    _writeLock.Release();
                }
            }
            catch
            {
                // Connection likely lost
            }
        }

        public void Disconnect()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            try { Cts.Cancel(); } catch { }
            try { Reader.Dispose(); } catch { }
            try { Writer.Dispose(); } catch { }
            try { Stream.Dispose(); } catch { }
            try { Client.Close(); } catch { }
            try { _writeLock.Dispose(); } catch { }
            try { Cts.Dispose(); } catch { }
        }
    }

    public class SocketManager
    {
        private TcpListener? _listener;
        private CancellationTokenSource? _cts;
        private readonly ConcurrentDictionary<string, Player> _players = new();
        private readonly ConcurrentDictionary<string, Room> _rooms = new();

        public event Action<string>? OnLog;
        public event Action? OnClientListChanged;
        public event Action? OnGameStateChanged;

        public bool IsRunning { get; private set; } = false;
        public int Port { get; private set; } = 0;
        public bool IsGameActive
        {
            get
            {
                if (_rooms.TryGetValue("default", out var room)) return room.IsGameActive;
                return false;
            }
        }
        public int ConnectedClientsCount => _players.Count;

        public SocketManager()
        {
            var defaultRoom = new Room("default", "Default Room");
            _rooms.TryAdd(defaultRoom.Id, defaultRoom);
        }

        public int[,] GetSolutionBoard()
        {
            if (_rooms.TryGetValue("default", out var room))
            {
                lock (room.GameLock)
                {
                    return (int[,])room.Engine.SolutionBoard.Clone();
                }
            }
            return new int[9, 9];
        }

        public int[,] GetPlayerBoard()
        {
            if (_rooms.TryGetValue("default", out var room))
            {
                lock (room.GameLock)
                {
                    return (int[,])room.Engine.PlayerBoard.Clone();
                }
            }
            return new int[9, 9];
        }

        public List<ClientSession> GetClientsList() => _players.Values.Select(p => p.Session).ToList();

        public void Start(int port)
        {
            if (IsRunning) return;

            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Any, port);

            try
            {
                _listener.Start();
                Port = ((IPEndPoint)_listener.LocalEndpoint).Port;
                IsRunning = true;
                Log($"Server started on port {Port}. Listening for connections...");
                Task.Run(() => AcceptClientsAsync(_cts.Token));
            }
            catch (Exception ex)
            {
                Log($"Error starting server: {ex.Message}");
                try { _listener.Stop(); } catch { }
                _listener = null;
                try { _cts.Cancel(); _cts.Dispose(); } catch { }
                _cts = null;
                IsRunning = false;
                Port = 0;
                throw;
            }
        }

        public void Stop()
        {
            if (!IsRunning) return;

            Log("Stopping server...");
            foreach (var room in _rooms.Values)
            {
                lock (room.GameLock) { room.IsGameActive = false; }
            }

            _cts?.Cancel();
            _listener?.Stop();

            foreach (var player in _players.Values) { player.Session.Disconnect(); }
            _players.Clear();

            _rooms.Clear();
            var defaultRoom = new Room("default", "Default Room");
            _rooms.TryAdd(defaultRoom.Id, defaultRoom);

            IsRunning = false;
            Port = 0;
            Log("Server stopped.");
            OnClientListChanged?.Invoke();
            OnGameStateChanged?.Invoke();
        }

        private async Task AcceptClientsAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (_listener == null) break;
                    TcpClient client = await _listener.AcceptTcpClientAsync(token);
                    _ = Task.Run(() => HandleClientAsync(client, token));
                }
                catch (ObjectDisposedException) { break; }
                catch (Exception ex)
                {
                    if (!token.IsCancellationRequested) Log($"Error accepting client: {ex.Message}");
                }
            }
        }

        private async Task HandleClientAsync(TcpClient tcpClient, CancellationToken token)
        {
            string ipAddress = tcpClient.Client.RemoteEndPoint?.ToString() ?? "Unknown";
            Log($"New connection attempt from {ipAddress}");
            ClientSession session = new ClientSession(tcpClient);

            try
            {
                while (!token.IsCancellationRequested && !session.Cts.Token.IsCancellationRequested)
                {
                    string? line = await session.Reader.ReadLineAsync(session.Cts.Token);
                    if (line == null) break;
                    await ProcessMessageAsync(session, line);
                }
            }
            catch (Exception ex)
            {
                if (!(ex is OperationCanceledException || ex is ObjectDisposedException || ex is IOException))
                {
                    Log($"Session error for {session.Username} ({ipAddress}): {ex.Message}");
                }
            }
            finally
            {
                session.Disconnect();
                if (_players.TryRemove(session.Id, out var player))
                {
                    Log($"Player {player.Username} ({ipAddress}) disconnected.");
                    if (player.RoomId != null && _rooms.TryGetValue(player.RoomId, out var room))
                    {
                        room.RemovePlayer(player.Id);
                        room.BroadcastPlayerList();
                    }
                    OnClientListChanged?.Invoke();
                }
            }
        }

        private async Task ProcessMessageAsync(ClientSession session, string messageJson)
        {
            try
            {
                var msg = JsonSerializer.Deserialize<NetworkMessage>(messageJson);
                if (msg == null) return;

                if (msg.Type != "CLIENT_CONNECT" && !_players.ContainsKey(session.Id))
                {
                    Log($"Warning: Received '{msg.Type}' from unregistered client '{session.Username}'.");
                    return;
                }

                switch (msg.Type)
                {
                    case "CLIENT_CONNECT":
                        session.Username = string.IsNullOrEmpty(msg.Username) ? "Guest" : msg.Username;
                        Player newPlayer = new Player(session.Id, session.Username, session);
                        _players[session.Id] = newPlayer;
                        Log($"Player '{newPlayer.Username}' joined the lobby.");

                        if (_rooms.TryGetValue("default", out var defaultRoom))
                        {
                            defaultRoom.AddPlayer(newPlayer);
                        }

                        await session.SendAsync(new NetworkMessage
                        {
                            Type = "SERVER_CONNECT_RESPONSE",
                            Success = true,
                            PlayerId = session.Id,
                            Message = "Successfully connected to server."
                        });

                        if (newPlayer.RoomId != null && _rooms.TryGetValue(newPlayer.RoomId, out var room))
                        {
                            room.BroadcastPlayerList();
                        }
                        OnClientListChanged?.Invoke();

                        if (newPlayer.RoomId != null && _rooms.TryGetValue(newPlayer.RoomId, out var rRoom))
                        {
                            bool isGameActiveCopy;
                            int[][]? boardJagged = null;
                            lock (rRoom.GameLock)
                            {
                                isGameActiveCopy = rRoom.IsGameActive;
                                if (isGameActiveCopy) boardJagged = ConvertToJagged(rRoom.Engine.PlayerBoard);
                            }

                            if (isGameActiveCopy && boardJagged != null)
                            {
                                await session.SendAsync(new NetworkMessage
                                {
                                    Type = "SERVER_START_GAME",
                                    Board = boardJagged
                                });
                            }
                        }
                        break;

                    case "CLIENT_READY":
                        if (msg.IsReady.HasValue && _players.TryGetValue(session.Id, out var player))
                        {
                            player.IsReady = msg.IsReady.Value;
                            Log($"Player '{player.Username}' is {(player.IsReady ? "READY" : "NOT READY")}");
                            if (player.RoomId != null && _rooms.TryGetValue(player.RoomId, out var pRoom))
                            {
                                pRoom.BroadcastPlayerList();
                            }
                            OnClientListChanged?.Invoke();
                        }
                        break;

                    case "CLIENT_MOVE":
                        if (!_players.TryGetValue(session.Id, out var movingPlayer) || movingPlayer.RoomId == null || !_rooms.TryGetValue(movingPlayer.RoomId, out var playRoom))
                            break;

                        bool isGameActiveCheck;
                        lock (playRoom.GameLock) { isGameActiveCheck = playRoom.IsGameActive; }

                        if (!isGameActiveCheck)
                        {
                            Log($"Ignored move from '{movingPlayer.Username}' because no active game in room.");
                            break;
                        }

                        if (msg.Row.HasValue && msg.Col.HasValue && msg.Value.HasValue)
                        {
                            int r = msg.Row.Value;
                            int c = msg.Col.Value;
                            int val = msg.Value.Value;

                            if (r >= 0 && r < 9 && c >= 0 && c < 9)
                            {
                                bool isCorrect = false;
                                bool alreadySolved = false;
                                bool isFinished = false;
                                bool shouldKick = false;

                                lock (playRoom.GameLock)
                                {
                                    if (!playRoom.IsGameActive) break;

                                    if (playRoom.Engine.PlayerBoard[r, c] == playRoom.Engine.SolutionBoard[r, c])
                                    {
                                        alreadySolved = true;
                                    }
                                    else
                                    {
                                        isCorrect = playRoom.Engine.CheckMove(r, c, val);
                                        if (isCorrect)
                                        {
                                            playRoom.Engine.ApplyMove(r, c, val);
                                            movingPlayer.Score += 10;
                                            Log($"'{movingPlayer.Username}' placed {val} at ({r}, {c}) - CORRECT (+10 pts)");
                                        }
                                        else
                                        {
                                            movingPlayer.Score = Math.Max(0, movingPlayer.Score - 5);
                                            Log($"'{movingPlayer.Username}' placed {val} at ({r}, {c}) - INCORRECT (-5 pts)");
                                            if (movingPlayer.Score == 0) shouldKick = true;
                                        }

                                        isFinished = playRoom.Engine.IsGameFinished();
                                        if (isFinished) playRoom.IsGameActive = false;
                                    }
                                }

                                if (alreadySolved) break;

                                playRoom.Broadcast(new NetworkMessage
                                {
                                    Type = "SERVER_MOVE_UPDATE",
                                    Row = r,
                                    Col = c,
                                    Value = val,
                                    PlayerId = movingPlayer.Id,
                                    Username = movingPlayer.Username,
                                    Score = movingPlayer.Score,
                                    Correct = isCorrect
                                });

                                OnGameStateChanged?.Invoke();

                                if (isFinished)
                                {
                                    playRoom.EndGame();
                                    OnGameStateChanged?.Invoke();
                                }

                                if (shouldKick)
                                {
                                    playRoom.Broadcast(new NetworkMessage
                                    {
                                        Type = "SERVER_CHAT",
                                        Username = "System",
                                        ChatText = $"Player '{movingPlayer.Username}' was kicked for reaching 0 points!"
                                    });
                                    Log($"Player '{movingPlayer.Username}' was kicked for reaching 0 points.");
                                    movingPlayer.Session.Disconnect();
                                }
                            }
                        }
                        break;

                    case "CLIENT_CHAT":
                        if (!string.IsNullOrEmpty(msg.ChatText) && _players.TryGetValue(session.Id, out var chatPlayer))
                        {
                            Log($"[CHAT][Room: {chatPlayer.RoomId}] {chatPlayer.Username}: {msg.ChatText}");
                            if (chatPlayer.RoomId != null && _rooms.TryGetValue(chatPlayer.RoomId, out var chatRoom))
                            {
                                chatRoom.Broadcast(new NetworkMessage
                                {
                                    Type = "SERVER_CHAT",
                                    Username = chatPlayer.Username,
                                    ChatText = msg.ChatText
                                });
                            }
                        }
                        break;

                    case "CLIENT_CREATE_ROOM":
                        if (_players.TryGetValue(session.Id, out var creator))
                        {
                            string newRoomId = Guid.NewGuid().ToString().Substring(0, 6);
                            string roomName = string.IsNullOrEmpty(msg.RoomName) ? $"Room {newRoomId}" : msg.RoomName;

                            Room newRoom = new Room(newRoomId, roomName) { CreatorId = creator.Id };
                            if (_rooms.TryAdd(newRoomId, newRoom))
                            {
                                Log($"Player '{creator.Username}' created room '{roomName}' (ID: {newRoomId})");

                                if (creator.RoomId != null && _rooms.TryGetValue(creator.RoomId, out var oldRoom))
                                {
                                    oldRoom.RemovePlayer(creator.Id);
                                    oldRoom.BroadcastPlayerList();
                                }

                                newRoom.AddPlayer(creator);

                                await session.SendAsync(new NetworkMessage
                                {
                                    Type = "SERVER_JOIN_ROOM_RESPONSE",
                                    Success = true,
                                    RoomId = newRoomId,
                                    Message = $"Successfully created and joined room '{roomName}'."
                                });

                                newRoom.BroadcastPlayerList();
                                OnClientListChanged?.Invoke();
                            }
                            else
                            {
                                await session.SendAsync(new NetworkMessage
                                {
                                    Type = "SERVER_JOIN_ROOM_RESPONSE",
                                    Success = false,
                                    Message = "Failed to create room."
                                });
                            }
                        }
                        break;

                    case "CLIENT_JOIN_ROOM":
                        if (_players.TryGetValue(session.Id, out var joiner) && !string.IsNullOrEmpty(msg.RoomId))
                        {
                            if (_rooms.TryGetValue(msg.RoomId, out var targetRoom))
                            {
                                Log($"Player '{joiner.Username}' joining room '{targetRoom.Name}'");
                                if (joiner.RoomId != null && _rooms.TryGetValue(joiner.RoomId, out var oldRoom))
                                {
                                    oldRoom.RemovePlayer(joiner.Id);
                                    oldRoom.BroadcastPlayerList();
                                }

                                targetRoom.AddPlayer(joiner);

                                await session.SendAsync(new NetworkMessage
                                {
                                    Type = "SERVER_JOIN_ROOM_RESPONSE",
                                    Success = true,
                                    RoomId = targetRoom.Id,
                                    Message = $"Successfully joined room '{targetRoom.Name}'."
                                });

                                targetRoom.BroadcastPlayerList();
                                OnClientListChanged?.Invoke();

                                bool isGameActiveCopy;
                                int[][]? boardJagged = null;
                                lock (targetRoom.GameLock)
                                {
                                    isGameActiveCopy = targetRoom.IsGameActive;
                                    if (isGameActiveCopy) boardJagged = ConvertToJagged(targetRoom.Engine.PlayerBoard);
                                }

                                if (isGameActiveCopy && boardJagged != null)
                                {
                                    await session.SendAsync(new NetworkMessage
                                    {
                                        Type = "SERVER_START_GAME",
                                        Board = boardJagged
                                    });
                                }
                            }
                            else
                            {
                                await session.SendAsync(new NetworkMessage
                                {
                                    Type = "SERVER_JOIN_ROOM_RESPONSE",
                                    Success = false,
                                    Message = "Room not found."
                                });
                            }
                        }
                        break;

                    case "CLIENT_LEAVE_ROOM":
                        if (_players.TryGetValue(session.Id, out var leaver) && leaver.RoomId != null)
                        {
                            if (leaver.RoomId != "default" && _rooms.TryGetValue(leaver.RoomId, out var currentRoom))
                            {
                                Log($"Player '{leaver.Username}' leaving room '{currentRoom.Name}'.");
                                currentRoom.RemovePlayer(leaver.Id);
                                currentRoom.BroadcastPlayerList();

                                if (_rooms.TryGetValue("default", out var lobbyRoom))
                                {
                                    lobbyRoom.AddPlayer(leaver);
                                    lobbyRoom.BroadcastPlayerList();
                                }

                                await session.SendAsync(new NetworkMessage
                                {
                                    Type = "SERVER_LEAVE_ROOM_RESPONSE",
                                    Success = true,
                                    Message = "Returned to default lobby."
                                });
                                OnClientListChanged?.Invoke();
                            }
                        }
                        break;

                    case "CLIENT_GET_ROOMS":
                        var roomsData = _rooms.Values.Select(r => new RoomData
                        {
                            Id = r.Id,
                            Name = r.Name,
                            PlayerCount = r.Players.Count,
                            IsGameActive = r.IsGameActive
                        }).ToList();

                        await session.SendAsync(new NetworkMessage
                        {
                            Type = "SERVER_ROOM_LIST",
                            Rooms = roomsData
                        });
                        break;
                }
            }
            catch (Exception ex)
            {
                Log($"Error processing message from {session.Username}: {ex.Message}");
            }
        }

        public void StartGame(int cellsToRemove)
        {
            if (!IsRunning) return;

            if (_rooms.TryGetValue("default", out var defaultRoom))
            {
                Log($"Starting a new game in lobby room. Difficulty: removing {cellsToRemove} cells.");
                defaultRoom.StartGame(cellsToRemove);
                OnGameStateChanged?.Invoke();
            }
        }

        private void Broadcast(object obj)
        {
            foreach (var player in _players.Values)
            {
                _ = player.Session.SendAsync(obj);
            }
        }

        private void Log(string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            OnLog?.Invoke($"[{time}] {message}");
        }

        private int[][] ConvertToJagged(int[,] array2D)
        {
            int[][] jagged = new int[9][];
            for (int r = 0; r < 9; r++)
            {
                jagged[r] = new int[9];
                for (int c = 0; c < 9; c++)
                {
                    jagged[r][c] = array2D[r, c];
                }
            }
            return jagged;
        }
    }

    // --- Các Models dữ liệu mạng được nhóm xuống dưới cùng ---
    public class NetworkMessage
    {
        public string Type { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? PlayerId { get; set; }
        public bool? Success { get; set; }
        public string? Message { get; set; }
        public bool? IsReady { get; set; }
        public int[][]? Board { get; set; }
        public int? Row { get; set; }
        public int? Col { get; set; }
        public int? Value { get; set; }
        public int? Score { get; set; }
        public bool? Correct { get; set; }
        public string? ChatText { get; set; }
        public List<PlayerData>? Players { get; set; }
        public string? RoomId { get; set; }
        public string? RoomName { get; set; }
        public List<RoomData>? Rooms { get; set; }
    }

    public class PlayerData
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int Score { get; set; }
        public bool IsReady { get; set; }
    }

    public class RoomData
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int PlayerCount { get; set; }
        public bool IsGameActive { get; set; }
    }
}