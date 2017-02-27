namespace Assets.Scripts.States {
	public class ResolveState : IGameState {
	    private readonly GameStateManager _gameStateManager;

	    public ResolveState(GameStateManager mapManager) { _gameStateManager = mapManager; }

	    public void UpdateState() {}

	    public void ChangeState() { _gameStateManager.CurrState = _gameStateManager.UnselectedActionState; }

	    public void Undo() {}

	    public void HandleMapInput(TileManager tileManager) { _gameStateManager.GetTileInfo(tileManager); }
	}
}
