using System.Collections.Generic;
using Assets.Scripts.Models.Commands;

namespace Assets.Scripts.Controllers {
    public interface ITurnResolver {
        void ResolveTurn(IDictionary<ITileController, Command> moves, ITileController[,] tileMap);
        bool IsTurnResolved();
    }
}
