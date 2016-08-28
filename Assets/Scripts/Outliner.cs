using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    class Outliner : MonoBehaviour
    {
        static public Outliner TheOne;

        public GameObject FarmInfoPanel;
        public GameObject QuarryInfoPanel;
        public GameObject ConstructionInfoPanel;
        public GameObject RoadInfoPanel;
        public GameObject RiverInfoPanel;

        public GameObject TeamOverview;
        public GameObject BoatOverview;

        public void Awake() { if (TheOne == null) TheOne = this; }
        public void Start()
        {
            RemovePanelInfo();
        }

        public void Update()
        {
            transform.Find("PanelTime").GetChild(1).GetComponent<Text>().text = God.TheOne.time.ToString("MM-dd");
            transform.Find("PanelPopulation").GetChild(1).GetComponent<Text>().text = People.totalPopulation.ToString("# ##0");
            transform.Find("PanelFood").GetChild(1).GetComponent<Text>().text = God.TheOne.farm.stock.ToString("# ##0");
            transform.Find("PanelFood").GetChild(3).GetComponent<Text>().text = God.TheOne.farm.storageCapacity.ToString("# ##0");
            try {
                UpdateInfoPanel(); }catch(NullReferenceException e) { Debug.Log("Null caught " + e.StackTrace); }
        }

        public void TimeUp()
        {
            God.TheOne.timeSetting++;
            if (God.TheOne.timeSetting > 5) God.TheOne.timeSetting = 5;
            transform.Find("PanelSpeed").GetChild(0).GetComponent<Text>().text = "Speed " + God.TheOne.timeSetting;
        }
        public void TimeDown()
        {
            God.TheOne.timeSetting--;
            if (God.TheOne.timeSetting < 1) God.TheOne.timeSetting = 1;
            transform.Find("PanelSpeed").GetChild(0).GetComponent<Text>().text = "Speed " + God.TheOne.timeSetting;
        }
        public void TimePause()
        {
            God.TheOne.isPaused = !God.TheOne.isPaused;
            transform.Find("PanelSpeed").GetChild(2).GetChild(0).GetComponent<Text>().fontStyle = God.TheOne.isPaused ? FontStyle.Bold : FontStyle.Normal;
        }
        public void ToggleRealmOverview()
        {
            RealmOverview.TheOne.gameObject.SetActive(!RealmOverview.TheOne.gameObject.activeSelf);
        }

        private void UpdateInfoPanel()
        {
            Transform panel = transform.Find("PanelCommunityInfo");
            if (panel.gameObject.activeSelf == false) return;
            panel.GetChild(1).GetChild(1).GetChild(1).GetComponent<Text>().text = People.PeopleAt(currentPanelInfo).ToString("# ##0");
            panel.GetChild(1).GetChild(2).GetChild(1).GetComponent<Text>().text = Mathf.Clamp01(People.unrest[currentPanelInfo]).ToString("##0%");
            panel.GetChild(1).GetChild(3).GetChild(1).GetComponent<Text>().text = (People.PeopleAt(currentPanelInfo) * People.foodAllowance[currentPanelInfo]).ToString("# ##0");
            switch (currentPanelInfo)
            {
            case People.Community.farm:
                Farm f = God.TheOne.farm;
                panel.GetChild(1).Find("Production").GetChild(1).GetComponent<Text>().text = f.production.ToString("# ##0");
                panel.GetChild(1).Find("Stock").GetChild(1).GetComponent<Text>().text = f.stock.ToString("# ##0");
                panel.GetChild(1).Find("Capacity").GetChild(1).GetComponent<Text>().text = f.storageCapacity.ToString("# ##0");
                panel.GetChild(1).Find("TechProgress").GetChild(1).GetComponent<Text>().text = f.techProgress.ToString("# ##0");
                break;
            case People.Community.quarry:
                Quarry q = God.TheOne.quarry;
                panel.GetChild(1).Find("Production").GetChild(1).GetComponent<Text>().text = q.production.ToString("# ##0");
                panel.GetChild(1).Find("Stock").GetChild(1).GetComponent<Text>().text = q.stock.ToString("# ##0");
                panel.GetChild(1).Find("TechProgress").GetChild(1).GetComponent<Text>().text = q.techProgress.ToString("# ##0");
                break;
            case People.Community.construction:
                Construction c = God.TheOne.construction;
                panel.GetChild(1).Find("Production").GetChild(1).GetComponent<Text>().text = c.workSpeed.ToString("# ##0");
                panel.GetChild(1).Find("Stock").GetChild(1).GetComponent<Text>().text = c.stock.ToString("# ##0");
                panel.GetChild(1).Find("TechProgress").GetChild(1).GetComponent<Text>().text = c.techProgress.ToString("# ##0");
                string taskNames = ""; string taskWork = "";
                foreach (var task in c.tasks) { taskNames += task.name + "\n"; taskWork += task.work.ToString("# ##0") + "\n"; }
                taskNames += "-----"; taskWork += "---";
                panel.GetChild(1).Find("Tasks1").GetChild(0).GetComponent<Text>().text = taskNames;
                panel.GetChild(1).Find("Tasks1").GetChild(1).GetComponent<Text>().text = taskWork;
                break;
            case People.Community.road:
                Road ro = God.TheOne.road;
                panel.GetChild(1).Find("Transit").GetChild(1).GetComponent<Text>().text = ro.stonesInTransit.ToString("# ##0");
                panel.GetChild(1).Find("FreePeople").GetChild(1).GetComponent<Text>().text = (People.PeopleAt(People.Community.river) - ro.peopleBusy).ToString("# ##0");
                panel.GetChild(1).Find("TechProgress").GetChild(1).GetComponent<Text>().text = ro.techProgress.ToString("# ##0");
                break;
            case People.Community.river:
                River ri = God.TheOne.river;
                panel.GetChild(1).Find("Transit").GetChild(1).GetComponent<Text>().text = ri.stonesInTransit.ToString("# ##0");
                panel.GetChild(1).Find("NumberOfBoats").GetChild(1).GetComponent<Text>().text = ri.boats.Count.ToString("# ##0");
                panel.GetChild(1).Find("FreePeople").GetChild(1).GetComponent<Text>().text = (People.PeopleAt(People.Community.river) - ri.peopleBusy).ToString("# ##0");
                panel.GetChild(1).Find("TechProgress").GetChild(1).GetComponent<Text>().text = ri.techProgress.ToString("# ##0");

                break;
            }
        }

        People.Community currentPanelInfo;
        public void setPanelInfo(People.Community com)
        {
            Transform panel = transform.Find("PanelCommunityInfo");
            panel.gameObject.SetActive(true);
            if (currentPanelInfo == com) return;
            currentPanelInfo = com;
            if (panel.childCount == 2) Destroy(panel.GetChild(1).gameObject);
            GameObject newInfo = null;
            switch (com)
            {
            case People.Community.farm: newInfo = Instantiate(FarmInfoPanel); break;
            case People.Community.quarry: newInfo = Instantiate(QuarryInfoPanel); break;
            case People.Community.construction: newInfo = Instantiate(ConstructionInfoPanel); break;
            case People.Community.road:
                newInfo = Instantiate(RoadInfoPanel);
                newInfo.transform.Find("Teams").GetChild(0).GetComponent<Button>().onClick.AddListener(() => ToggleRoadPanel());
                break;
            case People.Community.river:
                newInfo = Instantiate(RiverInfoPanel);
                newInfo.transform.Find("Boats").GetChild(0).GetComponent<Button>().onClick.AddListener(() => ToggleRiverPanel());
                break;
            }
            newInfo.transform.SetParent(panel);
        }
        public void RemovePanelInfo()
        {
            transform.Find("PanelCommunityInfo").gameObject.SetActive(false);
        }

        public void ToggleRoadPanel()
        {
            TeamOverview.SetActive(!TeamOverview.activeSelf);
        }

        public void ToggleRiverPanel()
        {
            BoatOverview.SetActive(!BoatOverview.activeSelf);
        }

        public void ToggleSetCrewForBoatPanel(River.Boat boat)
        {
            throw new NotImplementedException();
        }
    }
}
