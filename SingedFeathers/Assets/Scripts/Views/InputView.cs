using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Views {
    public class InputView : MonoBehaviour {
        public const string TURN_COUNT_STRING = "Turns Left: ";
        public Canvas GameHUD;
        public Canvas GameMenu;
        public Button FireButton;
        public Button WaterButton;
        public Button BlankButton;
        public Button UndoButton;
        public Button EndTurnButton;
        public Button BackButton;
        public Image InputImage;
        public Sprite Fire;
        public Sprite Water;
        public Sprite Blank;
        public Text TurnCountText;
        public Text OptionsText;
        public Text GameOverText;
        private Button[] _actionButtons;
        private ITurnController _turnController;
        private ITurnResolver _turnResolver;

        // Use this for initialization
        public void Start() { _actionButtons = new Button[] { FireButton, WaterButton }; }

        // Update is called once per frame
        public void Update() {
            if (_turnController == null) {
                DisableAllButtons();
                return;
            }
            UpdateButtons();
            UpdateImage();
            UpdateTurnCountText();
        }

        public void DisableAllButtons() {
            foreach (Button button in _actionButtons) {
                button.interactable = false;
            }

            UndoButton.interactable = false;
            BlankButton.interactable = false;
            EndTurnButton.interactable = false;

            // GameMenu UI elements
            BackButton.gameObject.SetActive(false);
            GameOverText.gameObject.SetActive(false);
            OptionsText.gameObject.SetActive(false);
        }

        public void UpdateButtons() {
            if (_turnController.CanTakeAction()) {
                foreach (Button button in _actionButtons) {
                    button.interactable = true;
                }
            } else {
                foreach (Button button in _actionButtons) {
                    button.interactable = false;
                }
            }

            // GameHUD UI elements
            UndoButton.interactable = _turnController.HasQueuedActions();
            BlankButton.interactable = _turnResolver.IsTurnResolved() && _turnController.HasTurnsLeft();
            EndTurnButton.interactable = _turnResolver.IsTurnResolved() && _turnController.HasTurnsLeft();

            // GameMenu UI elements
            BackButton.gameObject.SetActive(_turnController.HasTurnsLeft());
            GameOverText.gameObject.SetActive(!_turnController.HasTurnsLeft());
            OptionsText.gameObject.SetActive(_turnController.HasTurnsLeft());

            if (!_turnController.HasTurnsLeft()) {
                GameHUD.gameObject.SetActive(false);
                GameMenu.gameObject.SetActive(true);
            }
        }

        public void UpdateImage() {
            switch (_turnController.GetMoveType()) {
                case MoveType.Remove:
                    InputImage.sprite = Blank;
                    break;
                case MoveType.Fire:
                    InputImage.sprite = Fire;
                    break;
                case MoveType.Water:
                    InputImage.sprite = Water;
                    break;
            }
        }

        public void UpdateTurnCountText() { TurnCountText.text = TURN_COUNT_STRING + _turnController.GetTurnsLeft(); }

        public void HandleMapInput(TileView tileManager) { _turnController.ProcessAction(tileManager.GetTileController()); }

        public void SetTurnController(ITurnController turnController) { _turnController = turnController; }

        public void SetTurnResolver(ITurnResolver turnResolver) { _turnResolver = turnResolver; }
    }
}