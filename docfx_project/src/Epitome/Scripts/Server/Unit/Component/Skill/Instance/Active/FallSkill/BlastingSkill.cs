using cfg.skillActive;
using FishNet;
using Saber.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using XianXia.Terrain;

namespace XianXia.Unit
{
    public class BlastingSkill : ActiveSkill
    {
   
        //EffectSystem effectSystem;
        protected UnitMainSystem mainSystem;
        protected ProjectileSystem projectileSystem;
        protected UnitBodySystem bodySystem;
        protected TimerManagerSystem timerManager;
        protected NormalUtility normalUtility;
        //public override float SpellRange => 100f;
        public override bool NeedSpellAction => true;
        public override SpellTiggerType SpellTiggerType => SpellTiggerType.unit;
        //public override TargetType TargetType => targetType;
        //流星偏移量
        Vector3 offest = new Vector3(-5, 5, 0);
        float speed = 2;
        float influenceRange = 1.5f;
        //TargetType targetType=TargetType.enemy;
        const string warnCircle = "MagicCircleSimpleYellow";
        const string projectEffect = "CFX2_DoubleFireBall A";
        const string explodeEffect = "CFXR2 Magic Explosion Spherical (Lit, Purple+Blue)";

        protected Dictionary<SkillUtility.SkillEffectEvent, (float, float)> skillEffectEventDict = new Dictionary<SkillUtility.SkillEffectEvent, (float, float)>();
        public override void InitData(SkillActive skillActive)
        {
            base.InitData(skillActive);
            SkillUtility.SkillEffectEvent s;
            for(int i=0;i<skillActive.SkillEffect.Length;i++)
            {
                if (i >= skillActive.EffectNumer1.Length || i >= skillActive.EffectNumer3.Length) break;
                s = SkillUtility.GetSkillAction(skillActive.SkillEffect[i]);
                if (s != null)
                    skillEffectEventDict.Add(s, (skillActive.EffectNumer1[i], skillActive.EffectNumer3[i]));
            }
        }
        public override void Init(IContainerEntity magicOrgan)
        {
            base.Init(magicOrgan);
            //effectSystem = SkillSystem.World.FindSystem<EffectSystem>();
            projectileSystem = SkillSystem.World.FindSystem<ProjectileSystem>();
            bodySystem = SkillSystem.World.FindSystem<UnitBodySystem>();
            mainSystem = SkillSystem.World.FindSystem<UnitMainSystem>();
            timerManager = SkillSystem.World.FindSystem<TimerManagerSystem>();
            normalUtility = InstanceFinder.GetInstance<NormalUtility>();

        }
        public override void AcquireSkill()
        {
            base.AcquireSkill();

        }
        string SetName(StringBuilder stringBuilder,DateTime now,string name)
        {
            stringBuilder.Clear();
            stringBuilder.Append(name);
            stringBuilder.Append(now.Hour.ToString());
            stringBuilder.Append(now.Minute.ToString());
            stringBuilder.Append(now.Second.ToString());
            return stringBuilder.ToString();

        }
        protected override void OnSpell()
        {
            base.OnSpell();
            CreateProjectToTarget(Target);
        }
        protected void CreateProjectToTarget(Node target)
        {
            Vector3 targetPos = AStarPathfinding2D.GetNodeWorldPositionV3(target.Position, SkillSystem.Map);

            //根据独一无二的Key创建，到时候也根据这个删除客户端的特效
            string cycleName = warnCircle + NormalUtility.GetId();
            normalUtility.ORPC_CreateEffect(warnCircle, cycleName, targetPos);
            ProjectileSystem.ProjectileHelper projectileHelper = null;
            projectileHelper = projectileSystem.CreateProjectile("", targetPos + offest, 1, null, AStarPathfinding2D.GetNodeWorldPositionV3(target.Position, SkillSystem.Map), speed, 0, null, () => {
                DamageNode(target);
                //根据Key回收
                normalUtility.ORPC_RecycleEffect(cycleName);
                //根据父物体回收
                normalUtility.ORPC_RecycleEffect(projectileHelper.owner.gameObject);
                //创建，但自动回收
                normalUtility.ORPC_CreateEffect(explodeEffect, null, targetPos, default, default, true);
            }, null, false);
            normalUtility.ORPC_CreateEffect(projectEffect, projectileHelper.owner.gameObject, projectileHelper.owner.gameObject.transform.position, false);
        }
        protected void DamageNode(Node node)
        {
            List<Node> targets = SkillUtility.GetNodesForConditionOfCenter(ownerMagicOrgan, node, TargetType, influenceRange);
                //AStarPathfinding2D.FindTargetsInRange(node, influenceRange, (a, b) => { return SystemUtility.TargetFiltration2(ownerMagicOrgan.OwnerUnit, mainSystem.GetUnitByGridItem(b), TargetType); }, SkillSystem.Map, true, false, true);
            try
            {
                foreach (var n in targets)
                {
                    UnitBase v = mainSystem.GetUnitByGridItem(n);
                    if (v != null)
                    {
                        Damage damage = new Damage(ownerMagicOrgan.OwnerUnit, 0, false, false);
                        foreach(var s in skillEffectEventDict)
                        {
                            s.Key?.Invoke(ownerMagicOrgan.OwnerUnit,v, damage, s.Value.Item1, s.Value.Item2);
                        }
                    }
                        //bodySystem.ReceiveDamage(mybody, new Unit.Damage(ownerMagicOrgan.OwnerUnit, damageVal, false, true));
                }
            }catch(Exception ex)
            {
                Debug.LogError(ex.Message);
            }
 
        }
    }
}
