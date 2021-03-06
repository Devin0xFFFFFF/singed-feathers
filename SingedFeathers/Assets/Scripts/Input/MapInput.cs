﻿using Assets.Scripts.Views;
using UnityEngine;

namespace Assets.Scripts.Input {
    public class MapInput : MonoBehaviour {
        public InputView InputManager;

        public void Awake() {}

        // Update is called once per frame
        public void Update() {
#if !MOBILE_INPUT
            if (UnityEngine.Input.GetMouseButtonDown(0)) {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
                HandleInput(worldPoint);
            }
#elif MOBILE_INPUT
            if (Input.touches.Any()) {
                Touch touch = Input.touches.FirstOrDefault();
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(touch.position);
                HandleInput(worldPoint);
            }
#endif
        }

        public void HandleInput(Vector2 worldPoint) {
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            if (hit.collider != null) {
                if (hit.transform.gameObject.tag == "Tile") {
                    Debug.Log(hit.transform.gameObject.GetComponent<TileView>().Type);
                    InputManager.HandleMapInput(hit.transform.gameObject.GetComponent<TileView>());
                }
            }
        }
    }
}