using System.Collections.Generic;
using Assets.Scripts.Models.Commands;

namespace Assets.Scripts.Controllers {
    public interface ITurnResolver {
        void ResolveTurn(IDictionary<ITileController, ICommand> moves, IMapController mapController);
        bool IsTurnResolved();
    }
}
