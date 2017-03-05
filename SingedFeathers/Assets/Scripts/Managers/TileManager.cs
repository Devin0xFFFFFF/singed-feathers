using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Managers {
    public class TileManager : MonoBehaviour {
        public TileType type;
        private ITileController _tileController;

        // Use this for initialization
        public void Awake() {}

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