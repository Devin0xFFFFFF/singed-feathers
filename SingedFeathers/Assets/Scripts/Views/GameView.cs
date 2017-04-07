using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Service;
using Assets.Scripts.Utility;
using CoreGame.Controllers;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using CoreGame.Models.API.LobbyClient;
using UnityEngine.UI;
using Newtonsoft.Json.Utilities;
using Assets.Scripts.Controllers;

namespace Assets.Scripts.Views {
    public class GameView : MonoBehaviour {
        public List<TileView> TileSet;
        public PigeonView Pigeon;
        public GameInputView InputView;
        public WebTurnResolver TurnResolver;
        public GameSelection gameSelect;
        public Text PigeonCountText;
        private List<PigeonView> _pigeons;
        private Dictionary<TileType, TileView> _tileDictionary;
        private IMapController _mapController;
        private TileView[,] _map;
        private MapIO _mapIO;
        private int _width, _height;
        private float _tileSizeX, _tileSizeY;
        private bool _pigeonsRequireUpdate;
        private LobbyIO _lobbyIO;
        private bool _inLobby;
        private bool _shouldPoll;

        // Start here!
        public void Start() {
            UnitySystemConsoleRedirector.Redirect();
            if (!SinglePlayer.IsSinglePlayer()) {
                _lobbyIO = new LobbyIO();
                ReadyLobbyInfo readyLobby = new ReadyLobbyInfo();

                readyLobby.IsReady = true;
                readyLobby.ReadyPlayerID = PlayerPrefs.GetString("PlayerID");
                readyLobby.LobbyID = PlayerPrefs.GetString("LobbyID");
                _shouldPoll = false;
                StartCoroutine(_lobbyIO.ReadyLobby(readyLobby, delegate (ReadyLobbyResult result) {
                    if (result != null && result.IsSuccess()) {
                        Debug.Log("Readied in lobby");
                        _inLobby = true;
                        _shouldPoll = true;
                    }
                    if (TileSet.Any()) {
                        LoadTileDictionary();
                        LoadMap(GetMapSelection());
                    }
                }));
            } else {
                if (TileSet.Any()) {
                    LoadTileDictionary();
                    LoadMap(GetMapSelection());
                }
            }
        }

        public void Update() {
            if (_inLobby) {
                if (_shouldPoll) {
                    _shouldPoll = false;
                    Debug.Log("Getting GameID");
                    StartCoroutine(_lobbyIO.PollLobby(PlayerPrefs.GetString("LobbyID"), PlayerPrefs.GetString("PlayerID"), delegate (PollLobbyResult pollResult) {
                        if (pollResult != null && pollResult.IsGameStarted()) {
                            Debug.Log("got gameid " + pollResult.GetGameID());
                            TurnResolver.SetGameID(pollResult.GetGameID());
                            _inLobby = false;
                            InputView.ExitLobby();
                        } else {
                            StartCoroutine(PollTimer.Wait(delegate () {
                                _shouldPoll = true;
                            }));
                        }
                    }));
                }
            } else {
                if (_mapController != null) {
                    if (_mapController.ShouldPoll()) {
                        _mapController.Poll();
                    }

                    if (_pigeonsRequireUpdate && _mapController.IsTurnResolved()) {
                        foreach (PigeonView pigeon in _pigeons) {
                            pigeon.UpdatePigeon();
                        }
                        _pigeonsRequireUpdate = false;
                    }

                    if (_pigeonsRequireUpdate && _mapController.IsTurnResolved()) {
                        foreach (PigeonView pigeon in _pigeons) {
                            pigeon.UpdatePigeon();
                        }
                        _pigeonsRequireUpdate = false;
                        UpdatePigeonCount();
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
                Player player = new Player(PlayerPrefs.GetString("PlayerID"));
                _mapController = new MapController(null, player);
                if (!_mapController.GenerateMap(serializedMapData)) {
                    Debug.LogError("Failed to generate map.");
                    return;
                }
                if (!SinglePlayer.IsSinglePlayer()) {
                    _mapController.SetTurnResolver(TurnResolver);
                } else {
                    _mapController.SetTurnResolver(new LocalTurnResolver());
                }

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
                UpdatePigeonCount();
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

        public bool IsGameOver() { return _mapController.IsGameOver(); }

        public void FinishGame() {
            LeaveLobbyInfo leaveLobby = new LeaveLobbyInfo();
            leaveLobby.LeavePlayerID = PlayerPrefs.GetString("PlayerID");
            leaveLobby.LobbyID = PlayerPrefs.GetString("LobbyID");
            if (!SinglePlayer.IsSinglePlayer()) {
                StartCoroutine(_lobbyIO.LeaveLobby(leaveLobby, delegate (LeaveLobbyResult result) {
                    if (result != null && result.IsSuccess()) {
                        Debug.Log(result.ResultMessage);
                    }
                    gameSelect.LoadScene("GameSelectScene");
                }));
            } else {
                gameSelect.LoadScene("GameSelectScene");
            }
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
            _map[x, y].Position = new Position(x, y);
        }
        
        private void UpdatePigeonCount() { PigeonCountText.text = "Pigeons: " + _mapController.GetLivePigeonCount() + "/" + _pigeons.Count; }
    }
}