using CoreGame.Models;

namespace CoreGame.Controllers.Interfaces {
    public interface ITurnController {
        void SetMoveType(MoveType moveType);
        bool ProcessAction(ITileController tileController);
        void UndoAction();
        bool HasQueuedAction();
        int GetTurnsLeft();
        bool HasTurnsLeft();
        Delta GetAndResetMove();
        MoveType GetMoveType();
        string GetExecutionFailureReason(ITileController tileController);
    }
}