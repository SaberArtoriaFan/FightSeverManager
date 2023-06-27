using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Client
{
    public static class AnimatorParameters
    {
        public static readonly int Walk = Animator.StringToHash("Walk");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Death = Animator.StringToHash("Death");
        public static readonly int Spell = Animator.StringToHash("Spell");
        public static readonly int Damaged = Animator.StringToHash("Damaged");
        public static readonly int Idle = Animator.StringToHash("Idle");
        public static readonly int FinishAttack = Animator.StringToHash("FinishAttack");
        public static readonly int Reset = Animator.StringToHash("Reset");



    }
    public abstract class SpineFsmState:FSM.FSMBase
    {
       protected XianXiaSkeletonAnimationHandle animationHandle;

        public SpineFsmState(Animator animator, XianXiaSkeletonAnimationHandle animationHandle) : base(animator)
        {
            this.animationHandle = animationHandle;
        }
        public override void Destory()
        {
            base.Destory();
            animationHandle = null;
        }
    }
    public abstract class SpineTriggerFsmState : SpineFsmState
    {
        protected string animationName;
        Action ExitAction;
        protected bool isLoop = false;
        protected SpineTriggerFsmState(Animator animator, XianXiaSkeletonAnimationHandle animationHandle,string keyName,Action exitAction) : base(animator, animationHandle)
        {
            this.ExitAction = exitAction;
            animationName = keyName.ToLower();
            animationHandle.skeletonAnimation.AnimationState.Complete += RealExit;


        }

        protected  void RealExit(TrackEntry trackEntry)
        {
            if (trackEntry.ToString().ToLower() != animationName) return;
            Exit();
            ExitAction?.Invoke();
        }
        protected virtual void Exit()
        {

        }
        public sealed override void OnExit()
        {
            if (isLoop)
                Exit();
        }
        public override void Destory()
        {
            base.Destory();
            if(animationHandle!=null)
                animationHandle.skeletonAnimation.AnimationState.Complete -= RealExit;
            ExitAction = null;
        }
    }

    public abstract class SpineTriggerEventFsmState : SpineTriggerFsmState
    {
        Action TriggerAction;
        EventData eventData;
        protected SpineTriggerEventFsmState(Animator animator, XianXiaSkeletonAnimationHandle animationHandle, string keyName, Action exitAction) : base(animator, animationHandle, keyName, exitAction)
        {
        }
        public void InitTriggerEvent(string eventName, Action eventAction)
        {
            if (eventAction != null)
            {
                this.TriggerAction = eventAction;
                eventData = animationHandle.skeletonAnimation.Skeleton.Data.FindEvent(eventName);
                animationHandle.skeletonAnimation.AnimationState.Event += HandleAnimationStateEvent;
            }
        }

        protected void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e)
        {
            if(e.Data==eventData &&trackEntry.ToString().ToLower() == animationName) 
            {
                TriggerAction?.Invoke();
            }
        }
        public override void Destory()
        {
            base.Destory();
            TriggerAction = null;
            eventData = null;
        }
    }

    public class SpineIdle : SpineFsmState
    {
        public SpineIdle(Animator animator, XianXiaSkeletonAnimationHandle animationHandle) : base(animator, animationHandle)
        {
        }

        public override FSM_State State => FSM_State.idle;
    }
    public class SpineWalk : SpineFsmState
    {
        FSM.IWalk foot;
        public SpineWalk(Animator animator, XianXiaSkeletonAnimationHandle animationHandle,FSM.IWalk walk) : base(animator, animationHandle)
        {
            foot = walk;
        }

        public override FSM_State State => FSM_State.walk;
        public override void OnEnter()
        {
            base.OnEnter();
            animator.SetBool(AnimatorParameters.Walk,true);
            animationHandle.skeletonAnimation.loop = true;
            if(foot!=null)
                animationHandle.skeletonAnimation.timeScale = foot.WalkSpeed;
        }
        public override void OnStay()
        {
            base.OnStay();
            if(foot!=null)
                animationHandle.skeletonAnimation.timeScale = foot.WalkSpeed;
        }
        public override void OnExit()
        {
            base.OnExit();
            animator.SetBool(AnimatorParameters.Walk,false);
            if (animationHandle != null)
                animationHandle.skeletonAnimation.timeScale =1;
        }
        public override void Destory()
        {
            base.Destory();
            foot = null;
        }

    }

    public class SpineAttack : SpineTriggerEventFsmState
    {
        protected float speed;
        public SpineAttack(Animator animator, XianXiaSkeletonAnimationHandle animationHandle, string keyName, Action exitAction) : base(animator, animationHandle, keyName, exitAction)
        {
        }

        public override FSM_State State => FSM_State.attack;
        public void SetAttackSpeed(float speed)
        {
            this.speed = speed > 0 ? speed : 1;
        }
        public override void OnEnter()
        {
            base.OnEnter();
            animator.SetBool(AnimatorParameters.Attack, true);
            animationHandle.skeletonAnimation.loop = false;
            animationHandle.skeletonAnimation.timeScale = speed;
        }
        protected override void Exit()
        {
            base.Exit();
            animator.SetBool(AnimatorParameters.Attack, false);
            animationHandle.skeletonAnimation.timeScale = 1;
        }
    }

    public class SpineSpell : SpineTriggerEventFsmState
    {
        protected float speed;
        public SpineSpell(Animator animator, XianXiaSkeletonAnimationHandle animationHandle, string keyName, Action exitAction,bool isLoop) : base(animator, animationHandle, keyName, exitAction)
        {
            this.isLoop = isLoop;
        }

        public override FSM_State State => FSM_State.spell;
        public void SetSpellSpeed(float speed)
        {
            this.speed = speed > 0 ? speed : 1;
        }
       
        public override void OnEnter()
        {
            base.OnEnter();
            animator.SetBool(AnimatorParameters.Spell, true);
            animationHandle.skeletonAnimation.loop = false;
            animationHandle.skeletonAnimation.timeScale = speed;
        }
        protected override void Exit()
        {
            base.Exit();
            animator.SetBool(AnimatorParameters.Spell, false);
            animationHandle.skeletonAnimation.timeScale = 1;
        }
    }

    public class SpineDeath : SpineTriggerEventFsmState
    {
        public SpineDeath(Animator animator, XianXiaSkeletonAnimationHandle animationHandle, string keyName, Action exitAction) : base(animator, animationHandle, keyName, exitAction)
        {
        }

        public override FSM_State State => FSM_State.death;
        public override void OnEnter()
        {
            base.OnEnter();
            animator.SetBool(AnimatorParameters.Death, true);
            animationHandle.skeletonAnimation.loop = false;
            animationHandle.skeletonAnimation.timeScale =1;
        }
        protected override void Exit()
        {
            base.Exit();
            animator.SetBool(AnimatorParameters.Death, false);
        }
    }
}
