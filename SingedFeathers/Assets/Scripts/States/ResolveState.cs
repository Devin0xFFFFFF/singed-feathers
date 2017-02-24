using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolveState : IGameState {

    private readonly GameStateManager manager;

    public ResolveState(GameStateManager mapManager) {
        manager = mapManager;
    }

    public void UpdateState() {
        
    }

    public void ChangeState() { manager.currState = manager.unselectedActionState; }
    public void Undo() {}
    public void HandleMapInput (TileManager tileManager) {
        manager.GetTileInfo (tileManager);
    }
}
