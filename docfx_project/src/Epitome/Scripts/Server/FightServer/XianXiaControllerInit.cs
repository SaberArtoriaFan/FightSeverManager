using FishNet;
using System;
using System.Collections;
using UnityEngine;
using FishNet.Transporting;
using Proto;
using FishNet.Connection;
using Saber.ECS;

namespace XianXia
{
    public static class XianXiaControllerInit 
    {
        //static Timer timer;
        static  bool isRealGame = false;
        const float maxWaitTime = 20f;
        static bool isInitFight = false;
        public static void InitControllerManager(ControllerManager controllerManager)
        {
            controllerManager.AddRespondHandle(ActionCode.ReadyFightAction, Respond_StartFightFishNetServer);
            controllerManager.AddResultHandle(ActionCode.Login, ReturnCode.Fail, Respond_Fail_Login);
            controllerManager.AddResultHandle(ActionCode.UpdateResources, ReturnCode.Success, Respon_Succ_UpdateResource);
            controllerManager.AddResultHandle(ActionCode.UpdateResources, ReturnCode.Fail, Respon_Fail_UpdateResource);
            //controllerManager.AddRespondHandle(ActionCode.BreakFight, Respond_BreakFight);
            //controllerManager.AddRespondHandle(ActionCode.Login, Respond_Login);
            //controllerManager.AddResultHandle(ActionCode.Login, ReturnCode.Success, Respond_Fail_Login);
        }

        private static void Respon_Fail_UpdateResource(MainPack obj)
        {
            //������Ϸ
            FightServerManager.ConsoleWrite_Saber("��Ϸģʽ����");
            XianXia.Server.XianXiaUpdater.Instance.EnterTargetModel(0,null);
        }

        private static void Respon_Succ_UpdateResource(MainPack obj)
        {
            //������Դ
            FightServerManager.ConsoleWrite_Saber("�ȸ�����Դģʽ����,��Ŀ����ַ������Դ");
            XianXia.Server.XianXiaUpdater.Instance.EnterTargetModel(1,(res)=>
            {
                MainPack returnPack = new MainPack();
                returnPack.ActionCode = ActionCode.UpdateResources;
                returnPack.Word = FightServerManager.Instance.ProcessId.ToString(); ;
                if (res)
                    returnPack.ReturnCode = ReturnCode.Success;
                else
                   returnPack.ReturnCode = ReturnCode.Fail;
                FightServerManager.ConsoleWrite_Saber($"��Դ�ȸ��½���,���Ϊ{returnPack.ReturnCode},�ȴ����رա�");
                FightServerManager.Instance.Send(returnPack);
            });
        }

        /// <summary>
        /// ��Ӧ���յ����ս������Ϣ����ΪҪ�ر�ֱ�ӹر���
        /// </summary>
        /// <param name="obj"></param>
        private static void Respond_BreakFight(MainPack obj)
        {
            //Request_CloseApplication();
        }


