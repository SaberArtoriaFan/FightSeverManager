//using cfg.hero;
using FishNet;
using Saber.Camp;
using Saber.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XianXia.Terrain;

namespace XianXia.Unit
{
    public class UnitMainSystem : SingletonSystemBase<UnitMainManagerModel>
    {
        public event Action<PlayerMemeber> OnPlayerFailEvent;

        public void CorrectionPosition(UnitBase unit)
        {
            if (unit == null) return;
            //GridItem gridItem = GridMap.GetGridByWorldPosition(unit.transform.position, instance.GridMap);
            Node unitItem = GetGridItemByUnit(unit);
            if (unitItem == null) unitItem = AStarPathfinding2D.GetWorldPositionNode(unit.transform.position, instance.GridMap);
            if (unitItem == null) { Debug.LogError("出现在非法位置"); return; }
            //RemoveUnitFromPos(unit);
            //if (unitItem != gridItem)
            //{
            //    gridItem = GridUtility.FindNearestGridItem(gridItem, GridType.Ground);
            //    //if (gridItem == null) return;
            //}
            //AddUnitToPos(unit, gridItem);
            unit.gameObject.transform.position = AStarPathfinding2D.GetNodeWorldPositionV3(unitItem.Position, instance.GridMap);
        }

        /// <summary>
        /// 把格子信息之类的全部重置
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            instance.PosUnitDict.Clear();
            instance.UnitPosDict.Clear();
            foreach(var v in instance.UnitCoroutinesDict.Values)
            {
                if ( v.Item2 != null)
                {
                    world.StopCoroutine(v.Item2);
                }
            }
            instance.UnitCoroutinesDict.Clear();
            instance.PlayersUnitsDict.Clear();
            instance.UnitCreateQueue.Clear();
            instance.UnitCreateActionDict.Clear();
        }
        /// <summary>
        /// 移动时使用
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="gridItem"></param>
        public bool AddUnitToPos(UnitBase unit, Node gridItem)
        {
            if (unit == null || gridItem == null) return false;
            if (instance.PosUnitDict.ContainsKey(gridItem)) return false;
            if (instance.UnitPosDict.TryGetValue(unit, out var node) &&node==gridItem&& instance.PosUnitDict.TryGetValue(node, out UnitBase temp) && unit == temp) return false;
            RemoveUnitFromPos(unit);
            instance.UnitPosDict.Add(unit, gridItem);
            instance.PosUnitDict.Add(gridItem, unit);
            return true;
            ////更新动态障碍物表
            //AStarPathfinding2D.UpdateDynamicObstacle(gridItem.Position, instance.GridMap, true);
        }
        /// <summary>
        /// 更新动态表
        /// </summary>
        public void UpdateDynamicObstacle(Node gridItem,bool isAdd=true)
        {
            //if (gridItem == null) return;
            ////更新动态障碍物表
            //AStarPathfinding2D.UpdateDynamicObstacle(gridItem.Position, instance.GridMap,isAdd);
        }
        /// <summary>
        /// 更新动态表
        /// </summary>
        public void UpdateDynamicObstacleReal(Node gridItem, bool isAdd = true)
        {
            if (gridItem == null) return;
            //更新动态障碍物表
            AStarPathfinding2D.UpdateDynamicObstacle(gridItem.Position, instance.GridMap, isAdd);
        }
        /// <summary>
        /// 玩家离开地图时使用
        /// </summary>
        /// <param name="unit"></param>
        public void RemoveUnitFromPos(UnitBase unit)
        {
            if (unit == null) return;
            if (!instance.UnitPosDict.ContainsKey(unit)) return;
            
            Node gridItem = instance.UnitPosDict[unit];
            instance.UnitPosDict.Remove(unit);
            if (gridItem != null)
            {
                if (instance.PosUnitDict.ContainsKey(gridItem))
                    instance.PosUnitDict.Remove(gridItem);
                //AStarPathfinding2D.UpdateDynamicObstacle(gridItem.Position, instance.GridMap, false);
            }

        }

        private void RemoveUnitAction(UnitBase unit)
        {
            if (unit == null) return;
            if (instance.UnitCoroutinesDict.ContainsKey(unit))
            {
                if (instance.UnitCoroutinesDict[unit].Item2 != null)
                    World.StopCoroutine(instance.UnitCoroutinesDict[unit].Item2);
            }
        }
        public float GetDistanceOfTwoUnits(UnitBase a,UnitBase b)
        {
            return AStarPathfinding2D.GetDistance(GetGridItemByUnit(a), GetGridItemByUnit(b), instance.GridMap);
        }
        public Node GetGridItemByUnit(UnitBase unitBase)
        {
            Node res = null;
            if(unitBase == null) return null;
            if (instance.UnitPosDict.TryGetValue(unitBase, out res)) return res;
            else return null;
        }
        public Vector3 GetPosByUnit(UnitBase unitBase)
        {
            if(unitBase==null)return Vector3.forward*-1000;
            Node node=GetGridItemByUnit(unitBase);
            if(node==null) return Vector3.forward * -1000;
            return AStarPathfinding2D.GetNodeWorldPositionV3(node.Position, instance.GridMap);
        }
        public UnitBase GetUnitByGridItem(Node gridItem)
        {

            UnitBase res = null;
            if (gridItem == null || instance.PosUnitDict.TryGetValue(gridItem, out res)) return res;
            else return null;
        }

