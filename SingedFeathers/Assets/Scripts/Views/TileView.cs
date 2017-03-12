using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Views {
    public class TileView : MonoBehaviour {
        public TileType Type;
		public Sprite[] Sprites = new Sprite[4];
        private ITileController _tileController;
		private SpriteRenderer _spriteRenderer;

		public void Start() {
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer> ();
            _spriteRenderer.sprite = Sprites[0];
		}

        // Update is called once per frame
        public void Update() {
            if (_tileController.HasVisualStateChange()) {
                Transform child = transform.GetChild(0);
                int frame = GetSpriteHeatFrame();
                child.gameObject.SetActive(_tileController.IsOnFire());
                _spriteRenderer.sprite = Sprites[frame];
            }
        }

		public int GetSpriteHeatFrame() { return _tileController.GetSpriteHeatFrame(); }

        public bool HasVisualStateChange() { return _tileController.StateHasChanged; }
			
        public void ApplyHeat(int heat) { _tileController.ApplyHeat(heat); }

        public void SpreadFire() { _tileController.SpreadFire(); }

        public bool IsOnFire() { return _tileController.IsOnFire(); }

        public bool IsBurntOut() { return _tileController.IsBurntOut(); }
        
        public void SetController(ITileController controller) { _tileController = controller; }

        public ITileController GetTileController() { return _tileController; }
    }
}