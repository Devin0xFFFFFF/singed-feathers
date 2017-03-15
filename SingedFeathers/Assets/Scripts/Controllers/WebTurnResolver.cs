using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Models.Commands;
using CoreGame.Utility;

namespace Assets.Scripts.Controllers {
    public class WebTurnResolver : MonoBehaviour, ITurnResolver {
        private bool _isTurnResolved = true;
        private readonly JsonSerializerSettings _settings;

        public WebTurnResolver() {
            _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        }

        public bool IsTurnResolved() { return _isTurnResolved; }

        public void ResolveTurn(Delta delta, Map map) {
            _isTurnResolved = false;
            StartCoroutine(ExecuteAfterTime(2, delta, map));
        }

        private void ApplyDelta(string json, Map map) {
            List<Delta> translatedDeltaList = JsonConvert.DeserializeObject<List<Delta>>(json, _settings);
            foreach (Delta delta in translatedDeltaList) {
                Position position = delta.Position;
                ICommand iCommand = delta.Command;
                if (MapLocationValidator.PositionIsValid(position)) {
                    ITileController tileController = map.TileMap[position.X, position.Y];
                    iCommand.ExecuteCommand(tileController);
                }
            }
            TurnResolveUtility.SpreadFires(map);
            TurnResolveUtility.MovePigeons(map);
            _isTurnResolved = true;
        }

        private IEnumerator ExecuteAfterTime(float time, Delta delta, Map map) {
            yield return new WaitForSeconds(time);

            List<Delta> deltaList = new List<Delta>();
            if (delta != null) {
                deltaList.Add(delta);
            }

            string json = JsonConvert.SerializeObject(deltaList, _settings);

            ApplyDelta(json, map);
        }
    }
}
