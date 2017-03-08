using System.Collections.Generic;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Commands;
using Newtonsoft.Json;

namespace Assets.Scripts.Controllers {
    public class LocalTurnResolver : ITurnResolver {
        private bool _isTurnResolved = true;

        public bool IsTurnResolved() { return _isTurnResolved; }

        public void ResolveTurn(IDictionary<ITileController, ICommand> moves, ITileController[,] tileMap) {
            _isTurnResolved = false;
            int mapWidth = tileMap.GetLength(0);
            int mapHeight = tileMap.GetLength(1);
            foreach (KeyValuePair<ITileController, ICommand> move in moves) {
                Delta delta = new Delta(move.Key.Position, move.Value.GetCommand());
                string json = JsonConvert.SerializeObject(delta);
                Delta translatedDelta = JsonConvert.DeserializeObject<Delta>(json);
                Position position = translatedDelta.Position;
                ICommand iCommand = translatedDelta.Command.MakeICommand();
                if (mapWidth > position.X && position.X >= 0 && mapHeight > position.Y && position.Y >= 0) {
                    ITileController tileController = tileMap[position.X, position.Y];
                    iCommand.ExecuteCommand(tileController);
                }
            }
            _isTurnResolved = true;
        }
    }
}
