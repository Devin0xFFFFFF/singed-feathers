namespace Assets.Scripts.Controllers {
    public interface ITileController {
        TileType GetTileType();
        void StartTurn();
        bool Ignite();
        bool Extinguish();
        void ApplyHeat(int heat);
        void AddNeighbouringTile(ITileController neighbourController);
        void SpreadFire();
        bool IsBurntOut();
        bool IsLit();
    }
}
