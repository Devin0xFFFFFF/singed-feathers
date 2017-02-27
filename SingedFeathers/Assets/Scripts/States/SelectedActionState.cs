using Assets.Scripts.Models;

namespace Assets.Scripts.States {
	public class SelectedActionState : IGameState {
	    private readonly GameStateManager _gameStateManager;

	    public SelectedActionState(GameStateManager mapManager) { _gameStateManager = mapManager; }

	    public void UpdateState() {}

	    public void ChangeState() { _gameStateManager.CurrState = _gameStateManager.AppliedActionState; }

	    public void Undo() {}

	    public void HandleMapInput(TileManager tileManager) {
	        _gameStateManager.MapManager.AddCommand(new SetFireCommand(tileManager));
	        ChangeState();
	    }
	}
}
