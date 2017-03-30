using System.Collections.Generic;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using System.Collections;
using CoreGame.Models.Commands;
using CoreGame.Utility;
using Newtonsoft.Json;

namespace CoreGame.Controllers {
    public class LocalTurnResolver : ITurnResolver {

        public LocalTurnResolver() { }

        public bool IsTurnResolved() { return true; }

        public bool ShouldPoll() { return false; }

        public void ResolveTurn(Delta delta, Map map) {
            List<Delta> deltaList = new List<Delta>();
            if (delta != null) {
                deltaList.Add(delta);
            }
            ApplyDelta(deltaList, map);
        }

        public void Poll(Map map) { }

        private void ApplyDelta(List<Delta> deltaList, Map map) {
            IList<Delta> waterCommands = new List<Delta>();
            foreach (Delta delta in deltaList) {
                if (MapLocationValidator.PositionIsValid(delta.Position)) {
                    ApplyDelta(delta, map);
                    if (delta.Command.MoveType == MoveType.Water) { waterCommands.Add(delta); }
                }
            }
            TurnResolveUtility.SpreadFires(map);

            foreach (Delta delta in waterCommands) {
                ApplyDelta(delta, map);
            }

            TurnResolveUtility.MovePigeons(map);
        }

        private void ApplyDelta(Delta delta, Map map) {
            Position position = delta.Position;
            ICommand iCommand = delta.Command;
            ITileController tileController = map.TileMap[position.X, position.Y];
            iCommand.ExecuteCommand(tileController);
        }
    }
}
