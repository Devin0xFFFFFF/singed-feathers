using System.Collections.Generic;
using System.Timers;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Commands;
using Newtonsoft.Json;
using Assests.Scripts.Utility;

namespace Assets.Scripts.Controllers {
    public class LocalTurnResolver : ITurnResolver {
        private bool _isTurnResolved = true;

        public bool IsTurnResolved() { return _isTurnResolved; }

        public void ResolveTurn(IDictionary<ITileController, Command> moves, ITileController[,] tileMap) {
            _isTurnResolved = false;
            List<Delta> deltaList = new List<Delta>();
            foreach (KeyValuePair<ITileController, Command> move in moves) {
                Delta delta = new Delta(move.Key.Position, move.Value);
                deltaList.Add(delta);
            }
            if (CommandValidator.ValidateDeltas(deltaList, tileMap)) {
                string json = JsonConvert.SerializeObject(deltaList);

                Timer timer = new Timer(1000);
                timer.Elapsed += (sender, e) => ApplyDelta(sender, e, json, tileMap);
                timer.AutoReset = false;
                timer.Enabled = true;
            }
        }

        private void ApplyDelta(object sender, ElapsedEventArgs e, string json, ITileController[,] tileMap) {
            int mapWidth = tileMap.GetLength(0);
            int mapHeight = tileMap.GetLength(1);
            List<Delta> translatedDeltaList = JsonConvert.DeserializeObject<List<Delta>>(json);
            foreach (Delta delta in translatedDeltaList) {
                Position position = delta.Position;
                Command command = delta.Command;
                if (MapLocationValidator.PositionIsValid(position)) {
                    ITileController tileController = tileMap[position.X, position.Y];
                    command.ExecuteCommand(tileController);
                }
            }
            _isTurnResolved = true;
        }
    }
}
