using System;

namespace Assets.Scripts.Models.Commands {
    public class Command : IComparable<Command> {
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

        public int CompareTo(Command other) { return MoveType.CompareTo(other.MoveType); }
    }
}
