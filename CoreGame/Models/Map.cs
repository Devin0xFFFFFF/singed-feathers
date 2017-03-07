using System;
using System.Collections.Generic;
using Assets.Scripts.Controllers;

namespace Assets.Scripts.Models {
    [Serializable]
    public class Map {
        public int TurnsLeft;
        public int MaxMovesPerTurn;
        public Position InitialFirePosition;
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