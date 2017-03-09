using System.Collections.Generic;
using System.Timers;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Commands;
using Newtonsoft.Json;

namespace Assets.Scripts.Controllers {
    public class LocalTurnResolver : ITurnResolver {
        private bool _isTurnResolved = true;

        public bool IsTurnResolved() { return _isTurnResolved; }

        public void ResolveTurn(IDictionary<ITileController, ICommand> moves, ITileController[,] tileMap) {
            _isTurnResolved = false;
            List<Delta> deltaList = new List<Delta>();
            foreach (KeyValuePair<ITileController, ICommand> move in moves) {
                Delta delta = new Delta(move.Key.Position, move.Value.GetCommand());
                deltaList.Add(delta);
            }

            string json = JsonConvert.SerializeObject(deltaList);

            Timer timer = new Timer(3000);
            timer.Elapsed += (sender, e) => ApplyDelta(sender, e, json, tileMap);
            timer.AutoReset = false;
            timer.Enabled = true;
        }

        private void ApplyDelta(object sender, ElapsedEventArgs e, string json, ITileController[,] tileMap) {
            int mapWidth = tileMap.GetLength(0);
            int mapHeight = tileMap.GetLength(1);
            List<Delta> translatedDeltaList = JsonConvert.DeserializeObject<List<Delta>>(json);
            foreach (Delta delta in translatedDeltaList) {
                Position position = delta.Position;
                ICommand iCommand = delta.Command.MakeICommand();
                if (mapWidth > position.X && position.X >= 0 && mapHeight > position.Y && position.Y >= 0) {
                    ITileController tileController = tileMap[position.X, position.Y];
                    iCommand.ExecuteCommand(tileController);
                }
            }
            _isTurnResolved = true;
        }
    }
}
