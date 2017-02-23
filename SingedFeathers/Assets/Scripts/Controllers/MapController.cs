using System.Collections.Generic;
using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers {
    public class MapController : IMapController {
        const int HEAT = 100;
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

        public void ApplyHeat(int x, int y) {
            if (x >= 0 && y >= 0 && x < Width && y < Height) {
                _map[x, y].ApplyHeat(HEAT);
            }
        }

        public TileType GetTileType(int x, int y) {
            return _map[x, y].GetTileType();
        }

        public ITileController GetController(int x, int y) {
            return _map[x, y];
        }

        public IDictionary<NewStatus, IList<Position>> SpreadFires() {

            IDictionary<NewStatus, IList<Position>> modifiedTiles = new Dictionary<NewStatus, IList<Position>>();
            modifiedTiles.Add(NewStatus.BurntOut, new List<Position>());
            modifiedTiles.Add(NewStatus.OnFire, new List<Position>());

            // Update tiles
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {

                    ITileController tile = _map[x, y];
                    tile.SpreadFire();

                    if (tile.StateHasChanged) {
                        if (tile.IsBurntOut()) {
                            modifiedTiles[NewStatus.BurntOut].Add(new Position {X = x, Y = y});
                        }
                        if (tile.IsOnFire()) {
                            modifiedTiles[NewStatus.OnFire].Add(new Position {X = x, Y = y});
                        }
                    }
                }
            }

            // Reset map
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    _map[x, y].StateHasChanged = false;
                }
            }

            return modifiedTiles;
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
            if (y < Height - 1) {
                _map[x, y].AddNeighbouringTile(_map[x, y + 1]);
            }
            if (x < Width - 1) {
                _map[x, y].AddNeighbouringTile(_map[x + 1, y]);
            }
        }
    }
}
