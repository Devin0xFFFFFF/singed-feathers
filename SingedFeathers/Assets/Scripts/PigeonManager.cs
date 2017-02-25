using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Models;
using UnityEngine;

public class PigeonManager : MonoBehaviour {
    private int _health;
    private Position _currPosition;

	// Use this for initialization
	void Start() {
        _health = 100;
	}
	
	// Update is called once per frame
	void Update() {
        if (IsDed()) {
            gameObject.SetActive(false);
        }
	}

    public bool IsDed() { return _health <= 0; }

    public void SetPositon( int x, int y) {
        _currPosition = new Position{ X = x, Y = y };
    }

    public void UpdateStatus() {
        Move();
        TakeDamage();
    }

    public void Move() {
        //get neighbouring tiles
        //if n.heat < c.heat
        //rigidbody.move n
        //pos = n.pos
    }

    public void TakeDamage() { /*_health -= tile@pos.heat*/}
}
