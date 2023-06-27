using Saber.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.ECS;
using Saber.Camp;
using cfg.heroLevelData;
using cfg.heroInfo;

namespace XianXia.Unit
{
    public static class UnitUtility
    {
        public static bool IsUnitInvincible(UnitBase unit)
        {
            return unit?.FindOrganInBody<BodyOrgan>(ComponentType.body, false) == null;
        }
        public static void ClearStatusOrgan<T>(StatusOrganBase<T> target) where T : IComponentBase,IRealName
        {
            if (target == null) return;
            target.StatusList.Clear();
            target.StatusNumDict.Clear();
        }
        public static void StatusOrganAdd<T>(StatusOrganBase<T> target, string name, T t, Action<T> removeAction) where T : IComponentBase, IAllowOwnNum,IRealName
        {
            if (target == null||t==null||t.Enable==false) return;
            //Debug.Log(target.OwnerUnit + "获得技能QQQ" + name);
            t.Owner = target;
            target.StatusList.Add(t);
            if (!target.StatusNumDict.ContainsKey(name))
                target.StatusNumDict.Add(name, new List<T> { t });
            else if (target.StatusNumDict[name].Count < t.AllowOwnMaxNum)
            {
                target.StatusNumDict[name].Add(t);
            }
            else
            {
                int removeNum = target.StatusNumDict[name].Count - t.AllowOwnMaxNum + 1;
                while (removeNum > 0)
                {
                    removeAction.Invoke(target.StatusNumDict[name][target.StatusNumDict[name].Count - 1]);
                    removeNum--;
                }
                target.StatusNumDict[name].Add(t);
            }
        }
        internal static bool StatusOrganContains<T>(T buff,StatusOrganBase<T> bar) where T : IComponentBase, IRealName
        {
            return bar.StatusList.Contains(buff);
        }

        public static void StatusOrganRemove<T>(T buff)where T: IComponentBase,IRealName
        {
            if (buff!=null&&buff.Owner != null)
            {
                StatusOrganBase<T> statusBar = (StatusOrganBase<T>)buff.Owner;
                if (statusBar != null)
                {
                    if (statusBar.StatusList.Contains(buff)) statusBar.StatusList.Remove(buff);
                    if (statusBar.StatusNumDict.TryGetValue(buff.RealName, out var buffNum) && buffNum.Contains(buff)) buffNum.Remove(buff);

                }
            }
        }
        public static void StatusOrganRemove<T>(T buff,out bool res) where T : IComponentBase, IRealName
        {
            res = false;
            if (buff != null && buff.Owner != null)
            {
                StatusOrganBase<T> statusBar = (StatusOrganBase<T>)buff.Owner;
                if (statusBar != null)
                {
                    if (statusBar.StatusList.Contains(buff)) { statusBar.StatusList.Remove(buff);res = true; }
                    if (statusBar.StatusNumDict.TryGetValue(buff.RealName, out var buffNum) && buffNum.Contains(buff)) buffNum.Remove(buff);
                }
            }
        }
        public static T[] StatusOrganRemove<T>(string name, StatusOrganBase<T> bar, out bool res) where T : IComponentBase, IRealName
        {
            res = false;
            T[] items=null;
            if (bar != null && bar.OwnerUnit != null)
            {
                //StatusOrganBase<T> bar = (StatusOrganBase<T>)buff.Owner;
                if (bar != null)
                {
                    //if (bar.StatusList.Contains(buff)) { bar.StatusList.Remove(buff);res = true; }
                    if (bar.StatusNumDict.TryGetValue(name, out var buffNum) && buffNum.Count > 0)
                    {
                        items = buffNum.ToArray();
                        for(int i = 0; i < buffNum.Count; i++)
                        {
                            if (bar.StatusList.Contains(buffNum[i]))
                                bar.StatusList.Remove(buffNum[i]);
                        }
                        buffNum.Clear();
                        bar.StatusNumDict.Remove(name);
                    }
                }
            }
            res = items != null && items.Length > 0;
            return items;
        }

        public static bool CalculatePer(float per)
        {
            float p= UnityEngine.Random.Range(0f, 1.00f);
            //Debug.Log("CC" + p + "///" + per);
            return p <= per;
        }

