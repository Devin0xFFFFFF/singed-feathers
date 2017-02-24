using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayGUI : MonoBehaviour {

	public GameStateManager gsm;
	public Button fireButton;
	public Button undoButton;
	public Button endTurnButton;

	private IGameState currentState;

	// Use this for initialization
	void Start () {
		currentState = gsm.currState;
	}
	
	// Update is called once per frame
	void Update () {
		currentState = gsm.currState;
		UpdateButtons ();
	}

	void UpdateButtons() {
		if (currentState is UnselectedActionState) {
			fireButton.interactable = true;
			undoButton.interactable = false;
			endTurnButton.interactable = false;
		} else if (currentState is SelectedActionState) {
			fireButton.interactable = false;
			undoButton.interactable = false;
			endTurnButton.interactable = false;
		} else if (currentState is AppliedActionState) {
			fireButton.interactable = false;
			undoButton.interactable = true;
			endTurnButton.interactable = true;
		} else if (currentState is ResolveState) {
			fireButton.interactable = false;
			undoButton.interactable = false;
			endTurnButton.interactable = false;
		} 
	}
}
