using System.Collections.Generic;

namespace SudokuClient.Models
{
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
        public int PenaltySeconds { get; set; }
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
