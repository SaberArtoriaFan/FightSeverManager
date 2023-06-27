using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public class TestSkill : ActiveSkill
    {
        public override void AcquireSkill()
        {
            base.AcquireSkill();
            //Debug.Log(magicOrgan.OwnerUnit.gameObject.ToString() + "获得技能");

        }
        protected override void OnSpell()
        {
            base.OnSpell();
            Debug.Log(ownerMagicOrgan.OwnerUnit.gameObject.ToString()+ "成功释放技能");
        }
    }
}
