using UnityEngine;
using System.Collections;

namespace Assets.Scripts.GOclasses
{
    public class RoadGO : DynamicMapObject
    {
        protected override People.Community ownCommunity { get { return People.Community.road; } }
    }
}