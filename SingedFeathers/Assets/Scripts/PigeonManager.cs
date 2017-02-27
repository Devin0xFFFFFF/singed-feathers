using Assets.Scripts.Controllers;
using UnityEngine;

public class PigeonManager : MonoBehaviour {
    private const int MAX_HEALTH = 100;
    private const int FIRE_DAMAGE = 10;
    private int _health;
    private Vector3 _currPosition;
    private float _mapWidth;
    private float _mapHeight;
    private PigeonController _controller;

	// Use this for initialization
	void Start() { _health = MAX_HEALTH; }
	
	// Update is called once per frame
    void Update() {
        if (IsDead()) {
            gameObject.SetActive(false);
        }
    }

    public bool IsDead() { return _health <= 0; }

    public void SetCoordinates(int x, int y, float width, float height) {
        _controller = new PigeonController();
        _currPosition = new Vector3(x, y, 1);
        _controller.SetDimensions(width, height);
    }

    public void UpdateStatus(TileManager[,] map) {
        if (!IsDead()) {
            Vector3 delta = _controller.UpdatePosition(map, ref _currPosition);
            transform.Translate(delta, Space.World);
            _health = _controller.UpdateHealth(map, _currPosition, _health);
        }
    }        
}
