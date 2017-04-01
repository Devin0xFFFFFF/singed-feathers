﻿using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.Collections;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Utility;

using CoreGame.Models.API.GameService;
using Assets.Scripts.Service;

namespace Assets.Scripts.Controllers {
    public class WebTurnResolver : MonoBehaviour, ITurnResolver {
        private bool _isTurnResolved = true;
        private bool _receivedResponse = true;
        private readonly JsonSerializerSettings _settings;
        private GameServiceIO _gameServiceIO;
        private string _gameID;

        public WebTurnResolver(string gameID = "Test") {
            _gameID = gameID;
            _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        }

        public void Awake() {
            _gameServiceIO = new GameServiceIO();
        }
        public void SetGameID(string gameID) { _gameID = gameID; }

        public bool IsTurnResolved() { return _isTurnResolved; }

        public bool ShouldPoll() { return !_receivedResponse; }

        public void ResolveTurn(Delta delta, Map map, Player player) {
            _isTurnResolved = false;
            CommitTurnRequest commitTurnRequest = new CommitTurnRequest(_gameID, player.PlayerID, delta);
            SendCommitTurnRequest(commitTurnRequest);
        }

        public void Poll(Map map, Player player) {
            _receivedResponse = true;
            PollRequest request = new PollRequest(_gameID, player.PlayerID);
            SendPollRequest(request, map);
        }

        private IEnumerator ExecuteAfterTime(float time, Map map) {
            yield return new WaitForSeconds(time);
        }

        private void SendCommitTurnRequest(CommitTurnRequest request) {
            _receivedResponse = false;

            StartCoroutine(_gameServiceIO.CommitTurn(request, delegate (bool success) {
            }));
        }

        private void SendPollRequest(PollRequest request, Map map) {
            StartCoroutine(_gameServiceIO.PollGame(request, delegate (PollResponse response) {
                _receivedResponse = response.IsValid;
                if (response.IsValid) {
                    TurnResolveUtility.ApplyDelta(response.Turn, map);
                    _isTurnResolved = true;
                }
            }));
        }
    }
}
