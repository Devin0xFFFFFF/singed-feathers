using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : MonoBehaviour {

	//TODO: have a place to put constants
	const int BURN_HEAT = 5;

	//set these in the editor for now
	public bool _isFlammable;
	public bool _isBurntOut;
	public int _flashPoint;
	public int _durability;
	public int _burnDuration;
	public TileType type;

	int _heatThisTurn;
	bool _onFire;
	bool _shouldSpreadFireThisTurn;
	List<TileInfo> _neighbouringTiles;

	// Use this for initialization
	void Awake () {
		_heatThisTurn = 0;
		_onFire = false;
		_shouldSpreadFireThisTurn = false;
		_neighbouringTiles = new List<TileInfo> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void StartTurn() {
		_heatThisTurn = 0;
		_shouldSpreadFireThisTurn = _onFire;
	}

	public void SpreadFire() {
		if (_shouldSpreadFireThisTurn) {
			foreach (TileInfo neighbour in _neighbouringTiles) {
				neighbour.ApplyHeat(BURN_HEAT);
			}
			_burnDuration--;
			if (_burnDuration <= 0) {
				_isBurntOut = true;
			}
		}
	}

	public void AddTileToNeighbours(TileInfo tile) {
		_neighbouringTiles.Add (tile);
	}

	public void SetOnFire() {
		_onFire = true;
		//TODO: load/create a Fire object here
	}

	public void ApplyHeat(int heat) {
		if (_isFlammable && !_onFire) {
			_heatThisTurn += heat;
			if (_heatThisTurn >= _flashPoint) {
				SetOnFire();
			} else {
				_durability -= heat;
				if (_durability <= 0) {
					SetOnFire();
				}
			}
		}
	}

	public bool IsBurntOut() { return _isBurntOut; }
}
