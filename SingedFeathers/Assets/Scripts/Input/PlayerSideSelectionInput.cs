﻿using CoreGame.Models;
using Assets.Scripts.Service;
using CoreGame.Models.API.LobbyClient;
using UnityEngine;

namespace Assets.Scripts.Input {
    public class PlayerSideSelectionInput : MonoBehaviour {
        public void ChooseToSavePigeons() { 
            PlayerPrefs.SetInt("Side", (int)PlayerSideSelection.SavePigeons);
            CreateLobby();
        }

        public void ChooseToBurnPigeons() { 
            PlayerPrefs.SetInt("Side", (int)PlayerSideSelection.BurnPigeons);
            CreateLobby();
        }

        private void CreateLobby(){
            LobbyIO lobbyIO = new LobbyIO();
            CreateLobbyInfo lobbyInfo = new CreateLobbyInfo();
            lobbyInfo.HostPlayer = new Player(PlayerPrefs.GetString("PlayerID"), PlayerPrefs.GetString("PlayerName", "AnonPlayer"), (PlayerSideSelection)PlayerPrefs.GetInt("Side"));
            lobbyInfo.IsPublic = (PlayerPrefs.GetInt("IsPublic") == 1)? true : false;
            lobbyInfo.LobbyName = PlayerPrefs.GetString("LobbyName", "AnonLobby");
            lobbyInfo.MapID = PlayerPrefs.GetString("MapID");
            lobbyInfo.NumPlayers = PlayerPrefs.GetInt("NumPlayers");
            Debug.Log("Creating Lobby\n" + "Public:"+lobbyInfo.IsPublic + ", "+ lobbyInfo.NumPlayers + "\n"+ lobbyInfo.LobbyName + "\n" + PlayerPrefs.GetString("PlayerID") + "\n" +lobbyInfo.MapID );
            StartCoroutine(lobbyIO.CreateLobby(lobbyInfo, delegate(string lobbyID) {
                PlayerPrefs.SetString("LobbyID", lobbyID);
                Debug.Log(lobbyID == null);
                Debug.Log(lobbyID);
            }));
        }
    }

}



