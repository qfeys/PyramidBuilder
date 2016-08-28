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
        public Color colorNorm;
        public Color colorAlert;
        Dictionary<Transform, River.Boat> boatPanels = new Dictionary<Transform, River.Boat>();

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
                    newPanel.transform.GetChild(0).GetComponent<Text>().text = boat.name;
                    newPanel.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = boat.minCrew.ToString("# ##0");
                    newPanel.transform.GetChild(2).GetChild(1).GetComponent<Text>().text = boat.maxCrew.ToString("# ##0");
                    newPanel.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => Outliner.TheOne.ToggleSetCrewForBoatPanel(boat));
                    newPanel.transform.GetChild(4).GetChild(1).GetComponent<Text>().text = boat.capacity.ToString("# ##0");
                    newPanel.transform.GetChild(6).GetChild(0).GetComponent<Text>().text = boat.minTravelTime.ToString("# ##0");
                    newPanel.transform.GetChild(6).GetChild(1).GetComponent<Text>().text = boat.maxTravelTime.ToString("# ##0");
                    newPanel.transform.GetChild(8).GetComponent<Toggle>().onValueChanged.AddListener(b => boat.mayLeave = b);
                }
            }

            foreach(var panel in boatPanels)
            {
                panel.Key.GetChild(1).GetComponent<Text>().text = panel.Value.crew.ToString("# ##0");
                panel.Key.GetChild(1).GetComponent<Text>().color = panel.Value.IsActive ? colorNorm : colorAlert;
                panel.Key.GetChild(3).GetComponent<Button>().interactable = panel.Value.isInDock; 
                panel.Key.GetChild(4).GetChild(0).GetComponent<Text>().text = panel.Value.stones.ToString("# ##0");
                panel.Key.GetChild(5).GetComponent<Text>().text = panel.Value.timeTillArrival.ToString("# ##0");
                panel.Key.GetChild(7).GetComponent<Toggle>().isOn = panel.Value.isInDock;
            }
        }
    }
}
