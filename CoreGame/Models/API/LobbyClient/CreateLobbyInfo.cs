using System;

namespace CoreGame.Models.API.LobbyClient {
    [Serializable]
    public class CreateLobbyInfo {
        public Player HostPlayer;
        public string LobbyName;
        public string MapID;
        public int NumPlayers;
        public bool IsPublic;
    }
}