﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSelection : MonoBehaviour {
    // Use this for initialization
    public void Start() {}

    // Update is called once per frame
    public void Update() {}

    public void LoadScene(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }
}
