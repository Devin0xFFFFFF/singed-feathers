using System.Collections.Generic;
using System.Linq;
using System;
using CoreGame.Controllers.Interfaces;
using CoreGame.Models;

namespace CoreGame.Controllers {
    public class PigeonController : IPigeonController {
        public const int FIRE_DAMAGE = 10;
        public Position CurrentPosition { get { return _tileController.Position; } }
        public Position InitialPosition { get; private set; }
        public Pigeon Pigeon { get; }
        private ITileController _tileController;

        public PigeonController(ITileController tileController) {
            this.Pigeon = new Pigeon();
            _tileController = tileController;
            InitialPosition = _tileController.Position;
        }

        public int GetHealth() { return Pigeon.Health; }
        
        public bool HasMoved() { return InitialPosition != CurrentPosition; }

        public bool IsDead() { return Pigeon.Health <= 0; }

        public void Heal(int delta) {
            if (delta > 0) {
                Pigeon.Health = Math.Min(Pigeon.MAX_HEALTH, Pigeon.Health + delta);
            }
        }

        public void InflictDamage(int delta) {
            if (delta > 0) {
                Pigeon.Health = Math.Max(0, Pigeon.Health - delta);
            }
        }

        public void React() {
            if (!IsDead()) {
                Move();
                TakeFireDamage();
            }
        }

        public bool Kill() {
            if (!IsDead()) {
                Pigeon.Health = 0;
                _tileController.MarkUnoccupied();
                return true;
            }
            return false;
        }

        public bool Move() {
            InitialPosition = _tileController.Position;
            IList<ITileController> allPossibleDestinations = new List<ITileController>(_tileController.GetNeighbours());
            allPossibleDestinations.Add(_tileController);

            int maxHeat = allPossibleDestinations.Max(tile => tile.GetTileHeat());
            IEnumerable<ITileController> validDestinations = allPossibleDestinations.Where(tile => !tile.IsOnFire() && tile.CanBeOccupied());

            if (maxHeat > 0 && validDestinations.Any()) {
                // Move to the tile with min heat farthest from the most heated tile
                Position maxHeatPosition = allPossibleDestinations.Where(
                    tile => tile.GetTileHeat() == maxHeat).OrderByDescending(
                        tile => tile).First().Position;

                int minHeat = validDestinations.Min(tile => tile.GetTileHeat());
                validDestinations = validDestinations.Where(tile => tile.GetTileHeat() == minHeat);
                ITileController bestDestination = validDestinations.OrderByDescending(tile => maxHeatPosition.GetLargestDistanceFrom(tile.Position)).First();

                bool moved = bestDestination != _tileController;

                _tileController.MarkUnoccupied();
                _tileController = bestDestination;
                _tileController.MarkOccupied();

                return moved;
            } else {
                // Stay still
                return false;
            }
        }

        public void TakeFireDamage() {
            if (_tileController.IsOnFire()) {
                InflictDamage(FIRE_DAMAGE * 2);
            }

            foreach (ITileController neighbour in _tileController.GetNeighbours()) {
                if (neighbour.IsOnFire()) {
                    InflictDamage(FIRE_DAMAGE);
                }
            }

            if (IsDead()) {
                _tileController.MarkUnoccupied();
            }
        }
    }
}