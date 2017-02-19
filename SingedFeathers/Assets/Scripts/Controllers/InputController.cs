using System;
using UnityEngine;

namespace Assets.Scripts.Controllers { 
    [Serializable]
    public class InputController : IInputController {
        public void HandleInput(Vector2 worldPoint) {
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
            if (hit.collider != null) {
                if (hit.transform.gameObject.tag == "Tile") {
                    Debug.Log(hit.transform.gameObject.GetComponent<TileManager>().type);
                    hit.transform.gameObject.GetComponent<TileManager>().ApplyHeat(100);
                }
            }
        }
    }
}
