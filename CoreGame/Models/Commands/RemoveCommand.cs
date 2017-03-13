using CoreGame.Controllers.Interfaces;

namespace CoreGame.Models.Commands {
    class RemoveCommand : ICommand {
        public void ExecuteCommand(ITileController tileController) {}

        public bool CanBeExecutedOnTile(ITileController tileController) { return false; }

        public MoveType GetMoveType() { return MoveType.Remove; }

        public Command GetCommand() { return new Command(MoveType.Remove); }
    }
}
