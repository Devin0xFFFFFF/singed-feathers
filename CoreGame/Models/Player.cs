using System;

namespace CoreGame.Models {
    [Serializable]
    public class Player {
        public readonly string PlayerID;
        public string PlayerName;
        public PlayerSideSelection PlayerSideSelection { get; set; }
        public PlayerState PlayerState;
        public Delta Delta;

        public Player(string playerId, string playerName = "AnonPlayer", PlayerSideSelection playerSideSelection = PlayerSideSelection.SavePigeons, PlayerState playerState = PlayerState.LobbyUnready, Delta delta = null) {
            PlayerID = playerId;
            PlayerName = playerName;
            PlayerSideSelection = playerSideSelection;
            PlayerState = playerState;
            Delta = delta;
        }


        private static string GeneratePlayerID() { return "Player" + Guid.NewGuid().ToString(); }
    }
}