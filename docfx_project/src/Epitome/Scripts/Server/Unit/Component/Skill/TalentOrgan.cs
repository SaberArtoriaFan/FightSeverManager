using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.ECS;

namespace XianXia.Unit
{
    public class TalentOrgan : StatusOrganBase<PassiveSkill>
    {
        protected override ComponentType componentType => ComponentType.talent;

        public string[] GetPassiveSkillsName()
        {
            List<string> res = new List<string>();
            foreach(var v in StatusList)
            {
                if (v != null)
                    res.Add(v.SkillId.ToString());
            }
            return res.ToArray();
        }

    }
}
