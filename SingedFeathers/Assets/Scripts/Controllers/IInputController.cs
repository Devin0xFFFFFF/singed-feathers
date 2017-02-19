using UnityEngine;

namespace Assets.Scripts.Controllers
{
    interface IInputController {
        void HandleInput(Vector2 worldPoint);
    }
}
