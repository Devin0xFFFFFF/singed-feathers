namespace Assets.Scripts.MapIO {
    public class MapClientResult {
        public readonly string _requestType;
        public readonly long _responseCode;
        public readonly string _responseBody;
        public readonly bool _isError;
        public readonly string _errorMessage;

        public MapClientResult(string requestType, long responseCode, string responseBody, bool isError, string errorMessage) {
            _requestType = requestType;
            _responseCode = responseCode;
            _responseBody = responseBody;
            _isError = isError;
            _errorMessage = errorMessage;
        }
    }
}