        public static void Respond_StartFightFishNetServer(MainPack mainPack)
        {
            FightServerManager.ConsoleWrite_Saber("�յ������ͻ�����Ϣ���ȴ�Fishnet��ʼ����");
            FightServerManager.Instance.StartCoroutine(WaitForFishNetInit(()=>StartFightFishNetServer(mainPack)));
        }
        /// <summary>
        /// ����ս��������
        /// </summary>
        /// <param name="mainPack"></param>
       public  static void StartFightFishNetServer(MainPack mainPack)
        {
            InstanceFinder.ServerManager.SetStartOnHeadless(false);
            //if (timer != null) timer.Stop();
            //timer = null;

            if (mainPack.IpAndPortPack == null || mainPack.IpAndPortPack.Port==0) { mainPack.ReturnCode = ReturnCode.Fail; return; }
            FightServerManager.ConsoleWrite_Saber($"Receive IP{mainPack.IpAndPortPack.Ip}andPort{mainPack.IpAndPortPack.Port},StartConnection", ConsoleColor.Green);

            InstanceFinder.NetworkManager.TransportManager.Transport.SetServerBindAddress("Any", FishNet.Transporting.IPAddressType.IPv4);
            InstanceFinder.NetworkManager.TransportManager.Transport.SetPort((ushort)mainPack.IpAndPortPack.Port);
            InstanceFinder.ServerManager.StartConnection();

            Action<ServerConnectionStateArgs> action=null;
            action= (s) =>
            {
                if (s.ConnectionState == FishNet.Transporting.LocalConnectionState.Started) 
                {
                    mainPack.ReturnCode = ReturnCode.Success; 
                    InstanceFinder.ServerManager.OnServerConnectionState -= action;
                    //ע���¼������κ�ʱ��������ر�ʱ�ϴ���Ϣ������֪�ر�ԭ��
                    InstanceFinder.ServerManager.OnServerConnectionState += Request_BreakFight;
                    FightServerManager.Instance.SetFightIPAndPort(mainPack.IpAndPortPack.Ip, (ushort)mainPack.IpAndPortPack.Port);
                    //����GameManager���г�ʼ�����أ�����ģ��

                    InstanceFinder.NetworkManager.StartCoroutine(WaitForClientLogin(() =>
                    {
                        isInitFight = true;
                        if (isRealGame) GameManager.NewInstance.InitFight(mainPack);
                        else ServerTest.Instance.Test();

                    }));

                    //����������ߣ����ߺ�ʼս��
                    //���ʮ�������û���������Զ��ر�

                    #region ���һ��ʱ����û����ҵ�½�͹ر�
                    NormalUtility normalUtility = InstanceFinder.GetInstance<NormalUtility>();
                    Timer l_timer =null;
                    l_timer= TimerManager.Instance.AddTimer(() =>
                    {
                        if (InstanceFinder.ServerManager.Clients.Count <= 0)
                        {
                            FightServerManager.ConsoleWrite_Saber($"Long than 15s no Client Enter!!!",ConsoleColor.Red);
                            InstanceFinder.ServerManager.StopConnection(true);
                        }
                    }, 15f);
                    #endregion
                    #region ������߾Ϳ�ʼ��Ϸ
                    Action<NetworkConnection, RemoteConnectionStateArgs> action3 = null;
                    action3 = (n, r) =>
                    {
                        //FightServerClient.ConsoleWrite_Saber($"....didididid");

                        if (r.ConnectionState == RemoteConnectionState.Started)
                        {
                 

                            InstanceFinder.NetworkManager.StartCoroutine(WaitForGameManager(() =>
                            {

                                if (isRealGame)
                                    TimerManager.Instance.AddTimer(GameManager.NewInstance.StartFight, 1f);
                                else
                                    TimerManager.Instance.AddTimer(ServerTest.Instance.Test2, 1f);
                            }));
                            FightServerManager.ConsoleWrite_Saber($"Client online startFight");

                            InstanceFinder.ServerManager.OnRemoteConnectionState -= action3;
                        }

                    };
                    //#region ����ս���ͷ���ս�����
                    //Action<NetworkConnection, RemoteConnectionStateArgs> action4 = (n, r) =>
                    //{
                    //    if (r.ConnectionState == RemoteConnectionState.Stopped)
                    //    {

                    //    }
                    //};


                    //    #endregion
                    InstanceFinder.ServerManager.OnRemoteConnectionState += action3;
                    #endregion
                    #region ������������ر�
                    Action<NetworkConnection,RemoteConnectionStateArgs> action2 = null;
                    action2 = (n,r) =>
                    {
                        //FightServerClient.ConsoleWrite_Saber($"....didididid");

                        if (r.ConnectionState == RemoteConnectionState.Stopped)
                        {
                            FightServerManager.ConsoleWrite_Saber($"Client stopConnect");
                            InstanceFinder.ServerManager.StopConnection(false);
                        }    
                    };
                    InstanceFinder.ServerManager.OnRemoteConnectionState += action2;
                    #endregion
                }
                else if (s.ConnectionState == FishNet.Transporting.LocalConnectionState.Stopped) 
                { 
                    mainPack.ReturnCode = ReturnCode.Fail; 
                    InstanceFinder.ServerManager.OnServerConnectionState -= action;
                    mainPack.Info.Add("Create Fight Server Error��Please Check Port������");

                    FightServerManager.ConsoleWrite_Saber("Create Fight Server Error��Please Check Port������");
                    Request_CloseApplication(ReturnCode.Zero,new string[] { "Create Fight Server Error��Please Check Port������" });
                }

            };
            InstanceFinder.ServerManager.OnServerConnectionState += action;
        }

