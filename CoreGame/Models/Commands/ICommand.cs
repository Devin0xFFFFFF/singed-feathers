using CoreGame.Controllers.Interfaces;
using System;

namespace CoreGame.Models.Commands {
    public interface ICommand : IComparable<ICommand> {
        MoveType MoveType { get; }
        int Heat { get; }
        void ExecuteCommand(ITileController tileController);
        bool CanBeExecutedOnTile(ITileController tileController);
        string GetExecutionFailureReason(ITileController tileController);
    }
}
