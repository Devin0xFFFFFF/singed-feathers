using System;

namespace CoreGame.Models {
    [Serializable]
    public class Tile {
        public TileType Type;
        public int FlashPoint;
        public int MaxTurnsOnFire;
        public int TurnsOnFire;
        public int Heat;

        public Tile(TileType type, int flashPoint, int maxTurnsOnFire) {
            Type = type;
            FlashPoint = flashPoint;
            MaxTurnsOnFire = maxTurnsOnFire;
            Heat = 0;
            TurnsOnFire = 0;
        }
    }
}
