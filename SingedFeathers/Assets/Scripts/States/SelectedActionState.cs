using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedActionState : IGameState {

	private readonly GameStateManager manager;

	public SelectedActionState(GameStateManager mapManager) {
		manager = mapManager;
	}

	public void UpdateState() {

	}

	public void ChangeState() { manager.currState = manager.appliedActionState; }
	public void Undo() {}
	public void HandleMapInput (TileManager tileManager) {
		manager.mapManager.AddCommand (new SetFireCommand(tileManager));
		ChangeState ();
	}
}
