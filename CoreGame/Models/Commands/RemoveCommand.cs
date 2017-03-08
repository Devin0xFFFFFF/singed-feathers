using Assets.Scripts.Controllers;

namespace Assets.Scripts.Models.Commands {
    class RemoveCommand : ICommand {
        public void ExecuteCommand(ITileController tileController) {}

        public bool CanBeExecutedOnTile(ITileController tileController) { return false; }

        public MoveType GetMoveType() { return MoveType.Remove; }

    }
}
