using System.Collections.Generic;
using Assets.Scripts.Controllers;
using UnityEngine;
using Assets.Scripts.Models;

public class MapManager : MonoBehaviour {

	public GameStateManager GameStateManager;
	public List<TileManager> TileSet;
	private Dictionary<TileType, TileManager> _tileDictionary;
	private IMapController _mapController;
	private TileManager[,] _map;
	private List<ICommand> _turnCommands;
	private int _width, _height;
	private float _tileSizeX, _tileSizeY;
	private int _turnCount;

	// Start here!
	void Start() {
		if (TileSet.Count > 0) {
			LoadTileDictionary();
			LoadMap();
			_turnCommands = new List<ICommand>();
			LoadFires();
			_turnCount = 1;
		}
	}

	// Update is called once per frame
	void Update() {
		IGameState currState = GameStateManager.CurrState;
		if(currState is ResolveState) {
			ProcessTurn();
			currState.ChangeState();
		}
	}

	void LoadTileDictionary() {
		_tileDictionary = new Dictionary<TileType, TileManager>();
		foreach (TileManager tile in TileSet) {
			tile.Initialize();
			_tileDictionary.Add(tile.type, tile);
		}
	}

	void LoadMap() {
		_mapController = new MapController();
		_mapController.GenerateMap();
		_width = _mapController.Width;
		_height = _mapController.Height;
		_map = new TileManager[_width, _height];

		_tileSizeX = TileSet[0].GetComponent<Renderer>().bounds.size.x;
		_tileSizeY = TileSet[0].GetComponent<Renderer>().bounds.size.y;

		InstantiateTiles();
	}

	void LoadFires() { SetFire(2, 3);}

	void ProcessTurn() {
		Debug.Log ("Resolving turn: " + _turnCount);

		foreach(ICommand command in _turnCommands) {
			command.ExecuteCommand();
		}
		_turnCommands.Clear();

		IDictionary<NewStatus, IList<Position>> modifiedTilePositions = _mapController.SpreadFires();
		foreach (Position pos in modifiedTilePositions[NewStatus.BurntOut]) {
			UpdateTileType(TileType.Ash, pos.X, pos.Y);
		}

		_turnCount++;
	}

	void SetFire(int x, int y) { _mapController.ApplyHeat(x, y); }

	private void InstantiateTiles() {
		for (int x = 0; x < _width; x++) {
			for (int y = 0; y < _height; y++) {
				InstantiateTile(_mapController.GetTileType(x, y), x, y);
			}
		}
	}

	private void InstantiateTile(TileType type, int x, int y) {
		TileManager manager = _tileDictionary[type];
		_map[x, y] = Instantiate(manager, new Vector3(_tileSizeX * x, _tileSizeY * y, 1), Quaternion.identity);
		_map[x, y].SetController(_mapController.GetController(x, y));
	}

	private void UpdateTileType(TileType type, int x, int y) {
		Destroy(_map[x, y].gameObject);
		InstantiateTile(type, x, y);
	}

	public void AddCommand(ICommand command) { _turnCommands.Add(command); }

	public void UndoLastCommand() {
		if (_turnCommands.Count > 0) {
			_turnCommands.Remove(_turnCommands [_turnCommands.Count - 1]);
		}
	}
}
