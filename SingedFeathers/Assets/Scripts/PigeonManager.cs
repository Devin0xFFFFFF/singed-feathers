using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Models;
using UnityEngine;

public class PigeonManager : MonoBehaviour {
    private const int MAX_HEALTH = 100;
    private const int FIRE_DAMAGE = 10;
    private int _health;
    private Position _currPosition;
    private float _mapWidth;
    private float _mapHeight;

	// Use this for initialization
	void Start() {
        _health = MAX_HEALTH;
	}
	
	// Update is called once per frame
	void Update() {
        if (IsDead()) {
            gameObject.SetActive(false);
        }
	}

    public bool IsDead() { return _health <= 0; }

    public void SetCoordinates( int x, int y, float width, float height) {
        _currPosition = new Position{ X = x, Y = y };
        _mapWidth = width;
        _mapHeight = height;
    }

    public void UpdateStatus(TileManager[,] map) {
        if (!IsDead()) {
            Move(map);
            TakeDamage(map);
        }
    }


    private void Move(TileManager[,] map) {
        TileManager currTile = map[_currPosition.X, _currPosition.Y];
        List<Position> positions = GetNeighbouringPositions(map);

        if (currTile.IsOnFire()) {
            Vector3 move = new Vector3(0,0,0);

            foreach (Position pos in positions) {
                if (!map[pos.X, pos.Y].IsOnFire()) {
                    move.x = pos.X - _currPosition.X;
                    move.y = pos.Y - _currPosition.Y;
                    break;
                }
            }
            move.x *= _mapWidth;
            move.y *= _mapHeight;
            transform.Translate(move, Space.World);
        }
    }

    private List<Position> GetNeighbouringPositions(TileManager[,] map){
        List<Position> tiles = new List<Position>();

        if (_currPosition.X - 1 >= 0) {
            tiles.Add(new Position{X =_currPosition.X - 1, Y = _currPosition.Y});
        }
        if (_currPosition.Y + 1 < map.GetLength(1)) {
            tiles.Add(new Position{X =_currPosition.X, Y = _currPosition.Y + 1});
        }
        if (_currPosition.X + 1 < map.GetLength(0)) {
            tiles.Add(new Position{X =_currPosition.X + 1, Y = _currPosition.Y});
        }
        if (_currPosition.Y - 1 >= 0) {
            tiles.Add(new Position{X =_currPosition.X, Y = _currPosition.Y - 1});
        }

        return tiles;
    } 
    private void TakeDamage(TileManager[,] map) { 
        TileManager currTile = map[_currPosition.X, _currPosition.Y];
        List<Position> positions = GetNeighbouringPositions(map);

        if ( currTile.IsOnFire()){
            _health -= FIRE_DAMAGE * 2;
        }

        foreach (Position pos in positions){
            if ( map[pos.X, pos.Y].IsOnFire()){
                _health -= FIRE_DAMAGE;
            }
        }
    }
}
