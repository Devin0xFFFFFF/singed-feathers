using UnityEngine;

namespace Assets.Scripts.Views {
    public abstract class InputView : MonoBehaviour {
        public virtual void HandleMapInput(TileView tileManager) { }
    }
}