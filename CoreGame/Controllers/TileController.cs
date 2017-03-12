using System;
using System.Collections.Generic;
using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers {
    [Serializable]
    public class TileController : ITileController {
        public const int BURN_HEAT = 25;
        public const int HEAT_LOSS = 200;
        public Position Position { get; set; }
        public bool StateHasChanged { get; set; }
        public bool HasVisualChange { get; private set; }
        public bool IsOccupied { get; private set; }
        public Tile Tile { get; private set; }
        private readonly IList<ITileController> _neighbouringTiles;

        public TileController(TileType type, int x, int y) {
            Position = new Position(x, y);
            StateHasChanged = false;
            IsOccupied = false;
            Tile = InitializeTile(type);
            _neighbouringTiles = new List<ITileController>();
        }

        public TileType GetTileType() { return Tile.Type; }

        public bool IsFlammable() { return Tile.FlashPoint < int.MaxValue && !IsBurntOut(); }

        public bool IsSpreadingHeat() {
            // Spread heat if the tile has been burning for more than one turn (at the end of the last turn)
            return (StateHasChanged && IsBurntOut()) || (!StateHasChanged && IsOnFire());
        }

        public bool IsOnFire() { return IsFlammable() && Tile.Heat >= Tile.FlashPoint && !IsBurntOut(); }

        public bool IsBurntOut() { return Tile.Type == TileType.Ash || (Tile.TurnsOnFire > 0 &&  Tile.TurnsOnFire >= Tile.MaxTurnsOnFire); }

        public bool HasVisualStateChange() { return HasVisualChange || StateHasChanged; }

        public void AddNeighbouringTile(ITileController neighbourController) { _neighbouringTiles.Add(neighbourController); }

        public IEnumerable<ITileController> GetNeighbours() { return _neighbouringTiles; }

        // Calculate which sprite index the TileView should be using (for heat level indication)
        public int GetSpriteHeatFrame() {
            int heat = Tile.Heat;
            int frame = heat / BURN_HEAT;
            if (frame > 3) {
                frame = 3;
            }
            return frame;
        }

        public void SpreadFire() {
            bool startedOnFire = IsOnFire();

            // Apply heat for each neighbouring tile that is spreading heat
            bool neighbourAppliedHeat = false;
            foreach (ITileController neighbour in _neighbouringTiles) {
                if (neighbour.IsSpreadingHeat()) {
                    ApplyHeat(BURN_HEAT);
                    neighbourAppliedHeat = true;
                }
            }

            if (IsOnFire()) {
                Tile.TurnsOnFire += 1;
                if (startedOnFire && IsBurntOut()) {
                    int tmpHeat = Tile.Heat;
                    Tile.Type = TileType.Ash;
                    Tile.FlashPoint = int.MaxValue;
                    Extinguish();
                    Tile.Heat = tmpHeat;
                    StateHasChanged = true;
                }
            } else if (!neighbourAppliedHeat) {
                ReduceHeat(HEAT_LOSS);
                HasVisualChange = true;
            }
        }

        public void Extinguish() {
            bool startedOnFire = IsOnFire();
            Tile.Heat = 0;
            Tile.TurnsOnFire = 0;
            if (startedOnFire) {
                StateHasChanged = true;
            }
        }

        public void ApplyHeat(int heat) {
            bool startedOnFire = IsOnFire();
            Tile.Heat += heat;
            if (!startedOnFire && IsOnFire()) {
                StateHasChanged = true;
            }
        }

        public void ReduceHeat(int heat) {
            bool startedOnFire = IsOnFire();
            Tile.Heat = Math.Max(0, Tile.Heat - heat);
            if (startedOnFire && !IsOnFire()) {
                StateHasChanged = true;
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
    }
}