        //public  float CalculateTwoUnitDistance(UnitBase unit1,UnitBase unit2)
        //{
        //    return AStarPathfinding2D.GetDistance(GetGridItemByUnit(unit1),GetGridItemByUnit(unit2),instance.GridMap);
        //}

        /// <summary>
        /// 返回True,表明F1可以打断F2的行动，否则不行
        /// </summary>
        /// <param name="f1"></param>
        /// <param name="f2"></param>
        /// <returns></returns>
        public static bool ActionPriorityLevel(FSM_State f2)
        {
            if (f2 == FSM_State.idle || f2 == FSM_State.walk || f2 == FSM_State.run)
                return true;
            else
                return false;
        }
        public  bool IsUnitCanMove(UnitBase unit)
        {
            if (!instance.UnitCoroutinesDict.ContainsKey(unit)) return true;
            return ActionPriorityLevel(instance.UnitCoroutinesDict[unit].Item1);
        }


        public void UnitTakeAction(UnitBase unit,FSM_State fSM_State,IEnumerator coroutine,UnitBase target=null)
        {
            if (unit != null && coroutine != null)
            {


                if (!instance.UnitCoroutinesDict.ContainsKey(unit))
                {
                    LegOrgan leg = unit.FindOrganInBody<LegOrgan>(ComponentType.leg);
                    if (leg != null)
                    {
                        instance.MoveSystem.SetUnitLeg(leg, target);
                    }
                    instance.UnitCoroutinesDict.Add(unit, (fSM_State, unit.StartCoroutine(coroutine)));

                }
                else
                {
                    if (ActionPriorityLevel(instance.UnitCoroutinesDict[unit].Item1))
                    {
                        UnitStopAction(unit);
                        UnitTakeAction(unit, fSM_State, coroutine);
                    }
                }

            }

        }
        //public enum BreakActionType
        //{
        //    none,
        //    attack,
        //    spell,
        //    move,
        //    dead,
        //    all
        //}
        internal void UnitBreakAction(UnitBase unit,FSM_State breakActionType)
        {
            if (unit == null) return;
            if (!instance.UnitCoroutinesDict.ContainsKey(unit))
            {
                if(breakActionType == FSM_State.max||( breakActionType ==FSM_State.walk&&unit.GetComponent<CharacterFSM>().CurrentState==FSM_State.walk))
                {
                    unit.GetComponent<CharacterFSM>().SetCurrentState(FSM_State.idle);
                    Client_UnitProperty client_UnitProperty = unit.GetComponent<Client_UnitProperty>();
                    //client_UnitProperty.SetAnimatorParameter_Bool(FSM.AnimatorParameters.Walk, false);
                    client_UnitProperty.ORPC_ReIdle("Idle", 0);
                }
            }
            else
            {

                if (breakActionType == FSM_State.max || breakActionType == instance.UnitCoroutinesDict[unit].Item1)
                {
                    if (instance.UnitCoroutinesDict[unit].Item2 != null)
                        unit.StopCoroutine(instance.UnitCoroutinesDict[unit].Item2);
                    instance.UnitCoroutinesDict.Remove(unit);
                    unit.GetComponent<CharacterFSM>().SetCurrentState(FSM_State.idle);
                    unit.GetComponent<Client_UnitProperty>().ORPC_ReIdle("Idle", 0);
                    //InstanceFinder.GetInstance<NormalUtility>().ORPC_SetAnimatorParameter_Trigger(unit.GetComponent<Client_UnitProperty>(), FSM.AnimatorParameters.Reset);


                    //LegOrgan leg = unit.FindOrganInBody<LegOrgan>(ComponentType.leg);
                    //if (leg != null)
                    //{
                    //    leg.CanFindPath = true;
                    //}

                }
            }

        }
        internal void UnitBreakAction(UnitBase unit, FSM_State breakActionType,FSM_State toFsmState,string name)
        {
            if (unit == null) return;

            if (instance.UnitCoroutinesDict.ContainsKey(unit))
            {
                if (instance.UnitCoroutinesDict[unit].Item2 != null)
                    unit.StopCoroutine(instance.UnitCoroutinesDict[unit].Item2);
                instance.UnitCoroutinesDict.Remove(unit);
            }

            if (breakActionType == FSM_State.max || breakActionType == instance.UnitCoroutinesDict[unit].Item1)
            {
                unit.GetComponent<Client_UnitProperty>().ORPC_ReIdle(name, 0);
                unit.GetComponent<CharacterFSM>().SetCurrentState(toFsmState);

            }


        }
        internal void UnitStopAction(UnitBase unit)
        {
            if(unit!=null&& instance.UnitCoroutinesDict.ContainsKey(unit))
            {
                if(instance.UnitCoroutinesDict[unit].Item2!=null)
                    unit.StopCoroutine(instance.UnitCoroutinesDict[unit].Item2);
                instance.UnitCoroutinesDict.Remove(unit);

                LegOrgan leg = unit.FindOrganInBody<LegOrgan>(ComponentType.leg);
                if (leg != null)
                {
                    leg.CanFindPath = true;
                }
            }

        }
        //public FSM_State GetUnitAction(UnitBase unitBase)
        //{
        //    coroutine = null;
        //    if (unitBase == null) return FSM_State.max;
        //    if (!instance.UnitCoroutinesDict.ContainsKey(unitBase)) return FSM_State.idle;
        //    coroutine = instance.UnitCoroutinesDict[unitBase].Item2;
        //    return instance.UnitCoroutinesDict[unitBase].Item1;
        //}
        public FSM_State GetUnitAction(UnitBase unitBase)
        {
            if (unitBase == null) return FSM_State.max;
            if (!instance.UnitCoroutinesDict.ContainsKey(unitBase)) return FSM_State.idle;
            //if (!instance.UnitTimerHash.Contains(unitBase))
            //{
            //    CharacterFSM characterFSM = unitBase.GetComponent<CharacterFSM>();
            //    if (characterFSM != null && characterFSM.CurrentState == FSM_State.idle)
            //    {
            //        Coroutine coroutine = instance.UnitCoroutinesDict[unitBase].Item2;
            //        instance.UnitTimerHash.Add(unitBase);
            //        instance.TimerManagerSystem.AddTimer(() =>
            //        {
            //            if (unitBase == null) return;
            //            if (instance.UnitTimerHash.Contains(unitBase)) instance.UnitTimerHash.Remove(unitBase);
            //            if (instance.UnitCoroutinesDict[unitBase].Item2 == coroutine && characterFSM.CurrentState == FSM_State.idle)
            //            {
            //                UnitStopAction(unitBase);
            //            }
            //        }, 1f);
            //    }
            //}

            return instance.UnitCoroutinesDict[unitBase].Item1;
        }
        internal void RemoveUnitFromGame(UnitBase unitBase,float delay)
        {
            if (unitBase == null) return;
            GameObject go = unitBase.gameObject;
            //if (delay <= Time.deltaTime) delay = Time.deltaTime;

            Debug.Log("延迟死亡" + delay);
            RemoveUnitAction(unitBase);
            PlayerMemeber playerMemeber = unitBase.FindOrganInBody<BodyOrgan>(ComponentType.body).OwnerPlayer;
            instance.PlayersUnitsDict[playerMemeber].Remove(unitBase);

            if (instance.PlayersUnitsDict[playerMemeber].Count == 0)
                OnPlayerFailEvent?.Invoke(playerMemeber);

            UpdateDynamicObstacleReal(GetGridItemByUnit(unitBase), false);
            RemoveUnitFromPos(unitBase);

            BodyOrgan bodyOrgan = unitBase.FindOrganInBody<BodyOrgan>(ComponentType.body);



            unitBase.DestroyOrgan<AttackOrgan>(unitBase.FindOrganInBody<AttackOrgan>(ComponentType.attack));
            unitBase.DestroyOrgan<MagicOrgan>(unitBase.FindOrganInBody<MagicOrgan>(ComponentType.magic));
            unitBase.DestroyOrgan(unitBase.FindOrganInBody<TalentOrgan>(ComponentType.talent));
            unitBase.DestroyOrgan(unitBase.FindOrganInBody<LegOrgan>(ComponentType.leg));
            unitBase.DestroyOrgan(unitBase.FindOrganInBody<StatusOrgan>(ComponentType.statusHeart));
            unitBase.DestroyOrgan(unitBase.FindOrganInBody<UIShowOrgan>(ComponentType.uIShow));
            unitBase.DestroyOrgan(unitBase.FindOrganInBody<BodyOrgan>(ComponentType.body));

            //unitBase.DestoryAllOrgans();
            unitBase.enabled = false;
            InstanceFinder.GetInstance<NormalUtility>().Server_UnitDead(go);
#if UNITY_EDITOR
            if (go.GetComponent<TestUnitModel>() != null)
                GameObject.Destroy(go.GetComponent<TestUnitModel>());
#endif
            GameObject.Destroy(unitBase.GetComponent<CharacterFSM>());
            GameObject.Destroy(unitBase);
            if (delay > 0)
                instance.TimerManagerSystem.AddTimer(() => {
                    Debug.Log("准备销毁" + go.name);
                    if (go != null)
                    {
                        //Debug.Log("准备销毁" + go.name);
                        FightLog.Record($"玩家:{CampManager.GetPlayerEnum(playerMemeber)} 单位:{go.name},被移除出游戏");
                        InstanceFinder.GetInstance<NormalUtility>().Server_DespawnAndRecycleModel(go, go.name);
                    }
                    /*go.SetActive(false);*/
                }, delay);
            else if (go != null)
            {
                //Debug.Log("准备销毁" + go.name);
                FightLog.Record($"玩家:{CampManager.GetPlayerEnum(playerMemeber)} 单位:{go.name},被移除出游戏");
                InstanceFinder.GetInstance<NormalUtility>().Server_DespawnAndRecycleModel(go, go.name);
            }


            Debug.Log("准备销毁" + go.name+"111");

            //解除所以对这些器官的引用

            //这里写判断游戏是否已经结束的事件
        }
        public bool TargetFiltration(Node nodeA, Node nodeB, TargetType targetType)
        {
            // bool res = SystemUtility.TargetFiltration(GetUnitByGridItem(nodeA), GetUnitByGridItem(nodeB), targetType);
            //Debug.Log((nodeA!=null).ToString()+"ss"+(nodeB!=null).ToString() + "SSS");
            if (nodeA == null || nodeB == null) return false;
            return UnitUtility.TargetFiltration(GetUnitByGridItem(nodeA), GetUnitByGridItem(nodeB), targetType);
        }
        public UnitMainSystem() : base() 
        { 
        }

