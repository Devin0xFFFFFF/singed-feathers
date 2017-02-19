﻿using System.Collections.Generic;
using Assets.Scripts.Controllers;
using UnityEngine;
using Assets.Scripts.Model;

public class MapManager : MonoBehaviour {

    public List<TileManager> tileSet;
    public float UpdateWait = 2.0f; //TODO: change from a timer to a state based turn system
    private Dictionary<TileType, TileManager> _tileDictionary;
    private IMapController _mapController;
    private TileManager[,] _map;
    private int _width, _height;
    private float _tileSizeX, _tileSizeY;
    private float _waitTime = 0f;

    // Start here!
    void Start() {
        if (tileSet.Count > 0) {
            //there's probably a better approach than this, but it seems to work
            LoadTileDictionary();
            LoadMap();
            SetFire(2, 3); //TODO: remove this and load from file (initial fire positions)
        }
    }

    // Update is called once per frame
    void Update() {
        //if a valid turn, update tiles
        if (_waitTime >= UpdateWait) {
            ProcessTurn();
            _waitTime = 0f;
        } else {
            _waitTime += Time.deltaTime;
        }
    }

    void LoadTileDictionary() {
        _tileDictionary = new Dictionary<TileType, TileManager>();
        foreach (TileManager tile in tileSet) {
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

        _tileSizeX = tileSet[0].GetComponent<Renderer>().bounds.size.x;
        _tileSizeY = tileSet[0].GetComponent<Renderer>().bounds.size.y;

        InstantiateTiles();
    }

    void ProcessTurn() {
        _mapController.TakeTurn();
        IList<Position> newlyBurntTileLocations = _mapController.SpreadFires();
        foreach (Position pos in newlyBurntTileLocations) {
            UpdateTileType(TileType.Ash, pos.X, pos.Y);
        }
    }

    void SetFire(int x, int y) {
        _mapController.ApplyHeat(x, y, 100);
    }

    private void InstantiateTiles() {
        for (int x = 0; x < _width; x++) {
            for (int y = 0; y < _height; y++) {
                //TODO: change this to an object pooler
                InstantiateTile(_mapController.GetTileType(x, y), x, y);
            }
        }
    }

    private void InstantiateTile(TileType type, int x, int y) {
        TileManager manager = _tileDictionary[type];
        _map[x, y] = Instantiate(manager, new Vector3(_tileSizeX * x, _tileSizeY * y, 1), Quaternion.identity);
    }

    private void UpdateTileType(TileType type, int x, int y) {
        Destroy(_map[x, y]);
        InstantiateTile(type, x, y);
    }
}