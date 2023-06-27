using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public class Rage_BossSkill : TimeTriggeredPassiveSkill
    {
        float damageMultiple=2;
        float attackSpeedMultiple = 4f;
        float moveSpeedMultiple = 1.5f;
        float addAttackSpeed = 0;
        float addMoveSpeed = 0;
        SaberEvent<BodyOrgan, Damage> UnitDamagedBefore;
        AttackOrgan attackOrgan;
        LegOrgan legOrgan;
        public override void AcquireSkill()
        {
            base.AcquireSkill();
            UnitDamagedBefore =  SkillSystem.World.FindSystem<EventSystem>().GetEvent<BodyOrgan, Damage>(EventSystem.EventParameter.UnitDamagedBefore); 

        }
        protected override void TriggerMain()
        {
            base.TriggerMain();
            Debug.Log(ownerTalentOrgan.OwnerUnit.gameObject.name + "¿ñ±©ÁË£¡");
            UnitDamagedBefore.AddAction(DamageMultiple);
            attackOrgan = ownerTalentOrgan.OwnerUnit.FindOrganInBody<AttackOrgan>(ComponentType.attack);
            legOrgan = ownerTalentOrgan.OwnerUnit.FindOrganInBody<LegOrgan>(ComponentType.leg);
            addAttackSpeed = (attackOrgan.Origin_AttackSpeed * (attackSpeedMultiple - 1));
            addMoveSpeed = (legOrgan.MoveSpeed * (moveSpeedMultiple - 1));
            attackOrgan.Ex_attackSpeed += addAttackSpeed;
            legOrgan.MoveSpeed += addMoveSpeed;
        }
        void DamageMultiple(BodyOrgan body, Damage damage)
        {
            if (body == null || damage == null || damage.Source == null) return;
            if (damage.Source == ownerTalentOrgan.OwnerUnit) damage.Val =(int)(damageMultiple*damage.Val);
        }
        public override void LostSkill()
        {
            base.LostSkill();
            UnitDamagedBefore.RemoveAction(DamageMultiple);
            attackOrgan.Ex_attackSpeed -= addAttackSpeed;
            legOrgan.MoveSpeed -= addMoveSpeed;
        }
    }
}
