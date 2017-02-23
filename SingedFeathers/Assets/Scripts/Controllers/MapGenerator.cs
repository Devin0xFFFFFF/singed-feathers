using System.IO;
using Assets.Scripts.Models;
using Newtonsoft.Json;

namespace Assets.Scripts.Controllers {
    public class MapGenerator : IMapGenerator {
        public Map GenerateMap(int id) {
            Map map;
            string path = string.Format("..\\SingedFeathers\\Assets\\Resources\\map{0}.json", id);

            using (StreamReader r = new StreamReader(File.OpenRead(path))) {
                string json = r.ReadToEnd();
                map = JsonConvert.DeserializeObject<Map>(json);

                InitializeTileMapFromRaw(map);
            }
            return map;
        }

        private void InitializeTileMapFromRaw(Map map) {
            map.TileMap = new ITileController[map.Width,map.Height];
            for (int x = 0; x < map.Width; x++) {
                for (int y = 0; y < map.Height; y++) {
                    map.TileMap[x,y] = new TileController((TileType)map.RawMap[x,y]);
                }
            }
        }
    }
}
