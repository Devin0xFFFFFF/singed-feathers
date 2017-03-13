using System.Collections.Generic;
using Assets.Scripts.Models.Commands;
using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers {
    public interface ITurnResolver {
        void ResolveTurn(IDictionary<ITileController, ICommand> moves, Map map);
        bool IsTurnResolved();
    }
}
