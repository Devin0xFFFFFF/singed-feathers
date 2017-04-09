using Assets.Scripts.Service.Client;
using CoreGame.Models.API.LobbyClient;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Service.IO {
    public class LobbyIO : APersistenceIO {
        private ILobbyClient _client;

        public delegate void CreateLobbyCallback(string lobbyID);
        public delegate void GetLobbiesCallback(List<LobbyInfo> lobbies);
        public delegate void JoinLobbyCallback(JoinLobbyResult result);
        public delegate void LeaveLobbyCallback(LeaveLobbyResult result);
        public delegate void ReadyLobbyCallback(ReadyLobbyResult result);
        public delegate void PollLobbyCallback(PollLobbyResult result);

        public LobbyIO(ILobbyClient client = null) { _client = client ?? new LobbyClient(); }

        public IEnumerator CreateLobby(CreateLobbyInfo lobbyInfo, CreateLobbyCallback callback) {
            yield return _client.CreateLobby(lobbyInfo, delegate (ClientResult result) {
                callback(ParseCreateLobbyResult(result));
            });
        }

        public IEnumerator GetLobbies(GetLobbiesCallback callback) {
            yield return _client.GetLobbies(delegate (ClientResult result) {
                callback(ParseGetLobbiesResult(result));
            });
        }

        public IEnumerator JoinLobby(JoinLobbyInfo lobbyInfo, JoinLobbyCallback callback) {
            yield return _client.JoinLobby(lobbyInfo, delegate(ClientResult result) {
                callback(ParseJoinLobbyResult(result));
            });
        }

        public IEnumerator LeaveLobby(LeaveLobbyInfo lobbyInfo, LeaveLobbyCallback callback) {
            yield return _client.LeaveLobby(lobbyInfo, delegate (ClientResult result) {
                callback(ParseLeaveLobbyResult(result));
            });
        }

        public IEnumerator ReadyLobby(ReadyLobbyInfo lobbyInfo, ReadyLobbyCallback callback) {
            yield return _client.ReadyLobby(lobbyInfo, delegate (ClientResult result) {
                callback(ParseReadyLobbyResult(result));
            });
        }

        public IEnumerator PollLobby(string lobbyID, string playerID, PollLobbyCallback callback) {
            yield return _client.PollLobby(lobbyID, playerID, delegate (ClientResult result) {
                callback(ParsePollLobbyResult(result));
            });
        }

        public string ParseCreateLobbyResult(ClientResult result) {
            if (IsValidResult(result)) {
                string lobbyID = result.ResponseBody;
                Console.WriteLine("Lobby created: " + lobbyID);
                return lobbyID;
            } else {
                Console.WriteLine("Failed to create lobby: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                return null;
            }
        }

        public List<LobbyInfo> ParseGetLobbiesResult(ClientResult result) {
            if (IsValidResult(result)) {
                GetLobbiesResult deserializedResult = JsonConvert.DeserializeObject<GetLobbiesResult>(result.ResponseBody, _jsonSettings);
                Console.WriteLine("Lobbies fetched from server: " + deserializedResult.Lobbies);
                return deserializedResult.Lobbies;
            } else {
                Console.WriteLine("Failed to fetch lobbies: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                return null;
            }
        }

        public JoinLobbyResult ParseJoinLobbyResult(ClientResult result) {
            if (IsValidResult(result)) {
                JoinLobbyResult deserializedResult = JsonConvert.DeserializeObject<JoinLobbyResult>(result.ResponseBody, _jsonSettings);
                Console.WriteLine("JoinLobby result from server: " + deserializedResult.ResultCode + " " + deserializedResult.ResultMessage);
                return deserializedResult;
            }  else {
                Console.WriteLine("Failed to join lobby: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                return null;
            }
        }

        public LeaveLobbyResult ParseLeaveLobbyResult(ClientResult result) {
            if (IsValidResult(result)) {
                LeaveLobbyResult deserializedResult = JsonConvert.DeserializeObject<LeaveLobbyResult>(result.ResponseBody, _jsonSettings);
                Console.WriteLine("LeaveLobby result from server: " + deserializedResult.ResultCode + " " + deserializedResult.ResultMessage);
                return deserializedResult;
            } else {
                Console.WriteLine("Failed to leave lobby: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                return null;
            }
        }

        public ReadyLobbyResult ParseReadyLobbyResult(ClientResult result) {
            if (IsValidResult(result)) {
                ReadyLobbyResult deserializedResult = JsonConvert.DeserializeObject<ReadyLobbyResult>(result.ResponseBody, _jsonSettings);
                Console.WriteLine("ReadyLobby result from server: " + deserializedResult.ResultCode + " " + deserializedResult.ResultMessage);
                return deserializedResult;
            } else {
                Console.WriteLine("Failed to set ready in lobby: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                return null;
            }
        }

        public PollLobbyResult ParsePollLobbyResult(ClientResult result) {
            if (IsValidResult(result)) {
                PollLobbyResult pollResult = JsonConvert.DeserializeObject<PollLobbyResult>(result.ResponseBody, _jsonSettings);
                Console.WriteLine("Polled lobby from server: " + pollResult.ResultCode + " " + pollResult.ResultMessage);
                return pollResult;
            } else {
                Console.WriteLine("Failed to poll lobby: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                return null;
            }
        }

        [Serializable]
        private class GetLobbiesResult {
            public List<LobbyInfo> Lobbies;

            public GetLobbiesResult(List<LobbyInfo> lobbies) { Lobbies = lobbies; }
        }
    }
}
