namespace Assets.Scripts.Service.Client {
    public class ClientResult {
        public readonly string RequestType;
        public readonly long ResponseCode;
        public readonly string ResponseBody;
        public readonly bool IsError;
        public readonly string ErrorMessage;

        public ClientResult(string requestType, long responseCode, string responseBody, bool isError, string errorMessage) {
            RequestType = requestType;
            ResponseCode = responseCode;
            ResponseBody = responseBody;
            IsError = isError;
            ErrorMessage = errorMessage;
        }
    }
}