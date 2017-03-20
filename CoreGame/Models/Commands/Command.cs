using CoreGame.Controllers.Interfaces;
using System;

namespace CoreGame.Models.Commands {

    [Serializable]
    public class Command : ICommand {
        public MoveType MoveType { get; }
        public int Heat { get; }

        public Command(MoveType moveType, int heat = 0) {
            this.MoveType = moveType;
            Heat = Math.Max(0, heat);
        }

        public int CompareTo(ICommand other) { return MoveType.CompareTo(other.MoveType); }

        public void ExecuteCommand(ITileController tileController) {
            switch (MoveType) {
                case MoveType.Fire:
                    tileController.ApplyHeat(Heat);
                    break;
                case MoveType.Water:
                    tileController.ReduceHeat(Heat);
                    break;
            }
        }

        public bool CanBeExecutedOnTile(ITileController tileController) {
            switch (MoveType) {
                case MoveType.Fire:
                    return tileController.IsFlammable() && !tileController.IsOccupied && !tileController.IsOnFire();
                case MoveType.Water:
                    return tileController.IsFlammable();
            }
            return false;
        }
    }
}
