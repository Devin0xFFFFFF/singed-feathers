using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Input {
	public class NameInput : MonoBehaviour {
		public const int CHARACTER_LIMIT = 20;
		public InputField NameText;

		void Start () {
			NameText.characterLimit = CHARACTER_LIMIT;	
		}
	}
}
