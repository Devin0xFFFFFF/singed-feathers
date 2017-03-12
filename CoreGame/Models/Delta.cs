using System;
using Assets.Scripts.Models.Commands;

namespace Assets.Scripts.Models {
    [Serializable]
    public class Delta {
        public Position Position;
        public Command Command;

        public Delta(Position position, Command command) {
            Position = position;
            Command = command;
        }
    }
}
