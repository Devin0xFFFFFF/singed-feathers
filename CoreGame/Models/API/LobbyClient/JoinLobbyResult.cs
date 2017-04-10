using System;

namespace CoreGame.Models.API.LobbyClient {
    [Serializable]
    public class JoinLobbyResult : AResult {
        private static int LOBBY_FULL_CODE = 1;
        private static int ALREADY_IN_LOBBY_CODE = 2;

        public bool IsLobbyFull() { return ResultCode == LOBBY_FULL_CODE; }

        public bool IsAlreadyInLobby() { return ResultCode == ALREADY_IN_LOBBY_CODE; }
    }
}