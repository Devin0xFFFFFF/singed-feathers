using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Managers {
    public class MapManager : MonoBehaviour {
        public List<TileManager> TileSet;
        public PigeonManager Pigeon;
        private List<PigeonManager> _pigeons;
        private Dictionary<TileType, TileManager> _tileDictionary;
        private IMapController _mapController;
        private TileManager[,] _map;
        private int _width, _height;
        private float _tileSizeX, _tileSizeY;

        // Start here!
        public void Start() {
            if (TileSet.Count > 0) {
                LoadTileDictionary();
                LoadMap();
                LoadFires();
                LoadPigeons();
            }
        }

        // Update is called once per frame
        public void Update() {
        }

        public void LoadTileDictionary() {
            _tileDictionary = new Dictionary<TileType, TileManager>();
            foreach (TileManager tile in TileSet) {
                _tileDictionary.Add(tile.type, tile);
            }
        }

        public void LoadMap() {
            _mapController = new MapController();
            _mapController.GenerateMap();
            _width = _mapController.Width;
            _height = _mapController.Height;
            _map = new TileManager[_width, _height];

            _tileSizeX = TileSet[0].GetComponent<Renderer>().bounds.size.x;
            _tileSizeY = TileSet[0].GetComponent<Renderer>().bounds.size.y;

            InstantiateTiles();
        }

        public void LoadFires() {
            Position initialFirePosition = _mapController.GetInitialFirePosition();
            SetFire(initialFirePosition.X, initialFirePosition.Y);
        }

        public void LoadPigeons() {
            _pigeons = new List<PigeonManager>();
            IList<IPigeonController> controllers = _mapController.GetPigeonControllers();
            foreach (IPigeonController controller in controllers) {
                Position pigeonPosition = controller.CurrentPosition;
                PigeonManager pigeon = Instantiate(Pigeon, new Vector3(_tileSizeX * pigeonPosition.X, _tileSizeY * pigeonPosition.Y, 1), Quaternion.identity);
                pigeon.SetDimensions(_tileSizeX, _tileSizeY);
                pigeon.SetController(controller);
                _pigeons.Add(pigeon);
            }
        }

        public void ProcessTurn() {
            Debug.Log("Resolving turn: " + _mapController.GetTurnController().GetTurnsLeft());

            _mapController.EndTurn();

            IDictionary<NewStatus, IList<Position>> modifiedTilePositions = _mapController.SpreadFires();
            foreach (Position pos in modifiedTilePositions[NewStatus.BurntOut]) {
                UpdateTileType(TileType.Ash, pos.X, pos.Y);
            }

            _mapController.MovePigeons();
            foreach (PigeonManager pigeon in _pigeons) {
                pigeon.UpdatePigeon();
            }
        }

        public void SetFire(int x, int y) { _mapController.ApplyHeat(x, y); }

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
            _map[x, y].SetController(_mapController.GetTileController(x, y));
        }

        private void UpdateTileType(TileType type, int x, int y) {
            Destroy(_map[x, y].gameObject);
            InstantiateTile(type, x, y);
        }

        public ITurnController GetTurnController() {
            return _mapController.GetTurnController();
        }

        public ITurnResolver GetTurnResolver() {
            return _mapController.GetTurnResolver();
        }

        public void Undo() {
            _mapController.GetTurnController().UndoAllActions();
        }

        public void Fire() {
            _mapController.GetTurnController().SetMoveType(MoveTypes.Fire);
        }

        public void Blank() {
            _mapController.GetTurnController().SetMoveType(MoveTypes.Blank);
        }

        public void Water() {
            _mapController.GetTurnController().SetMoveType(MoveTypes.Water);
        }
    }
}