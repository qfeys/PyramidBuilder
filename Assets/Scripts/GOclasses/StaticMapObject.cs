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

        public void Start()
        {
            God.TheOne.Report(this);
        }

        internal void SetNumberOfPersons(int n)
        {
            transform.GetChild(0).GetComponent<ParticleSystem>().Emit(n);
            numberOfpersons = n;
        }
    }
}
