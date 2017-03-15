using System.Collections.Generic;
using CoreGame.Models.Commands;

namespace CoreGame.Controllers.Interfaces {
    public interface ITurnResolver {
        void ResolveTurn(IDictionary<ITileController, ICommand> moves, ITileController[,] tileMap);
        bool IsTurnResolved();
    }
}