        public static Vector3 GetBodyPos(UnitBase unit)
        {
            if (unit == null) return Vector3.zero;
            UIShowOrgan uIShowOrgan = unit.FindOrganInBody<UIShowOrgan>(ComponentType.uIShow);
            return uIShowOrgan != null ? unit.transform.position + Vector3.up * uIShowOrgan.UnitHight / 2 : unit.transform.position + Vector3.up * 1f;


        }
        /// <summary>
        /// 返回两个格子之间的关系
        /// 如果不存在占领棋子则返回none
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="otherGrid"></param>
        /// <returns></returns>
        public static CampRelation RelationOfTwoGrids(UnitBase chess1, UnitBase chess2)
        {
            if (chess1 == null || chess2 == null) return CampRelation.none;
            return RelationOfTwoGrids(chess1.FindOrganInBody<BodyOrgan>(ComponentType.body), chess2.FindOrganInBody<BodyOrgan>(ComponentType.body));
        }
        /// <summary>
        /// 返回两个格子之间的关系
        /// 如果不存在占领棋子则返回none
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="otherGrid"></param>
        /// <returns></returns>
        public static CampRelation RelationOfTwoGrids(BodyOrgan bodyOrgan1, BodyOrgan bodyOrgan2)
        {
            if (bodyOrgan1 == null || bodyOrgan2 == null) return CampRelation.none;

            return CampManager.Instance.CampsRealtion(bodyOrgan1.OwnerPlayer.BelongCamp, bodyOrgan2.OwnerPlayer.BelongCamp);
        }
        /// <summary>
        /// 返回两个格子之间的关系
        /// 如果不存在占领棋子则返回none
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="otherGrid"></param>
        /// <returns></returns>
        public static CampRelation RelationOfTwoGrids(PlayerMemeber bodyOrgan1, PlayerMemeber bodyOrgan2)
        {
            if (bodyOrgan1 == null || bodyOrgan2 == null) return CampRelation.none;
            return CampManager.Instance.CampsRealtion(bodyOrgan1.BelongCamp, bodyOrgan2.BelongCamp);
        }
        public static bool TargetFiltration(UnitBase unitA, UnitBase unitB, TargetType targetType)
        {
            //Debug.Log(unitA + "SSS" + unitB);
            if (targetType == TargetType.none) return false;
            if (unitA == null || unitB == null) return false;
            BodyOrgan bodyA = unitA.FindOrganInBody<BodyOrgan>(ComponentType.body);
            BodyOrgan bodyB = unitB.FindOrganInBody<BodyOrgan>(ComponentType.body);
            if (bodyA == null || bodyB == null) return false;

            //包括自己是强包含
            if (unitA == unitB)
            {
                if ((targetType & TargetType.self) != 0) return true;
                else return false;

            }
           // Debug.Log(unitA + "aa" + unitB + "RRR");

            if (bodyA.OwnerPlayer == bodyB.OwnerPlayer && (targetType & TargetType.selfUnit) != 0) return true;
            CampRelation campRelation = UnitUtility.RelationOfTwoGrids(bodyA, bodyB);
            //Debug.Log(((targetType & TargetType.enemy) != 0 && campRelation == CampRelation.hostile) + "SSS");
            if ((targetType & TargetType.enemy) != 0 && campRelation == CampRelation.hostile) return true;
            if ((targetType & TargetType.friend) != 0 && campRelation == CampRelation.friendly) return true;
            return false;

        }

