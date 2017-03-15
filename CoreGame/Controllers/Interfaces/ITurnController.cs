using System.Collections.Generic;
using CoreGame.Models;
using CoreGame.Models.Commands;

namespace CoreGame.Controllers.Interfaces {
    public interface ITurnController {
        void SetMoveType(MoveType moveType);
        bool ProcessAction(ITileController tileController);
        void UndoLastAction();
        void ClearTile(ITileController tileController);
        bool CanTakeAction();
        bool HasQueuedActions();
        int GetTurnsLeft();
        bool HasTurnsLeft();
        IDictionary<ITileController, ICommand> GetAndResetMoves();
        MoveType GetMoveType();
    }
}