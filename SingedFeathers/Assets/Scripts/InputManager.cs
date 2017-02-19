using System.Linq;
using Assets.Scripts.Controllers;
using UnityEngine;

public class InputManager : MonoBehaviour {
    private IInputController _inputController;

    void Awake() {
        _inputController = new InputController();
    }

    // Update is called once per frame
    void Update() {
    #if !MOBILE_INPUT
        if (Input.GetMouseButtonDown(0)) {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _inputController.HandleInput(worldPoint);
        }
    #elif MOBILE_INPUT
        if (Input.touches.Any()) {
            Touch touch = Input.touches.FirstOrDefault();
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(touch.position);
            _inputController.HandleInput(worldPoint);
        }

    #endif
    }
}
