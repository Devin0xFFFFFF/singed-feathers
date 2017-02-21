using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Assets.Scripts.Model;

public class TileController : ITileController {
    const int BURN_HEAT = 10;
    private readonly Tile _tile;
    private readonly List<ITileController> _neighbouringTiles;
    
    public TileController(TileType type) {
        _tile = InitializeTile(type);
        _neighbouringTiles = new List<ITileController>();
    }

    public TileType GetTileType() {
        return _tile.Type;
    }

	public bool IsFlammable() {
		return _tile.FlashPoint.HasValue;
	}

	public bool IsOnFire() {
		return IsFlammable() && _tile.Heat >= _tile.FlashPoint;
	}

	public bool IsBurntOut() {
		return _tile.TurnsOnFire >= _tile.MaxTurnsOnFire;
	}

    public void AddNeighbouringTile(ITileController neighbourController) {
        _neighbouringTiles.Add(neighbourController);
    }
    
    public void SpreadFire() {
		if (IsOnFire()) {
			if (_tile.TurnsOnFire >= 1 && !IsBurntOut ()) {
				foreach (ITileController neighbour in _neighbouringTiles) {
					neighbour.ApplyHeat (BURN_HEAT);
				}
			}
			_tile.TurnsOnFire += 1;
			if (_tile.Type != TileType.Ash && IsBurntOut()) {
                _tile.Type = TileType.Ash;
            }
        }
    }

    public void Extinguish() {
        _tile.Heat = 0;
    }

    public void ApplyHeat(int heat) {
        _tile.Heat += heat;
    }

    private Tile InitializeTile(TileType type) {
        switch (type) {
            case TileType.Wood:
                return new Tile(type, 20, 3);
            case TileType.Grass:
                return new Tile(type, 10, 3);
            case TileType.Stone:
				return new Tile(type, null, 1);
            case TileType.Ash:
            case TileType.Error:
            default:
                return new Tile(type, 0, 1);
        }
    }
}