using cfg.skillPassive;
using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public  class AttackBeforePassiveSkill : AttackPassiveSkill
    {
        protected SaberEvent<AttackOrgan, BodyOrgan, Damage> attackBeforeEvent;
        protected override void EffectTrigger(List<UnitBase> targets, OrganBase self, OrganBase other, Damage d)
        {
            foreach (var s in skillEffectEventDict)
                s.Key?.Invoke(self.OwnerUnit, other.OwnerUnit, d, s.Value.Item1, s.Value.Item2);
        }
        public override void Init(IContainerEntity owner)
        {
            base.Init(owner);
            attackBeforeEvent = eventSystem.GetEvent<AttackOrgan, BodyOrgan, Damage>(EventSystem.EventParameter.UnitAttackBefore);
        }
        public override void InitData(SkillPassive sp)
        {
            base.InitData(sp);
            findID = 0;
        }
        public override void AcquireSkill()
        {
            base.AcquireSkill();
            attackBeforeEvent.AddAction(Trigger);
        }
        public override void LostSkill()
        {
            base.LostSkill();
            attackBeforeEvent.RemoveAction(Trigger);
        }
        public override void Destory()
        {
            base.Destory();
            attackBeforeEvent = null;
        }

        protected override List<UnitBase> NoVaildFindID(OrganBase self, OrganBase other, Damage d)
        {
            return new List<UnitBase>() { other?.OwnerUnit };
        }
    }
}
