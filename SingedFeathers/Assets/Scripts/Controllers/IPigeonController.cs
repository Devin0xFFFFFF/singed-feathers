using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers {
    public interface IPigeonController {
        Position InitialPosition { get; }
        Position CurrentPosition { get; }
        Pigeon Pigeon { get; }
        int GetHealth();
        bool IsDead();
        bool Kill();
        bool Move();
        void UpdateHealth();
        bool HasMoved();
        void React();
    }
}