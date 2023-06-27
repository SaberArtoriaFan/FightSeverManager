using FSM;
using Saber.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Saber.ECS;
using XianXia.Terrain;
using FishNet;
using Saber.Camp;
using System.Linq;

namespace XianXia.Unit
{
    public class UnitAttackSystem : NormalSystemBase<AttackOrgan>
    {
        bool showAttackRange = true;
        bool showInSelected = true;
        Dictionary<AttackOrgan, UnitBase> attackDict = new Dictionary<AttackOrgan, UnitBase>();
        Dictionary<AttackOrgan, Damage> attackDamageDict = new Dictionary<AttackOrgan, Damage>();
        UnitMainSystem mainSystem;
        UnitMoveSystem unitMoveSystem;
        UnitBodySystem unitBodySystem;
        ProjectileSystem projectileSystem;
        //RisingSpaceUISystem risingSpaceUISystem;
        //ABManagerSystem aBManagerSystem;
        EventSystem eventSystem;
        AStarPathfinding2D map;

        SaberEvent<AttackOrgan, BodyOrgan,Damage> UnitAttackBefore;
        //SaberEvent<AttackOrgan, BodyOrgan> UnitAttack;
        SaberEvent<AttackOrgan, BodyOrgan, Damage> UnitAttackAfter;
        int ID_AttackBefore;
        int ID_AttackAfter;
        //public string ProjectilePackageName => "fight";

        public override void Awake(WorldBase world)
        {
            base.Awake(world);
            //aBManagerSystem=world.FindSystem<ABManagerSystem>();
            eventSystem = world.FindSystem<EventSystem>();
            UnitAttackBefore = eventSystem.RegisterEvent<AttackOrgan, BodyOrgan,Damage>(EventSystem.EventParameter.UnitAttackBefore,out ID_AttackBefore);
            //UnitAttack = eventSystem.RegisterEvent<AttackOrgan, BodyOrgan>("UnitAttack");
            UnitAttackAfter = eventSystem.RegisterEvent<AttackOrgan, BodyOrgan, Damage>(EventSystem.EventParameter.UnitAttackAfter,out ID_AttackAfter);
            //risingSpaceUISystem = world.FindSystem<RisingSpaceUISystem>();
        }
        public override void Start()
        {
            base.Start();
            mainSystem = World.FindSystem<UnitMainSystem>();
            unitMoveSystem= World.FindSystem<UnitMoveSystem>();
            unitBodySystem=World.FindSystem<UnitBodySystem>();
            projectileSystem=world.FindSystem<ProjectileSystem>();

            map = Object.FindObjectOfType<AStarPathfinding2D>();
            eventSystem.GetEvent<BodyOrgan, UnitBase>(EventSystem.EventParameter.UnitDeadBefore).AddAction(RemoveAttackTarget);
            //eventSystem.GetEvent<UnitBase, Vector3>("UnitMoveToNodeEvent").AddAction(FindAttackTarget);
            eventSystem.GetEvent<BodyOrgan, Damage>(EventSystem.EventParameter.UnitDamagedAfter).AddAction(DamageToAttack);
            eventSystem.GetEvent<UnitBase>(EventSystem.EventParameter.UnitFindNoPathEvent).AddAction(FindNotPathToGo);
        }

        private void FindNotPathToGo(UnitBase unit)
        {
            AttackOrgan attackOrgan = unit.FindOrganInBody<AttackOrgan>(ComponentType.attack);
            if(attackOrgan != null&&attackDict.ContainsKey(attackOrgan))
                attackDict.Remove(attackOrgan);
        }

        private void DamageToAttack(BodyOrgan selfBody, Damage soure)
        {
            if (selfBody == null || selfBody.OwnerUnit == null || soure == null) return;
            AttackOrgan attackOrgan = selfBody.OwnerUnit.FindOrganInBody<AttackOrgan>(ComponentType.attack);
            if(attackOrgan == null) return;
            BodyOrgan bodyOrgan=soure.Source.FindOrganInBody<BodyOrgan>(ComponentType.body);
            if (bodyOrgan == null) return;
            if (UnitUtility.RelationOfTwoGrids(selfBody, bodyOrgan) != Saber.Camp.CampRelation.hostile) return;
            if (!attackDict.ContainsKey(attackOrgan)) attackDict.Add(attackOrgan, bodyOrgan.OwnerUnit);
            else if (attackDict[attackOrgan] == null || attackDict[attackOrgan] == null) attackDict[attackOrgan] = bodyOrgan.OwnerUnit;
            else return;
            Debug.Log("���Ĺ���������" + attackDict[attackOrgan].ToString());

        }

