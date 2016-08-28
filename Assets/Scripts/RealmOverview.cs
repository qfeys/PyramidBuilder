using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class RealmOverview : MonoBehaviour ,IDragHandler
    {
        static public RealmOverview TheOne;

        public GameObject commuityPanel;

        public void Awake() { if (TheOne == null) TheOne = this; }

        Color colorNorm = new Color(0.855f, 0.797f, 0.301f, 0.781f);         // (219, 204, 77, 200)
        Color colorAlert = new Color(0.852f, 0.125f, 0.152f, 0.781f);         // (218, 32, 32, 200)



        // Use this for initialization
        void Start()
        {
            gameObject.SetActive(false);
        }

        public void Init()
        {
            foreach (var com in People.communityList)
            {
                Transform panel = Instantiate(commuityPanel).transform;
                panel.SetParent(transform);
                panel.name = com.ToString();
                panel.GetChild(0).GetChild(0).GetComponent<Text>().text = com.ToString();
                var c = com;
                panel.GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().onValueChanged.AddListener((v) => TrySetPopulation(c, v));
                panel.GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Slider>().onValueChanged.AddListener((v) => TrySetFood(c, v));
            }
        }

        bool isChanging = false;
        public void TrySetPopulation(People.Community com, float value)
        {
            if (isChanging) return;
            isChanging = true;
            var comList = People.communityList;
            for(int i = 0; i<comList.Count;i++)
                transform.GetChild(i + 1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().color = colorNorm;
            People.TrySetPopulation(com, value);
            for (int i = 0; i < comList.Count; i++)
            {
                transform.GetChild(i+1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<Slider>().value = People.populationDistribution[comList[i]];
                transform.GetChild(i+1).GetChild(1).GetChild(0).GetChild(0).GetChild(1).GetComponent<Text>().text = People.PeopleAt(comList[i]).ToString("# ##0");
            }
            isChanging = false;
        }

        public void TrySetFood(People.Community com, float value)
        {
            People.TrySetFood(com, value);
            transform.Find(com.ToString()).GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>().text = People.foodAllowance[com].ToString("0.00");
        }
        public void SetFoodSlider(People.Community com, float value)
        {
            transform.Find(com.ToString()).GetChild(1).GetChild(0).GetChild(1).GetChild(0).GetComponent<Slider>().value = value;
            transform.Find(com.ToString()).GetChild(1).GetChild(0).GetChild(1).GetChild(1).GetComponent<Text>().text = People.foodAllowance[com].ToString("0.00");
        }

        public void UpdateUnrest()
        {
            var comList = People.communityList;
            for (int i = 0; i < comList.Count; i++)
            {
                transform.GetChild(i + 1).GetChild(0).GetChild(1).GetComponent<Text>().text = "Unrest: " + Mathf.Clamp01(People.unrest[comList[i]]).ToString("##0%");
            }
        }

        internal void LockPopBar(People.Community com)
        {
            transform.Find(com.ToString()).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().color = colorAlert;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position += (Vector3)eventData.delta;
        }
    }
}
