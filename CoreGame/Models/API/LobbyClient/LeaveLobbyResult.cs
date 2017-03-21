using Newtonsoft.Json;
using System;

namespace CoreGame.Models.API.LobbyClient {
    [Serializable]
    public class LeaveLobbyResult : AResult {
        private static int NOT_IN_LOBBY_CODE = 1;

        public bool IsNotInLobby() { return ResultCode == NOT_IN_LOBBY_CODE; }
    }
}