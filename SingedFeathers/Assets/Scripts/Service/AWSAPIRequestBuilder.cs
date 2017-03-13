using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Security.Cryptography;

namespace Assets.Scripts.Service {
    public class AWSAPIRequestBuilder {
        // Relevant Guides for making requests to an APIGateway
        // Amazon Request Docs: http://docs.aws.amazon.com/apigateway/api-reference/making-http-requests/
        // Amazon Request Signing Docs: http://docs.aws.amazon.com/apigateway/api-reference/signing-requests/
        // Good Example: http://stackoverflow.com/questions/32906755/aws-api-gateway-signature

        private const string API_ENDPOINT_PROTOCOL = "https://";

        private const string ENCRYPTION_ALG = "AWS4-HMAC-SHA256";
        private const string SERVICE_NAME = "execute-api";
        private const string SERVICE_FOR_SIGNING = "apigateway";

        private const string HTTP_METHOD_GET = "GET";
        private const string HTTP_METHOD_PUT = "PUT";

        private const string AUTHORIZATION_HEADER = "authorization";
        private const string CONTENT_TYPE_HEADER = "content-type";
        private const string HOST_HEADER = "host";
        private const string X_AMZ_DATE_HEADER = "x-amz-date";
        private const string X_AMZ_CONTENT_SHA256_HEADER = "x-amz-content-sha256";

        private const string CONTENT_TYPE_JSON = "application/json";
        private const string SIGNED_HEADERS = "content-type;host;x-amz-date";

        private AWSAPIClientConfig _apiConfig;

        public AWSAPIRequestBuilder(AWSAPIClientConfig apiConfig) { _apiConfig = apiConfig; }

        public UnityWebRequest BuildGetRequest(string path, SortedDictionary<string, string> queryParameters) {
            string canonicalUri = GetCanonicalUri(path);
            string canonicalQueryString = BuildCanonicalQueryString(queryParameters);

            string hashedRequestPayload = HashRequestPayload(string.Empty); //For GET there is no payload, use ""
            string requestDate = GetAmazonDate();
            string authorization = SignRequest(hashedRequestPayload, HTTP_METHOD_GET, canonicalUri, canonicalQueryString, requestDate);

            string requestUri = BuildRequestUri(canonicalUri, canonicalQueryString);
            UnityWebRequest request = UnityWebRequest.Get(requestUri);

            request.SetRequestHeader(CONTENT_TYPE_HEADER, CONTENT_TYPE_JSON);
            request.SetRequestHeader(X_AMZ_DATE_HEADER, requestDate);
            request.SetRequestHeader(AUTHORIZATION_HEADER, authorization);
            request.SetRequestHeader(X_AMZ_CONTENT_SHA256_HEADER, hashedRequestPayload);

            return request;
        }

        public UnityWebRequest BuildPutRequest(string path, string serializedData) {
            string canonicalUri = GetCanonicalUri(path);
            string canonicalQueryString = string.Empty;

            string hashedRequestPayload = HashRequestPayload(serializedData);
            string requestDate = GetAmazonDate();
            string authorization = SignRequest(hashedRequestPayload, HTTP_METHOD_PUT, canonicalUri, canonicalQueryString, requestDate);

            string requestUri = BuildRequestUri(canonicalUri, canonicalQueryString);
            UnityWebRequest request = UnityWebRequest.Put(requestUri, serializedData);

            request.SetRequestHeader(CONTENT_TYPE_HEADER, CONTENT_TYPE_JSON);
            request.SetRequestHeader(X_AMZ_DATE_HEADER, requestDate);
            request.SetRequestHeader(AUTHORIZATION_HEADER, authorization);
            request.SetRequestHeader(X_AMZ_CONTENT_SHA256_HEADER, hashedRequestPayload);

            return request;
        }

        private string GetCanonicalUri(string path) { return _apiConfig.ApiEndpointStage + "/" + path + "/"; }

        private string BuildRequestUri(string canonicalUri, string canonicalQueryString) { return API_ENDPOINT_PROTOCOL + _apiConfig.ApiEndpointHostname + canonicalUri + "?" + canonicalQueryString; }

