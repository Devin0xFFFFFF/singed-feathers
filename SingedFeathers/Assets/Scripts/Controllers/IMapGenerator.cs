using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers {
    public interface IMapGenerator {
        Map GenerateMap(int id);
    }
}