        static IEnumerator WaitForFishNetInit(Action action)
        {
            WaitUntil waitUntil = new WaitUntil(() =>
              {
                  Debug.Log($"�ȴ� NetworkManager:{InstanceFinder.NetworkManager != null},is Initial {InstanceFinder.NetworkManager?.Initialized}");
                  return InstanceFinder.NetworkManager != null && InstanceFinder.NetworkManager.Initialized;
              });
            yield return waitUntil;
            Debug.Log($"�ȴ� ��������ʼ����");

            action?.Invoke();

        }
        static IEnumerator WaitForGameManager(Action action)
        {
            WaitUntil waitUntil = new WaitUntil(() =>
            {
                Debug.Log($"�ȴ� GameManger Init:{GameManager.NewInstance != null},wait InitFight:{isInitFight},is Initial");
                return isInitFight&&GameManager.NewInstance != null && GameManager.NewInstance.World!=null;
            });
            yield return waitUntil;
            yield return new WaitForSeconds(Time.deltaTime * 2);
            Debug.Log($"�ȴ� ����,GameManger is Inited");

            action?.Invoke();

        }
        static IEnumerator WaitForClientLogin(Action action)
        {
            float timer = 0;
            WaitUntil waitUntil = new WaitUntil(() =>
            {
                timer += Time.deltaTime;
                Debug.Log($"�ȴ� Client Init:{InstanceFinder.GetInstance<NormalUtility>()?.clientLogin},is Initial");
                return timer>maxWaitTime||(InstanceFinder.GetInstance<NormalUtility>() != null && InstanceFinder.GetInstance<NormalUtility>().clientLogin);
            });
            yield return waitUntil;
            yield return new WaitForSeconds(Time.deltaTime * 2);
            Debug.Log($"�ȴ� ����,Client  is Inited");

            action?.Invoke();

        }
        /// <summary>
        /// ��¼ʧ��ֱ��ע��
        /// </summary>
        /// <param name="mainPack"></param>
        static void Respond_Fail_Login(MainPack mainPack)
        {
            FightServerManager.ConsoleWrite_Saber("��¼ʧ��");

            //if (timer == null) return;
            //timer.Stop();
            //timer = null;
            mainPack.ReturnCode = default;
            Request_CloseApplication(ReturnCode.Zero);
            //Application.Quit();
        }
        ///// <summary>
        ///// ��η���ֱ��Ŀ����յ�
        ///// </summary>
        ///// <param name="mainPack"></param>
        //public static void Excess_LoginRequest()
        //{


        //    Timer timer2 = null;
        //    timer2 = TimerManager.Instance.AddTimer(() =>
        //    {
        //        if (FightServerManager.Instance.IsSocketActive && InstanceFinder.IsOffline)
        //        {
        //            timer2.Stop();
        //        }
        //    }, Time.deltaTime*3,true);
        //}
        static void Request_BreakFight(ServerConnectionStateArgs serverConnectionStateArgs)
        {
            if (serverConnectionStateArgs.ConnectionState != LocalConnectionState.Stopped) return;
            //���Ը�����Ϸ״̬�ж�����������رշ�������
            //��������������ش�����־
            //����ս����־
            FightServerManager.ConsoleWrite_Saber("Fight server(this) is Offline");
            //������������success
            //mainPack.ReturnCode=ReturnCode.Success;

            //�ر���Ϸ���̣��黹�˿�
            GameManager gameManager = GameManager.NewInstance;
            ReturnCode returnCode ;
            if (gameManager!=null&&gameManager._GameStatus == GameStatus.Finish)
            {
                if (gameManager._GameResult ==GameResult.Fail)
                    returnCode = ReturnCode.Fail;
                else if (gameManager._GameResult == GameResult.Success)
                    returnCode = ReturnCode.Success;
                else 
                    returnCode = ReturnCode.Zero;
            }
            else
                returnCode = ReturnCode.Zero;
            Request_CloseApplication(returnCode,new string[] { $"Fight Server Close,{0}" } );
            InstanceFinder.ServerManager.OnServerConnectionState -= Request_BreakFight;
        }

        /// <summary>
        /// �����������˿ںſ���
        /// </summary>
        public static void Request_Login()
        {
            MainPack mainPack = new MainPack();
            mainPack.ActionCode = ActionCode.Login;
            mainPack.Word = FightServerManager.Instance.ProcessId.ToString();
            FightServerManager.Instance.Send(mainPack);
            //if (timer != null) timer.Stop();
            //timer = TimerManager.Instance.AddTimer(() =>
            //{
            //    Request_CloseApplication();
            //}, 10);

        }
        /// <summary>
        /// ����������ر��Լ�
        /// </summary>
        public static void Request_CloseApplication(ReturnCode returnCode=ReturnCode.Zero, string[] info = null)
        {
            FightServerManager.ConsoleWrite_Saber("Ask for Close this process");
            MainPack mainPack = new MainPack();
            mainPack.ActionCode = ActionCode.BreakFight;
            mainPack.Word = FightServerManager.Instance.ProcessId.ToString();
            mainPack.ReturnCode = returnCode;
            if (info != null)
            {
                foreach(var v in info)
                {
                    mainPack.Info.Add(v);
                }
            }
            mainPack.Info.Add("Fight Log Start");
            mainPack.Info.Add(FightLog.OutPut());
            mainPack.Info.Add("Fight Log End");
            FightServerManager.Instance.Send(mainPack);
        }



    }
}
