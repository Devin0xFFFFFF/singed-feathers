using CoreGame.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Views {
    public class MapMakerInputView : InputView {

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
        private bool _setFire;
        private bool _setPigeon;
        private bool _removePlacedObject;

        void Start() {
            _mapMakerView = GetComponent<MapMakerView>();
            _tileSet = _mapMakerView.TileSet;
            InitializeTileButtons();
            UpdateNumberOfTurns();
        }

        public override void HandleMapInput(TileView tileManager) { 
            if (_selectedTileType != TileType.Error) {
                _mapMakerView.UpdateTileType(_selectedTileType, tileManager);
            }
            else if (_setFire) {
                _mapMakerView.SetFire(tileManager);
            }
            else if (_setPigeon) {
                _mapMakerView.SetPigeon(tileManager);
            }
            else if (_removePlacedObject) {
                _mapMakerView.ResetTile(tileManager);
            }
        }

        public void SetSelectedButton(Button button) {
            if(_selectedButton != null) {
                _selectedButton.interactable = true;
            }
            _selectedTileType = TileType.Error;
            _setFire = false;
            _setPigeon = false;
            _removePlacedObject = false;
            _selectedButton = button;
            _selectedButton.interactable = false;
        }

        public void SetSelectedTileType(TileType tileType) {
            _selectedTileType = tileType;
        }

        public void SetFire() { _setFire = true; }

        public void SetPigeon() { _setPigeon = true; }

        public void SetRemove() { _removePlacedObject = true; }

        public void UpdateNumberOfTurns() { NumberOfTurnsText.text = NumberOfTurnsSlider.value.ToString(); }

        private void InitializeTileButtons() {
            foreach(TileView tile in _tileSet) {
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
