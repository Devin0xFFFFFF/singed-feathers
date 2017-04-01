using Newtonsoft.Json;
using System;

namespace CoreGame.Models.API.LobbyClient {
    [Serializable]
    public class PollLobbyResult : AResult {
        private static int NOT_IN_LOBBY_CODE = 1;
        private static int GAME_STARTED_CODE = 2;

        private static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };

        public bool IsNotInLobby() { return ResultCode == NOT_IN_LOBBY_CODE; }

        public bool IsGameStarted() { return ResultCode == GAME_STARTED_CODE; }

        public string GetGameID() {
			if (IsGameStarted ()) {
                return JsonConvert.DeserializeObject<string>(ResultMessage);
            } else {
                return null;
            }
        }
    }
}