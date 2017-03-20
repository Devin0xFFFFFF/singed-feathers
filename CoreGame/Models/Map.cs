using System;
using System.Collections.Generic;
using CoreGame.Controllers.Interfaces;

namespace CoreGame.Models {
    [Serializable]
    public class Map {
        public int TurnsLeft;
        public IList<Position> InitialFirePositions;
        public IList<Position> InitialPigeonPositions;
        public ITileController[,] TileMap;
        public IList<IPigeonController> Pigeons;
        public TileType[,] RawMap;
        public ITurnController TurnController;
        public ITurnResolver TurnResolver;
        public int Width;
        public int Height;
    }
}