using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Managers {
    public class TileView : MonoBehaviour {
        public TileType type;
        private ITileController _TileController;

        // Use this for initialization
        public void Awake() {}

        // Update is called once per frame
        public void Update() {
            Transform child = transform.GetChild(0);
            child.gameObject.SetActive(_TileController.IsOnFire());
        }
			
        public void ApplyHeat(int heat) { _TileController.ApplyHeat(heat); }

        public void SpreadFire() { _TileController.SpreadFire(); }

        public bool IsOnFire() { return _TileController.IsOnFire(); }

        public bool IsBurntOut() { return _TileController.IsBurntOut(); }
        
        public void SetController(ITileController controller) { _TileController = controller; }

        public ITileController GetTileController() { return _TileController; }
    }
}