using System.Collections;
using CoreGame.Models.API.LobbyClient;

namespace Assets.Scripts.Service.Client {
    public interface ILobbyClient {
        IEnumerator CreateLobby(CreateLobbyInfo lobbyInfo, APersistenceClient.ResultCallback resultCallback);

        IEnumerator GetLobbies(APersistenceClient.ResultCallback resultCallback);

        IEnumerator JoinLobby(JoinLobbyInfo lobbyInfo, APersistenceClient.ResultCallback resultCallback);

        IEnumerator LeaveLobby(LeaveLobbyInfo lobbyInfo, APersistenceClient.ResultCallback resultCallback);

        IEnumerator ReadyLobby(ReadyLobbyInfo lobbyInfo, APersistenceClient.ResultCallback resultCallback);

        IEnumerator PollLobby(string lobbyID, string playerID, APersistenceClient.ResultCallback resultCallback);
    }
}