using System.Collections.Generic;
using Assets.Scripts.Models;
using Assets.Scripts.Models.Commands;
using Newtonsoft.Json;
using Assests.Scripts.Utility;
using Assets.Scripts.Controllers;
using UnityEngine;
using System.Collections;

namespace Assets.Scripts.Controllwers {
    public class WebTurnResolver : MonoBehaviour, ITurnResolver {
        private bool _isTurnResolved = true;

        public bool IsTurnResolved() { return _isTurnResolved; }

        public void ResolveTurn(IDictionary<ITileController, ICommand> moves, Map map) {
            _isTurnResolved = false;
            StartCoroutine(ExecuteAfterTime(3, moves, map));
        }

        private void ApplyDelta(string json, Map map) {
            List<Delta> translatedDeltaList = JsonConvert.DeserializeObject<List<Delta>>(json);
            foreach (Delta delta in translatedDeltaList) {
                Position position = delta.Position;
                ICommand iCommand = delta.Command.MakeICommand();
                if (MapLocationValidator.PositionIsValid(position)) {
                    ITileController tileController = map.TileMap[position.X, position.Y];
                    iCommand.ExecuteCommand(tileController);
                }
            }
            TurnResolveUtility.SpreadFires(map);
            TurnResolveUtility.MovePigeons(map);
            _isTurnResolved = true;
        }

        private IEnumerator ExecuteAfterTime(float time, IDictionary<ITileController, ICommand> moves, Map map) {
            yield return new WaitForSeconds(time);

            List<Delta> deltaList = new List<Delta>();
            foreach (KeyValuePair<ITileController, ICommand> move in moves) {
                Delta delta = new Delta(move.Key.Position, move.Value.GetCommand());
                deltaList.Add(delta);
            }

            string json = JsonConvert.SerializeObject(deltaList);

            ApplyDelta(json, map);
        }
    }
}
