using Assets.Scripts.Controllers;

namespace Assets.Scripts.Models.Commands {
    public class AddWaterCommand : ICommand {
        private readonly int _heat;

        public AddWaterCommand(int heat) { _heat = heat; }

        public void ExecuteCommand(ITileController tileController) { tileController.ReduceHeat(_heat); }

        public bool CanBeExecutedOnTile(ITileController tileController) { return tileController.IsFlammable() && tileController.HasPositiveHeat(); }

        public MoveTypes GetMoveType() { return MoveTypes.Water; }
    }
}