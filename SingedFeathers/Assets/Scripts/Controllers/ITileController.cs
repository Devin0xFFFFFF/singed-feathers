using System.Collections.Generic;
using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers {

    public interface ITileController {
        bool StateHasChanged { get; set; }
        TileType GetTileType();
        Position Position { get; set; }
        void Extinguish();
        void ApplyHeat(int heat);
        void AddNeighbouringTile(ITileController neighbourController);
        IEnumerable<ITileController> GetNeighbours();
        void SpreadFire();
        bool IsBurntOut();
        bool IsOnFire();
        bool IsSpreadingHeat();
    }
}