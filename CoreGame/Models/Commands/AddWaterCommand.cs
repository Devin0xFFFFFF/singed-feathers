using Assets.Scripts.Controllers;
using System;

namespace Assets.Scripts.Models.Commands {
    public class AddWaterCommand : ICommand {
        private readonly int _heat;

        public AddWaterCommand(int heat) { _heat = Math.Max(0, heat); }

        public void ExecuteCommand(ITileController tileController) { tileController.ReduceHeat(_heat); }

        public bool CanBeExecutedOnTile(ITileController tileController) { return tileController.IsFlammable() && !tileController.IsHeatZero(); }

        public MoveType GetMoveType() { return MoveType.Water; }

        public Command GetCommand() { return new Command(GetMoveType(), _heat); }
    }
}