using System.IO;
using Assets.Scripts.Models;
using SimpleJSON;

namespace Assets.Scripts.Controllers {
    public class MapGenerator : IMapGenerator {
        public Map GenerateMap(int id) {
            Map map;
            using (StreamReader r = new StreamReader(File.OpenRead(string.Format("C:\\proj\\git\\SingedFeathers\\SingedFeathers\\Assets\\Resources\\map{0}.json", id)))) {
                string json = r.ReadToEnd();
                JSONNode jsonNode = JSON.Parse(json);

                int width = ParseInt(jsonNode, "Width");
                int height = ParseInt(jsonNode, "Height");

                map = new Map() {
                    InitialFirePosition = ParseInitialFirePosition(jsonNode),
                    Width = width,
                    Height = height,
                    TileMap = ParseMap(jsonNode, width, height)
                };
            }
            return map;
        }

        private ITileController[,] ParseMap(JSONNode json, int width, int height) {
            ITileController[,] tileMap = new ITileController[width,height];
            JSONNode map = json["Map"];
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    tileMap[x, y] = ParseTileController(map, x, y);
                }
            }
            return tileMap;
        }

        private Position ParseInitialFirePosition(JSONNode json) {
            return new Position() {
                X = ParseInt(json["InitialFirePosition"], "X"),
                Y = ParseInt(json["InitialFirePosition"], "Y"),
            };
        }

        private int ParseInt(JSONNode json, string key) {
            return int.Parse(json[key].Value);
        }

        private int ParseInt(JSONNode json, int x, int y) {
            return int.Parse(json[y][x]);
        }

        private ITileController ParseTileController(JSONNode json, int x, int y) {
            return new TileController(ParseTileType(json, x, y));
        }

        private TileType ParseTileType(JSONNode json, int x, int y) {
            return (TileType) ParseInt(json, x, y);
        }
    }
}
