using cfg.skillPassive;
using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XianXia.Unit
{
    public  class HPTriggerPassiveSkill : DamageAfterPassiveSkill
    {
        protected int criticalHPer;
        protected bool ishigh = false;
        protected bool isContinueOnCondition = false;

        public override void InitData(SkillPassive sp)
        {
            base.InitData(sp);
            criticalHPer = sp.SkillHp;
        }
        protected override bool TrrigerCondition(OrganBase self, OrganBase other, Damage d)
        {
            BodyOrgan body = (BodyOrgan)self;
            return base.TrrigerCondition(self, other, d)&& (float)body.Health_Curr / (float)body.Health_Max < (float)criticalHPer/100;
        }
    }
}
