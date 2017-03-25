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
        private List<TileView> _tileSet;
        private MapMakerView _mapMakerView;

        void Start() {
            _mapMakerView = GetComponent<MapMakerView>();
            _tileSet = _mapMakerView.TileSet;
            InitializeTileButtons();
        }

        void Update() {

                 
        }

        public override void HandleMapInput(TileView tileManager) { 
            Vector3 position = tileManager.gameObject.transform.position;

            //based on selection, do stuff to the tile
        }

        private void InitializeTileButtons() {
            foreach(TileView tile in _tileSet) {
                Button newButton = Instantiate(TileButtonBase);
                newButton.GetComponent<Image>().sprite = tile.GetComponent<SpriteRenderer>().sprite;
                newButton.transform.SetParent(TileButtonContainer.GetComponent<RectTransform>());
            }
        }
    }
}
