using Assets.Scripts.Controllers;
using System;

namespace Assets.Scripts.Models.Commands {
    public interface ICommand : IComparable<ICommand> {
        MoveType MoveType { get; }
        int Heat { get; }
        void ExecuteCommand(ITileController tileController);
        bool CanBeExecutedOnTile(ITileController tileController);
    }
}
