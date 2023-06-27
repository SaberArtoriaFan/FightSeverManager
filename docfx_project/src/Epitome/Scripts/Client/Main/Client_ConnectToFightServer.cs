using FishNet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Transporting;
using Saber.ECS;

namespace XianXia.Client
{


    public static class Client_ConnectToFightServer 
    {
        public static void AddExitFightActionOnce(Action exitFightAction)
        {
            WaitForInit(() => { Client_InstanceFinder.GetInstance<Client_ExitFightUI>().OnExitEvent += exitFightAction;
                Debug.Log("添加退出战斗事件成功！！"); }, () =>
              {
                //Debug.Log((Client_InstanceFinder.GetInstance<IGameOverHandler>("NormalUtility")!= null).ToString()+"获取Normal");
                return Client_InstanceFinder.GetInstance<Client_ExitFightUI>() != null;
              }); ;
        }
        /// <summary>
        /// 添加游戏结束事件，比如胜利，失败跳出不同的对话框
        /// 如果是一次性事件则勾选ISOnce，一般建议选择一次性
        /// </summary>
        /// <param name="gameOverEvent"></param>
        /// <param name="isOnce"></param>
        public static void AddGameSuccAndFailEvent(Action<GameResult> gameOverEvent, bool isOnce)
        {
            WaitForInit(() => Client_InstanceFinder.GetInstance<IGameOverHandler>("NormalUtility").AddGameSuccAndFailEvent(gameOverEvent, isOnce), () =>
              {
                  //Debug.Log((Client_InstanceFinder.GetInstance<IGameOverHandler>("NormalUtility")!= null).ToString()+"获取Normal");
                  return Client_InstanceFinder.GetInstance<IGameOverHandler>("NormalUtility") != null;
              });
        }
        public static void RemoveGameSuccAndFailEvent(Action<GameResult> gameOverEvent)
        {
            WaitForInit(() => Client_InstanceFinder.GetInstance<IGameOverHandler>("NormalUtility").RemoveGameSuccAndFailEvent(gameOverEvent)
, () =>
            {
                return Client_InstanceFinder.GetInstance<IGameOverHandler>("NormalUtility") != null;
            });
        }
        static void WaitForInit(Action endAction, Func<bool> waitUntilFunc, float maxDelayTime = 20f)
        {
            float timer = 0;
            Timer timer1 = null;
            Action action = () =>
            {
                timer++;
                if (waitUntilFunc() == true)
                {
                    endAction.Invoke();
                    timer1.Stop();
                    return;
                }
                if (timer >= maxDelayTime)
                    timer1.Stop();
            };
            timer1= TimerManager.Instance.AddTimer(action, 1, true);
        }
        /// <summary>
        /// 绑定一些事件，比如失败之后跳出 UI界面告知玩家
        /// </summary>
        /// <param name="success"></param>
        /// <param name="fail"></param>
        public static void SetConnectSuccessEventAndFailEvent(Action success,Action fail)
        {
            Action<ClientConnectionStateArgs> action = null;
            action=(c) =>
        {
            if (c.ConnectionState == LocalConnectionState.Stopped)
            {
                Debug.Log($"连接战斗服务器失败");
                fail?.Invoke();
                InstanceFinder.ClientManager.OnClientConnectionState -= action;
            }
            else if (c.ConnectionState == LocalConnectionState.Started)
            {
                Debug.Log($"连接战斗服务器成功");
                success?.Invoke();
                InstanceFinder.ClientManager.OnClientConnectionState-=action;
            }
        };
            InstanceFinder.ClientManager.OnClientConnectionState += action;
        }
        public static void Connect(string ip,ushort port)
        {
            if (InstanceFinder.TransportManager.Transport.GetConnectionState(false) != LocalConnectionState.Stopped)
            {
                Debug.LogError("客户端已经连接或者正在连接战斗服务器，你确定要继续连接吗？");
                return;
            }

            InstanceFinder.NetworkManager.TransportManager.Transport.SetClientAddress(ip);
            InstanceFinder.NetworkManager.TransportManager.Transport.SetPort(port);
            InstanceFinder.ClientManager.StartConnection();
        }
    }
}
