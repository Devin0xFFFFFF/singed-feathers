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

        public void Start() {
            _mapClient = new MapPersistenceClient ();

            StartCoroutine(_mapClient.GetMaps(delegate (MapClientResult result) {
                if (result.IsError || result.ResponseCode != 200) {
                    Debug.LogError("Failed to fetch maps from server: " + result.ErrorMessage ?? result.ResponseCode + " " + result.ResponseBody);
                    //return;
                }
               
                Debug.Log("Maps fetched from server: " + result.ResponseBody);
                Dictionary<string, MapInfo[]> dict = JsonConvert.DeserializeObject<Dictionary<string,MapInfo[]>>(result.ResponseBody);
                MapInfo[] maps; 
                dict.TryGetValue("Maps", out maps);
                foreach(MapInfo map in maps){
                    GameObject mapButton = Instantiate(MapSelectButton);
                    Button tempButton = mapButton.GetComponent<Button>();
                    tempButton.GetComponentInChildren<Text>().text = "Map Name: "+map.MapName+"\t Creator: "
                        +map.CreatorName+"\n Play Type: " + map.MapType;
                    tempButton.onClick.AddListener(delegate {SelectMap(map.MapName);});

                    mapButton.transform.SetParent( this.GetComponent<RectTransform>());
                }

            
            }));
        }
        public void SelectMap(string MapID) { PlayerPrefs.SetString ("MapID", MapID); }

    }

}
