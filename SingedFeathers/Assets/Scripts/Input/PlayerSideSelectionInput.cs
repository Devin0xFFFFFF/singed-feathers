using CoreGame.Models;
using Assets.Scripts.Service;
using CoreGame.Models.API.LobbyClient;
using UnityEngine;
using Assets.Scripts.Utility;

namespace Assets.Scripts.Input {
    public class PlayerSideSelectionInput : MonoBehaviour {
		private static string _lobbyName = "AnonLobby";
		public LobbyIO _lobbyIO;
		public GameSelection _gameselect;

        public void Awake() {
            _lobbyIO = new LobbyIO();
        }

        public void ChooseToSavePigeons() { 
            PlayerPrefs.SetInt("Side", (int)PlayerSideSelection.SavePigeons);
            if (!SinglePlayer.IsSinglePlayer()) {
                CreateLobby(_lobbyName);
            } else {
                _gameselect.LoadScene("GameScene");
            }
        }

        public void ChooseToBurnPigeons() { 
            PlayerPrefs.SetInt("Side", (int)PlayerSideSelection.BurnPigeons);
            if (!SinglePlayer.IsSinglePlayer()) {
                CreateLobby(_lobbyName);
            } else {
                _gameselect.LoadScene("GameScene");
            }
        }

        public void SetLobbyName(string lobbyName) {
            _lobbyName = lobbyName;
        }

        private void CreateLobby(string lobbyName) {
            CreateLobbyInfo lobbyInfo = new CreateLobbyInfo();
            lobbyInfo.HostPlayer = new Player(PlayerPrefs.GetString("PlayerID"), PlayerPrefs.GetString("PlayerName", "AnonPlayer"), (PlayerSideSelection)PlayerPrefs.GetInt("Side"));
            lobbyInfo.IsPublic = (PlayerPrefs.GetInt("IsPublic") == 1)? true : false;
            lobbyInfo.LobbyName = PlayerPrefs.GetString("LobbyName", lobbyName);
            Debug.Log("Setting Lobby name to: " + lobbyName);
            lobbyInfo.MapID = PlayerPrefs.GetString("MapID");
            lobbyInfo.NumPlayers = PlayerPrefs.GetInt("NumPlayers");

            Debug.Log("Creating Lobby\n" + "Public:"+lobbyInfo.IsPublic + ", "+ lobbyInfo.NumPlayers + "\n"+ lobbyInfo.LobbyName + "\n" + PlayerPrefs.GetString("PlayerID") + "\n" +lobbyInfo.MapID );
            
            StartCoroutine(_lobbyIO.CreateLobby(lobbyInfo, delegate(string lobbyID) {
                PlayerPrefs.SetString("LobbyID", lobbyID);
                _gameselect.LoadScene("GameScene");
            }));
        }
    }

}



