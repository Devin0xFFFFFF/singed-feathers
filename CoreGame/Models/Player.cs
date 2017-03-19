using System;

namespace CoreGame.Models {
    [Serializable]
    public class Player {
        public readonly string PlayerID;
        public string PlayerName;
        public string Role;
        public bool IsReady;

        public Player(string playerName = "AnonPlayer", string role = null, bool isReady = false) {
            PlayerID = GeneratePlayerID();
            PlayerName = playerName;
            Role = role;
            IsReady = isReady;
        }

        private static string GeneratePlayerID() { return "Player" + Guid.NewGuid().ToString(); }
    }
}