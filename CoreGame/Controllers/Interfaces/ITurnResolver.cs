using CoreGame.Models;

namespace CoreGame.Controllers.Interfaces {
    public interface ITurnResolver {
        void ResolveTurn(Delta delta, Map map);
        bool IsTurnResolved();
        bool ShouldPoll();
        void Poll(Map map);
    }
}
