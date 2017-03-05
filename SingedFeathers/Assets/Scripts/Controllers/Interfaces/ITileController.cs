using System.Collections.Generic;
using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers {
    public interface ITileController {
        bool StateHasChanged { get; set; }
        bool IsOccupied { get; }
        Position Position { get; set; }
        bool CanBeOccupied();
        TileType GetTileType();
        void Extinguish();
        void ApplyHeat(int heat);
        void ReduceHeat(int heat);
        void AddNeighbouringTile(ITileController neighbourController);
        IEnumerable<ITileController> GetNeighbours();
        void SpreadFire();
        bool IsBurntOut();
        bool IsFlammable();
        bool IsOnFire();
        bool IsSpreadingHeat();
        bool MarkOccupied();
        bool MarkUnoccupied();
    }
}