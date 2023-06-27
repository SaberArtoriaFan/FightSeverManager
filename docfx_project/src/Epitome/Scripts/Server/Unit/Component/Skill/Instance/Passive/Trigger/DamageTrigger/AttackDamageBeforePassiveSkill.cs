using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{ 
    public class AttackDamageBeforePassiveSkill :DamageOtherBeforePassiveSkill
    {
        protected override void EffectTrigger(List<UnitBase> targets, OrganBase self, OrganBase other, Damage d)
        {
            //对伤害进行处理
            //以及添加buff
            foreach (var s in skillEffectEventDict)
            {
                s.Key?.Invoke(self.OwnerUnit, other.OwnerUnit,d, s.Value.Item1, s.Value.Item2);
            }
        }
        protected override bool TrrigerCondition(OrganBase self, OrganBase other, Damage d)
        {
            return base.TrrigerCondition(self, other, d)&&d.IsAttack;
        }
    }
}
