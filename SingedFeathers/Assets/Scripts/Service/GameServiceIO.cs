using CoreGame.Models.API.GameService;
using Newtonsoft.Json;
using System;
using System.Collections;

namespace Assets.Scripts.Service {
    class GameServiceIO : APersistenceIO {
        private GameServiceClient _client;

        public delegate void CommitTurnCallback(bool success);
        public delegate void PollGameCallback(PollResponse result);

        public GameServiceIO() { _client = new GameServiceClient(); }

        public IEnumerator CommitTurn(CommitTurnRequest commitTurnRequest, CommitTurnCallback callback) {
            yield return _client.CommitTurn(commitTurnRequest, delegate (ClientResult result) {
                System.IO.File.WriteAllText("Commit.txt", result.ResponseCode.ToString());
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
                System.IO.File.WriteAllText("Poll.txt", result.ResponseCode.ToString());
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
    }
}
