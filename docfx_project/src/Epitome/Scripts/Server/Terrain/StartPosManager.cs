using Saber.Camp;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Terrain
{
    [RequireComponent(typeof(AStarPathfinding2D))]
    public class StartPosManager : SerializedMonoBehaviour
    {
        [OdinSerialize]
        Dictionary<PlayerEnum, Dictionary<byte, Vector2Int>> playerPosDict;
        [SerializeField]
        int edge = 2;
        [SerializeField]
        int width=4;
        [SerializeField]
        int high=4;
        [SerializeField]
        bool isShowDraw = false;
        [SerializeField]
        [HideInInspector]
        AStarPathfinding2D map;

        public Dictionary<PlayerEnum, Dictionary<byte, Vector2Int>> PlayerPosDict { get => playerPosDict; }

        [Button]
        internal void InitDict()
        {
            playerPosDict = new Dictionary<PlayerEnum, Dictionary<byte, Vector2Int>>();
            map = GetComponent<AStarPathfinding2D>();
            if (map.MapX <(edge+ width)*2 || map.MapY < high)
            {
                Debug.LogError("地图太小啦，站的格子超过了");
                return;
            }
            int startX = edge + width-1;
            InitPlayerDict(PlayerEnum.player, map, startX);
            startX = map.MapX - edge-1;
            InitPlayerDict(PlayerEnum.monster, map, startX);
        }
        void InitPlayerDict(PlayerEnum playerEnum,AStarPathfinding2D aStar,int startX)
        {
            int endX = startX - width;
            int startY = (aStar.MapY + high) / 2-1;
            int endY = startY - high;
            byte index = 1;
            int x = startX, y = startY;
            playerPosDict.Add(playerEnum, new Dictionary<byte, Vector2Int>());
            while (x > endX)
            {
                playerPosDict[playerEnum].Add(index++, new Vector2Int(x, y));
                y--;
                if (y <= endY)
                {
                    y = startY;
                    x--;
                    if (x < endX) break;
                }
            }
        }
        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (isShowDraw&&map!=null)
            {
                int[,] temp = new int[,] { { -1, -1 }, { -1, 1 }, { 1, 1 }, { 1, -1 }, { -1, -1 } };
                foreach (var u in playerPosDict)
                {
                    foreach(var v in u.Value.Values)
                    {
                        Vector3 pos = AStarPathfinding2D.GetNodeWorldPositionV3(v, map);
                        for (int i = 0; i < temp.GetLength(0) - 1; i++)
                        {
                            float x = map.startPos.x + map.NodeHalfSize + pos.x * map.NodeSize;
                            float y = map.startPos.y + map.NodeHalfSize + pos.y * map.NodeSize;
                            Gizmos.color = Color.black;
                            GLUtility.DrawLine(new Vector3(pos.x + map.NodeHalfSize * temp[i, 0], pos.y + map.NodeHalfSize * temp[i, 1], map.startPos.z), new Vector3(pos.x + map.NodeHalfSize * temp[i + 1, 0], pos.y + map.NodeHalfSize * temp[i + 1, 1], map.startPos.z), 12);
                            Gizmos.color = Color.white;

                        }
                    }
                }
            }
#endif
        }
    }
}
