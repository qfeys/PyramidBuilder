using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{

    public static class People
    {

        public static int totalPopulation { get; private set; }
        public enum Community { farm, quarry, transportRoad, transportRiver, construction, military };
        static public Dictionary<Community, float> populationDistribution { get; private set; }
        static public Dictionary<Community, float> foodAllowance { get; private set; }
        static public Dictionary<Community, float> unrest { get; private set; }
        static public List<Community> communityList { get { return Enum.GetValues(typeof(Community)).Cast<Community>().ToList(); } }

        static public void Init()
        {
            totalPopulation = 1000;
            populationDistribution = communityList.ToDictionary(t => t, t => 0.2f);
            foodAllowance = communityList.ToDictionary(t => t, t => 1.0f);
            unrest = communityList.ToDictionary(t => t, t => 0.0f);
            SanitisePopDist();
        }

        /// <summary>
        /// proceed one timestep
        /// </summary>
        static public void Tick()
        {
            // Food
            Farm f = God.TheOne.farm;
            float requestedFood = communityList.Sum(c => populationDistribution[c] * foodAllowance[c]);
            if (f.stock < requestedFood) // To much food allocated, reduce allocations
            {
                float reductionRatio = f.stock / requestedFood;
                foreach (var com in communityList) { foodAllowance[com] *= reductionRatio; }
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

                statisticalGrowth += Math.Pow(1 + growth / 100, 1 / 12f) * PeopleAt(com);
            }
            totalPopulation += (int)statisticalGrowth + God.random.NextDouble() < statisticalGrowth - Math.Floor(statisticalGrowth) ? 1 : 0;
            // unrest
            float suppression = God.TheOne.military.totalSuppression / totalPopulation;
            float inequality = foodAllowance.Values.Max() - foodAllowance.Values.Min();
            // due to pyramids
            foreach (var com in communityList)
            {       // Has maximum of 0.5 at 0.5 food, foes to 0 for 1.0 food and reaches minimum of -0.5 at 2.0 food
                float foodQuality;
                if (foodAllowance[com] < 0.5) foodQuality = -0.5f;
                else if (foodAllowance[com] < 1.0) foodQuality = foodAllowance[com] - 1;
                else if (foodAllowance[com] < 2.0) foodQuality = (foodAllowance[com] - 1) / 2;
                else foodQuality = 0.5f;
                unrest[com] = foodQuality + inequality - suppression;
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
            Debug.Log("" + totalAllocPop);
            foreach (var com in communityList)
            {
                populationDistribution[com] /= totalAllocPop;
            }
            Debug.Log("" + populationDistribution.Sum(kvp => kvp.Value));
        }

        static public void TrySetPopulation(Community com, float value)
        {
            float others = populationDistribution.Where(kvp => kvp.Key != com).Sum(kvp => kvp.Value);
            if (others == 0) others = float.Epsilon;
            float reductionRatio = (1 - value) / others;
            communityList.ForEach(c => populationDistribution[c] *= reductionRatio);
            populationDistribution[com] = value;
        }

        static public void TrySetFood(Community com, float value)
        {
            foodAllowance[com] = Mathf.Clamp(value, 0, 2);
        }
    }
}