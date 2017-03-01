using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers {
    public interface IPigeonController{
        Position InitialPosition { get; }
        Position CurrentPosition { get; }
        bool IsDead();
        bool Move();
        void UpdateHealth();
        bool HasMoved();
        void React();
    }
}