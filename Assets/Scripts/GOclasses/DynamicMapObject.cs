using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.GOclasses
{
    public class DynamicMapObject : MonoBehaviour
    {
        protected List<Team> teams;   // Progress on the track + number of persons
        public List<Transform> route;
        public GameObject prototypeTeam;
        float totalLength;
        int passivePersons;
        Dictionary<int, float> distances;
        protected virtual People.Community ownCommunity { get { throw new Exception("Consider this class abstract"); } }

        public void Start()
        {
            God.TheOne.Report(this);
            teams = new List<Team>();
            totalLength = 0;
            distances = new Dictionary<int, float>();
            for (int i = 0; i < route.Count - 1; i++)
            {
                totalLength += (route[i + 1].position - route[i].position).magnitude;       // Assume y = 0 for every waypoint
                distances.Add(i, (route[i + 1].position - route[i].position).magnitude);
            }
            God.TheOne.AdditionalUpdates += Tick;
        }

        virtual protected void Tick()
        {
        }

        public void SetNumberOfPersons(int n)
        {
            if (teams == null || teams.Count == 0) passivePersons = n;
            else passivePersons = n - teams.Sum(t => t.persons);
            if (passivePersons < 0) throw new ArgumentException("invalid number of persons. They are working on the road.");
            var em = transform.GetChild(0).GetComponent<ParticleSystem>().emission;
            var rate = new ParticleSystem.MinMaxCurve();
            rate.constantMin = passivePersons / 30f;
            rate.constantMax = passivePersons / 30f;
            em.rate = rate;
        }

        public Team launchTeam(int id, int persons)
        {
            Team t = new Team(id, Instantiate(prototypeTeam), persons, this);
            var em = t.core.GetComponent<ParticleSystem>().emission;
            var rate = new ParticleSystem.MinMaxCurve();
            rate.constantMin = persons / 30f;
            rate.constantMax = persons / 30f;
            em.rate = rate;
            t.core.GetComponent<ParticleSystem>().Play();
            teams.Add(t);
            return t;
        }

        Vector3 FindPosition(float progress)
        {
            float distanceTraveled = totalLength * progress;
            int i = 0;
            while (distanceTraveled > distances[i])
            {
                distanceTraveled -= distances[i];
                if (i == distances.Count - 1) break;
                i++;
            }
            float fractionOfLastLeg = distanceTraveled / distances[i];
            return route[i].position + (route[i + 1].position - route[i].position) * fractionOfLastLeg;
        }

        public void OnMouseUpAsButton()
        {
            Outliner.TheOne.setPanelInfo(ownCommunity);
        }


        public class Team
        {
            public readonly int id;
            public readonly GameObject core;
            public int persons { get; private set; }
            readonly DynamicMapObject boss;
            public float progress { get; private set; }
            public Team(int id, GameObject core, int persons, DynamicMapObject boss)
            {
                this.id = id; this.core = core; this.persons = persons; this.boss = boss; progress = 0;
                core.transform.position = boss.FindPosition(progress);
                core.transform.GetComponent<ParticleSystem>().Emit(persons);
            }

            public void SetPRogress(float newProgress)
            {
                progress = newProgress;
                core.transform.position = boss.FindPosition(progress);
            }

            public void SetPersons(int newPersons)
            {
                persons = newPersons;
                var em = core.GetComponent<ParticleSystem>().emission;
                var rate = new ParticleSystem.MinMaxCurve();
                rate.constantMin = persons / 30f;
                rate.constantMax = persons / 30f;
                em.rate = rate;
            }
        }

    }

}
