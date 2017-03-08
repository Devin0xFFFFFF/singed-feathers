using System.Collections.Generic;
using Assets.Scripts.Models.Commands;

namespace Assets.Scripts.Controllers {
    class LocalTurnResolver :ITurnResolver {
        private bool _isTurnResolved = true;

        public bool IsTurnResolved() { return _isTurnResolved; }

        public void ResolveTurn(IDictionary<ITileController, ICommand> moves) {
            _isTurnResolved = false;
            foreach (KeyValuePair<ITileController, ICommand> move in moves) {
                move.Value.ExecuteCommand(move.Key);
            }
            _isTurnResolved = true;
        }
    }
}
