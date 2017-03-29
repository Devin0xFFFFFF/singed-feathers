using CoreGame.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Views {
    public class MapMakerInputView : InputView {
        enum Action {
            None, SetFire, SetPigeon, RemoveObject
        }

        public Canvas UploadMapCanvas;
        public Canvas HelpCanvas;
        public Canvas MenuCanvas;
        public Button AddFireButton;
        public Button AddPigeonButton;
        public Button TileButtonBase;
        public GameObject TileButtonContainer;
        public Slider NumberOfTurnsSlider;
        public Text NumberOfTurnsText;
        private List<TileView> _tileSet;
        private MapMakerView _mapMakerView;
        private Button _selectedButton;
        private TileType _selectedTileType;
        private Action _action;

        void Start() {
            _mapMakerView = GetComponent<MapMakerView>();
            _tileSet = _mapMakerView.TileSet;
            InitializeTileButtons();
            UpdateNumberOfTurns();
        }

        public override void HandleMapInput(TileView tileManager) {
            if (!UploadMapCanvas.gameObject.activeInHierarchy && !HelpCanvas.gameObject.activeInHierarchy && !MenuCanvas.gameObject.activeInHierarchy) {
                if (_selectedTileType != TileType.Error) {
                    _mapMakerView.UpdateTileType(_selectedTileType, tileManager);
                } else {
                    switch (_action) {
                        case Action.SetFire:
                            _mapMakerView.SetFire(tileManager);
                            break;
                        case Action.SetPigeon:
                            _mapMakerView.SetPigeon(tileManager);
                            break;
                        case Action.RemoveObject:
                            _mapMakerView.ResetTile(tileManager);
                            break;
                        case Action.None:
                        default:
                            break;
                    }
                }
            }
        }

        public void SetSelectedButton(Button button) {
            if(_selectedButton != null) {
                _selectedButton.interactable = true;
            }
            _selectedTileType = TileType.Error;
            _selectedButton = button;
            _selectedButton.interactable = false;
            _action = Action.None;
        }

        public void SetSelectedTileType(TileType tileType) { _selectedTileType = tileType; }

        public void SetFire() { _action = Action.SetFire; }

        public void SetPigeon() { _action = Action.SetPigeon; }

        public void SetRemove() { _action = Action.RemoveObject; }

        public void UpdateNumberOfTurns() { NumberOfTurnsText.text = NumberOfTurnsSlider.value.ToString(); }
        
        public int GetNumTurns() { return (int)NumberOfTurnsSlider.value; }

        private void InitializeTileButtons() {
            foreach (TileView tile in _tileSet) {
                Button newButton = Instantiate(TileButtonBase);
                newButton.GetComponent<Image>().sprite = tile.GetComponent<SpriteRenderer>().sprite;
                newButton.transform.SetParent(TileButtonContainer.GetComponent<RectTransform>());
                newButton.onClick.AddListener(delegate { 
                    SetSelectedButton(newButton); 
                    SetSelectedTileType(tile.Type);
                });
            }
        }
    }
}