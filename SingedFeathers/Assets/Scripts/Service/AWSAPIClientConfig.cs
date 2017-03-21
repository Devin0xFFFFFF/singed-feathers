namespace Assets.Scripts.Service {
    public class AWSAPIClientConfig {
        public readonly string ApiEndpointHostname;
        public readonly string ApiEndpointStage;
        public readonly string ApiEndpointRegion;

        public readonly string AccessKey;
        public readonly string SecretKey;

        public AWSAPIClientConfig() {
            ApiEndpointHostname = "qpmatnypjg.execute-api.us-west-2.amazonaws.com";
            ApiEndpointStage = "/prod";
            ApiEndpointRegion = "us-west-2";

            AccessKey = "AKIAJ5PCN3FEJRCIDQJA";
            SecretKey = "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";
        }
    }
}