using Assets.Scripts.Models;

namespace Assets.Scripts.Service {
    public interface IMapGeneratorService {
        Map GenerateMap(string serializedMap);
    }
}