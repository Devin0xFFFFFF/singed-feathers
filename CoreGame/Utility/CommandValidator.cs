using System.Collections.Generic;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;

namespace CoreGame.Utility {
    public class CommandValidator {
        private CommandValidator() { }

        public static bool ValidateDelta(Delta delta, ITileController[,] tileMap) {
            if (tileMap == null) {
                return false;
            } else if (delta == null) {
                return true;
            }

            Position position = delta.Position;
            if (MapLocationValidator.PositionIsValid(position)) {
                ITileController tileController = tileMap[position.X, position.Y];
                return delta.Command.CanBeExecutedOnTile(tileController);
            } else {
                return false;
            }
        }
    }
}