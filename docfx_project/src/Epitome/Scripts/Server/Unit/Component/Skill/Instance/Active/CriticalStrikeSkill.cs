using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public class CriticalStrikeSkill : ActiveSkill
    {
        //下一次技能暴击，更换弹道
        public override bool NeedSpellAction => false;
        public override SpellTiggerType SpellTiggerType => SpellTiggerType.immediate;
        //RisingSpaceUISystem risingSpaceSystem;
        Projectile projectile;
        GameObject projectileModel;
        UnitAttackSystem attackSystem;
        AttackOrgan attackOrgan;
        Projectile originProjectile;
        GameObject originProjectileModel;
        float damageMultiple = 5f;
        //SaberEvent<>
        public override void AcquireSkill()
        {
            base.AcquireSkill();
            projectile = new Projectile("CycloneRed", 0, 0);
            attackSystem = SkillSystem.World.FindSystem<UnitAttackSystem>();
            projectileModel = attackSystem.GetProjectileModel_Main(projectile.ModelName);
            attackOrgan = ownerMagicOrgan.OwnerUnit.FindOrganInBody<AttackOrgan>(ComponentType.attack);
            //risingSpaceSystem=SkillSystem.World.FindSystem<RisingSpaceUISystem>();
        }

       protected override void OnSpell()
        {
            base.OnSpell();
            SkillSystem.ChangeActionToEvent<AttackOrgan, BodyOrgan,Damage>("UnitAttackBefore", ReplaceTar, true);
            SkillSystem.ChangeActionToEvent<AttackOrgan, BodyOrgan, Damage>("UnitAttackAfter", ReBackTar, true);
            //SkillSystem.ChangeActionToEvent<BodyOrgan, Damage>("UnitDamagedBefore", AddDamage, true);
            //Debug.Log("暴击！！");

        }
        protected void ReplaceTar(AttackOrgan attackOrgan, BodyOrgan enemyBody,Damage damage)
        {
            if (attackOrgan == null || enemyBody == null || attackOrgan != this.attackOrgan) return;
            originProjectile = attackOrgan.Projectile;
            projectile = new Projectile(projectile.ModelName, originProjectile.FlySpeed, originProjectile.Arc, originProjectile.Curve);
            //projectile.Curve = originProjectile.Curve;
            //projectile.Arc = originProjectile.Arc;
            //projectile.FlySpeed=originProjectile.FlySpeed;
            attackSystem.ChangeWeapon(attackOrgan, projectile);
            //Debug.Log("触发暴击！！");
            damage.Val += (int)(damage.Val * (damageMultiple - 1));
            damage.IsCriticalStrike = true;
            //attackOrgan.ExtraAttackVal += (int)(attackOrgan.OriginAttackVal * (damageMultiple-1));
            SkillSystem.ChangeActionToEvent<AttackOrgan, BodyOrgan, Damage>("UnitAttackBefore", ReplaceTar, false);
        }
        protected void ReBackTar(AttackOrgan attackOrgan, BodyOrgan enemyBody,Damage damage)
        {
            if (attackOrgan == null || enemyBody == null || attackOrgan != this.attackOrgan) return;
            attackSystem.ChangeWeapon(attackOrgan, originProjectile);
            SkillSystem.ChangeActionToEvent<AttackOrgan, BodyOrgan, Damage>("UnitAttackAfter", ReBackTar, false);
        }
        public override void LostSkill()
        {
            base.LostSkill();
            projectileModel = null;
            attackSystem = null;
            attackOrgan = null;
            originProjectile = null;
            originProjectileModel = null;
        }

        //protected void AddDamage(BodyOrgan bodyOrgan,Damage damage)
        //{
        //    if (bodyOrgan == null || damage == null|| damage.Source !=attackOrgan.OwnerUnit) return;
        //    damage.Val += (int)(damage.Val* DamageMultiple);
        //    SkillSystem.ChangeActionToEvent<BodyOrgan,Damage>("UnitDamagedBefore", AddDamage, false);
        //}

    }
}
