using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.GOclasses
{
    public class StaticMapObject : MonoBehaviour
    {
        protected int numberOfpersons;
        protected virtual People.Community ownCommunity { get { throw new Exception("Consider this class abstract"); } }

        public void Start()
        {
            God.TheOne.Report(this);
        }

        internal void SetNumberOfPersons(int n)
        {
            var em = transform.GetChild(0).GetComponent<ParticleSystem>().emission;
            em.rate = new ParticleSystem.MinMaxCurve(n / 60);
            numberOfpersons = n;
        }

        public void OnMouseUpAsButton()
        {
            Debug.Log("Mouse Clicked" + ownCommunity);
            Outliner.TheOne.setPanelInfo(ownCommunity);
        }
    }
}
