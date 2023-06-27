using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Client
{
    public class ClientSkeletonAnimationManager : AutoSingleton<ClientSkeletonAnimationManager>
    {
        Dictionary<(Spine.Animation, Spine.Animation), Spine.Animation> transitionDict = new Dictionary<(Spine.Animation, Spine.Animation), Spine.Animation>();
        //Dictionary<string,>
        public void RegisterCondition((Spine.Animation, Spine.Animation) fromTo, Spine.Animation transition)
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
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
