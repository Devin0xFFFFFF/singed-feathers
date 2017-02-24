using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnselectedActionState : IGameState {

    private readonly GameStateManager manager;

    public UnselectedActionState(GameStateManager mapManager) {
        manager = mapManager;
    }

    public void UpdateState() {

    }

    public void ChangeState() { manager.currState = manager.selectedActionState; }
    public void Undo() {}
    public void HandleMapInput (TileManager tileManager) {
        manager.GetTileInfo (tileManager);
    }
}
