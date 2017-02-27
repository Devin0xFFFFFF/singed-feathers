namespace Assets.Scripts.Models {
	public class SetFireCommand : ICommand {
	    private readonly TileManager _tileManager;

	    public SetFireCommand(TileManager tileManager) { _tileManager = tileManager; }

	    public void ExecuteCommand() { _tileManager.ApplyHeat(100); }
	}
}
