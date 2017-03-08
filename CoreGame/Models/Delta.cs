using System;
using Assets.Scripts.Models.Commands;

namespace Assets.Scripts.Models {
    [Serializable]
    public class Delta {
        public Position _position;
        public Command _command;

        public Delta(Position position, Command command) {
            _position = position;
            _command = command;
        }
    }
}
