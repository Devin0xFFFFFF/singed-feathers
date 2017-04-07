using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utility {
    public class PollTimer {
        public delegate void PollCallback();
        private const float _waitTime = 0.5;

        public static IEnumerator Wait(PollCallback pollCallback) {
            yield return new WaitForSeconds(_waitTime);
            pollCallback();
        }
    }
}