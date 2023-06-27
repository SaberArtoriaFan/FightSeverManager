using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Client
{
    public interface IAnimationHandle
    {
        void PlayAnimationForState(int nameHash, int layerIndex);
    }

    // This StateMachineBehaviour handles sending the Mecanim state information to the component that handles playing the Spine animations.
    public class XianXiaMecanimToAnimationHandle : StateMachineBehaviour
    {
        IAnimationHandle animationHandle;
        //bool initialized;

        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animationHandle==null)
            {
                animationHandle = animator.GetComponentInChildren<IAnimationHandle>();
                //initialized = true;
            }
            //Debug.Log(stateInfo.shortNameHash + stateInfo.IsName("attack").ToString());
            animationHandle?.PlayAnimationForState(stateInfo.shortNameHash, layerIndex);
        }
    }
}
