using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using SudokuServer.GameLogic;
using SudokuServer.Network;

namespace SudokuServer.Models
{
    public class Room
    {
        public string Id { get; }
        public string Name { get; }
        public ConcurrentDictionary<string, Player> Players { get; } = new ConcurrentDictionary<string, Player>();
        public SudokuCore Engine { get; } = new SudokuCore();
        public bool IsGameActive { get; set; } = false;
        public object GameLock { get; } = new object();
        public string? CreatorId { get; set; }

        public Room(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public bool AddPlayer(Player player)
        {
            player.RoomId = Id;
            player.Score = 0;
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
            int[][] boardJagged;
            lock (GameLock)
            {
                Engine.GenerateNewGame(cellsToRemove);
                IsGameActive = true;

                // Reset player scores
                foreach (var player in Players.Values)
                {
                    player.Score = 0;
                }

                boardJagged = ConvertToJagged(Engine.PlayerBoard);
            }

            Broadcast(new NetworkMessage
            {
                Type = "SERVER_START_GAME",
                Board = boardJagged
            });

            BroadcastPlayerList();
        }

        public void EndGame()
        {
            lock (GameLock)
            {
                IsGameActive = false;
            }

            // Find winner (highest score)
            var sortedPlayers = Players.Values.OrderByDescending(p => p.Score).ToList();
            string winnerName = sortedPlayers.FirstOrDefault()?.Username ?? "None";

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
