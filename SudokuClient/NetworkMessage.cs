using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuClient
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
        public string? RoomId { get; set; }
        public string? RoomName { get; set; }
    }
}
