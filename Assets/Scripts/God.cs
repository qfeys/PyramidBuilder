using UnityEngine;
using System;
using System.Collections.Generic;

namespace Assets.Scripts
{

    public class God : MonoBehaviour {
        public static God TheOne;

        public DateTime time { get; private set; }
        float clock;
        public int timeSetting;
        Dictionary<int, float> deltaTime = new Dictionary<int, float>() { { 1, 1.0f }, { 2, 0.5f }, { 3, 0.25f }, { 4, 0.125f }, { 5, 0.0625f } };
        public bool isPaused;

        public int pyramidTracker = 0;

        internal readonly Farm farm = new Farm();
        internal readonly Quarry quarry = new Quarry();
        internal readonly Road road = new Road();
        internal readonly River river = new River();
        internal readonly Construction construction = new Construction();
        internal readonly Military military = new Military();

        internal GOclasses.QuarryGO quarryGO;
        internal GOclasses.FarmGO farmGO;
        internal GOclasses.ConstructionGO constructionGO;
        internal GOclasses.RoadGO roadGO;
        internal GOclasses.RiverGO riverGO;

        internal UnityEngine.Events.UnityAction AdditionalUpdates;

        static internal System.Random random = new System.Random();

        public void Awake() { if (TheOne == null) TheOne = this; }
        // Use this for initialization
        void Start()
        {
            time = new DateTime(1, 1, 1);
            People.Init();
            timeSetting = 1;
            isPaused = true;
            RealmOverview.TheOne.Init();
        }

        // Update is called once per frame
        void Update() {
            if (isPaused == false)
                clock += Time.deltaTime;
            if(clock >= deltaTime[timeSetting]) // Daily tick
            {
                clock = 0;
                time = time.AddDays(1);
                farm.Tick();
                construction.Tick();
                river.Tick();
                road.Tick();
                quarry.Tick();
                military.Tick();
                if (time.Day == 1)              // Monthly tick
                {
                    People.Tick();
                    RealmOverview.TheOne.UpdateUnrest();
                }
                AdditionalUpdates();
            }
        }

        public void Report(object obj)
        {
            switch (obj.GetType().ToString())
            {
            case "Assets.Scripts.GOclasses.QuarryGO":
                quarryGO = (GOclasses.QuarryGO)obj;
                quarryGO.SetNumberOfPersons(120);
                break;
            case "Assets.Scripts.GOclasses.FarmGO":
                farmGO = (GOclasses.FarmGO)obj;
                farmGO.SetNumberOfPersons(120);
                break;
            case "Assets.Scripts.GOclasses.ConstructionGO":
                constructionGO = (GOclasses.ConstructionGO)obj;
                constructionGO.SetNumberOfPersons(120);
                break;
            case "Assets.Scripts.GOclasses.RoadGO":
                roadGO = (GOclasses.RoadGO)obj;
                roadGO.SetNumberOfPersons(120);
                break;
            case "Assets.Scripts.GOclasses.RiverGO":
                riverGO = (GOclasses.RiverGO)obj;
                riverGO.SetNumberOfPersons(120);
                break;
            default:
                Debug.Log("Bad report by: " + obj.GetType().ToString());
                break;
            }
        }

        public void Console(string s)
        {
            Debug.Log(s);
        }

        internal void UpdatePeopleDrawn()
        {
            quarryGO.SetNumberOfPersons(People.PeopleAt(People.Community.quarry));
            farmGO.SetNumberOfPersons(People.PeopleAt(People.Community.farm));
            constructionGO.SetNumberOfPersons(People.PeopleAt(People.Community.construction));
        }
    }
}
