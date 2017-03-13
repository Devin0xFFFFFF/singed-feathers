using Assets.Scripts.Models;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Service {
    public class MapPersistenceClient {
        private const string CREATE_MAP_PATH = "createMap";
        private const string GET_MAP_DATA_PATH = "getMapData";
        private const string GET_MAPS_PATH = "getMaps";

        private const string MAP_ID_FIELD = "MapID";

        private AWSAPIRequestBuilder _requestBuilder;

        public delegate void ResultCallback(MapClientResult result);

        public MapPersistenceClient(AWSAPIConfig apiConfig = null) { _requestBuilder = new AWSAPIRequestBuilder(apiConfig ?? new AWSAPIConfig()); }

        public IEnumerator CreateMap(MapInfo mapInfo, ResultCallback resultCallback) {
            string serializedMapInfo = SerializeMapInfo(mapInfo);

            UnityWebRequest request = _requestBuilder.BuildPutRequest(CREATE_MAP_PATH, serializedMapInfo);

            yield return request.Send();

            resultCallback(ConvertRequestToMapClientResult(CREATE_MAP_PATH, request));
        }

        public IEnumerator GetMapData(string mapID, ResultCallback resultCallback) {
            SortedDictionary<string, string> queryParameters = BuildGetMapDataQueryParameters(mapID);

            UnityWebRequest request = _requestBuilder.BuildGetRequest(GET_MAP_DATA_PATH, queryParameters);

            yield return request.Send();

            resultCallback(ConvertRequestToMapClientResult(GET_MAP_DATA_PATH, request));
        }

        public IEnumerator GetMaps(ResultCallback resultCallback) {
            SortedDictionary<string, string> queryParameters = BuildGetMapsQueryParameters();

            UnityWebRequest request = _requestBuilder.BuildGetRequest(GET_MAPS_PATH, queryParameters);

            yield return request.Send();

            resultCallback(ConvertRequestToMapClientResult(GET_MAPS_PATH, request));
        }

        private string SerializeMapInfo(MapInfo mapInfo) {
            return JsonConvert.SerializeObject(mapInfo);
        }

        private SortedDictionary<string, string> BuildGetMapDataQueryParameters(string mapID) {
            return new SortedDictionary<string, string> {
                { MAP_ID_FIELD, mapID }
            };
        }

        private SortedDictionary<string, string> BuildGetMapsQueryParameters() { return new SortedDictionary<string, string> { }; }

        private static MapClientResult ConvertRequestToMapClientResult(string requestType, UnityWebRequest request) {
            string responseBody = request.downloadHandler.text;

            return new MapClientResult(requestType, request.responseCode, responseBody, request.isError, request.error);
        }
    }
}