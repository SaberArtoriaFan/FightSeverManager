using cfg.skillPassive;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public class HemophagiaPassiveSkill : DamageOtherAfterPassiveSkill
    {
        // Start is called before the first frame update
        float per=0;
        public override void InitData(SkillPassive sp)
        {
            base.InitData(sp);
            int index = 0;
            for(int i=0;i<sp.SkillEffect.Length;i++)
            {
                if (sp.SkillEffect[i] == 7)
                    index = i;
            }
            if (sp.EffectNumer1.Length <= index) return;
            else per = sp.EffectNumer1[index]/100;
        }
        protected override void EffectTrigger(List<UnitBase> targets, OrganBase self, OrganBase other, Damage d)
        {
             BodyOrgan bodyOrgan= self.OwnerUnit.FindOrganInBody<BodyOrgan>(Saber.ECS.ComponentType.body);
            if (bodyOrgan == null&&per<=0) return;
            bodySystem.UnitHeal(bodyOrgan, (int)(d.Val * per), self.OwnerUnit);
        }

    }
}
