using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers {
    public class PigeonController : IPigeonController {
        public const int FIRE_DAMAGE = 10;
        public Position CurrentPosition { get { return _tileController.Position; } }
        public Position InitialPosition { get; private set; }
        private ITileController _tileController;
        private readonly Pigeon _pigeon;

        public PigeonController(ITileController tileController) {
            _pigeon = new Pigeon();
            _tileController = tileController;
            InitialPosition = _tileController.Position;
        }

        public bool HasMoved() { return InitialPosition != CurrentPosition; }

        public bool IsDead() { return _pigeon.Health <= 0; }

        public void React() {
            Move();
            UpdateHealth();
        }

        public bool Move() {
            InitialPosition = _tileController.Position;
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