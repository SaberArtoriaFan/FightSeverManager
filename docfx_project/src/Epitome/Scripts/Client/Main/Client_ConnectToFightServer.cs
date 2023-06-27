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
                Debug.Log("����˳�ս���¼��ɹ�����"); }, () =>
              {
                //Debug.Log((Client_InstanceFinder.GetInstance<IGameOverHandler>("NormalUtility")!= null).ToString()+"��ȡNormal");
                return Client_InstanceFinder.GetInstance<Client_ExitFightUI>() != null;
              }); ;
        }
        /// <summary>
        /// �����Ϸ�����¼�������ʤ����ʧ��������ͬ�ĶԻ���
        /// �����һ�����¼���ѡISOnce��һ�㽨��ѡ��һ����
        /// </summary>
        /// <param name="gameOverEvent"></param>
        /// <param name="isOnce"></param>
        public static void AddGameSuccAndFailEvent(Action<GameResult> gameOverEvent, bool isOnce)
        {
            WaitForInit(() => Client_InstanceFinder.GetInstance<IGameOverHandler>("NormalUtility").AddGameSuccAndFailEvent(gameOverEvent, isOnce), () =>
              {
                  //Debug.Log((Client_InstanceFinder.GetInstance<IGameOverHandler>("NormalUtility")!= null).ToString()+"��ȡNormal");
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
        /// ��һЩ�¼�������ʧ��֮������ UI�����֪���
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
                Debug.Log($"����ս��������ʧ��");
                fail?.Invoke();
                InstanceFinder.ClientManager.OnClientConnectionState -= action;
            }
            else if (c.ConnectionState == LocalConnectionState.Started)
            {
                Debug.Log($"����ս���������ɹ�");
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
                Debug.LogError("�ͻ����Ѿ����ӻ�����������ս������������ȷ��Ҫ����������");
                return;
            }

            InstanceFinder.NetworkManager.TransportManager.Transport.SetClientAddress(ip);
            InstanceFinder.NetworkManager.TransportManager.Transport.SetPort(port);
            InstanceFinder.ClientManager.StartConnection();
        }
    }
}
