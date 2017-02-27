using UnityEngine;

namespace Assets.Scripts.States {
	public class GameStateManager : MonoBehaviour {
	    public MapManager MapManager;
	    [HideInInspector] public IGameState CurrState;
	    [HideInInspector] public UnselectedActionState UnselectedActionState;
	    [HideInInspector] public SelectedActionState SelectedActionState;
	    [HideInInspector] public AppliedActionState AppliedActionState;
	    [HideInInspector] public ResolveState ResolveState;

	    public void Awake() {
	        UnselectedActionState = new UnselectedActionState(this);
	        SelectedActionState = new SelectedActionState(this);
	        AppliedActionState = new AppliedActionState(this);
	        ResolveState = new ResolveState(this);
	    }

	    // Use this for initialization
	    public void Start() { CurrState = UnselectedActionState; }
	    
	    // Update is called once per frame
	    public void Update() { CurrState.UpdateState(); }

	    public void GetTileInfo(TileManager tileManager) {}

	    public void ChangeState() { CurrState.ChangeState(); }

	    public void Undo() { CurrState.Undo(); }

	    public void EndTurn() { CurrState = ResolveState; }

	    public void HandleMapInput(TileManager tileManager) { CurrState.HandleMapInput(tileManager); }

	    public int GetNumberOfTurns() { return MapManager.GetNumberOfTurns(); }
	}
}
