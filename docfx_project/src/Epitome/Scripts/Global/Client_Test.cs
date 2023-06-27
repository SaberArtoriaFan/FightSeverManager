using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia
{
    public class Client_Test : MonoBehaviour
    {
        // Start is called before the first frame update
        void Awake()
        {
#if !UNITY_SERVER
            Debug.Log("绑定事件");
            XianXia.Client.Client_ConnectToFightServer.AddGameSuccAndFailEvent((r) =>
            {
                if (r == Saber.ECS.GameResult.Success)
                    Debug.Log("游戏成功");
                else
                    Debug.Log("游戏失败");
            }, true);
#endif
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
