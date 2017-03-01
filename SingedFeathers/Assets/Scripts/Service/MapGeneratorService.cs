using System;
using System.IO;
using UnityEngine;
using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using Newtonsoft.Json;

namespace Assets.Scripts.Service {
    public class MapGeneratorService : IMapGeneratorService {
        private const int MINIMUM_MAP_ID = 1;

        public Map GenerateMap(int id = 1) {
            if (id < MINIMUM_MAP_ID) {
                return null;
            }

			try {
				TextAsset targetFile = Resources.Load<TextAsset>(string.Format("Map{0}", id));

				string json = targetFile.text;

	            Map map = JsonConvert.DeserializeObject<Map>(json);
	            InitializeTileMapFromRaw(map);

	            return map;
            } catch (Exception e) {
                Console.Write(string.Format("Error loading Map{0}: {1}", id,  e));

                return null;
            }
        }

        private static void InitializeTileMapFromRaw(Map map) {
            map.TileMap = new ITileController[map.Width, map.Height];
            for (int x = 0; x < map.Width; x++) {
                for (int y = 0; y < map.Height; y++) {
                    map.TileMap[x, y] = new TileController(map.RawMap[x, y]);
                }
            }
        }
    }
}