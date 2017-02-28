using UnityEngine;

namespace Assets.Scripts.MapIO {
    public class MapIO : MonoBehaviour {
        private MapPersistenceClient _client;

        void Awake() {
            if (_client == null) {
                _client = new MapPersistenceClient(new AWSAPIGatewayConfig());
            }
        }

        public void CreateMap(string mapName, string creatorName, string mapType, string serializedMapData, MapPersistenceClient.ResultCallback resultCallback) {
            StartCoroutine(_client.CreateMap(mapName, creatorName, mapType, serializedMapData, resultCallback));
        }

        public void GetMapData(string mapID, MapPersistenceClient.ResultCallback resultCallback) {
            StartCoroutine(_client.GetMapData(mapID, resultCallback));
        }

        public void GetMaps(MapPersistenceClient.ResultCallback resultCallback) {
            StartCoroutine(_client.GetMaps(resultCallback));
        }
    }
}