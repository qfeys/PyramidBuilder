using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts
{
    class KeyboardListener : MonoBehaviour
    {
        public void Update()
        {
            if (Input.GetButtonDown("Pause"))
                Outliner.TheOne.TimePause();
            if (Input.GetButtonDown("Time+"))
                Outliner.TheOne.TimeUp();
            if (Input.GetButtonDown("Time-"))
                Outliner.TheOne.TimeDown();
            if (Input.GetButtonDown("Overview"))
                Outliner.TheOne.ToggleRealmOverview();
            if (Input.GetButtonDown("Sound"))
                God.TheOne.gameObject.GetComponent<AudioSource>().enabled = !God.TheOne.gameObject.GetComponent<AudioSource>().enabled;
        }
    }
}
