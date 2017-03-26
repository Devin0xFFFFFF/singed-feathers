using CoreGame.Models.API.MapClient;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Service {
    class CommitIO : APersistenceIO {
        public delegate void CommitTurnCallback();

        public CommitIO() { }

        public IEnumerator CreateMap(CreateMapInfo mapInfo, CreateMapCallback callback) {
            yield return _client.CreateMap(mapInfo, delegate (ClientResult result) {
                if (IsValidResult(result)) {
                    string mapID = result.ResponseBody;
                    Console.WriteLine("Map created: " + mapID);
                    callback(mapID);
                } else {
                    Console.WriteLine("Failed to create map: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                    callback(null);
                }
            });
        }

        public IEnumerator GetMapData(string mapID, GetMapDataCallback callback) {
            yield return _client.GetMapData(mapID, delegate (ClientResult result) {
                if (IsValidResult(result)) {
                    string serializedMapData = result.ResponseBody;
                    Console.WriteLine("Map data fetched from server: " + serializedMapData);
                    callback(serializedMapData);
                } else {
                    Console.WriteLine("Failed to fetch map data from server: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                    callback(null);
                }
            });
        }

        public IEnumerator GetMaps(GetMapsCallback callback) {
            yield return _client.GetMaps(delegate (ClientResult result) {
                if (IsValidResult(result)) {
                    GetMapsResult deserializedResult = JsonConvert.DeserializeObject<GetMapsResult>(result.ResponseBody, _jsonSettings);
                    Console.WriteLine("Maps fetched from server: " + deserializedResult.Maps);
                    callback(deserializedResult.Maps);
                } else {
                    Console.WriteLine("Failed to fetch maps from server: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                    callback(null);
                }
            });
        }

        [Serializable]
        private class GetMapsResult {
            public List<MapInfo> Maps;

            public GetMapsResult(List<MapInfo> maps) { Maps = maps; }
        }
    }
}
