using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using UnityEngine;

public class TileManager : MonoBehaviour {
    public TileType type;
    private ITileController _tileController;

    // Use this for initialization
    public void Awake() { Initialize(); }

    // Update is called once per frame
    public void Update() {
        if (_tileController.IsOnFire()) {
            Transform child = transform.GetChild(0);
            child.gameObject.SetActive(true);
        }
    }

    public void ApplyHeat(int heat) { _tileController.ApplyHeat(heat); }

    public void SpreadFire() { _tileController.SpreadFire(); }

    public bool IsOnFire() { return _tileController.IsOnFire(); }

    public bool IsBurntOut() { return _tileController.IsBurntOut(); }

    public void Initialize() {
        if (_tileController == null) {
            _tileController = new TileController(type);
        }
    }

    public void SetController(ITileController controller) { _tileController = controller; }
}
