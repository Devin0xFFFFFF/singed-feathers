using Assets.Scripts.Service.Client;
using CoreGame.Models.API.MapClient;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Service.IO {
    public class MapIO : APersistenceIO {
        private IMapClient _client;

        public delegate void CreateMapCallback(string mapID);
        public delegate void GetMapDataCallback(string serializedMapData);
        public delegate void GetMapsCallback(List<MapInfo> maps);

        public MapIO(IMapClient client = null) { _client = client ?? new MapClient(); }

        public IEnumerator CreateMap(CreateMapInfo mapInfo, CreateMapCallback callback) {
            yield return _client.CreateMap(mapInfo, delegate (ClientResult result) {
                callback(ParseCreateMapResult(result));
            });
        }

        public IEnumerator GetMapData(string mapID, GetMapDataCallback callback) {
            yield return _client.GetMapData(mapID, delegate (ClientResult result) {
                callback(ParseGetMapDataResult(result));
            });
        }

        public IEnumerator GetMaps(GetMapsCallback callback) {
            yield return _client.GetMaps(delegate (ClientResult result) {
                callback(ParseGetMapsResult(result));
            });
        }

        public string ParseCreateMapResult(ClientResult result) {
            if (IsValidResult(result)) {
                string mapID = result.ResponseBody;
                Console.WriteLine("Map created: " + mapID);
                return mapID;
            } else {
                Console.WriteLine("Failed to create map: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                return null;
            }
        }

        public string ParseGetMapDataResult(ClientResult result) {
            if (IsValidResult(result)) {
                string serializedMapData = result.ResponseBody;
                Console.WriteLine("Map data fetched from server: " + serializedMapData);
                return serializedMapData;
            } else {
                Console.WriteLine("Failed to fetch map data from server: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                return null;
            }
        }

        public List<MapInfo> ParseGetMapsResult(ClientResult result) {
            if (IsValidResult(result)) {
                GetMapsResult deserializedResult = JsonConvert.DeserializeObject<GetMapsResult>(result.ResponseBody, _jsonSettings);
                Console.WriteLine("Maps fetched from server: " + deserializedResult.Maps);
                return deserializedResult.Maps;
            } else {
                Console.WriteLine("Failed to fetch maps from server: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                return null;
            }
        }

        [Serializable]
        private class GetMapsResult {
            public List<MapInfo> Maps;

            public GetMapsResult(List<MapInfo> maps) { Maps = maps; }
        }
    }
}
