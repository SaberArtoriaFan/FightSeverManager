using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public class HealSkill : PassiveSkill
    {
        EventSystem eventSystem;
        UnitBodySystem unitBodySystem;
        //TimerManagerSystem timerManagerSystem;
        SaberEvent<AttackOrgan, BodyOrgan, Damage> attackAfterEvent;
        //UnitBase ownerUnit;
        //float per = 0.15f;
        //float builtIn_CD = 5f;
        //bool isCD = true;
        float healPer = 0.15f;
        int triggerNum = 5;
        int num = 0;
        public override void Init(IContainerEntity owner)
        {
            base.Init(owner);
            eventSystem = SkillSystem.World.FindSystem<EventSystem>();
            //timerManagerSystem = SkillSystem.World.FindSystem<TimerManagerSystem>();
            attackAfterEvent = eventSystem.GetEvent<AttackOrgan, BodyOrgan, Damage>(EventSystem.EventParameter.UnitAttackAfter);
            unitBodySystem = SkillSystem.World.FindSystem<UnitBodySystem>();
            //ownerUnit = ownerTalentOrgan.OwnerUnit;
        }
        public override void Destory()
        {
            base.Destory();
            eventSystem = null;
            attackAfterEvent = null;
        }
        public override void AcquireSkill()
        {
            base.AcquireSkill();
            attackAfterEvent.AddAction(AttackEvent);
        }

        private void AttackEvent(AttackOrgan attackOrgan, BodyOrgan enemy, Damage damage)
        {
            if (attackOrgan == null || attackOrgan.OwnerUnit == null || ownerTalentOrgan==null||ownerTalentOrgan.OwnerUnit==null||attackOrgan.OwnerUnit != ownerTalentOrgan.OwnerUnit || damage == null) return;
            if (/*isCD &&*/ damage.IsAttack/*&& SystemUtility.CalculatePer(per)*/)
            {
                num++;
                if (num >= triggerNum)
                {
                    num = 0;
                    BodyOrgan bodyOrgan = attackOrgan.OwnerUnit.FindOrganInBody<BodyOrgan>(ComponentType.body);
                    if (bodyOrgan == null) return;
                    unitBodySystem.UnitHeal(bodyOrgan, (int)(bodyOrgan.Health_Max * healPer),bodyOrgan.OwnerUnit);
                }
                //isCD = false;
                //damage.Val = (int)(damage.Val * (damageMultiple));
                //damage.IsCriticalStrike = true;
                //damage.va
                //timerManagerSystem.AddTimer(() => isCD = true, builtIn_CD);
            }
        }

        public override void LostSkill()
        {
            base.LostSkill();
            attackAfterEvent.RemoveAction(AttackEvent);

        }
    }
}