    //    private void FindAttackTarget(UnitBase unit, Vector3 pos)
    //    {
    //        if (unit == null) return;
    //        var v = unit.FindOrganInBody<AttackOrgan>(ComponentType.attack);
    //        if (v == null ) return;
    //        if (attackDict.TryGetValue(v, out BodyOrgan enemyBody) && enemyBody != null && enemyBody.OwnerUnit != null)
    //        {
    //            //Debug.Log(Vector2.Distance(v.OwnerUnit.transform.position,enemyBody.OwnerUnit.transform.position) + "BBB111");

    //            if (AStarPathfinding2D.GetDistance(unitPosSystem.GetGridItemByUnit(v.OwnerUnit),
    //                unitPosSystem.GetGridItemByUnit(enemyBody.OwnerUnit), map) <= v.AttackRange)
    //            {
    //                unit.FindOrganInBody<LegOrgan>(ComponentType.leg).IsStand = true;
    //                AttackAction(v, enemyBody);
    //                return;
    //            }
    //        }
    //        //Ѱ�Ҹ���Ĺ�������
    //        //Debug.Log(unitPosSystem.GetUnitAction(v.OwnerUnit).ToString()+ UnitMainSystem.ActionPriorityLevel(unitPosSystem.GetUnitAction(v.OwnerUnit)) + "AAA111");
    //        Node target = AStarPathfinding2D.FindNearestNodeInAttackRange(unitPosSystem.GetGridItemByUnit(v.OwnerUnit), v.WarningRange, EnemyTest, map, true, false, false);
    //        UnitBase enemy = unitPosSystem.GetUnitByGridItem(target);
    //        //˵��һ������Ŀ���
    //        //վ�ŵ��˲��ܱ�����
    //        if (enemy != null)
    //        {
    //            enemyBody = enemy.FindOrganInBody<BodyOrgan>(ComponentType.body);
    //            if (attackDict.ContainsKey(v))
    //            {
    //                if (attackDict[v] == enemyBody) return;
    //            }
    //            if (attackDict.ContainsKey(v))
    //                attackDict[v] = enemyBody;
    //            else
    //                attackDict.Add(v, enemyBody);
    //            Debug.Log("���ù���Ŀ��" + v.ToString() + "//" + enemyBody.ToString());
    //            unitMoveSystem.UpdatePath(v.OwnerUnit.FindOrganInBody<LegOrgan>(ComponentType.leg), enemy);

    //        }
        
    //}

