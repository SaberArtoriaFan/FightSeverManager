using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.ECS;
using UnityEngine.UIElements;
using XianXia.Terrain;
using static UnityEngine.GraphicsBuffer;
using System.Threading;
using System;
using UnityEngine.EventSystems;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;
using Saber.Camp;

namespace XianXia.Unit
{

    public class UnitMoveSystem :NormalSystemBase<LegOrgan>
    {
        float errorPositionCorrection =0.2f;
        UnitMainSystem mianSystem;
        Saber.ECS.EventSystem eventSystem;
        AStarPathfinding2D map;
        float mid_X=0;
        //Terrain.AStarPathfinding2D map;
        private bool isCalculatingPath;
        private Thread calculateThread;
        SaberEvent<UnitBase, UnityEngine.Vector3> UnitMoveToNodeEvent;
        SaberEvent<UnitBase> UnitFindNoPathEvent;

        //Dictionary<PlayerMemeber, float> playerTargetDict = new Dictionary<PlayerMemeber, float>();
        Queue<LegOrgan> needCalculatePathLegs=new Queue<LegOrgan>();
        //HashSet<LegOrgan> hashSet_NeedCalculatePathLegs = new HashSet<LegOrgan>();
        //public void SetPlayerTarget(PlayerMemeber player, float targetX)
        //{
        //    if (player != null && !playerTargetDict.ContainsKey(player))
        //    {
        //        playerTargetDict.Add(player,targetX);
        //    }
        //}
        int ID_MoveToNode;
        int ID_FindNoPath;
        public override void Awake(WorldBase world)
        {
            base.Awake(world);
            eventSystem = world.FindSystem<Saber.ECS.EventSystem>();
            UnitMoveToNodeEvent= eventSystem.RegisterEvent<UnitBase, UnityEngine.Vector3>(Saber.ECS.EventSystem.EventParameter.UnitMoveToNodeEvent,out ID_MoveToNode);
            UnitFindNoPathEvent = eventSystem.RegisterEvent<UnitBase>(Saber.ECS.EventSystem.EventParameter.UnitFindNoPathEvent,out ID_FindNoPath);
        }

        public override void Start()
        {
            base.Start();
            mianSystem=World.FindSystem<UnitMainSystem>();
            map= UnityEngine.Object.FindObjectOfType<AStarPathfinding2D>();
            eventSystem.GetEvent<BodyOrgan,UnitBase>(Saber.ECS.EventSystem.EventParameter.UnitDeadBefore).AddAction(RemoveFollowTarget);
            //world.StartCoroutine(IE_CalculatePath());
            UpdatePath();
            //Debug.Log(unitPosSystem.ToString());

        }
        public override void Reset()
        {
            base.Reset();
            map.MapReset();
            isCalculatingPath = false;
            world.StartCoroutine(WaitForThreadFinish(() =>
            {
                needCalculatePathLegs.Clear();
                UpdatePath();
            }));
        }
        IEnumerator WaitForThreadFinish(Action action)
        {
            yield return new WaitUntil(() => !calculateThread.IsAlive);
            action?.Invoke();
        }
        private void RemoveFollowTarget(BodyOrgan body, UnitBase damage)
        {
            if (body == null || damage == null ) return;
            LegOrgan leg = damage?.FindOrganInBody<LegOrgan>(ComponentType.leg);
            if (leg != null &&leg.Enable==true&&leg.OwnerUnit!=null&& leg.Follower==body.OwnerUnit)
            {
                UpdatePath(leg, null);
            }
        }

        //bool IsNeedReroute(UnitBase unit, GridItem node)
        //{
        //    if (unitPosSystem.GetUnitByGridItem(node)==unit) return false;
        //    if (!node.canMove || node.chessStandLimit || node.collisionLimit || node.edgeLimit)
        //        return true;
        //    return false;
        //}
        //bool IsNeedReroute(GridItem node)
        //{
        //    if (!node.canMove || node.chessStandLimit || node.collisionLimit || node.edgeLimit)
        //        return true;
        //    return false;
        //}
        //public static bool IsFaceTo(Vector3 forward, Vector3 dir, float limit = 0)
        //{
        //    //60����ǰ��
        //    float x = Vector3.Dot(forward.normalized, dir.normalized);
        //    //Debug.Log(x + "asas");
        //    return x >= limit;
        //}
        //public void MoveTo(LegOrgan leg,Vector2 pos,GridMap map)
        //{
        //    //Debug.Log(leg == null);
        //    if (!leg.IsCanMove) return;
        //    UnitBase unit = leg.OwnerUnit;
        //    //a
        //    GridItem start = GridMap.GetGridByWorldPosition(unit.transform.position, map);
        //    GridItem end = GridMap.GetGridByWorldPosition(pos, map);
        //    //Debug.Log(start.position + "//" + end.position);
        //    Stack<GridItem> path;
        //    GridUtility.AStarPathFinding(out path, start, end);
        //    ActionStatus actionStatus = new ActionStatus(ActionStatus.ActionStatusType.low);
        //    unitPosSystem.UnitTakeAction(unit, FSM_State.walk,IE_ChessMoveToAction(leg, path,actionStatus),actionStatus);
        //}
        //IEnumerator IE_ChessMoveToAction(LegOrgan moveChess, Stack<GridItem> path,ActionStatus actionStatus)
        //{
        //    UnitBase chess = moveChess.OwnerUnit;
        //    if (path == null || path.Count <= 1)
        //    {
        //        unitPosSystem.UnitStopAction(chess);
        //        yield break;
        //    }
        //    CharacterFSM characterFSM = moveChess.OwnerUnit.GetComponent<CharacterFSM>();
        //    GridItem node = null;
        //    GridItem pre = path.Pop();
        //    GridItem origin = pre;
        //    Vector3 dir = Vector3.zero;
        //    Vector3 temDir = Vector3.zero;
        //    GridMap map = pre.gridMap;
        //    WaitForSeconds waitForOne = new WaitForSeconds(Time.deltaTime);

