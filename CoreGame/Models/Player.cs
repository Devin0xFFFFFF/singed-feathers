using System;

namespace CoreGame.Models {
    [Serializable]
    public class Player {
        public readonly string PlayerID;
        public string PlayerName;
        public PlayerSideSelection PlayerSideSelection { get; set; }
        public bool IsReady;

        public Player(string playerName = "AnonPlayer", PlayerSideSelection playerSideSelection = PlayerSideSelection.SavePigeons, bool isReady = false) {
            PlayerID = GeneratePlayerID();
            PlayerName = playerName;
            PlayerSideSelection = playerSideSelection;
            IsReady = isReady;
        }

        private static string GeneratePlayerID() { return "Player" + Guid.NewGuid().ToString(); }
    }
}