        public override void Start()
        {
            base.Start();
            instance.MoveSystem = world.FindSystem<UnitMoveSystem>();
            instance.AttackSystem = world.FindSystem<UnitAttackSystem>();
            instance.TimerManagerSystem=world.FindSystem<TimerManagerSystem>();
            //world.StartCoroutine(IE_Main());
        }
        //public  UnitBase SwapnUnit(Transform tr,Vector3 pos,Projectile projectile,float attackRange,float warningRange,PlayerMemeber player)
        //{
        //    Node node = AStarPathfinding2D.GetWorldPositionNode(pos, instance.GridMap);
        //    //Debug.Log(node + "1a1");

        //    node = AStarPathfinding2D.FindNearestNode(AStarPathfinding2D.GetNode(node.Position, instance.GridMap), 10, (u, c) => { return true; }, instance.GridMap, true, true);
        //    CharacterFSM characterFSM=tr.gameObject.AddComponent<CharacterFSM>();
        //    UnitBase unit=tr.gameObject.AddComponent<UnitBase>();
        //    world.FindSystem<UnitBodySystem>().SetUnitPlayer(unit.AddOrgan<BodyOrgan>(ComponentType.body), player);
        //    AttackOrgan attackOrgan= unit.AddOrgan<AttackOrgan>(ComponentType.attack);
        //    attackOrgan.AttackRange = attackRange;
        //    attackOrgan.WarningRange=warningRange;
        //    instance.AttackSystem.ChangeWeapon(attackOrgan, projectile);
                
