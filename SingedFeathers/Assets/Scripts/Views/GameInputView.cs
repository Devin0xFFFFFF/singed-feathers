using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;

namespace Assets.Scripts.Views {
    public class GameInputView : InputView {
        public const string TURN_COUNT_STRING = "Turns Left: ";
        public const string SIDE_CHOSEN_STRING = "You have chosen to {0} the pigeons.";
        public Canvas GameHUD;
        public Canvas GameMenu;
        public Button FireButton;
        public Button WaterButton;
        public Button UndoButton;
        public Button EndTurnButton;
        public Button BackButton;
        public Button HowToPlayButton;
        public Image InputImage;
        public GameObject ControlBorderRed;
        public GameObject ControlBorderBlue;
        public Text TurnCountText;
        public Text SideChosenText;
        public Text OptionsText;
        public Text GameOverText;
        public Text GameOverStatusText;
        public GameObject WaitingPanel;
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
                SetWaitingPanel(true);
                return;
            }
            SetWaitingPanel(false);
            UpdateButtons();
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

        public bool IsGameOver() { return !_turnController.HasTurnsLeft() || _gameView.IsGameOver(); }

        public void UpdateButtons() {
            bool isGameOver = IsGameOver();

            if (!isGameOver) {
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
            EndTurnButton.interactable = _turnResolver.IsTurnResolved() && !isGameOver;

            // GameMenu UI elements
            BackButton.gameObject.SetActive(!isGameOver);
            GameOverText.gameObject.SetActive(isGameOver);
            GameOverStatusText.gameObject.SetActive(isGameOver);
            OptionsText.gameObject.SetActive(!isGameOver);
            HowToPlayButton.gameObject.SetActive(!isGameOver);

            if (isGameOver) {
                GameHUD.gameObject.SetActive(false);
                GameMenu.gameObject.SetActive(true);
                GameOverStatusText.text = _gameView.GetGameOverPlayerStatus();
            }
        }
        
        public void UpdateSideChosenText(string side) { SideChosenText.text = string.Format(SIDE_CHOSEN_STRING, side); }

        public void UpdateTurnCountText() { TurnCountText.text = TURN_COUNT_STRING + _turnController.GetTurnsLeft(); }

        public override void HandleMapInput(TileView tileManager) { 
            Vector3 position = tileManager.gameObject.transform.position;

            if (GameHUD.gameObject.activeInHierarchy && 
                _turnController.ProcessAction(tileManager.GetTileController()) &&
                _turnResolver.IsTurnResolved()) {
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
            border.transform.localScale = new Vector3(1.6f, 1.6f, transform.localScale.z);
            _borders.Add(pos, border);
        }

        private void RemoveBorder(Vector3 pos) {
            GameObject border = null;
            _borders.TryGetValue(pos, out border);
            if (border != null) {
                Destroy(border.gameObject);
            }
            _borders.Remove(pos);
        }

        private void SetWaitingPanel(bool isActive) { WaitingPanel.SetActive(isActive); }
    }
}