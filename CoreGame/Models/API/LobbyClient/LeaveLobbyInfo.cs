using System;

namespace CoreGame.Models.API.LobbyClient {
    [Serializable]
    public class LeaveLobbyInfo {
        public string LeavePlayerID;
        public string LobbyID;
    }
}