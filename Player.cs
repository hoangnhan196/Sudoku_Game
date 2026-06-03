using System;
using SudokuServer.Network;

namespace SudokuServer.Models
{
    public class Player
    {
        public string Id { get; set; }
        public string Username
        {
            get => Session.Username;
            set => Session.Username = value;
        }
        public int Score
        {
            get => Session.Score;
            set => Session.Score = value;
        }
        public bool IsReady
        {
            get => Session.IsReady;
            set => Session.IsReady = value;
        }
        public string? RoomId { get; set; }
        public ClientSession Session { get; }

        public Player(string id, string username, ClientSession session)
        {
            Id = id;
            Session = session;
            Username = username;
            Score = 0;
            IsReady = false;
            RoomId = null;
        }
    }
}
