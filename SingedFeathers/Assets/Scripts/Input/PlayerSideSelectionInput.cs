using System;
using CoreGame.Models;
using UnityEngine;

namespace Assets.Scripts.Input {
    public class PlayerSideSelectionInput : MonoBehaviour {
        public void ChooseToSavePigeons() { PlayerPrefs.SetInt("Side", (int)PlayerSideSelection.SavePigeons); }

        public void ChooseToBurnPigeons() { PlayerPrefs.SetInt("Side", (int)PlayerSideSelection.BurnPigeons); }
    }

}



