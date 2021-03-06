﻿using Assets.Scripts.Service.Client;
using CoreGame.Models.API.GameService;
using Newtonsoft.Json;
using System;
using System.Collections;

namespace Assets.Scripts.Service.IO {
    public class GameIO : APersistenceIO {
        private readonly IGameClient _client;
        private const int ERROR_CODE = 3;

        public delegate void CommitTurnCallback(bool success);
        public delegate void PollGameCallback(PollResponse result);
        public delegate void TestGameCallback(int response);

        public GameIO(IGameClient client = null) { _client = client ?? new GameClient(); }

        public IEnumerator CommitTurn(CommitTurnRequest commitTurnRequest, CommitTurnCallback callback) {
            yield return _client.CommitTurn(commitTurnRequest, delegate (ClientResult result) {
                callback(ParseCommitTurnResult(result));
            });
        }

        public IEnumerator PollGame(PollRequest pollRequest, PollGameCallback callback) {
            yield return _client.PollGame(pollRequest, delegate (ClientResult result) {
                callback(ParsePollGameResult(result));
            });
        }

        public IEnumerator Surrender(PollRequest pollRequest, PollGameCallback callback) {
            yield return _client.Surrender(pollRequest, delegate (ClientResult result) {
                callback(ParseSurrenderResult(result));
            });
        }

        public bool ParseCommitTurnResult(ClientResult result) {
            if (IsValidResult(result)) {
                bool success = JsonConvert.DeserializeObject<bool>(result.ResponseBody);
                Console.WriteLine("Response Received " + success);
                return success;
            } else {
                Console.WriteLine("Failed to commit turn: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                return false;
            }
        }

        public PollResponse ParsePollGameResult(ClientResult result) {
            if (IsValidResult(result)) {
                PollResponse response = JsonConvert.DeserializeObject<PollResponse>(result.ResponseBody);
                Console.WriteLine("Poll Response Received " + result.ResponseBody);
                return response;
            } else {
                Console.WriteLine("Failed to poll game: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                return new PollResponse(false, null);
            }
        }

        public PollResponse ParseSurrenderResult(ClientResult result) {
            if (IsValidResult(result)) {
                PollResponse response = JsonConvert.DeserializeObject<PollResponse>(result.ResponseBody);
                Console.WriteLine("Response Received " + response);
                return response;
            } else {
                Console.WriteLine("Failed to poll game: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                return new PollResponse(false, null);
            }
        }

        public IEnumerator Test(TestGameCallback callback) {
            yield return _client.Test(delegate(ClientResult result) {
                if (IsValidResult(result)) {
                    int response = Int32.Parse(result.ResponseBody);
                    Console.WriteLine("Test Response Recived " + response);
                    callback(response);
                } else {
                    Console.WriteLine("Failed to test game: " + result.ErrorMessage ??
                                      result.ResponseCode + " " + result.ResponseBody);
                    callback(ERROR_CODE);
                }
            });
        }
    }
}