﻿using System;
using CoreGame.Models.Commands;
using Newtonsoft.Json;

namespace CoreGame.Models {
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