        private void RemoveAttackTarget(BodyOrgan body, UnitBase damage)
        {
            if (damage == null) return;
            AttackOrgan attackOrgan = damage.FindOrganInBody<AttackOrgan>(ComponentType.attack);
            if (attackOrgan!=null&&attackDict.ContainsKey(attackOrgan))
                attackDict.Remove(attackOrgan);
            if (body == null) return;
            attackOrgan = body.OwnerUnit.FindOrganInBody<AttackOrgan>(ComponentType.attack);
            if (attackOrgan != null && attackDict.ContainsKey(attackOrgan))
                attackDict.Remove(attackOrgan);
            //Debug.Log("�Ƴ�����������");
        }
        public void ChangeWeapon(AttackOrgan t,Projectile projectile)
        {
            if (t == null || t.OwnerUnit == null) return;
            //if (projectile == null && t.Projectile == null) return;
            Debug.Log("��������" + t.OwnerUnit.name +
                projectile?.ModelName);
            if (projectile != null && t.Projectile != null && projectile.ModelName == t.Projectile.ModelName) return;
            t.Projectile= projectile;

            Transform[] children = t.OwnerUnit.GetComponentsInChildren<Transform>();
            foreach (var c in children)
            {
                if (c.name.Equals("ProjectileDropPoint"))
                {
                    t.ProjectileDropPoint = c;
                    break;
                }
            }
            if (t.ProjectileDropPoint == null)
            {
                TimerManager.Instance.AddTimer(() =>
                {
                    var v = t.OwnerUnit?.FindOrganInBody<BodyOrgan>(ComponentType.body)?.PartBodyDict;
                    if (v!=null&&v.ContainsKey(2))
                        t.ProjectileDropPoint = t.OwnerUnit.FindOrganInBody<BodyOrgan>(ComponentType.body).PartBodyDict[2];
                }, Time.deltaTime*2);
            }


            //t.ProjectileModel = model;
            //if (t.Projectile != null && t.Projectile.ModelName != ""&&model==null)
            //{
            //    t.ProjectileModel = GetProjectileModel_Main(t.Projectile.ModelName);
            //    //if(t.ProjectileModel!=null) projectileSystem.InitPool(t.ProjectileModel, t.Projectile.ModelName);

            //}
        }
        public GameObject GetProjectileModel_Main(string name)
        {
            if (name != "")
            {
                //name = name.ToLower();
                Object obj = ABUtility.Load<Object>(ABUtility.ProjectMainName+name);
                if (obj == null) { Debug.Log(name+"����Դģ�Ͷ�ʧ"); return null; }

                else return obj as GameObject;
            }
            else
                return null;
        }
        public void ChangeFsm(AttackOrgan t)
        {
            t.CharacterFSM.RemoveState(FSM_State.attack);
            if (!t.IsHasTrajectory||string.IsNullOrEmpty(t.Projectile.ModelName) )
            //Debug.Log("��ӹ�����Ϊ");{
            {
                t.CharacterFSM.AddState(new Spine_Attack(t.OwnerUnit.GetComponentInChildren<Animator>(), t.AttackTime, () => DamageOther(t),t.OwnerUnit.FindOrganInBody<BodyOrgan>(ComponentType.body).ClothesSpineID));
                Debug.Log(t.OwnerUnit.gameObject.name + "�л�Ϊ��ս");
            }
            else
            {
                Debug.Log(t.OwnerUnit.gameObject.name + "�л�ΪԶ��");
                t.CharacterFSM.AddState(new Spine_Attack(t.OwnerUnit.GetComponentInChildren<Animator>(), t.AttackTime, () => Launchrajectory(t), t.OwnerUnit.FindOrganInBody<BodyOrgan>(ComponentType.body).ClothesSpineID));

            }
            (t.CharacterFSM.FindFSMState(FSM_State.attack) as Spine_Attack).SetFinishTime(t.AttackAnimationLong);
            t.AttackState = t.CharacterFSM.FindFSMState(FSM_State.attack) as Attack;
        }
        public UnitBase GetClosedEnemy(AttackOrgan a)
        {
            PlayerMemeber player = UnitUtility.GetUnitBelongPlayer(a.OwnerUnit);
            List<UnitBase> list= SkillUtility.ScreeningUnit_Distance(mainSystem.GetUnitByCondition(player, TargetType.enemy), 1, a.OwnerUnit,true);
            if (list != null && list.Count > 0) return list[0];
            else return null;
        }
        #region ��д����
        //protected override void InitAfterSpawn(AttackOrgan t)
        //{
        //    base.InitAfterSpawn(t);
        //    //Debug.Log(t.CharacterFSM + "000");
        //    //if (t.CharacterFSM == null) t.CharacterFSM = t.Owner.GetComponent<CharacterFSM>();
        //    if (t != null && t.CharacterFSM != null)
        //    {
        //        //if (!t.IsHasTrajectory)
        //        //{
        //        //    //Debug.Log("��ӹ�����Ϊ");
        //        //    t.CharacterFSM.AddState(new Attack(t.OwnerUnit.GetComponent<Animator>(), t.AttackAnimationLong, t.AttackTime, () => DamageOther(t)));

        //        //}
        //        //else
        //        //{
        //        //    t.CharacterFSM.AddState(new Attack(t.OwnerUnit.GetComponent<Animator>(), t.AttackAnimationLong, t.AttackTime, () => Launchrajectory(t)));

        //        //}
        //        //�е����Ļ�ûд
        //    }
        //}

        //protected override void InitializeBeforeRecycle(AttackOrgan t)
        //{
        //    if (t != null && t.CharacterFSM != null)
        //        t.CharacterFSM.RemoveState(FSM_State.attack);
        //    base.InitializeBeforeRecycle(t);
        //}
        //void AttackSystem(AttackOrgan v)
        //{
        //    if (v == null || v.OwnerUnit == null || !UnitMainSystem.ActionPriorityLevel(unitPosSystem.GetUnitAction(v.OwnerUnit))) continue;

        //    BodyOrgan enemyBody, lastEnemy;
        //    lastEnemy = null;
        //    if (attackDict.ContainsKey(v))
        //        lastEnemy = attackDict[v];
        //    enemyBody = lastEnemy;

