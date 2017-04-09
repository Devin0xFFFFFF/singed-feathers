using System.Collections;
using UnityEngine.Networking;
using CoreGame.Models.API.GameService;
using System.Collections.Generic;

namespace Assets.Scripts.Service {
    public class GameServiceClient : APersistenceClient {
        private const string COMMIT_TURN_PATH = "CommitTurn";
        private const string POLL_GAME_PATH = "PollGame";
        private const string SURRENDER_PATH = "Surrender";
        private const string TEST_PATH = "TestGameService";

        public GameServiceClient(AWSAPIClientConfig apiConfig = null) : base(apiConfig) { }

        public IEnumerator CommitTurn(CommitTurnRequest commitTurnRequest, ResultCallback resultCallback) {
            UnityWebRequest request = _requestBuilder.BuildPutRequest(COMMIT_TURN_PATH, SerializeObject(commitTurnRequest));
            yield return request.Send();
            ReturnResult(resultCallback, COMMIT_TURN_PATH, request);
        }

        public IEnumerator PollGame(PollRequest pollRequest, ResultCallback resultCallback) {
            UnityWebRequest request = _requestBuilder.BuildPutRequest(POLL_GAME_PATH, SerializeObject(pollRequest));
            yield return request.Send();
            ReturnResult(resultCallback, POLL_GAME_PATH, request);
        }

        public IEnumerator Surrender(PollRequest pollRequest, ResultCallback resultCallback) {
            UnityWebRequest request = _requestBuilder.BuildPutRequest(SURRENDER_PATH, SerializeObject(pollRequest));
            yield return request.Send();
            ReturnResult(resultCallback, SURRENDER_PATH, request);
        }

        public IEnumerator Test(ResultCallback resultCallback) {
            UnityWebRequest request = _requestBuilder.BuildGetRequest(TEST_PATH, new SortedDictionary<string, string>());
            yield return request.Send();
            ReturnResult(resultCallback, TEST_PATH, request);
        }
    }
}