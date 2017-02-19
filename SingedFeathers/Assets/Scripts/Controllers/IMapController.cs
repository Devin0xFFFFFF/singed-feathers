using System.Collections.Generic;
using Assets.Scripts.Model;

namespace Assets.Scripts.Controllers {
    public interface IMapController {
        int Width { get; }
        int Height { get; }
        void GenerateMap();
        void ApplyHeat(int x, int y, int heat);
        TileType GetTileType(int x, int y);
        void TakeTurn();
        IList<Position> SpreadFires();
    }
}
