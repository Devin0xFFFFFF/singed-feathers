using CoreGame.Models;

namespace CoreGame.Service {
    public interface IMapGeneratorService {
        Map GenerateMap(string serializedMap);
        Map GenerateDefaultMap(int width, int height);
    }
}