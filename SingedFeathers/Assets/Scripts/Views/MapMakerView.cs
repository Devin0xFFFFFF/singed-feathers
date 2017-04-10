using CoreGame.Controllers;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using System.Collections.Generic;
using System.Linq;
using CoreGame.Models.API.MapClient;
using CoreGame.Utility;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Service.IO;

namespace Assets.Scripts.Views {
    public class MapMakerView : MonoBehaviour {
        public const string WAIT_TEXT = "Processing...";
        public const string SUCCESS_TEXT = "Success! Map uploaded!";
        public const string FAILURE_TEXT = "Our server has failed us... Try again!";
        public const string INVALID_INPUT_TEXT = "Both fields are required!";
        public const string NO_PIGEONS_TEXT = "You need at least one pigeon in your map!";
        public const string NO_FLAMMABLE_TILES_TEXT = "You have no flammable tiles!";
        public Canvas Modal;
        public List<TileView> TileSet;
        public MapMakerInputView InputView;
        public PigeonView Pigeon;
        public InputField MapTitle;
        public InputField AuthorsName;
        public Text ResultText;
        private Dictionary<TileType, TileView> _tileDictionary;
        private IMapController _mapController;
        private TileView[,] _map;
        private List<PigeonView> _pigeons;
        private MapIO _mapIO;
        private float _tileSizeX, _tileSizeY;

    	// Use this for initialization
    	void Start() {
            if (TileSet.Any()) {
                LoadTileDictionary();
                LoadMap();
                _mapIO = new MapIO();
            }
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
            _map = new TileView[_mapController.Width, _mapController.Height];
            _pigeons = new List<PigeonView>();

            _tileSizeX = TileSet[0].GetComponent<Renderer>().bounds.size.x;
            _tileSizeY = TileSet[0].GetComponent<Renderer>().bounds.size.y;

            InstantiateTiles();
        }

        public void UpdateTileType(TileType type, TileView tile) {
            bool isOnFire = tile.IsOnFire();
            Position tilePos = tile.Position;
            Destroy(_map[tilePos.X, tilePos.Y].gameObject);
            InstantiateTile(type, tilePos.X, tilePos.Y);
            bool isFlammable = _map[tilePos.X, tilePos.Y].IsFlammable();

            if (isFlammable && isOnFire) {
                SetFire(_map[tilePos.X, tilePos.Y]);
            } else if (isOnFire) {
                RemoveFire(_map[tilePos.X, tilePos.Y]);
            }
        }

        public void SetFire(TileView tile) {
            Position tilePos = tile.Position;
            if (_mapController.AddInitialFirePosition(tilePos)) {
                _mapController.ApplyHeat(tilePos.X, tilePos.Y);
                tile.UpKeep();
            }
        }

        public void SetPigeon(TileView tile) {
            Position pigeonPosition = tile.Position;
            if (_mapController.AddInitialPigeonPosition(pigeonPosition)) {
                PigeonView pigeon = Instantiate(Pigeon, new Vector3(_tileSizeX * pigeonPosition.X - 1f, _tileSizeY * pigeonPosition.Y - 2.5f, 1) * 1.6f, Quaternion.identity);
                pigeon.SetController(new PigeonController(tile.GetTileController()));
                pigeon.SetDimensions(_tileSizeX, _tileSizeY);
                _pigeons.Add(pigeon);
            }
        }

        public void UpdateNumberOfTurns() {
            int numTurns = (int) InputView.NumberOfTurnsSlider.value;
            _mapController.UpdateNumberOfTurns(numTurns);
        }

        public void ResetTile(TileView tile) {
            RemovePigeon(tile);
            RemoveFire(tile);
            UpdateTileType(TileType.Grass, tile);
        }

        public void SaveMap() {
            Debug.Log("Saving map...");
            ResetResultText();
            ResultText.gameObject.SetActive(true);

            IEnumerable<ITileController> tileControllers = from TileView tile in _map select tile.GetTileController();
            MapMakerValidationResult result = MapMakerInputValidator.ValidateInput(MapTitle.text, AuthorsName.text, _pigeons.Select(p => p.Position), tileControllers);

            if (result != MapMakerValidationResult.Valid) {
                UpdateResultText(result);
                return;
            }

            CreateMapInfo mapInfo = new CreateMapInfo {
                CreatorName = AuthorsName.text,
                MapName = MapTitle.text,
                MapType = "Versus",
                SerializedMapData = _mapController.SerializeMap()
            };

            StartCoroutine(_mapIO.CreateMap(mapInfo, delegate(string mapId) {
                if (mapId == null) {
                    Debug.LogError("Failed to save map.");
                    ShowResultText(FAILURE_TEXT);
                } else {
                    Debug.Log("Map saved!");
                    ShowResultText(SUCCESS_TEXT, Color.green);
                }
            }));
        }

        public void CloseModal() {
            Modal.gameObject.SetActive(false);
            ResetResultText();
        }

        private void ShowResultText(string text, Color? colour = null) {
            ResultText.text = text;
            ResultText.color = colour ?? Color.red;
        }

        private void ResetResultText() {
            ResultText.gameObject.SetActive(false);
            ResultText.text = WAIT_TEXT;
            ResultText.color = Color.black;
        }

        private void InstantiateTiles() {
            for (int x = 0; x < _mapController.Width; x++) {
                for (int y = 0; y < _mapController.Height; y++) {
                    InstantiateTile(_mapController.GetTileType(x, y), x, y);
                }
            }
        }
        
        private void InstantiateTile(TileType type, int x, int y) {
            TileView manager = _tileDictionary[type];
            _map[x, y] = Instantiate(manager, new Vector3(_tileSizeX * x - 1f, _tileSizeY * y - 2.5f, 1) * 1.6f, Quaternion.identity);
            _mapController.UpdateTileController(type, x, y);
            _map[x, y].SetController(_mapController.GetTileController(x, y));
            _map[x, y].Position = new Position(x, y);
        }

        private void RemovePigeon(TileView tile) {
            Position tilePos = tile.Position;
            bool pigeonRemoved = _mapController.RemoveInitialPigeonPosition(tilePos);
            if (pigeonRemoved) {
                PigeonView pigeon = _pigeons.FirstOrDefault(p => p.Position.Equals(tilePos));
                if (pigeon != null) {
                    _pigeons.Remove(pigeon);
                    Destroy(pigeon.gameObject);
                    _mapController.RemoveInitialPigeonPosition(tilePos);
                }
            }
        }

        private void RemoveFire(TileView tile) {
            Position tilePos = tile.Position;
            bool fireRemoved = _mapController.RemoveInitialFirePosition(tilePos);
            if (fireRemoved) {
                _mapController.ReduceHeat(tilePos.X, tilePos.Y);
            }
        }

        private void UpdateResultText(MapMakerValidationResult result) {
            switch (result) {
                case MapMakerValidationResult.InvalidNoPigeons:
                    ShowResultText(NO_PIGEONS_TEXT);
                    break;
                case MapMakerValidationResult.InvalidNoFlammableTiles:
                    ShowResultText(NO_FLAMMABLE_TILES_TEXT);
                    break;
                case MapMakerValidationResult.InvalidInput:
                    ShowResultText(INVALID_INPUT_TEXT);
                    break;
            }
        }
    }
}