        //    //GridMap.UpdateZone(moveChess, 3, 1, pre.gridMap, false);

        //    float dis = 0;
        //    Vector3 tempDir;
        //    //�ж��Ƿ����ƶ������б�����ռ�˸���
        //    bool isRestartPath = false;

        //    WaitUntil waitMove = new WaitUntil(() =>
        //    {
        //        if (IsNeedReroute(chess, node))
        //        {
        //            isRestartPath = true;
        //            while (path.Count > 0)
        //                node = path.Pop();
        //            GridUtility.AStarPathFinding(out path, pre, node);
        //            return true;
        //        }
        //        int x = dir.x > 0 ? x = 1 : x = -1;
        //        characterFSM.SendX(x);
        //        //chess.transform.rotation = Quaternion.Euler(Vector3.up * 180 * x);

        //        chess.transform.Translate(dir * moveChess.MoveSpeed*0.01f * Time.deltaTime, Space.World); ;
        //        tempDir = (node.position - new Vector2(chess.transform.position.x, chess.transform.position.y)).normalized;
        //        //���һ��������
        //        dis = Vector2.Distance(node.position, new Vector2(chess.transform.position.x, chess.transform.position.y));
        //        return dis < 0.1f || Vector2.Angle(tempDir, dir) > (Mathf.PI - 0.1f);
        //    });

        //    characterFSM.SetCurrentState(FSM_State.walk);

        //    while (path.Count > 0)
        //    {
        //        actionStatus.StatusType = ActionStatus.ActionStatusType.low;
        //        if (isRestartPath)
        //        {
        //            isRestartPath = false;
        //            if (path.Count > 1)
        //                pre = path.Pop();
        //            else
        //                break;
        //        }
        //        node = path.Pop();
        //        if (path.Count==0 && IsNeedReroute(node))
        //            break;
        //        //�����ƶ�����:
        //        dir = (node.position - new Vector2(chess.transform.position.x, chess.transform.position.y)).normalized;
        //        //GridMap.UpdateZone(moveChess, size.x, size.y, node.gridMap, true);
        //        if (IsNeedReroute(chess, node))
        //        {
        //            isRestartPath = true;
        //            while (path.Count > 0)
        //                node = path.Pop();
        //            GridUtility.AStarPathFinding(out path, pre, node);
        //            continue;
        //        }
        //        pre = node;
        //        //�ƶ���ȥ
        //        actionStatus.StatusType = ActionStatus.ActionStatusType.high;

        //        yield return waitMove;
        //        if (isRestartPath)
        //            continue;
        //        unitPosSystem.AddUnitToPos(chess, node);
        //        yield return waitForOne;

        //    }

        //    characterFSM.SetCurrentState(FSM_State.idle);
        //    unitPosSystem.UnitStopAction(chess);
        //    //GridMap.UpdateZone(moveChess, size.x, size.y, node.gridMap, true);
        //    //Debug.Log("����Ŀ�����" + node.Id);
        //}
        //public void MoveFollow(LegOrgan moveChess, UnitBase follower, GridMap map,float range=1)
        //{
        //    if (moveChess == null || follower == null) return;
        //    if (!moveChess.IsCanMove) return;
        //    UnitBase unit = moveChess.OwnerUnit;

        //    GridItem start = unitPosSystem.GetGridItemByUnit(unit);
        //    GridItem end = unitPosSystem.GetGridItemByUnit(follower);
        //    Stack<GridItem> path;
        //    GridUtility.AStarPathFinding(out path, start, end,range);
        //    if (path != null && path.Count >= 1)
        //    {
        //        ActionStatus actionStatus = new ActionStatus(ActionStatus.ActionStatusType.low);
        //        unitPosSystem.UnitTakeAction(unit, FSM_State.walk, IE_MoveFollow(moveChess, follower, path, end, range,actionStatus),actionStatus);

        //    }

        //}

