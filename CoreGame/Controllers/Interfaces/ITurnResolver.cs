using CoreGame.Models;

namespace CoreGame.Controllers.Interfaces {
    public interface ITurnResolver {
        void ResolveTurn(Delta delta, ITileController[,] tileMap);
        bool IsTurnResolved();
    }
}
