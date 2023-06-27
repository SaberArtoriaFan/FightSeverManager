using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public class DamageOtherAfterPassiveSkill:DamageTriggerPassiveSkill
    {
        protected SaberEvent<BodyOrgan, Damage> UnitDamagedAfter;
        public override void Init(IContainerEntity owner)
        {
            base.Init(owner);
            UnitDamagedAfter = eventSystem.GetEvent<BodyOrgan, Damage>(EventSystem.EventParameter.UnitDamagedAfter);

        }
        public override void AcquireSkill()
        {
            base.AcquireSkill();
            UnitDamagedAfter.AddAction(DamageTrigger);
        }
        public override void LostSkill()
        {
            base.LostSkill();
            UnitDamagedAfter.RemoveAction(DamageTrigger);
        }
        protected virtual void DamageTrigger(BodyOrgan b, Damage u)
        {
            Trigger(u.Source.FindOrganInBody<BodyOrgan>(ComponentType.body), b, u);
        }
    }
}
