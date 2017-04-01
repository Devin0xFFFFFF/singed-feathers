using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Service;
using Assets.Scripts.Utility;
using CoreGame.Controllers;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Models.API.LobbyClient;
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
        private MapIO _mapIO;
        private bool _pigeonsRequireUpdate;
        private LobbyIO _lobbyIO;
        private bool _inLobby;

        // Start here!
        public void Start() {
            UnitySystemConsoleRedirector.Redirect();
            _lobbyIO = new LobbyIO();
            ReadyLobbyInfo readyLobby = new ReadyLobbyInfo();

            readyLobby.IsReady = true;
            readyLobby.ReadyPlayerID = PlayerPrefs.GetString("PlayerID");
            readyLobby.LobbyID = PlayerPrefs.GetString("LobbyID");
            StartCoroutine(_lobbyIO.ReadyLobby(readyLobby, delegate(ReadyLobbyResult result) {
                if (result.IsSuccess()) {
                    Debug.Log("Readied in lobby______________________________________________");
                    _inLobby = false;
                }
                if (TileSet.Count > 0) {
                    LoadTileDictionary();
                    LoadMap(GetMapSelection());
                }
            }));

            /*if (TileSet.Count > 0) {
                LoadTileDictionary();
                LoadMap(GetMapSelection());
            }*/
        }

        public void Update() {
            if (!_inLobby) {
                Debug.Log("Getting GameID");
                StartCoroutine(_lobbyIO.PollLobby(PlayerPrefs.GetString("LobbyID"), PlayerPrefs.GetString("PlayerID"), delegate(PollLobbyResult pollResult) {
                    if (pollResult.IsGameStarted()) {
                        Debug.Log("got gameid " + pollResult.GetLobbyInfo().GameID);
                        TurnResolver.SetGameID(pollResult.GetLobbyInfo().GameID);
                        _inLobby = true;
                    }
                }));
                
            } else {

                if (_mapController != null) {
                    Debug.Log("polling");
                    if (_mapController.ShouldPoll()) {
                        _mapController.Poll();
                    }
                
                    if (_pigeonsRequireUpdate && _mapController.IsTurnResolved()) {
                        foreach (PigeonView pigeon in _pigeons) {
                            pigeon.UpdatePigeon();
                        }
                        _pigeonsRequireUpdate = false;
                    }
                }
            }
        }

        public void LoadTileDictionary() {
            _tileDictionary = new Dictionary<TileType, TileView>();
            foreach (TileView tile in TileSet) {
                _tileDictionary.Add(tile.Type, tile);
            }
        }
            
        public void LoadMap(string mapID = "Map3") {
            _mapIO = new MapIO();
            StartCoroutine(_mapIO.GetMapData(mapID, delegate (string serializedMapData) {
                if (serializedMapData == null) {
                    Debug.LogError("Failed to retrieve map.");
                    return;
                }
                _mapController = new MapController();
                if (!_mapController.GenerateMap(serializedMapData)) {
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
                PigeonView pigeon = Instantiate(Pigeon, new Vector3(_tileSizeX * pigeonPosition.X  - 1f, _tileSizeY * pigeonPosition.Y - 2.5f, 1) * 1.6f, Quaternion.identity);
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

        public string GetMapSelection() { return PlayerPrefs.GetString("MapID", "Map3"); }

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
            _map[x, y] = Instantiate(manager, new Vector3(_tileSizeX * x - 1f, _tileSizeY * y - 2.5f, 1) * 1.6f, Quaternion.identity);
            _map[x, y].SetController(_mapController.GetTileController(x, y));
        }

        private void UpdateTileType(TileType type, int x, int y) {
            Destroy(_map[x, y].gameObject);
            InstantiateTile(type, x, y);
        }
    }
}
