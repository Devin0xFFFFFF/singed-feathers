using Assets.Scripts.Controllers;

namespace Assets.Scripts.Models.Commands {
    class BlankCommand : ICommand {
        public void ExecuteCommand(ITileController tileController) {}

        public bool CanBeExecutedOnTile(ITileController tileController) { return false; }

        public MoveTypes GetMoveType() { return MoveTypes.Blank; }
    }
}
