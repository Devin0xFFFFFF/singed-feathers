using UnityEngine;
using Assets.Scripts.Service;
using CoreGame.Models.API.LobbyClient;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Scripts.Input {
    public class LobbySelectionInput : MonoBehaviour {
        public GameObject LobbySelectButton;
        private LobbyIO _lobbyIO;
        private List<GameObject> _buttons;

        public void Start() {
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
                    tempButton.GetComponentInChildren<Text>().text = "Lobby Name: "+ lobby.LobbyName +"\n Map ID: "
                        + lobby.MapID ;
                    
                    tempButton.onClick.AddListener(delegate { SelectLobby(lobby.MapID); });

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
        public void SelectLobby(string mapID) { 
            PlayerPrefs.SetString("MapID", mapID);
            //TODO: figure out what to call to get into a lobby
        }
    }
}
