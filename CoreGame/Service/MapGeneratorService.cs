using System;
using System.Collections.Generic;
using CoreGame.Controllers;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using Newtonsoft.Json;

namespace CoreGame.Service {
    public class MapGeneratorService : IMapGeneratorService {
        public Map GenerateMap(string serializedMap) {
            if (serializedMap == null) {
                return null;
            }

            try {
                Map map = JsonConvert.DeserializeObject<Map>(serializedMap);

                InitializeTileMapFromRaw(map);
                InitializePigeons(map);
                InitializeStateManagers(map);

                return map;
            } catch (Exception) {
                return null;
            }
        }

        public Map GenerateDefaultMap(int width, int height) {
            if (width <= 0 || height <= 0) {
                return null;
            }

            Map newMap = new Map ();
            newMap.Height = width;
            newMap.Width = height;
            TileType[,] rawMap = new TileType[width, height];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    rawMap [x, y] = TileType.Grass;
                }
            }
            newMap.RawMap = rawMap;
            newMap.InitialFirePositions = new List<Position>();
            newMap.InitialPigeonPositions = new List<Position>();
            newMap.Pigeons = new List<IPigeonController>();
            InitializeTileMapFromRaw(newMap);
            return newMap;
        }
        
        private void InitializeTileMapFromRaw(Map map) {
            map.TileMap = new ITileController[map.Width, map.Height];
            for (int x = 0; x < map.Width; x++) {
                for (int y = 0; y < map.Height; y++) {
                    map.TileMap[y, x] = new TileController(map.RawMap[y, x], y, x);
                }
            }
        }

        private void InitializePigeons(Map map) {
            map.Pigeons = new List<IPigeonController>();
            foreach (Position pigeonPosition in map.InitialPigeonPositions) {
                ITileController tile = map.TileMap[pigeonPosition.X, pigeonPosition.Y];
                map.Pigeons.Add(new PigeonController(tile));
                tile.MarkOccupied();
            }
        }

        private void InitializeStateManagers(Map map) {
            map.TurnController = new TurnController(map.TurnsLeft);
            map.TurnResolver = new LocalTurnResolver();
        }
    }
}