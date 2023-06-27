using cfg.skillActive;
using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XianXia.Terrain;

namespace XianXia.Unit
{
    public class NormalUnitActiveSkill : ActiveSkill,ISelectTargetSkill
    {
        protected UnitMainSystem mainSystem;
        protected UnitBodySystem bodySystem;
        int findID;
        public override SpellTiggerType SpellTiggerType => SpellTiggerType.unit;
        protected Dictionary<SkillUtility.SkillEffectEvent, (float, float)> skillEffectEventDict = new Dictionary<SkillUtility.SkillEffectEvent, (float, float)>();

        public override void Init(IContainerEntity magicOrgan)
        {
            base.Init(magicOrgan);
            mainSystem = SkillSystem.World.FindSystem<UnitMainSystem>();
            bodySystem = SkillSystem.World.FindSystem<UnitBodySystem>();
        }
        public override void InitData(SkillActive skillActive)
        {
            base.InitData(skillActive);
            SkillUtility.SkillEffectEvent s;
            findID = skillActive.TargetDetermination;
            for (int i = 0; i < skillActive.SkillEffect.Length; i++)
            {
                if (i >= skillActive.EffectNumer1.Length || i >= skillActive.EffectNumer3.Length) break;
                s = SkillUtility.GetSkillAction(skillActive.SkillEffect[i]);
                if (s != null)
                    skillEffectEventDict.Add(s, (skillActive.EffectNumer1[i], skillActive.EffectNumer3[i]));
            }
        }

        Node ISelectTargetSkill.FindTarget()
        {
            List<UnitBase> res = SkillUtility.GetFindTargetsFunc(findID, TargetType, ownerMagicOrgan.OwnerUnit, 1, SpellRange);
            if (res.Count > 0) return mainSystem.GetGridItemByUnit(res[0]);
            else return null;
        }
        protected override void OnSpell()
        {
            base.OnSpell();
            UnitBase targetUnit = mainSystem.GetUnitByGridItem(Target);
            if (targetUnit == null)
            {
                Debug.LogError("似乎丢失了技能释放目标");
                return;
            }
            Damage damage = new Damage(ownerMagicOrgan.OwnerUnit, 0, false, false);
            foreach (var s in skillEffectEventDict)
            {
                s.Key?.Invoke(ownerMagicOrgan.OwnerUnit, targetUnit, damage, s.Value.Item1, s.Value.Item2);
            }
            if (damage.Val > 0) bodySystem.ReceiveDamage(targetUnit.FindOrganInBody<BodyOrgan>(ComponentType.body), damage);
        }
        
    }
}
