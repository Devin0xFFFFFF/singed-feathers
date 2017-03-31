using System.Collections.Generic;
using System.Linq;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;

namespace CoreGame.Utility {
    public class MapMakerInputValidator {
        public static MapMakerValidationResult ValidateInput(string mapName, string authorName, IEnumerable<Position> pigeonPositions, IEnumerable<ITileController> tiles) {
            if (!AtLeastOneFlammableTile(tiles)) {
                return MapMakerValidationResult.InvalidNoFlammableTiles;
            } else if (!AtLeastOnePigeon(pigeonPositions)) {
                return MapMakerValidationResult.InvalidNoPigeons;
            } else if (!ValidInputReceived(mapName, authorName)) {
                return MapMakerValidationResult.InvalidInput;
            }
            return MapMakerValidationResult.Valid;
        }

        private static bool ValidInputReceived(string mapName, string authorName) { return !string.IsNullOrEmpty(mapName) && !string.IsNullOrEmpty(authorName); }
    
        private static bool AtLeastOneFlammableTile(IEnumerable<ITileController> tiles) { return tiles != null && tiles.Any(t => t.IsFlammable()); }

        private static bool AtLeastOnePigeon(IEnumerable<Position> pigeonPositions) { return pigeonPositions != null && pigeonPositions.Any(); }
    }
}