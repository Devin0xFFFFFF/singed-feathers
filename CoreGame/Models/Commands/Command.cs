using CoreGame.Controllers.Interfaces;
using System;

namespace CoreGame.Models.Commands {

    [Serializable]
    public class Command : IComparable<Command> {
        public const string ACTION_NOT_ALLOWED = "Move not allowed! This tile {0}";
        public MoveType MoveType { get; }
        public int Heat { get; }

        public Command(MoveType moveType, int heat = 0) {
            this.MoveType = moveType;
            Heat = Math.Max(0, heat);
        }

        public int CompareTo(Command other) { return MoveType.CompareTo(other.MoveType); }

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

        public string GetExecutionFailureReason(ITileController tileController) {
            if (!CanBeExecutedOnTile(tileController)) {
                switch (MoveType) {
                    case MoveType.Fire:
                        if (tileController.IsBurntOut()) {
                            return string.Format(ACTION_NOT_ALLOWED, "is already burnt out.");
                        } else if (!tileController.IsFlammable()) {
                            return string.Format(ACTION_NOT_ALLOWED, "is not flammable.");
                        } else if (tileController.IsOccupied) {
                            return string.Format(ACTION_NOT_ALLOWED, "is occupied.");
                        } else if (tileController.IsOnFire()) {
                            return string.Format(ACTION_NOT_ALLOWED, "is already on fire.");
                        }
                        break;
                    case MoveType.Water:
                        if (tileController.IsBurntOut()) {
                            return string.Format(ACTION_NOT_ALLOWED, "is already burnt out.");
                        } else if (!tileController.IsFlammable()) {
                            return string.Format(ACTION_NOT_ALLOWED, "cannot be on fire.");
                        }
                        break;
                }
            }
            return "";
        }
    }
}
