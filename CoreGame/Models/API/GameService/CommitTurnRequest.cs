using System;

namespace CoreGame.Models.API.GameService {
    [Serializable]
    public class CommitTurnRequest {
        public string GameId;
        public string PlayerId;
        public Delta Delta;

        public CommitTurnRequest(string gameId, string playerId, Delta delta) {
            GameId = gameId;
            PlayerId = playerId;
            Delta = delta;
        }
    }
}
