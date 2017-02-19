using Assets.Scripts.Controllers;
using UnityEngine;

public class TileManager : MonoBehaviour {

    public TileType type;
    private ITileController _tileController;

    // Use this for initialization
    void Awake() {
        Initialize();
    }

    // Update is called once per frame
    void Update() {}

    public void ApplyHeat(int heat) {
        _tileController.ApplyHeat(heat);
    }

    public void StartTurn() {
        _tileController.StartTurn();
    }

    public void SpreadFire() {
        _tileController.SpreadFire();
    }

    public bool IsBurntOut() {
        return _tileController.IsBurntOut();
    }

    public void Initialize() {
        if (_tileController == null) {
            _tileController = new TileController(type);
        }
    }
}
