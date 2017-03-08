using System.Collections.Generic;
using Assets.Scripts.Models;
using Assets.Scripts.Service;

namespace Assets.Scripts.Controllers {
    public class MapController : IMapController {
        public const int HEAT = 100;
        public int Width { get { return _map.Width; } }
        public int Height { get { return _map.Height; } }
        private readonly IMapGeneratorService _mapGenerator;
        private Map _map;

        public MapController(IMapGeneratorService mapGenerator = null) { _mapGenerator = mapGenerator ?? new MapGeneratorService(); }

        public void GenerateMap() {
            _map = _mapGenerator.GenerateMap();
            LinkNeighbouringTiles();
        }

        public void ApplyHeat(int x, int y) {
            if (CoordinatesAreValid(x, y)) {
                _map.TileMap[x, y].ApplyHeat(HEAT);
            }
        }

        public void EndTurn() { _map.TurnResolver.ResolveTurn(_map.TurnController.GetAndResetMoves(), this); }
		
        public TileType GetTileType(int x, int y) {
            if (CoordinatesAreValid(x, y)) {
                return _map.TileMap[x, y].GetTileType();
            }
            return TileType.Error;
        }

        public ITileController GetTileController(int x, int y) {
            if (CoordinatesAreValid(x, y)) {
                return _map.TileMap[x, y];
            }
            return null;
        }

        public ITileController GetTileController(Position position) { return GetTileController(position.X, position.Y); }

        public IList<IPigeonController> GetPigeonControllers() { return _map.Pigeons; }

        public Position GetInitialFirePosition() { return _map.InitialFirePosition; }

        public ITurnResolver GetTurnResolver() { return _map.TurnResolver; }

        public ITurnController GetTurnController() { return _map.TurnController; }

        public int GetTurnsLeft() { return _map.TurnController.GetTurnsLeft(); }

        public void UndoAllActions() { _map.TurnController.UndoAllActions(); }

        public void Fire() { _map.TurnController.SetMoveType(MoveType.Fire); }

        public void Water() { _map.TurnController.SetMoveType(MoveType.Water); }

        public void Cancel() { _map.TurnController.SetMoveType(MoveType.Remove); }

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
                            modifiedTiles[NewStatus.BurntOut].Add(new Position(x, y));
                        }
                        if (tile.IsOnFire()) {
                            modifiedTiles[NewStatus.OnFire].Add(new Position(x, y));
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

        public void MovePigeons() {
            foreach (IPigeonController pigeon in _map.Pigeons) {
                if (!pigeon.IsDead()) {
                    pigeon.React();
                }
            }
        }

        private bool CoordinatesAreValid(int x, int y) { return x >= 0 && y >= 0 && x < Width && y < Height; }

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