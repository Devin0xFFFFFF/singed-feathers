using CoreGame.Models;

namespace CoreGame.Utility {
    public class MapLocationValidator {
        private static int _mapHeight;
        private static int _mapWidth;
        private MapLocationValidator() { }

        public static void InitializeValues(Map map) {
            _mapHeight = map.Height;
            _mapWidth = map.Width;
        }

        public static bool PositionIsValid(Position position) { return CoordinatesAreValid(position.X, position.Y); }

        public static bool CoordinatesAreValid(int x, int y) { return _mapWidth > x && x >= 0 && _mapHeight > y && y >= 0; }
    }
}