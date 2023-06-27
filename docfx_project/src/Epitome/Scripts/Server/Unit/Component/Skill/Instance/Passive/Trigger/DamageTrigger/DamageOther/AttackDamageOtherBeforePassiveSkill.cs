using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public class AttackDamageOtherBeforePassiveSkill : DamageOtherBeforePassiveSkill
    {
        protected override bool TrrigerCondition(OrganBase self, OrganBase other, Damage d)
        {
            return base.TrrigerCondition(self, other, d)&&d.IsAttack;
        }
    }
}
