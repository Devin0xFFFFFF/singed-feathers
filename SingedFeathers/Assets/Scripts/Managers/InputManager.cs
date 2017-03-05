using Assets.Scripts.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Managers {
    public class InputManager : MonoBehaviour {
        public const string TURN_COUNT_STRING = "Turns Left: ";
        private ITurnController _TurnController;
        private ITurnResolver _TurnResolver;
        public MapManager MapManager;
        public Button FireButton;
        public Button WaterButton;
        public Button BlankButton;
        public Button UndoButton;
        public Button EndTurnButton;
        private Button[] ActionButtons;
        public Text TurnCountText;

        // Use this for initialization
        public void Start() {
            ActionButtons = new Button[] { FireButton, WaterButton };
        }

        // Update is called once per frame
        public void Update() {
            Initialize();
            UpdateButtons();
            UpdateTurnCountText();
        }

        private void Initialize() {
            if (_TurnController == null) {
                _TurnController = MapManager.GetTurnController();
                _TurnResolver = MapManager.GetTurnResolver();
            }
        }

        public void UpdateButtons() {
            if (_TurnController.CanTakeAction()) {
                foreach (Button button in ActionButtons) {
                    button.interactable = true;
                }
            } else {
                foreach (Button button in ActionButtons) {
                    button.interactable = false;
                }
            }

            UndoButton.interactable = _TurnController.HasQueuedActions();

            BlankButton.interactable = _TurnResolver.IsTurnResolved() && _TurnController.HasTurnsLeft();
            EndTurnButton.interactable = _TurnResolver.IsTurnResolved() && _TurnController.HasTurnsLeft();
        }

        public void UpdateTurnCountText() { TurnCountText.text = TURN_COUNT_STRING + _TurnController.GetTurnsLeft(); }

        public void HandleMapInput(TileManager tileManager) { _TurnController.ProcessAction(tileManager.GetTileController()); }

        public void SetTurnController(ITurnController turnController) { _TurnController = turnController; }

        public void SetTurnResolver(ITurnResolver turnResolver) { _TurnResolver = turnResolver; }
    }
}