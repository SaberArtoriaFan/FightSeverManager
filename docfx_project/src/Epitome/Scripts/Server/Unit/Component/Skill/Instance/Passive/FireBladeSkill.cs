using Saber.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public class FireBladeSkill : PassiveSkill
    {
        EventSystem eventSystem;
        TimerManagerSystem timerManagerSystem;
        SaberEvent<AttackOrgan, BodyOrgan, Damage> attackBeforeEvent;
        UnitBase ownerUnit;
        float per = 0.15f;
        float builtIn_CD = 0f;
        bool isCD = true;
        float damageMultiple = 3.0f;
        public override void Init(IContainerEntity owner)
        {
            base.Init(owner);
            eventSystem=SkillSystem.World.FindSystem<EventSystem>();
            timerManagerSystem = SkillSystem.World.FindSystem<TimerManagerSystem>();
            attackBeforeEvent=eventSystem.GetEvent<AttackOrgan, BodyOrgan, Damage>(EventSystem.EventParameter.UnitAttackBefore);
            ownerUnit = ownerTalentOrgan.OwnerUnit;
        }
        public override void Destory()
        {
            base.Destory();
            eventSystem = null;
            attackBeforeEvent = null;
        }
        public override void AcquireSkill()
        {
            base.AcquireSkill();
            attackBeforeEvent.AddAction(CriticalStrike);
        }

        private void CriticalStrike(AttackOrgan attackOrgan, BodyOrgan enemy, Damage damage)
        {
            if(attackOrgan==null||attackOrgan.OwnerUnit==null||attackOrgan.OwnerUnit!=ownerTalentOrgan.OwnerUnit||enemy==null||enemy.OwnerUnit==null||damage==null) return;
            if (isCD&&damage.IsAttack&&!damage.IsCriticalStrike&&UnitUtility.CalculatePer(per))
            {
                damage.Val= (int)(damage.Val * (damageMultiple));
                damage.IsCriticalStrike = true;
                //damage.va
                if (builtIn_CD > 0)
                {
                    isCD = false;
                    timerManagerSystem.AddTimer(() => isCD = true, builtIn_CD);

                }
            }
        }

        public override void LostSkill()
        {
            base.LostSkill();
            attackBeforeEvent.RemoveAction(CriticalStrike);

        }
    }
}
