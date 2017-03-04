using System.Collections.Generic;
using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers {
    public interface ITileController {
        Tile Tile { get; }
        bool StateHasChanged { get; set; }
        bool IsOccupied { get; }
        Position Position { get; set; }
        bool CanBeOccupied();
        TileType GetTileType();
        void Extinguish();
        void ApplyHeat(int heat);
        void AddNeighbouringTile(ITileController neighbourController);
        IEnumerable<ITileController> GetNeighbours();
        void SpreadFire();
        bool IsBurntOut();
        bool IsOnFire();
        bool IsSpreadingHeat();
        bool OccupyTile();
        bool LeaveTile();
    }
}