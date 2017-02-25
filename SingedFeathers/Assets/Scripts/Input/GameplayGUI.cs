using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayGUI : MonoBehaviour {

    public GameStateManager GameStateManager;
    public Button FireButton;
    public Button UndoButton;
    public Button EndTurnButton;

    private IGameState _currentState;

    // Use this for initialization
    void Start() { _currentState = GameStateManager.CurrState; }
    
    // Update is called once per frame
    void Update() {
        _currentState = GameStateManager.CurrState;
        UpdateButtons();
    }

    void UpdateButtons() {
        if (_currentState is UnselectedActionState) {
            FireButton.interactable = true;
            UndoButton.interactable = false;
            EndTurnButton.interactable = false;
        } else if (_currentState is SelectedActionState) {
            FireButton.interactable = false;
            UndoButton.interactable = false;
            EndTurnButton.interactable = false;
        } else if (_currentState is AppliedActionState) {
            FireButton.interactable = false;
            UndoButton.interactable = true;
            EndTurnButton.interactable = true;
        } else if (_currentState is ResolveState) {
            FireButton.interactable = false;
            UndoButton.interactable = false;
            EndTurnButton.interactable = false;
        } 
    }
}
