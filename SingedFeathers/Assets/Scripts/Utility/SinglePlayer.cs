using UnityEngine;

namespace Assets.Scripts.Utility {
    public class SinglePlayer {
        public static bool IsSinglePlayer() {
            return PlayerPrefs.GetInt("NumPlayers") == 1;
        }
    }
}
