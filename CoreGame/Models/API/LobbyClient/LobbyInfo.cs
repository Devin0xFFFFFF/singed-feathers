using System;
using System.Collections.Generic;

namespace CoreGame.Models.API.LobbyClient {
    [Serializable]
    public class LobbyInfo {
        public string LobbyID;
        public int CreationTime;
        public string GameID;
        public bool IsPublic;
        public string LobbyName;
        public string MapID;
        public int NumPlayers;
        public List<Player> Players;
    }
}