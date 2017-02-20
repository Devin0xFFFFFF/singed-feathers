namespace Assets.Scripts.Model {
    public class Tile {
        public TileType Type;
        public bool IsFlammable;
        public int FlashPoint;
        public int Durability;
        public int BurnDuration;
        public bool OnFire;
        public int Heat;
        public bool IsBurntOut {
            get { return BurnDuration <= 0; }
        }

        public Tile(TileType type, bool isFlammable, int flashPoint, int durability, int burnDuration, int heat = 0, bool onFire = false) {
            Type = type;
            IsFlammable = isFlammable;
            FlashPoint = flashPoint;
            Durability = durability;
            BurnDuration = burnDuration;
            OnFire = onFire;
            Heat = heat;
        }
    }
}
