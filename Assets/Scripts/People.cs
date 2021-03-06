﻿using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace Assets.Scripts
{

    public static class People
    {

        public static int totalPopulation { get; private set; }
        public enum Community { farm, quarry, road, river, construction, military };
        static public Dictionary<Community, float> populationDistribution { get; private set; }
        static public Dictionary<Community, float> foodAllowance { get; private set; }
        static public Dictionary<Community, float> unrest { get; private set; }
        static public List<Community> communityList { get { return Enum.GetValues(typeof(Community)).Cast<Community>().ToList(); } }

        static public void Init()
        {
            totalPopulation = 1000;
            populationDistribution = communityList.ToDictionary(t => t, t => 0.2f);
            SanitisePopDist();
            foodAllowance = communityList.ToDictionary(t => t, t => 1.0f);
            unrest = communityList.ToDictionary(t => t, t => 0.0f);
        }

        /// <summary>
        /// proceed one timestep
        /// </summary>
        static public void Tick()
        {
            // Food
            Farm f = God.TheOne.farm;
            float requestedFood = communityList.Sum(c => populationDistribution[c] * foodAllowance[c])*totalPopulation * DateTime.DaysInMonth(God.TheOne.time.AddDays(-1).Year, God.TheOne.time.AddDays(-1).Month);
            if (f.stock < requestedFood) // To much food allocated, reduce allocations
            {
                float reductionRatio = f.stock / requestedFood;
                foreach (var com in communityList) { foodAllowance[com] *= reductionRatio; RealmOverview.TheOne.SetFoodSlider(com, foodAllowance[com]); }
                requestedFood = communityList.Sum(c => populationDistribution[c] * foodAllowance[c]);
                if (f.stock < requestedFood) throw new Exception("Bad recalculation of the food requests");
            }
            f.TakeFood(requestedFood);
            // growth
            // at normal food it is 2% per year, at 0.5 food 0%, at 0 food -100%, max growth is at 1.5 food with 3% growth
            double statisticalGrowth = 0;
            foreach (var com in communityList)
            {
                float growth;   // in percent
                if (foodAllowance[com] > 1.5f) growth = 3;
                else if (foodAllowance[com] > 1.0f) growth = 2 + (foodAllowance[com] - 1) * 2;
                else if (foodAllowance[com] > 0.5f) growth = (foodAllowance[com] - 0.5f) * 4;
                else growth = (foodAllowance[com] - 0.5f) * -100;
                statisticalGrowth += Math.Pow(1 + growth / 100, 1 / 12f) * PeopleAt(com) - PeopleAt(com);
            }
            totalPopulation += (int)statisticalGrowth;
            totalPopulation += God.random.NextDouble() < statisticalGrowth - Math.Floor(statisticalGrowth) ? 1 : 0;
            // unrest
            float suppression = God.TheOne.military.averageSupression;
            float inequality = God.TheOne.military.inequality;
            float pyramidUnrest = God.TheOne.military.pyramidUnrest;
            foreach (var com in communityList)
            {       // Has maximum of 0.5 at 0.5 food, foes to 0 for 1.0 food and reaches minimum of -0.5 at 2.0 food
                float foodQuality;
                if (foodAllowance[com] < 0.5) foodQuality = -0.5f;
                else if (foodAllowance[com] < 1.0) foodQuality = foodAllowance[com] - 1;
                else if (foodAllowance[com] < 2.0) foodQuality = (foodAllowance[com] - 1) / 2;
                else foodQuality = 0.5f;
                unrest[com] = foodQuality + inequality - suppression;
            }
            if (unrest.All(u => u.Value >= 1))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("EndScreen");
                GameObject.Find("FinalText").GetComponent<Text>().text = "You build " + God.TheOne.pyramidTracker + " pyramids before you got killed!";
                GameObject.Find("FinalText").GetComponent<AudioSource>().Play();
            }
        }

        static public int PeopleAt(Community comunity)
        {
            return (int)(totalPopulation * populationDistribution[comunity]);
        }

        static public float Productivity(Community comunity)
        {
            return 1 - unrest[comunity];
        }

        static void SanitisePopDist()
        {
            float totalAllocPop = populationDistribution.Sum(kvp => kvp.Value);
            foreach (var com in communityList)
            {
                populationDistribution[com] /= totalAllocPop;
            }
        }

        static public void TrySetPopulation(Community com, float value)
        {
            var comList = communityList;
            float personDiff = (int)((populationDistribution[com] - value) );
            float equalShare = personDiff / (comList.Count-1);
            communityList.ForEach(c => populationDistribution[c] += equalShare);
            populationDistribution[com] = value;
            if (God.TheOne.road.peopleBusy > populationDistribution[Community.road] * totalPopulation)
            {
                populationDistribution[Community.road] = (float)God.TheOne.road.peopleBusy / totalPopulation;
                RealmOverview.TheOne.LockPopBar(Community.road);
            }
            if (God.TheOne.river.peopleBusy > populationDistribution[Community.river] * totalPopulation)
            {
                populationDistribution[Community.river] = God.TheOne.river.peopleBusy / totalPopulation;
                RealmOverview.TheOne.LockPopBar(Community.river);
            }
            SanitisePopDist();
            God.TheOne.UpdatePeopleDrawn();
            
        }

        static public void TrySetFood(Community com, float value)
        {
            foodAllowance[com] = Mathf.Clamp(value, 0, 2);
        }
    }
}