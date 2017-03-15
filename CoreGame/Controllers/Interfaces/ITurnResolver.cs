using System.Collections.Generic;
using CoreGame.Models.Commands;
using CoreGame.Models;

namespace CoreGame.Controllers.Interfaces {
    public interface ITurnResolver {
        void ResolveTurn(IDictionary<ITileController, ICommand> moves, Map map);
        bool IsTurnResolved();
    }
}
