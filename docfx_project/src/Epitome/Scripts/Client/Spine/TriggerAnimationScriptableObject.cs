using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Client
{
    [CreateAssetMenu(menuName ="XianXia/TriggerAnimation")]
    public class TriggerAnimationScriptableObject : ScriptableObject
    {
        [SerializeField]
        string[] animationNames;

        public string[] AnimationNames { get => animationNames; set => animationNames = value; }
    }
}
