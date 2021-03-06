﻿using UnityEngine;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Utility;

using CoreGame.Models.API.GameService;
using Assets.Scripts.Utility;
using Assets.Scripts.Service.IO;

namespace Assets.Scripts.Controllers {
    public class WebTurnResolver : MonoBehaviour, ITurnResolver {
        private bool _isTurnResolved = true;
        private bool _receivedResponse = true;
        private GameIO _gameServiceIO;
        private string _gameID;

        public WebTurnResolver(string gameID = "Test") { _gameID = gameID; }

        public void Awake() {
            _gameServiceIO = new GameIO();
            _isTurnResolved = true;
            _receivedResponse = true;
        }

        public void SetGameID(string gameID) { _gameID = gameID; }

        public bool IsTurnResolved() { return _isTurnResolved; }

        public bool ShouldPoll() { return !_receivedResponse; }

        public void ResolveTurn(Delta delta, Map map, Player player) {
            if (_isTurnResolved == true) {
                _isTurnResolved = false;
                CommitTurnRequest commitTurnRequest = new CommitTurnRequest(_gameID, player.PlayerID, delta);
                SendCommitTurnRequest(commitTurnRequest);
            }
        }

        public void Poll(Map map, Player player) {
            Debug.Log("sending PollRequest: " + _gameID + " " + player.PlayerID);
            _receivedResponse = true;
            PollRequest request = new PollRequest(_gameID, player.PlayerID);
            SendPollRequest(request, map);
        }

        private void SendCommitTurnRequest(CommitTurnRequest request) {
            _receivedResponse = false;
            StartCoroutine(_gameServiceIO.CommitTurn(request, delegate (bool success) {
            }));
        }

        private void SendPollRequest(PollRequest request, Map map) {
            StartCoroutine(_gameServiceIO.PollGame(request, delegate (PollResponse response) {
                if (response.IsValid) {
                    TurnResolveUtility.ApplyDelta(response.Turn, map);
                    _isTurnResolved = true;
                    _receivedResponse = true;
                } else {
                    StartCoroutine(PollTimer.ExecuteAfterWait(delegate () {
                        _receivedResponse = false;
                    }));
                }
            }));
        }
    }
}
