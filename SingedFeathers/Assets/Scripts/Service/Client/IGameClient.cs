using System.Collections;
using CoreGame.Models.API.GameService;

namespace Assets.Scripts.Service.Client {
    public interface IGameClient {
        IEnumerator CommitTurn(CommitTurnRequest commitTurnRequest, APersistenceClient.ResultCallback resultCallback);

        IEnumerator PollGame(PollRequest pollRequest, APersistenceClient.ResultCallback resultCallback);

        IEnumerator Surrender(PollRequest pollRequest, APersistenceClient.ResultCallback resultCallback);

        IEnumerator Test(APersistenceClient.ResultCallback resultCallback);
    }
}