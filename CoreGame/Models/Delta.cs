using System;
using CoreGame.Models.Commands;
using Newtonsoft.Json;

namespace CoreGame.Models {
    [Serializable]
    public class Delta {
        public Position Position;
        public ICommand Command;

        public Delta(Position position, ICommand command) {
            Position = position;
            Command = command;
        }

        [JsonConstructor]
        public Delta(Position position, Command command) {
            Position = position;
            Command = command;
        }
    }
}