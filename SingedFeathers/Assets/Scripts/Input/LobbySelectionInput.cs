using UnityEngine;
using Assets.Scripts.Service;
using CoreGame.Models.API.LobbyClient;
using System.Collections.Generic;
using UnityEngine.UI;
using CoreGame.Models;
using Assets.Scripts;
using Assets.Scripts.Utility;

namespace Assets.Scripts.Input {
    public class LobbySelectionInput : MonoBehaviour {
        public GameObject LobbySelectButton;
        public GameSelection SceneSelector;
        private LobbyIO _lobbyIO;
        private List<GameObject> _buttons;

        public void Start() {
            UnitySystemConsoleRedirector.Redirect();
            _lobbyIO = new LobbyIO();
            _buttons = new List<GameObject>();
            StartCoroutine(_lobbyIO.GetLobbies(delegate(List<LobbyInfo> lobbies) {
                if (lobbies == null) {
                    Debug.LogError("Failed to retrieve lobbies.");
                    return;
                }

                Debug.Log("Lobbies fetched from server");

                foreach (LobbyInfo lobby in lobbies) {
                    GameObject lobbyButton = Instantiate(LobbySelectButton);
                    Button tempButton = lobbyButton.GetComponent<Button>();
                    string text = "Lobby Name: " + lobby.LobbyName + "\t Map Name: " + lobby.MapName;
                    PlayerSideSelection side = PlayerSideSelection.SavePigeons;
                    if (lobby.Players.Count>0) {
                        text = text + "\n Host: " + lobby.Players[0].PlayerName;
                        if (lobby.Players[0].PlayerSideSelection == PlayerSideSelection.SavePigeons){
                            side = PlayerSideSelection.BurnPigeons;
                        }
                    }
                    if (lobby.NumPlayers == lobby.Players.Count) {
                        text = text + "\t LOBBY IS FULL";
                    }
                    tempButton.GetComponentInChildren<Text>().text = text;
                    
                    tempButton.onClick.AddListener(delegate { SelectLobby(lobby.MapID, lobby.LobbyID, side); });

                    lobbyButton.transform.SetParent(this.GetComponent<RectTransform>());
                    _buttons.Add(lobbyButton);
                }
            }));
        }

        public void Refresh() {
            foreach (GameObject button in _buttons) {
                Destroy(button.gameObject);
            }
            Start();
        }
        public void SelectLobby(string mapID, string lobbyID, PlayerSideSelection side) { 
            PlayerPrefs.SetString("MapID", mapID);
            PlayerPrefs.SetInt("Side", (int)side);

            JoinLobbyInfo joinlobby = new JoinLobbyInfo();
            joinlobby.JoinPlayer = new Player(PlayerPrefs.GetString("PlayerID"), PlayerPrefs.GetString("PlayerName", "AnonPlayer"), side);
            joinlobby.LobbyID = lobbyID;
            Debug.Log("Attempting to join " + lobbyID);
            StartCoroutine(_lobbyIO.JoinLobby(joinlobby, delegate(JoinLobbyResult result) {
                Debug.Log(result.ResultMessage);
                if (result.IsSuccess()){
                    SceneSelector.LoadScene("GameScene");
                }
            }));

        }
    }
}
