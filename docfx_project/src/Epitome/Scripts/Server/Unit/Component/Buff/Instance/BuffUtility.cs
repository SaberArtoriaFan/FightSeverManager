using FishNet;
using Saber.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public static class BuffUtility 
    {
        public static bool IsCanAddBuff(bool isDeBuff, StatusOrgan statusOrgan)
        {
            if (!isDeBuff) return true;
            return !UnitUtility.IsUnitInvincible(statusOrgan.OwnerUnit);
        }
        public static BuffPriorityType CalculateBuffDisperseType(int val)
        {
            switch(val)
            {
                case 0:
                    return BuffPriorityType.weak;
                case 1:
                    return BuffPriorityType.strong;
                case 2:
                    return BuffPriorityType.forever;
            }
            return BuffPriorityType.forever;
        }
        #region 特殊buff
        public static void Slient(MagicOrgan magicOrgan, UnitMainSystem mainSystem, bool isSlient)
        {
            if (magicOrgan != null)
                magicOrgan.SetEnable(isSlient);
            if (magicOrgan != null)
            {
                magicOrgan.SetEnable(isSlient);
                if (!magicOrgan.Enable && !isSlient)
                    mainSystem.UnitBreakAction(magicOrgan.OwnerUnit, FSM_State.spell);
            }
        }
        public static void Disarm(AttackOrgan attackOrgan ,UnitMainSystem mainSystem, bool isDisarm)
        {
            if (attackOrgan != null)
            {
                attackOrgan.SetEnable(isDisarm);
                if (!attackOrgan.Enable && !isDisarm)
                    mainSystem.UnitBreakAction(attackOrgan.OwnerUnit, FSM_State.attack);
            }
        }
        public static void Fetter(LegOrgan legOrgan, UnitMainSystem mainSystem, bool isFetter)
        {
            if (legOrgan != null)
                legOrgan.SetEnable(isFetter);
            if (legOrgan != null)
            {
                legOrgan.SetEnable(isFetter);
                if (!legOrgan.Enable && !isFetter)
                    mainSystem.UnitBreakAction(legOrgan.OwnerUnit, FSM_State.walk);
            }
        }
        public static void Dizziness(UnitBase unit,UnitMainSystem mainSystem,bool isDizziness)
        {
            if (unit != null)
            {
                unit.SetEnable(isDizziness);
                if (!unit.Enable&&!isDizziness)
                    mainSystem.UnitBreakAction(unit,FSM_State.max);
            }
        }
        public static Action<StatusOrgan, Buff> Invincible(StatusOrgan body,bool isInvincible,EventSystem eventSystem,Action<StatusOrgan,Buff>  action1)
        {
            Action<StatusOrgan, Buff>  action = null;
            if (body != null)
            {
                BodyOrgan bodyOrgan = body.OwnerUnit?.FindOrganInBody<BodyOrgan>(Saber.ECS.ComponentType.body);
                if (bodyOrgan != null)
                {
 
                    bodyOrgan.SetEnable(isInvincible);
                    IMagicPointRecover magicOrgan = body.OwnerUnit?.FindOrganInBody<MagicOrgan>(ComponentType.magic);

                    if (!bodyOrgan.Enable)
                    {
                        action = (s, b) =>
                        {
                            ForbidDeBuff(body, s, b);
                        };
                        Action<MagicOrgan, ActiveSkill> spellAfter = null;
                        spellAfter = (m, a) =>
                     {
                         if (magicOrgan != null)
                         {
                             magicOrgan.CanRecordMagicPoint = false;
                             magicOrgan.MagicPoint_Curr = 0;
                         }

                         eventSystem.GetEvent<MagicOrgan, ActiveSkill>(EventSystem.EventParameter.UnitSpellAfter).RemoveAction(spellAfter);
                     };
                        eventSystem.GetEvent<StatusOrgan, Buff>(EventSystem.EventParameter.UnitAddBuffEvent).AddAction(action);
                        eventSystem.GetEvent<MagicOrgan, ActiveSkill>(EventSystem.EventParameter.UnitSpellAfter).AddAction(spellAfter);
                    }
                    else
                    {
                        eventSystem.GetEvent<StatusOrgan, Buff>(EventSystem.EventParameter.UnitAddBuffEvent).RemoveAction(action1);
                        if (magicOrgan != null)
                        {
                            magicOrgan.CanRecordMagicPoint = bodyOrgan.Enable;
                            magicOrgan.MagicPoint_Curr = 0;
                        }
                    }
                }
            }
            return action;
        }
        public static void ForbidDeBuff(StatusOrgan self, StatusOrgan statusOrgan, Buff buff)
        {
            if (buff.IsDeBuff == false || (self != statusOrgan && self.OwnerUnit != statusOrgan.OwnerUnit)) return;
            buff.Enable = false;
        }
        #endregion
        #region 加载特效
        public static string LoadEffectForUnit(UnitBase unit,string effectName,int offestPos)
        {
            //NormalUtility normalUtility = InstanceFinder.GetInstance<NormalUtility>();
            string key = effectName + NormalUtility.GetId();
            InstanceFinder.GetInstance<NormalUtility>().ORPC_CreateUnitEffect(effectName, key, unit.gameObject, CalculateEffectOffestPos(unit, offestPos), false);
            return key;
        }
        public static void RecycleEffect(string key) => InstanceFinder.GetInstance<NormalUtility>()?.ORPC_RecycleEffect(key);
        public static Vector3 CalculateEffectOffestPos(UnitBase unit,int offestPos)
        {
            //脚
            if (offestPos == 3)
                return Vector3.zero;
            UIShowOrgan bodyOrgan = unit.FindOrganInBody<UIShowOrgan>(Saber.ECS.ComponentType.uIShow);
            if (bodyOrgan != null)
            {
                if (offestPos == 1)
                    return bodyOrgan.UnitHight * Vector3.up;
                else if(offestPos==2)
                    return bodyOrgan.UnitHight * Vector3.up/2;
            }
            return Vector3.zero;
        }
        #endregion
        #region 普通buff
        /// <summary>
        /// 加固定数值的，要加百分比的自己算完传进来
        /// </summary>
        /// <param name="statusOrgan"></param>
        /// <param name="num"></param>
        /// <param name="continueLong"></param>
        public static void AddAttack(StatusOrgan statusOrgan, float num)
        {
            AttackOrgan attackOrgan = null;
            if(statusOrgan!=null&&statusOrgan.OwnerUnit!=null&&(attackOrgan= statusOrgan.OwnerUnit.FindOrganInBody<AttackOrgan>(Saber.ECS.ComponentType.attack))!=null)
                attackOrgan.ExtraAttackVal += (int)num;
        }
        public static void ReduceAttack(StatusOrgan statusOrgan, float num)
        {
            AttackOrgan attackOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (attackOrgan = statusOrgan.OwnerUnit.FindOrganInBody<AttackOrgan>(Saber.ECS.ComponentType.attack)) != null)
                attackOrgan.ExtraAttackVal -= (int)num;
        }
        public static void AddDefence(StatusOrgan statusOrgan, float num)
        {
            BodyOrgan bodyOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (bodyOrgan = statusOrgan.OwnerUnit.FindOrganInBody<BodyOrgan>(Saber.ECS.ComponentType.body)) != null)
                bodyOrgan.Ex_def += (int)num;
        }
        public static void ReduceDefence(StatusOrgan statusOrgan, float num)
        {
            BodyOrgan bodyOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (bodyOrgan = statusOrgan.OwnerUnit.FindOrganInBody<BodyOrgan>(Saber.ECS.ComponentType.body)) != null)
                bodyOrgan.Ex_def -= (int)num;
        }
        public static void AddAttackRange(StatusOrgan statusOrgan, float num)
        {
            AttackOrgan attackOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (attackOrgan = statusOrgan.OwnerUnit.FindOrganInBody<AttackOrgan>(Saber.ECS.ComponentType.attack)) != null)
                attackOrgan.Ex_AttackRange += (int)num;
        }
        public static void ReduceAttackRange(StatusOrgan statusOrgan, float num)
        {
            AttackOrgan attackOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (attackOrgan = statusOrgan.OwnerUnit.FindOrganInBody<AttackOrgan>(Saber.ECS.ComponentType.attack)) != null)
                attackOrgan.Ex_AttackRange -= (int)num;
        }
        public static void AddAttackSpeed(StatusOrgan statusOrgan, float num)
        {
            AttackOrgan attackOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (attackOrgan = statusOrgan.OwnerUnit.FindOrganInBody<AttackOrgan>(Saber.ECS.ComponentType.attack)) != null)
                attackOrgan.Ex_attackSpeed += (int)num;
        }
        public static void ReduceAttackSpeed(StatusOrgan statusOrgan, float num)
        {
            AttackOrgan attackOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (attackOrgan = statusOrgan.OwnerUnit.FindOrganInBody<AttackOrgan>(Saber.ECS.ComponentType.attack)) != null)
                attackOrgan.Ex_attackSpeed -= (int)num;
        }
        public static void AddMoveSpeed(StatusOrgan statusOrgan, float num)
        {
            LegOrgan legOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (legOrgan = statusOrgan.OwnerUnit.FindOrganInBody<LegOrgan>(Saber.ECS.ComponentType.leg)) != null)
                legOrgan.MoveSpeed += (int)num;
        }
        public static void ReduceMoveSpeed(StatusOrgan statusOrgan, float num)
        {
            LegOrgan legOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (legOrgan = statusOrgan.OwnerUnit.FindOrganInBody<LegOrgan>(Saber.ECS.ComponentType.leg)) != null)
                legOrgan.MoveSpeed -= (int)num;
        }
        public static void AddCriHitProb(StatusOrgan statusOrgan, float num)
        {
            AttackOrgan attackOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (attackOrgan = statusOrgan.OwnerUnit.FindOrganInBody<AttackOrgan>(Saber.ECS.ComponentType.attack)) != null)
                attackOrgan.Ex_AttackCriticalChance += (int)num;
        }
        public static void ReduceCriHitProb(StatusOrgan statusOrgan, float num)
        {
            AttackOrgan attackOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (attackOrgan = statusOrgan.OwnerUnit.FindOrganInBody<AttackOrgan>(Saber.ECS.ComponentType.attack)) != null)
                attackOrgan.Ex_AttackCriticalChance -= (int)num;
        }
        public static void AddCriHitDamage(StatusOrgan statusOrgan, float num)
        {
            AttackOrgan attackOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (attackOrgan = statusOrgan.OwnerUnit.FindOrganInBody<AttackOrgan>(Saber.ECS.ComponentType.attack)) != null)
                attackOrgan.Ex_AttackCriticalDamage += (int)num;
        }
        public static void ReduceCriHitDamage(StatusOrgan statusOrgan, float num)
        {
            AttackOrgan attackOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (attackOrgan = statusOrgan.OwnerUnit.FindOrganInBody<AttackOrgan>(Saber.ECS.ComponentType.attack)) != null)
                attackOrgan.Ex_AttackCriticalDamage -= (int)num;
        }
        public static void AddEvade(StatusOrgan statusOrgan, float num)
        {
            BodyOrgan bodyOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (bodyOrgan = statusOrgan.OwnerUnit.FindOrganInBody<BodyOrgan>(Saber.ECS.ComponentType.body)) != null)
                bodyOrgan.Ex_Evade += (int)num;
        }
        public static void ReduceEvade(StatusOrgan statusOrgan, float num)
        {
            BodyOrgan bodyOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (bodyOrgan = statusOrgan.OwnerUnit.FindOrganInBody<BodyOrgan>(Saber.ECS.ComponentType.body)) != null)
                bodyOrgan.Ex_Evade -= (int)num;
        }
        public static void AddHitRate(StatusOrgan statusOrgan, float num)
        {
            AttackOrgan attackOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (attackOrgan = statusOrgan.OwnerUnit.FindOrganInBody<AttackOrgan>(Saber.ECS.ComponentType.attack)) != null)
                attackOrgan.Ex_AttackHitrate += (int)num;
        }
        public static void ReduceHitRate(StatusOrgan statusOrgan, float num)
        {
            AttackOrgan attackOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (attackOrgan = statusOrgan.OwnerUnit.FindOrganInBody<AttackOrgan>(Saber.ECS.ComponentType.attack)) != null)
                attackOrgan.Ex_AttackHitrate -= (int)num;
        }
        public static void ContinueHeal(StatusOrgan statusOrgan, float num,UnitBodySystem bodySystem,UnitBase source)
        {
            BodyOrgan bodyOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (bodyOrgan = statusOrgan.OwnerUnit.FindOrganInBody<BodyOrgan>(Saber.ECS.ComponentType.body)) != null)
                bodySystem.UnitHeal(bodyOrgan, (int)num, source);
        }
        public static void ContinueDamage(StatusOrgan statusOrgan, float num, UnitBodySystem bodySystem, UnitBase source)
        {
            BodyOrgan bodyOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (bodyOrgan = statusOrgan.OwnerUnit.FindOrganInBody<BodyOrgan>(Saber.ECS.ComponentType.body)) != null)
                bodySystem.ReceiveDamage(bodyOrgan, new Damage(source, (int)num, false, false));
        }
        public static void AddHPMax(StatusOrgan statusOrgan, float num)
        {
            BodyOrgan bodyOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (bodyOrgan = statusOrgan.OwnerUnit.FindOrganInBody<BodyOrgan>(Saber.ECS.ComponentType.body)) != null)
                bodyOrgan.Ex_health_Max -= (int)num;
        }
        public static void ReduceHPMax(StatusOrgan statusOrgan, float num)
        {
            BodyOrgan bodyOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (bodyOrgan = statusOrgan.OwnerUnit.FindOrganInBody<BodyOrgan>(Saber.ECS.ComponentType.body)) != null)
                bodyOrgan.Ex_health_Max -= (int)num;
        }
        public static void AddDamagePerVal(StatusOrgan statusOrgan, float num)
        {
            AttackOrgan attackOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (attackOrgan = statusOrgan.OwnerUnit.FindOrganInBody<AttackOrgan>(Saber.ECS.ComponentType.attack)) != null)
                attackOrgan.Ex_CausedDamagePer += (int)num;
        }
        public static void ReduceDamagePerVal(StatusOrgan statusOrgan, float num)
        {
            AttackOrgan attackOrgan = null;
            if (statusOrgan != null && statusOrgan.OwnerUnit != null && (attackOrgan = statusOrgan.OwnerUnit.FindOrganInBody<AttackOrgan>(Saber.ECS.ComponentType.attack)) != null)
                attackOrgan.Ex_CausedDamagePer -= (int)num;
        }
        #endregion
    }
}
