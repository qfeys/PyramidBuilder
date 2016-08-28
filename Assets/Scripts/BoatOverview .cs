using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    class BoatOverview : MonoBehaviour, IDragHandler
    {

        public GameObject BoatPanelPrefab;
        Dictionary<Transform, River.Boat> boatPanels;

        public void OnDrag(PointerEventData eventData)
        {
            transform.position += (Vector3)eventData.delta;
        }

        public void Update()
        {
            List<River.Boat> boats = God.TheOne.river.boats;
            foreach(var boat in boats)
            {
                if (boatPanels.ContainsValue(boat) == false)
                {
                    GameObject newPanel = Instantiate(BoatPanelPrefab);
                    newPanel.transform.SetParent(transform);
                    boatPanels.Add(newPanel.transform, boat);
                }
            }

            foreach(var panel in boatPanels)
            {
                panel.Key.GetChild(0).GetComponent<Text>().text = panel.Value.name;
                panel.Key.GetChild(1).GetComponent<Text>().text = panel.Value.crew.ToString("# ##0");
                panel.Key.GetChild(2).GetChild(0).GetComponent<Text>().text = panel.Value.minCrew.ToString("# ##0");
                panel.Key.GetChild(2).GetChild(1).GetComponent<Text>().text = panel.Value.maxCrew.ToString("# ##0");
                panel.Key.GetChild(4).GetChild(0).GetComponent<Text>().text = panel.Value.stones.ToString("# ##0");
                panel.Key.GetChild(4).GetChild(1).GetComponent<Text>().text = panel.Value.capacity.ToString("# ##0");
                panel.Key.GetChild(5).GetComponent<Text>().text = panel.Value.timeTillArrival.ToString("# ##0");
                panel.Key.GetChild(6).GetChild(0).GetComponent<Text>().text = panel.Value.minTravelTime.ToString("# ##0");
                panel.Key.GetChild(6).GetChild(1).GetComponent<Text>().text = panel.Value.maxTravelTime.ToString("# ##0");
                panel.Key.GetChild(7).GetComponent<Toggle>().isOn = panel.Value.isInDock;
            }
        }
    }
}
