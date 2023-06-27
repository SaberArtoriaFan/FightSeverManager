using Saber.Base;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XianXia.Unit;
using Saber.ECS;
namespace XianXia
{

    public class GameWorld : BaseWorld
    {

        protected override void InitSystem()
        {
            base.InitSystem();
            //AddSystem<RisingSpaceUISystem>();
            AddSystem<ProjectileSystem>();
            AddSystem<MagicSkillSystem>();
            AddSystem<PassiveSkillSystem>();
            AddSystem<BuffSystem>();
            //初始化单位事件在这执行
            AddSystem<UnitMainSystem>();
            AddSystem<UnitStatusSystem>();
            AddSystem<UnitSpellSystem>();
            AddSystem<UnitTalentSystem>();
            AddSystem<UnitAttackSystem>();
            AddSystem<UnitMoveSystem>();
            AddSystem<UnitUIShowSystem>();
            //死亡事件在这里处理
            AddSystem<UnitBodySystem>();
        }

    }
}