        //    //û�й���Ŀ����߹���Ŀ�����Լ�������������ʱ
        //    if (/*!unitMoveSystem.IsFollowEachOther(v.OwnerUnit.FindOrganInBody<LegOrgan>(ComponentType.leg))&&*/(enemyBody == null || enemyBody.OwnerUnit == null || SystemUtility.RelationOfTwoGrids(enemyBody.OwnerUnit, v.OwnerUnit) != Saber.Camp.CampRelation.hostile || AStarPathfinding2D.GetDistance(unitPosSystem.GetGridItemByUnit(v.OwnerUnit),
        //            unitPosSystem.GetGridItemByUnit(enemyBody.OwnerUnit), map) > v.AttackRange * map.NodeSize))
        //    {
        //        Node target = AStarPathfinding2D.FindNearestNodeInAttackRange(unitPosSystem.GetGridItemByUnit(v.OwnerUnit), v.WarningRange, EnemyTest, map, true, false, false);
        //        UnitBase enemy = unitPosSystem.GetUnitByGridItem(target);
        //        if (enemy != null) enemyBody = enemy.FindOrganInBody<BodyOrgan>(ComponentType.body);
        //    }
        //    //����Լ�����Ŀ����û�е��˾ͼ�������ԭ���ĵ���
        //    if (enemyBody == null)
        //        enemyBody = lastEnemy;
        //    ////��֮ǰ�������˸��µ��ˣ���һ��ȷ��ֵ��������Ÿ���
        //    //else if (enemyBody != lastEnemy && lastEnemy != null && lastEnemy.OwnerUnit != null)
        //    //{
        //    //    float disE = AStarPathfinding2D.GetDistance(unitPosSystem.GetGridItemByUnit(enemyBody.OwnerUnit), unitPosSystem.GetGridItemByUnit(v.OwnerUnit), map);
        //    //    float disL = AStarPathfinding2D.GetDistance(unitPosSystem.GetGridItemByUnit(lastEnemy.OwnerUnit), unitPosSystem.GetGridItemByUnit(v.OwnerUnit), map);
        //    //    //�����Сû�и�����Ҫ
        //    //    if (disL - disE < map.NodeSize)
        //    //    {
        //    //        Debug.Log(disL-disE+"+��಻�󣬲�������");
        //    //        enemyBody = lastEnemy;
        //    //    }
        //    //}
        //    //Ѱ�Ҹ���Ĺ�������
        //    //Debug.Log(unitPosSystem.GetUnitAction(v.OwnerUnit).ToString()+ UnitMainSystem.ActionPriorityLevel(unitPosSystem.GetUnitAction(v.OwnerUnit)) + "AAA111");

        //    //˵��һ������Ŀ���
        //    //վ�ŵ��˲��ܱ�����
        //    if (enemyBody != null && enemyBody.OwnerUnit != null)
        //    {
        //        if (SystemUtility.RelationOfTwoGrids(enemyBody.OwnerUnit, v.OwnerUnit) != Saber.Camp.CampRelation.hostile)
        //        {
        //            if (attackDict.ContainsKey(v)) attackDict.Remove(v);
        //            Debug.Log("���ܹ����Ѿ�");
        //            continue;
        //        }
        //        if (lastEnemy == null)
        //            attackDict.Add(v, enemyBody);
        //        else
        //            attackDict[v] = enemyBody;
        //        //Debug.Log("���ù���Ŀ��" + v.OwnerUnit.ToString() + "//" + enemyBody.OwnerUnit.ToString());
        //        if (AStarPathfinding2D.GetDistance(unitPosSystem.GetGridItemByUnit(v.OwnerUnit),
        //            unitPosSystem.GetGridItemByUnit(enemyBody.OwnerUnit), map) <= v.AttackRange * map.NodeSize)
        //        {
        //            //ս���ľͿ�ֱ�ӹ���
        //            if (unitMoveSystem.IsUnitStand(v.OwnerUnit))
        //                AttackAction(v, enemyBody);
        //            else
        //                //����ͻص���λ��
        //                unitMoveSystem.GoBackPos(v.OwnerUnit);

        //        }
        //        else
        //        {
        //            LegOrgan leg = v.OwnerUnit.FindOrganInBody<LegOrgan>(ComponentType.leg);
        //            //��׷�˲����л���׷��Ŀ�����·��û�ˣ�·��û�˻����ڹ�����Χ�ڣ��ǿ϶�Ҫ������·��
        //            if (leg != null && (leg.Follower != enemyBody.OwnerUnit || leg.Path.Count == 0))
        //            {
        //                unitMoveSystem.UpdatePath(leg, enemyBody.OwnerUnit);
        //                //Debug.Log(leg.OwnerUnit+ "���¸���Ŀ��");
        //            }

