using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using CoreGame.Models.API.MapClient;

namespace Assets.Scripts.Service.Client {
    public class MapClient : APersistenceClient, IMapClient {
        private const string CREATE_MAP_PATH = "CreateMap";
        private const string GET_MAP_DATA_PATH = "GetMapData";
        private const string GET_MAPS_PATH = "GetMaps";

        private const string MAP_ID_FIELD = "MapID";

        public MapClient(AWSAPIClientConfig apiConfig = null) : base(apiConfig) { }

        public IEnumerator CreateMap(CreateMapInfo mapInfo, ResultCallback resultCallback) {
            UnityWebRequest request = _requestBuilder.BuildPutRequest(CREATE_MAP_PATH, SerializeObject(mapInfo));
            yield return request.Send();
            ReturnResult(resultCallback, CREATE_MAP_PATH, request);
        }

        public IEnumerator GetMapData(string mapID, ResultCallback resultCallback) {
            SortedDictionary<string, string> queryParameters = BuildGetMapDataQueryParameters(mapID);

            UnityWebRequest request = _requestBuilder.BuildGetRequest(GET_MAP_DATA_PATH, queryParameters);
            yield return request.Send();
            ReturnResult(resultCallback, GET_MAP_DATA_PATH, request);
        }

        public IEnumerator GetMaps(ResultCallback resultCallback) {
            SortedDictionary<string, string> queryParameters = BuildGetMapsQueryParameters();

            UnityWebRequest request = _requestBuilder.BuildGetRequest(GET_MAPS_PATH, queryParameters);
            yield return request.Send();
            ReturnResult(resultCallback, GET_MAPS_PATH, request);
        }

        private SortedDictionary<string, string> BuildGetMapDataQueryParameters(string mapID) {
            return new SortedDictionary<string, string> {
                { MAP_ID_FIELD, mapID }
            };
        }

        private SortedDictionary<string, string> BuildGetMapsQueryParameters() { return new SortedDictionary<string, string> { }; }
    }
}