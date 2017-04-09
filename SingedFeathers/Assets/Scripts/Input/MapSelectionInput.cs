using UnityEngine;
using Assets.Scripts.Service;
using CoreGame.Models.API.MapClient;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Assets.Scripts.Input {
    public class MapSelectionInput : MonoBehaviour {
        public GameObject MapSelectButton;
        private MapIO _mapIO;
        private List<GameObject> _buttons;

        public void Start() {
            _mapIO = new MapIO();
            _buttons = new List<GameObject>();
            StartCoroutine(_mapIO.GetMaps(delegate(List<MapInfo> maps) {
                if (maps == null) {
                    Debug.LogError("Failed to retrieve maps.");
                    return;
                }
               
                Debug.Log("Maps fetched from server");

                foreach (MapInfo map in maps) {
                    GameObject mapButton = Instantiate(MapSelectButton);
                    Button tempButton = mapButton.GetComponent<Button>();
                    tempButton.GetComponentInChildren<Text>().text = "Map Name: "+ map.MapName +"\t Creator: "
                        + map.CreatorName + "\n Play Type: " + map.MapType;
                    tempButton.onClick.AddListener(delegate { SelectMap(map.MapID); });

                    mapButton.transform.SetParent(this.GetComponent<RectTransform>());
                    _buttons.Add(mapButton);
                }
            }));
        }

        public void Refresh() {
            foreach (GameObject button in _buttons) {
                Destroy(button.gameObject);
            }
            Start();
        }
        public void SelectMap(string MapID) { PlayerPrefs.SetString("MapID", MapID); }
    }
}
