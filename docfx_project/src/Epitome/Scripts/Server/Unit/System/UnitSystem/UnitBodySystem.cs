using Saber.Base;
using Saber.Camp;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.ECS;
using XianXia.Terrain;
using FSM;
using FishNet;

namespace XianXia.Unit
{
    public class UnitBodySystem : NormalSystemBase<BodyOrgan>
    {

        private bool showDamgeRisingSpace = true;
        bool isCorrectionPosition = true;



        UnitMainSystem mainSystem;
        EventSystem eventSystem;
        AStarPathfinding2D gridMap;
        //RisingSpaceUISystem risingSpaceSystem;

        SaberEvent<BodyOrgan, Damage> UnitDamagedBefore;
        SaberEvent<BodyOrgan, Damage> UnitDamagedAfter;
        SaberEvent<UnitBase> UnitDeadAfter;
        SaberEvent<BodyOrgan, UnitBase> UnitDeadBefore;
        int ID_DamageBefore;
        int ID_DamageAfter;
        int ID_DeadAfter;
        int ID_DeadBefore;

        Vector3 risingSpaceDir;
        Queue<(UnitBase,float)> waitForDead = new Queue<(UnitBase,float)>();


        public override void Awake(WorldBase world)
        {
            base.Awake(world);
            mainSystem = world.FindSystem<UnitMainSystem>();
            eventSystem= world.FindSystem<EventSystem>();
            UnitDeadBefore = eventSystem.RegisterEvent<BodyOrgan, UnitBase>(EventSystem.EventParameter.UnitDeadBefore,out ID_DamageBefore);
            UnitDeadAfter = eventSystem.RegisterEvent<UnitBase>(EventSystem.EventParameter.UnitDeadAfter,out ID_DamageAfter);
            UnitDamagedAfter = eventSystem.RegisterEvent<BodyOrgan, Damage>(EventSystem.EventParameter.UnitDamagedAfter,out ID_DamageAfter);
            UnitDamagedBefore = eventSystem.RegisterEvent<BodyOrgan, Damage>(EventSystem.EventParameter.UnitDamagedBefore,out ID_DamageBefore);
            //risingSpaceSystem=world.FindSystem<RisingSpaceUISystem>();
            risingSpaceDir = new Vector3(1, 1,0);
        }
        public override void Start()
        {
            base.Start();


            gridMap = Object.FindObjectOfType<AStarPathfinding2D>();
        }

        //public override void DestoryComponent(BodyOrgan t)
        //{

        //    t.Destory();
        //   // base.DestoryComponent(t);
        //}
        //public override BodyOrgan SpawnComponent(EntityBase entity)
        //{
        //    // return base.SpawnComponent(entity);
        //    var t = new BodyOrgan();
        //    t.Init(entity);
        //    return t;
        //}

        public void ReceiveDamage(BodyOrgan bodyOrgan,Damage damage)
        {
            //无敌判定
            if (bodyOrgan == null ||bodyOrgan.Enable==false|| damage == null||damage.Val<=0) return;
            //闪避加在这
            if (UnitUtility.CalculatePer(bodyOrgan.Evade))
            {
                InstanceFinder.GetInstance<NormalUtility>().ORPC_ShowRisingSpace("evade", UnitUtility.GetBodyPos(bodyOrgan.OwnerUnit), risingSpaceDir, Color.white, 28);
                return;
            }

            UnitDamagedBefore.Trigger(ID_DamageBefore,bodyOrgan, damage);
            int val;
            if (damage.IsAttack)
            {
                float defval = (float)bodyOrgan.Def / 100f + 1f;
                val = (int)((float)damage.Val / defval);
                Debug.Log($"初始伤害{damage.Val},削减倍数{defval},最终伤害{val}");
            }

            else
                val = damage.Val;

            bodyOrgan.Health_Curr -= val;

            if (bodyOrgan.OwnerUnit!=null)
            {
                if (damage.IsCriticalStrike)
                    InstanceFinder.GetInstance<NormalUtility>().ORPC_ShowRisingSpace(val.ToString(), bodyOrgan.OwnerUnit.transform.position + Vector3.up * 0.5f, risingSpaceDir, Color.red, 56, TMPro.FontStyles.Bold);
                else if (showDamgeRisingSpace)
                    InstanceFinder.GetInstance<NormalUtility>().ORPC_ShowRisingSpace(val.ToString(), bodyOrgan.OwnerUnit.transform.position + Vector3.up * 0.5f, risingSpaceDir);
                Debug.Log("body" + bodyOrgan + "受到了" + damage.Val + "伤害,来源于" + damage.Source);
                FightLog.Record($"玩家:{CampManager.GetPlayerEnum(bodyOrgan.OwnerPlayer)} 单位:{bodyOrgan.OwnerUnit.gameObject.name}受到了{damage.Val}点伤害，来源于 玩家{UnitUtility.GetUnitBelongPlayerEnum(damage.Source)} 单位:{damage.Source.gameObject.name}。剩余生命值:{bodyOrgan.Health_Curr}点");
            }

            UnitDamagedAfter.Trigger(ID_DamageAfter,bodyOrgan,damage);

            if (bodyOrgan.Health_Curr<=0)
                UnitDead(bodyOrgan, damage);
        }

