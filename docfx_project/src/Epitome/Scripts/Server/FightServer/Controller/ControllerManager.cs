using Proto;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace XianXia
{
    public class ControllerManager 
    {
        Dictionary<ActionCode, BaseController> allController = new Dictionary<ActionCode, BaseController>();
        private Queue<MainPack> packList=new Queue<MainPack>();
        FightServerClient fightServerClient;
        public MonoBehaviour Host { get;private set; }
        public ControllerManager(FightServerClient fightServer,MonoBehaviour host)
        {
            fightServerClient = fightServer;
            this.Host = host;
            Init();
        }
        // Start is called before the first frame update
         void Init()
        {
            //反射生成控制器
            Assembly assembly = Assembly.GetExecutingAssembly();
            foreach(var t in assembly.GetTypes())
            {
                if(typeof(BaseController).IsAssignableFrom(t)&&t.IsAbstract==false)
                {
                    BaseController b= Activator.CreateInstance(t) as BaseController;
                    if (b == null) break;
                    if (!allController.ContainsKey(b.ActionCode)) { 
                        allController.Add(b.ActionCode, b);
                        b.Init(this);
                    }

                    FightServerManager.ConsoleWrite_Saber($"{b.ActionCode}Method was register");
                }
            }

        }

        // Update is called once per frame
        internal void Update()
        {
            DealPack();
        }
        internal void Destroy()
        {
            foreach(var v in allController.Values)
            {
                if (v != null)
                    v.Destory();
            }
            allController.Clear();
            packList.Clear();
        }
        public void AddPackToDeal(MainPack pack)
        {
            packList.Enqueue(pack);
        }
       private void DealPack()
        {
            if (packList != null && packList.Count > 0)
            {
                while (packList.Count > 0)
                {
                    MainPack pack = packList.Dequeue();
                    FightServerManager.ConsoleWrite_Saber($"在处理{pack.ActionCode}");
                    if (allController.ContainsKey(pack.ActionCode))
                    {
                        allController[pack.ActionCode].Respond(pack);
                    }
                }
            }
            try
            {

            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.Message);
            }
        }
        public void AddRespondHandle(ActionCode actionCode, Action<MainPack> respondHandle) => RespondHandle(actionCode, respondHandle, true);

        public void AddResultHandle(ActionCode actionCode, ReturnCode returnCode, Action<MainPack> handle) => ResultHandle(actionCode, returnCode, handle, true);
        public void RemoveRespondHandle(ActionCode actionCode, Action<MainPack> respondHandle) => RespondHandle(actionCode, respondHandle,false);
        public void RemoveResultHandle(ActionCode actionCode, ReturnCode returnCode, Action<MainPack> handle) => ResultHandle(actionCode, returnCode, handle, false);

        void RespondHandle(ActionCode actionCode, Action<MainPack> respondHandle,bool isAdd)
        {
            if (allController.ContainsKey(actionCode))
            {
                if (isAdd)
                {
                    FightServerManager.ConsoleWrite_Saber($"成功添加{actionCode}响应处理方法{respondHandle.ToString()}！");
                    allController[actionCode].RespondAction += respondHandle;

                }
                else
                {
                    FightServerManager.ConsoleWrite_Saber($"成功移除{actionCode}响应处理方法{respondHandle.ToString()}！");
                    allController[actionCode].RespondAction -= respondHandle;

                }
            }
            else
                FightServerManager.ConsoleWrite_Saber($"尝试添加不存在的指令{actionCode}！");

        }
        void ResultHandle(ActionCode actionCode, ReturnCode returnCode, Action<MainPack> handle,bool isAdd)
        {
            if (allController.ContainsKey(actionCode))
            {
                if (isAdd)
                {
                    if (returnCode == ReturnCode.Success)
                        allController[actionCode].SuccessAction += handle;
                    else if (returnCode == ReturnCode.Fail)
                        allController[actionCode].FailAction += handle;
                    FightServerManager.ConsoleWrite_Saber($"成功添加{actionCode}指令{returnCode}处理方法{handle.ToString()}！");

                }
                else
                {
                    if (returnCode == ReturnCode.Success)
                        allController[actionCode].SuccessAction -= handle;
                    else if (returnCode == ReturnCode.Fail)
                        allController[actionCode].FailAction -= handle;
                    FightServerManager.ConsoleWrite_Saber($"成功移除{actionCode}指令{returnCode}处理方法{handle.ToString()}！");
                }


            }
            else
                FightServerManager.ConsoleWrite_Saber($"尝试添加不存在的指令{actionCode}！");

        }
        internal void Send(MainPack mainPack)
        {
            fightServerClient.Send(mainPack);
        }
    }
}
