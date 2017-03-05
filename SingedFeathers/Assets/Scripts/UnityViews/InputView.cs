using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Managers {
    public class InputView : MonoBehaviour {
        public const string TURN_COUNT_STRING = "Turns Left: ";
        private ITurnController _TurnController;
        private ITurnResolver _TurnResolver;
        public Button FireButton;
        public Button WaterButton;
        public Button BlankButton;
        public Button UndoButton;
        public Button EndTurnButton;
        public Image InputImage;
        public Sprite Fire;
        public Sprite Water;
        public Sprite Blank;
        private Button[] _ActionButtons;
        public Text TurnCountText;

        // Use this for initialization
        public void Start() {
            _ActionButtons = new Button[] { FireButton, WaterButton };
        }

        // Update is called once per frame
        public void Update() {
            UpdateButtons();
            UpdateImage();
            UpdateTurnCountText();
        }

        public void UpdateButtons() {
            if (_TurnController.CanTakeAction()) {
                foreach (Button button in _ActionButtons) {
                    button.interactable = true;
                }
            } else {
                foreach (Button button in _ActionButtons) {
                    button.interactable = false;
                }
            }

            UndoButton.interactable = _TurnController.HasQueuedActions();

            BlankButton.interactable = _TurnResolver.IsTurnResolved() && _TurnController.HasTurnsLeft();
            EndTurnButton.interactable = _TurnResolver.IsTurnResolved() && _TurnController.HasTurnsLeft();
        }

        public void UpdateImage() {
            switch (_TurnController.GetMoveType()) {
                case MoveTypes.Cancel:
                    InputImage.sprite = Blank;
                    break;
                case MoveTypes.Fire:
                    InputImage.sprite = Fire;
                    break;
                case MoveTypes.Water:
                    InputImage.sprite = Water;
                    break;
            }
        }

        public void UpdateTurnCountText() { TurnCountText.text = TURN_COUNT_STRING + _TurnController.GetTurnsLeft(); }

        public void HandleMapInput(TileView tileManager) { _TurnController.ProcessAction(tileManager.GetTileController()); }

        public void SetTurnController(ITurnController turnController) { _TurnController = turnController; }

        public void SetTurnResolver(ITurnResolver turnResolver) { _TurnResolver = turnResolver; }
    }
}