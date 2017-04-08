using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utility {
    public class PollTimer {
        public delegate void PollCallback();
        private const float _waitTime = 0.5f;

        public static IEnumerator ExecuteAfterWait(PollCallback pollCallback) {
            yield return new WaitForSecondsRealtime(_waitTime);
            pollCallback();
        }
    }
}