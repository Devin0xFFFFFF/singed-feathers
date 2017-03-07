using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Views {
    public class TileView : MonoBehaviour {
        public TileType Type;
        private ITileController _tileController;

        // Update is called once per frame
        public void Update() {
            Transform child = transform.GetChild(0);
            child.gameObject.SetActive(_tileController.IsOnFire());
        }
			
        public void ApplyHeat(int heat) { _tileController.ApplyHeat(heat); }

        public void SpreadFire() { _tileController.SpreadFire(); }

        public bool IsOnFire() { return _tileController.IsOnFire(); }

        public bool IsBurntOut() { return _tileController.IsBurntOut(); }
        
        public void SetController(ITileController controller) { _tileController = controller; }

        public ITileController GetTileController() { return _tileController; }
    }
}