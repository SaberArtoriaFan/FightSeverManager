using cfg.skillPassive;
using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public  class AttackAfterPassiveSkill : AttackPassiveSkill
    {
        protected SaberEvent<AttackOrgan, BodyOrgan, Damage> attackAfterEvent;

        public override void Init(IContainerEntity owner)
        {
            base.Init(owner);
            attackAfterEvent = eventSystem.GetEvent<AttackOrgan, BodyOrgan, Damage>(EventSystem.EventParameter.UnitAttackAfter);
            bodySystem = SkillSystem.World.FindSystem<UnitBodySystem>();
        }
        public override void InitData(SkillPassive sp)
        {
            base.InitData(sp);
            //改变findID，那么效果就一定是取被攻击对象
            //方便给负面buff
            findID = 0;
        }
        public override void AcquireSkill()
        {
            base.AcquireSkill();
            attackAfterEvent.AddAction(Trigger);
        }
        public override void LostSkill()
        {
            base.LostSkill();
            attackAfterEvent.RemoveAction(Trigger);
        }
        protected override void EffectTrigger(List<UnitBase> targets, OrganBase self, OrganBase other, Damage d)
        {
            foreach (var s in skillEffectEventDict)
            {
                Damage damage = new Damage(self.OwnerUnit, 0, false, false);
                s.Key?.Invoke(self.OwnerUnit, other.OwnerUnit, d, s.Value.Item1, s.Value.Item2);
                if (damage.Val > 0) bodySystem.ReceiveDamage((BodyOrgan)other, damage);
            }
        }
        public override void Destory()
        {
            base.Destory();
            attackAfterEvent = null;
        }

        protected override List<UnitBase> NoVaildFindID(OrganBase self, OrganBase other, Damage d)
        {
            return new List<UnitBase>(1) { other.OwnerUnit };
        }
    }
}