        //        }
        //    }
        //}
        public override void Update()
        {
            base.Update();
            for (int i=0;i<allComponents.Count;i++)
            {
                AttackOrgan v = allComponents[i];
                if (v == null || v.OwnerUnit == null) { Debug.LogError("���ִ��󣬾��棡��");continue; }
                //����е��
                if (v.Enable == false) continue;
                //��ѣ��
                if (v.OwnerUnit.Enable == false) continue;
                if ( !UnitMainSystem.ActionPriorityLevel(mainSystem.GetUnitAction(v.OwnerUnit))) continue;
                Node myNode = mainSystem.GetGridItemByUnit(v.OwnerUnit);

                BodyOrgan enemyBody, lastEnemy;
                lastEnemy = null;
                if (attackDict.ContainsKey(v))
                {
                    lastEnemy = attackDict[v].FindOrganInBody<BodyOrgan>(ComponentType.body,false);
                    if (lastEnemy == null) attackDict.Remove(v);
                }

                enemyBody = lastEnemy;
                bool originDis = false;
                //û�й���Ŀ����߹���Ŀ�����Լ�������������ʱ
                if (enemyBody == null || enemyBody.OwnerUnit == null||enemyBody.Enable==false/*�޵��ж�*/ || (AStarPathfinding2D.GetDistance(myNode, mainSystem.GetGridItemByUnit(enemyBody.OwnerUnit), map) > v.AttackRange * map.NodeSize))
/*!unitMoveSystem.IsFollowEachOther(v.OwnerUnit.FindOrganInBody<LegOrgan>(ComponentType.leg))&&*//*|| SystemUtility.RelationOfTwoGrids(enemyBody.OwnerUnit, v.OwnerUnit) != Saber.Camp.CampRelation.hostile*/
                {

                    if (v.WarningRange >= map.MapX)
                        enemyBody = GetClosedEnemy(v)?.FindOrganInBody<BodyOrgan>(ComponentType.body);
                    else
                    {
                        Node target = AStarPathfinding2D.FindNearestNodeInAttackRange(myNode, v.WarningRange, EnemyTest, map, true, false, false);
                        UnitBase enemy = mainSystem.GetUnitByGridItem(target);
                        if (enemy != null) enemyBody = enemy.FindOrganInBody<BodyOrgan>(ComponentType.body);
                    }
                }
                //����Լ�����Ŀ����û�е��˾ͼ�������ԭ���ĵ���
                if (enemyBody==null||enemyBody.OwnerUnit==null)
                    enemyBody = lastEnemy;
                ////��֮ǰ�������˸��µ��ˣ���һ��ȷ��ֵ��������Ÿ���
                //else if (enemyBody != lastEnemy && lastEnemy != null && lastEnemy.OwnerUnit != null)
                //{
                //    float disE = AStarPathfinding2D.GetDistance(unitPosSystem.GetGridItemByUnit(enemyBody.OwnerUnit), unitPosSystem.GetGridItemByUnit(v.OwnerUnit), map);
                //    float disL = AStarPathfinding2D.GetDistance(unitPosSystem.GetGridItemByUnit(lastEnemy.OwnerUnit), unitPosSystem.GetGridItemByUnit(v.OwnerUnit), map);
                //    //�����Сû�и�����Ҫ
                //    if (disL - disE < map.NodeSize)
                //    {
                //        Debug.Log(disL-disE+"+��಻�󣬲�������");
                //        enemyBody = lastEnemy;
                //    }
                //}
                //Ѱ�Ҹ���Ĺ�������
                //Debug.Log(unitPosSystem.GetUnitAction(v.OwnerUnit).ToString()+ UnitMainSystem.ActionPriorityLevel(unitPosSystem.GetUnitAction(v.OwnerUnit)) + "AAA111");

                //˵��һ������Ŀ���
                //վ�ŵ��˲��ܱ�����
                if (enemyBody != null && enemyBody.OwnerUnit != null)
                {
                    if (UnitUtility.RelationOfTwoGrids(enemyBody.OwnerUnit, v.OwnerUnit) != Saber.Camp.CampRelation.hostile)
                    {
                        if (attackDict.ContainsKey(v)) attackDict.Remove(v);
                        Debug.LogWarning("���ܹ����Ѿ�");
                        continue;
                    }
                    if (lastEnemy == null)
                        attackDict.Add(v, enemyBody.OwnerUnit);
                    else
                        attackDict[v] = enemyBody.OwnerUnit;
                    //Debug.Log("���ù���Ŀ��" + v.OwnerUnit.ToString() + "//" + enemyBody.OwnerUnit.ToString());
                    if ((AStarPathfinding2D.GetDistance(myNode, mainSystem.GetGridItemByUnit(enemyBody.OwnerUnit), map) <= v.AttackRange * map.NodeSize))
                    {
                        //ս���ľͿ�ֱ�ӹ���
                        if (unitMoveSystem.IsUnitStand(v.OwnerUnit))
                            AttackAction(v, enemyBody);
                        else
                            //����ͻص���λ��
                            unitMoveSystem.GoBackPos(v.OwnerUnit);

                    }
                    else
                    {
                        LegOrgan leg = v.OwnerUnit.FindOrganInBody<LegOrgan>(ComponentType.leg);
                        //��׷�˲����л���׷��Ŀ�����·��û�ˣ�·��û�˻����ڹ�����Χ�ڣ��ǿ϶�Ҫ������·��
                        if (leg != null && (leg.Follower != enemyBody.OwnerUnit || leg.Path.Count == 0))
                        {
                            unitMoveSystem.UpdatePath(leg, enemyBody.OwnerUnit);
                            //Debug.Log(leg.OwnerUnit+ "���¸���Ŀ��");
                        }
                    }
                }
            }
        }
        //public override void LateUpdate()
        //{
        //    base.LateUpdate();

