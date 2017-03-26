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
        public PigeonView Pigeon;
        private Dictionary<TileType, TileView> _tileDictionary;
        private IMapController _mapController;
        private TileView[,] _map;
        private List<PigeonView> _pigeons;
        private List<Position> _firePositions;
        private int _width, _height;
        private float _tileSizeX, _tileSizeY;

    	// Use this for initialization
    	void Start () {
            if (TileSet.Count > 0) {
                LoadTileDictionary();
                LoadMap();
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
            bool isOnFire = tile.GetTileController().IsOnFire();
            Position tilePos = tile.Position;
            Destroy(_map[tilePos.X, tilePos.Y].gameObject);
            InstantiateTile(type, tilePos.X, tilePos.Y);
            bool isFlammable = _map[tilePos.X, tilePos.Y].GetTileController().IsFlammable();

            if (isFlammable && isOnFire) {
                SetFire(_map[tilePos.X, tilePos.Y]);
            }
            else if (isOnFire) {
                RemoveFire(_map[tilePos.X, tilePos.Y]);
            }
        }

        public void SetFire(TileView tile) {
            Position tilePos = tile.Position;
            if(tile.GetTileController().IsFlammable()) {
                _mapController.ApplyHeat(tilePos.X, tilePos.Y);
                tile.GetTileController().UpKeep();
                _firePositions.Add(tilePos);
            }
        }

        public void SetPigeon(TileView tile) {
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
            // TODO: add FirePositions, PigeonPositions, NumTurns, MapMatrix to the _map object...
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
            _mapController.UpdateTileController(type, x, y);
            _map[x, y].SetController(_mapController.GetTileController(x, y));
            _map[x, y].Position = new Position(x, y);
        }

        private void LoadPigeon() {
            IList<IPigeonController> controllers = _mapController.GetPigeonControllers();
            foreach (IPigeonController controller in controllers) {
                Position pigeonPosition = controller.CurrentPosition;
                PigeonView pigeon = Instantiate(Pigeon, new Vector3(_tileSizeX * pigeonPosition.X  - 1f, _tileSizeY * pigeonPosition.Y - 2.5f, 1) * 1.6f, Quaternion.identity);
                pigeon.SetDimensions(_tileSizeX, _tileSizeY);
                pigeon.SetController(controller);
                _pigeons.Add(pigeon);
            }
        }

        private void RemovePigeon(TileView tile) {
            Position tilePos = tile.Position;
            PigeonView pigeonToDelete = null;
            foreach (PigeonView pigeon in _pigeons) {
                if (pigeon.GetPosition().X == tilePos.X && pigeon.GetPosition().Y == tilePos.Y) {
                    pigeonToDelete = pigeon;
                }
            }
            if(pigeonToDelete != null) {
                _pigeons.Remove(pigeonToDelete);
                Destroy(pigeonToDelete.gameObject);
            }
        }

        private void RemoveFire(TileView tile) {
            Position tilePos = tile.Position;
            _mapController.ReduceHeat(tilePos.X, tilePos.Y);
            Position firePosToDelete = null;
            foreach (Position firePos in _firePositions) {
                if (firePos.X == tilePos.X && firePos.Y == tilePos.Y) {
                    firePosToDelete = firePos;
                }
            }
            if(firePosToDelete != null) {
                _firePositions.Remove(firePosToDelete);
            }
        }
    }
}
