using System;

namespace CoreGame.Models.API.MapClient {
    [Serializable]
    public class CreateMapInfo {
        public string MapName;
        public string CreatorName;
        public string MapType;
        public string SerializedMapData;
    }
}