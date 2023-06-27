using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public class AttackHemophagiaPassiveSkill : PassiveSkill
    {
        protected int cd;
        protected int lifeTimeCount;
        protected bool isCd = true;
        protected SaberEvent<AttackOrgan, BodyOrgan, Damage> attackAfterEvent;
        protected EventSystem eventSystem;
        protected TimerManagerSystem timerManagerSystem;
        protected Dictionary<SkillUtility.SkillEffectEvent, (float, float)> skillEffectEventDict;

        public override void Init(IContainerEntity owner)
        {
            base.Init(owner);
            eventSystem = SkillSystem.World.FindSystem<EventSystem>();
            timerManagerSystem = SkillSystem.World.FindSystem<TimerManagerSystem>();
            skillEffectEventDict = new Dictionary<SkillUtility.SkillEffectEvent, (float, float)>();
            attackAfterEvent = eventSystem.GetEvent<AttackOrgan, BodyOrgan, Damage>(EventSystem.EventParameter.UnitAttackAfter);
        }
        public override void AcquireSkill()
        {
            base.AcquireSkill();
        }
        public override void LostSkill()
        {
            base.LostSkill();
        }
        protected void Trigger(AttackOrgan a, BodyOrgan b, Damage d)
        {
            //判断是否CD,以及是否被禁止使用
            if (!isCd || !Enable) return;
            //判断是否自己触发
            if (a == null || a.OwnerUnit !=ownerTalentOrgan.OwnerUnit || !d.IsAttack) return;
            //判断是否合法，是否无敌
            if (b == null || !b.UnitAlive || !b.Enable) return;



            //判断使用次数
            if (lifeTimeCount > 0)
            {
                if (--lifeTimeCount == 0)
                {
                    Enable = false;
                    return;
                }
            }
            //判断CD
            if (cd > 0)
            {
                isCd = false;
                timerManagerSystem.AddTimer(() => isCd = true, cd);
            }
        }
    }
}
