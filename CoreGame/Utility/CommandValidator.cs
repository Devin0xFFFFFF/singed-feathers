using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using System.Collections.Generic;

namespace Assests.Scripts.Utility {
    public class CommandValidator {
        private static Map _map;
        private CommandValidator() { }

        public static void InitializeValues(Map map) {
            _map = map;
        }

        public static bool ValidateDeltas(List<Delta> deltas, ITileController[,] tileMap) {
            if (deltas.Count > _map.MaxMovesPerTurn) {
                return false;
            }

            foreach (Delta delta in deltas) {
                Position position = delta.Position;
                if (MapLocationValidator.PositionIsValid(position)) {
                    ITileController tileController = tileMap[position.X, position.Y];
                    if (!delta.Command.MakeICommand().CanBeExecutedOnTile(tileController)) {
                        return false;
                    }
                } else {
                    return false;
                }
            }

            return true;
        }
    }
}
