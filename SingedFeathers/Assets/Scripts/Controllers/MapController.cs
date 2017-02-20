using System.Collections.Generic;
using Assets.Scripts.Model;

namespace Assets.Scripts.Controllers {
    public class MapController : IMapController {
        public int Width { get; private set; }
        public int Height { get; private set; }
        private ITileController[,] _map;
        
        private readonly int[,] _testMapRaw = {
            {2, 3, 3, 3, 3},
            {2, 3, 2, 3, 3},
            {3, 3, 3, 3, 3},
            {2, 3, 3, 2, 3},
            {2, 3, 3, 3, 2}
        };

        public void GenerateMap() {
            Width = _testMapRaw.GetLength(0);
            Height = _testMapRaw.GetLength(1);
            _map = new ITileController[Width, Height];

            InitializeTiles();
            LinkNeighbouringTiles();
        }

        public void ApplyHeat(int x, int y, int heat) {
            if (x >= 0 && y >= 0 && x < Width && y < Height) {
                _map[x, y].ApplyHeat(heat);
            }
        }

        public TileType GetTileType(int x, int y) {
            return _map[x, y].GetTileType();
        }

        public ITileController GetController(int x, int y) {
            return _map[x, y];
        }

        public void StartTurn() {
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    _map[x, y].StartTurn();
                }
            }
        }

        public IDictionary<NewStatus, IList<Position>> SpreadFires() {
            bool alreadyLit = false;
            Position pos = null;
            IDictionary<NewStatus, IList<Position>> modifiedTiles = InitializeModifiedTilesDict();
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    ITileController tile = _map[x, y];
                    pos = new Position { X = x, Y = y };
                    alreadyLit = tile.IsLit();
                    if (!tile.IsBurntOut()) {
                        tile.SpreadFire();
                        if (tile.IsBurntOut()) {
                            modifiedTiles[NewStatus.BurntOut].Add(pos);
                        }
                        if (!alreadyLit && tile.IsLit()) {
                            modifiedTiles[NewStatus.OnFire].Add(pos);
                        }
                    }
                }
            }
            return modifiedTiles;
        }

        private IDictionary<NewStatus, IList<Position>> InitializeModifiedTilesDict() {
            IDictionary<NewStatus, IList<Position>> dict = new Dictionary<NewStatus, IList<Position>>();

            IList<Position> newlyBurntTilePositions = new List<Position>();
            IList<Position> newlyLitTilePositions = new List<Position>();

            dict.Add(NewStatus.BurntOut, newlyBurntTilePositions);
            dict.Add(NewStatus.OnFire, newlyLitTilePositions);

            return dict;
        }

        private void InitializeTiles() {
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    _map[x, y] = new TileController((TileType) _testMapRaw[x, y]);
                }
            }
        }

        private void LinkNeighbouringTiles() {
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    LinkTileToNeighbours(x, y);
                }
            }
        }

        private void LinkTileToNeighbours(int x, int y) {
            if (x > 0) {
                _map[x, y].AddNeighbouringTile(_map[x - 1, y]);
            }
            if (y > 0) {
                _map[x, y].AddNeighbouringTile(_map[x, y - 1]);
            }
            if (x < Width - 1) {
                _map[x, y].AddNeighbouringTile(_map[x + 1, y]);
            }
            if (y < Height - 1) {
                _map[x, y].AddNeighbouringTile(_map[x, y + 1]);
            }
        }
    }
}
