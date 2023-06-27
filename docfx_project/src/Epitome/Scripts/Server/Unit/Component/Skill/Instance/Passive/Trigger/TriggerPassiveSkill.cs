using cfg.skillPassive;
using Saber.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XianXia.Unit
{
    public abstract class TriggerPassiveSkill : PassiveSkill
    {
        protected int findID;
        protected int lifeCount;
        protected int count;
        protected int currCount = 0;
        protected bool isCountSkill=false;
        protected float rate;
        protected bool isRateSkill = false;
        /// <summary>
        /// 可以取几个对象
        /// </summary>
        protected int effectNum = 0;

        protected int cd;
        protected bool isCded = true;

        protected TargetType targetType;

        protected EventSystem eventSystem;
        protected UnitMainSystem mainSystem;
        protected UnitBodySystem bodySystem;
        protected TimerManagerSystem timerManagerSystem;
        //protected SaberEvent<BodyOrgan, UnitBase> UnitDamagedAfter;
        protected Dictionary<SkillUtility.SkillEffectEvent, (float, float)> skillEffectEventDict;


        public override void Init(IContainerEntity owner)
        {
            base.Init(owner);
            Enable = true;
            mainSystem = SkillSystem.World.FindSystem<UnitMainSystem>();
            bodySystem = SkillSystem.World.FindSystem<UnitBodySystem>();
            eventSystem = SkillSystem.World.FindSystem<EventSystem>();
            timerManagerSystem = SkillSystem.World.FindSystem<TimerManagerSystem>();
            //UnitDamagedAfter = eventSystem.GetEvent<BodyOrgan, UnitBase>(EventSystem.EventParameter.UnitDamagedAfter);
            skillEffectEventDict = new Dictionary<SkillUtility.SkillEffectEvent, (float, float)>();
        }

        public override void InitData(SkillPassive sp)
        {
            base.InitData(sp);
            lifeCount = SkillUtility.CalculateLifeTime(sp.SkillCount);
            effectNum = sp.ObjectNum;
            targetType = SkillUtility.CalculateTargetObject(sp.SkillObject);
            findID = sp.TargetDetermination;
            cd = sp.SkillCd;
            count = sp.SkillCount;
            rate = (float)sp.SkillRate/100;
            isCountSkill = count > 0 ? true : false;
            isRateSkill = rate > 0 ? true : false;
            SkillUtility.SkillEffectEvent s;
            for (int i = 0; i < sp.SkillEffect.Length; i++)
            {
                if (i >= sp.EffectNumer1.Length || i >= sp.EffectNumer3.Length) break;
                s = SkillUtility.GetSkillAction(sp.SkillEffect[i]);
                if (s != null)
                {
                    skillEffectEventDict.Add(s, (sp.EffectNumer1[i], sp.EffectNumer3[i]));
                    Debug.Log("加入被动效果" + sp.SkillEffect[i]);
                }
            }
        }
        public override void AcquireSkill()
        {
            base.AcquireSkill();
            isCded = true;
            //UnitDamagedAfter.AddAction(Trigger);
        }
        public override void LostSkill()
        {
            base.LostSkill();
            //UnitDamagedAfter.RemoveAction(Trigger);
        }
        public override void Destory()
        {
            base.Destory();
            eventSystem = null;
            timerManagerSystem = null;
            //attackBeforeEvent = null;
            skillEffectEventDict?.Clear();
            skillEffectEventDict = null;
        }
        protected virtual bool TrrigerCondition(OrganBase self, OrganBase other,Damage d)
        {
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targets"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="d"></param>
        protected abstract void EffectTrigger(List<UnitBase> targets, OrganBase self, OrganBase other, Damage d);
        protected void Trigger(OrganBase self, OrganBase other,Damage d)
        {
            //Debug.Log($"TTT{Enable},{lifeCount},{isCded},{isRateSkill},{isCountSkill},{TrrigerCondition(self, other, d)}");
            if (!TrrigerCondition(self, other,d)) return;
            if (!Enable) return;
            if (lifeCount == 0) return;
            if (!isCded) return;
            if (isRateSkill && !UnitUtility.CalculatePer(rate)) return;
            if (isCountSkill && ++currCount < count) return;

            currCount = 0;
            lifeCount= lifeCount>0?lifeCount--:lifeCount;
            List<UnitBase> enemies;

            if (SkillUtility.IsValidFindID(findID))
                enemies = FindTarget(targetType);
            else
                enemies = NoVaildFindID(self, other,d);
            TriggerBeforeEvent(enemies, other, self, d);
            EffectTrigger(enemies,self , other, d);
            TriggerAfterEvent(enemies, other, self, d);
            //SkillUtility.ShowRisingSpace(RealName, ownerTalentOrgan.OwnerUnit.transform.position + 0.6f * Vector3.up, Vector3.up + Vector3.left, Color.yellow);
            //foreach (var v in enemies)
            //{
            //    if (v != null)
            //    {
            //        BodyOrgan bodyOrgan = v.FindOrganInBody<BodyOrgan>(ComponentType.body);
            //        Damage damage = new Damage(b.OwnerUnit, 0, false, false);
            //        foreach (var s in skillEffectEventDict)
            //        {
            //            s.Key?.Invoke(ownerTalentOrgan.OwnerUnit, bodyOrgan, damage, s.Value.Item1, s.Value.Item2);
            //        }
            //        if (damage.Val > 0) bodySystem.ReceiveDamage(bodyOrgan, damage);
            //    }
            //}
            if (cd > 0)
            {
                isCded = false;
                timerManagerSystem.AddTimer(() => isCded = true, cd);
            }
        }

        protected virtual void TriggerAfterEvent(List<UnitBase> enemies, OrganBase other, OrganBase self, Damage d)
        {
        }

        protected virtual void TriggerBeforeEvent(List<UnitBase> enemies, OrganBase other, OrganBase self, Damage d)
        {
        }

        protected abstract List<UnitBase> NoVaildFindID(OrganBase self, OrganBase other, Damage d);
        protected virtual List<UnitBase> FindTarget(TargetType targetType)
        {
            return SkillUtility.GetFindTargetsFunc(findID, targetType, ownerTalentOrgan.OwnerUnit, effectNum);
        }
    }
}

