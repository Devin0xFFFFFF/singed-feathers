﻿using System.Linq;
using Assets.Scripts.Controllers;
using UnityEngine;

public class InputManager : MonoBehaviour {

	private InputState state;

    void Awake() {
    }

    // Update is called once per frame
    void Update() {
    #if !MOBILE_INPUT
        if (Input.GetMouseButtonDown(0)) {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
                Debug.Log(hit.transform.gameObject.GetComponent<TileManager>().type);
				if (state == InputState.Fire) {
					hit.transform.gameObject.GetComponent<TileManager> ().ApplyHeat (100);
				}
            }
        }
    }

	public void SetFireState() {
		state = InputState.Fire;
		Debug.Log (state);
	}
}
