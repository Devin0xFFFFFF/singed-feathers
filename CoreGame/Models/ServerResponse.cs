using System;
using System.Collections.Generic;

namespace CoreGame.Models {
    [Serializable]
    public class ServerResponse {
        public bool IsValid;
        public List<Delta> Turn;

        public ServerResponse(bool isValid, List<Delta> turn) {
            IsValid = isValid;
            Turn = turn;
        }
    }
}