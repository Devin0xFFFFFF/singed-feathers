using Assets.Scripts.Models;

namespace Assets.Scripts.Controllers {
    public interface IPigeonController {
        bool IsDead();
        bool Move();
        void UpdateHealth();
        Position CurrentPosition();
    }
}