        //    unit.AddOrgan<LegOrgan>(ComponentType.leg);
        //    unit.AddOrgan<UIShowOrgan>(ComponentType.uIShow);
        //    AddUnitToPos(unit,node);
        //    return unit;
        //}
        ///// <summary>
        ///// 临时使用
        ///// </summary>
        ///// <param name="testUnitModel"></param>
        ///// <param name="player"></param>
        ///// <returns></returns>
        //public UnitBase InitUnit(TestUnitModel testUnitModel, PlayerMemeber player)
        //{
        //    return Sp
        //}
        //public UnitBase InitUnit(string unitName, PlayerMemeber player)
        //{
        //    //根据名字查表得到数据
        //    return null;
        //}
        public UnitBase[] GetUnitByCondition(PlayerMemeber player,TargetType targetType)
        {
            CampRelation campRelation = CampRelation.none ;
            switch (targetType)
            {
                case TargetType.enemy:
                    campRelation = CampRelation.hostile;
                    break;
                case TargetType.friend:
                case TargetType.selfUnit:
                    campRelation = CampRelation.friendly;
                    break;
            }
            IEnumerable<UnitBase> unitBases = new List<UnitBase>();

            foreach (var v in instance.PlayersUnitsDict)
            {
                if (UnitUtility.RelationOfTwoGrids(player, v.Key) == campRelation)
                {
                    unitBases = unitBases.Concat(v.Value);

                    //foreach (var u in v.Value)
                    //{
                    //    if (u != null && unitBases.Contains(u) == false)
                    //        unitBases.Add(u);
                    //}
                }
            }

            return unitBases.ToArray();

        }
        public int GetUnitCountByCondition(PlayerMemeber player, TargetType targetType)
        {
            CampRelation campRelation = CampRelation.none;
            switch (targetType)
            {
                case TargetType.enemy:
                    campRelation = CampRelation.hostile;
                    break;
                case TargetType.friend:
                case TargetType.selfUnit:
                    campRelation = CampRelation.friendly;
                    break;
            }
            IEnumerable<UnitBase> unitBases = new List<UnitBase>();

            foreach (var v in instance.PlayersUnitsDict)
            {
                if (UnitUtility.RelationOfTwoGrids(player, v.Key) == campRelation)
                {
                    unitBases = unitBases.Concat(v.Value);

                    //foreach (var u in v.Value)
                    //{
                    //    if (u != null && unitBases.Contains(u) == false)
                    //        unitBases.Add(u);
                    //}
                }
            }

            return unitBases.Count();

        }
        //public UnitBase[] GetAllEnemy(PlayerMemeber player)
        //{
        //    IEnumerable<UnitBase> unitBases = new List<UnitBase>();
        //    foreach(var v in instance.PlayersUnitsDict)
        //    {
        //        if (SystemUtility.RelationOfTwoGrids(player, v.Key) == CampRelation.hostile)
        //        {
        //            unitBases = unitBases.Concat(v.Value);

