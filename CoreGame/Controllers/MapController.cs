using System.Collections.Generic;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Service;
using CoreGame.Utility;

namespace CoreGame.Controllers {
    public class MapController : IMapController {
        public const int HEAT = 100;
        public const string WIN = "You won!";
        public const string LOSE = "You lost!";
        public const string NO_PIGEONS_SURVIVED = "No pigeons survived!";
        public const string A_PIGEON_SURVIVED = "A pigeon survived!";
        public int Width { get { return _map.Width; } }
        public int Height { get { return _map.Height; } }
        private readonly IMapGeneratorService _mapGenerator;
        private Map _map;
        private Player _player;

        public MapController(IMapGeneratorService mapGenerator = null) { 
            _mapGenerator = mapGenerator ?? new MapGeneratorService();
            _player = new Player("Player1");
        }

        public bool GenerateMap(string serializedMap) {
            _map = _mapGenerator.GenerateMap(serializedMap);
            if (_map == null) {
                return false;
            }
            MapLocationValidator.InitializeValues(_map);
            LinkNeighbouringTiles();
            InitializeFires();

            return true;
        }

        public void SetPlayerSideSelection(PlayerSideSelection playerSideSelection){ _player.PlayerSideSelection = playerSideSelection; }

        public PlayerSideSelection GetPlayerSideSelection() { return _player.PlayerSideSelection; }

        public string GetGameOverPlayerStatus() {
            PlayerSideSelection playerSideSelection = GetPlayerSideSelection();
            string winOrLose = "";
            string reason;

            if (AreAllPigeonsDead()) {
                reason = NO_PIGEONS_SURVIVED;
                if (playerSideSelection == PlayerSideSelection.BurnPigeons) {
                    winOrLose = WIN;
                }
                if (playerSideSelection == PlayerSideSelection.SavePigeons) {
                    winOrLose = LOSE;
                }
            } else {
                reason = A_PIGEON_SURVIVED;
                if (playerSideSelection == PlayerSideSelection.BurnPigeons) {
                    winOrLose = LOSE;
                }
                if (playerSideSelection == PlayerSideSelection.SavePigeons) {
                    winOrLose = WIN;
                }
            }
            return string.Format("{0} {1}", winOrLose, reason);
        }

        public bool IsMapBurntOut() {
            bool isBurntOut = true;
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    ITileController tile = _map.TileMap[x, y];
                    if (tile.IsFlammable() && !tile.IsBurntOut()) {
                        isBurntOut = false;
                        break;
                    }
                }
            }
            return isBurntOut;
        }

        public bool AreAllPigeonsDead() {
            bool areAllPigeonsDead = true;
            foreach (IPigeonController pigeon in _map.Pigeons) {
                if (!pigeon.IsDead()) {
                    areAllPigeonsDead = false;
                    break;
                }
            }
            return areAllPigeonsDead;
        }

        public void ApplyHeat(int x, int y) {
            if (MapLocationValidator.CoordinatesAreValid(x, y)) {
                _map.TileMap[x, y].ApplyHeat(HEAT);
            }
        }
            
        public void EndTurn() { _map.TurnResolver.ResolveTurn(_map.TurnController.GetAndResetMove(), _map, _player); }

        public void ApplyDelta(IList<Delta> deltaList) {
            TurnResolveUtility.ApplyDelta(deltaList, _map);
        }

        public void ApplyTurn(IList<Delta> deltaList) { }

        public bool IsTurnResolved() { return _map.TurnResolver.IsTurnResolved(); }

        public TileType GetTileType(int x, int y) {
            if (MapLocationValidator.CoordinatesAreValid(x, y)) {
                return _map.TileMap[x, y].GetTileType();
            }
            return TileType.Error;
        }

        public ITileController GetTileController(int x, int y) {
            if (MapLocationValidator.CoordinatesAreValid(x, y)) {
                return _map.TileMap[x, y];
            }
            return null;
        }
        
        public IList<IPigeonController> GetPigeonControllers() { return _map.Pigeons; }

        public ITurnResolver GetTurnResolver() { return _map.TurnResolver; }

        public void SetTurnResolver(ITurnResolver turnResolver) { _map.TurnResolver = turnResolver; }

        public ITurnController GetTurnController() { return _map.TurnController; }

        public int GetTurnsLeft() { return _map.TurnController.GetTurnsLeft(); }

        public void UndoAction() { _map.TurnController.UndoAction(); }

        public void Fire() { _map.TurnController.SetMoveType(MoveType.Fire); }

        public void Water() { _map.TurnController.SetMoveType(MoveType.Water); }

        public bool ShouldPoll() { return _map.TurnResolver.ShouldPoll(); }

        public bool ValidateDelta(Delta delta) { return CommandValidator.ValidateDelta(delta, _map.TileMap); }

        public void Poll() { _map.TurnResolver.Poll(_map, _player); }

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

        private void InitializeFires() {
            foreach (Position position in _map.InitialFirePositions) {
                ApplyHeat(position.X, position.Y);
            }
            TurnResolveUtility.SpreadFires(_map);
        }
    }
}