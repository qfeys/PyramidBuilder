using UnityEngine;
using System.Collections;

namespace Assets.Scripts.GOclasses
{
    public class ConstructionGO : StaticMapObject
    {
        protected override People.Community ownCommunity { get { return People.Community.construction; } }
    }
}
