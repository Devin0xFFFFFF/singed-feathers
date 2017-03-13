using System.Collections.Generic;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Commands;
using Newtonsoft.Json;
using Assests.Scripts.Utility;

namespace Assets.Scripts.Controllers {
    public class LocalTurnResolver : ITurnResolver {
        private bool _isTurnResolved = true;

        public bool IsTurnResolved() { return _isTurnResolved; }

        public void ResolveTurn(IDictionary<ITileController, ICommand> moves, ITileController[,] tileMap) {
            _isTurnResolved = false;
            List<Delta> deltaList = new List<Delta>();
            foreach (KeyValuePair<ITileController, ICommand> move in moves) {
                Delta delta = new Delta(move.Key.Position, move.Value);
                deltaList.Add(delta);
            }
            if (CommandValidator.ValidateDeltas(deltaList, tileMap)) {
                string json = JsonConvert.SerializeObject(deltaList);
                ApplyDelta(json, tileMap);
            }
        }

        private void ApplyDelta(string json, ITileController[,] tileMap) {
            IList<Delta> translatedDeltaList = JsonConvert.DeserializeObject<List<Delta>>(json);
            foreach (Delta delta in translatedDeltaList) {
                Position position = delta.Position;
                ICommand command = delta.Command;
                if (MapLocationValidator.PositionIsValid(position)) {
                    ITileController tileController = tileMap[position.X, position.Y];
                    command.ExecuteCommand(tileController);
                }
            }
            _isTurnResolved = true;
        }
    }
}
