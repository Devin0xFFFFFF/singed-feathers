using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Assets.Scripts.Service {
    public abstract class APersistenceClient {
        protected readonly AWSAPIRequestBuilder _requestBuilder;
        private readonly JsonSerializerSettings _jsonSettings;

        public delegate void ResultCallback(ClientResult result);

        protected APersistenceClient(AWSAPIClientConfig apiConfig = null) {
            _requestBuilder = new AWSAPIRequestBuilder(apiConfig ?? new AWSAPIClientConfig()); 
            _jsonSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
        }

        protected string SerializeObject(object obj) { return JsonConvert.SerializeObject(obj, _jsonSettings); }

        protected void ReturnResult(ResultCallback resultCallback, string requestPath, UnityWebRequest request) { resultCallback(ConvertRequestToClientResult(requestPath, request)); }

        private ClientResult ConvertRequestToClientResult(string requestType, UnityWebRequest request) {
            string responseBody = request.downloadHandler.text;

            return new ClientResult(requestType, request.responseCode, responseBody, request.isError, request.error);
        }
    }
}
