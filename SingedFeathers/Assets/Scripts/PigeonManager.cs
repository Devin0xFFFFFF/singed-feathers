using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts {
    public class PigeonManager : MonoBehaviour {
        private IPigeonController _pigeonController;
        private float _width;
        private float _height;

        // Use this for initialization
        public void Start() { }
	
        // Update is called once per frame
        public void Update() {
            if (_pigeonController.IsDead()) {
                gameObject.SetActive(false);
            }
        }

        public void SetController(IPigeonController controller) { _pigeonController = controller; }

        public void SetDimensions(float width, float height) {
            _width = width;
            _height = height;
        }

        public void UpdatePigeon() {
            Position startPosition = _pigeonController.CurrentPosition();

            bool moved = _pigeonController.Move();
            _pigeonController.UpdateHealth();

            if (moved && !_pigeonController.IsDead()) {
                Position newPosition = _pigeonController.CurrentPosition();
                Vector3 delta = new Vector3(newPosition.X, newPosition.Y, 1) - new Vector3(startPosition.X, startPosition.Y, 1);
                delta.x *= _height;
                delta.y *= _width;

                transform.Translate(delta, Space.World);
            }
        }
    }
}