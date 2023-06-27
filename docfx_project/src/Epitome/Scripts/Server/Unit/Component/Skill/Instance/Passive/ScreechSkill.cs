using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public class ScreechSkill : PassiveSkill
    {
        UnitMainSystem mainSystem;
        UnitBodySystem bodySystem;
        EventSystem eventSystem;
        SaberEvent<BodyOrgan, UnitBase> UnitDamagedAfter;

        float triggerPer = 0.5f;
        int damageVal=500;
        void Main(BodyOrgan bodyOrgan , UnitBase unit)
        {
            if (this.Enable==false||bodyOrgan == null ||bodyOrgan.UnitAlive==false|| bodyOrgan.OwnerUnit != ownerTalentOrgan.OwnerUnit) return;
            if (((float)bodyOrgan.Health_Curr / (float)bodyOrgan.Health_Max) <= 0.5f)
            {
                this.Enable = false;
                UnitBase[] enemys= mainSystem.GetUnitByCondition(bodyOrgan.OwnerPlayer,TargetType.enemy);
                foreach(var v in enemys)
                {
                    bodySystem.ReceiveDamage(v.FindOrganInBody<BodyOrgan>(ComponentType.body), new Damage(bodyOrgan.OwnerUnit, damageVal, false, true));
                }
                UnitDamagedAfter.RemoveAction(Main);


            }
            //eventSystem.RegisterEvent<BodyOrgan, UnitBase>("UnitDamagedBefore");
        }
        public override void Init(IContainerEntity owner)
        {
            base.Init(owner);
            mainSystem = SkillSystem.World.FindSystem<UnitMainSystem>();
            bodySystem = SkillSystem.World.FindSystem<UnitBodySystem>();
            eventSystem = SkillSystem.World.FindSystem<EventSystem>();
            UnitDamagedAfter = eventSystem.GetEvent<BodyOrgan, UnitBase>("UnitDamagedAfter");
        }
        public override void AcquireSkill()
        {
            base.AcquireSkill();
            UnitDamagedAfter.AddAction(Main);
        }
        public override void LostSkill()
        {
            base.LostSkill();
            //UnitDamagedAfter.AddAction(Main);

        }
    }
}
