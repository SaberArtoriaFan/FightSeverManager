using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XianXia.Client
{
    public class SpineAnimationTransitionManager :AutoSingleton<SpineAnimationTransitionManager>
    {
        Dictionary<(Spine.Animation, Spine.Animation), Spine.Animation> transitionDict = new Dictionary<(Spine.Animation, Spine.Animation), Spine.Animation>();
        //Dictionary<string,>
        HashSet<int> alreadyInitDataAssets=new HashSet<int>();
        [SerializeField]
        TriggerAnimationScriptableObject triggerAnimations;
        HashSet<int> triggerAnimationHashSet;
        public void RegisterCondition((Spine.Animation,Spine.Animation) fromTo,Spine.Animation transition)
        {
            if (!transitionDict.ContainsKey(fromTo))
            {
                Debug.Log("注册过渡动画成功" + transition.Name);
                transitionDict.Add(fromTo, transition);
            }
        }
        public Spine.Animation TryGetCondition((Spine.Animation, Spine.Animation) fromTo)
        {
            if (transitionDict.TryGetValue(fromTo, out var v)) return v;
            else return null;
        }
        public bool IsTriggerAnimation(int hash)
        {
            return triggerAnimationHashSet.Contains(hash);
        }
        public bool RegisterAlready(int hash)
        {
            if (alreadyInitDataAssets.Contains(hash)) return true;
            else
            {
                alreadyInitDataAssets.Add(hash);
                return false;
            }
        }
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);
            if (triggerAnimations == null)
                triggerAnimations = ABUtility.Load<TriggerAnimationScriptableObject>($"{ABUtility.ScriptableObjectMainName}XianXiaTiggerAnimation");
            triggerAnimationHashSet = new HashSet<int>(triggerAnimations.AnimationNames.Select(u=>Animator.StringToHash(u.ToLower())));
        }
    }
}
