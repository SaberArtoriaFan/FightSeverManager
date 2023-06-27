using cfg.skillPassive;
using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{ 
    public abstract class AttackPassiveSkill : TriggerPassiveSkill
    {

        protected override bool TrrigerCondition(OrganBase self, OrganBase other,Damage damage)
        {
            //�ж��Ƿ��Լ�����
            if (self == null || self.OwnerUnit != ownerTalentOrgan.OwnerUnit || !damage.IsAttack) return false;
            //�ж��Ƿ�Ϸ����Ƿ��޵�
            if (other == null || !((BodyOrgan)other).UnitAlive || !other.Enable) return false;
            return true;
        }
    }
    //public abstract class AttackPassiveSkill : PassiveSkill
    //{
    //    protected int cd;
    //    protected int lifeTimeCount;
    //    protected bool isCded = true;

    //    protected EventSystem eventSystem;
    //    protected TimerManagerSystem timerManagerSystem;
    //    //protected SaberEvent<AttackOrgan, BodyOrgan, Damage> attackBeforeEvent;
    //    protected UnitBase self;

    //    protected Dictionary<SkillUtility.SkillEffectEvent, (float, float)> skillEffectEventDict;

    //    public override void Init(IContainerEntity owner)
    //    {
    //        base.Init(owner);
    //        eventSystem = SkillSystem.World.FindSystem<EventSystem>();
    //        timerManagerSystem = SkillSystem.World.FindSystem<TimerManagerSystem>();
    //       // attackBeforeEvent = eventSystem.GetEvent<AttackOrgan, BodyOrgan, Damage>(EventSystem.EventParameter.UnitAttackBefore);
    //        self = ownerTalentOrgan.OwnerUnit;
    //        skillEffectEventDict = new Dictionary<SkillUtility.SkillEffectEvent, (float, float)>();
    //    }

    //    public override void InitData(SkillPassive sp)
    //    {
    //        base.InitData(sp);
    //        cd = sp.SkillCd;
    //        lifeTimeCount = SkillUtility.CalculateLifeTime(sp.SkillCount);

    //        SkillUtility.SkillEffectEvent s;
    //        for (int i = 0; i < sp.SkillEffect.Length; i++)
    //        {
    //            if (i >= sp.EffectNumer1.Length || i >= sp.EffectNumer3.Length) break;
    //            s = SkillUtility.GetSkillAction(sp.SkillEffect[i]);
    //            if (s != null)
    //                skillEffectEventDict.Add(s, (sp.EffectNumer1[i], sp.EffectNumer3[i]));
    //        }
    //    }
    //    protected virtual void AttackEffectTrigger(AttackOrgan a, BodyOrgan b, Damage d)
    //    {
    //        foreach (var s in skillEffectEventDict)
    //            s.Key?.Invoke(a.OwnerUnit, b, d, s.Value.Item1, s.Value.Item2);
    //    }
    //    protected  void Trigger(AttackOrgan a, BodyOrgan b, Damage d)
    //    {
    //        //�ж��Ƿ�CD,�Լ��Ƿ񱻽�ֹʹ��
    //        if (!isCded || !Enable) return;
    //        //�ж��Ƿ��Լ�����
    //        if (a == null || a.OwnerUnit != self || !d.IsAttack) return;
    //        //�ж��Ƿ�Ϸ����Ƿ��޵�
    //        if (b == null || !b.UnitAlive || !b.Enable) return;
    //        //�������
    //        if (!TriggerCondition()) return;
    //        AttackEffectTrigger(a, b, d);
    //        TriggerAfterEvent(a,b,d);
    //        //�ж�ʹ�ô���
    //        if (lifeTimeCount > 0)
    //        {
    //            if (--lifeTimeCount == 0)
    //            {
    //                Enable = false;
    //                return;
    //            }
    //        }
    //        //�ж�CD
    //        if (cd > 0)
    //        {
    //            isCded = false;
    //            timerManagerSystem.AddTimer(() => isCded = true, cd);
    //        }

    //    }

    //    protected abstract bool TriggerCondition();
    //    protected abstract void TriggerAfterEvent(AttackOrgan a, BodyOrgan b, Damage d);
    //    public override void AcquireSkill()
    //    {
    //        base.AcquireSkill();
    //        //attackBeforeEvent.AddAction(Trigger);
    //        isCded = true;


    //    }
    //    public override void LostSkill()
    //    {
    //        base.LostSkill();
    //        //attackBeforeEvent.RemoveAction(Trigger);
    //    }
    //    public override void Destory()
    //    {
    //        base.Destory();
    //        eventSystem = null;
    //        timerManagerSystem = null;
    //        //attackBeforeEvent = null;
    //        self = null;
    //        skillEffectEventDict?.Clear();
    //        skillEffectEventDict = null;
    //    }
    //}
}
