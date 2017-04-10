using System;

namespace CoreGame.Models.API.LobbyClient {
    [Serializable]
    public class ReadyLobbyResult : AResult {
        private static int NOT_IN_LOBBY_CODE = 1;
        private static int GAME_STARTED_CODE = 2;

        public bool IsNotInLobby() { return ResultCode == NOT_IN_LOBBY_CODE; }

        public bool IsGameStarted() { return ResultCode == GAME_STARTED_CODE; }
    }
}