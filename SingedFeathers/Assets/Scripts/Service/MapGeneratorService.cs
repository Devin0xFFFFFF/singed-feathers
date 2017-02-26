using System;
using System.IO;
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
            string path;
            if (!Scripts.EnvironmentVariables.IsTestingWithTravis()) {
                path = string.Format("../SingedFeathers/Assets/Resources/Map{0}.json", id);
            } else {
                path = string.Format("../../../../SingedFeathers/Assets/Resources/Map{0}.json", id);
            }

            try {
                using (StreamReader r = new StreamReader(File.OpenRead(path))) {
                    Map map;

                    string json = r.ReadToEnd();
                    map = JsonConvert.DeserializeObject<Map>(json);
                    InitializeTileMapFromRaw(map);

                    return map;
                }
            } catch (Exception e) {
                Console.Write(string.Format("Error loading map file with path {0}: {1}", path,  e));

                return null;
            }
        }

        private void InitializeTileMapFromRaw(Map map) {
            map.TileMap = new ITileController[map.Width, map.Height];
            for (int x = 0; x < map.Width; x++) {
                for (int y = 0; y < map.Height; y++) {
                    map.TileMap[x, y] = new TileController(map.RawMap[x, y]);
                }
            }
        }
    }
}
