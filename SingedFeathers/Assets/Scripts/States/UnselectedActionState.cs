using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnselectedActionState : IGameState {

    private readonly GameStateManager _gameStateManager;

    public UnselectedActionState(GameStateManager mapManager) { _gameStateManager = mapManager; }

    public void UpdateState() {}

    public void ChangeState() { _gameStateManager.CurrState = _gameStateManager.SelectedActionState; }

    public void Undo() {}

    public void HandleMapInput(TileManager tileManager) { _gameStateManager.GetTileInfo(tileManager); }
}
