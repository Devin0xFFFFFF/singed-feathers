using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Assets.Scripts.Model;

public class TileController : ITileController {
    const int BURN_HEAT = 5;
    private readonly Tile _tile;
    private readonly List<ITileController> _neighbouringTiles;
    
    public TileController(TileType type) {
        _tile = InitializeTile(type);
        _neighbouringTiles = new List<ITileController>();
    }

    public TileType GetTileType() {
        return _tile.Type;
    }

    public bool IsBurntOut() {
        return _tile.IsBurntOut;
    }

    public bool IsLit() {
        return _tile.OnFire;
    }

    public void AddNeighbouringTile(ITileController neighbourController) {
        _neighbouringTiles.Add(neighbourController);
    }
    
    public void SpreadFire() {
        if (_tile.OnFire && !_tile.IsBurntOut) {
            foreach (ITileController neighbour in _neighbouringTiles) {
                neighbour.ApplyHeat(BURN_HEAT);
            }
            _tile.BurnDuration--;
            if (_tile.Type != TileType.Ash && _tile.IsBurntOut) {
                _tile.Type = TileType.Ash;
            }
        }
    }

    public void StartTurn() {
        _tile.Heat = 0;
    }

    public bool Ignite() {
        if (!_tile.OnFire && _tile.IsFlammable && (_tile.Heat >= _tile.FlashPoint || _tile.Durability == 0)) {
            _tile.OnFire = true;
        }
        return _tile.OnFire;
    }

    public bool Extinguish() {
        _tile.OnFire = false;
        return true;
    }

    public void ApplyHeat(int heat) {
        _tile.Heat += heat;
        TakeDamage(heat);
        Ignite();
    }

    private Tile InitializeTile(TileType type) {
        switch (type) {
            case TileType.Wood:
                return new Tile(type, true, 15, 20, 5);
            case TileType.Grass:
                return new Tile(type, true, 5, 10, 3);
            case TileType.Stone:
                return new Tile(type, false, 0, 0, 1);
            case TileType.Ash:
            case TileType.Error:
            default:
                return new Tile(type, true, 0, 0, 1);
        }
    }

    private void TakeDamage(int heat) {
        if (_tile.Durability > 0) {
            _tile.Durability -= heat;
        }
    }
}