        private string SignRequest(string hashedRequestPayload, string requestMethod, string canonicalUri, string canonicalQueryString, string requestDate) {
            string dateStamp = DateTime.UtcNow.ToString("yyyyMMdd");
            string credentialScope = string.Format("{0}/{1}/{2}/aws4_request", dateStamp, _apiConfig.ApiEndpointRegion, SERVICE_NAME);

            SortedDictionary<string, string> headers = new SortedDictionary<string, string> {
                { CONTENT_TYPE_HEADER, CONTENT_TYPE_JSON },
                { HOST_HEADER, _apiConfig.ApiEndpointHostname },
                { X_AMZ_DATE_HEADER, requestDate }
            };

            string canonicalHeaders = string.Empty;
            foreach (string header in headers.Keys) {
                canonicalHeaders += header.ToLowerInvariant() + ":" + headers[header].Trim() + "\n";
            }

            // Task 1: Create a Canonical Request For Signature Version 4
            string canonicalRequest = requestMethod + "\n" + canonicalUri + "\n" + canonicalQueryString + "\n" + canonicalHeaders + "\n" + SIGNED_HEADERS + "\n" + hashedRequestPayload;
            string hashedCanonicalRequest = HexEncode(Hash(ToBytes(canonicalRequest)));

            // Task 2: Create a String to Sign for Signature Version 4
            string stringToSign = ENCRYPTION_ALG + "\n" + requestDate + "\n" + credentialScope + "\n" + hashedCanonicalRequest;

            // Task 3: Calculate the AWS Signature Version 4
            byte[] signingKey = GetSignatureKey(_apiConfig.SecretKey, dateStamp, _apiConfig.ApiEndpointRegion, SERVICE_NAME);
            string signature = HexEncode(HmacSha256(stringToSign, signingKey));

            // Task 4: Prepare a signed request
            // Authorization: algorithm Credential=access key ID/credential scope, SignedHeadaers=SignedHeaders, Signature=signature
            string authorization = string.Format("{0} Credential={1}/{2}/{3}/{4}/aws4_request, SignedHeaders={5}, Signature={6}",
            ENCRYPTION_ALG, _apiConfig.AccessKey, dateStamp, _apiConfig.ApiEndpointRegion, SERVICE_NAME, SIGNED_HEADERS, signature);

            return authorization;
        }

        private static string HashRequestPayload(string requestPayload) { return HexEncode(Hash(ToBytes(requestPayload))); }

        private static string GetAmazonDate() { return DateTime.UtcNow.ToString("yyyyMMddTHHmmss") + "Z"; }

        private static string BuildCanonicalQueryString(SortedDictionary<string, string> queryParameters) {
            string canonicalQueryString = string.Empty;

            if (queryParameters != null) {
                int count = 0;
                foreach (KeyValuePair<string, string> param in queryParameters) {
                    canonicalQueryString += param.Key + "=" + param.Value;
                    if (count < queryParameters.Count - 1) {
                        canonicalQueryString += "&";
                    }
                    count++;
                }
            }

            return canonicalQueryString;
        }

        private static string CreateRequestPayload(string jsonString) { return HexEncode(Hash(ToBytes(jsonString))); }

        private static byte[] GetSignatureKey(string key, string dateStamp, string regionName, string serviceName) {
            byte[] kDate = HmacSha256(dateStamp, ToBytes("AWS4" + key));
            byte[] kRegion = HmacSha256(regionName, kDate);
            byte[] kService = HmacSha256(serviceName, kRegion);

            return HmacSha256("aws4_request", kService);
        }

        private static byte[] ToBytes(string str) { return Encoding.UTF8.GetBytes(str.ToCharArray()); }

        private static string HexEncode(byte[] bytes) { return BitConverter.ToString(bytes).Replace("-", string.Empty).ToLowerInvariant(); }

        private static byte[] Hash(byte[] bytes) { return SHA256.Create().ComputeHash(bytes); }

        private static byte[] HmacSha256(string data, byte[] key) { return new HMACSHA256(key).ComputeHash(ToBytes(data)); }
    }
}