        //            //foreach (var u in v.Value)
        //            //{
        //            //    if(u!=null&&unitBases.Contains(u)==false)
        //            //    unitBases.Add(u);
        //            //}
        //        }
        //    }
        //    return unitBases.ToArray();
        //}
        public int GetNumberUnitOfMine(PlayerMemeber player)
        {
            if (instance.PlayersUnitsDict.ContainsKey(player))
                return instance.PlayersUnitsDict[player].Count;
            else
                return 0;
        }
        public UnitBase[] GetAllOfMine(PlayerMemeber player)
        {
            if (instance.PlayersUnitsDict.ContainsKey(player))
                return instance.PlayersUnitsDict[player].ToArray();
            else
                return null;
        }
        public UnitBase[] GetAllUnits()
        {
            IEnumerable<UnitBase> unitBases = new List<UnitBase>();
            foreach (var v in instance.PlayersUnitsDict)
            {
                unitBases.Concat(v.Value);
                    //foreach (var u in v.Value)
                    //{
                    //    if (u != null && unitBases.Contains(u) == false)
                    //        unitBases.Add(u);
                    //}
                }           
        
            return unitBases.ToArray();
        }
        public int GetAllSpellingUnits()
        {
            int res = 0;
            foreach(var v in instance.UnitCoroutinesDict)
            {
                if (v.Key!=null&&(v.Value.Item1 == FSM_State.attack || v.Value.Item1 == FSM_State.spell))
                    res++;
            }
            return res;
        }
        //public static PlayerEnum GetUnitBelongPlayer(UnitBase unit)
        //{
        //    if (unit == null) return default;
        //    return  CampManager.GetPlayerEnum(unit.FindOrganInBody<BodyOrgan>(ComponentType.body)?.OwnerPlayer);
        //} 
        //public UnitBase SwapnUnit(int heroID, Vector3 pos, PlayerEnum playerEnum)
        //{
        //    Hero hero = LubanMgr.GetHeroData(heroID);

        //    if (hero == null) return null;
        //    PlayerMemeber player = CampManager.GetPlayerMemeber(playerEnum);
        //    Node node = AStarPathfinding2D.GetWorldPositionNode(pos, instance.GridMap);
        //    //加载模型
        //    GameObject heroModel = ABUtility.Load<GameObject>(ABUtility.UnitMainName + hero.HeroPrefab);

        //    //node = AStarPathfinding2D.FindNearestNode(AStarPathfinding2D.GetNode(node.Position, instance.GridMap), 10, (u, c) => { return true; }, instance.GridMap, true, true);
        //    CharacterFSM characterFSM = heroModel.gameObject.AddComponent<CharacterFSM>();
        //    UnitBase unit = heroModel.gameObject.AddComponent<UnitBase>();
        //    BodyOrgan bodyOrgan = unit.AddOrgan<BodyOrgan>(ComponentType.body);
        //    AttackOrgan attackOrgan = unit.AddOrgan<AttackOrgan>(ComponentType.attack);
        //    //技能要放在比较后面
        //    if (testUnitModel.activeSkill != "")
        //    {
        //        MagicOrgan magicOrgan = unit.AddOrgan<MagicOrgan>(ComponentType.magic);
        //        world.FindSystem<UnitSpellSystem>().GainSkill(magicOrgan, testUnitModel.activeSkill);
        //    }
        //    else if (testUnitModel.passiveSkills != null && testUnitModel.passiveSkills.Length > 0)
        //    {
        //        TalentOrgan talentOrgan = unit.AddOrgan<TalentOrgan>(ComponentType.talent);
        //        foreach (var v in testUnitModel.passiveSkills)
        //        {
        //            world.FindSystem<UnitTalentSystem>().GainSkill(talentOrgan, v);

        //        }
        //    }

        //    //MagicSkillSystem
        //    unit.transform.position = AStarPathfinding2D.GetNodeWorldPositionV3(node.Position, instance.GridMap);
        //    UIShowOrgan uIShowOrgan = unit.AddOrgan<UIShowOrgan>(ComponentType.uIShow);
        //    uIShowOrgan.UnitHight = testUnitModel.hight;
        //    //world.FindSystem<UnitUIShowSystem>().InitShow(uIShowOrgan);

        //    bodyOrgan.Health_Max = testUnitModel.health_Max;
        //    bodyOrgan.Health_Curr = testUnitModel.health_Curr;
        //    bodyOrgan.Def = testUnitModel.def;

        //    world.FindSystem<UnitBodySystem>().SetUnitPlayer(bodyOrgan, player);
        //    attackOrgan.AttackRange = testUnitModel.attackRange;
        //    attackOrgan.WarningRange = testUnitModel.warningRange;
        //    attackOrgan.AttackAnimationLong = testUnitModel.attackAnimationLong;
        //    attackOrgan.OriginAttackVal = testUnitModel.attackVal;
        //    attackOrgan.AttackTime = testUnitModel.attackTime;

