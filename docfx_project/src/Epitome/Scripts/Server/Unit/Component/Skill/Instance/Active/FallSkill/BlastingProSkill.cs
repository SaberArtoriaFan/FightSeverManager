using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public class BlastingProSkill : BlastingSkill
    {
        public override SpellTiggerType SpellTiggerType => SpellTiggerType.immediate;
        protected override void OnSpell()
        {
            foreach (var v in SkillUtility.FindUnit_Random(UnitUtility .GetUnitBelongPlayer(ownerMagicOrgan.OwnerUnit), TargetType, effectNum,true))
            {
                CreateProjectToTarget(mainSystem.GetGridItemByUnit(v));
            }
        }
    }
}
