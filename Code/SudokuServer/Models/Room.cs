using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SudokuServer.Game;
using SudokuServer.Network;

namespace SudokuServer.Models
{
    public class Room
    {
        public string Id { get; }
        public string Name { get; }
        public ConcurrentDictionary<string, Player> Players { get; } = new ConcurrentDictionary<string, Player>();
        public SudokuEngine Engine { get; } = new SudokuEngine();
        public bool IsGameActive { get; set; } = false;
        public object GameLock { get; } = new object();
        public string? CreatorId { get; set; }
        public DateTime GameStartTime { get; set; }

        public Room(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public bool AddPlayer(Player player)
        {
            player.RoomId = Id;
            player.Score = 0;
            player.PenaltySeconds = 0;
            player.IsReady = false;
            return Players.TryAdd(player.Id, player);
        }

        public bool RemovePlayer(string playerId)
        {
            if (Players.TryRemove(playerId, out var player))
            {
                player.RoomId = null;
                return true;
            }
            return false;
        }

        public void Broadcast(object message)
        {
            foreach (var player in Players.Values)
            {
                _ = player.Session.SendAsync(message);
            }
        }

        public void BroadcastPlayerList()
        {
            var list = Players.Values.Select(p => new PlayerData
            {
                Id = p.Id,
                Username = p.Username,
                Score = p.Score,
                PenaltySeconds = p.PenaltySeconds,
                IsReady = p.IsReady
            }).ToList();

            Broadcast(new NetworkMessage
            {
                Type = "SERVER_PLAYER_LIST",
                Players = list
            });
        }

        public void StartGame(int cellsToRemove)
        {
            // Run countdown + game start on background thread to not block
            Task.Run(async () =>
            {
                // Countdown 3-2-1
                for (int i = 3; i >= 1; i--)
                {
                    Broadcast(new NetworkMessage
                    {
                        Type = "SERVER_COUNTDOWN",
                        Message = $"⏳ Game bắt đầu sau {i} giây..."
                    });
                    await Task.Delay(1000);
                }

                int[][] boardJagged;
                lock (GameLock)
                {
                    Engine.GenerateNewGame(cellsToRemove);
                    IsGameActive = true;
                    GameStartTime = DateTime.UtcNow;

                    // Reset player penalties
                    foreach (var player in Players.Values)
                    {
                        player.Score = 0;
                        player.PenaltySeconds = 0;
                    }

                    boardJagged = ConvertToJagged(Engine.PlayerBoard);
                }

                Broadcast(new NetworkMessage
                {
                    Type = "SERVER_START_GAME",
                    Board = boardJagged
                });

                BroadcastPlayerList();
            });
        }

        public void EndGame()
        {
            double elapsedSeconds;
            lock (GameLock)
            {
                IsGameActive = false;
                elapsedSeconds = (DateTime.UtcNow - GameStartTime).TotalSeconds;
            }

            // Rank by lowest total time (elapsed + penalty)
            var sortedPlayers = Players.Values
                .OrderBy(p => elapsedSeconds + p.PenaltySeconds)
                .ToList();
            var winner = sortedPlayers.FirstOrDefault();
            int elapsedMin = (int)elapsedSeconds / 60;
            int elapsedSec = (int)elapsedSeconds % 60;

            string resultLines = "";
            foreach (var p in sortedPlayers)
            {
                double total = elapsedSeconds + p.PenaltySeconds;
                int tMin = (int)total / 60;
                int tSec = (int)total % 60;
                resultLines += $"\n  {p.Username}: {tMin:D2}:{tSec:D2} (phạt +{p.PenaltySeconds}s)";
            }

            string winnerName = winner?.Username ?? "None";

            Broadcast(new NetworkMessage
            {
                Type = "SERVER_GAME_OVER",
                Message = $"Hoàn thành! Thời gian: {elapsedMin:D2}:{elapsedSec:D2}\nNgười thắng: {winnerName}{resultLines}",
                Players = sortedPlayers.Select(p => new PlayerData
                {
                    Id = p.Id,
                    Username = p.Username,
                    Score = p.Score,
                    PenaltySeconds = p.PenaltySeconds,
                    IsReady = p.IsReady
                }).ToList()
            });
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
}
