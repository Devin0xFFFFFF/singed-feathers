using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using System.Collections.Generic;

namespace Assests.Scripts.Utility {
    public class CommandValidator {
        private static int _maxMovesPerTurn;
        private CommandValidator() { }

        public static void InitializeValues(Map map) { _maxMovesPerTurn = map.MaxMovesPerTurn; }

        public static bool ValidateDeltas(List<Delta> deltas, ITileController[,] tileMap) {
            if (deltas.Count > _maxMovesPerTurn) {
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
