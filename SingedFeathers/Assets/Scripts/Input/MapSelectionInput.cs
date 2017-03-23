using CoreGame.Models;
using UnityEngine;
using Assets.Scripts.Service;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UI;

namespace Assets.Scripts.Input {
    public class MapSelectionInput : MonoBehaviour {
        private MapPersistenceClient _mapClient;
        public GameObject MapSelectButton;
        private List<GameObject> _activeButtons;

        public void Start() {
            _mapClient = new MapPersistenceClient ();

            StartCoroutine(_mapClient.GetMaps(delegate (MapClientResult result) {
                if (result.IsError || result.ResponseCode != 200) {
                    Debug.LogError("Failed to fetch maps from server: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                    //return;
                }
               
                Debug.Log("Maps fetched from server: " + result.ResponseBody);
                Dictionary<string, MapInfo[]> dict = JsonConvert.DeserializeObject<Dictionary<string,MapInfo[]>>(result.ResponseBody);
                List<MapInfo> maps = new List<MapInfo>();//MapInfo[] maps; 
                //dict.TryGetValue("Maps", out maps);
                maps.Add(new MapInfo());
                //foreach(MapInfo map in maps){
                GameObject newButton = Instantiate(MapSelectButton);
                newButton.transform.parent = this.transform;
                    _activeButtons.Add(newButton);
                //}

            
            }));
        }
        public void SelectMap() {  }

    }

}