        //    instance.AttackSystem.ChangeWeapon(attackOrgan, testUnitModel.projectile);
        //    instance.AttackSystem.ChangeFsm(attackOrgan);
        //    unit.AddOrgan<LegOrgan>(ComponentType.leg);//.MoveSpeed=testUnitModel.moveSpeed;
        //    AddUnitToPos(unit, node);
        //    UpdateDynamicObstacle(node);

        //    if (instance.UnitCreateActionDict.ContainsKey(unit.gameObject))
        //    {
        //        instance.UnitCreateActionDict[unit.gameObject]?.Invoke(unit);
        //        instance.UnitCreateActionDict.Remove(unit.gameObject);
        //    }
        //    if (!instance.PlayersUnitsDict.ContainsKey(player))
        //        instance.PlayersUnitsDict.Add(player, new List<UnitBase>());
        //    instance.PlayersUnitsDict[player].Add(unit);

        //    return unit;
        //}
        public void SwapnUnitInPos(UnitModel unitModel, GameObject go,PlayerEnum playerEnum,Vector2Int pos, Action<UnitBase> finishAction = null)
        {
            InstanceFinder.GetInstance<NormalUtility>().ORPC_StrongSyncPosition(go, AStarPathfinding2D.GetNodeWorldPositionV3(pos, instance.GridMap));
            //go.transform.position= AStarPathfinding2D.GetNodeWorldPositionV3(pos, instance.GridMap);
            SwapnUnit(unitModel, go, playerEnum,finishAction);
        }
       private void SwapnUnit(UnitModel unitModel,GameObject go,PlayerEnum playerEnum ,Action<UnitBase> finishAction = null)
        {
            if (!instance.UnitCreateQueue.Contains((go,playerEnum,unitModel)))
            {
                instance.UnitCreateQueue.Enqueue((go, playerEnum,unitModel));
                if (finishAction != null)
                    instance.UnitCreateActionDict.Add(go, finishAction);

            }
        }
        public void Test_SwapnUnit(TestUnitModel testUnitModel, Action<UnitBase> finishAction = null)
        {
            //if (!instance.UnitCreateQueue.Contains(testUnitModel.gameObject))
            //{
            //    instance.UnitCreateQueue.Enqueue(testUnitModel.gameObject);
            //    if (finishAction != null)
            //        instance.UnitCreateActionDict.Add(testUnitModel.gameObject, finishAction);

            //}
        }
        /// <summary>
        /// 有些属性可能是由客户端来初始化的
        /// </summary>
        /// <param name="go"></param>
        private void ClientSpawnUnit(GameObject go)
        {
            InstanceFinder.GetInstance<NormalUtility>().ORPC_SpawnUnit(go);
        }
        private UnitBase SwapnUnitReal(UnitModel testUnitModel,GameObject model,PlayerEnum playerEnum)
        {
            if (testUnitModel == null) return null;


            ClientSpawnUnit(model);

            PlayerMemeber player = CampManager.GetPlayerMemeber(playerEnum);
            Node node = AStarPathfinding2D.GetWorldPositionNode(model.transform.position, instance.GridMap);
            //Debug.Log(node + "1a1");

            //node = AStarPathfinding2D.FindNearestNode(AStarPathfinding2D.GetNode(node.Position, instance.GridMap), 10, (u, c) => { return true; }, instance.GridMap, true, true);
            //if(testUnitModel.gameObject.GetComponent)
            CharacterFSM characterFSM = model.GetComponent<CharacterFSM>();
            if (characterFSM != null) characterFSM.Init();
            else characterFSM = model.AddComponent<CharacterFSM>();
            NormalUtility normalUtility = InstanceFinder.GetInstance<NormalUtility>();
            //Client_UnitProperty client_UnitProperty = model.GetComponent<Client_UnitProperty>();
            //characterFSM.SetSendXAction((f) => normalUtility.ORPC_SetFlip(client_UnitProperty,f));
            characterFSM.SetSendXAction((f) => normalUtility.ORPC_SetRotateY(model, f));
            UnitBase unit =model.AddComponent<UnitBase>();
            BodyOrgan bodyOrgan =unit.AddOrgan<BodyOrgan>(ComponentType.body);
            IClothes clothes= unit.GetComponentInChildren<IClothes>();
            if (clothes != null)
                bodyOrgan?.SetClothSpineID(clothes.Init(testUnitModel.ModelName));
            else if(InstanceFinder.GetInstance<XianXia.Spine.SpineAnimationDict>().heroAnimationDict.TryGetValue(testUnitModel.ModelName, out int id))//是被复用的模型
                bodyOrgan?.SetClothSpineID(id);
            AttackOrgan attackOrgan = unit.AddOrgan<AttackOrgan>(ComponentType.attack);
            TalentOrgan talentOrgan = unit.AddOrgan<TalentOrgan>(ComponentType.talent);
            MagicOrgan magicOrgan = unit.AddOrgan<MagicOrgan>(ComponentType.magic);
            StatusOrgan statusOrgan = unit.AddOrgan<StatusOrgan>(ComponentType.statusHeart);
            bodyOrgan.OwnerPlayer = player;


            bodyOrgan.Health_Max = testUnitModel.Health_Max;
            bodyOrgan.Health_Curr = testUnitModel.Health_Curr;
            bodyOrgan.Def = testUnitModel.Def;
            bodyOrgan.Evade = testUnitModel.Evade;
            bodyOrgan.ModelName = testUnitModel.ModelName;

            attackOrgan.Origin_AttackRange = testUnitModel.AttackRange;
            attackOrgan.WarningRange = testUnitModel.WarningRange;
            attackOrgan.OriginAttackVal = testUnitModel.AttackVal;
            //attackOrgan.AttackTime = testUnitModel.AttackTime;
            attackOrgan.Origin_AttackSpeed = testUnitModel.AttackSpeed;
            attackOrgan.Or_AttackHitrate = testUnitModel.AttackHitrate;
            attackOrgan.Or_AttackCriticalDamage = testUnitModel.AttackCriticaldamage;
            attackOrgan.Or_AttackCriticalChance = testUnitModel.AttackCriticalchance;

            SkillUtility.SetSkillMagicPoint(testUnitModel.Mp_Max, 0, testUnitModel.Attack_mp, testUnitModel.Attacked_mp, magicOrgan);


            instance.AttackSystem.ChangeWeapon(attackOrgan,testUnitModel.Projectile);
            instance.AttackSystem.ChangeFsm(attackOrgan);
            magicOrgan.SetAnimationFinishTime(bodyOrgan.ClothesSpineID);
            unit.AddOrgan<LegOrgan>(ComponentType.leg).MoveSpeed = testUnitModel.MoveSpeed;//.MoveSpeed=testUnitModel.moveSpeed;

            //设置完基础属性再设置技能
            //技能要放在比较后面
            if (testUnitModel.ActiveSkill!=0)
            {
                //magicOrgan.AnimationLong = testUnitModel.spellTime;
                world.FindSystem<UnitSpellSystem>().GainSkill(magicOrgan, testUnitModel.ActiveSkill);
                Debug.Log(magicOrgan.OwnerUnit + "拥有" + testUnitModel.ActiveSkill);

            }
            if (testUnitModel.PassiveSkills != null && testUnitModel.PassiveSkills.Length > 0)
            {
                foreach (var v in testUnitModel.PassiveSkills)
                {
                    if (v!=0)
                        world.FindSystem<UnitTalentSystem>().GainSkill(talentOrgan, v);
                }
            }
            //MagicSkillSystem
            Vector3 pos = AStarPathfinding2D.GetNodeWorldPositionV3(node.Position, instance.GridMap);

            unit.transform.position = pos;
            //设置完技能再设置SHOWORGAN
            UIShowOrgan uIShowOrgan = unit.AddOrgan<UIShowOrgan>(ComponentType.uIShow);
            uIShowOrgan.UnitHight=testUnitModel.Hight;
            world.FindSystem<UnitBodySystem>().SetUnitPlayerColor(bodyOrgan, player);
            //world.FindSystem<UnitUIShowSystem>().InitShow(uIShowOrgan);
            //unit.GetComponent<Client_UnitProperty>().ORPC_ChangeLayer(unit.gameObject, LayerMask.NameToLayer("Default"));



            if (!AddUnitToPos(unit, node))
                Debug.LogError(unit.name+"该单位Spwan时没有被分配到Node上");
            UpdateDynamicObstacle(node);





            if (instance.UnitCreateActionDict.ContainsKey(unit.gameObject))
            {
                instance.UnitCreateActionDict[unit.gameObject]?.Invoke(unit);
                instance.UnitCreateActionDict.Remove(unit.gameObject);
            }
            if (!instance.PlayersUnitsDict.ContainsKey(player))
                instance.PlayersUnitsDict.Add(player, new List<UnitBase>());
            instance.PlayersUnitsDict[player].Add(unit);
            //testUnitModel.Init(unit,world);

            FightLog.Record($"玩家:{CampManager.GetPlayerEnum(player)} 单位:{unit.gameObject.name},被添加到游戏中{unit.transform.position.ToString()}");
#if UNITY_EDITOR&&UNITY_SERVER
            unit.gameObject.AddComponent<TestUnitModel>();
#endif

            return unit;
        }

