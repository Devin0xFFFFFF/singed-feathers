using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Assets.Scripts.MapIO {
    public class MapPersistenceClient {
        private const string CREATE_MAP_PATH = "createMap";
        private const string GET_MAP_DATA_PATH = "getMapData";
        private const string GET_MAPS_PATH = "getMaps";

        public delegate void ResultCallback(MapClientResult result);

        private AWSAPIGatewayClient _client;

        public MapPersistenceClient(AWSAPIGatewayConfig apiConfig) {
            _client = new AWSAPIGatewayClient(apiConfig);
        }

        public IEnumerator CreateMap(string mapName, string creatorName, string mapType, string serializedMapData, ResultCallback resultCallback) {
            string serializedMapInfo = "{ \"MapInfo\": { " +
                "\"MapName\": \"" + mapName +
                "\", \"CreatorName\": \"" + creatorName +
                "\", \"MapType\": \"" + mapType +
                "\", \"MapData\": \"" + serializedMapData +
                "\" } }";

            yield return _client.Put(CREATE_MAP_PATH, serializedMapInfo, delegate (UnityWebRequest webRequest) {
                resultCallback(ConvertRequestToMapClientResult(CREATE_MAP_PATH, webRequest));
            });
        }

        public IEnumerator GetMapData(string mapID, ResultCallback resultCallback) {
            SortedDictionary<string, string> queryParameters = new SortedDictionary<string, string> {
            { "MapID", mapID }
        };

            yield return _client.Get(GET_MAP_DATA_PATH, queryParameters, delegate (UnityWebRequest webRequest) {
                resultCallback(ConvertRequestToMapClientResult(GET_MAP_DATA_PATH, webRequest));
            });
        }

        public IEnumerator GetMaps(ResultCallback resultCallback) {
            SortedDictionary<string, string> queryParameters = new SortedDictionary<string, string>
            {
            };

            yield return _client.Get(GET_MAPS_PATH, queryParameters, delegate (UnityWebRequest webRequest) {
                resultCallback(ConvertRequestToMapClientResult(GET_MAPS_PATH, webRequest));
            });
        }

        private static MapClientResult ConvertRequestToMapClientResult(string requestType, UnityWebRequest request) {
            string responseBody = request.downloadHandler.text;

            return new MapClientResult(requestType, request.responseCode, responseBody, request.isError, request.error);
        }
    }
}