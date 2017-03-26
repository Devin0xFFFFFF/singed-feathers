using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Views {
    public class PigeonView : MonoBehaviour {
        private Animator _animator;
        private IPigeonController _pigeonController;
        private float _width;
        private float _height;
        private bool _hasDied;

        public void Start() {
            _hasDied = false;
            _animator = transform.GetComponent<Animator>();
            transform.localScale = new Vector3(1.6f, 1.6f, transform.localScale.z);
        }
	
        // Update is called once per frame
        public void Update() {
            if (!_pigeonController.IsDead()) {
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

            if (!_hasDied && _pigeonController.IsDead()) {
                StartCoroutine(PigeonExplode());
                _hasDied = true;
            }
        }

        private IEnumerator PigeonExplode() {
            _animator.Play("Explode");
            yield return new WaitForSeconds(1.0f);
            gameObject.SetActive(false);
        }
    }
}