using System.Collections.Generic;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Models.Commands;
using CoreGame.Utility;
using Newtonsoft.Json;

namespace CoreGame.Controllers {
    public class LocalTurnResolver : ITurnResolver {
        private readonly JsonSerializerSettings _settings;

        public LocalTurnResolver() {
            _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        }

        private bool _isTurnResolved = true;

        public bool IsTurnResolved() { return _isTurnResolved; }

        public void ResolveTurn(Delta delta, ITileController[,] tileMap) {
            _isTurnResolved = false;
            List<Delta> deltaList = new List<Delta>();
            if (delta != null) {
                deltaList.Add(delta);
            }

            if (CommandValidator.ValidateDeltas(deltaList, tileMap)) {
                string json = JsonConvert.SerializeObject(deltaList, _settings);
                ApplyDelta(json, tileMap);
            }
        }

        private void ApplyDelta(string json, ITileController[,] tileMap) {
            IList<Delta> translatedDeltaList = JsonConvert.DeserializeObject<List<Delta>>(json, _settings);
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