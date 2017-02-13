using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

	// Update is called once per frame
	void Update () {

		#if !MOBILE_INPUT
		if (Input.GetMouseButtonDown (0)) {
			Vector2 worldPoint = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			RaycastHit2D hit = Physics2D.Raycast (worldPoint, Vector2.zero);

			if (hit.collider != null) {
				if (hit.transform.gameObject.tag == "Tile") {
					Debug.Log (hit.transform.gameObject.GetComponent <TileInfo>().type);
					hit.transform.gameObject.GetComponent <TileInfo>().ApplyHeat (100);
				} 
			} 
		}
		#endif
		//TODO: add mobile input 
	}
}
