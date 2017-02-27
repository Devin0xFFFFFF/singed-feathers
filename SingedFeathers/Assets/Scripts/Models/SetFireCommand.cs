public class SetFireCommand : ICommand {
    private TileManager _tileManager;

    public SetFireCommand(TileManager tileManager) { _tileManager = tileManager; }

    public void ExecuteCommand() { _tileManager.ApplyHeat(100); }
}