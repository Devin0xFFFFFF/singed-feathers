using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CoreGame.Models;

namespace Assets.Scripts.Input {
	public class SetPlayerPrefsInput : MonoBehaviour {
		// Use this for initialization
		void Start() {}
		
		// Update is called once per frame
		void Update() {}

	    public void SetPlayerName(string name) { 
			PlayerPrefs.SetString("PlayerName", name); 
			Debug.Log("Player name set to: " + name);
		}
	        
	    public void SetPlayerID() { 
	        string id = Player.GeneratePlayerID();
	        PlayerPrefs.SetString("PlayerID", id); 
	    }

	    public void SetSinglePlayer() {
	        PlayerPrefs.SetInt("IsPublic", 0);
	        PlayerPrefs.SetInt("NumPlayers", 1);
	    }

	    public void SetMultiPlayer() {
	        PlayerPrefs.SetInt("IsPublic", 1);
	        PlayerPrefs.SetInt("NumPlayers", 2);
	    }
	}
}
