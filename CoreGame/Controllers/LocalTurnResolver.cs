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

        public void ResolveTurn(Delta delta, Map map) {
            _isTurnResolved = false;
            List<Delta> deltaList = new List<Delta>();
            if (delta != null) {
                deltaList.Add(delta);
            }

            if (CommandValidator.ValidateDeltas(deltaList, map.TileMap)) {
                string json = JsonConvert.SerializeObject(deltaList, _settings);
                Timer timer = new Timer(3000);
                timer.Elapsed += (sender, e) => ApplyDelta(sender, e, json, map);
                timer.AutoReset = false;
                timer.Enabled = true;
            } else {
                _isTurnResolved = true;
            }
        }

        private void ApplyDelta(object sender, ElapsedEventArgs e, string json, Map map) {
            IList<Delta> translatedDeltaList = JsonConvert.DeserializeObject<List<Delta>>(json, _settings);

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