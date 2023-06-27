using FishNet;
using Proto;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace XianXia
{
    
    public class FightServerManager :Singleton<FightServerManager>
    {
        FightServerClient serverClient;
        ControllerManager controllerManager;
        [SerializeField]
        ushort port=5000;

        string fight_IP;
        ushort fight_Port;

        int processId;
        Process process;
        string playerID;

        public Action<string,ConsoleColor> OnConsloe;
        public static void ConsoleWrite_Saber(string s, ConsoleColor color = default)
        {
            if(instance.OnConsloe!=null)
                Instance.OnConsloe(s, color);
            else
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

        }
        public bool IsSocketActive => serverClient.IsActive;
        internal void SetFightIPAndPort(string ip,ushort port)
        {
            fight_IP = ip;
            fight_Port = port;
        }

        public ControllerManager ControllerManager { get => controllerManager;}
        public string Fight_IP { get => fight_IP;  }
        public ushort Fight_Port { get => fight_Port; }
        public int ProcessId { get => processId; }
        public string PlayerID { get => playerID; set => playerID = value; }

        //        public void QuitApplication()
        //        {
        //            FightServerClient.ConsoleWrite_Saber("Ask for Close this process");
        //#if !UNITY_EDITOR

        //#endif
        //        }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this.gameObject);
            serverClient = new FightServerClient();
            InitControllerManager();
            serverClient.controllerManager = controllerManager;
        }
        public void StartWork()
        {

#if UNITY_SERVER

            process = System.Diagnostics.Process.GetCurrentProcess();
            processId = process.Id;
            serverClient.OnCloseSocketEvent += () =>
            {
                FightServerManager.ConsoleWrite_Saber("Close Socket XIXI");
#if !UNITY_EDITOR

                process.Kill();
#endif

            };

            FightServerManager.ConsoleWrite_Saber($"ProcessID:{processId}");
            serverClient.InitSocket("127.0.0.1", port);
            XianXiaControllerInit.Request_Login();
#if UNITY_EDITOR && UNITY_SERVER
            MainPack mainPack = new MainPack();
                mainPack.IpAndPortPack = new IPAndPortPack();
                mainPack.IpAndPortPack.Ip = "a";
                mainPack.IpAndPortPack.Port = 7070;
                FightServerManager.ConsoleWrite_Saber("±‡º≠∆˜≤‚ ‘");


                XianXiaControllerInit.Respond_StartFightFishNetServer(mainPack);
#endif
            //GameManager.NewInstance.OnFinishGameEvent += () =>
            //{
            //};
            //TimerManager.instance.AddTimer(() => {
            //    XianXiaControllerInit.
            //    Excess_LoginRequest();
            //}, 2);
            //MainPack mainPack = null;
            //mainPack.HeroAndPosList.
#endif
        }
        private void Start()
        {
            StartWork();
        }
        [Button]
        void Test()
        {
            //XianXiaControllerInit.Request_StartFightFishNetServer();
            //MainPack mainPack = new MainPack();

            //mainPack.ActionCode = ActionCode.UpdateResources;
            //HeroAndPosPack heroAndPosPack = new HeroAndPosPack();
            //heroAndPosPack.List.Add(new );
            //heroAndPosPack.PosList.Add(1);
            //HeroAndPosPack heroAndPosPack1 = new HeroAndPosPack();
            //heroAndPosPack1.HeroNameList.Add("AAA");
            //heroAndPosPack1.PosList.Add(2);
            //mainPack.HeroAndPosList.Add(heroAndPosPack);
            //mainPack.HeroAndPosList.Add(heroAndPosPack1);

            //serverClient.Send(mainPack);
        }
        void InitControllerManager()
        {
            controllerManager = new ControllerManager(serverClient, this);
            XianXiaControllerInit.InitControllerManager(controllerManager);
        }

        private void Update()
        {
            controllerManager.Update();
        }
        protected override  void OnDestroy()
        {
            serverClient?.Destory();
            //serverClient
            controllerManager?.Destroy();
            base.OnDestroy();
#if UNITY_SERVER && !UNITY_EDITOR
                        System.Diagnostics.Process.GetCurrentProcess().Kill();
#endif


        }
        public void Send(MainPack mainPack)
        {
            //Debug.Log($"∑¢ÀÕ{mainPack.ActionCode}«Î«Û");
            serverClient.Send(mainPack);
        }
    }
}
