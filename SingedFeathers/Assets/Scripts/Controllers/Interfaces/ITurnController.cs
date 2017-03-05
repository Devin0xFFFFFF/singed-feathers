using Assets.Scripts.Models;
using Assets.Scripts.Models.Commands;
using System.Collections.Generic;

namespace Assets.Scripts.Controllers {
    public interface ITurnController {
        void SetMoveType(MoveTypes moveType);
        //void UpdateIntensity(int intensity);
        void ProcessAction(ITileController tileController);
        void UndoAllActions();
        void ClearTile(ITileController tileController);
        bool CanTakeAction();
        bool HasQueuedActions();
        int GetTurnsLeft();
        bool HasTurnsLeft();
        IDictionary<ITileController, ICommand> GetAndResetMoves();
        MoveTypes GetMoveType();
    }
}