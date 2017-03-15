using System.Collections.Generic;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Models.Commands;
using CoreGame.Utility;
using Newtonsoft.Json;
using System.Timers;

namespace CoreGame.Controllers {
    public class LocalTurnResolver : ITurnResolver {
        private readonly JsonSerializerSettings _settings;

        public LocalTurnResolver() {
            _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        }

        private bool _isTurnResolved = true;

        public bool IsTurnResolved() { return _isTurnResolved; }

        public void ResolveTurn(IDictionary<ITileController, ICommand> moves, Map map) {
            _isTurnResolved = false;
            IList<Delta> deltaList = new List<Delta>();
            foreach (KeyValuePair<ITileController, ICommand> move in moves) {
                Delta delta = new Delta(move.Key.Position, move.Value);
                deltaList.Add(delta);
            }

            string json = JsonConvert.SerializeObject(deltaList);

            ApplyDelta(json, map);
        }

        private void ApplyDelta(string json, Map map) {
            List<Delta> translatedDeltaList = JsonConvert.DeserializeObject<List<Delta>>(json);
            foreach (Delta delta in translatedDeltaList) {
                Position position = delta.Position;
                ICommand command = delta.Command;
                if (MapLocationValidator.PositionIsValid(position)) {
                    ITileController tileController = map.TileMap[position.X, position.Y];
                    command.ExecuteCommand(tileController);
                }
            }
            TurnResolveUtility.SpreadFires(map);
            TurnResolveUtility.MovePigeons(map);
            _isTurnResolved = true;
        }
    }
}