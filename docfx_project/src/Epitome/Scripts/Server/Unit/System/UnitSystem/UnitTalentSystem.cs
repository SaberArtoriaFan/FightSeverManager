using cfg.skillPassive;
using Saber.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Unit
{
    public class UnitTalentSystem : UnitSkillSystem<TalentOrgan, PassiveSkill>
    {
        protected override void GainSkillAfter(PassiveSkill s, TalentOrgan organ, object data = null)
        {
            base.GainSkillAfter(s, organ, data);
            s.InitData(data as SkillPassive);
        }
        protected override string GainSkillBefore(int id, out object data)
        {
            SkillPassive skillPassive = LubanMgr.GetSkillPassiveData(id);
            if (skillPassive == null)
            {
                data = null;
                return string.Empty;
            }
            data = skillPassive;
            return skillPassive.SkillResourcename;
        }
    }
}
