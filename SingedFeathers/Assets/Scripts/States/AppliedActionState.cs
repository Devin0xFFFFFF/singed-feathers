namespace Assets.Scripts.States {
	public class AppliedActionState : IGameState {
	    private readonly GameStateManager _gameStateManager;

	    public AppliedActionState(GameStateManager mapManager) { _gameStateManager = mapManager; }

	    public void UpdateState() {}

	    public void ChangeState() { _gameStateManager.CurrState = _gameStateManager.ResolveState; }

	    public void Undo() { 
	        _gameStateManager.MapManager.UndoLastCommand();
	        _gameStateManager.CurrState = _gameStateManager.UnselectedActionState;
	    }
			
	    public void HandleMapInput(TileManager tileManager) { _gameStateManager.GetTileInfo(tileManager); }
	}
}

