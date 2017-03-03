using Assets.Scripts.Models;
using UnityEngine;

namespace Assets.Scripts.MapIO {
    public class MapIO : MonoBehaviour {
        private MapPersistenceClient _client;

        public void Awake() {
            if (_client == null) {
                _client = new MapPersistenceClient(new AWSAPIGatewayConfig());
            }
        }

        public void CreateMap(MapInfo mapInfo, MapPersistenceClient.ResultCallback resultCallback) { StartCoroutine(_client.CreateMap(mapInfo, resultCallback)); }

        public void GetMapData(string mapID, MapPersistenceClient.ResultCallback resultCallback) { StartCoroutine(_client.GetMapData(mapID, resultCallback)); }

        public void GetMaps(MapPersistenceClient.ResultCallback resultCallback) { StartCoroutine(_client.GetMaps(resultCallback)); }
    }
}