        //    //foreach(var v in allComponents)
        //    //{
        //    //    //Ѱ�Ҹ���Ĺ�������
        //    //    //Debug.Log(unitPosSystem.GetUnitAction(v.OwnerUnit).ToString()+ UnitMainSystem.ActionPriorityLevel(unitPosSystem.GetUnitAction(v.OwnerUnit)) + "AAA111");
        //    //    if (v!=null&&v.OwnerUnit!=null&&UnitMainSystem.ActionPriorityLevel(unitPosSystem.GetUnitAction(v.OwnerUnit)))
        //    //    {
        //    //        GridItem target = GridUtility.AStarFindRecentlyGrid(unitPosSystem.GetGridItemByUnit(v.OwnerUnit), v.WarningRange, EnemyTest);
        //    //        UnitBase enemy = unitPosSystem.GetUnitByGridItem(target);
        //    //        //˵��һ������Ŀ���
        //    //        if (enemy != null)
        //    //        {
        //    //            BodyOrgan enemyBody = enemy.FindOrganInBody<BodyOrgan>(ComponentType.body);
        //    //            if (!attackDict.TryGetValue(v, out BodyOrgan enemyBody1) || enemyBody1 != enemyBody)
        //    //            {
        //    //                if (!attackDict.TryAdd(v, enemyBody))
        //    //                    attackDict[v] = enemyBody;
        //    //                unitMoveSystem.MoveFollow(v.OwnerUnit.FindOrganInBody<LegOrgan>(ComponentType.leg), enemy, gridMap, v.AttackRange);
        //    //            }
        //    //        }
        //    //    }
        //    //}

        //}
        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
#if UNITY_EDITOR
            if (showAttackRange && !showInSelected)
            {
                foreach (var v in allComponents)
                {
                    if (v != null && v.Owner != null && v.OwnerUnit.transform != null)
                    {
                        Gizmos.color = Color.red;
                        GLUtility.DrawWireDisc(v.OwnerUnit.transform.position, Vector3.forward, v.AttackRange*map.NodeSize);
                        Gizmos.color = Color.yellow;
                        Vector2 mapY = new Vector2(map.startPos.y, map.startPos.y + map.MapHeight);
                        GLUtility.DrawLine(new Vector3(v.OwnerUnit.transform.position.x + v.WarningRange * map.NodeSize, mapY.x), new Vector3(v.OwnerUnit.transform.position.x + v.WarningRange * map.NodeSize, mapY.y), 3);
                        GLUtility.DrawLine(new Vector3(v.OwnerUnit.transform.position.x - v.WarningRange * map.NodeSize, mapY.x), new Vector3(v.OwnerUnit.transform.position.x - v.WarningRange * map.NodeSize, mapY.y), 3);

                    }
                }
            }
#endif

        }

        public override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
#if UNITY_EDITOR

