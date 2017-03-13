using Assets.Scripts.Models;
using Assets.Scripts.Models.Commands;
using System.Collections.Generic;

namespace Assets.Scripts.Controllers {
    public interface ITurnController {
        void SetMoveType(MoveType moveType);
        bool ProcessAction(ITileController tileController);
        void UndoAllActions();
        void ClearTile(ITileController tileController);
        bool CanTakeAction();
        bool HasQueuedActions();
        int GetTurnsLeft();
        bool HasTurnsLeft();
        IDictionary<ITileController, ICommand> GetAndResetMoves();
        MoveType GetMoveType();
    }
}