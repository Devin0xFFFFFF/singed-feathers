using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;

namespace Assets.Scripts.Views {
    public class InputView : MonoBehaviour {
        public const string TURN_COUNT_STRING = "Turns Left: ";
        public const string SIDE_CHOSEN_STRING = "You have chosen to {0} the pigeons.";
        public const string NO_SIDE_CHOSEN_STRING = "You have not chosen a side.";
        public Canvas GameHUD;
        public Canvas GameMenu;
        public Button FireButton;
        public Button WaterButton;
        public Button UndoButton;
        public Button EndTurnButton;
        public Button BackButton;
        public Button HowToPlayButton;
        public Image InputImage;
        public Sprite Fire;
        public Sprite Water;
        public GameObject ControlBorderRed;
        public GameObject ControlBorderBlue;
        public Text TurnCountText;
        public Text SideChosenText;
        public Text OptionsText;
        public Text GameOverText;
        public Text GameOverStatusText;
        private Button[] _actionButtons;
        private ITurnController _turnController;
        private ITurnResolver _turnResolver;
        private Dictionary<Vector3, GameObject> _borders;
        private GameView _gameView;

        // Use this for initialization
        public void Start() { 
            _actionButtons = new Button[] { FireButton, WaterButton };
            _borders = new Dictionary<Vector3, GameObject>();
            _gameView = GetComponent<GameView>();
        }

        public void ClearSelected() { 
            foreach (GameObject border in _borders.Values) {
                Destroy(border.gameObject);
            }
            _borders = new Dictionary<Vector3, GameObject>();
        }

        // Update is called once per frame
        public void Update() {
            if (_turnController == null || !_turnResolver.IsTurnResolved()) {
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
            EndTurnButton.interactable = false;

            // GameMenu UI elements
            BackButton.gameObject.SetActive(false);
            GameOverText.gameObject.SetActive(false);
            GameOverStatusText.gameObject.SetActive(false);
            OptionsText.gameObject.SetActive(false);
        }

        public void UpdateButtons() {
            if (_turnController.HasTurnsLeft()) {
                foreach (Button button in _actionButtons) {
                    button.interactable = true;
                }
            } else {
                foreach (Button button in _actionButtons) {
                    button.interactable = false;
                }
            }

            // GameHUD UI elements
            UndoButton.interactable = _turnController.HasQueuedAction();
            EndTurnButton.interactable = _turnResolver.IsTurnResolved() && _turnController.HasTurnsLeft();

            // GameMenu UI elements
            BackButton.gameObject.SetActive(_turnController.HasTurnsLeft());
            GameOverText.gameObject.SetActive(!_turnController.HasTurnsLeft());
            GameOverStatusText.gameObject.SetActive(!_turnController.HasTurnsLeft());
            OptionsText.gameObject.SetActive(_turnController.HasTurnsLeft());
            HowToPlayButton.gameObject.SetActive(_turnController.HasTurnsLeft());

            if (!_turnController.HasTurnsLeft()) {
                GameHUD.gameObject.SetActive(false);
                GameMenu.gameObject.SetActive(true);
                GameOverStatusText.text = _gameView.GetGameOverPlayerStatus();
            }
        }

        public void UpdateImage() {
            switch (_turnController.GetMoveType()) {
                case MoveType.Fire:
                    InputImage.sprite = Fire;
                    break;
                case MoveType.Water:
                    InputImage.sprite = Water;
                    break;
            }
        }

        public void UpdateSideChosenText(string side) {
            if (side.Equals("not chosen")) {
                SideChosenText.text = NO_SIDE_CHOSEN_STRING;
            }
            else {
                SideChosenText.text = string.Format(SIDE_CHOSEN_STRING, side);
            }
        }

        public void UpdateTurnCountText() { TurnCountText.text = TURN_COUNT_STRING + _turnController.GetTurnsLeft(); }

        public void HandleMapInput(TileView tileManager) { 
            Vector3 position = tileManager.gameObject.transform.position;

            if (_turnController.ProcessAction(tileManager.GetTileController())) {
                ClearSelected();
                CreateBorder(position);
            }
        }

        public void SetTurnController(ITurnController turnController) { _turnController = turnController; }

        public void SetTurnResolver(ITurnResolver turnResolver) { _turnResolver = turnResolver; }

        private void CreateBorder(Vector3 pos) {
            GameObject border = null; 
            _borders.TryGetValue(pos, out border);
            if (border == null) {
                switch (_turnController.GetMoveType()) {
                    case MoveType.Water:
                        border = Instantiate(ControlBorderBlue, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity);
                        break;
                    case MoveType.Fire:
                        border = Instantiate(ControlBorderRed, new Vector3(pos.x, pos.y, pos.z), Quaternion.identity);
                        break;
                }
            }
            _borders.Add(pos, border);
        }

        private void RemoveBorder(Vector3 pos){
            GameObject border = null;
            _borders.TryGetValue(pos, out border);
            if (border != null) {
                Destroy(border.gameObject);
            }
            _borders.Remove(pos);
        }
    }
}