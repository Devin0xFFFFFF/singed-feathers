﻿using Assets.Scripts.Controllers;
using System;

namespace Assets.Scripts.Models.Commands {
    [Serializable]
    public class Command : IComparable<Command> {
        public MoveType MoveType { get; private set; }
        public int Heat { get; private set; }

        public Command(MoveType moveType, int heat = 0) {
            MoveType = moveType;
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
                    return tileController.IsFlammable() && !tileController.IsHeatZero();
            }
            return false;
        }
    }
}
