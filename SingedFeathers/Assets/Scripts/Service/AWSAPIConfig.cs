namespace Assets.Scripts.Service {
    public class AWSAPIConfig {
        public readonly string ApiEndpointHostname;
        public readonly string ApiEndpointStage;
        public readonly string ApiEndpointRegion;

        public readonly string AccessKey;
        public readonly string SecretKey;

        public AWSAPIConfig() {
            ApiEndpointHostname = "pb3g13anjh.execute-api.us-west-2.amazonaws.com";
            ApiEndpointStage = "/prod";
            ApiEndpointRegion = "us-west-2";

            AccessKey = "AKIAJZDMBMR5AD7HA3QQ";
            SecretKey = "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";
        }
    }
}