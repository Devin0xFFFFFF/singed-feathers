﻿namespace Assets.Scripts.Models.Commands {
    public class Command {
        public MoveType MoveType;
        public int? Heat;

        public Command(MoveType moveType, int? heat = null) {
            MoveType = moveType;
            Heat = heat;
        }

        public ICommand MakeICommand() {
            if (Heat != null) {
                switch (MoveType) {
                    case MoveType.Fire:
                        return new SetFireCommand(Heat.Value);
                    case MoveType.Water:
                        return new AddWaterCommand(Heat.Value);
                }
            }
            return new RemoveCommand();
        }
    }
}