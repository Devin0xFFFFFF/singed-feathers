using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppliedActionState : IGameState {

    private readonly GameStateManager manager;

    public AppliedActionState(GameStateManager mapManager) {
        manager = mapManager;
    }

    public void UpdateState() {

    }

    public void ChangeState() { manager.currState = manager.resolveState; }
    public void Undo() { 
        manager.mapManager.UndoLastCommand ();
        manager.currState = manager.unselectedActionState;
    }

    public void HandleMapInput (TileManager tileManager) {
        manager.GetTileInfo (tileManager);
    }
}
