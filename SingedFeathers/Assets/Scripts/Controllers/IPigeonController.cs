using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers {
    public interface IPigeonController {
        Position InitialPosition { get; }
        Position CurrentPosition { get; }
        int Health { get; }
        bool IsDead();
        bool Kill();
        bool Move();
        void UpdateHealth();
        bool HasMoved();
        void React();
    }
}