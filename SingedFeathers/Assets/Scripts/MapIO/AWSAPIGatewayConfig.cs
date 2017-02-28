namespace Assets.Scripts.MapIO {
    public class AWSAPIGatewayConfig {
        public readonly string _apiEndpointHostname;
        public readonly string _apiEndpointStage;
        public readonly string _apiEndpointRegion;

        public readonly string _accessKey;
        public readonly string _secretKey;

        public AWSAPIGatewayConfig() {
            // TODO: Make these configured from a file or something equivalent to protect our secrets

            _apiEndpointHostname = "pb3g13anjh.execute-api.us-west-2.amazonaws.com";
            _apiEndpointStage = "/prod";
            _apiEndpointRegion = "us-west-2";

            _accessKey = "AKIAJZDMBMR5AD7HA3QQ";
            _secretKey = "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";
        }
    }
}