using FishNet;
using Saber.ECS;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XianXia;

public class FightGlobalManager : MonoBehaviour
{
#if UNITY_EDITOR||UNITY_SERVER
    [SerializeField]
    private string serverGlobal= "ServerGlobal";
#endif
#if !UNITY_SERVER || UNITY_EDITOR
    [SerializeField]
    private string clientGlobal= "ClientGlobal";
#endif
    [SerializeField]
    private string networkGlobal = "NetWorkGlobal";
    [SerializeField]
    private string[] needSpawnInit;


    private static void ConsloeWriteLine(string s,ConsoleColor color=default)
    {
#if UNITY_SERVER && !UNITY_EDITOR
        if (color==default||color == Console.ForegroundColor)
            Console.WriteLine("Saber:" + s+"\t"+ DateTime.Now);
        else
        {
            ConsoleColor origin = Console.ForegroundColor;
                Console.ForegroundColor = color;
            Console.WriteLine("Saber:" + s+"\t"+ DateTime.Now);
                Console.ForegroundColor = origin;

        }
#elif UNITY_SERVER && UNITY_EDITOR
        UnityEngine.Debug.Log(s);
#endif
    }
    private void Awake()
    {
#if UNITY_SERVER
        InstanceFinder.ServerManager.Spawn(GameObject.Instantiate(ABUtility.Load<GameObject>($"{ABUtility.InitMainName}{networkGlobal}")));
        //        FightServerManager.Instance.StartWork();
        FightServerManager.Instance.OnConsloe = ConsloeWriteLine;
        GameObject.Instantiate(ABUtility.Load<GameObject>($"{ABUtility.InitMainName}{serverGlobal}"))?.transform?.SetParent(this.transform);
        //Test();
#else
         GameObject.Instantiate(ABUtility.Load<GameObject>($"{ABUtility.InitMainName}{clientGlobal}"))?.transform?.SetParent(this.transform);
#endif
    }
   
    private void Test()
    {
        if (needSpawnInit != null && needSpawnInit.Length > 0)
        {
            foreach(var v in needSpawnInit)
            {
                InstanceFinder.ServerManager.Spawn(GameObject.Instantiate(ABUtility.Load<GameObject>($"{ABUtility.InitMainName}{v}")));

            }
        }
    }
}
