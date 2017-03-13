using CoreGame.Models;

namespace CoreGame.Utility {
    public class MapLocationValidator {
        private static int MapHeight;
        private static int MapWidth;
        private MapLocationValidator() { }

        public static void InitializeValues(Map map) {
            MapHeight = map.Height;
            MapWidth = map.Width;
        }

        public static bool PositionIsValid(Position position) { return CoordinatesAreValid(position.X, position.Y); }

        public static bool CoordinatesAreValid(int x, int y) { return MapWidth > x && x >= 0 && MapHeight > y && y >= 0; }
    }
}
