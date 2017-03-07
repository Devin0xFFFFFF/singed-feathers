using Assets.Scripts.Controllers;
using System;

namespace Assets.Scripts.Models.Commands {
	public class SetFireCommand : ICommand {
	    private readonly int _heat;

	    public SetFireCommand(int heat) { _heat = Math.Max(0, heat); }

	    public void ExecuteCommand(ITileController tileController) { tileController.ApplyHeat(_heat); }

        public bool CanBeExecutedOnTile(ITileController tileController) { return tileController.IsFlammable() && !tileController.IsOccupied; }

        public MoveType GetMoveType() { return MoveType.Fire; }
    }
}