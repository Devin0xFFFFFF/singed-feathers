using System.Collections;
using CoreGame.Models.API.MapClient;

namespace Assets.Scripts.Service.Client {
    public interface IMapClient {
        IEnumerator CreateMap(CreateMapInfo mapInfo, APersistenceClient.ResultCallback resultCallback);

        IEnumerator GetMapData(string mapID, APersistenceClient.ResultCallback resultCallback);

        IEnumerator GetMaps(APersistenceClient.ResultCallback resultCallback);
    }
}