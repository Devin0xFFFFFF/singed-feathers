using System.Collections.Generic;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Models.Commands;
using CoreGame.Utility;
using Newtonsoft.Json;

namespace CoreGame.Controllers {
    public class LocalTurnResolver : ITurnResolver {
        private readonly JsonSerializerSettings _settings;

        public LocalTurnResolver() {
            _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
        }

        private bool _isTurnResolved = true;

        public bool IsTurnResolved() { return _isTurnResolved; }

        public bool ShouldPoll() { return false; }

        public void Poll(Map map) {}

        public void ResolveTurn(Delta delta, Map map, Player player) {
            _isTurnResolved = false;
            List<Delta> deltaList = new List<Delta>();
            if (delta != null) {
                deltaList.Add(delta);
            }

            TurnResolveUtility.ApplyDelta(deltaList, map);
        }
    }
}
