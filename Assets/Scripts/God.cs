﻿using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts
{

    public class God : MonoBehaviour {
        public static God TheOne;

        int time;
        float clock;
        public int timeSetting;
        Dictionary<int, float> deltaTime = new Dictionary<int, float>() { { 1, 1.0f }, { 2, 0.5f }, { 3, 0.25f }, { 4, 0.125f }, { 5, 0.0625f } };
        public bool isPaused;

        internal readonly Farm farm = new Farm();
        internal readonly Quarry quarry = new Quarry();
        internal readonly Road road = new Road();
        internal readonly River river = new River();
        internal readonly Construction construction = new Construction();
        internal readonly Military military = new Military();

        // Use this for initialization
        void Start()
        {
            if (TheOne == null) TheOne = this;
            time = 0;
            People.Init();
            timeSetting = 1;
            isPaused = true;
        }

        // Update is called once per frame
        void Update() {
            clock += Time.deltaTime;
            if(clock >= deltaTime[timeSetting])
            {
                clock = 0;
                People.Tick();
                farm.Tick();
                quarry.Tick();
                road.Tick();
                river.Tick();
                construction.Tick();
                military.Tick();
            }
        }

        public void Console(string s)
        {
            Debug.Log(s);
        }
    }
}
