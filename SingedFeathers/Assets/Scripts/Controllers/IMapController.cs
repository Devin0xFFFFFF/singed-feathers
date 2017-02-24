﻿using System.Collections.Generic;
using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers {

    public interface IMapController {

        int Width { get; }
        int Height { get; }
        void GenerateMap();
        void ApplyHeat(int x, int y);
        TileType GetTileType(int x, int y);
        IDictionary<NewStatus, IList<Position>> SpreadFires();
        ITileController GetController(int x, int y);
    }
}
