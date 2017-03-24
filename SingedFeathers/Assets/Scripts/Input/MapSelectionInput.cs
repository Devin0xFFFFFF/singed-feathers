using CoreGame.Models.API.MapClient;
using UnityEngine;
using Assets.Scripts.Service;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.UI;

namespace Assets.Scripts.Input {
    public class MapSelectionInput : MonoBehaviour {
        private MapIO _mapIO;
        public GameObject MapSelectButton;

        public void Start() {
            _mapIO = new MapIO ();

            StartCoroutine(_mapIO.GetMaps(delegate (List<MapInfo> maps) {
                if (maps == null) {
                    Debug.LogError("Failed to retrieve maps.");
                    return;
                }
               
                Debug.Log("Maps fetched from server");

                foreach(MapInfo map in maps){
                    GameObject mapButton = Instantiate(MapSelectButton);
                    Button tempButton = mapButton.GetComponent<Button>();
                    tempButton.GetComponentInChildren<Text>().text = "Map Name: "+map.MapName+"\t Creator: "
                        +map.CreatorName+"\n Play Type: " + map.MapType;
                    tempButton.onClick.AddListener(delegate {SelectMap(map.MapID);});

                    mapButton.transform.SetParent( this.GetComponent<RectTransform>());
                }

            
            }));
        }
        public void SelectMap(string MapID) { PlayerPrefs.SetString ("MapID", MapID); }

    }

}
