
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XianXia.Terrain;
using static UnityEngine.UI.CanvasScaler;

namespace XianXia
{
    //public class UnitMovement : MonoBehaviour
    //{
    //    public Collider2D aStarPathTrigger;
    //    public List<Node> standGridItem = new List<Node>();
    //    CharacterFSM characterFSM;
    //    float speed = 1.5f;
    //    Coroutine task;
    //    float avoidRange = 1.5f;
    //    public Transform feetPoint;
    //    LayerMask unitLayer;
    //    [SerializeField]
    //    Vector2Int size;
    //    float attackRange;
    //    UnitMovement attackTarget;
    //    int healthPoint;
    //    public CharacterFSM CharacterFSM { get => characterFSM; }

    //    void Start()
    //    {
    //        characterFSM = GetComponent<CharacterFSM>();
    //        characterFSM.AddState(new FSM.Walk(GetComponent<Animator>()));
    //        characterFSM.AddState(new FSM.Attack(GetComponent<Animator>(),60*Time.deltaTime, 0.2f, Damage));
    //        Debug.Log(30 * Time.deltaTime);
    //        aStarPathTrigger = GetComponent<Collider2D>();
    //        feetPoint = transform.Find("FootCenter");
    //        //GridMap.UpdateZone(this, size.x, size.y, FindObjectOfType<GridMap>(), true);

    //    }
    //    //interactionTrigger.OverlapCollider(ContactFilterManager.Instance.InteractiveContactFilter, findColliders);
    //    bool IsNeedReroute()
    //    {
    //        RaycastHit2D[] hits = new RaycastHit2D[5];

    //        int reslut = aStarPathTrigger.Raycast(aStarPathTrigger.transform.forward, hits, avoidRange, unitLayer);
    //        Debug.Log("碰撞检测——" + reslut);
    //        for (int i = 0; i < hits.Length; i++)
    //        {
    //            if (hits[i] == default || hits[i].collider.transform == aStarPathTrigger.transform) continue;
    //            return true;
    //        }
    //        return false;
    //    }
    //    bool IsNeedReroute(Node node)
    //    {
    //        if (standGridItem.Contains(node)) return false;
    //        if (!node.canMove || node.chessStandLimit || node.collisionLimit || node.edgeLimit)
    //            return true;
    //        return false;
    //    }
    //    public static bool IsFaceTo(Vector3 forward, Vector3 dir, float limit = 0)
    //    {
    //        //60度正前方
    //        float x = Vector3.Dot(forward.normalized, dir.normalized);
    //        //Debug.Log(x + "asas");
    //        return x >= limit;
    //    }
    //    IEnumerator IE_ChessMoveToAction(UnitMovement moveChess, Stack<Node> path)
    //    {
    //        if (path == null || path.Count <= 1) yield break;
    //        CharacterFSM characterFSM = moveChess.GetComponent<CharacterFSM>();
    //        GameObject chess = moveChess.gameObject;
    //        Node node = null;
    //        Node pre = path.Pop();
    //        Node origin = pre;
    //        Vector3 dir = Vector3.zero;
    //        Vector3 temDir = Vector3.zero;
    //        //GridMap.UpdateZone(moveChess, 3, 1, pre.gridMap, false);

    //        float dis = 0;
    //        Vector3 tempDir;

    //        WaitUntil waitMove = new WaitUntil(() =>
    //        {
    //            int x = dir.x > 0 ? x = 1 : x = -1;
    //            characterFSM.SendX(x);

    //            chess.transform.Translate(dir * speed * Time.deltaTime, Space.World); ;
    //            tempDir = (node.position - new Vector2(chess.transform.position.x, chess.transform.position.y)).normalized;
    //            //添加一个地面检测
    //            dis = Vector2.Distance(node.position, new Vector2(transform.position.x, transform.position.y));
    //            return dis < 0.1f || Vector2.Angle(tempDir, dir) > (Mathf.PI - 0.1f);
    //        });

    //        characterFSM.SetCurrentState(global::FSM_State.walk);

    //        while (path.Count > 0)
    //        {
    //            node = path.Pop();
    //            //计算移动方向:
    //            dir = (node.position - new Vector2(chess.transform.position.x, chess.transform.position.y)).normalized;
    //            pre = node;
    //            //GridMap.UpdateZone(moveChess, size.x, size.y, node.gridMap, true);
    //            if (moveChess.IsNeedReroute(node))
    //            {
    //                while (path.Count > 0)
    //                    node = path.Pop();
    //                moveChess.MoveTo(node.position);
    //                yield break;
    //            }
    //            //移动过去
    //            yield return waitMove;


    //        }

    //        characterFSM.SetCurrentState(FSM_State.idle);
    //        //GridMap.UpdateZone(moveChess, size.x, size.y, node.gridMap, true);
    //        Debug.Log("到达目标格子" + node.Id);
    //    }
    //    public void Attack()
    //    {
    //        characterFSM.SetCurrentState(FSM_State.attack);
    //        if (task != null)
    //            StopCoroutine(task);
    //        task = null;
    //    }
    //    public void Damage()
    //    {
    //        if (attackTarget == null) return;
    //        Debug.Log(this.name+"攻击了"+attackTarget.name);
    //        //attackTarget.healthPoint -= 10;
    //    }
    //    public void Attack(UnitMovement unit,out bool res)
    //    {
    //        res = false;
    //        if (Vector3.Distance(unit.transform.position, transform.position) > attackRange) return;
    //        attackTarget= unit;
    //        characterFSM.SetCurrentState(FSM_State.attack);
    //        if (task != null)
    //            StopCoroutine(task);
    //        task = null;
    //    }
    //    bool IsCanMove => characterFSM.CurrentState == FSM_State.idle || characterFSM.CurrentState == FSM_State.walk;

    //    //public void MoveTo(Vector2 pos)
    //    //{
    //    //    if (!IsCanMove) return;
    //    //    AStarPathfinding2D map = GameObject.FindObjectOfType<AStarPathfinding2D>();
    //    //    Node start = AStarPathfinding2D.GetGridByWorldPosition(transform.position, map);
    //    //    Node end = AStarPathfinding2D.GetGridByWorldPosition(pos, map);
    //    //    Debug.Log(start.position + "//" + end.position);
    //    //    Stack<Node> path;
    //    //    GridUtility.AStarPathFinding(out path, start,end);
    //    //    if(task!=null)
    //    //        StopCoroutine(task);
    //    //    task= StartCoroutine(IE_ChessMoveToAction(this, path));
    //    //}

        
    //}
}
