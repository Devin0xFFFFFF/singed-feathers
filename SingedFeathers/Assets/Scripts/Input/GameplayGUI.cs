using Assets.Scripts.States;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Input {
    public class GameplayGUI : MonoBehaviour {
        public const string TURN_COUNT_STRING = "Turn Number: ";
        public GameStateManager GameStateManager;
        public Button FireButton;
        public Button UndoButton;
        public Button EndTurnButton;
        public Text TurnCountText;
        private IGameState _currentState;

        // Use this for initialization
        public void Start() { _currentState = GameStateManager.CurrState; }
    
        // Update is called once per frame
        public void Update() {
            _currentState = GameStateManager.CurrState;
            UpdateButtons();
            UpdateTurnCountText();
        }

        public void UpdateButtons() {
            if (_currentState is UnselectedActionState) {
                FireButton.interactable = true;
                UndoButton.interactable = false;
                EndTurnButton.interactable = true;
            } else if (_currentState is SelectedActionState) {
                FireButton.interactable = false;
                UndoButton.interactable = false;
                EndTurnButton.interactable = true;
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

        public void UpdateTurnCountText() {
            int currTurn = GameStateManager.GetNumberOfTurns();
            TurnCountText.text = TURN_COUNT_STRING + currTurn;
        }
    }
}