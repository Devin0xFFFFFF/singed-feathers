using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using Newtonsoft.Json;

namespace Assets.Scripts.Service {
    public class MapGeneratorService : IMapGeneratorService {
        private const int MINIMUM_MAP_ID = 1;
        private const int DEFAULT_MAX_TURNS = 8;

        public Map GenerateMap(int id = 1) {
            if (id < MINIMUM_MAP_ID) {
                return null;
            }

			try {
				TextAsset targetFile = Resources.Load<TextAsset>(string.Format("Map{0}", id));

				string json = targetFile.text;

	            Map map = JsonConvert.DeserializeObject<Map>(json);
	            InitializeTileMapFromRaw(map);
                InitializePigeons(map);
                InitializeStateManagers(map);

	            return map;
            } catch (Exception e) {
                Console.Write(string.Format("Error loading Map{0}: {1}", id,  e));
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
            map.TurnController = new TurnController(DEFAULT_MAX_TURNS);
            map.TurnResolver = new LocalTurnResolver();
        }
    }
}