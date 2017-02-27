using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers {

    public interface ITileController {
        bool StateHasChanged { get; set; }
        TileType GetTileType();
        void Extinguish();
        void ApplyHeat(int heat);
        void AddNeighbouringTile(ITileController neighbourController);
        void SpreadFire();
        bool IsBurntOut();
        bool IsOnFire();
        bool IsSpreadingHeat();
    }
}