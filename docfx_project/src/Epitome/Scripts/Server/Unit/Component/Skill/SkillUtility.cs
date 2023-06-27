using cfg.shiftShape;
using cfg.skillActive;
using cfg.skillPassive;
using cfg.summoned;
using FishNet;
using Saber.Base;
using Saber.Camp;
using Saber.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XianXia.Terrain;
using XianXia.Unit;

namespace XianXia.Unit
{
    public  class SkillUtility:AutoSingleton<SkillUtility> 
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m">自己</param>
        /// <param name="b">目标</param>
        /// <param name="damage">伤害值</param>
        /// <param name="num">参数</param>
        /// <param name="continueLong">持续时长</param>
        public delegate void SkillEffectEvent(UnitBase m, UnitBase b,Damage damage, float num, float continueLong);
        Dictionary<int, SkillEffectEvent> skillEffectDict = new Dictionary<int, SkillEffectEvent>();
        Dictionary<int,Func<TargetType,UnitBase,int, float,List<UnitBase>>> findTargetsDict = new Dictionary<int, Func< TargetType, UnitBase, int,float, List<UnitBase>>>();
        WorldBase world;
        UnitMainSystem mainSystem;
        UnitAttackSystem attackSystem;
        UnitBodySystem bodySystem;
        AStarPathfinding2D map;

        Node playerStartNode;
        Node monsterStartNode;
        public static WorldBase World => instance.world;
        static UnitMainSystem MainSystem => instance.mainSystem;
        static AStarPathfinding2D Map => instance.map;

        static UnitBodySystem BodySystem => instance.bodySystem;

        public static Node PlayerStartNode { get => instance.playerStartNode; }
        public static Node MonsterStartNode { get =>instance.monsterStartNode;  }

        public static void ShowRisingSpace(string message,Vector3 pos,Vector3 dir,Color color,int size=48, TMPro.FontStyles fontStyles=TMPro.FontStyles.Normal)=>InstanceFinder.GetInstance<NormalUtility>().ORPC_ShowRisingSpace(message, pos,dir,color, size, fontStyles);
        public static string GetSkillResouceNameById(int id,bool isActive)
        {
            if (isActive)
            {
                SkillActive sa = LubanMgr.GetSkillActiveData(id);
                if (sa != null)
                    return sa.SkillResourcename;
                else
                    Debug.LogError($"主动技能ID:{id}不在表中");
            }
            else
            {
                SkillPassive sp = LubanMgr.GetSkillPassiveData(id);
                if (sp != null)
                    return sp.SkillResourcename;
                else
                    Debug.LogError($"被动技能ID:{id}不在表中");
            }
            return string.Empty;
        }
        public static List<Node> GetNodesForConditionOfCenter(MagicOrgan m,Node target,TargetType targetType,float influenceRange)
        {
           return AStarPathfinding2D.FindTargetsInRange(target, influenceRange, (Func<Node, Node, bool>)((a, b) => { return UnitUtility.TargetFiltration((UnitBase)m.OwnerUnit,(UnitBase)instance.mainSystem.GetUnitByGridItem((Node)b), (TargetType)targetType); }), Map, true, false, true);

        }

        public static TargetType CalculateTargetObject(int num)
        {
            switch (num)
            {
                case  1:
                    return TargetType.self;
                case 2:
                    return TargetType.enemy;
                case 3:
                    return TargetType.friend;
                case 4:
                    return TargetType.enemy | TargetType.friend;
            }
            FightServerManager.ConsoleWrite_Saber("技能目标计算错误，传入的值不是合法的");
            return TargetType.none;
        }

