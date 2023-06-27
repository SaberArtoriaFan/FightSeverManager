using cfg.skillActive;
using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace XianXia.Unit
{
    public  class NormalImmediateActiveSkill : ActiveSkill
    {
        protected UnitMainSystem mainSystem;
        protected UnitBodySystem bodySystem;
        int findID;
        public override SpellTiggerType SpellTiggerType => SpellTiggerType.immediate;
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
        protected override void OnSpell()
        {
            base.OnSpell();
            List<UnitBase> targets = FindTarget(this.TargetType);
            foreach(var v in targets)
            {
                BodyOrgan mybody;
                if (v != null && (mybody = v.FindOrganInBody<BodyOrgan>(ComponentType.body)) != null)
                {
                    Damage damage = new Damage(ownerMagicOrgan.OwnerUnit, 0, false, false);
                    foreach (var s in skillEffectEventDict)
                    {
                        s.Key?.Invoke(ownerMagicOrgan.OwnerUnit, mybody.OwnerUnit,damage, s.Value.Item1, s.Value.Item2);
                    }
                    if (damage.Val > 0) bodySystem.ReceiveDamage(mybody, damage);

                }
            }

        }
        protected virtual List<UnitBase> FindTarget(TargetType targetType)
        {
            return SkillUtility.GetFindTargetsFunc(findID, targetType, ownerMagicOrgan.OwnerUnit, effectNum,SpellRange);
        }
    }
}
