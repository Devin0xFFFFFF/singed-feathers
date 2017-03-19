using System;

namespace CoreGame.Models.API.LobbyClient {
    [Serializable]
    public class JoinLobbyInfo {
        public Player JoinPlayer;
        public string LobbyID;
    }
}