        public static int CalculateLifeTime(int num)
        {
            return num > 0 ? num : -1;
        }
        internal static void SetSkillMagicPoint(int max, int start, int attack, int attacked, MagicOrgan t)
        {
            IMagicPointRecover magicPointRecover = t;
            magicPointRecover.CanRecordMagicPoint = true;
            magicPointRecover.MagicPoint_Max = max;
            magicPointRecover.MagicPoint_Curr = start;
            magicPointRecover.MagicPoint_Attack = attack;
            magicPointRecover.MagicPoint_Damaged = attacked;
        }
        #region 召唤物
        public static SummonSkill.Summon GetSummonData(int id,int num,float liveTime)
        {

            //跳转召唤物的表，获取数据填入
            cfg.summoned.Summoned summoned = LubanMgr.GetSummonData(id);
            if (summoned == null)
            {
                FightLog.Record($"召唤物ID:{id}错误，无法召唤召唤物");
                return null;
            }
            string s = $"{id}_{summoned.HeroPrefab}";
            string path = $"{ABUtility.SummonMainName}{summoned.HeroPrefab}";
            Saber.Base.ObjectPool<GameObject> p = InstanceFinder.GetInstance<NormalUtility>().Server_InitSpawnPool(path, s, null,0);      
            return new SummonSkill.Summon(num, liveTime,summoned,p);
        }
        public static void KillSummon(SummonSkill.Summon summon,UnitBase unit)
        {
            if(summon.Contains(unit))
                BodySystem.UnitDead(unit.FindOrganInBody<BodyOrgan>(ComponentType.body),null);
        }

        public static UnitModel GetSummonedModel(Summoned summon)
        {
            if (summon == null) return null;
            if (MainSystem.TryGetUnitModel($"Sum_{summon.HeroPrefab}", out UnitModel res)) return res;
            else return MainSystem.RegisterUnitModel(
                new UnitModel($"Sum_{summon.HeroPrefab}", summon.HeroPrefab,
                summon.HeroHigh, summon.SummonedHealth, summon.SummonedHealth, summon.SummonedDefence, summon.SummonedMovementspeed, 0,(float) summon.SummonedAttackrange/100, summon.SummonedAttack, summon.SummonedAttackspeed, (float)summon.HeroAlertdistance/100,
                Projectile.GetProjectile(summon.ProjectilePrefab)
                , summon.SummonedStrongskill, summon.SummonedSkill, summon.SummonedMp, summon.SummonedMp, summon.AttackedMp, summon.AttackMp
                , (float)summon.SummonedHitrate/100, (float)summon.SummonedCriticalchance/100, (float)summon.SummonedCriticaldamage/100, (float)summon.SummonedEvade/100
                ));
        }
        #endregion
        public static ShiftShape GetShiftShapeModel(int id)
        {
            return LubanMgr.GetShiftShapeData(id);
        }

        public static SkillEffectEvent GetSkillAction(int id)
        {
            if (instance.skillEffectDict.TryGetValue(id, out SkillEffectEvent res)) return res;
            else return null;
        }
        #region 单位筛选
        public static List<UnitBase> ScreeningUnit_Smart(UnitBase[] res, int effectNum, UnitBase unit, TargetType targetType, bool isFiltInvincible, float minDis=-1)
        {
            List<UnitBase> list = res.ToList();
            if (isFiltInvincible)
                list.RemoveAll((u) => { BodyOrgan b = u.FindOrganInBody<BodyOrgan>(ComponentType.body); return b == null || b.Enable == false; });
            list.Sort((a, b) => UnitUtility.SmartSortUnitFiltration(unit, a, b,targetType ));
            if (minDis > 0)
                list.RemoveAll((u) => { return instance.mainSystem.GetDistanceOfTwoUnits(u, unit) < minDis; });
            if(list.Count>effectNum)
                list.RemoveRange(effectNum, list.Count - effectNum);


            return list;
        }
        public static List<UnitBase> ScreeningUnit_Distance(UnitBase[] res, int effectNum,UnitBase unit,bool isFiltInvincible,float minDis=-1)
        {
            List<UnitBase> list = res.ToList();
            if(isFiltInvincible)
                list.RemoveAll((u) => { BodyOrgan b = u.FindOrganInBody<BodyOrgan>(ComponentType.body); return b == null || b.Enable == false; });
            list.Sort((a, b) => UnitUtility.DistanceSortUnitFiltration(unit, a, b, instance.mainSystem));
            if(list.Count>effectNum)
                list.RemoveRange(effectNum, list.Count - effectNum);
            if (minDis > 0)
                list.RemoveAll((u) => { return instance.mainSystem.GetDistanceOfTwoUnits(u, unit) < minDis; });

            return list;
        }
        public static List<UnitBase> FindUnit_Random(PlayerMemeber playerMemeber, TargetType targetType, int num, bool isFiltInvincible, UnitBase unit=null,  float minDis=-1)
        {
            if (num < 1) return new List<UnitBase>();
            List<UnitBase> targets = MainSystem.GetUnitByCondition(playerMemeber, targetType).ToList();
            if (isFiltInvincible)
                targets.RemoveAll((u) => { BodyOrgan b = u.FindOrganInBody<BodyOrgan>(ComponentType.body); return b == null || b.Enable == false; });
            if (minDis > 0&&unit!=null)
                targets.RemoveAll((u) => { return instance.mainSystem.GetDistanceOfTwoUnits(u, unit) < minDis; });
            if (num >= targets.Count) return targets;
            int count = 0;
            System.Random random = new System.Random();
            List<int> array = new List<int>();
            while (count < targets.Count)
                array.Add(count++);
            List<UnitBase> res = new List<UnitBase>();
            for (int i = 0; i < num; i++)
            {
                count = random.Next(0, array.Count);
                res.Add(targets[array[count]]);
                array.Remove(count);
            }
            return res;
        }