        ////�ƶ����ܷ���Э����
        //IEnumerator IE_MoveFollow(LegOrgan moveChess,UnitBase follower,Stack<GridItem> path,GridItem end,float range=1,ActionStatus actionStatus=null)
        //{
        //    if (moveChess == null || follower == null || path == null || path.Count <= 1) { unitPosSystem.UnitStopAction(moveChess.OwnerUnit);yield break; }
        //    CharacterFSM characterFSM = moveChess.OwnerUnit.GetComponent<CharacterFSM>();
        //    UnitBase chess = moveChess.OwnerUnit;
        //    GridItem node = null;
        //    GridItem pre = path.Pop();
        //    GridItem origin = pre;
        //    Vector3 dir = Vector3.zero;
        //    Vector3 temDir = Vector3.zero;
        //    GridMap map = pre.gridMap;
        //    GridItem target;
        //    //GridMap.UpdateZone(moveChess, 3, 1, pre.gridMap, false);

        //    float dis = 0;

        //    Vector3 tempDir;
        //    //�ж��Ƿ����ƶ������б�����ռ�˸���
        //    bool isRestartPath = false;
        //    WaitUntil waitMove = new WaitUntil(() =>
        //    {
        //        if (follower == null) return true;
        //        if (IsNeedReroute(chess, node))
        //        {
        //            isRestartPath = true;
        //            while (path.Count > 0)
        //                node = path.Pop();
        //            GridUtility.AStarPathFinding(out path, pre, node);
        //            return true;
        //        }
        //        int x = dir.x > 0 ? x = 1 : x = -1;
        //        characterFSM.SendX(x);

        //        chess.transform.Translate(dir * moveChess.MoveSpeed * 0.01f * Time.deltaTime, Space.World); ;
        //        tempDir = (node.position - new Vector2(chess.transform.position.x, chess.transform.position.y)).normalized;
        //        //���һ��������
        //        dis = Vector2.Distance(node.position, new Vector2(chess.transform.position.x, chess.transform.position.y));
        //        if (node == end)
        //            return Vector2.Distance(chess.transform.position,follower.transform.position)<range;
        //        else
        //            return dis < 0.1f || Vector2.Angle(tempDir, dir) > (Mathf.PI - 0.1f);
        //    });
        //    WaitForSeconds waitForOne = new WaitForSeconds(Time.deltaTime);
        //    characterFSM.SetCurrentState(FSM_State.walk);

        //    while (path.Count > 0)
        //    {
        //        actionStatus.StatusType = ActionStatus.ActionStatusType.low;
        //        if (isRestartPath)
        //        {
        //            isRestartPath = false;
        //            if (path.Count > 1)
        //                pre = path.Pop();
        //            else
        //                break;
        //        }
        //        node = path.Pop();
        //        if (node == end && IsNeedReroute(node))
        //            break;
        //        //�����ƶ�����:
        //        dir = (node.position - new Vector2(chess.transform.position.x, chess.transform.position.y)).normalized;
        //        //GridMap.UpdateZone(moveChess, size.x, size.y, node.gridMap, true);
        //        //�ж�·�����Ƿ����ϰ���
        //        if (IsNeedReroute(chess, node))
        //        {
        //            yield return waitForOne;
        //            if (IsNeedReroute(chess, node))
        //            {
        //                GridUtility.AStarPathFinding(out path, pre, end, range);
        //                isRestartPath = true;
        //                continue;
        //            }
        //        }
        //        pre = node;
        //        //�ƶ���ȥ
        //        actionStatus.StatusType = ActionStatus.ActionStatusType.high;
        //        yield return waitMove;
        //        if (isRestartPath)
        //            continue;

        //        unitPosSystem.AddUnitToPos(chess, node);
        //        //�ж�Ŀ���Ƿ��ƶ��ˣ��ƶ��˾����¼���·��
        //        target = unitPosSystem.GetGridItemByUnit(follower);
        //        if(target == null)break;
        //        if (target!= end)
        //        {
        //            GridUtility.AStarPathFinding(out path, node, target, range);
        //            isRestartPath=true;
        //            end = target;
        //        }
        //        yield return waitForOne;
        //    }

        //    characterFSM.SetCurrentState(FSM_State.idle);
        //    unitPosSystem.UnitStopAction(chess);
        //    //GridMap.UpdateZone(moveChess, size.x, size.y, node.gridMap, true);
        //    Debug.Log("����Ŀ�����" + node.Id);
        //}
        public void StopAll()
        {
            LegOrgan v;
            for (int i = 0; i < allComponents.Count; i++)
            {
                v= allComponents[i];
                v.CanFindPath = false;
                v.Path.Clear();
                v.CharacterFSM.SetCurrentState(FSM_State.idle);
            }
        }
        public override void Update()
        {
            base.Update();
            LegOrgan v;
            for(int i=0;i<allComponents.Count;i++)
            {
                v = allComponents[i];
                if (v != null&&v.OwnerUnit!=null)
                {
                    //��������
                    if (v.Enable == false) continue;
                    //��ѣ����
                    if (v.OwnerUnit.Enable == false) continue;
                    FSM_State fSM_State = mianSystem.GetUnitAction(v.OwnerUnit);
                    if (UnitMainSystem.ActionPriorityLevel(fSM_State))
                    {
                        //Debug.Log(fSM_State + "f//" + v.Follower + "aa" + v.IsStand + "qq" + v.Path.Count);
                        //Debug.Log(v.OwnerUnit.gameObject.name + "QWE"+allComponents.Count);
                        if (fSM_State == FSM_State.idle && v.Follower == null && v.IsStand && v.Path.Count==0&&v.CharacterFSM.CurrentState==FSM_State.idle&& Mathf.Abs(mid_X - v.OwnerUnit.transform.position.x) > map.NodeSize)
                            UpdatePath(v, null);
                        if(UnitMainSystem.ActionPriorityLevel(v.CharacterFSM.CurrentState))
                            UnitsMoveSystem(v);
                    }
                }
            }
        }

