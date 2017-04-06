using CoreGame.Controllers.Interfaces;
using CoreGame.Models;

namespace CoreGame.Utility {
    public class TurnResolveUtility {
        public static void SpreadFires(Map map) {
            for (int x = 0; x < map.Width; x++) {
                for (int y = 0; y < map.Height; y++) {
                    ITileController tile = map.TileMap[x, y];
                    tile.SpreadFire();
                }
            }

            for (int x = 0; x < map.Width; x++) {
                for (int y = 0; y < map.Height; y++) {
                    ITileController tile = map.TileMap[x, y];
                    tile.UpKeep();
                }
            }
        }

        public static void MovePigeons(Map map) {
            foreach (IPigeonController pigeon in map.Pigeons) {
                pigeon.React();
            }
        }
    }
}
