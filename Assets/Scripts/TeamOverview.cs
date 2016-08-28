using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    class TeamOverview : MonoBehaviour, IDragHandler
    {

        public GameObject TeamPanelPrefab;
        List<Transform> teamPanels;

        public void OnDrag(PointerEventData eventData)
        {
            transform.position += (Vector3)eventData.delta;
        }

        public void Update()
        {
            List<Road.Team> teams = God.TheOne.road.teamsOnTheWay;
            for (int i = 0; i < teams.Count; i++)
            {
                if(teamPanels.Count <= i) {
                    GameObject newPanel = Instantiate(TeamPanelPrefab);
                    newPanel.transform.SetParent(transform);
                    teamPanels.Add(newPanel.transform);
                }
                teamPanels[i].GetChild(0).GetComponent<Text>().text = teams[i].people.ToString("# ##0");
                teamPanels[i].GetChild(1).GetComponent<Text>().text = teams[i].stones.ToString("# ##0");
                teamPanels[i].GetChild(2).GetComponent<Text>().text = teams[i].TimeLeft.ToString("# ##0");
            }

        }
    }
}