            if (showAttackRange && !showInSelected)
            {
                foreach (var v in allComponents)
                {
                    if (v != null && v.Owner != null && v.OwnerUnit.transform != null)
                    {
                        Gizmos.color = Color.red;
                        GLUtility.DrawWireDisc(v.OwnerUnit.transform.position, Vector3.forward, v.AttackRange * map.NodeSize);
                        Gizmos.color = Color.yellow;
                        Vector2 mapY = new Vector2(map.startPos.y, map.startPos.y + map.MapHeight);
                        GLUtility.DrawLine(new Vector3(v.OwnerUnit.transform.position.x + v.WarningRange*map.NodeSize, mapY.x), new Vector3(v.OwnerUnit.transform.position.x + v.WarningRange * map.NodeSize, mapY.y));
                        GLUtility.DrawLine(new Vector3(v.OwnerUnit.transform.position.x - v.WarningRange*map.NodeSize, mapY.x), new Vector3(v.OwnerUnit.transform.position.x - v.WarningRange * map.NodeSize, mapY.y));

                    }
                }
            }
#endif

        }

        #endregion


        #region ���������ַ�ʽ
        private void Launchrajectory(AttackOrgan t)
        {
            if(t==null||t.OwnerUnit==null||!t.IsHasTrajectory||!attackDict.ContainsKey(t)) { Debug.Log("����һ�δ��󹥻�"); return; }
            BodyOrgan target = attackDict[t].FindOrganInBody<BodyOrgan>(ComponentType.body);
            if (target == null || target.OwnerUnit == null) { Debug.Log("����һ�δ��󹥻�"); return; }
            attackDamageDict.TryGetValue(t, out Damage damage);
            if (damage == null) damage = new Damage(t.OwnerUnit, t.AttackVal, true);
            else attackDamageDict.Remove(t);
            FightLog.Record($"���:{UnitUtility.GetUnitBelongPlayerEnum(t.OwnerUnit)} ��λ:{t.OwnerUnit.gameObject.name},�����˵���{t.Projectile.ModelName};Ŀ��Ϊ ���{CampManager.GetPlayerEnum(target.OwnerPlayer)} ��λ:{target.OwnerUnit.gameObject.name}");
            //Debug.Log((projectileSystem == null).ToString() + "Ͷ����ϵͳΪ�գ���");
            if (t.ProjectileDropPoint == null) t.ProjectileDropPoint = t.OwnerUnit.transform;
            _ = projectileSystem.CreateProjectile(t.Projectile.ModelName, 
                t.ProjectileDropPoint.position,
                1, target.PartBodyDict[2],Vector3.zero,
                t.Projectile.FlySpeed, t.Projectile.Arc, t.Projectile.Curve,
                () => { t.AttackActionOnce?.Invoke();
                    t.AttackActionOnce = null;
                    DamageOther(t, target, damage); },
                () => { return target == null ||
                    target.OwnerUnit == null || 
                    target.UnitAlive == false; },true);
        }
        private void DamageOther(AttackOrgan attackOrgan,BodyOrgan target,Damage damage)
        {
            if (attackOrgan != null&&target!=null)
            {
                
                unitBodySystem.ReceiveDamage(target,damage);
                //�˺�Ŀ��
                //Debug.Log(attackOrgan.ToString() + "��" + enemyBody.ToString()+"����˳ɶ��˺�");
                if (!target.UnitAlive)
                    attackDict.Remove(attackOrgan);
            }
        }
        private void DamageOther(AttackOrgan t)
        {
            if (t != null && attackDict.TryGetValue(t, out UnitBase enemyBody)&&enemyBody!=null)
            {
                BodyOrgan bodyOrgan = enemyBody.FindOrganInBody<BodyOrgan>(ComponentType.body);
                attackDamageDict.TryGetValue(t, out Damage damage);
                if (damage == null) damage = new Damage(t.OwnerUnit, t.AttackVal, true);
                else attackDamageDict.Remove(t);
                unitBodySystem.ReceiveDamage(bodyOrgan, damage);
                //�˺�Ŀ��
                //Debug.Log(attackOrgan.ToString() + "��" + enemyBody.ToString()+"����˳ɶ��˺�");
                if (bodyOrgan==null||!bodyOrgan.UnitAlive)
                    attackDict.Remove(t);
            }
        }
        public void AttackAction(AttackOrgan attackOrgan, BodyOrgan enemyBody)
        {
            if (attackOrgan == null || enemyBody == null) return;
            Debug.Log(attackOrgan.OwnerUnit + "��������" + enemyBody.OwnerUnit);
            mainSystem.UnitTakeAction(attackOrgan.OwnerUnit, FSM_State.attack, IE_Attack(attackOrgan, enemyBody,new Damage(attackOrgan.OwnerUnit,attackOrgan.AttackVal,true)), enemyBody.OwnerUnit);
        }
        #endregion

        //void UnreferenceAll
        public UnitBase FindAttackTarget(AttackOrgan attackOrgan)
        {
            if (attackDict.ContainsKey(attackOrgan)) return attackDict[attackOrgan];
            else return null;
        }

        IEnumerator IE_Attack(AttackOrgan attackOrgan,BodyOrgan enemyBody,Damage damage)
        {
            WaitUntil waitAttackFinish = new WaitUntil(() =>
            {
                return attackOrgan.CharacterFSM.CurrentState != FSM_State.attack;
            });
           


            if (attackOrgan == null || attackOrgan.OwnerUnit == null || enemyBody == null || enemyBody.OwnerUnit == null||damage==null)
            {
                mainSystem.UnitStopAction(attackOrgan.OwnerUnit);
                yield break;
            }

            //float dis=Vector2.Distance(attackOrgan.OwnerUnit.transform.position,enemyBody.OwnerUnit.transform.position);
            //��Ϊ�ϰ���
            mainSystem.UpdateDynamicObstacleReal(mainSystem.GetGridItemByUnit(attackOrgan.OwnerUnit));
            //������λ������
            mainSystem.CorrectionPosition(attackOrgan.OwnerUnit);
            //if (dis > attackOrgan.AttackRange)
            //{
            //    unitPosSystem.UnitStopAction(attackOrgan.OwnerUnit);
            //    yield break;
            //}
            if (attackDamageDict.ContainsKey(attackOrgan)) attackDamageDict.Remove(attackOrgan);
            bool isHitrated = UnitUtility.CalculatePer(attackOrgan.AttackHitrate);
            //�ж�������
            if (!isHitrated)
            {
                InstanceFinder.GetInstance<NormalUtility>().ORPC_ShowRisingSpace("Miss", UnitUtility.GetBodyPos(attackOrgan.OwnerUnit), Vector3.right + Vector3.up, Color.red, 28);
                damage.Val = 0;
                //unitPosSystem.UnitStopAction(attackOrgan.OwnerUnit);
                //yield break;
            }
            else
            {
                //���ܵ��µı���д����
                UnitAttackBefore.Trigger(ID_AttackBefore,attackOrgan, enemyBody, damage);


                //�ж�����
                if (damage.IsCriticalStrike == false && UnitUtility.CalculatePer(attackOrgan.AttackCriticalChance))
                {
                    damage.IsCriticalStrike = true;
                    damage.Val = (int)(damage.Val * attackOrgan.AttackCriticalDamage);
                }
                //��������˺��ٷֱȼ���
                damage.Val =(int) (damage.Val * attackOrgan.CausedDamagePer);
            }


            //ת��
            Vector2 dir=enemyBody.OwnerUnit.transform.position- attackOrgan.OwnerUnit.transform.position;

            int x = dir.x > 0 ? x = 1 : x = -1;
            attackOrgan.AttackState.SetSpeed(attackOrgan.AttackSpeed);
            //attackOrgan.CharacterFSM.Animator.speed = attackOrgan.AttackSpeed;
            attackOrgan.CharacterFSM.SendX(x);
            //���ù���Ŀ��
            attackDamageDict.Add(attackOrgan, damage);

            if (attackDict.ContainsKey(attackOrgan)) attackDict[attackOrgan] = enemyBody.OwnerUnit;
            else attackDict.Add(attackOrgan, enemyBody.OwnerUnit);
            //��ʼ����
            attackOrgan.CharacterFSM.SetCurrentState(FSM_State.attack);
            yield return new WaitForSeconds(Time.deltaTime);
            yield return waitAttackFinish;
            //������ҡ
            if(isHitrated)
                UnitAttackAfter.Trigger(ID_AttackAfter,attackOrgan,enemyBody, damage);
            //ȡ���ϰ���
            mainSystem.UnitStopAction(attackOrgan.OwnerUnit);
            //AttackSystem(v);
        }

        bool EnemyTest(Node self,Node target)
        {
            UnitBase selfBody = mainSystem.GetUnitByGridItem(self);
            UnitBase targetBody = mainSystem.GetUnitByGridItem(target);
            if(selfBody==null||targetBody==null||targetBody.FindOrganInBody<BodyOrgan>(ComponentType.body,false)==null)return false;
            return UnitUtility.RelationOfTwoGrids(selfBody,targetBody) == Saber.Camp.CampRelation.hostile;
        }

    }
}
