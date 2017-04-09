using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Models.Commands;
using System.Collections.Generic;

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

        public static void ApplyDelta(IList<Delta> deltaList, Map map) {
            IList<Delta> waterCommands = new List<Delta>();
            foreach (Delta delta in deltaList) {
                if (MapLocationValidator.PositionIsValid(delta.Position)) {
                    ApplyDelta(delta, map);
                    if (delta.Command.MoveType == MoveType.Water) {
                        waterCommands.Add(delta);
                    }
                }
            }
            SpreadFires(map);

            foreach (Delta delta in waterCommands) {
                ApplyDelta(delta, map);
            }

            MovePigeons(map);
        }

        private static void ApplyDelta(Delta delta, Map map) {
            Position position = delta.Position;
            Command command = delta.Command;
            ITileController tileController = map.TileMap[position.X, position.Y];
            command.ExecuteCommand(tileController);
        }
    }
}
