using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Models.Commands;
using CoreGame.Utility;

using System.IO;
using CoreGame.Models.API.GameService;

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

        public void ResolveTurn(Delta delta, Map map, Player player) {
            _isTurnResolved = false;
            CommitTurnRequest commitTurnRequest = new CommitTurnRequest("Test", player.PlayerID, delta);
            SendCommitTurnRequest(commitTurnRequest);
        }

        public void Poll(Map map, Player player) {
            _receivedResponse = true;
            PollRequest request = new PollRequest("Test", player.PlayerID);
            StartCoroutine(ExecuteAfterTime(0.1f, map));
        }

        private IEnumerator ExecuteAfterTime(float time, Map map) {
            yield return new WaitForSeconds(time);

            Delta translatedDelta = JsonConvert.DeserializeObject<Delta>(JsonDelta, _settings);

            List<Delta> deltaList = new List<Delta>();
            if (translatedDelta != null) {
                deltaList.Add(translatedDelta);
            }
            bool success = Random.Range(0, 2) == 0;
            PollResponse response = new PollResponse(success, deltaList);
            _receivedResponse = response.IsValid;
            if (response.IsValid) {
                TurnResolveUtility.ApplyDelta(response.Turn, map);
                _isTurnResolved = true;
            }
        }

        private void SendCommitTurnRequest(CommitTurnRequest request) {
            _receivedResponse = false;
            StartCoroutine(_mapIO.GetMapData(mapID, delegate (string serializedMapData) {
                if (serializedMapData == null) {
                    Debug.LogError("Failed to retrieve map.");
                    return;
                }
                _mapController = new MapController();
                if (!_mapController.GenerateMap(serializedMapData)) {
                    Debug.LogError("Failed to generate map.");
                    return;
                }
                _mapController.SetTurnResolver(TurnResolver);

                _width = _mapController.Width;
                _height = _mapController.Height;
                _map = new TileView[_width, _height];

                _tileSizeX = TileSet[0].GetComponent<Renderer>().bounds.size.x;
                _tileSizeY = TileSet[0].GetComponent<Renderer>().bounds.size.y;

                InstantiateTiles();
                SetPlayerSideSelection();
                SetPlayerSideSelectionText();
                Debug.Log(_mapController.GetPlayerSideSelection());

                LoadPigeons();
                LoadInputView();
            }));
        }

        private void SendPollRequest(PollRequest request) {
            _receivedResponse = false;
        }
    }
}
