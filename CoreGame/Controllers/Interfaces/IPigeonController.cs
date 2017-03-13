using CoreGame.Models;

namespace CoreGame.Controllers.Interfaces {
    public interface IPigeonController {
        Position InitialPosition { get; }
        Position CurrentPosition { get; }
        Pigeon Pigeon { get; }
        int GetHealth();
        void Heal(int delta);
        void InflictDamage(int delta);
        bool IsDead();
        bool Kill();
        bool Move();
        void TakeFireDamage();
        bool HasMoved();
        void React();
    }
}