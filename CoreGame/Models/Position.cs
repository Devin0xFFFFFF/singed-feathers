using System;

namespace Assets.Scripts.Models {
    [Serializable]
    public class Position {
        public int X;
        public int Y;

        public Position(int x, int y) {
            X = x;
            Y = y;
        }
    }
}