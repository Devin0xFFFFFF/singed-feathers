using System.Collections.Generic;
using Assets.Scripts.Models;
using Assets.Scripts.Service;

namespace Assets.Scripts.Controllers {
    public class MapController : IMapController {
        const int HEAT = 100;
        public int Width { get { return _map.Width; }}
        public int Height { get { return _map.Height; }}
        private readonly IMapGeneratorService _mapGenerator;
        private Map _map;

        public MapController(IMapGeneratorService mapGenerator = null) {
            _mapGenerator = mapGenerator ?? new MapGeneratorService();
        }

        public void GenerateMap() {
            _map = _mapGenerator.GenerateMap(1);
            LinkNeighbouringTiles();
        }

        public void ApplyHeat(int x, int y) {
            if (x >= 0 && y >= 0 && x < Width && y < Height) {
                _map.TileMap[x, y].ApplyHeat(HEAT);
            }
        }

        public TileType GetTileType(int x, int y) {
            return _map.TileMap[x, y].GetTileType();
        }

        public ITileController GetController(int x, int y) {
            return _map.TileMap[x, y];
        }

        public IDictionary<NewStatus, IList<Position>> SpreadFires() {

            IDictionary<NewStatus, IList<Position>> modifiedTiles = new Dictionary<NewStatus, IList<Position>>();
            modifiedTiles.Add(NewStatus.BurntOut, new List<Position>());
            modifiedTiles.Add(NewStatus.OnFire, new List<Position>());

            // Update tiles
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {

                    ITileController tile = _map.TileMap[x, y];
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
                    _map.TileMap[x, y].StateHasChanged = false;
                }
            }

            return modifiedTiles;
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
                _map.TileMap[x, y].AddNeighbouringTile(_map.TileMap[x - 1, y]);
            }
            if (y > 0) {
                _map.TileMap[x, y].AddNeighbouringTile(_map.TileMap[x, y - 1]);
            }
            if (y < Height - 1) {
                _map.TileMap[x, y].AddNeighbouringTile(_map.TileMap[x, y + 1]);
            }
            if (x < Width - 1) {
                _map.TileMap[x, y].AddNeighbouringTile(_map.TileMap[x + 1, y]);
            }
        }
    }
}
