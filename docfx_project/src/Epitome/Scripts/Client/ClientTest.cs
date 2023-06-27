using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XianXia.Client
{
    public class ClientTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {


#if !UNITY_SERVER
            XianXia.Client.Client_ConnectToFightServer.AddExitFightActionOnce(() =>
            {
                Saber.ECS.ABUtility.LoadAsyncScene("/Main/Scene/OfflineScene.unity");
                Debug.Log("ddd");
            });
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
