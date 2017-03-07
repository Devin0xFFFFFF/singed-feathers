using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers {
    public interface IPigeonController {
        Position InitialPosition { get; }
        Position CurrentPosition { get; }
        int Health { get; }
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