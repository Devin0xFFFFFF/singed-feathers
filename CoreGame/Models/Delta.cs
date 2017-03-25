using System;
using CoreGame.Models.Commands;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace CoreGame.Models {
    [Serializable]
    public class Delta {
        public Position Position;
        public ICommand Command;
        private static JsonSerializerSettings _settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };

        public Delta(Position position, ICommand command) {
            Position = position;
            Command = command;
        }

        [JsonConstructor]
        public Delta(Position position, Command command) {
            Position = position;
            Command = command;
        }

        public static IList<Delta> Deserialize(string serilaizedDeltas) {
            return JsonConvert.DeserializeObject<List<Delta>>(serilaizedDeltas, _settings);
        }
    }
}