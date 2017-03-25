using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Views {
    public class MapMakerInputView : InputView {

        public Button AddFireButton;
        public Button AddPigeonButton;
        public GameObject TileSet;
        private MapMakerView _mapMakerView;

        void Start() {
            _mapMakerView = GetComponent<MapMakerView>();
        }

        public override void HandleMapInput(TileView tileManager) { 
            Vector3 position = tileManager.gameObject.transform.position;

            //based on selection, do stuff to the tile
        }
    }
}
