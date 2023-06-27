using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public class DamageOtherBeforePassiveSkill : DamageTriggerPassiveSkill
    {
        protected SaberEvent<BodyOrgan, Damage> UnitDamagedBefore;
        public override void Init(IContainerEntity owner)
        {
            base.Init(owner);
            UnitDamagedBefore = eventSystem.GetEvent<BodyOrgan, Damage>(EventSystem.EventParameter.UnitDamagedBefore);
        }
        public override void AcquireSkill()
        {
            base.AcquireSkill();
            UnitDamagedBefore.AddAction(DamageTrigger);
        }
        public override void LostSkill()
        {
            base.LostSkill();
            UnitDamagedBefore.RemoveAction(DamageTrigger);
        }
        protected virtual void DamageTrigger(BodyOrgan b, Damage d)
        {
            Trigger(d.Source.FindOrganInBody<BodyOrgan>(ComponentType.body),b , d);
        }
    }
}
