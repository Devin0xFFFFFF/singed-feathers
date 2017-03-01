using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers {
    public class PigeonController : IPigeonController {
        public const int FIRE_DAMAGE = 10;
        private ITileController _tileController;
        private Pigeon _pigeon;
        private Position _initialPosition;

        public PigeonController(ITileController tileController) {
            _pigeon = new Pigeon();
            _tileController = tileController;
        }

        public bool IsDead() { return _pigeon.Health <= 0; }
        
        public Position CurrentPosition() { return _tileController.Position; }

        public bool Move() {
            if (_tileController.IsOnFire()) {
                foreach (ITileController neighbour in _tileController.GetNeighbours()) {
                    if (!neighbour.IsOnFire()) {
                        _tileController = neighbour;
                        return true;
                    }
                }
            }
            return false;
        }

        public void UpdateHealth() {
            if (_tileController.IsOnFire()) {
                _pigeon.Health -= FIRE_DAMAGE * 2;
            }

            foreach (ITileController neighbour in _tileController.GetNeighbours()) {
                if (neighbour.IsOnFire()) {
                    _pigeon.Health -= FIRE_DAMAGE;
                }
            }
        }
    }
}
