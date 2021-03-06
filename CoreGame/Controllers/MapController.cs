﻿using System.Collections.Generic;
using System.Linq;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Service;
using CoreGame.Utility;

namespace CoreGame.Controllers {
    public class MapController : IMapController {
        public const int HEAT = 100;
        public const int DEFAULT_WIDTH = 8;
        public const int DEFAULT_HEIGHT = 8;
        public const int MAX_TURNS = 20;
        public const int MIN_TURNS = 5;
        public const string WIN = "You win!";
        public const string LOSE = "You lose!";
        public const string NO_PIGEONS_SURVIVED = "No pigeons survived!";
        public const string A_PIGEON_SURVIVED = "A pigeon survived!";
        public int Width { get { return _map.Width; } }
        public int Height { get { return _map.Height; } }
        private readonly Player _player;
        private readonly IMapGeneratorService _mapGenerator;
        private Map _map;

        public MapController(IMapGeneratorService mapGenerator = null, Player player = null) {
            _mapGenerator = mapGenerator ?? new MapGeneratorService();
            _player = player ?? new Player ("Player1");
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
            if (_map == null) {
                return false;
            }
            MapLocationValidator.InitializeValues(_map);

            return true;
        }

        public string SerializeMap() {
            _map.TileMap = null;
            return _mapGenerator.SerializeMap(_map);
        }

        public void SetPlayerSideSelection(PlayerSideSelection playerSideSelection){ _player.PlayerSideSelection = playerSideSelection; }

        public PlayerSideSelection GetPlayerSideSelection() { return _player.PlayerSideSelection; }

        public bool IsGameOver() { return !_map.TurnController.HasTurnsLeft() || IsMapBurntOut() || AreAllPigeonsDead(); }

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
            
        public void EndTurn() { _map.TurnResolver.ResolveTurn(_map.TurnController.GetAndResetMove(), _map, _player); }

        public void ApplyDelta(IList<Delta> deltaList) { TurnResolveUtility.ApplyDelta(deltaList, _map); }

        public void ApplyTurn(IList<Delta> deltaList) {}

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

        public bool UpdateTileController(TileType type, int x, int y) {
            if (MapLocationValidator.CoordinatesAreValid(x, y)) {
                _map.TileMap[x, y] = new TileController(type, x, y);
                _map.RawMap[x, y] = type;
                return true;
            }
            return false;
        }

        public bool AddInitialPigeonPosition(Position position) {
            if (MapLocationValidator.PositionIsValid(position)) {
                return AddPosition(_map.InitialPigeonPositions, position);
            }
            return false;
        }

        public bool RemoveInitialPigeonPosition(Position position) {
            if (MapLocationValidator.PositionIsValid(position)) {
                return RemovePosition(_map.InitialPigeonPositions, position);
            }
            return false;
        }

        public bool AddInitialFirePosition(Position position) {
            if (MapLocationValidator.PositionIsValid(position) && CanSetFire(position)) {
                ApplyHeat(position.X, position.Y);
                return AddPosition(_map.InitialFirePositions, position);
            }
            return false;
        }

        public bool RemoveInitialFirePosition(Position position) {
            if (MapLocationValidator.PositionIsValid(position)) {
                ReduceHeat(position.X, position.Y);
                return RemovePosition(_map.InitialFirePositions, position);
            }
            return false;
        }

        public IList<IPigeonController> GetPigeonControllers() { return _map.Pigeons; }

        public ITurnResolver GetTurnResolver() { return _map.TurnResolver; }

        public void SetTurnResolver(ITurnResolver turnResolver) { _map.TurnResolver = turnResolver; }

        public ITurnController GetTurnController() { return _map.TurnController; }

        public int GetTurnsLeft() { return _map.TurnController.GetTurnsLeft(); }

        public bool UpdateNumberOfTurns(int numTurns) {
            if (numTurns >= MIN_TURNS && numTurns <= MAX_TURNS) {
                _map.TurnsLeft = numTurns;
                return true;
            }
            return false;
        }

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

        private bool RemovePosition(IList<Position> positions, Position position) {
            Position existing = positions.FirstOrDefault(p => p.Equals(position));
            if (existing != null) {
                positions.Remove(existing);
                return true;
            }
            return false;
        }

        private bool AddPosition(IList<Position> positions, Position position) {
            if (!positions.Any(p => p.Equals(position))) {
                positions.Add(position);
                return true;
            }
            return false;
        }

        private bool CanSetFire(Position position) {
            ITileController tile = GetTileController(position.X, position.Y);
            return !tile.IsOnFire() && tile.IsFlammable();
        }
    }
}