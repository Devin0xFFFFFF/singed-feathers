using Assets.Scripts.Controllers;

namespace Assets.Scripts.Models.Commands {
	public class SetFireCommand : ICommand {
	    private readonly int _heat;

	    public SetFireCommand(int heat) { _heat = heat; }

	    public void ExecuteCommand(ITileController tileController) { tileController.ApplyHeat(_heat); }

        public bool CanBeExecutedOnTile(ITileController tileController) { return tileController.IsFlammable(); }
    }
}