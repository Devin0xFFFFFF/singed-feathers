using System;
using System.Collections.Generic;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;

namespace CoreGame.Controllers {
    [Serializable]
    public class TileController : ITileController {
        public const int BURN_HEAT = 10;
        public Position Position { get; set; }
        public bool IsOccupied { get; private set; }
        public Tile Tile { get; private set; }
        private readonly IList<ITileController> _neighbouringTiles;

        public TileController(TileType type, int x, int y) {
            Position = new Position(x, y);
            IsOccupied = false;
            Tile = InitializeTile(type);
            _neighbouringTiles = new List<ITileController>();
        }

        public TileType GetTileType() { return Tile.Type; }

        public int GetTileHeat() { return Tile.Heat; }

        public bool IsFlammable() { return Tile.FlashPoint < int.MaxValue && !IsBurntOut(); }

        public bool IsOnFire() { return Tile.IsOnFire; }

        public bool IsBurntOut() { return Tile.TurnsOnFire > 0 &&  Tile.TurnsOnFire >= Tile.MaxTurnsOnFire; }

        public void AddNeighbouringTile(ITileController neighbourController) { _neighbouringTiles.Add(neighbourController); }

        public IEnumerable<ITileController> GetNeighbours() { return _neighbouringTiles; }

        public void SpreadFire() {
            if (IsOnFire()) {
                foreach (ITileController neighbour in _neighbouringTiles) {
                    neighbour.ApplyHeat(BURN_HEAT);
                }
            }
        }

        public void UpKeep() {
            if (IsOnFire()) {
                Tile.TurnsOnFire++;
            }

            Tile.IsOnFire = CheckIfOnFire();
        }

        public void ApplyHeat(int heat) {
            Tile.Heat += heat;
        }

        public void ReduceHeat(int heat) {
            Tile.Heat = Math.Max(0, Tile.Heat - heat);
            if (!CheckIfOnFire()) {
                Tile.IsOnFire = false;
            }
        }

        public bool MarkOccupied() {
            if (CanBeOccupied()) {
                IsOccupied = true;
                return true;
            }
            return false;
        }

        public bool MarkUnoccupied() {
            if (IsOccupied) {
                IsOccupied = false;
                return true;
            }
            return false;
        }

        public bool CanBeOccupied() { return !IsOccupied; }

        public bool IsHeatZero() { return Tile.Heat == 0; }

        private Tile InitializeTile(TileType type) {
            switch (type) {
                case TileType.Wood:
                    return new Tile(type, 20, 3);
                case TileType.Grass:
                    return new Tile(type, 10, 3);
                case TileType.Stone:
                case TileType.Ash:
                case TileType.Error:
                default:
                    return new Tile(type, int.MaxValue, 0);
            }
        }

        private bool CheckIfOnFire() { return IsFlammable() && Tile.Heat >= Tile.FlashPoint && !IsBurntOut(); }
    }
}
