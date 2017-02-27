namespace Assets.Scripts.States {
    public interface IGameState {
        void UpdateState();
        void ChangeState();
        void Undo();
        void HandleMapInput(TileManager tileManager);
    }
}
