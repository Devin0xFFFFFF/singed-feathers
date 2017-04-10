using System.Collections.Generic;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Utility;

namespace CoreGame.Controllers {
    public class LocalTurnResolver : ITurnResolver {

        public LocalTurnResolver() { }

        public bool IsTurnResolved() { return true; }

        public bool ShouldPoll() { return false; }

        public void Poll(Map map, Player player) {}

        public void ResolveTurn(Delta delta, Map map, Player player) {
            List<Delta> deltaList = new List<Delta>();
            if (delta != null) {
                deltaList.Add(delta);
            }
            TurnResolveUtility.ApplyDelta(deltaList, map);
        }
    }
}