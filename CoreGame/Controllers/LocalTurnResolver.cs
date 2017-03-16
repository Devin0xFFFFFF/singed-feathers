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

        public bool ShouldPoll() { return false; }

        public void Poll(Map map) {}

        public void ResolveTurn(Delta delta, Map map) {
            _isTurnResolved = false;
            List<Delta> deltaList = new List<Delta>();
            if (delta != null) {
                deltaList.Add(delta);
            }

            string json = JsonConvert.SerializeObject(deltaList, _settings);

            ApplyDelta(json, map);
        }

        private void ApplyDelta(string json, Map map) {
            List<Delta> translatedDeltaList = JsonConvert.DeserializeObject<List<Delta>>(json, _settings);
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
