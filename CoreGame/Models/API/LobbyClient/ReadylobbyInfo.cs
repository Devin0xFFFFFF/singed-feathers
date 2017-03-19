using System;

namespace CoreGame.Models.API.LobbyClient {
    [Serializable]
    public class ReadyLobbyInfo {
        public string ReadyPlayerID;
        public bool IsReady;
        public string LobbyID;
    }
}