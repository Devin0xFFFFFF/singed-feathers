using System;

namespace CoreGame.Models {
    [Serializable]
    public class Position : IComparable<Position> {
        public int X;
        public int Y;

        public Position(int x, int y) {
            X = x;
            Y = y;
        }

        public int CompareTo(Position other) {
            return (X * 100 + Y).CompareTo(other.X * 100 + other.Y);
        }

        public int GetLargestDistanceFrom(Position other) {
            return Math.Max(Math.Abs(X - other.X), Math.Abs(Y - other.Y));
        }

        public override string ToString() { return "X: " + X.ToString() + " Y: " + Y.ToString(); }

        public override bool Equals(object obj) {
            Position other = obj as Position;
            if (other != null) {
                return this.X == other.X && this.Y == other.Y;
            }
            return false;
        }
    }
}