using System.Collections.Generic;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;

namespace CoreGame.Utility {
    public class CommandValidator {
        private static int _maxMovesPerTurn;
        private CommandValidator() { }

        public static void InitializeValues(Map map) { _maxMovesPerTurn = map.MaxMovesPerTurn; }

        public static bool ValidateDeltas(List<Delta> deltas, ITileController[,] tileMap) {
            if (InvalidInputReceived(deltas, tileMap) || deltas.Count > _maxMovesPerTurn) {
                return false;
            }

            foreach (Delta delta in deltas) {
                Position position = delta.Position;
                if (MapLocationValidator.PositionIsValid(position)) {
                    ITileController tileController = tileMap[position.X, position.Y];
                    if (!delta.Command.CanBeExecutedOnTile(tileController)) {
                        return false;
                    }
                } else {
                    return false;
                }
            }

            return true;
        }

        private static bool InvalidInputReceived(List<Delta> deltas, object tilemap) { return deltas == null || tilemap == null; }
    }
}