        public void SetUnitLeg(LegOrgan leg, UnitBase target)
        {
            if (leg == null ) return;
            //Debug.Log("111");
            mianSystem.UpdateDynamicObstacle(mianSystem.GetGridItemByUnit(leg.OwnerUnit), false);
            LegOrgan otherLeg = leg.Follower!=null? leg.Follower.FindOrganInBody<LegOrgan>(ComponentType.leg):null;
           

            DeletePosFromFlowField(leg);
            //leg.LastPathLength=
            if (target != null&&leg.CurrentPathIndex<leg.Path.Count)
            {
                Vector3 pos = leg.Path[leg.CurrentPathIndex];
                leg.Path.Clear();
                leg.Path.Add(pos);
            }else
                leg.Path.Clear();
            leg.CurrentPathIndex = 0;
            leg.Follower = target;
            leg.Pos = mianSystem.GetPosByUnit(leg.OwnerUnit);
            leg.FollowerPos = leg.HasFollower ? mianSystem.GetPosByUnit(leg.Follower) : Vector3.zero;

            if (otherLeg!=null&& target != otherLeg.OwnerUnit &&otherLeg.Follower==leg.OwnerUnit)
                UpdatePath(otherLeg, otherLeg.Follower);
            //if (IsFollowEachOther(leg))
            //    leg.IsFollowEachOther = true;
            //else
            //    leg.IsFollowEachOther = false;
            //leg.IsStand = true;
        }
        private void DeletePosFromFlowField(LegOrgan leg)
        {
            if (leg == null || leg.pathG == null || leg.pathG.Count == 0) return;
            float f;
            Node node=null;
            int time = leg.Path.Count - leg.pathG.Count;
            while (leg.pathG.Count > 0 && time < leg.Path.Count)
            {
                f = leg.pathG.Dequeue();
                if(leg.Path.Count>time)
                    node = AStarPathfinding2D.GetWorldPositionNode(leg.Path[time], map);
                if (node != null)
                    AStarPathfinding2D.DeletePosFromFlowField(node, f, map);
                time++;
            }
            leg.pathG.Clear();
        }

