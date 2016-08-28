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
            case People.Community.transportRoad: newInfo = Instantiate(RoadInfoPanel); break;
            case People.Community.transportRiver: newInfo = Instantiate(RiverInfoPanel); break;
            }
            newInfo.transform.SetParent(panel);
        }
        public void RemovePanelInfo()
        {
            transform.Find("PanelCommunityInfo").gameObject.SetActive(false);
        }
    }
}
