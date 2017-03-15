﻿using System.Collections.Generic;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Service;
using CoreGame.Utility;

namespace CoreGame.Controllers {
    public class MapController : IMapController {
        public const int HEAT = 100;
        public int Width { get { return _map.Width; } }
        public int Height { get { return _map.Height; } }
        private readonly IMapGeneratorService _mapGenerator;
        private Map _map;

        public MapController(IMapGeneratorService mapGenerator = null) { _mapGenerator = mapGenerator ?? new MapGeneratorService(); }

        public bool GenerateMap(string serializedMap) {
            _map = _mapGenerator.GenerateMap(serializedMap);
            if (_map == null) {
                return false;
            }
            MapLocationValidator.InitializeValues(_map);
            CommandValidator.InitializeValues(_map);
            LinkNeighbouringTiles();
            InitializeFires();

            return true;
        }

        public void ApplyHeat(int x, int y) {
            if (MapLocationValidator.CoordinatesAreValid(x, y)) {
                _map.TileMap[x, y].ApplyHeat(HEAT);
            }
        }
            
        public void EndTurn() {
            _map.TurnResolver.ResolveTurn(_map.TurnController.GetAndResetMoves(), _map.TileMap);
            SpreadFires();
            MovePigeons();
        }
		
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

        public ITurnController GetTurnController() { return _map.TurnController; }

        public int GetTurnsLeft() { return _map.TurnController.GetTurnsLeft(); }

        public void UndoAllActions() { _map.TurnController.UndoAllActions(); }

        public void Fire() { _map.TurnController.SetMoveType(MoveType.Fire); }

        public void Water() { _map.TurnController.SetMoveType(MoveType.Water); }

        public void Cancel() { _map.TurnController.SetMoveType(MoveType.Remove); }

        public void SpreadFires() {
            // Update tiles
            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    ITileController tile = _map.TileMap[x, y];
                    tile.SpreadFire();
                }
            }

            for (int x = 0; x < Width; x++) {
                for (int y = 0; y < Height; y++) {
                    ITileController tile = _map.TileMap[x, y];
                    tile.UpKeep();
                }
            }
        }

        public void MovePigeons() {
            foreach (IPigeonController pigeon in _map.Pigeons) {
                pigeon.React();
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
            Position position = _map.InitialFirePosition;
            ApplyHeat(position.X, position.Y);
            SpreadFires();
        }
    }
}