        public bool TryGetUnitModel(string key,out UnitModel res)
        {
            if (instance.UnitModelDict.TryGetValue(key, out res)) return true;
            else return false;
        }
        public UnitModel RegisterUnitModel(UnitModel unitModel)
        {
            if (instance.UnitModelDict.ContainsKey(unitModel.ModelName))return unitModel;
            else
            {
                instance.UnitModelDict.Add(unitModel.ModelName, unitModel);
                return unitModel;
            }

        }

        //private UnitBase Test_SwapnUnitReal(UnitModel testUnitModel, GameObject model)
        //{
        //    if (testUnitModel == null) return null;


        //    ClientSpawnUnit(model);

        //    PlayerMemeber player = CampManager.GetPlayerMemeber(testUnitModel.player);
        //    Node node = AStarPathfinding2D.GetWorldPositionNode(model.transform.position, instance.GridMap);
        //    //Debug.Log(node + "1a1");

        //    //node = AStarPathfinding2D.FindNearestNode(AStarPathfinding2D.GetNode(node.Position, instance.GridMap), 10, (u, c) => { return true; }, instance.GridMap, true, true);
        //    //if(testUnitModel.gameObject.GetComponent)
        //    CharacterFSM characterFSM = model.GetComponent<CharacterFSM>();
        //    if (characterFSM != null) GameObject.Destroy(characterFSM);
        //    characterFSM = model.AddComponent<CharacterFSM>();

