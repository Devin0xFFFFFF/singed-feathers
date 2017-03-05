using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Managers {
    public class PigeonView : MonoBehaviour {
        private IPigeonController _PigeonController;
        private float _Width;
        private float _Height;

        // Use this for initialization
        public void Start() {}
	
        // Update is called once per frame
        public void Update() {
            if (_PigeonController.IsDead()) {
                gameObject.SetActive(false);
            }
        }

        public void SetController(IPigeonController controller) { _PigeonController = controller; }

        public void SetDimensions(float width, float height) {
            _Width = width;
            _Height = height;
        }

        public void UpdatePigeon() {
            if (_PigeonController.HasMoved()) {
                Position start = _PigeonController.InitialPosition;
                Position end = _PigeonController.CurrentPosition;

                Vector3 delta = new Vector3(end.X, end.Y, 1) - new Vector3(start.X, start.Y, 1);
                delta.x *= _Height;
                delta.y *= _Width;

                transform.Translate(delta, Space.World);
            }
        }
    }
}