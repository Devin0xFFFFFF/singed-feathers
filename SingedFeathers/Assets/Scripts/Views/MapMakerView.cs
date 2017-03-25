using CoreGame.Controllers;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Views {
    public class MapMakerView : MonoBehaviour {

        public List<TileView> TileSet;
        public MapMakerInputView InputView;
        private Dictionary<TileType, TileView> _tileDictionary;
        private IMapController _mapController;
        private TileView[,] _map;
        private IList<Position> _firePositions;
        private IList<Position> _pigeonPositions;
        private int _width, _height;
        private float _tileSizeX, _tileSizeY;

    	// Use this for initialization
    	void Start () {
            if (TileSet.Count > 0) {
                LoadTileDictionary();
                LoadMap();
            }
    	}
    	
    	// Update is called once per frame
    	void Update () {
    		
        }

        public void LoadTileDictionary() {
            _tileDictionary = new Dictionary<TileType, TileView>();
            foreach (TileView tile in TileSet) {
                _tileDictionary.Add(tile.Type, tile);
            }
        }

        public void LoadMap() { 

            _mapController = new MapController();
            if (!_mapController.GenerateDefaultMap()) {
                Debug.LogError("Failed to generate map.");
                return;
            }
            _width = _mapController.Width;
            _height = _mapController.Height;
            _map = new TileView[_width, _height];

            _tileSizeX = TileSet[0].GetComponent<Renderer>().bounds.size.x;
            _tileSizeY = TileSet[0].GetComponent<Renderer>().bounds.size.y;

            InstantiateTiles();
        }

        private void InstantiateTiles() {
            for (int x = 0; x < _width; x++) {
                for (int y = 0; y < _height; y++) {
                    InstantiateTile(_mapController.GetTileType(x, y), x, y);
                }
            }
        }

        private void InstantiateTile(TileType type, int x, int y) {
            TileView manager = _tileDictionary[type];
            _map[x, y] = Instantiate(manager, new Vector3(_tileSizeX * x - 1f, _tileSizeY * y - 2.5f, 1) * 1.6f, Quaternion.identity);
            _map[x, y].SetController(_mapController.GetTileController(x, y));
        }

        private void UpdateTileType(TileType type, int x, int y) {
            Destroy(_map[x, y].gameObject);
            InstantiateTile(type, x, y);
        }
    }
}
