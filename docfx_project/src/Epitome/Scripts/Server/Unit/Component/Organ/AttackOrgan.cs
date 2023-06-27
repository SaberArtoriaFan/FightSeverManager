using FSM;
using Saber.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.ECS;
using System;
using FishNet;

namespace XianXia.Unit
{
    public interface IAttackOrgan
    {
        int AttackVal { get; }
        int AttackRange { get; }
    }
    public class FloatAttributeContainer
    {
        float originValue=0;
        float exValue=0;
        //T sumValue;

        public float OriginValue { get => originValue; set => originValue = value; }
        public float ExValue { get => exValue; set => exValue = value; }
        public float SumValue { get => originValue + exValue; }
        internal void Clear()
        {
            originValue = 0;
            exValue = 0;
        }
    }
    public class IntAttributeContainer
    {
        int originValue=0;
        int exValue=0;
        //T sumValue;

        public int OriginValue { get => originValue; set => originValue = value; }
        public int ExValue { get => exValue; set => exValue = value; }
        public int SumValue { get => originValue+exValue;  }

        internal void Clear()
        {
            originValue = 0;
            exValue = 0;
        }
    }
    public class Spine_Attack : Attack
    {
        Client_UnitProperty unitProperty;
        int hashID;
        public Spine_Attack(Animator animator, float attackTime, Action attackEvent,int hashID) : base(animator, attackTime, attackEvent)
        {
            unitProperty = animator.transform.GetComponentInParent<Client_UnitProperty>();
            this.hashID = hashID;
        }
        public override void SetFinishTime(float v)
        {
            float res = InstanceFinder.GetInstance<XianXia.Spine.SpineAnimationDict>().GetAnimationLong(hashID, FSM.AnimatorParameters.Attack);
            //Debug.Log(hashID+"读取到的动画时长为"+ FSM.AnimatorParameters.Attack+"_" + res);
            if (res <= 0) { res = v;Debug.LogError(animator.gameObject.name+ "没读取到攻击动画的结束时长"); }
            base.SetFinishTime(res);
        }
        protected override void SetAnimatorParameter(bool isEnter)
        {
            animator.SetBool(FSM.AnimatorParameters.Attack, isEnter);
            //Debug.Log("设值动画参数"+animator.GetBool(FSM.AnimatorParameters.Attack));
            //InstanceFinder.GetInstance<NormalUtility>().ORPC_SetAnimatorParameter_Bool(unitProperty, FSM.AnimatorParameters.Attack, isEnter);
            //unitProperty.ORPC_AnimatorParameter_Bool(FSM.AnimatorParameters.Attack, isEnter);
        }
    }
    public class AttackOrgan : OrganBase
    {
        float attackAnimationLong = 1f;
        float attackTime=0.5f;
        FloatAttributeContainer attackRange=new FloatAttributeContainer();
        IntAttributeContainer attackVal=new IntAttributeContainer() ;
        FloatAttributeContainer origin_attackSpeed = new FloatAttributeContainer();
        //float ex_attackSpeed = 0;
       // int extraAttackVal=0;
        float warningRange = 3.5f;
        FloatAttributeContainer attackHitrate =new FloatAttributeContainer();
        FloatAttributeContainer attackCriticalChance =new FloatAttributeContainer();
        FloatAttributeContainer attackCriticalDamage =new FloatAttributeContainer();
        FloatAttributeContainer causedDamagePer = new FloatAttributeContainer();
        Projectile projectile = null;
        Transform projectileDropPoint = null;
        //GameObject projectileModel=null;
        Action attackActionOnce = null;
        protected override ComponentType componentType => ComponentType.attack;
        public CharacterFSM CharacterFSM { get => characterFSM; }
        public float AttackAnimationLong { get => attackAnimationLong; set => attackAnimationLong = value; }
        public float AttackTime { get => attackTime; set => attackTime = value; }