        public void UnitHeal(BodyOrgan bodyOrgan,int num,UnitBase source)
        {
            if (bodyOrgan == null||bodyOrgan.OwnerUnit==null) return;
            bodyOrgan.Health_Curr += num;
            InstanceFinder.GetInstance<NormalUtility>().ORPC_ShowRisingSpace(num.ToString(), bodyOrgan.OwnerUnit.transform.position + Vector3.up * 0.5f, risingSpaceDir,Color.green);
            Debug.Log("body" + bodyOrgan.OwnerUnit +"受到了来自"+source+"的治疗："+num);
            FightLog.Record($"玩家:{CampManager.GetPlayerEnum(bodyOrgan.OwnerPlayer)} 单位:{bodyOrgan.OwnerUnit.gameObject.name}受到了{num}点治疗，来源于 玩家{UnitUtility.GetUnitBelongPlayerEnum(source)} 单位:{source.gameObject.name}。剩余生命值:{bodyOrgan.Health_Curr}点");

        }
        public void UnitDead(BodyOrgan bodyOrgan, Damage damage)
        {
            if (bodyOrgan == null||bodyOrgan.OwnerUnit==null) return;
            if (damage == null) damage = Damage.GodDamage;
            UnitDeadBefore.Trigger(ID_DeadBefore,bodyOrgan, damage.Source);
            //mainSystem.UnitBreakAction(bodyOrgan.OwnerUnit,  FSM_State.max,FSM_State.death,"Death");
            bodyOrgan.CharacterFSM.SetCurrentState(FSM_State.death);
            //bodyOrgan.OwnerUnit.GetComponent<Client_UnitProperty>().ORPC_ReDeath("Idle", 0);
            
            //InstanceFinder.GetInstance<NormalUtility>().ORPC_SetAnimatorParameter_Bool(bodyOrgan.OwnerUnit.GetComponent<Client_UnitProperty>(), FSM.AnimatorParameters.Death,true);
            Debug.Log("body" + bodyOrgan.OwnerUnit + "死亡，凶手是"+damage.Source);
            FightLog.Record($"玩家:{CampManager.GetPlayerEnum(bodyOrgan.OwnerPlayer)} 单位:{bodyOrgan.OwnerUnit.gameObject.name}死亡，凶手是 玩家{UnitUtility.GetUnitBelongPlayerEnum(damage.Source)} 单位:{damage?.Source?.gameObject?.name}。");
            var v = (bodyOrgan.OwnerUnit, bodyOrgan.DeadTime);
            Debug.Log("单位死亡时间是" + bodyOrgan.DeadTime);
            if (!waitForDead.Contains(v))
                waitForDead.Enqueue(v);
            //mainSystem.RemoveUnitFromGame(bodyOrgan.OwnerUnit,bodyOrgan.DeadTime);
            UnitDeadAfter.Trigger(ID_DeadAfter,damage.Source);
        }

        public void SetUnitPlayerColor(BodyOrgan bodyOrgan,PlayerMemeber playerMemeber)
        {
            if (bodyOrgan == null) return;
            UIShowOrgan uIShowOrgan = bodyOrgan.OwnerUnit.FindOrganInBody<UIShowOrgan>(ComponentType.uIShow);
            if (uIShowOrgan != null) InstanceFinder.GetInstance<NormalUtility>().Server_SetUnitColor(bodyOrgan.OwnerUnit.gameObject, playerMemeber.Color);
        }
        /// <summary>
        /// 返回两个格子之间的关系
        /// 如果不存在占领棋子则返回none
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="otherGrid"></param>
        /// <returns></returns>
        public CampRelation RelationOfTwoGrids(Node grid, Node otherGrid) => UnitUtility.RelationOfTwoGrids(mainSystem.GetUnitByGridItem(grid), mainSystem.GetUnitByGridItem(otherGrid));

        public override void Update()
        {
            base.Update();
            
            while (waitForDead.Count>0)
            {
                var v = waitForDead.Dequeue();
                mainSystem.RemoveUnitFromGame(v.Item1, v.Item2);
            }
        }
    }
}
