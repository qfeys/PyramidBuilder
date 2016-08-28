using UnityEngine;
using System.Collections;

namespace Assets.Scripts.GOclasses
{
    public class MilitaryGO : StaticMapObject
    {
        protected override People.Community ownCommunity { get { return People.Community.military; } }
    }
}
