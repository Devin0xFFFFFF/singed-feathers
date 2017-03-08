using System.Collections.Generic;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Commands;
using Newtonsoft.Json;

namespace Assets.Scripts.Controllers {
    public class LocalTurnResolver : ITurnResolver {
        private bool _isTurnResolved = true;

        public bool IsTurnResolved() { return _isTurnResolved; }

        public void ResolveTurn(IDictionary<ITileController, ICommand> moves, IMapController mapController) {
            _isTurnResolved = false;
            foreach (KeyValuePair<ITileController, ICommand> move in moves) {
                Delta delta = new Delta(move.Key.Position, move.Value.GetCommand());
                string json = JsonConvert.SerializeObject(delta);
                Delta translatedDelta = JsonConvert.DeserializeObject<Delta>(json);
                ICommand iCommand = translatedDelta._command.MakeICommand();
                ITileController tileController = mapController.GetTileController(translatedDelta._position);
                iCommand.ExecuteCommand(tileController);
            }
            _isTurnResolved = true;
        }
    }
}
