﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class RealmOverview : MonoBehaviour
    {
        static public RealmOverview TheOne;

        public GameObject commuityPanel;

        public void Awake() { if (TheOne == null) TheOne = this; }

        Color colorNorm = new Color(219, 204, 77);
        Color colorAlert = new Color(218, 32, 32);



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

        // Update is called once per frame
        void Update()
        {

        }

        bool isChanging = false;
        public void TrySetPopulation(People.Community com, float value)
        {
            if (isChanging) return;
            isChanging = true;
            People.TrySetPopulation(com, value);
            var comList = People.communityList;
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
    }
}
