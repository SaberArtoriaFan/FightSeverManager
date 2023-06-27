using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public abstract class DamageTriggerPassiveSkill : TriggerPassiveSkill
    {

        protected override void EffectTrigger(List<UnitBase> targets, OrganBase self, OrganBase other, Damage d)
        {
            Debug.Log("WWWW" + targets.Count);
            foreach (var v in targets)
            {
                if (v != null)
                {
                    Debug.Log("WWWW" + v.gameObject.name);
                    Damage damage = new Damage(self.OwnerUnit, 0, false, false);
                    foreach (var s in skillEffectEventDict)
                    {
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


        protected override bool TrrigerCondition(OrganBase self, OrganBase other,Damage d)
        {
            if (self == null) return false;
            if (self.OwnerUnit != ownerTalentOrgan.OwnerUnit) return false;
            return true;
        }
        /// <summary>
        /// 返回对你造成伤害的人
        /// </summary>
        /// <param name="b"></param>
        /// <param name="u"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        protected override List<UnitBase> NoVaildFindID(OrganBase self, OrganBase other, Damage d)
        {
            return new List<UnitBase>(1) { other.OwnerUnit };
        }

    }
}
