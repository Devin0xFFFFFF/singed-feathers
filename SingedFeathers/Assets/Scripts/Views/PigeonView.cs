using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using UnityEngine;

namespace Assets.Scripts.Views {
    public class PigeonView : MonoBehaviour {
        private Animator _animator;
        private IPigeonController _pigeonController;
        private float _width;
        private float _height;

        public void Start() {
            _animator = transform.GetComponent<Animator>();
            transform.localScale = new Vector3(1.6f, 1.6f, transform.localScale.z);
        }
	
        // Update is called once per frame
        public void Update() {
            if (_pigeonController.IsDead()) {
                gameObject.SetActive(false);
            } else {
                _animator.SetFloat("Rand", Random.Range(0.0f, 1.0f));
            }
        }

        public void SetController(IPigeonController controller) { _pigeonController = controller; }

        public void SetDimensions(float width, float height) {
            _width = width;
            _height = height;
        }

        public void UpdatePigeon() {
            if (_pigeonController.HasMoved()) {
                Position start = _pigeonController.InitialPosition;
                Position end = _pigeonController.CurrentPosition;

                Vector3 delta = new Vector3(end.X, end.Y, 1) - new Vector3(start.X, start.Y, 1);
                delta.x *= _height;
                delta.y *= _width;
                delta *= 1.6f;

                transform.Translate(delta, Space.World);
            }
        }
    }
}