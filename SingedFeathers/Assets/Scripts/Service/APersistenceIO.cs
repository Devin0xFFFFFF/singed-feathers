using Newtonsoft.Json;

namespace Assets.Scripts.Service {
    public abstract class APersistenceIO {
        protected readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };

        protected static bool IsValidResult(ClientResult result) { return !IsError(result); }

        private static bool IsError(ClientResult result) { return result.IsError || result.ResponseCode != 200; }
    }
}
