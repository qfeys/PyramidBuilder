using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class RealmOverview : MonoBehaviour
    {
        static public RealmOverview TheOne;

        public GameObject commuityPanel;

        public void Awake() { if (TheOne == null) TheOne = this; }



        // Use this for initialization
        void Start()
        {
            gameObject.SetActive(false);
        }

        public void Init()
        {
            foreach (var com in People.communityList)
            {
                Transform panel = Instantiate(commuityPanel).transform;
                panel.SetParent(transform);
                panel.GetChild(0).GetChild(0).GetComponent<Text>().text = com.ToString();
                Debug.Log("Panel: " + com.ToString());
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
