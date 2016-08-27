using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{

    public static class People
    {

        static int totalPopulation;
        public enum Community { farm, quarry, transportRoad, transportRiver, construction, military };
        static public Dictionary<Community, float> populationDistribution { get; private set; }
        static public Dictionary<Community, float> foodAllowance { get; private set; }
        static public Dictionary<Community, float> unrest { get; private set; }

        static public void Init()
        {
            totalPopulation = 1000;
            populationDistribution = Enum.GetValues(typeof(Community)).Cast<Community>().ToDictionary(t => t, t => 0.0f);
            foodAllowance = Enum.GetValues(typeof(Community)).Cast<Community>().ToDictionary(t => t, t => 1.0f);
            unrest = Enum.GetValues(typeof(Community)).Cast<Community>().ToDictionary(t => t, t => 0.0f);
        }

        /// <summary>
        /// proceed one timestep
        /// </summary>
        static public void Tick()
        {

        }

        static public int PeopleAt(Community comunity)
        {
            return (int)(totalPopulation * populationDistribution[comunity]);
        }

        static public float Productivity(Community comunity)
        {
            return 1 - unrest[comunity];
        }
    }
}