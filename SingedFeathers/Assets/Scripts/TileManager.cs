using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour {

	public List<TileInfo> tileSet;
	public float updateWait = 2.0f; //TODO: change from a timer to a state based turn system

	private TileInfo[,] map;
	private Dictionary<TileType, TileInfo> tileDictionary;
	private int width, height;
	private float tileSizeX, tileSizeY;
	private float waitTime = 0f;

	//TODO: remove this and load from file
	private int[,] testMapRaw = { 
		{2,3,3,3,3},
		{2,3,2,3,3},
		{3,3,3,3,3},
		{2,3,3,2,3},
		{2,3,3,3,2}
	};

	// Start here!
	void Start () {
		if (tileSet.Count > 0) {
			//there's probably a better approach than this, but it seems to work
			LoadTileDictionary ();
			LoadMap (testMapRaw);
			SetFire (2,3); //TODO: remove this and load from file (initial fire positions)
		}
	}
	
	// Update is called once per frame
	void Update () {
		//if a valid turn, update tiles
		if (waitTime >= updateWait) {
			ProcessTurn ();
			waitTime = 0f;
		} else {
			waitTime += Time.deltaTime;
		}
	}

	void LoadTileDictionary() {
		tileDictionary = new Dictionary<TileType, TileInfo> ();
		foreach(TileInfo tile in tileSet) {
			tileDictionary.Add (tile.type, tile);
		}
	}

	void LoadMap(int[,] mapRaw) {
		width = mapRaw.GetLength (0);
		height = mapRaw.GetLength (1);
		map = new TileInfo[width, height];

		tileSizeX = tileSet[0].GetComponent <Renderer>().bounds.size.x;
		tileSizeY = tileSet[0].GetComponent <Renderer>().bounds.size.y;

		for(int i = 0; i < width; i++) {
			for(int j = 0; j < height; j++) {
				//TODO: change this to an object pooler
				map[i,j] = Instantiate (
					tileDictionary[(TileType)mapRaw[i,j]], 
					new Vector3 (tileSizeX * i, tileSizeY * j, 1), 
					Quaternion.identity);
			}
		}

		for(int i = 0; i < width; i++) {
			for(int j = 0; j < height; j++) {
				LinkNearbyTiles (i, j);
			}
		}
	}

	void LinkNearbyTiles(int x, int y) {
		if (x > 0) {
			map[x,y].AddTileToNeighbours(map[x - 1,y]);
		}
		if (y > 0) {
			map[x,y].AddTileToNeighbours(map[x,y - 1]);
		}
		if (x < width - 1) {
			map[x,y].AddTileToNeighbours(map[x + 1,y]);
		}
		if (y < height - 1) {
			map[x,y].AddTileToNeighbours(map[x,y + 1]);
		}
	}

	void ProcessTurn() {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				map[x,y].StartTurn();
			}
		}
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				map[x,y].SpreadFire();
				if(map[x,y].IsBurntOut()) {
					Destroy (map [x, y]);
					map[x,y] = Instantiate (tileDictionary[TileType.ash], new Vector3 (tileSizeX * x, tileSizeY * y, 1), Quaternion.identity);
				}
			}
		}
	}

	void SetFire(int x, int y) {
		if (x >= 0 && y >= 0 && x < width && y < height) {
			map[x,y].ApplyHeat(100);
		}
	}
}
