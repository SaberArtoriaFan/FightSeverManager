using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XianXia.Terrain;

namespace XianXia.Unit
{
    public class SummonHelpSkill : SummonSkill
    {
        public override void Init(IContainerEntity magicOrgan)
        {
            base.Init(magicOrgan);

        }
        protected override List<Node> GetSummonedPosition(int summonNum)
        {
            //对方老家
            Debug.Log("caoP" + UnitUtility.GetUnitBelongPlayerEnum(ownerMagicOrgan.OwnerUnit));
            bool isPlayer = UnitUtility.GetUnitBelongPlayerEnum(ownerMagicOrgan.OwnerUnit) == Saber.Camp.PlayerEnum.player;
            Node tar =  isPlayer? SkillUtility.PlayerStartNode : SkillUtility.MonsterStartNode;
            var res = AStarPathfinding2D.FindNearestNode(tar, 100, summonNum, (a, b) =>
            {
                if (mainSystem.GetUnitByGridItem(b) == null&&Mathf.Abs(b.Position.x-tar.Position.x)<=2) return true;
                else return false;
            }, SkillSystem.Map, true, true, true);
            if (res.Count < summonNum)
            {
                res = AStarPathfinding2D.FindNearestNode(tar, 100, summonNum, (a, b) =>
                {
                    if (mainSystem.GetUnitByGridItem(b) == null) return true;
                    else return false;
                }, SkillSystem.Map, true, true, true);
            }
            return res;
        }
    }
}
