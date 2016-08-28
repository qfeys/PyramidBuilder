using UnityEngine;
using System.Collections;

namespace Assets.Scripts.GOclasses
{
    public class RiverGO : DynamicMapObject
    {
        protected override People.Community ownCommunity { get { return People.Community.river; } }
    }
}