        //    UnitBase unit = model.AddComponent<UnitBase>();
        //    BodyOrgan bodyOrgan = unit.AddOrgan<BodyOrgan>(ComponentType.body);
        //    AttackOrgan attackOrgan = unit.AddOrgan<AttackOrgan>(ComponentType.attack);
        //    TalentOrgan talentOrgan = unit.AddOrgan<TalentOrgan>(ComponentType.talent);
        //    MagicOrgan magicOrgan = unit.AddOrgan<MagicOrgan>(ComponentType.magic);
        //    //技能要放在比较后面
        //    if (!String.IsNullOrEmpty(testUnitModel.activeSkill))
        //    {
        //        //magicOrgan.AnimationLong = testUnitModel.spellTime;
        //        world.FindSystem<UnitSpellSystem>().GainSkill(magicOrgan, testUnitModel.activeSkill);
        //        Debug.Log(magicOrgan.OwnerUnit + "拥有" + testUnitModel.activeSkill);

        //    }
        //    if (testUnitModel.passiveSkills != null && testUnitModel.passiveSkills.Length > 0)
        //    {
        //        foreach (var v in testUnitModel.passiveSkills)
        //        {
        //            if (!String.IsNullOrEmpty(v))
        //                world.FindSystem<UnitTalentSystem>().GainSkill(talentOrgan, v);
        //        }
        //    }

        //    bodyOrgan.Health_Max = testUnitModel.health_Max;
        //    bodyOrgan.Health_Curr = testUnitModel.health_Curr;
        //    bodyOrgan.Def = testUnitModel.def;
        //    bodyOrgan.Evade = testUnitModel.evade;


        //    //MagicSkillSystem
        //    unit.transform.position = AStarPathfinding2D.GetNodeWorldPositionV3(node.Position, instance.GridMap);
        //    UIShowOrgan uIShowOrgan = unit.AddOrgan<UIShowOrgan>(ComponentType.uIShow);
        //    uIShowOrgan.UnitHight = testUnitModel.hight;
        //    //world.FindSystem<UnitUIShowSystem>().InitShow(uIShowOrgan);



        //    world.FindSystem<UnitBodySystem>().SetUnitPlayer(bodyOrgan, player);
        //    attackOrgan.AttackRange = testUnitModel.attackRange;
        //    attackOrgan.WarningRange = testUnitModel.warningRange;
        //    attackOrgan.OriginAttackVal = testUnitModel.attackVal;
        //    attackOrgan.AttackTime = testUnitModel.attackTime;
        //    attackOrgan.AttackSpeed = testUnitModel.attackSpeed;
        //    attackOrgan.AttackHitrate = testUnitModel.attackHitrate;
        //    attackOrgan.AttackCriticalDamage = testUnitModel.attackCriticaldamage;
        //    attackOrgan.AttackCriticalChance = testUnitModel.attackCriticalchance;


        //    instance.AttackSystem.ChangeWeapon(attackOrgan, testUnitModel.projectile);
        //    instance.AttackSystem.ChangeFsm(attackOrgan);
        //    unit.AddOrgan<LegOrgan>(ComponentType.leg).MoveSpeed = testUnitModel.moveSpeed;//.MoveSpeed=testUnitModel.moveSpeed;
        //    AddUnitToPos(unit, node);
        //    UpdateDynamicObstacle(node);


        //    if (instance.UnitCreateActionDict.ContainsKey(unit.gameObject))
        //    {
        //        instance.UnitCreateActionDict[unit.gameObject]?.Invoke(unit);
        //        instance.UnitCreateActionDict.Remove(unit.gameObject);
        //    }
        //    if (!instance.PlayersUnitsDict.ContainsKey(player))
        //        instance.PlayersUnitsDict.Add(player, new List<UnitBase>());
        //    instance.PlayersUnitsDict[player].Add(unit);
        //    //testUnitModel.Init(unit,world);

        //    FightLog.Record($"玩家:{CampManager.GetPlayerEnum(player)} 单位:{unit.gameObject.name},被添加到游戏中{unit.transform.position.ToString()}");


        //    return unit;
        //}
        public override void Update()
        {
            base.Update();
            while (instance.UnitCreateQueue.Count > 0)
            {
                (GameObject, PlayerEnum, UnitModel) go = instance.UnitCreateQueue.Dequeue();
                SwapnUnitReal(go.Item3,go.Item1,go.Item2);
            }
            //while (instance.UnitDeadQeueue.Count > 0)
            //{
            //    UnityEngine.Object @object = instance.UnitDeadQeueue.Dequeue();
            //    Debug.Log(@object.name + "被销毁！");
            //    GameObject.Destroy(@object);
            //}
        }

        public override void LateUpdate()
        {
            base.LateUpdate();

            //AStarPathfinding2D.UpdateDynamicObstacleList(instance.GridMap);
            
        }
        

    }
}
