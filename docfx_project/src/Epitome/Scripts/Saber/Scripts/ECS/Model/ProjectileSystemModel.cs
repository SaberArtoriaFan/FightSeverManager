using Saber.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Saber.ECS
{
    public class ProjectileSystemModel : SystemModelBase
    {
        [SerializeField]
        AnimationCurve defaultAnimationCurve;
        private HashSet<string> poolHashSet = new HashSet<string>();

        public AnimationCurve DefaultAnimationCurve { get => defaultAnimationCurve; }
        public HashSet<string> PoolHashSet { get => poolHashSet; }

    }
}
