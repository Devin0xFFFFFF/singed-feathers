namespace Assets.Scripts.Controllers {
    public interface ITileController {
        TileType GetTileType();
        void Extinguish();
        void ApplyHeat(int heat);
        void AddNeighbouringTile(ITileController neighbourController);
        void SpreadFire();
        bool IsBurntOut();
        bool IsOnFire();
    }
}
