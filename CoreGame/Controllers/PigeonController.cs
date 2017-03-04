using System;
using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers {
    public class PigeonController : IPigeonController {
        public const int FIRE_DAMAGE = 10;
        public Position CurrentPosition { get { return _tileController.Position; } }
        public Position InitialPosition { get; private set; }
        public Pigeon Pigeon { get; }
        private ITileController _tileController;

        public PigeonController(ITileController tileController) {
            Pigeon = new Pigeon();
            _tileController = tileController;
            InitialPosition = _tileController.Position;
        }

        public int GetHealth() { return Pigeon.Health; }
        
        public bool HasMoved() { return InitialPosition != CurrentPosition; }

        public bool IsDead() { return Pigeon.Health <= 0; }

        public void Heal(int delta) {
            if (delta > 0) {
                _pigeon.Health = Math.Min(Pigeon.MAX_HEALTH, _pigeon.Health + delta);
            }
        }

        public void InflictDamage(int delta) {
            if (delta > 0) {
                _pigeon.Health = Math.Max(0, _pigeon.Health - delta);
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
            if (_tileController.IsOnFire()) {
                foreach (ITileController neighbour in _tileController.GetNeighbours()) {
                    if (!neighbour.IsOnFire() && neighbour.CanBeOccupied()) {
                        // Move to new tile
                        _tileController.MarkUnoccupied();
                        _tileController = neighbour;
                        _tileController.MarkOccupied();
                        return true;
                    }
                }
            }
            return false;
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