using UnityEngine;
using System.Collections;

namespace Assets.Scripts.GOclasses
{
    public class RiverGO : DynamicMapObject
    {
        protected override People.Community ownCommunity { get { return People.Community.river; } }

        protected override void Tick()
        {
            for (int i = teams.Count - 1; i >= 0; i--)
            {
                Team tgo = teams[i];
                var boat = God.TheOne.river.boats.Find(b => b.id == tgo.id);
                if (boat == null)
                {
                    Destroy(teams[i].core);
                    teams.RemoveAt(i);
                }
                else
                {
                    tgo.SetPRogress(1 - (float)boat.timeTillArrival / God.TheOne.road.transportTime);
                }
            }
        }

        public void changeCrew(int id, int newCrew)
        {
            teams.Find(b => b.id == id).SetPersons(newCrew);
        }
    }
}