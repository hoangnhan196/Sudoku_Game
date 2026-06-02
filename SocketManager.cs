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
using SudokuServer.Game;

namespace SudokuServer.Network
{
    public class ClientSession
    {
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
            await _writeLock.WaitAsync();
            try
            {
                string json = JsonSerializer.Serialize(obj);
                await Writer.WriteLineAsync(json);
            }
            catch
            {
                // Connection likely lost
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public void Disconnect()
        {
            try
            {
                Cts.Cancel();
                Reader.Dispose();
                Writer.Dispose();
                Stream.Dispose();
                Client.Close();
                _writeLock.Dispose();
            }
            catch { }
        }
    }

    public class SocketManager
    {
        private TcpListener? _listener;
        private CancellationTokenSource? _cts;
        private readonly ConcurrentDictionary<string, ClientSession> _clients = new();
        private readonly SudokuEngine _engine = new();
        private bool _isGameActive = false;

        public event Action<string>? OnLog;
        public event Action? OnClientListChanged;
        public event Action? OnGameStateChanged;

        public bool IsRunning { get; private set; } = false;
        public bool IsGameActive => _isGameActive;
        public int ConnectedClientsCount => _clients.Count;

        public int[,] GetSolutionBoard() => _engine.SolutionBoard;
        public int[,] GetPlayerBoard() => _engine.PlayerBoard;

        public List<ClientSession> GetClientsList() => _clients.Values.ToList();

        public void Start(int port)
        {
            if (IsRunning) return;

            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Any, port);
            
            try
            {
                _listener.Start();
                IsRunning = true;
                Log($"Server started on port {port}. Listening for connections...");
                Task.Run(() => AcceptClientsAsync(_cts.Token));
            }
            catch (Exception ex)
            {
                Log($"Error starting server: {ex.Message}");
                Stop();
                throw;
            }
        }