        void UnitsMoveSystem(LegOrgan leg)
        {
            if (leg == null || leg.OwnerUnit == null || leg.Path == null) return;

            if (leg.Path.Count == 0)
            {

                // ����Ƿ���վ��ԭ�ز�������û�е����յ�Ҳ�����ڵȸ�����
                if (( leg.Follower != null && !IsFollowEachOther(leg)&&Vector3.Distance(leg.FollowerPos, mianSystem.GetPosByUnit(leg.Follower)) > errorPositionCorrection))
                {
                    Debug.Log(leg.OwnerUnit + "//"+leg.Follower+"�������ƶ���·���Ƿ���Ҫ���£���");
                    UpdatePath(leg, leg.Follower);
                    return;
                }
                //���վ����û��Ҫ�����·���ˣ����Ҷ���״̬�����߾Ͱ�״̬�ĵ�
                if (leg.IsStand&& leg.IsFinding==false && leg.CharacterFSM.CurrentState==FSM_State.walk)
                    leg.CharacterFSM.SetCurrentState(FSM_State.idle);
                leg.IsStand = true;
                //ͣ�����˵�ȻҪ�����ϰ���
                mianSystem.UpdateDynamicObstacleReal(mianSystem.GetGridItemByUnit(leg.OwnerUnit));
                return;
            }
            //if(path.Count)
            UnityEngine.Transform transform =leg.OwnerUnit.transform;
            Vector3 targetPos = leg.Path[leg.CurrentPathIndex];
            Vector3 destinationPos = leg.Path[leg.Path.Count - 1];
            Node node = AStarPathfinding2D.GetWorldPositionNode(targetPos, map);
            Vector3 dir = targetPos - transform.position;

            int x = dir.x > 0 ? x = 1 : x = -1;
            leg.CharacterFSM.SendX(x);
            leg.CharacterFSM.SetCurrentState(FSM_State.walk);
            // ���Ƶ�λ�ƶ��ͳ���
            transform.Translate(dir.normalized * leg.MoveSpeed * 0.01f * Time.deltaTime, Space.World);
            //transform.position += dir.normalized * leg.MoveSpeed * 0.01f * Time.deltaTime;
            float distance = Vector3.Distance(transform.position, targetPos);
            Node myNode = mianSystem.GetGridItemByUnit(leg.OwnerUnit);


            if (distance < 0.1f)
            {
                // ����Ŀ��㣬�л�����һ��Ŀ���
                //����ȫ�ָ�������λ��
                leg.IsStand = true;
                leg.OwnerUnit.transform.position = targetPos;
                if(leg.pathG.Count>0)
                    AStarPathfinding2D.DeletePosFromFlowField(node, leg.pathG.Dequeue(), map);

                leg.CurrentPathIndex++;
                _=mianSystem.AddUnitToPos(leg.OwnerUnit, node);

                // ����Ƿ���Ҫ����·��
                if ((!IsFollowEachOther(leg)&&leg.Follower != null && Vector3.Distance(leg.FollowerPos, mianSystem.GetPosByUnit(leg.Follower)) > errorPositionCorrection))
                {
                    Debug.Log(leg.OwnerUnit+"//"+leg.Follower+ "�������ƶ���·���Ƿ���Ҫ���£���");
                    UpdatePath(leg, leg.Follower);
                    return;
                }
                if (leg.CurrentPathIndex == leg.Path.Count)
                {
                    if (leg.Follower == null)
                    {
                        UpdatePath(leg, null); return;
                    }
                    else
                    {
                        DeletePosFromFlowField(leg);
                        leg.Path.Clear();
                        leg.CurrentPathIndex = 0;
                    }
                }
                else if (leg.CurrentPathIndex > leg.Path.Count)
                {
                    Debug.Log("��ǰ��·��·��������Ҫ���£���");
                    leg.CharacterFSM.SetCurrentState(FSM_State.idle);
                    UpdatePath(leg, leg.Follower);
                    return;
                }
            }
            else
                leg.IsStand = false;

            mianSystem.UpdateDynamicObstacleReal(myNode, false);


            if (leg.CurrentPathIndex<leg.Path.Count&&!IsPathValid(AStarPathfinding2D.GetWorldPositionNodePos(leg.Path[leg.CurrentPathIndex], map), leg.OwnerUnit))
            {

                Debug.Log(leg.OwnerUnit+"���ϰ��·���Ƿ���Ҫ���£���");
                UpdatePath(leg, leg.Follower);
                return;
            }
        }
        internal bool IsUnitStand(UnitBase unitBase)
        {
            if (unitBase == null) return false;
            LegOrgan leg = unitBase.FindOrganInBody<LegOrgan>(ComponentType.leg);
            //Node node=unitPosSystem.GetGridItemByUnit(unitBase);
            //Vector3 pos = AStarPathfinding2D.GetNodeWorldPositionV3(node.Position, map);
            //return Vector3.Distance(pos, unitBase.transform.position)<0.1f;

            if(leg == null) return false;
            return leg.IsStand;
        }
        /// <summary>
        /// ����λ��
        /// </summary>
        /// <param name="unitBase"></param>
        internal void GoBackPos(UnitBase unitBase)
        {
            if (unitBase == null) return;
            LegOrgan leg = unitBase.FindOrganInBody<LegOrgan>(ComponentType.leg);
            if (leg == null) return;
            SetUnitLeg(leg, leg.Follower);
            leg.Path.Add(mianSystem.GetPosByUnit(unitBase));
        }

        internal void UpdatePath(LegOrgan leg,UnitBase target)
        {
            if (leg == null || leg.CanFindPath == false ||leg.IsFinding) return;
            //if (obu.ContainsKey(leg.OwnerUnit))
            //{
            //    Node node = obu[leg.OwnerUnit];
            //    obu.Remove(leg.OwnerUnit);
            //    if (ob.ContainsKey(node) && ob[node]==leg.OwnerUnit)
            //        ob.Remove(node);
            //}
            //Debug.Log("QQQ");
            SetUnitLeg(leg, target);
            leg.IsFinding = true;
            needCalculatePathLegs.Enqueue(leg);
            try
            {
                //Debug.Log(leg.OwnerUnit.name);
                //Debug.Log(needCalculatePathLegs.Count);
                //Debug.Log(needCalculatePathLegs.Count);
                //Debug.Log(leg + "//" + target);
                //Debug.Log("WWW");


                //Debug.Log(needCalculatePathLegs.Contains(leg));

            }
            catch(Exception ex)
            {
                Debug.LogError(ex.Message);

            }

        }
        private void UpdatePath()
        {     
            if (!isCalculatingPath)
            {
                isCalculatingPath = true;
                calculateThread = new Thread(CalculatePath);
                calculateThread.Start();
            }
        }

