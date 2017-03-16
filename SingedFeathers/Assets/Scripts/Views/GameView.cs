using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Service;
using Assets.Scripts.Utility;
using CoreGame.Controllers;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using Newtonsoft.Json.Utilities;
using Assets.Scripts.Controllers;

namespace Assets.Scripts.Views {
    public class GameView : MonoBehaviour {
        public List<TileView> TileSet;
        public PigeonView Pigeon;
        public InputView InputView;
        public WebTurnResolver TurnResolver;
        private List<PigeonView> _pigeons;
        private Dictionary<TileType, TileView> _tileDictionary;
        private IMapController _mapController;
        private TileView[,] _map;
        private int _width, _height;
        private float _tileSizeX, _tileSizeY;
        private MapPersistenceClient _mapClient;
        private bool _pigeonsRequireUpdate;

        // Start here!
        public void Start() {
            UnitySystemConsoleRedirector.Redirect();
            if (TileSet.Count > 0) {
                LoadTileDictionary();
                LoadMap();
            }
        }

        public void Update() {
            if (_pigeonsRequireUpdate && _mapController.IsTurnResolved()) {
                foreach (PigeonView pigeon in _pigeons) {
                    pigeon.UpdatePigeon();
                }
                _pigeonsRequireUpdate = false;
            }
        }

        public void LoadTileDictionary() {
            _tileDictionary = new Dictionary<TileType, TileView>();
            foreach (TileView tile in TileSet) {
                _tileDictionary.Add(tile.Type, tile);
            }
        }

        public void LoadMap(string mapID = "Map1") {
            _mapClient = new MapPersistenceClient();
            StartCoroutine(_mapClient.GetMapData(mapID, delegate (MapClientResult result) {
                if (result.IsError || result.ResponseCode != 200) {
                    Debug.LogError("Failed to fetch map from server: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                    return;
                }

                Debug.Log("Map fetched from server: " + result.ResponseBody);
                _mapController = new MapController();
                if (!_mapController.GenerateMap(result.ResponseBody)) {
                    Debug.LogError("Failed to generate map.");
                    return;
                }
                _mapController.SetTurnResolver(TurnResolver);

                _width = _mapController.Width;
                _height = _mapController.Height;
                _map = new TileView[_width, _height];

                _tileSizeX = TileSet[0].GetComponent<Renderer>().bounds.size.x;
                _tileSizeY = TileSet[0].GetComponent<Renderer>().bounds.size.y;

                InstantiateTiles();
                SetPlayerSideSelection();
                SetPlayerSideSelectionText();
                Debug.Log(_mapController.GetPlayerSideSelection());

                LoadPigeons();
                LoadInputView();
            }));
        }

        public void LoadInputView() {
            InputView.SetTurnController(_mapController.GetTurnController());
            InputView.SetTurnResolver(_mapController.GetTurnResolver());
        }

        public void LoadPigeons() {
            AotHelper.EnsureList<IPigeonController>();
            _pigeons = new List<PigeonView>();
            IList<IPigeonController> controllers = _mapController.GetPigeonControllers();
            foreach (IPigeonController controller in controllers) {
                Position pigeonPosition = controller.CurrentPosition;
                PigeonView pigeon = Instantiate(Pigeon, new Vector3(_tileSizeX * pigeonPosition.X, _tileSizeY * pigeonPosition.Y, 1), Quaternion.identity);
                pigeon.SetDimensions(_tileSizeX, _tileSizeY);
                pigeon.SetController(controller);
                _pigeons.Add(pigeon);
            }
        }

        public void ProcessTurn() {
            Debug.Log("Resolving turn: " + _mapController.GetTurnsLeft());

            _mapController.EndTurn();
            InputView.ClearSelected();
            _pigeonsRequireUpdate = true;
        }
        
        public ITurnController GetTurnController() { return _mapController.GetTurnController(); }

        public ITurnResolver GetTurnResolver() { return _mapController.GetTurnResolver(); }

        public void Undo() { 
            _mapController.UndoAction();
            InputView.ClearSelected();
        }

        public void Fire() { _mapController.Fire(); }

        public void Water() { _mapController.Water(); }

        public void SetPlayerSideSelection() { _mapController.SetPlayerSideSelection((PlayerSideSelection)PlayerPrefs.GetInt("Side")); }

        public void SetPlayerSideSelectionText() { 
            PlayerSideSelection playerSideSelection = _mapController.GetPlayerSideSelection();
            string side = "";
            if (playerSideSelection == PlayerSideSelection.SavePigeons) {
                side = "save";
            }
            if (playerSideSelection == PlayerSideSelection.BurnPigeons) {
                side = "burn";
            }
            InputView.UpdateSideChosenText(side);
        }

        public string GetGameOverPlayerStatus() { return _mapController.GetGameOverPlayerStatus(); }

        public bool IsGameOver() { return _mapController.IsMapBurntOut() || _mapController.AreAllPigeonsDead(); }

        private void InstantiateTiles() {
            for (int x = 0; x < _width; x++) {
                for (int y = 0; y < _height; y++) {
                    InstantiateTile(_mapController.GetTileType(x, y), x, y);
                }
            }
        }

        private void InstantiateTile(TileType type, int x, int y) {
            TileView manager = _tileDictionary[type];
            _map[x, y] = Instantiate(manager, new Vector3(_tileSizeX * x, _tileSizeY * y, 1), Quaternion.identity);
            _map[x, y].SetController(_mapController.GetTileController(x, y));
        }

        private void UpdateTileType(TileType type, int x, int y) {
            Destroy(_map[x, y].gameObject);
            InstantiateTile(type, x, y);
        }
    }
}
