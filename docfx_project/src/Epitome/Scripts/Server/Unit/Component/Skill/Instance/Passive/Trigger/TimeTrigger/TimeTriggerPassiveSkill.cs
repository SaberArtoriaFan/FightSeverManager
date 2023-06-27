using cfg.skillPassive;
using FishNet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public class TimeTriggerPassiveSkill : TriggerPassiveSkill
    {
        float skillTiming;
        protected override void EffectTrigger(List<UnitBase> targets, OrganBase self, OrganBase other, Damage d)
        {
            Debug.Log("发现目标数量"+targets.Count);

            foreach (var v in targets)
            {
                if (v != null)
                {
                    //Debug.Log("WWWW" + v.gameObject.name);
                    Damage damage = new Damage(self.OwnerUnit, 0, false, false);
                    foreach (var s in skillEffectEventDict)
                    {
                        Debug.Log("拥有Buff" + s.Key.Method.Name);
                        s.Key?.Invoke(self.OwnerUnit, v, damage, s.Value.Item1, s.Value.Item2);
                    }
                    if (damage.Val > 0)
                    {
                        BodyOrgan bodyOrgan = v.FindOrganInBody<BodyOrgan>(Saber.ECS.ComponentType.body);
                        bodySystem.ReceiveDamage(bodyOrgan, damage);
                    }
                }
            }
        }

        protected override List<UnitBase> NoVaildFindID(OrganBase self, OrganBase other, Damage d)
        {
            return new List<UnitBase>() { self.OwnerUnit };
        }
        public override void InitData(SkillPassive sp)
        {
            base.InitData(sp);
            this.skillTiming = sp.SkillTiming;
        }
        public override void AcquireSkill()
        {
            base.AcquireSkill();
            Action<TimingSystemUI> action=null;
            Timer timer = null;
            action = (t) =>
            {
                //可能游戏已经结束
                if(t==null||GameManager.NewInstance==null) timer.Stop();
                Debug.Log($"prev:{t._TimePast},,skillTiming{skillTiming}");
                if (t._TimePast >= skillTiming)
                {
                    Debug.Log("触发时间技能");
                    Trigger(ownerTalentOrgan, null, null);
                    timer.Stop();
                }
            };
            TimingSystemUI timingSystemUI = InstanceFinder.GetInstance<TimingSystemUI>();
            timer = TimerManager.Instance.AddTimer(() => action(timingSystemUI), 1, true);
        }

    }
}
