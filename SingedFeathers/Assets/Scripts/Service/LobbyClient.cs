using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using CoreGame.Models.API.LobbyClient;

namespace Assets.Scripts.Service {
    public class LobbyClient : APersistenceClient {
        private const string CREATE_LOBBY_PATH = "CreateLobby";
        private const string GET_LOBBIES_PATH = "GetLobbies";
        private const string JOIN_LOBBY_PATH = "JoinLobby";
        private const string LEAVE_LOBBY_PATH = "LeaveLobby";
        private const string READY_LOBBY_PATH = "ReadyLobby";
        private const string POLL_LOBBY_PATH = "PollLobby";

        private const string LOBBY_ID_FIELD = "LobbyID";
        private const string PLAYER_ID_FIELD = "PlayerID";

        public LobbyClient(AWSAPIClientConfig apiConfig = null) : base(apiConfig) { }

        public IEnumerator CreateLobby(CreateLobbyInfo lobbyInfo, ResultCallback resultCallback) {
            UnityWebRequest request = _requestBuilder.BuildPutRequest(CREATE_LOBBY_PATH, SerializeObject(lobbyInfo));
            yield return request.Send();
            ReturnResult(resultCallback, CREATE_LOBBY_PATH, request);
        }

        public IEnumerator GetLobbies(ResultCallback resultCallback) {
            SortedDictionary<string, string> queryParameters = BuildGetLobbiesQueryParameters();

            UnityWebRequest request = _requestBuilder.BuildGetRequest(GET_LOBBIES_PATH, queryParameters);
            yield return request.Send();
            ReturnResult(resultCallback, GET_LOBBIES_PATH, request);
        }

        public IEnumerator JoinLobby(JoinLobbyInfo lobbyInfo, ResultCallback resultCallback) {
            UnityWebRequest request = _requestBuilder.BuildPutRequest(JOIN_LOBBY_PATH, SerializeObject(lobbyInfo));
            yield return request.Send();
            ReturnResult(resultCallback, JOIN_LOBBY_PATH, request);
        }

        public IEnumerator LeaveLobby(LeaveLobbyInfo lobbyInfo, ResultCallback resultCallback) {
            UnityWebRequest request = _requestBuilder.BuildPutRequest(LEAVE_LOBBY_PATH, SerializeObject(lobbyInfo));
            yield return request.Send();
            ReturnResult(resultCallback, LEAVE_LOBBY_PATH, request);
        }

        public IEnumerator ReadyLobby(ReadyLobbyInfo lobbyInfo, ResultCallback resultCallback) {
            UnityWebRequest request = _requestBuilder.BuildPutRequest(READY_LOBBY_PATH, SerializeObject(lobbyInfo));
            yield return request.Send();
            ReturnResult(resultCallback, READY_LOBBY_PATH, request);
        }

        public IEnumerator PollLobby(string lobbyID, string playerID, ResultCallback resultCallback) {
            SortedDictionary<string, string> queryParameters = BuildPollLobbyQueryParameters(lobbyID, playerID);

            UnityWebRequest request = _requestBuilder.BuildGetRequest(POLL_LOBBY_PATH, queryParameters);
            yield return request.Send();
            ReturnResult(resultCallback, POLL_LOBBY_PATH, request);
        }

        private SortedDictionary<string, string> BuildGetLobbiesQueryParameters() { return new SortedDictionary<string, string> { }; }

        private SortedDictionary<string, string> BuildPollLobbyQueryParameters(string lobbyID, string playerID) {
            return new SortedDictionary<string, string> {
                { LOBBY_ID_FIELD, lobbyID },
                { PLAYER_ID_FIELD, playerID }
            };
        }
    }
}