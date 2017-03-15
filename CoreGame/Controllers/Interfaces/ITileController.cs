using System.Collections.Generic;
using CoreGame.Models;

namespace CoreGame.Controllers.Interfaces {
    public interface ITileController {
        Tile Tile { get; }
        bool IsOccupied { get; }
        Position Position { get; set; }
        bool CanBeOccupied();
        TileType GetTileType();
        void ApplyHeat(int heat);
        void ReduceHeat(int heat);
        void AddNeighbouringTile(ITileController neighbourController);
        IEnumerable<ITileController> GetNeighbours();
        void SpreadFire();
        void UpKeep();
        bool IsBurntOut();
        bool IsFlammable();
        bool IsOnFire();
        bool MarkOccupied();
        bool MarkUnoccupied();
        bool IsHeatZero();
        int GetTileHeat();
    }
}