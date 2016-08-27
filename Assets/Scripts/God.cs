using UnityEngine;
using System.Collections;

public class God : MonoBehaviour {

    int Time;
    People people;
    float deltaTime;

	// Use this for initialization
	void Start () {
        Time = 0;
        people = new People();
	}
	
	// Update is called once per frame
	void Update () {
        people.Tick();
	}
}
