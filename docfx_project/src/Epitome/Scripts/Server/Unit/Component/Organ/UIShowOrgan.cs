using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public class UIShowOrgan : OrganBase
    {
        BodyOrgan bodyOrgan;
        MagicOrgan magicOrgan;
        //HealthMagicPointShowUI healthMagicPointShowUI;
        Client_UnitProperty unitProperty;

        float unitHight = 1.5f;
        float unitScale = 1f;
        Vector3 originScale;
        protected override ComponentType componentType => ComponentType.uIShow;

        public float HealthPer { get { if (bodyOrgan == null||bodyOrgan.Health_Max==-1) return -1; else return (float)bodyOrgan.Health_Curr / (float)bodyOrgan.Health_Max; } }
        public float MagicPer { get { if (magicOrgan == null||!magicOrgan.HasSkill||magicOrgan.MagicPoint_Max<=0) return -1; else return (float)magicOrgan.MagicPoint_Curr / (float)magicOrgan.MagicPoint_Max; } }

        //public HealthMagicPointShowUI HealthMagicPointShowUI { get => healthMagicPointShowUI;internal set => healthMagicPointShowUI = value; }
        public BodyOrgan BodyOrgan { get => bodyOrgan; }
        public MagicOrgan MagicOrgan { get => magicOrgan; }
        public float UnitHight { get => unitHight; set => unitHight = value; }
        public float UnitScale { get => unitScale; set 
            {
                unitScale = value;
                if (OwnerUnit == null) return;
                OwnerUnit.transform.localScale = originScale * unitScale;
            } }

        public Vector3 OriginScale { get => originScale; set => originScale = value; }
        public Client_UnitProperty UnitProperty { get => unitProperty; set => unitProperty = value; }

        //override 
        protected override void InitComponent(EntityBase unit)
        {
            base.InitComponent(unit);
            originScale = unit.transform.localScale;
            bodyOrgan = OwnerUnit.FindOrganInBody<BodyOrgan>(ComponentType.body);
            magicOrgan = OwnerUnit.FindOrganInBody<MagicOrgan>(ComponentType.magic);
            //Debug.Log(bodyOrgan.Health_Curr + "µÎµÎµÎ");

        }
        public override void Destory()
        {
            base.Destory();
            bodyOrgan=null;
            magicOrgan=null;
            //healthMagicPointShowUI;
            unitHight = 0;
            unitScale =0;
            //Vector3 originScale;
        }
    }
}
