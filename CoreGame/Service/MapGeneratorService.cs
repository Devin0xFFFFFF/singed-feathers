using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using Newtonsoft.Json;

namespace Assets.Scripts.Service {
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
            map.TurnController = new TurnController(map.TurnsLeft, map.MaxMovesPerTurn);
            map.TurnResolver = new LocalTurnResolver();
        }
    }
}