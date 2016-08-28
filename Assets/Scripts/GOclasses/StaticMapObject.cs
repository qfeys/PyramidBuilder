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
            numberOfpersons = n;
            var em = transform.GetChild(0).GetComponent<ParticleSystem>().emission;
            var rate = new ParticleSystem.MinMaxCurve();
            rate.constantMin = n / 30f;
            rate.constantMax = n / 30f;
            em.rate = rate;
        }

        public void OnMouseUpAsButton()
        {
            Outliner.TheOne.setPanelInfo(ownCommunity);
        }
    }
}
