using System;

namespace CoreGame.Models.API.GameService {
    [Serializable]
    public class PollRequest {
        public string GameId;
        public string PlayerId;

        public PollRequest(string gameId, string playerId) {
            GameId = gameId;
            PlayerId = playerId;
        }
    }
}
