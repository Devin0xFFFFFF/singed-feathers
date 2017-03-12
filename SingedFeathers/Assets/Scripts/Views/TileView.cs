using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Views {
    public class TileView : MonoBehaviour {
        public TileType Type;
        public Sprite SpriteHeatLevel0;
        public Sprite SpriteHeatLevel1;
        public Sprite SpriteHeatLevel2;
        public Sprite SpriteHeatLevel3;
        private Sprite[] _heatLevelSprites;
        private ITileController _tileController;
        private SpriteRenderer _spriteRenderer;
        private const int HEAT_PER_INDEX = 5;

        public void Start() {
            _heatLevelSprites = new Sprite[] { SpriteHeatLevel0, SpriteHeatLevel1, SpriteHeatLevel2, SpriteHeatLevel3 };
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        }


        // Update is called once per frame
        public void Update() {
            Transform child = transform.GetChild(0);
            child.gameObject.SetActive(_tileController.IsOnFire());
           // if (_tileController.StateHasChanged) {
                int spriteIndex = _tileController.GetTileHeat() / HEAT_PER_INDEX;
                if (spriteIndex < _heatLevelSprites.Length) {
                    _spriteRenderer.sprite = _heatLevelSprites[spriteIndex];
                }
           // }
        }
			
        public void ApplyHeat(int heat) { _tileController.ApplyHeat(heat); }

        public void SpreadFire() { _tileController.SpreadFire(); }

        public bool IsOnFire() { return _tileController.IsOnFire(); }

        public bool IsBurntOut() { return _tileController.IsBurntOut(); }
        
        public void SetController(ITileController controller) { _tileController = controller; }

        public ITileController GetTileController() { return _tileController; }
    }
}