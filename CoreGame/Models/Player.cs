using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CoreGame.Models {
    [Serializable]
    public class Player {
        public readonly string PlayerID;
        public string PlayerName;
        public PlayerSideSelection PlayerSideSelection { get; set; }
        public PlayerState PlayerState;
        private static JsonSerializerSettings _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        public Player(string playerName = "AnonPlayer", PlayerSideSelection playerSideSelection = PlayerSideSelection.SavePigeons, PlayerState playerState = PlayerState.LobbyUnready) {
            PlayerID = GeneratePlayerID();
            PlayerName = playerName;
            PlayerSideSelection = playerSideSelection;
            PlayerState = playerState;
        }

        public static IList<Player> Deserialize(string serializedPlayers) {
            return JsonConvert.DeserializeObject<List<Player>>(serializedPlayers, _settings);
        }

        private static string GeneratePlayerID() { return "Player" + Guid.NewGuid().ToString(); }
    }
}