namespace Assets.Scripts.Model {
    public class Tile {
		// TODO: save Tile position
		public TileType Type;
		public int? FlashPoint;
		public int Health;
		public int MaxTurnsOnFire;
		public int TurnsOnFire;
		public int Heat;

		public Tile(TileType type, int? flashPoint, int maxTurnsOnFire) {
			Type = type;
			FlashPoint = flashPoint;
			MaxTurnsOnFire = maxTurnsOnFire;
			Heat = 0;
			TurnsOnFire = 0;
		}
    }
}
