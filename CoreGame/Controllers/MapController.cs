using System.Collections.Generic;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Service;
using CoreGame.Utility;

namespace CoreGame.Controllers {
    public class MapController : IMapController {
        public const int HEAT = 100;
        public const int DEFAULT_WIDTH = 8;
        public const int DEFAULT_HEIGHT = 8;
        public const string WIN = "You won!";
        public const string LOSE = "You lost!";
        public const string NO_PIGEONS_SURVIVED = "No pigeons survived!";
        public const string A_PIGEON_SURVIVED = "A pigeon survived!";
        private readonly Player _player;
        private readonly IMapGeneratorService _mapGenerator;
        public int Width { get { return _map.Width; } }
        public int Height { get { return _map.Height; } }
        private Map _map;

        public MapController(IMapGeneratorService mapGenerator = null) { 
            _mapGenerator = mapGenerator ?? new MapGeneratorService();
            _player = new Player();
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

        public bool GenerateDefaultMap() {
            _map = _mapGenerator.GenerateDefaultMap(DEFAULT_WIDTH, DEFAULT_HEIGHT);
            if(_map == null) {
                return false;
            }
            MapLocationValidator.InitializeValues(_map);

            return true;
        }

        public string SerializeMap(int width, int height, IList<Position> pigeonPositions, IList<Position> firePositions, TileType[,] tileMap, int numTurns) {
            return _mapGenerator.SerializeMap(width, height, tileMap, firePositions, pigeonPositions, numTurns);
        }

        public string GetPlayerName() { return _player.PlayerName; }

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
            return $"{winOrLose} {reason}";
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

        public bool AreAllPigeonsDead() { return GetLivePigeonCount() == 0; }

        public int GetLivePigeonCount() {
            int livePigeons = 0;
            foreach (IPigeonController pigeon in _map.Pigeons) {
                if (!pigeon.IsDead()) {
                    livePigeons++;
                }
            }
            return livePigeons;
        }

        public void ApplyHeat(int x, int y) {
            if (MapLocationValidator.CoordinatesAreValid(x, y)) {
                _map.TileMap[x, y].ApplyHeat(HEAT);
            }
        }

        public void ReduceHeat(int x, int y) {
            if (MapLocationValidator.CoordinatesAreValid(x, y)) {
                _map.TileMap[x, y].ReduceHeat(HEAT);
            }
        }
            
        public void EndTurn() { _map.TurnResolver.ResolveTurn(_map.TurnController.GetAndResetMove(), _map); }

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

        public void UpdateTileController(TileType type, int x, int y) {
            if (MapLocationValidator.CoordinatesAreValid(x, y)) {
                _map.TileMap[x, y] = new TileController(type, x, y);
                _map.RawMap[x, y] = type;
            }
        }

        public PigeonController AddPigeonToMap(Position position) {
            ITileController tile = _map.TileMap [position.X, position.Y];
            PigeonController pigeonController = new PigeonController(tile);
            _map.Pigeons.Add(pigeonController);
            tile.MarkOccupied();
            return pigeonController;
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

        public void Poll() { _map.TurnResolver.Poll(_map); }

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