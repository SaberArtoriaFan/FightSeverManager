using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public class GrowUpSkill : ActiveSkill
    {
        float size = 0;
        float sizeMax = 0.5f;
        UIShowOrgan showOrgan;
        public override bool NeedSpellAction => false;
        //public override string RealName => "GrowUpSkill";
        public override SpellTiggerType SpellTiggerType { get => SpellTiggerType.immediate; internal set => base.SpellTiggerType = value; }
        public override void Init(IContainerEntity magicOrgan)
        {
            base.Init(magicOrgan);
            showOrgan = ownerMagicOrgan.OwnerUnit.FindOrganInBody<UIShowOrgan>(ComponentType.uIShow);
        }
        protected override void OnSpell()
        {
            base.OnSpell();
            if(ownerMagicOrgan!=null&&ownerMagicOrgan.OwnerUnit!=null)
            {
                if (sizeMax >size)
                {
                    size += 0.05f;
                    showOrgan.UnitScale =1+ size;

                }
                BodyOrgan bodyOrgan = ownerMagicOrgan.OwnerUnit.FindOrganInBody<BodyOrgan>(Saber.ECS.ComponentType.body);
                bodyOrgan.Health_Curr += 20;
                
            }
        }
    }
}
