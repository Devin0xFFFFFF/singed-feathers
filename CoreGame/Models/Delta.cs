using System;
using Assets.Scripts.Models.Commands;

namespace Assets.Scripts.Models {
    [Serializable]
    public class Delta {
        public Position position;
        public Command command;

        public Delta(Position position, Command command) {
            this.position = position;
            this.command = command;
        }
    }
}