        public void Stop()
        {
            if (!IsRunning) return;

            Log("Stopping server...");
            _isGameActive = false;

            _cts?.Cancel();
            _listener?.Stop();

            foreach (var client in _clients.Values)
            {
                client.Disconnect();
            }
            _clients.Clear();

            IsRunning = false;
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
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    if (!token.IsCancellationRequested)
                    {
                        Log($"Error accepting client: {ex.Message}");
                    }
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
                    if (line == null) break; // Client disconnected gracefully

                    await ProcessMessageAsync(session, line);
                }
            }
            catch (Exception ex)
            {
                Log($"Session error for {session.Username} ({ipAddress}): {ex.Message}");
            }
            finally
            {
                session.Disconnect();
                if (_clients.TryRemove(session.Id, out _))
                {
                    Log($"Player {session.Username} ({ipAddress}) disconnected.");
                    BroadcastPlayerList();
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

                switch (msg.Type)
                {
                    case "CLIENT_CONNECT":
                        session.Username = string.IsNullOrEmpty(msg.Username) ? "Guest" : msg.Username;
                        
                        _clients[session.Id] = session;
                        Log($"Player '{session.Username}' joined the lobby.");

                        // Send connect confirmation
                        await session.SendAsync(new NetworkMessage
                        {
                            Type = "SERVER_CONNECT_RESPONSE",
                            Success = true,
                            PlayerId = session.Id,
                            Message = "Successfully connected to server."
                        });

                        // Broadcast updated player list
                        BroadcastPlayerList();
                        OnClientListChanged?.Invoke();

                        // If game is active, send the current board to the new player
                        if (_isGameActive)
                        {
                            await session.SendAsync(new NetworkMessage
                            {
                                Type = "SERVER_START_GAME",
                                Board = ConvertToJagged(_engine.PlayerBoard)
                            });
                        }
                        break;

                    case "CLIENT_READY":
                        if (msg.IsReady.HasValue)
                        {
                            session.IsReady = msg.IsReady.Value;
                            Log($"Player '{session.Username}' is {(session.IsReady ? "READY" : "NOT READY")}");
                            BroadcastPlayerList();
                            OnClientListChanged?.Invoke();
                        }
                        break;

                    case "CLIENT_MOVE":
                        if (!_isGameActive)
                        {
                            Log($"Ignored move from '{session.Username}' because no active game.");
                            break;
                        }

                        if (msg.Row.HasValue && msg.Col.HasValue && msg.Value.HasValue)
                        {
                            int r = msg.Row.Value;
                            int c = msg.Col.Value;
                            int val = msg.Value.Value;

                            // Validate board constraints
                            if (r >= 0 && r < 9 && c >= 0 && c < 9)
                            {
                                // Check if cell is already solved in the player board
                                if (_engine.PlayerBoard[r, c] == _engine.SolutionBoard[r, c])
                                {
                                    // Already filled correctly
                                    break;
                                }

                                bool isCorrect = _engine.CheckMove(r, c, val);
                                if (isCorrect)
                                {
                                    _engine.ApplyMove(r, c, val);
                                    session.Score += 10;
                                    Log($"'{session.Username}' placed {val} at ({r}, {c}) - CORRECT (+10 pts)");
                                }
                                else
                                {
                                    session.Score = Math.Max(0, session.Score - 5); // Prevent negative score
                                    Log($"'{session.Username}' placed {val} at ({r}, {c}) - INCORRECT (-5 pts)");
                                }

                                // Broadcast move update
                                Broadcast(new NetworkMessage
                                {
                                    Type = "SERVER_MOVE_UPDATE",
                                    Row = r,
                                    Col = c,
                                    Value = val,
                                    PlayerId = session.Id,
                                    Username = session.Username,
                                    Score = session.Score,
                                    Correct = isCorrect
                                });

                                OnGameStateChanged?.Invoke();

                                // Check if game finished
                                if (_engine.IsGameFinished())
                                {
                                    EndGame();
                                }
                            }
                        }
                        break;

                    case "CLIENT_CHAT":
                        if (!string.IsNullOrEmpty(msg.ChatText))
                        {
                            Log($"[CHAT] {session.Username}: {msg.ChatText}");
                            Broadcast(new NetworkMessage
                            {
                                Type = "SERVER_CHAT",
                                Username = session.Username,
                                ChatText = msg.ChatText
                            });
                        }
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

            Log($"Starting a new game. Difficulty: removing {cellsToRemove} cells.");
            _engine.GenerateNewGame(cellsToRemove);
            _isGameActive = true;

            // Reset player scores
            foreach (var client in _clients.Values)
            {
                client.Score = 0;
            }

            Broadcast(new NetworkMessage
            {
                Type = "SERVER_START_GAME",
                Board = ConvertToJagged(_engine.PlayerBoard)
            });

            BroadcastPlayerList();
            OnGameStateChanged?.Invoke();
        }

        private void EndGame()
        {
            _isGameActive = false;
            Log("Sudoku solved! Game Over.");

            // Find winner (highest score)
            var sortedPlayers = _clients.Values.OrderByDescending(c => c.Score).ToList();
            string winnerName = sortedPlayers.FirstOrDefault()?.Username ?? "None";
            
            Log($"Winner: {winnerName} with {sortedPlayers.FirstOrDefault()?.Score ?? 0} points!");

            Broadcast(new NetworkMessage
            {
                Type = "SERVER_GAME_OVER",
                Message = $"Game completed! Winner: {winnerName}",
                Players = sortedPlayers.Select(p => new PlayerData
                {
                    Id = p.Id,
                    Username = p.Username,
                    Score = p.Score,
                    IsReady = p.IsReady
                }).ToList()
            });

            OnGameStateChanged?.Invoke();
        }

        private void BroadcastPlayerList()
        {
            var list = _clients.Values.Select(p => new PlayerData
            {
                Id = p.Id,
                Username = p.Username,
                Score = p.Score,
                IsReady = p.IsReady
            }).ToList();

            Broadcast(new NetworkMessage
            {
                Type = "SERVER_PLAYER_LIST",
                Players = list
            });
        }

        private void Broadcast(object obj)
        {
            foreach (var client in _clients.Values)
            {
                _ = client.SendAsync(obj);
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
    }

    public class PlayerData
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public int Score { get; set; }
        public bool IsReady { get; set; }
    }
}