        public static  List<UnitBase> FindUnit_CurrAttack(TargetType targetType,UnitAttackSystem attackSystem,UnitBase unit)
        {
            Debug.Log("寻找到" + attackSystem.FindAttackTarget(unit.FindOrganInBody<AttackOrgan>(ComponentType.attack)));
            UnitBase target = attackSystem.FindAttackTarget(unit.FindOrganInBody<AttackOrgan>(ComponentType.attack));
            BodyOrgan bodyOrgan = null;
            if (target!=null)
                 bodyOrgan= target.FindOrganInBody<BodyOrgan>(ComponentType.body,false);
            if (bodyOrgan == null) target = null;
            return new List<UnitBase>(1) { target };
        }
        #endregion
        public static List<UnitBase> GetFindTargetsFunc(int id, TargetType targetType, UnitBase u, int effectNum,float spellRange=-1)
        {
            if (instance.findTargetsDict.TryGetValue(id, out Func<TargetType,UnitBase,int,float, List<UnitBase>> ob))
                return ob?.Invoke(targetType, u, effectNum,spellRange);
            else
            {
                Debug.LogError($"输入了错误的寻找目标函数的ID{id}");
                return new List<UnitBase>();
            }
        }
        public static bool IsValidFindID(int id)
        {
            return instance.findTargetsDict.ContainsKey(id);
        }

