using System;
using Assets.Scripts.Controllers;

namespace Assets.Scripts.Models {
    [Serializable]
    public class Map {
        public Position InitialFirePosition;
        public ITileController[,] TileMap;
        public int[,] RawMap;
        public int Width;
        public int Height;
    }
}
