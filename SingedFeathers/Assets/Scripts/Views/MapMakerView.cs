using CoreGame.Controllers;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Service;
using CoreGame.Models.API.MapClient;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Views {
    public class MapMakerView : MonoBehaviour {
        public enum ValidationResult {
            Valid, InvalidNoPigeons, InvalidNoFlammableTiles, InvalidInput 
        }

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
        private List<Position> _firePositions;
        private MapIO _mapIO;
        private int _width, _height;
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
            _width = _mapController.Width;
            _height = _mapController.Height;
            _map = new TileView[_width, _height];
            _pigeons = new List<PigeonView>();
            _firePositions = new List<Position>();

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
            if (tile.IsFlammable() && !tile.IsOnFire()) {
                _mapController.ApplyHeat(tilePos.X, tilePos.Y);
                tile.UpKeep();
                _firePositions.Add(tilePos);
            }
        }

        public void SetPigeon(TileView tile) {
            if (tile.IsOccupied()) {
                return;
            }

            Position pigeonPosition = tile.Position;
            PigeonView pigeon = Instantiate(Pigeon, new Vector3(_tileSizeX * pigeonPosition.X  - 1f, _tileSizeY * pigeonPosition.Y - 2.5f, 1) * 1.6f, Quaternion.identity);
            pigeon.SetDimensions(_tileSizeX, _tileSizeY);
            pigeon.SetController(_mapController.AddPigeonToMap(pigeonPosition));
            _pigeons.Add(pigeon);
        }

        public void ResetTile(TileView tile) {
            RemovePigeon(tile);
            RemoveFire(tile);
            UpdateTileType(TileType.Grass, tile);
        }

        public void SaveMap() {
            Debug.Log("Saving map...");
            ResultText.gameObject.SetActive(true);

            if (ValidateUploadRequest() != ValidationResult.Valid) {
                return;
            }

            TileType[,] tileTypeMap = RecordTileTypes();
            CreateMapInfo mapInfo = new CreateMapInfo {
                CreatorName = AuthorsName.text,
                MapName = MapTitle.text,
                MapType = "Versus",
                SerializedMapData =
                    _mapController.SerializeMap(_width, _height, _pigeons.Select(p => p.Position).ToList(),
                        _firePositions, tileTypeMap, InputView.GetNumTurns())
            };

            StartCoroutine(_mapIO.CreateMap(mapInfo, delegate(string mapId) {
                if (mapId == null) {
                    Debug.LogError("Failed to save map.");
                    ShowFailureText();
                } else {
                    Debug.Log("Map saved!");
                    ShowSuccessText();
                }
            }));
        }

        public void CloseModal() {
            Modal.gameObject.SetActive(false);
            ResetResultText();
        }

        private ValidationResult ValidateUploadRequest() {
            if (!HasFlammableTile()) {
                ShowNoFlammableTileText();
                return ValidationResult.InvalidNoFlammableTiles;
            } else if (!_pigeons.Any()) {
                ShowNoPigeonText();
                return ValidationResult.InvalidNoPigeons;
            } else if (!ValidInputReceived()) {
                ShowInvalidInputText();
                return ValidationResult.InvalidInput;
            }
            return ValidationResult.Valid;
        }

        private void ShowNoFlammableTileText() {
            ResultText.text = NO_FLAMMABLE_TILES_TEXT;
            ResultText.color = Color.red;
        }

        private void ShowNoPigeonText() {
            ResultText.text = NO_PIGEONS_TEXT;
            ResultText.color = Color.red;
        }

        private bool ValidInputReceived() { return AuthorsName.text.Count() > 0 && MapTitle.text.Count() > 0; }

        private bool HasFlammableTile() {
            for (int x = 0; x < _width; x++) {
                for (int y = 0; y < _height; y++) {
                    if (_map[x, y].IsFlammable()) {
                        return true;
                    }
                }
            }
            return false;
        }

        private void ResetResultText() {
            ResultText.gameObject.SetActive(false);
            ResultText.text = WAIT_TEXT;
            ResultText.color = Color.black;
        }

        private void ShowSuccessText() {
            ResultText.text = SUCCESS_TEXT;
            ResultText.color = Color.green;
        }

        private void ShowInvalidInputText() {
            ResultText.text = INVALID_INPUT_TEXT;
            ResultText.color = Color.red;
        }

        private void ShowFailureText() {
            ResultText.text = FAILURE_TEXT;
            ResultText.color = Color.red;
        }

        private void InstantiateTiles() {
            for (int x = 0; x < _width; x++) {
                for (int y = 0; y < _height; y++) {
                    InstantiateTile(_mapController.GetTileType(x, y), x, y);
                }
            }
        }

        private TileType[,] RecordTileTypes() {
            TileType[,] map = new TileType[_width, _height];
            for (int x = 0; x < _width; x++) {
                for (int y = 0; y < _height; y++) {
                    map[x, y] = _map[x, y].Type;
                }
            }
            return map;
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
            PigeonView pigeonToDelete = _pigeons.FirstOrDefault(p => p.Position.X == tilePos.X && p.Position.Y == tilePos.Y);
            if (pigeonToDelete != null) {
                _pigeons.Remove(pigeonToDelete);
                Destroy(pigeonToDelete.gameObject);
            }
        }

        private void RemoveFire(TileView tile) {
            Position tilePos = tile.Position;
            Position firePosToDelete = _firePositions.FirstOrDefault(f => f.X == tilePos.X && f.Y == tilePos.Y);
            if (firePosToDelete != null) {
                _firePositions.Remove(firePosToDelete);
                _mapController.ReduceHeat(tilePos.X, tilePos.Y);
            }
        }
    }
}