        private void CalculatePath()
        {
            isCalculatingPath = true;
            while (isCalculatingPath)
            {

                while (needCalculatePathLegs.Count > 0)
                {

                    //Debug.Log(111);
                    LegOrgan leg = needCalculatePathLegs.Peek();
                    if (leg == null || leg.OwnerUnit == null || leg.Path.Count >1) { _ = needCalculatePathLegs.Dequeue(); continue; }
                    AttackOrgan attackOrgan = leg.OwnerUnit.FindOrganInBody<AttackOrgan>(ComponentType.attack);
                    float range = 0;
                    //Debug.Log(leg);
                    // ������·��֮ǰ������վ�·��

                    Vector3 target;
                    lock (leg)
                    {
                        //Ŀ���ѱ�ռ��
                        if (leg.Follower != null)
                        {
                            target = leg.FollowerPos;
                            float waitRnage = 1.5f;
                            if (attackOrgan != null)
                                waitRnage = attackOrgan.AttackRange * map.NodeSize;

                            if (AStarPathfinding2D.GetDistance(mianSystem.GetGridItemByUnit(leg.Follower), mianSystem.GetGridItemByUnit(leg.OwnerUnit), map) < waitRnage)
                            {
                                //unitPosSystem.AddUnitToPos(leg.OwnerUnit, node);

                                //DeletePosFromFlowField(leg);
                                //if(leg.Path.Count>1)
                                //{
                                //    Vector3 pos = leg.Path[0];
                                //    leg.Path.Clear();
                                //    leg.Path.Add(pos);
                                //    leg.CurrentPathIndex = 0;
                                //}
                                Debug.Log("���ڹ�����Χ�ڣ������ٹ滮·��");
                                leg.IsFinding = false;
                                _ = needCalculatePathLegs.Dequeue();
                                continue;



                            }
                            else
                            {
                                range = 0;
                                //����ǻ�����沢�ҶԷ������ж��У��ͺͶԷ�Լ����·��
                                if (UnitMainSystem.ActionPriorityLevel(mianSystem.GetUnitAction(leg.Follower)) && IsFollowEachOther(leg))
                                {
                                    LegOrgan otherLeg = leg.Follower.FindOrganInBody<LegOrgan>(ComponentType.leg);

                                    if (leg.IsStand == true && otherLeg != null && otherLeg.Path.Count <= 1)
                                    {
                                        Node disNode = otherLeg.Path.Count > 0 ? AStarPathfinding2D.GetWorldPositionNode(otherLeg.Path[otherLeg.Path.Count - 1], map) : mianSystem.GetGridItemByUnit(otherLeg.OwnerUnit);
                                        if (AStarPathfinding2D.GetDistance(disNode, mianSystem.GetGridItemByUnit(leg.OwnerUnit), map) <= attackOrgan.AttackRange * map.NodeSize)
                                        {
                                            _ = needCalculatePathLegs.Dequeue();
                                            leg.IsFinding = false;
                                            continue;
                                        }
                                    }

                                    Node a = mianSystem.GetGridItemByUnit(leg.OwnerUnit);
                                    Node b = mianSystem.GetGridItemByUnit(leg.Follower);

                                    //AttackOrgan otherAttackOrgan = leg.Follower.FindOrganInBody<AttackOrgan>(ComponentType.attack);
                                    //float dis = AStarPathfinding2D.GetDistance(a, b, map);
                                    //float aDis = dis - otherAttackOrgan.AttackRange - attackOrgan.AttackRange;

                                    Vector2Int remainder = new Vector2Int((a.Position.x + b.Position.x) % 2, (a.Position.y + b.Position.y) % 2);
                                    Vector2Int mid = (a.Position + b.Position) / 2;
                                    Vector3 bpos = AStarPathfinding2D.GetNodeWorldPositionV3(b.Position, map);
                                    bool isNeedSwap = CompareRight(a, b, remainder);
                                    target = AStarPathfinding2D.GetNodeWorldPositionV3(mid, map);

                                    Node other = AStarPathfinding2D.FindClosestNodeByDir(mid, b.Position - mid, waitRnage, map);

                                    lock (otherLeg)
                                    {
                                        SetUnitLeg(otherLeg, otherLeg.Follower);
                                        otherLeg.Path = Terrain.AStarPathfinding2D.FindingPathWorldPos(bpos, AStarPathfinding2D.GetNodeWorldPositionV3(other.Position, map), map, leg.Follower, out otherLeg.pathG, GridType.Ground, 0);
                                        otherLeg.CurrentPathIndex = 0;
                                    }
                                    //Set
                                }
                                else if (attackOrgan != null)
                                    range = attackOrgan.AttackRange;
                            }


                        }
                        else
                        {
                            //����ͣס
                            if (Mathf.Abs(mid_X - leg.Pos.x) < map.NodeSize)
                            {
                                DeletePosFromFlowField(leg);
                                leg.Path.Clear();
                                leg.CurrentPathIndex = 0;
                                _ = needCalculatePathLegs.Dequeue();
                                leg.IsFinding = false;
                                if (attackOrgan != null && attackOrgan.OwnerUnit != null)
                                    attackOrgan.WarningRange = 1000;
                                continue;


                            }
                            target = new Vector3(leg.Pos.x, leg.Pos.y, 0) + map.NodeSize * 2 * (Vector3.right * mid_X - Vector3.right * leg.Pos.x).normalized;
                            //Ŀ�ĵز����߾ͻ�Ŀ�ĵ�
                            if (!AStarPathfinding2D.IsWalkable(target, map))
                            {
                                Node node = AStarPathfinding2D.FindNearestNode(mianSystem.GetGridItemByUnit(leg.OwnerUnit), AStarPathfinding2D.GetWorldPositionNode(target, map), map, true);
                                if (node != null)
                                    target = AStarPathfinding2D.GetNodeWorldPositionV3(node.Position, map);
                                else
                                {
                                    _ = needCalculatePathLegs.Dequeue();
                                    leg.IsFinding = false;
                                    continue;
                                }
                            }
                        }
                        //Debug.Log(leg.Pos);
                        //Ѱ·
                        leg.Path = Terrain.AStarPathfinding2D.FindingPathWorldPos(leg.Pos, target, map, leg.OwnerUnit, out leg.pathG, GridType.Ground, range);
                        leg.CurrentPathIndex = 0;
                        //if (leg.Path.Count > 0) unitPosSystem.UpdateDynamicObstacleReal(unitPosSystem.GetGridItemByUnit(leg.OwnerUnit), false);
                        _ = needCalculatePathLegs.Dequeue();
                        if (leg.Path.Count == 0)
                            UnitFindNoPathEvent.Trigger(ID_FindNoPath,leg.OwnerUnit);
                        leg.IsFinding = false;
                        //Debug.Log(leg.Path.Count);
                    }

                    //try
                    //{


                    //}
                    //catch (Exception ex)
                    //{
                    //    Debug.LogError(ex.Message);
                    //}

                }
            }

            try
            {

                //yield return new WaitForSeconds(Time.deltaTime);
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
        
        //IEnumerator IE_CalculatePath()
        //{
        //    isCalculatingPath = true;
        //    while (isCalculatingPath)
        //    {

        //        while (needCalculatePathLegs.Count > 0)
        //        {
        //            //Debug.Log(111);
        //            LegOrgan leg = needCalculatePathLegs.Peek();
        //            if (leg == null || leg.OwnerUnit == null || leg.Path.Count != 0) { _ = needCalculatePathLegs.Dequeue(); continue; }
        //            AttackOrgan attackOrgan = leg.OwnerUnit.FindOrganInBody<AttackOrgan>(ComponentType.attack);
        //            float range = 0;
        //            //Debug.Log(leg);
        //            // ������·��֮ǰ������վ�·��

        //            Vector3 target;
        //            lock (leg)
        //            {
        //                //Ŀ���ѱ�ռ��
        //                if (leg.Follower != null)
        //                {
        //                    target = leg.FollowerPos;
        //                    float waitRnage = 1.5f;
        //                    if (attackOrgan != null)
        //                        waitRnage = attackOrgan.AttackRange;

        //                    if (AStarPathfinding2D.GetDistance(unitPosSystem.GetGridItemByUnit(leg.Follower), unitPosSystem.GetGridItemByUnit(leg.OwnerUnit), map) < waitRnage)
        //                    {
        //                        //unitPosSystem.AddUnitToPos(leg.OwnerUnit, node);

        //                        //DeletePosFromFlowField(leg);
        //                        //if(leg.Path.Count>1)
        //                        //{
        //                        //    Vector3 pos = leg.Path[0];
        //                        //    leg.Path.Clear();
        //                        //    leg.Path.Add(pos);
        //                        //    leg.CurrentPathIndex = 0;
        //                        //}
        //                        Debug.Log("���ڹ�����Χ�ڣ������ٹ滮·��");
        //                        _ = needCalculatePathLegs.Dequeue();
        //                        continue;



        //                    }
        //                    else
        //                    {
        //                        range = 0;
        //                        //����ǻ�����沢�ҶԷ������ж��У��ͺͶԷ�Լ����·��
        //                        if (UnitMainSystem.ActionPriorityLevel(unitPosSystem.GetUnitAction(leg.Follower)) && IsFollowEachOther(leg))
        //                        {
        //                            Node a = unitPosSystem.GetGridItemByUnit(leg.OwnerUnit);
        //                            Node b = unitPosSystem.GetGridItemByUnit(leg.Follower);

        //                            float dis= AStarPathfinding2D.GetDistance(a, b, map);
        //                            Vector2Int remainder = new Vector2Int((a.Position.x + b.Position.x) % 2, (a.Position.y + b.Position.y) % 2);
        //                            Vector2Int mid = (a.Position + b.Position) / 2;
        //                            Vector3 bpos = AStarPathfinding2D.GetNodeWorldPositionV3(b.Position, map);
        //                            bool isNeedSwap = CompareRight(a, b, remainder);
        //                            target = AStarPathfinding2D.GetNodeWorldPositionV3(mid, map);

        //                            Node other = AStarPathfinding2D.FindClosestNodeByDir(mid, b.Position - mid, waitRnage, map);
        //                            //Vector3 enemyTarget = target + ( bpos- target).normalized * map.NodeSize;
        //                            LegOrgan otherLeg = leg.Follower.FindOrganInBody<LegOrgan>(ComponentType.leg);
        //                            //if (otherLeg != null &&)
        //                            lock (otherLeg)
        //                            {
        //                                SetUnitLeg(otherLeg, otherLeg.Follower);
        //                                otherLeg.Path = Terrain.AStarPathfinding2D.FindingPathWorldPos(bpos, AStarPathfinding2D.GetNodeWorldPositionV3(other.Position, map), map, leg.Follower, out otherLeg.pathG, GridType.Ground, 0);
        //                                otherLeg.CurrentPathIndex = 0;
        //                            }
        //                            //Set
        //                        }
        //                        else if (attackOrgan != null)
        //                            range = attackOrgan.AttackRange;
        //                    }


        //                }
        //                else
        //                {
        //                    //����ͣס
        //                    if (Mathf.Abs(mid_X - leg.Pos.x) < map.NodeSize)
        //                    {

        //                        DeletePosFromFlowField(leg);
        //                        leg.Path.Clear();
        //                        leg.CurrentPathIndex = 0;
        //                        _ = needCalculatePathLegs.Dequeue();

        //                        continue;


        //                    }
        //                    target = new Vector3(mid_X, leg.Pos.y, 0);
        //                    //Ŀ�ĵز����߾ͻ�Ŀ�ĵ�
        //                    if (!AStarPathfinding2D.IsWalkable(target, map))
        //                    {
        //                        Node node = AStarPathfinding2D.FindNearestNode(unitPosSystem.GetGridItemByUnit(leg.OwnerUnit), AStarPathfinding2D.GetWorldPositionNode(target, map), map, true);
        //                        if (node != null)
        //                            target = AStarPathfinding2D.GetNodeWorldPositionV3(node.Position, map);
        //                        else
        //                        {
        //                            _ = needCalculatePathLegs.Dequeue();
        //                            continue;
        //                        }
        //                    }
        //                }
        //                //Debug.Log(leg.Pos);
        //                //Ѱ·
        //                leg.Path = Terrain.AStarPathfinding2D.FindingPathWorldPos(leg.Pos, target, map, leg.OwnerUnit, out leg.pathG, GridType.Ground, range);
        //                leg.CurrentPathIndex = 0;

        //                _ = needCalculatePathLegs.Dequeue();

        //                Debug.Log(leg.Path.Count);
        //            }

        //            //try
        //            //{


        //            //}
        //            //catch (Exception ex)
        //            //{
        //            //    Debug.LogError(ex.Message);
        //            //}

        //        }
        //        yield return new WaitForSeconds(Time.deltaTime);
        //    }
        //}

        public bool IsFollowEachOther(LegOrgan leg)
        {

            if (leg == null||leg.OwnerUnit==null ) return false;
            UnitBase target = leg.Follower;
            if (target == null) return false;
            if (leg.Follower!=target)return false;
            LegOrgan otherLeg = target.FindOrganInBody<LegOrgan>(ComponentType.leg);
            if(otherLeg == null) return false;
            if (otherLeg.Follower != leg.OwnerUnit) return false;
            return true;
        }
        /// <summary>
        /// ������Ϻ��a���е����
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="reamiander"></param>
        private bool CompareRight(Node a,Node b,Vector2Int reamiander)
        {
            bool res = false;
            void Swap()
            {
                res = true;
            }
            if(reamiander.x==0&&reamiander.y==0) return res;
            Vector2Int dir = a.Position - b.Position;
            //dir.x>0˵��a��b���ҷ���dir.y>0˵��a��b���Ϸ�
            if (reamiander.x != 0 && reamiander.y != 0)
            {
                if (dir.x >= 0 && dir.y >= 0) Swap();
                else if (dir.x <= 0 && dir.y <= 0) return res;
                else if (dir.x > 0 && Mathf.Abs(dir.x) - Mathf.Abs(dir.y) > 0) Swap();
                else if (dir.y > 0 && Mathf.Abs(dir.y) - Mathf.Abs(dir.x) > 0) Swap();
            }
            else if (reamiander.x != 0)
            {
                if (dir.x > 0) { Swap(); }
            }else
            {
                if(dir.y>0) { Swap(); }
            }
            return res;
        }
        [Obsolete]
        private bool IsPathValid(Vector3 from,Vector3 to)
        {
            return Terrain.AStarPathfinding2D.IsPathValid(from, to, map, false);
        }
        private bool IsPathValid(Vector2Int pos,UnitBase unit)
        {
            if (!AStarPathfinding2D.IsTerrainable(pos, map)) return false;
            Node node=AStarPathfinding2D.GetNode(pos, map);
            if(node==null)return false;
            if (mianSystem.GetUnitByGridItem(node) == null) return true;
            if (mianSystem.GetUnitByGridItem(node) != unit) return false;
            return true;
        }
        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            foreach (var leg in allComponents)
            {
                if (leg != null && leg.OwnerUnit != null && leg.Path.Count > 0)
                {
                    Gizmos.color = Color.blue;
                    Vector3 pre = leg.OwnerUnit.transform.position;
                    for(int i=leg.CurrentPathIndex; i<leg.Path.Count; i++)
                    {
                        Gizmos.DrawLine(pre, leg.Path[i]);
                        pre = leg.Path[i];
                    }
                }
            }
        }
        public override void OnDestory()
        {
            base.OnDestory();
            isCalculatingPath = false;
        }
    }
}
