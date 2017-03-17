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
        private bool _receivedResponse = true;
        private readonly JsonSerializerSettings _settings;
        private string JsonDelta;

        public WebTurnResolver() {
            _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        }

        public void Awake() {
            Random.InitState(3);
            //Three is the most random number
        }

        public bool IsTurnResolved() { return _isTurnResolved; }

        public bool ShouldPoll() { return !_receivedResponse; }

        public void ResolveTurn(Delta delta, Map map) {
            _isTurnResolved = false;
            SendDelta(delta);
        }

        public void Poll(Map map) {
            _receivedResponse = true;
            StartCoroutine(ExecuteAfterTime(0.1f, map));
        }

        private void ApplyDelta(List<Delta> deltaList, Map map) {
            foreach (Delta delta in deltaList) {
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

        private IEnumerator ExecuteAfterTime(float time, Map map) {
            yield return new WaitForSeconds(time);

            Delta translatedDelta = JsonConvert.DeserializeObject<Delta>(JsonDelta, _settings);

            List<Delta> deltaList = new List<Delta>();
            if (translatedDelta != null) {
                deltaList.Add(translatedDelta);
            }
            bool success = Random.Range(0, 2) == 0;
            ServerResponse response = new ServerResponse(success, deltaList);
            _receivedResponse = response.IsValid;
            if (response.IsValid) {
                ApplyDelta(response.Turn, map);
            }
        }

        private void SendDelta(Delta delta) {
            JsonDelta = JsonConvert.SerializeObject(delta, _settings);
            _receivedResponse = false;
        }
    }
}
