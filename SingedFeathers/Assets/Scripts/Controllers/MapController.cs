using System.Collections.Generic;
using Assets.Scripts.Model;

namespace Assets.Scripts.Controllers {
    public class MapController : IMapController {
        public int Width { get; private set; }
        public int Height { get; private set; }
        private ITileController[,] _map;

        //TODO: remove this and load from file
        private readonly int[,] _testMapRaw = {
            {2, 3, 3, 3, 3},
            {2, 3, 2, 3, 3},
            {3, 3, 3, 3, 3},
            {2, 3, 3, 2, 3},
            {2, 3, 3, 3, 2}
        };

        public void GenerateMap() {
            //TODO: pull data from file etc...
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

        public void SartTurn() {
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    _map[x, y].StartTurn();
                }
            }
        }

        public IList<Position> SpreadFires() {
            IList<Position> newlyBurntTilePositions = new List<Position>();
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    ITileController tile = _map[x, y];
                    if (!tile.IsBurntOut()) {
                        tile.SpreadFire();
                        //ApplyHeatToNeighbours(x, y);
                        if (tile.IsBurntOut()) {
                            newlyBurntTilePositions.Add(new Position {X = x, Y = y});
                        }
                    }
                }
            }
            return newlyBurntTilePositions;
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
