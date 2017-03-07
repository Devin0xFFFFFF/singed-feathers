namespace Assets.Scripts.MapIO {
    public class MapClientResult {
        public readonly string RequestType;
        public readonly long ResponseCode;
        public readonly string ResponseBody;
        public readonly bool IsError;
        public readonly string ErrorMessage;

        public MapClientResult(string requestType, long responseCode, string responseBody, bool isError, string errorMessage) {
            RequestType = requestType;
            ResponseCode = responseCode;
            ResponseBody = responseBody;
            IsError = isError;
            ErrorMessage = errorMessage;
        }
    }
}