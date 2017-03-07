using Assets.Scripts.Controllers;

namespace Assets.Scripts.Models.Commands {
    public interface ICommand {
        void ExecuteCommand(ITileController tileController);

        bool CanBeExecutedOnTile(ITileController tileController);

        MoveType GetMoveType();
    }
}