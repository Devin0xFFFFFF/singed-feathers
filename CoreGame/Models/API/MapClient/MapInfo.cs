using System;

namespace CoreGame.Models.API.MapClient {
    [Serializable]
    public class MapInfo {
        public string MapID;
        public string MapName;
        public string CreatorName;
        public string MapType;
        public int CreationTime;
    }
}