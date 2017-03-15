using UnityEngine;
using System;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;

namespace Assets.Scripts.Views {
    public class TileView : MonoBehaviour {
        public TileType Type;
        public Sprite SpriteHeatLevel0;
        public Sprite SpriteHeatLevel1;
        public Sprite SpriteHeatLevel2;
        public Sprite SpriteHeatLevel3;
        public Sprite BurntOutSprite;
        private Sprite[] _heatLevelSprites;
        private ITileController _tileController;
        private SpriteRenderer _spriteRenderer;
        private const int HEAT_PER_INDEX = 10;
        private int _maxSpriteIndex;

        public void Start() {
            _heatLevelSprites = new Sprite[] { SpriteHeatLevel0, SpriteHeatLevel1, SpriteHeatLevel2, SpriteHeatLevel3 };
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            _maxSpriteIndex = _heatLevelSprites.Length - 1;
        }

        // Update is called once per frame
        public void Update() {
            Transform child = transform.GetChild(0);
            child.gameObject.SetActive(_tileController.IsOnFire());
            SetSprite();
        }
			
        public void ApplyHeat(int heat) { _tileController.ApplyHeat(heat); }

        public void SpreadFire() { _tileController.SpreadFire(); }

        public bool IsOnFire() { return _tileController.IsOnFire(); }

        public bool IsBurntOut() { return _tileController.IsBurntOut(); }
        
        public void SetController(ITileController controller) { _tileController = controller; }

        public ITileController GetTileController() { return _tileController; }

        private void SetSprite() {
            if (_tileController.IsBurntOut()) {
                _spriteRenderer.sprite = BurntOutSprite;
            } else {
                int spriteIndex = Math.Min(_tileController.GetTileHeat() / HEAT_PER_INDEX, _maxSpriteIndex);
                _spriteRenderer.sprite = _heatLevelSprites[spriteIndex];
            }
        }
    }
}