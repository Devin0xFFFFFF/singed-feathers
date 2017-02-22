using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Assets.Scripts.Model;

public class TileController : ITileController {
    const int BURN_HEAT = 10;
	public bool StateHasChanged { get; set; }
    private readonly Tile _tile;
    private readonly List<ITileController> _neighbouringTiles;
    
    public TileController(TileType type) {
		StateHasChanged = false;
        _tile = InitializeTile(type);
        _neighbouringTiles = new List<ITileController>();
    }

    public TileType GetTileType() {
        return _tile.Type;
    }

	public bool IsFlammable() {
		return _tile.FlashPoint.HasValue && !IsBurntOut();
	}

	public bool IsSpreadingHeat() {
		// Spread heat if the tile has been burning for more than one turn (at the end of the last turn)
		return (StateHasChanged && IsBurntOut ()) || (!StateHasChanged && IsOnFire ());
	}

	public bool IsOnFire() {
		return IsFlammable() && _tile.Heat >= _tile.FlashPoint;
	}

	public bool IsBurntOut() {
		return _tile.Type == TileType.Ash || _tile.TurnsOnFire >= _tile.MaxTurnsOnFire;
	}

    public void AddNeighbouringTile(ITileController neighbourController) {
        _neighbouringTiles.Add(neighbourController);
    }
    
    public void SpreadFire() {
		bool startedOnFire = IsOnFire();

		// Apply heat for each neighbouring tile that is spreading heat
		foreach (ITileController neighbour in _neighbouringTiles) {
			if (neighbour.IsSpreadingHeat()) {
				ApplyHeat(BURN_HEAT);
			}
		}
			
		if (IsOnFire()) {
			_tile.TurnsOnFire += 1;

			if (startedOnFire && IsBurntOut()) {
				_tile.Type = TileType.Ash;
				_tile.FlashPoint = null;
				_tile.MaxTurnsOnFire = 0;
				Extinguish();
				StateHasChanged = true;
			}
		}
    }

    public void Extinguish() {
        _tile.Heat = 0;
		_tile.TurnsOnFire = 0;
    }

    public void ApplyHeat(int heat) {
		bool startedOnFire = IsOnFire();
		_tile.Heat += heat;
		if (!startedOnFire && IsOnFire()) {
			StateHasChanged = true;
		}
    }

    private Tile InitializeTile(TileType type) {
        switch (type) {
            case TileType.Wood:
                return new Tile(type, 20, 3);
            case TileType.Grass:
                return new Tile(type, 10, 3);
            case TileType.Stone:
				return new Tile(type, null, 0);
			case TileType.Ash:
			case TileType.Error:
			default:
				return new Tile (type, null, 0);
        }
    }
}