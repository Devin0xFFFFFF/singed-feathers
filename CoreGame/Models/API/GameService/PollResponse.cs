using System;
using System.Collections.Generic;

namespace CoreGame.Models.API.GameService {
    [Serializable]
    public class PollResponse {
        public bool IsValid;
        public List<Delta> Turn;

        public PollResponse(bool isValid, List<Delta> turn) {
            IsValid = isValid;
            Turn = turn;
        }
    }
}