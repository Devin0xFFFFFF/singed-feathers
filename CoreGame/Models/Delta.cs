using System;
using Assets.Scripts.Models.Commands;

namespace Assets.Scripts.Models {
    [Serializable]
    public class Delta : IComparable<Delta> {
        public Position Position;
        public Command Command;

        public Delta(Position position, Command command) {
            Position = position;
            Command = command;
        }

        public int CompareTo(Delta other) { return Command.CompareTo(other.Command); }
    }
}
