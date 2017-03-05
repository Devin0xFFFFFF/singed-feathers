using System.Collections.Generic;
using Assets.Scripts.Controllers;
using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.Managers {
    public class MapView : MonoBehaviour {
        public List<TileView> TileSet;
        public PigeonView Pigeon;
        public InputView InputView;
        private List<PigeonView> _Pigeons;
        private Dictionary<TileType, TileView> _TileDictionary;
        private IMapController _MapController;
        private TileView[,] _Map;
        private int _Width, _Height;
        private float _TileSizeX, _TileSizeY;

        // Start here!
        public void Start() {
            if (TileSet.Count > 0) {
                LoadTileDictionary();
                LoadMap();
                LoadFires();
                LoadPigeons();
                LoadInputView();
            }
        }

        // Update is called once per frame
        public void Update() { }

        public void LoadTileDictionary() {
            _TileDictionary = new Dictionary<TileType, TileView>();
            foreach (TileView tile in TileSet) {
                _TileDictionary.Add(tile.type, tile);
            }
        }

        public void LoadMap() {
            _MapController = new MapController();
            _MapController.GenerateMap();
            _Width = _MapController.Width;
            _Height = _MapController.Height;
            _Map = new TileView[_Width, _Height];

            _TileSizeX = TileSet[0].GetComponent<Renderer>().bounds.size.x;
            _TileSizeY = TileSet[0].GetComponent<Renderer>().bounds.size.y;

            InstantiateTiles();
        }

        public void LoadFires() {
            Position initialFirePosition = _MapController.GetInitialFirePosition();
            SetFire(initialFirePosition.X, initialFirePosition.Y);
        }

        public void LoadInputView() {
            InputView.SetTurnController(_MapController.GetTurnController());
            InputView.SetTurnResolver(_MapController.GetTurnResolver());
        }

        public void LoadPigeons() {
            _Pigeons = new List<PigeonView>();
            IList<IPigeonController> controllers = _MapController.GetPigeonControllers();
            foreach (IPigeonController controller in controllers) {
                Position pigeonPosition = controller.CurrentPosition;
                PigeonView pigeon = Instantiate(Pigeon, new Vector3(_TileSizeX * pigeonPosition.X, _TileSizeY * pigeonPosition.Y, 1), Quaternion.identity);
                pigeon.SetDimensions(_TileSizeX, _TileSizeY);
                pigeon.SetController(controller);
                _Pigeons.Add(pigeon);
            }
        }

        public void ProcessTurn() {
            Debug.Log("Resolving turn: " + _MapController.GetTurnsLeft());

            _MapController.EndTurn();

            IDictionary<NewStatus, IList<Position>> modifiedTilePositions = _MapController.SpreadFires();
            foreach (Position pos in modifiedTilePositions[NewStatus.BurntOut]) {
                UpdateTileType(TileType.Ash, pos.X, pos.Y);
            }

            _MapController.MovePigeons();
            foreach (PigeonView pigeon in _Pigeons) {
                pigeon.UpdatePigeon();
            }
        }

        public void SetFire(int x, int y) { _MapController.ApplyHeat(x, y); }

        private void InstantiateTiles() {
            for (int x = 0; x < _Width; x++) {
                for (int y = 0; y < _Height; y++) {
                    InstantiateTile(_MapController.GetTileType(x, y), x, y);
                }
            }
        }

        private void InstantiateTile(TileType type, int x, int y) {
            TileView manager = _TileDictionary[type];
            _Map[x, y] = Instantiate(manager, new Vector3(_TileSizeX * x, _TileSizeY * y, 1), Quaternion.identity);
            _Map[x, y].SetController(_MapController.GetTileController(x, y));
        }

        private void UpdateTileType(TileType type, int x, int y) {
            Destroy(_Map[x, y].gameObject);
            InstantiateTile(type, x, y);
        }

        public ITurnController GetTurnController() { return _MapController.GetTurnController(); }

        public ITurnResolver GetTurnResolver() { return _MapController.GetTurnResolver(); }

        public void UndoAll() { _MapController.UndoAllActions(); }

        public void Fire() { _MapController.Fire(); }

        public void Water() { _MapController.Water(); }

        public void Cancel() { _MapController.Cancel(); }
    }
}
