using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigeonManager : MonoBehaviour {
    private const int MAX_HEALTH = 100;
    private const int FIRE_DAMAGE = 10;
    private int _health;
    private Vector3 _currPosition;
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

    public void SetCoordinates(int x, int y, float width, float height) {
        _currPosition = new Vector3(x, y, 1);
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
        TileManager currTile = map[(int)_currPosition.x, (int)_currPosition.y];
        List<Vector3> positions = GetNeighbouringPositions(map);

        if (currTile.IsOnFire()) {
            Vector3 move;

            foreach (Vector3 pos in positions) {
                if (!map[(int)pos.x, (int)pos.y].IsOnFire()) {
                    move = pos - _currPosition;

                    move.x *= _mapWidth;
                    move.y *= _mapHeight;

                    transform.Translate(move, Space.World);

                    _currPosition = pos;
                    break;
                }
            }
        }
    }

    private List<Vector3> GetNeighbouringPositions(TileManager[,] map){
        List<Vector3> tiles = new List<Vector3>();

        if (_currPosition.y + 1 < map.GetLength(1)) {
            tiles.Add(new Vector3(_currPosition.x, _currPosition.y + 1, 1));
        }
        if (_currPosition.x + 1 < map.GetLength(0)) {
            tiles.Add(new Vector3(_currPosition.x + 1, _currPosition.y, 1));
        }
        if (_currPosition.y - 1 >= 0) {
            tiles.Add(new Vector3(_currPosition.x, _currPosition.y - 1, 1));
        }
        if (_currPosition.x - 1 >= 0) {
            tiles.Add(new Vector3(_currPosition.x - 1, _currPosition.y, 1));
        } 
        return tiles;
    } 

    private void TakeDamage(TileManager[,] map) { 
        TileManager currTile = map[(int)_currPosition.x, (int)_currPosition.y];
        List<Vector3> positions = GetNeighbouringPositions(map);

        if (currTile.IsOnFire()){
            _health -= FIRE_DAMAGE * 2;
        }

        foreach (Vector3 pos in positions){
            if (map[(int)pos.x, (int)pos.y].IsOnFire()){
                _health -= FIRE_DAMAGE;
            }
        }
    }
}
