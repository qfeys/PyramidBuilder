using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class People {

    int totalPopulation;
    public enum Communities { farm, quarry, transportRoad, transportRiver, construction, military};
    public Dictionary<Communities, float> populationDistribution;
    public Dictionary<Communities, float> foodAllowance;

    public People()
    {
        totalPopulation = 1000;
        populationDistribution = Enum.GetValues(typeof(Communities)).Cast<Communities>().ToDictionary(t => t, t => 0.0f);
        foodAllowance = Enum.GetValues(typeof(Communities)).Cast<Communities>().ToDictionary(t => t, t => 1.0f);
    }

    /// <summary>
    /// proceed one timestep
    /// </summary>
    public void Tick()
    {

    }
}
