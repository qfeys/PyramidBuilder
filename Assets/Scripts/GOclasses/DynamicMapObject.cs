using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.GOclasses
{
    public class DynamicMapObject : MonoBehaviour
    {
        List<Team> teams;   // Progress on the track + number of persons
        public List<Transform> route;
        public GameObject prototypeTeam;
        float totalLength;
        int passivePersons;
        Dictionary<int, float> distances;

        public void Start()
        {
            God.TheOne.Report(this);
            teams = new List<Team>();
            totalLength = 0;
            distances = new Dictionary<int, float>();
            for (int i = 0; i < route.Count-1; i++)
            {
                totalLength += (route[i + 1].position - route[i].position).magnitude;       // Assume y = 0 for every waypoint
                distances.Add(i, (route[i + 1].position - route[i].position).magnitude);
            }
        }

        public void AddPersons(int p)
        {
            passivePersons += p;
            transform.GetChild(0).GetComponent<ParticleSystem>().Emit(passivePersons);
        }

        public void SetNumberOfPersons(int n)
        {
            passivePersons = n - teams.Sum(t => t.persons);
            if (passivePersons < 0) throw new ArgumentException("invalid number of persons. They are working on the road.");
            transform.GetChild(0).GetComponent<ParticleSystem>().Emit(passivePersons);
        }

        public void launchTeam(int id, int persons)
        {
            teams.Add(new Team(id, Instantiate(prototypeTeam), persons, this));
        }

         Vector3 FindPosition(float progress)
        {
            float distanceTraveled = totalLength * progress;
            int i = 0;
            while (distanceTraveled > distances[i])
            {
                distanceTraveled -= distances[i];
                i++;
            }
            float fractionOfLastLeg = distanceTraveled / distances[i];
            return route[i].position + (route[i + 1].position - route[i].position) * fractionOfLastLeg;
        }


        class Team
        {
            public readonly int id;
            public readonly GameObject core;
            public readonly int persons;
            readonly DynamicMapObject boss;
            public float progress { get; private set; }
            public Team(int id, GameObject core, int persons, DynamicMapObject boss) { this.id = id; this.core = core;  this.persons = persons; this.boss = boss; progress = 0;
                core.transform.position = boss.route[0].position;
                core.transform.GetComponent<ParticleSystem>().Emit(persons);
            }

            public void SetPRogress(float newProgress)
            {
                progress = newProgress;
                core.transform.position = boss.FindPosition(progress);
            }
            
        }

    }

}
