using FishNet.Utility;
using Saber.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XianXia;

public class NetworkManagerInit : MonoBehaviour
{
    [SerializeField]
    string offlineScencePath;
    [SerializeField]
    string fightScencePath;
//    private void OnValidate()
//    {
//#if UNITY_EDITOR
//        if (!Application.isPlaying)
//        {
//            if (fightScence != "")
//            {
//                fightScencePath = fightScence;
//                Debug.Log("����ս������" + fightScencePath);
//            }
//        }
//#endif
//    }
    private void Awake()
    {
        //����������ҪFish-Networking��Refresh DefaultPrefabs
        GameObject go= GameObject.Instantiate(ABUtility.Load<GameObject>("/Main/Common/Network/NetworkManager.prefab"));
        go.GetComponent<DefaultScene>().SetOnlineScene(fightScencePath);
        go.GetComponent<DefaultScene>().SetOfflineScene(offlineScencePath);
        Debug.Log("Game����ս������" + fightScencePath + go.GetComponent<DefaultScene>().GetOnlineScene());

        Destroy(this.gameObject);
        //FightServerManager
    }


}
