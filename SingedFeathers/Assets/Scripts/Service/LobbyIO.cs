using CoreGame.Models.API;
using CoreGame.Models.API.LobbyClient;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Service {
    class LobbyIO : APersistenceIO {
        private LobbyClient _client;

        public delegate void CreateLobbyCallback(string lobbyID);
        public delegate void GetLobbiesCallback(List<LobbyInfo> lobbies);
        public delegate void ResultInfoCallback(ResultInfo result);
        public delegate void PollLobbyCallback(PollLobbyResult pollResult);

        public LobbyIO() { _client = new LobbyClient(); }

        public IEnumerator CreateLobby(CreateLobbyInfo lobbyInfo, CreateLobbyCallback callback) {
            yield return _client.CreateLobby(lobbyInfo, delegate (ClientResult result) {
                if(IsValidResult(result)) {
                    string lobbyID = result.ResponseBody;
                    Console.WriteLine("Lobby created: " + lobbyID);
                    callback(lobbyID);
                } else {
                    Console.WriteLine("Failed to create lobby: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                    callback(null);
                }
            });
        }

        public IEnumerator GetLobbies(GetLobbiesCallback callback) {
            yield return _client.GetLobbies(delegate (ClientResult result) {
                if (IsValidResult(result)) {
                    GetLobbiesResult deserializedResult = JsonConvert.DeserializeObject<GetLobbiesResult>(result.ResponseBody, _jsonSettings);
                    Console.WriteLine("Lobbies fetched from server: " + deserializedResult.Lobbies);
                    callback(deserializedResult.Lobbies);
                } else {
                    Console.WriteLine("Failed to fetch lobbies: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                    callback(null);
                }
            });
        }

        public IEnumerator JoinLobby(JoinLobbyInfo lobbyInfo, ResultInfoCallback callback) {
            yield return _client.JoinLobby(lobbyInfo, delegate(ClientResult result) {
                if (IsValidResult(result)) {
                    ResultInfo deserializedResult = JsonConvert.DeserializeObject<ResultInfo>(result.ResponseBody, _jsonSettings);
                    Console.WriteLine("JoinLobby result from server: " + deserializedResult.ResultCode + " " + deserializedResult.ResultMessage);
                    callback(deserializedResult);
                } else {
                    Console.WriteLine("Failed to join lobby: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                    callback(null);
                }
            });
        }

        public IEnumerator LeaveLobby(LeaveLobbyInfo lobbyInfo, ResultInfoCallback callback) {
            yield return _client.LeaveLobby(lobbyInfo, delegate (ClientResult result) {
                if (IsValidResult(result)) {
                    ResultInfo deserializedResult = JsonConvert.DeserializeObject<ResultInfo>(result.ResponseBody, _jsonSettings);
                    Console.WriteLine("LeaveLobby result from server: " + deserializedResult.ResultCode + " " + deserializedResult.ResultMessage);
                    callback(deserializedResult);
                } else {
                    Console.WriteLine("Failed to leave lobby: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                    callback(null);
                }
            });
        }

        public IEnumerator ReadyLobby(ReadyLobbyInfo lobbyInfo, ResultInfoCallback callback) {
            yield return _client.ReadyLobby(lobbyInfo, delegate (ClientResult result) {
                if (IsValidResult(result)) {
                    ResultInfo deserializedResult = JsonConvert.DeserializeObject<ResultInfo>(result.ResponseBody, _jsonSettings);
                    Console.WriteLine("ReadyLobby result from server: " + deserializedResult.ResultCode + " " + deserializedResult.ResultMessage);
                    callback(deserializedResult);
                } else {
                    Console.WriteLine("Failed to set ready in lobby: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                    callback(null);
                }
            });
        }

        public IEnumerator PollLobby(string lobbyID, string playerID, PollLobbyCallback callback) {
            yield return _client.PollLobby(lobbyID, playerID, delegate (ClientResult result) {
                if (IsValidResult(result)) {
                    PollLobbyResult pollResult = JsonConvert.DeserializeObject<PollLobbyResult>(result.ResponseBody, _jsonSettings);
                    Console.WriteLine("Polled lobby from server: " + pollResult.ResultCode + " " + pollResult.ResultMessage);
                    callback(pollResult);
                } else {
                    Console.WriteLine("Failed to poll lobby: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                    callback(null);
                }
            });
        }

        [Serializable]
        private class GetLobbiesResult {
            public List<LobbyInfo> Lobbies;

            public GetLobbiesResult(List<LobbyInfo> lobbies) { Lobbies = lobbies; }
        }
    }
}