        /// <summary>
        /// 两具肉体素质比较，主要比较生命，防御
        /// 如果A比B弱返回-1，否则1
        /// </summary>
        /// <param name="unitA"></param>
        /// <param name="unitB"></param>
        /// <returns></returns>
        public static int UnitCompare(UnitBase unitA, UnitBase unitB)
        {
            BodyOrgan bodyOrganA = unitA.FindOrganInBody<BodyOrgan>(ComponentType.body);
            BodyOrgan bodyOrganB = unitB.FindOrganInBody<BodyOrgan>(ComponentType.body);
            if (bodyOrganA == null) return -1;
            if (bodyOrganB == null) return 1;
            return bodyOrganA.Health_Curr.CompareTo(bodyOrganB.Health_Curr);
        }
        public static PlayerEnum GetUnitBelongPlayerEnum(UnitBase unit)
        {
            if (unit == null) return default;
            return CampManager.GetPlayerEnum(unit.FindOrganInBody<BodyOrgan>(ComponentType.body)?.OwnerPlayer);
        }
        public static PlayerMemeber GetUnitBelongPlayer(UnitBase unit)
        {
            if (unit == null) return null;
            return unit.FindOrganInBody<BodyOrgan>(ComponentType.body)?.OwnerPlayer;

        }
        public static int SmartSortUnitFiltration(UnitBase spellUnit, UnitBase unitA, UnitBase unitB, TargetType targetType)
        {
            if (spellUnit == null || targetType == TargetType.none) return 0;
            if (unitA == null) return -1;
            if (unitB == null) return 1;
            LegOrgan leg = spellUnit.FindOrganInBody<LegOrgan>(ComponentType.leg);
            if (leg != null && leg.Follower != null)
            {
                if (leg.Follower == unitA) return 1;
                if (leg.Follower == unitB) return -1;
            }
            if ((targetType & TargetType.enemy) != 0)
                return UnitCompare(unitA, unitB);
            return -UnitCompare(unitA, unitB);

        }
        public static int DistanceSortUnitFiltration(UnitBase me,UnitBase unitA,UnitBase unitB,UnitMainSystem unitMainSystem)
        {
            return unitMainSystem.GetDistanceOfTwoUnits(me, unitA).CompareTo(unitMainSystem.GetDistanceOfTwoUnits(me, unitB));
        }

        public static UnitModel GetUnitModelByHeroLevelID(int id,UnitMainSystem mainSystem)
        {
            HeroLevelData heroLevelData = LubanMgr.GetHeroLevelData(id);
            if (heroLevelData == null)
            {
                FightServerManager.ConsoleWrite_Saber($"heroLevelID:{id}是无法被寻找到的，请检查HeroLevel表");
                return null;
            }            
            HeroInfo heroInfo = LubanMgr.GetHeroData(heroLevelData.HeroId);
            if (heroInfo == null)
            {
                FightServerManager.ConsoleWrite_Saber($"heroID:{heroLevelData.HeroId}是无法被寻找到的，请检查HeroInfo表");
                return null;
            }
#if UNITY_EDITOR
            Debug.Log(heroLevelData.ToString());
            Debug.Log(heroInfo.ToString());
#endif

            //foreach(var v in heroLevelData)
            if (mainSystem.TryGetUnitModel($"Hero_{heroInfo.HeroPrefab}", out UnitModel res)) return res;
            else return mainSystem.RegisterUnitModel(
                new UnitModel($"Hero_{heroInfo.HeroPrefab}", heroInfo.HeroPrefab, heroInfo.HeroHigh,
                heroLevelData.HeroHealth, heroLevelData.HeroHealth, heroLevelData.HeroDefence, heroLevelData.HeroMovementspeed, heroInfo.HeroAttackTime,
               (float)heroLevelData.HeroAttackrange / 100, heroLevelData.HeroAttack, heroLevelData.HeroAttackspeed, (float)heroLevelData.HeroAlertdistance / 100,
                Projectile.GetProjectile(heroInfo.ProjectilePrefab),
                heroLevelData.HeroStrongskill, heroLevelData.HeroSkill, heroLevelData.HeroMp, heroLevelData.HeroMp, heroLevelData.AttackedMp, heroLevelData.AttackMp,
               (float)heroLevelData.HeroHitrate / 100, (float)heroLevelData.HeroCriticalchance / 100, (float)heroLevelData.HeroCriticaldamage / 100, (float)heroLevelData.HeroEvade / 100));
        }

        public static void FristFightListen(Action<AttackOrgan, BodyOrgan, Damage> action)
        {
            SaberEvent<AttackOrgan, BodyOrgan, Damage> saberEvent = GameManager.MainWorld.FindSystem<EventSystem>().GetEvent<AttackOrgan, BodyOrgan, Damage>(EventSystem.EventParameter.UnitAttackBefore);
            Action<AttackOrgan, BodyOrgan, Damage> action1 = null;
            action1 = (a, b, d) =>
            {
                action?.Invoke(a,b,d);
                saberEvent.RemoveAction(action1);
            };
            saberEvent.AddAction(action1);
        }
    }
}