        private void Start()
        {
            world = GameObject.FindObjectOfType<WorldBase>();
            mainSystem = world.FindSystem<UnitMainSystem>();
            bodySystem = world.FindSystem<UnitBodySystem>();
            attackSystem = world.FindSystem<UnitAttackSystem>();
            map = GameObject.FindObjectOfType<AStarPathfinding2D>();
             monsterStartNode= AStarPathfinding2D.GetNode(new Vector2Int(map.MapX - 1, map.MapY / 2), map);
            playerStartNode = AStarPathfinding2D.GetNode(new Vector2Int(0, map.MapY / 2), map);

            Debug.Log("cao" + playerStartNode.Position.x + "aa" + playerStartNode.Position.y);
            Debug.Log("caoD" + monsterStartNode.Position.x + "aa" + monsterStartNode.Position.y);


            skillEffectDict.Add(1, FixedDamage);
            skillEffectDict.Add(2, FixedHeal);
            skillEffectDict.Add(3, SkillAttackPerDamage);
            skillEffectDict.Add(4, PerHeal);
            skillEffectDict.Add(8, CriticalStrike);


            findTargetsDict.Add(1, FindTarget_Curr);
            findTargetsDict.Add(2, FindTarget_Self);
            findTargetsDict.Add(3, FindTargets_Closed);
            findTargetsDict.Add(4, FindTargets_Smart);
            findTargetsDict.Add(5, FindTargets_Random);
            findTargetsDict.Add(6, FindTargets_All);

            InitBuffAction();
        }
        #region FindTarget
        static List<UnitBase> FindTarget_Curr(TargetType targetType, UnitBase u, int effectNum, float spellRange) => SkillUtility.FindUnit_CurrAttack(targetType, instance.attackSystem, u);
        static List<UnitBase> FindTarget_Self(TargetType targetType, UnitBase u, int effectNum, float spellRange) => new List<UnitBase>(1) { u };
        static List<UnitBase> FindTargets_Closed(TargetType targetType, UnitBase u, int effectNum,float spellRange)
        {
            UnitBase[] res = null;
            res = instance.mainSystem.GetUnitByCondition(UnitUtility.GetUnitBelongPlayer(u), targetType);
            if (effectNum == 0 ) return res.ToList();
            if (spellRange <= 0 && effectNum >= res.Length) return res.ToList();
            return SkillUtility.ScreeningUnit_Distance(res, effectNum, u,targetType==TargetType.enemy?true:false, spellRange);
        }
        static List<UnitBase> FindTargets_All(TargetType targetType, UnitBase u, int effectNum, float spellRange)
        {
            UnitBase[] res = null;
            res = instance.mainSystem.GetUnitByCondition(UnitUtility.GetUnitBelongPlayer(u), targetType);
            return res.ToList();
        }
        static List<UnitBase> FindTargets_Smart(TargetType targetType, UnitBase u, int effectNum, float spellRange)
        {
            UnitBase[] res = null;
            res = instance.mainSystem.GetUnitByCondition(UnitUtility.GetUnitBelongPlayer(u), targetType);
            if (effectNum == 0 ||(spellRange<=0&& effectNum >= res.Length)) return res.ToList();
            return SkillUtility.ScreeningUnit_Smart(res, effectNum, u, targetType, targetType == TargetType.enemy ? true : false, spellRange);
        }
        static List<UnitBase> FindTargets_Random(TargetType targetType, UnitBase u, int effectNum, float spellRange)
        {
            UnitBase[] res = null;
            res = instance.mainSystem.GetUnitByCondition(UnitUtility.GetUnitBelongPlayer(u), targetType);
            if (effectNum == 0 || (spellRange <= 0 && effectNum >= res.Length)) return res.ToList();
            return SkillUtility.FindUnit_Random(UnitUtility.GetUnitBelongPlayer(u), targetType, effectNum, targetType == TargetType.enemy ? true : false, u,  spellRange);
        }
        #endregion
        #region skilleffect
        /// <summary>
        /// 只操作伤害值
        /// </summary>
        /// <param name="m"></param>
        /// <param name="b"></param>
        /// <param name="damage"></param>
        /// <param name="fixedNum"></param>
        /// <param name="continueNum"></param>
        static void FixedDamage(UnitBase m, UnitBase b, Damage damage, float fixedNum, float continueNum)
        {
            if (fixedNum > 0)
            {
                if (m == null || b == null || damage == null)
                {
                    FightServerManager.ConsoleWrite_Saber("技能数据传入参数错误");
                    return;
                }
                damage.Val += (int)fixedNum;
                //World.FindSystem<UnitBodySystem>().ReceiveDamage(b, damage);
            }
        }
        /// <summary>
        /// 操作body
        /// </summary>
        /// <param name="m"></param>
        /// <param name="b"></param>
        /// <param name="damage"></param>
        /// <param name="fixedNum"></param>
        /// <param name="continueNum"></param>
        static void FixedHeal(UnitBase m, UnitBase b,Damage damage, float fixedNum,  float continueNum)
        {
            if (fixedNum > 0)
                World.FindSystem<UnitBodySystem>().UnitHeal(b.FindOrganInBody<BodyOrgan>(ComponentType.body), (int)fixedNum, m);
        }
        /// <summary>
        /// 操作伤害
        /// </summary>
        /// <param name="m"></param>
        /// <param name="b"></param>
        /// <param name="damage"></param>
        /// <param name="fixedNum"></param>
        /// <param name="continueNum"></param>
        static void SkillAttackPerDamage(UnitBase m, UnitBase b, Damage damage, float fixedNum, float continueNum)
        {
            if(fixedNum>0)
            {
                if (m == null || b == null || damage == null)
                {
                    FightServerManager.ConsoleWrite_Saber("技能数据传入参数错误");
                    return;
                }
                damage.Val += (int)(m.FindOrganInBody<AttackOrgan>(ComponentType.attack)?.AttackVal * (fixedNum / 100 - 1));
                //World.FindSystem<UnitBodySystem>().ReceiveDamage(b, damage);
            }
        }
        /// <summary>
        /// 操作body
        /// </summary>
        /// <param name="m"></param>
        /// <param name="b"></param>
        /// <param name="damage"></param>
        /// <param name="fixedNum"></param>
        /// <param name="continueNum"></param>
        static void PerHeal(UnitBase m, UnitBase b, Damage damage, float fixedNum, float continueNum)
        {
            if (fixedNum > 0)
            {
                BodyOrgan body = b.FindOrganInBody<BodyOrgan>(ComponentType.body);
                if (m == null || b == null ||body==null)
                {
                    FightServerManager.ConsoleWrite_Saber("技能数据传入参数错误");
                    return;
                }

                World.FindSystem<UnitBodySystem>().UnitHeal(body, (int)(body.Health_Max * fixedNum), m);

            }
        }
        /// <summary>
        /// 操作伤害
        /// </summary>
        /// <param name="m"></param>
        /// <param name="b"></param>
        /// <param name="damage"></param>
        /// <param name="fixedNum"></param>
        /// <param name="continueNum"></param>
        static void CriticalStrike(UnitBase m, UnitBase b, Damage damage, float fixedNum, float continueNum)
        {
            if (fixedNum > 0)
            {
                if (m == null || b == null || damage == null)
                {
                    FightServerManager.ConsoleWrite_Saber("技能数据传入参数错误");
                    return;
                }
                int v = damage.Val;
                damage.Val += (int)((m.FindOrganInBody<AttackOrgan>(ComponentType.attack)?.AttackVal * (fixedNum / 100 - 1)));
                Debug.Log($"暴击伤害增加{damage.Val - v},暴击倍数{(fixedNum / 100 - 1)}");
                damage.IsCriticalStrike = true;
                //World.FindSystem<UnitBodySystem>().ReceiveDamage(b, damage);

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m">伤害来源</param>
        /// <param name="b">被攻击者</param>
        /// <param name="damage"></param>
        /// <param name="fixedNum"></param>
        /// <param name="continueNum"></param>
        static void Hemophagia(UnitBase m, UnitBase b, Damage damage, float fixedNum, float continueNum)
        {
            if (fixedNum > 0)
            {
                BodyOrgan bodyOrgan;
                if (m == null ||  (bodyOrgan = m.FindOrganInBody<BodyOrgan>(ComponentType.body))==null|| b == null || damage == null)
                {
                    FightServerManager.ConsoleWrite_Saber("技能数据传入参数错误");
                    return;
                }else
                    World.FindSystem<UnitBodySystem>().UnitHeal(bodyOrgan, (int)(damage.Val * (fixedNum / 100 - 1)), m);
            }
        }
        void InitBuffAction()
        {
            int[] buffIds = LubanMgr.GetAllBuffID();
            foreach(var v in buffIds)
            {
                //Debug.Log($"TTTv{v},{}")
                skillEffectDict.Add(v, (m, b, d, f, c) =>
                {
                    if (b == null) return;
                    StatusOrgan s;
                    if ((s= b.FindOrganInBody<StatusOrgan>(ComponentType.statusHeart, true))!=null)
                        world.FindSystem<BuffSystem>().AddInherentBuff(v, s, m, f, (int)c);
                });
            }
        }
        #endregion
    }
}
