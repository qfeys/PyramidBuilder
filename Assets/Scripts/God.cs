using UnityEngine;
using System.Collections;

namespace Assets.Scripts
{

    public class God : MonoBehaviour {
        public static God TheOne;

        int Time;
        float deltaTime;

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
            Time = 0;
            People.Init();
        }

        // Update is called once per frame
        void Update() {
            People.Tick();
        }

        public void Console(string s)
        {
            Debug.Log(s);
        }
    }
}
