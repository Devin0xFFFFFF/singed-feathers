using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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
        public GameObject ControlBorderRed;
        public GameObject ControlBorderBlue;
        public Text TurnCountText;
        public Text OptionsText;
        public Text GameOverText;
        private Button[] _actionButtons;
        private ITurnController _turnController;
        private ITurnResolver _turnResolver;
        private Dictionary<Vector3, GameObject> _borders;

        // Use this for initialization
        public void Start() { 
            _actionButtons = new Button[] { FireButton, WaterButton };
            _borders = new Dictionary<Vector3, GameObject>();
        }

        public void ClearSelected() { 
            foreach (GameObject border in _borders.Values) {
                Destroy(border.gameObject);
            }
            _borders = new Dictionary<Vector3, GameObject>();
        }

        // Update is called once per frame
        public void Update() {
            UpdateButtons();
            UpdateImage();
            UpdateTurnCountText();
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

        public void HandleMapInput(TileView tileManager) { 
            Vector3 position = tileManager.gameObject.transform.position;

            if (_turnController.ProcessAction(tileManager.GetTileController())) {
                createBorder(position);
            }

            if (_turnController.GetMoveType() == MoveType.Remove) {
                removeBorder(position);
            }
        }

        public void SetTurnController(ITurnController turnController) { _turnController = turnController; }

        public void SetTurnResolver(ITurnResolver turnResolver) { _turnResolver = turnResolver; }

        private void createBorder(Vector3 pos) {
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

        private void removeBorder(Vector3 pos){
            GameObject border = null;
            _borders.TryGetValue(pos, out border);
            if (border != null) {
                Destroy(border.gameObject);
            }
            _borders.Remove(pos);
        }
    }
}