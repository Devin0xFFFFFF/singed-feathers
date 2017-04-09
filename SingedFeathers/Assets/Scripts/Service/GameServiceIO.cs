using CoreGame.Models.API.GameService;
using Newtonsoft.Json;
using System;
using System.Collections;

namespace Assets.Scripts.Service {
    public class GameServiceIO : APersistenceIO {
        private GameServiceClient _client;
        private const int ERROR_CODE = 3;

        public delegate void CommitTurnCallback(bool success);
        public delegate void PollGameCallback(PollResponse result);
        public delegate void TestGameCallback(int response);

        public GameServiceIO() { _client = new GameServiceClient(); }

        public IEnumerator CommitTurn(CommitTurnRequest commitTurnRequest, CommitTurnCallback callback) {
            yield return _client.CommitTurn(commitTurnRequest, delegate (ClientResult result) {
                if (IsValidResult(result)) {
                    bool success = JsonConvert.DeserializeObject<bool>(result.ResponseBody);
                    Console.WriteLine("Response Recived " + success);
                    callback(success);
                } else {
                    Console.WriteLine("Failed to commit turn: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                    callback(false);
                }
            });
        }

        public IEnumerator PollGame(PollRequest pollRequest, PollGameCallback callback) {
            yield return _client.PollGame(pollRequest, delegate (ClientResult result) {
                if (IsValidResult(result)) {
                    PollResponse response = JsonConvert.DeserializeObject<PollResponse>(result.ResponseBody);
                    Console.WriteLine("Poll Response Recived " + result.ResponseBody);
                    callback(response);
                } else {
                    Console.WriteLine("Failed to poll game: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                    callback(new PollResponse(false, null));
                }
            });
        }

        public IEnumerator Surrender(PollRequest pollRequest, PollGameCallback callback) {
            yield return _client.Surrender(pollRequest, delegate (ClientResult result) {
                if (IsValidResult(result)) {
                    PollResponse response = JsonConvert.DeserializeObject<PollResponse>(result.ResponseBody);
                    Console.WriteLine("Response Recived " + response);
                    callback(response);
                } else {
                    Console.WriteLine("Failed to poll game: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                    callback(new PollResponse(false, null));
                }
            });
        }

        public IEnumerator Test(TestGameCallback callback) {
            yield return _client.Test(delegate (ClientResult result) {
                if (IsValidResult(result)) {
                    int response = Int32.Parse(result.ResponseBody);
                    Console.WriteLine("Test Response Recived " + response);
                    callback(response);
                } else {
                    Console.WriteLine("Failed to test game: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                    callback(ERROR_CODE);
                }
            });
        }
    }
}
