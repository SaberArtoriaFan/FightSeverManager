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
            //�ж��Ƿ�CD,�Լ��Ƿ񱻽�ֹʹ��
            if (!isCd || !Enable) return;
            //�ж��Ƿ��Լ�����
            if (a == null || a.OwnerUnit !=ownerTalentOrgan.OwnerUnit || !d.IsAttack) return;
            //�ж��Ƿ�Ϸ����Ƿ��޵�
            if (b == null || !b.UnitAlive || !b.Enable) return;



            //�ж�ʹ�ô���
            if (lifeTimeCount > 0)
            {
                if (--lifeTimeCount == 0)
                {
                    Enable = false;
                    return;
                }
            }
            //�ж�CD
            if (cd > 0)
            {
                isCd = false;
                timerManagerSystem.AddTimer(() => isCd = true, cd);
            }
        }
    }
}
