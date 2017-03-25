using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Assets.Scripts.Views {
    public abstract class InputView : MonoBehaviour {
        public virtual void HandleMapInput(TileView tileManager) { }
    }
}