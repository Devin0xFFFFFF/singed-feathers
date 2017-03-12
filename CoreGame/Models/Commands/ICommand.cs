using Assets.Scripts.Controllers;
using System;

namespace Assets.Scripts.Models.Commands {
    public interface ICommand {
        void ExecuteCommand(ITileController tileController);
        bool CanBeExecutedOnTile(ITileController tileController);
        MoveType GetMoveType();
        Command GetCommand();
    }
}