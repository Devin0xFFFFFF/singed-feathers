using CoreGame.Models;
using System.Collections.Generic;

namespace CoreGame.Service {
    public interface IMapGeneratorService {
        Map GenerateMap(string serializedMap);
        Map GenerateDefaultMap(int width, int height);
        string SerializeMap(int width, int height, TileType[,] tileMap, IList<Position> firePositions, IList<Position> pigeonPositions, int numTurns);
    }
}