using UnityEngine;
using System.Collections;

namespace Assets.Scripts.GOclasses
{
    public class RoadGO : DynamicMapObject
    {
        protected override People.Community ownCommunity { get { return People.Community.road; } }

        protected override void Tick()
        {
            for (int i = teams.Count - 1; i >= 0; i--)
            {
                Team tgo = teams[i];
                var team = God.TheOne.road.teamsOnTheWay.Find(t => t.id == tgo.id);
                if (team == null) teams.RemoveAt(i);
                else
                {
                    tgo.SetPRogress(1 - (float)team.TimeLeft / God.TheOne.road.transportTime);
                }
            }
        }
    }
}