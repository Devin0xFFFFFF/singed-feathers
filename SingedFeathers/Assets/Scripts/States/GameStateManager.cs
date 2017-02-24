using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour {

    public MapManager mapManager;

    [HideInInspector] public IGameState currState;
    [HideInInspector] public UnselectedActionState unselectedActionState;
    [HideInInspector] public SelectedActionState selectedActionState;
    [HideInInspector] public AppliedActionState appliedActionState;
    [HideInInspector] public ResolveState resolveState;

    void Awake() {
        unselectedActionState = new UnselectedActionState (this);
        selectedActionState = new SelectedActionState (this);
        appliedActionState = new AppliedActionState (this);
        resolveState = new ResolveState (this);
    }

    // Use this for initialization
    void Start () {
        currState = unselectedActionState;
    }
    
    // Update is called once per frame
    void Update () {
        currState.UpdateState();
    }

    public void GetTileInfo(TileManager tileManager) {
        //call MapManager.getTileInfo(tileManager);
    }

    public void ChangeState() {
        currState.ChangeState ();
    }

    public void Undo() {
        currState.Undo ();
    }

    public void HandleMapInput(TileManager tileManager) {
        currState.HandleMapInput(tileManager);
    }
}