        CharacterFSM characterFSM = null;
        Attack attackState;
        public bool IsHasTrajectory => projectile!=null;

        public float AttackRange
        {
            //最小攻击范围
            get => attackRange.SumValue>=1.25f?attackRange.SumValue:1.25f; 
        }
        public float Origin_AttackRange { get => attackRange.OriginValue; internal set
            {
                if (value <= 1.2) value = 1.25f;
                attackRange.OriginValue = value;
            }
        }
        public float Ex_AttackRange
        {
            get => attackRange.ExValue;
            set => attackRange.ExValue = value;
        }
        public float WarningRange { get => warningRange; internal set => warningRange = value; }
        public int AttackVal { get => attackVal.SumValue; }
        public int OriginAttackVal { get => attackVal.OriginValue;set=> attackVal.OriginValue=value; }
        public Projectile Projectile { get => projectile;internal set => projectile = value; }
        //public GameObject ProjectileModel { get => projectileModel; set => projectileModel = value; }
        public Action AttackActionOnce { get => attackActionOnce; set => attackActionOnce = value; }
        public int ExtraAttackVal { get => attackVal.ExValue; set => attackVal.ExValue = value; }
        public float AttackSpeed { get => origin_attackSpeed.SumValue; }
        public Attack AttackState { get => attackState; set => attackState = value; }
        public float Ex_attackSpeed { get =>origin_attackSpeed.ExValue; set => origin_attackSpeed.ExValue = value; }
        public float Origin_AttackSpeed { get => origin_attackSpeed.OriginValue; set => origin_attackSpeed.OriginValue = value; }
        public float AttackHitrate { get => attackHitrate.SumValue;}
        public float AttackCriticalChance { get => attackCriticalChance.SumValue;  }
        public float AttackCriticalDamage { get => attackCriticalDamage.SumValue;  }
        public float Ex_AttackHitrate { get => attackHitrate.ExValue; set => attackHitrate.ExValue = value; }
        public float Ex_AttackCriticalChance { get => attackCriticalChance.ExValue; set => attackCriticalChance.ExValue = value; }
        public float Ex_AttackCriticalDamage { get => attackCriticalDamage.ExValue; set => attackCriticalDamage.ExValue = value; }
        public float Or_AttackHitrate { get => attackHitrate.OriginValue; set => attackHitrate.OriginValue = value; }
        public float Or_AttackCriticalChance { get => attackCriticalChance.OriginValue; set => attackCriticalChance.OriginValue= value; }
        public float Or_AttackCriticalDamage { get => attackCriticalDamage.OriginValue; set => attackCriticalDamage.OriginValue = value; }
        public float CausedDamagePer => causedDamagePer.SumValue;
        public float Ex_CausedDamagePer { get => causedDamagePer.ExValue; set => causedDamagePer.ExValue = value; }
        public float Or_CausedDamagePer { get => causedDamagePer.OriginValue; set => causedDamagePer.OriginValue = value; }
        public Transform ProjectileDropPoint { get => projectileDropPoint; set => projectileDropPoint = value; }

        protected override void InitComponent(EntityBase owner)
        {
            base.InitComponent(owner);
            characterFSM=owner.GetComponent<CharacterFSM>();

            Or_CausedDamagePer = 1;
            //attackState=characterFSM.f
        }
        public override void Destory()
        {
            base.Destory();
             attackAnimationLong = 1f;
             attackTime = 0.5f;
            attackRange.Clear();
             attackVal.Clear();
             origin_attackSpeed.Clear();
            //float ex_attackSpeed = 0;
            // int extraAttackVal=0;
             warningRange = 3.5f;
            attackHitrate.Clear() ;
            attackCriticalChance.Clear();
            attackCriticalDamage.Clear(); ;
            projectileDropPoint = null;
             projectile = null;
             //projectileModel = null;
             attackActionOnce = null;
            characterFSM?.RemoveState(FSM_State.attack);
            characterFSM = null;
            attackState=null;
        }



    }
}
