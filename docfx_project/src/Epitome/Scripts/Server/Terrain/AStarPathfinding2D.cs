
using Saber.Base;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace XianXia.Terrain
{

    [Serializable]
    public class Node
    {
        [SerializeField]
        private Vector2Int position;
        public Vector2Int Position { get=>position;  }

        public Node(Vector2Int position)
        {
            this.position = position;
        }
    }
    public interface IDynamicObstacle
    {
        Transform transform { get;}
    }

    public class AStarPathfinding2D:MonoBehaviour
    {
        
        private class NodeDerivant : IComparable<NodeDerivant>
        {
            public NodeDerivant(Node node)
            {
                Node = node;
            }
            public NodeDerivant()
            {

            }

            public Node Node { get; set; }
            public Node Parent { get; set; }
            /// <summary>
            /// 起点的花销
            /// </summary>
            public float GCost { get; set; }
            /// <summary>
            /// 和终点的花销
            /// </summary>
            public float HCost { get; set; }
            public float FCost { get { return GCost + HCost; } }
            public int CompareTo(NodeDerivant other)
            {
                int compare = FCost.CompareTo(other.FCost);
                if (compare == 0)
                    compare = HCost.CompareTo(other.HCost);
                return -compare;
            }
            public void Reset()
            {
                GCost = 0;
                HCost=0;
                Parent = null;
                Node = null;
            }
        }
        public bool ShowGrid = true;

        public Node[] allNodeArray;
        //public Node[][] allNodes;
        [SerializeField]
        [HideInInspector]
        List<Vector2Int> fixed_obstacleList;

        HashSet<Vector2Int> fixed_obstacleHash;

        HashSet<Vector2Int> dynamic_obstacleHash;
        List<Transform> dynamicObstacle;
        //bool 
        [HideInInspector]
        [SerializeField]
        //地图左下角起始点
        public Vector3 startPos;
        // 地图宽度
        public int MapWidth;
        // 地图高度
        public int MapHeight;
        // 节点间距
        public float NodeSize = 1f;
        //节点半间距
        // 节点间距
        public float NodeHalfSize = 0.5f;
        // 起点和终点相连时的代价
        public float ConnectedCost  = 1f;
        // 对角线移动时的额外代价
        public float DiagonalCost = 1.41421356f;
        [SerializeField]
        private int mapX;
        [SerializeField]
        private int mapY;
        [SerializeField]
        private LayerMask obstacleLayer;

        private List<int[]> findDir = new List<int[]>() { new int[2] { 1, 0 }, new int[] { 0, 1 }, new int[] { -1, 0 }, new int[] { 0, -1 },new int[] { -1, -1 },new int[] {-1,1 },new int[] {1,1 },new int[] {1,-1 } };

        Dictionary<Node, Dictionary<float, IDynamicObstacle>> fieldFlew=new Dictionary<Node, Dictionary<float, IDynamicObstacle>>();

        public int MapX { get => mapX; }
        public int MapY { get => mapY;  }
        private void Start()
        {
            InitDynamicObstacle();
            InitFixedObstacle();
        }

        public int[][] UseDir()
        {
            int[][] res = findDir.ToArray();
            //int[] temp = findDir[0];
            //findDir.RemoveAt(0);
            //findDir.Add(temp);
            return res;
        }
        [Button]
        void DrawMap()
        {
            startPos = this.transform.position;

            mapX = (int)(MapWidth / NodeSize);
            mapY= (int)(MapHeight / NodeSize);
            allNodeArray = new Node[mapY * mapX];

            //allNodes =new Node[mapX][];
            fixed_obstacleList = new List<Vector2Int>();
            //fixed_obstacleHash=new HashSet<Vector2Int>();
            //dynamic_obstacleHash = new HashSet<Vector2Int>();
            NodeHalfSize = NodeSize / 2;
            int num = 0;
            for(int i = 0; i < mapX; i++)
            {
                ///allNodes[i] = new Node[mapY];
                for(int j = 0; j < mapY; j++)
                {
                    allNodeArray[num++] = new Node(Vector2Int.right * i + Vector2Int.up * j);
                    //allNodes[i][j] = new Node(Vector2Int.right*i+Vector2Int.up*j);
                }
            }
            ScanObstable();
        }
        [Button]
        private void ScanObstable()
        {
            foreach (var grid in allNodeArray)
            {

                    Collider2D[] collider2Ds = new Collider2D[8];
                    int res = Physics2D.OverlapCircleNonAlloc(GetNodeWorldPositionV3(grid.Position, this), NodeHalfSize, collider2Ds, obstacleLayer);
                    //Debug.Log("QQQ"+res);

                    foreach (var v in collider2Ds)
                    {
                        if (v == null || v.isTrigger) continue;
                        //Debug.Log("QQQ");
                        if (!fixed_obstacleList.Contains(grid.Position))
                            fixed_obstacleList.Add(grid.Position);
                        break;
                    }
                

            }
        }
        private void Update()
        {
            //
        }
       public void MapReset()
        {
            dynamicObstacle.Clear();
            fieldFlew.Clear();
        }
        public static Node FindClosestNodeByDir(Node node,Vector2Int dir,float range, AStarPathfinding2D map)
        {
            return FindClosestNodeByDir(node.Position, dir, range,map);
        }
        public static Node FindClosestNodeByDir(Vector2Int node, Vector2Int dir,float range ,AStarPathfinding2D map)
        {
            if (range <= 1) range = 1.2f;
            bool isHor = Mathf.Abs(dir.x) > Mathf.Abs(dir.y);
            if (dir.x > 0) dir.x = 1;
            else if (dir.x < 0) dir.x = -1;
            if (dir.y > 0) dir.y = 1;
            else if (dir.y < 0) dir.y = -1;
            Vector2Int pos = node + dir;
            if (GetDistance(node.x, node.y, pos.x, pos.y, map) <= range) return GetNode(pos, map);
            if (!isHor)
                return FindClosestNodeByDir(node, Vector2Int.up * dir.y, range, map);
            else
                return FindClosestNodeByDir(node, Vector2Int.right * dir.x, range, map);
        }
        public static void DeletePosFromFlowField(Node node,float f,AStarPathfinding2D map)
        {
            if (!map.fieldFlew.ContainsKey(node)) return;
            if (!map.fieldFlew[node].ContainsKey(f)) return;
            map.fieldFlew[node].Remove(f);
        }
        protected  void InitDynamicObstacle()
        {
            dynamicObstacle = new List<Transform>();
            dynamic_obstacleHash = new HashSet<Vector2Int>();
            //IDynamicObstacle[] obstacles=GameObject.FindObjectsOfType<IDynamicObstacle>();
        }
        protected void InitFixedObstacle()
        {
            fixed_obstacleHash = new HashSet<Vector2Int>();
            foreach(var v in fixed_obstacleList)
            {
                fixed_obstacleHash.Add(v);
            }
        }
        public static void UpdateDynamicObstacle(Vector2Int pos, AStarPathfinding2D map, bool isAdd = true)
        {
            if (isAdd)
            {
                if (!map.dynamic_obstacleHash.Contains(pos))
                    map.dynamic_obstacleHash.Add(pos);
            }
            else
            {
                if (map.dynamic_obstacleHash.Contains(pos))
                    map.dynamic_obstacleHash.Remove(pos);
            }

        }
        public static void UpdateDynamicObstacle(Transform tr,AStarPathfinding2D map,bool isAdd=true)
        {
            if (isAdd)
            {
                if (!map.dynamicObstacle.Contains(tr))
                    map.dynamicObstacle.Add(tr);
            }
            else
            {
                if (map.dynamicObstacle.Contains(tr))
                    map.dynamicObstacle.Remove(tr);
            }

        }
        public static void UpdateDynamicObstacleList(AStarPathfinding2D map)
        {
            Vector2Int pos;
            map.dynamic_obstacleHash.Clear();
            foreach (var o in map.dynamicObstacle)
            {
                pos = GetWorldPositionNodePos(o.transform.position, map);
                if (!map.dynamic_obstacleHash.Contains(pos))
                    map.dynamic_obstacleHash.Add(pos);
            }
        }

        private static int CompareToH(Node a,Node b,Dictionary<Node,NodeDerivant> dict)
        {
            if (a == null) return -1;
            if(b == null) return 1;
            return dict[a].CompareTo(dict[b]);
        }
        private static int CompareToDis(Node a, Node b, Dictionary<Node, float> dict)
        {
            if (a == null) return -1;
            if (b == null) return 1;
            return dict[a].CompareTo(dict[b]);


        }
        /// <summary>
        /// 用来检测终点是否可达的，必须是完全可行走
        /// </summary>
        /// <param name="center"></param>
        /// <param name="target"></param>
        /// <param name="map"></param>
        /// <param name="gridType"></param>
        /// <returns></returns>
        public static Node FindNearestNode(Node center,Node target,AStarPathfinding2D map, bool isUpDowmFrist=false,float range=float.MaxValue )
        {
            if (target == null) return null;
            //if(ContasinsSelf&&IsWalkable())

            List<Node> openList = new List<Node>();
            HashSet<Node> closeList = new HashSet<Node>();
            openList.Add(target);
            int listCount=1;
            int time = 0;
            Node node,temp;
            int[,] findDir = new int[,] { { -1, 0 }, { 0, -1 }, { 0, 1 }, { 1, 0 },{ 1,1},{1,-1 },{-1,-1 },{-1,1 } };
            if(isUpDowmFrist)
                findDir = new[,] { {0, -1}, { 0, 1 }, { 1, 0 }, { -1, 0 }, { 1, 1 }, { 1, -1 }, { -1, -1 }, { -1, 1 } };
            List<NodeDerivant> listDer = new List<NodeDerivant>();
            Dictionary<Node, NodeDerivant>  nodeDerivvantDict = new Dictionary<Node, NodeDerivant>();
            NodeDerivant nodeDerivant=new NodeDerivant(target);
             nodeDerivvantDict.Add(target, nodeDerivant);
            while (openList.Count > 0)
            {
                //openList.Sort((a, b) => { return CompareToH(a, b, nodeDerivvantDict); });
                listCount = openList.Count;
                time++;
                for (int i = 0; i < listCount; i++)
                {
                    node = openList[0];

                    for (int j = 0; j < findDir.GetLength(0); j++)
                    {
                        int x = node.Position.x + findDir[j, 0];
                        int y = node.Position.y + findDir[j, 1];
                        if (!IsPosValid(x, y, map)) continue;
                        Vector2Int pos = Vector2Int.right * x + Vector2Int.up * y;
                        if (!IsWalkable(pos, map)) continue;
                        float dis = GetDistance(x, y, target.Position.x, target.Position.y, map);
                        //if (dis <= 1.2f * map.NodeSize) return AStarPathfinding2D.GetNode(pos, map);
                        if (range <dis) continue;
                        temp = AStarPathfinding2D.GetNode(pos, map);
                        float newGCost = nodeDerivvantDict[node].GCost + GetDistance(temp, node, map);

                        if (!nodeDerivvantDict.ContainsKey(temp))
                        {
                            nodeDerivant = new NodeDerivant(temp);
                            nodeDerivant.Parent = node;
                            nodeDerivvantDict.Add(temp, nodeDerivant);
                            nodeDerivant.GCost = newGCost;
                            nodeDerivant.HCost = GetDistance(temp, center, map);
                        }
                        else if (newGCost < nodeDerivvantDict[temp].GCost)
                        {
                            nodeDerivvantDict[temp].GCost = newGCost;
                            nodeDerivvantDict[temp].Parent = node;

                        }
                        else
                            continue;
                        listDer.Add(nodeDerivvantDict[temp]);


                        if (!closeList.Contains(temp))
                        {
                            closeList.Add(temp);

                        }
                        if (!openList.Contains(temp))
                            openList.Add(temp);
                    }

                    openList.RemoveAt(0);
                }
                if(listDer.Count > 0)
                {
                    listDer.Sort();
                    return listDer[0].Node;
                }
            }
            return null;
        }
        //[Obsolete]
        public static List<Node> FindNearestNode(Node center, float range, int num,Func<Node,Node,bool> test , AStarPathfinding2D map,bool isTerrainObstale,bool isUnitObstale,bool containsSelf=true)
        {
            List<Node> res = new List<Node>();
            if (center == null) return null;
            if (containsSelf)
            {
                if ((!isTerrainObstale || !IsTerrainable(center.Position, map)) && (!isUnitObstale || !IsUnitable(center.Position, map)) && test(center, center)) res.Add(center);
            }
            if (res.Count >= num) return res;
            List<Node> openList = new List<Node>();
            HashSet<Node> closeList = new HashSet<Node>();
            openList.Add(center);
            int listCount = 1;
            Node node, temp;
            int[,] findDir = new int[,] { { -1, 0 }, { 0, -1 }, { 0, 1 }, { 1, 0 }, { 1, 1 }, { 1, -1 }, { -1, -1 }, { -1, 1 } };
            while (openList.Count > 0)
            {
                //openList.Sort((a, b) => { return CompareToH(a, b, nodeDerivvantDict); });
                listCount = openList.Count;
                for (int i = 0; i < listCount; i++)
                {
                    node = openList[0];

                    for (int j = 0; j < findDir.GetLength(0); j++)
                    {
                        int x = node.Position.x + findDir[j, 0];
                        int y = node.Position.y + findDir[j, 1];
                        if (!IsPosValid(x, y, map)) continue;

                        Vector2Int pos = Vector2Int.right * x + Vector2Int.up * y;
                        //只有这行不同
                        if (GetDistance(pos.x,pos.y,center.Position.x,center.Position.y,map) > range) continue;
                        temp = GetNode(pos, map);
                        if (!closeList.Contains(temp))
                        {
                            closeList.Add(temp);
                            if (!openList.Contains(temp))
                                openList.Add(temp);
                        }


                        //计算路径，单位和地形都要过滤
                        if (isTerrainObstale && !IsTerrainable(pos, map)) continue;
                        if (isUnitObstale && !IsUnitable(pos, map)) continue;

                        if (test(center, temp))
                        {
                            res.Add(temp);
                            if (res.Count >= num) return res;
                        }


                    }



                    openList.RemoveAt(0);
                }

            }
            return null;
        }

        /// <summary>
        /// 与上个函数唯一不同就是range表示x轴距离
        /// </summary>
        /// <param name="center"></param>
        /// <param name="range"></param>
        /// <param name="test"></param>
        /// <param name="map"></param>
        /// <param name="isTerrainObstale"></param>
        /// <param name="isUnitObstale"></param>
        /// <param name="containsSelf"></param>
        /// <returns></returns>
        public static Node FindNearestNodeInAttackRange(Node center, float range, Func<Node, Node, bool> test, AStarPathfinding2D map, bool isTerrainObstale, bool isUnitObstale, bool containsSelf = true)
        {
            if (center == null) return null;
            range *= map.NodeSize;
            if (containsSelf)
            {
                if (!(isTerrainObstale && !IsTerrainable(center.Position, map)) && !(isUnitObstale && !IsUnitable(center.Position, map)) && test(center, center)) return center;
            }
            List<Node> openList = new List<Node>();
            HashSet<Node> closeList = new HashSet<Node>();
            //Dictionary<Node, NodeDerivant> nodeDerivvantDict=new Dictionary<Node, NodeDerivant>();
            //nodeDerivvantDict.Add(center, new NodeDerivant(center));
            openList.Add(center);
            closeList.Add(center);
            int listCount = 1;
            Node node, temp;
            int[,] findDir = new int[,] { { -1, 0 }, { 0, -1 }, { 0, 1 }, { 1, 0 }, { 1, 1 }, { 1, -1 }, { -1, -1 }, { -1, 1 } };
            List<Node>res = new List<Node>();
            while (openList.Count > 0)
            {
                //openList.Sort((a, b) => { return CompareToH(a, b, nodeDerivvantDict); });
                listCount = openList.Count;
                for (int i = 0; i < listCount; i++)
                {
                    //openList.Sort((a, b) => { return CompareToH(a, b, nodeDerivvantDict); });

                    node = openList[0];

                    for (int j = 0; j < findDir.GetLength(0); j++)
                    {
                        int x = node.Position.x + findDir[j, 0];
                        int y = node.Position.y + findDir[j, 1];
                        if (!IsPosValid(x, y, map)) continue;

                        Vector2Int pos = Vector2Int.right * x + Vector2Int.up * y;
                        //只有这行不同
                        if (Mathf.Abs(x-center.Position.x)*map.NodeSize > range) continue;
                        temp = GetNode(pos, map);
                        if (temp == null) continue;
                        if (closeList.Contains(temp)) continue;
                        closeList.Add(temp);


         

                        //计算路径，单位和地形都要过滤
                        if (isTerrainObstale && !IsTerrainable(pos, map)) continue;
                        if (isUnitObstale && !IsUnitable(pos, map)) continue;
                        if (test(center, temp))
                        {
                            //float newGCost = nodeDerivvantDict[node].GCost + GetDistance(node, temp, map);
                            res.Add(temp);

                        }
                        if (!openList.Contains(temp))
                                openList.Add(temp);



                    }



                    openList.RemoveAt(0);
                }

            }
            if (res.Count == 0)
                return null;
            //for (int i = 0; i < res.Count; i++)
            //{
            //    dict.Add(res[i], GetDistance(res[i], center,map));
            //}
            //res.Sort((a, b) => { return CompareToDis(a, b, nodeDerivvantDict); });
            return res[0];
        }
        
        public static List<Node> FindTargetsInRange(Node center,float range,Func<Node,Node,bool>test,AStarPathfinding2D map, bool isTerrainObstale, bool isUnitObstale, bool containsSelf = true,int num=-1)
        {
            List<Node> res = new List<Node>();
            if (center == null) return null;
            if (containsSelf)
            {
                if (!(isTerrainObstale && !IsTerrainable(center.Position, map)) && !(isUnitObstale && !IsUnitable(center.Position, map)) && test(center, center)) res.Add(center);
            }
            List<Node> openList = new List<Node>();
            HashSet<Node> closeList = new HashSet<Node>();
            //Dictionary<Node, float> dict = new Dictionary<Node, float>();
            openList.Add(center);
            closeList.Add(center);
            int listCount = 1;
            Node node, temp;
            int[,] findDir = new int[,] { { -1, 0 }, { 0, -1 }, { 0, 1 }, { 1, 0 }, { 1, 1 }, { 1, -1 }, { -1, -1 }, { -1, 1 } };
            while (openList.Count > 0)
            {
                //openList.Sort((a, b) => { return CompareToH(a, b, nodeDerivvantDict); });
                listCount = openList.Count;
                for (int i = 0; i < listCount; i++)
                {
                    node = openList[0];

                    for (int j = 0; j < findDir.GetLength(0); j++)
                    {
                        int x = node.Position.x + findDir[j, 0];
                        int y = node.Position.y + findDir[j, 1];
                        if (!IsPosValid(x, y, map)) continue;

                        Vector2Int pos = Vector2Int.right * x + Vector2Int.up * y;
                        //只有这行不同
                        if (GetDistance(pos.x,pos.y,center.Position.x,center.Position.y,map) > range) continue;
                        temp = GetNode(pos, map);
                        if (temp == null) continue;
                        if (closeList.Contains(temp)) continue;
                        closeList.Add(temp);



                        //计算路径，单位和地形都要过滤
                        if (isTerrainObstale && !IsTerrainable(pos, map)) continue;
                        if (isUnitObstale && !IsUnitable(pos, map)) continue;
                        //Debug.Log("tempRRR");

                        if (test(center, temp))
                        { res.Add(temp);if (num > 0 && res.Count >= num) return res; }
                        if (!openList.Contains(temp))
                            openList.Add(temp);



                    }



                    openList.RemoveAt(0);
                }

            }
            //if (res.Count == 0)
            //    return null;

            //res
            return res;
        }
        public static Node FindTargetInRange(Node center, float range, Func<Node, Node, bool> filtration, Func<Node,Node,int>sortFunc, AStarPathfinding2D map, bool isTerrainObstale, bool isUnitObstale, bool containsSelf = true)
        {
            if(sortFunc== null)sortFunc=(a,b)=> { return 0; };
            List<Node> nodes = FindTargetsInRange(center, range, filtration, map, isTerrainObstale, isUnitObstale, containsSelf);
            if(nodes.Count== 0) return null;    
            nodes.Sort((a,b)=> { return (int)sortFunc(a, b) ; });
            return nodes[0];
        }
        static Node FinderNearestCanReachNode(Node oldCenter,HashSet<Node> nodes,AStarPathfinding2D map)
        {
            List<Node> list = nodes.ToList();
            list.Sort((a, b) =>
            {
                return GetDistance(a, oldCenter, map).CompareTo(GetDistance(b, oldCenter, map));
            });
            if (list.Count > 0) return list[0];
            else return null;
        }
        public static List<Vector2Int> FindingPath(Vector3 startPos,Vector3 endPos, AStarPathfinding2D map,IDynamicObstacle unit, out Queue<float> pathG, GridType gridType=GridType.Ground, float stopRange = 0)
        {
            pathG = new Queue<float>();
            stopRange *= map.NodeSize;
            if(stopRange>0&& stopRange <= map.NodeSize)return new List<Vector2Int>();
            if (map == null) { Debug.LogError("map is null!"); return new List<Vector2Int>(); }
            if (startPos == endPos) return new List<Vector2Int>();
            Node startNode = GetWorldPositionNode(startPos, map);
            //Debug.Log("StartNode" + startNode.Position);
            Node endNode = GetWorldPositionNode(endPos, map);
            if (startNode == null || endNode == null) { Debug.LogError("start or endNode is null!"); return new List<Vector2Int>(); }
            if (startNode == endNode) return new List<Vector2Int>();
            if(stopRange>=GetDistance(startNode,endNode,map))return new List<Vector2Int>();
            if (!AStarPathfinding2D.IsWalkable(endNode.Position, map))
            {
                //更换终点
                //Debug.Log("更换终点");
                Node realEndNode = FindNearestNode(startNode, endNode, map, false, stopRange > 0 ? stopRange : float.MaxValue);
                if (realEndNode == null&&stopRange>0)
                    realEndNode = FindNearestNode(startNode, endNode, map, false, float.MaxValue);
                if (realEndNode == null)
                {
                    Debug.LogError("无路可走");
                    return new List<Vector2Int>();
                }
                endNode = realEndNode;
                return FindingPath(startPos, GetNodeWorldPositionV3(endNode.Position, map), map, unit, out pathG, gridType);
            }
            List<Node> openList = new List<Node>();
            HashSet<Node>closeList=new HashSet<Node>();
            Dictionary<Node, NodeDerivant> nodeDerivvantDict = new Dictionary<Node, NodeDerivant>();

            Node node,temp;
            NodeDerivant nodeDerivant;
            int listCount;
            bool isFindEnd = false;
            //Vector3 cost = new Vector3();
            //HashSet<Vector2Int> obstacleHash = map.fixed_obstacleHash;
            int[][] findDir = map.UseDir();
            //检测终点是否可抵达

            openList.Add(startNode);
            nodeDerivvantDict.Add(startNode, new NodeDerivant(startNode));
            while (openList.Count > 0)
            {
                openList.Sort((a, b) => { return CompareToH(a, b, nodeDerivvantDict); }) ;
                listCount = openList.Count;
                node = openList[0];
                if (stopRange > 0 && GetDistance(node, endNode, map) < stopRange)
                {
                    //Debug.Log("aaa");
                    isFindEnd = true;
                    endNode = node;
                }
                else
                {
                    for (int i = 0; i < listCount; i++)
                    {
                        node = openList[0];


                        for (int j = 0; j < findDir.GetLength(0); j++)
                        {
                            int x = node.Position.x + findDir[j][0];
                            int y = node.Position.y + findDir[j][1];
                            if (!IsPosValid(x, y, map)) continue;
                            Vector2Int pos = Vector2Int.right * x + Vector2Int.up * y;
                            if (!IsWalkable(pos, map)) continue;
                            //Debug.Log(x + "***" + y);
                            temp = GetNode(pos, map);

                            float newGCost = nodeDerivvantDict[node].GCost + GetDistance(node, temp, map);

                            //查询流场里是否已占据这个点
                            if (map.fieldFlew.ContainsKey(temp) && map.fieldFlew[temp].ContainsKey(newGCost)) continue;

                            if (temp == endNode)
                                isFindEnd = true;

                            if (!nodeDerivvantDict.ContainsKey(temp))
                            {
                                nodeDerivant = new NodeDerivant(temp);
                                nodeDerivant.Parent = node;
                                nodeDerivvantDict.Add(temp, nodeDerivant);
                                nodeDerivant.GCost = newGCost;
                                nodeDerivant.HCost = GetDistance(temp, endNode, map);
                            }
                            else if (newGCost < nodeDerivvantDict[temp].GCost)
                            {
                                nodeDerivvantDict[temp].GCost = newGCost;
                                nodeDerivvantDict[temp].Parent = node;

                            }
                            else
                                continue;
                            if (!closeList.Contains(temp))
                            {
                                closeList.Add(temp);
                            }
                            if (!openList.Contains(temp))
                                openList.Add(temp);

                        }
                        openList.RemoveAt(0);
                    }
                }

                //判断是否是终点
                if (isFindEnd)
                {
                    return GeneratePath(endNode, nodeDerivvantDict,unit,map,out pathG);
                }
            }
            //从找过的表里找个最靠近目标的
            return GeneratePath(FinderNearestCanReachNode(endNode, closeList, map), nodeDerivvantDict, unit, map, out pathG);
        }
        public static List<Vector3> FindingPathWorldPos(Vector3 startPos, Vector3 endPos, AStarPathfinding2D map,IDynamicObstacle unit, out Queue<float> pathG, GridType gridType = GridType.Ground, float stopRange = 0)
        {
            pathG = null;
            var res = FindingPath(startPos, endPos, map, unit,out pathG,gridType,stopRange);
            //Debug.Log(res.Count + "111");
            return res.Select(U => GetNodeWorldPositionV3(U, map)).ToList();
        }
        private static List<Vector2Int> GeneratePath(Node endNode, Dictionary<Node, NodeDerivant> nodeDerivvantDict,IDynamicObstacle dynamicObstacle,AStarPathfinding2D map,out Queue<float> pathG)
        {
            pathG = new Queue<float>();
            if (endNode == null) return new List<Vector2Int>();
            List<Node> res=new List<Node>();
            NodeDerivant nodeDerivant = nodeDerivvantDict[endNode];
            while (nodeDerivant.Parent != null)
            {
                res.Add(nodeDerivant.Node);
                nodeDerivant = nodeDerivvantDict[nodeDerivant.Parent];
            }
            res.Reverse();

            foreach (var v in res)
            {
                if (!map.fieldFlew.ContainsKey(v))
                    map.fieldFlew.Add(v, new Dictionary<float, IDynamicObstacle>());
                if (!map.fieldFlew[v].ContainsKey(nodeDerivvantDict[v].GCost))
                    map.fieldFlew[v].Add(nodeDerivvantDict[v].GCost, dynamicObstacle);
                pathG.Enqueue(nodeDerivvantDict[v].GCost);
            }
            nodeDerivvantDict.Clear();
            //Debug.Log("寻路结束" + res.Count);
            return res.Select(u=>u.Position).ToList();
        }

        public static Vector2 GetNodeWorldPositionV2(Vector2Int pos,AStarPathfinding2D aStarPathfinding2D)
        {
            float x = aStarPathfinding2D.startPos.x + aStarPathfinding2D.NodeHalfSize + pos.x * aStarPathfinding2D.NodeSize;
            float y = aStarPathfinding2D.startPos.y + aStarPathfinding2D.NodeHalfSize + pos.y * aStarPathfinding2D.NodeSize;
            return Vector2.right*x+Vector2.up*y;
        }
        public static Vector3 GetNodeWorldPositionV3(Vector2Int pos, AStarPathfinding2D aStarPathfinding2D)
        {
            float x = aStarPathfinding2D.startPos.x + aStarPathfinding2D.NodeHalfSize + pos.x * aStarPathfinding2D.NodeSize;
            float y = aStarPathfinding2D.startPos.y + aStarPathfinding2D.NodeHalfSize + pos.y * aStarPathfinding2D.NodeSize;
            return Vector3.right * x + Vector3.up * y;
        }
        public static Vector2Int GetWorldPositionNodePos(Vector3 pos, AStarPathfinding2D aStarPathfinding2D)
        {
            int x = (int)((pos.x - aStarPathfinding2D.startPos.x) / aStarPathfinding2D.NodeSize);
            int y = (int)((pos.y - aStarPathfinding2D.startPos.y) / aStarPathfinding2D. NodeSize);
            //int z = Mathf.RoundToInt(worldPosition.z / NodeSize);

            return Vector2Int.right * x + Vector2Int.up * y;
        }
        public static Node GetWorldPositionNode(Vector3 pos, AStarPathfinding2D aStarPathfinding2D)
        {
            int x = (int)((pos.x - aStarPathfinding2D.startPos.x) / aStarPathfinding2D.NodeSize);
            int y = (int)((pos.y - aStarPathfinding2D.startPos.y) / aStarPathfinding2D.NodeSize);
            //Debug.Log(x + "---" + y);
            //Debug.Log(aStarPathfinding2D.allNodeArray.Length+"s");
            return GetNode(x, y, aStarPathfinding2D);
        }
        public static Node GetNode(Vector2Int pos, AStarPathfinding2D aStarPathfinding2D)
        {
            return GetNode(pos.x, pos.y,aStarPathfinding2D);
        }
        public static Node GetNode(int x,int y, AStarPathfinding2D aStarPathfinding2D)
        {
            if (IsPosValid(x, y, aStarPathfinding2D))
            {
                Node node= aStarPathfinding2D.allNodeArray[x * aStarPathfinding2D.mapY + y];
                //Debug.Log(node.Position + "wqe");
                return node;
            }
            else
                return null;
        }
        public static bool IsPosValid(int x,int y, AStarPathfinding2D aStarPathfinding2D)
        {
            return x >= 0 && x < aStarPathfinding2D.mapX && y >= 0 && y < aStarPathfinding2D.mapY;
        }
        public static bool IsTerrainable(Vector2Int pos, AStarPathfinding2D aStarPathfinding2D)
        {
            return !aStarPathfinding2D.fixed_obstacleHash.Contains(pos);
        }
        public static bool IsUnitable(Vector2Int pos, AStarPathfinding2D aStarPathfinding2D)
        {
            return !aStarPathfinding2D.dynamic_obstacleHash.Contains(pos);
        }
        public static bool IsWalkable(Vector3 pos, AStarPathfinding2D aStarPathfinding2D)
        {
            Vector2Int posInt = GetWorldPositionNodePos(pos, aStarPathfinding2D);
            return IsWalkable(posInt,aStarPathfinding2D);
        }
        public static bool IsWalkable(Vector2Int pos, AStarPathfinding2D aStarPathfinding2D)
        {
            return IsTerrainable(pos, aStarPathfinding2D) && IsUnitable(pos, aStarPathfinding2D);
        }
        [Obsolete]
        public static bool IsPathValid(Vector2Int target, AStarPathfinding2D map)
        {
            return IsWalkable(target, map);
        }
        [Obsolete]
        public static bool IsPathValid(Vector3 from,Vector3 to,AStarPathfinding2D map, bool containsSelf = false)
        {
            Vector2Int start = GetWorldPositionNodePos(from, map);
            Vector2Int end=GetWorldPositionNodePos(to, map);
            if (IsPosValid(start.x, start.y, map) && IsPosValid(end.x, end.y, map))
                return HasObstacleBetween(start, end, map,containsSelf);
            else
                return false;
        }
        [Obsolete]
        private static bool HasObstacleBetween(Vector2Int a, Vector2Int b,AStarPathfinding2D map,bool containsSelf=true)
        {
            Vector2 direction = GetNodeWorldPositionV2(b,map) - GetNodeWorldPositionV2(a,map);
            float distance = direction.magnitude;
            int steps = Mathf.CeilToInt(distance / map.NodeSize);
            int start = containsSelf ? 0 : 1;
            for (int i = start; i < steps; i++)
            {
                Vector2 samplePoint = GetNodeWorldPositionV2(a,map) + direction.normalized * i * map.NodeSize;
                if (!IsWalkable(samplePoint,map))
                    return true;
            }

            return false;
        }
        public static float GetDistance(Node a,Node b,AStarPathfinding2D map)
        {
            if(a==null || b==null) return 0;
            return GetDistance(a.Position.x, a.Position.y, b.Position.x, b.Position.y, map);
        }
        public static float GetDistance(int x1,int y1,int x2,int y2, AStarPathfinding2D map)
        {
            int dx = Mathf.Abs(x1 - x2);
            int dy = Mathf.Abs(y1 - y2);

            return Mathf.Sqrt(Mathf.Pow(dx,2)+Mathf.Pow(dy,2))*map.NodeSize;
            //// 对角线距离
            //int min = Mathf.Min(dx, dy);
            //int max = Mathf.Max(dx, dy);
            //int mid = dx + dy - min - max;
            //return map.DiagonalCost * min + map.ConnectedCost * (mid - min) + map.DiagonalCost * (max - mid);
        }
        private void OnDrawGizmos()
        {
            if (!ShowGrid) return;
            Gizmos.color = Color.green;
            for (int i = 0; i <= mapX; i++)
            {
                GLUtility.DrawLine(new Vector3(startPos.x + NodeSize * i, startPos.y, startPos.z), new Vector3(startPos.x + NodeSize * i, startPos.y + (mapY ) * NodeSize, startPos.z), 5);
            }
            for (int j = 0; j <= mapY; j++)
            {
                GLUtility.DrawLine(new Vector3(startPos.x, startPos.y + NodeSize * j, startPos.z), new Vector3(startPos.x + NodeSize * (mapX), startPos.y + j * NodeSize, startPos.z), 5);
            }
            int[,] temp = new int[,] { { -1, -1 }, { -1, 1 }, { 1, 1 }, { 1, -1 }, { -1, -1 } };
            //Debug.Log(fixed_obstacleHash.Count);
            foreach (var v in fixed_obstacleList)
            {
                Vector3 pos = GetNodeWorldPositionV3(v, this);
                for (int i = 0; i < temp.GetLength(0) - 1; i++)
                {
                    float x = startPos.x + NodeHalfSize + pos.x * NodeSize;
                    float y = startPos.y + NodeHalfSize + pos.y * NodeSize;
                    Gizmos.color = Color.red;
                    GLUtility.DrawLine(new Vector3(pos.x + NodeHalfSize * temp[i, 0], pos.y + NodeHalfSize * temp[i, 1], startPos.z), new Vector3(pos.x + NodeHalfSize * temp[i + 1, 0], pos.y + NodeHalfSize * temp[i + 1, 1], startPos.z), 5);

                }
            }
            if (dynamic_obstacleHash == null) return;
            foreach (var v in dynamic_obstacleHash)
            {
                Vector3 pos = GetNodeWorldPositionV3(v, this);
                for (int i = 0; i < temp.GetLength(0) - 1; i++)
                {
                    float x = startPos.x + NodeHalfSize + pos.x * NodeSize;
                    float y = startPos.y + NodeHalfSize + pos.y * NodeSize;
                    Gizmos.color = Color.red;
                    GLUtility.DrawLine(new Vector3(pos.x + NodeHalfSize * temp[i, 0], pos.y + NodeHalfSize * temp[i, 1], startPos.z), new Vector3(pos.x + NodeHalfSize * temp[i + 1, 0], pos.y + NodeHalfSize * temp[i + 1, 1], startPos.z), 5);

                }
            }
        }

    }


}
