using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInput : MonoBehaviour {

	public GameStateManager gsm;

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
				gsm.HandleMapInput (hit.transform.gameObject.GetComponent<TileManager>());
			}
		}
	}

	/*public void SetState(string newState) {
		state = GameState.Fire;
		Debug.Log (state);
	}*/
}
