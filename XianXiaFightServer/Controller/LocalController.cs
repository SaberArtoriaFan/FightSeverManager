using Proto;
using System;
using System.Diagnostics;
using System.Text;
using XianXiaFightGameServer.FightClientToServer;
using XianXiaFightGameServer.Local;
using XianXiaFightGameServer.Tool;
using XianXiaFightServer.Tool;

namespace XianXiaFightGameServer.Controller
{
   public class LocalController : BaseController
    {
        public LocalController(ControllerManager controllerManager) : base(controllerManager)
        {
        }

        public override RequestType RequestType => RequestType.Loacl;
        /// <summary>
        /// 本地战斗服务器登录，记录其进程ID，并给其分配任务
        /// </summary>
        /// <param name="localServer"></param>
        /// <param name="localClient"></param>
        /// <param name="mainPack">若此项不为空则将给目标回包</param>
        /// <returns></returns>
        public MainPack Login(LocalServer localServer, LocalClient localClient, MainPack mainPack)
        {
            //服务器登录时应该在word里写入自己的物理位置，以此判断是几号服务器
            //可能是多余信息
            if (localClient.Port != 0) {
                Saber.SaberDebug.LogError($"Check if error,this fightProcess  gived a vaild port{localClient.Port}");
                return null;
            }
            localClient.ProcessId = Convert.ToInt32(mainPack.Word);
            ClientServer clientServer = InstanceFinder.GetInstance<ClientServer>();
            //不是更新的线程
            if(localServer.ResourceHelper.UpdatingProcessID != localClient.ProcessId)
            {
                //更新的不要去让他等待战斗
                if (!localServer.WaitForFightProcess.Contains(localClient))
                    localServer.WaitForFightProcess.Enqueue(localClient);
                //发送进行游戏命令
                MainPack pack = new MainPack();
                pack.ActionCode = ActionCode.UpdateResources;
                pack.ReturnCode = ReturnCode.Fail;
                mainPack = pack;
            }
            //是
            else
            {
                //发送更新资源命令
                MainPack pack = new MainPack();
                pack.ActionCode = ActionCode.UpdateResources;
                pack.ReturnCode = ReturnCode.Success;
                mainPack = pack;
            }




            //MainPack fighPack = clientServer.DequeueFightData();
            ////localClient.Send(LocalClientRequest.LocalClientIsLogin());
            //if (fighPack == null)
            //{

            //    return null;
            //    ////为空说明不需要该进程了
            //    //WriteLineUtility.WriteLine($"Check if error,opened a fightProcess:{localClient.ProcessId} but no player need it", ConsoleColor.Yellow);
            //    ////返回的时一个login包
            //    //mainPack.ReturnCode = ReturnCode.Fail;
            //    //return mainPack;
            //}
            //WriteLineUtility.WriteLine($"{mainPack.Word}号服务器实例登录，领取了{fighPack.Word}玩家的战斗任务，IP:{fighPack.IpAndPortPack.Ip},Port:{fighPack.IpAndPortPack.Port}");
            //mainPack.ActionCode = ActionCode.ReadyFightAction;
            ////IP地址端口号
            //mainPack.IpAndPortPack = new IPAndPortPack();
            //mainPack.IpAndPortPack.Ip = clientServer.Self_IP;
            //mainPack.IpAndPortPack.Port = fighPack.IpAndPortPack.Port;
            //mainPack.HeroAndPosList.Clear();
            ////战斗队列
            //foreach(var v in fighPack.HeroAndPosList)
            //{
            //    mainPack.HeroAndPosList.Add(v);
            //}
            //localClient.Port = (ushort)fighPack.IpAndPortPack.Port;
            ////目标玩家记录（用来返回给总服务器寻找玩家的）
            //mainPack.Word = fighPack.Word;
            //fighPack = null;



            //localClient.Send(mainPack);
            //controllerManager.send
            return mainPack;
        }
        /// <summary>
        /// 战斗服务器收到端口号与初始化信息后连接成功，给战斗管理服务器返回此消息
        /// </summary>
        /// <param name="localServer"></param>
        /// <param name="localClient"></param>
        /// <param name="mainPack"></param>
        /// <returns></returns>
        public MainPack ReadyFightAction(LocalServer localServer,LocalClient localClient,MainPack mainPack)
        {
            if(mainPack.ReturnCode== ReturnCode.Success)
            {
                //接收包里的地址和端口号
                //发给总服务器告诉它们准备好了

                Saber.SaberDebug.Log($"{mainPack.Word}player apply fightserver->port:{mainPack.IpAndPortPack.Port},is over");
                InstanceFinder.GetInstance<ClientServer>().Send(mainPack);
                //移除从等待区
                if (localServer.WaitForFightProcess.Contains(localClient)) {
                    Saber.SaberDebug.LogWarning($"{localClient.ProcessId} fightProcess is in waitQueue,but it is start game!!!", ConsoleColor.Red);
                    lock (localServer.WaitForFightProcess)
                    {
                        List<LocalClient> localClients = localServer.WaitForFightProcess.ToList();
                        localClients.Remove(localClient);
                        localServer.SetWaitForFightProcess(new Queue<LocalClient>(localClients));
                    }
                }
            }
            else
            {
                Saber.SaberDebug.LogError($"{mainPack.Word}player apply fightserver->port:{mainPack.IpAndPortPack.Port},is fail");
                InstanceFinder.GetInstance<ClientServer>().Send(mainPack);
            }

            return null;
        }
        /// <summary>
        /// 战斗中断消息，正常中断，意外中断都走此
        /// </summary>
        /// <param name="localServer"></param>
        /// <param name="localClient"></param>
        /// <param name="mainPack">包中的ReturnCode告知了关闭类型，也就是战斗结果</param>
        /// <returns></returns>
        public MainPack BreakFight(LocalServer localServer, LocalClient localClient, MainPack mainPack)
        {
            int processId = Convert.ToInt32(mainPack.Word);
            //核对，怕关错
            if (localClient.ProcessId == processId)
            {
                MainPack returnPack= new MainPack();
                returnPack.ActionCode = ActionCode.BreakFight;
                returnPack.ReturnCode = mainPack.ReturnCode;
                returnPack.Word = localClient.PlayerID;
                if(localClient.FightInfo!= null)
                {
                    foreach(var v in localClient.FightInfo)
                        returnPack.Info.Add(v);
                }
                //可以传关卡之类的 returnPack.Info.Add();
                switch (mainPack.ReturnCode)
                {
                    case ReturnCode.Success:
                        break;
                    case ReturnCode.Fail:
                        break;
                    case ReturnCode.Zero: 
                        break;
                }
                //告诉总服务器战斗结束了，以及结果
                InstanceFinder.GetInstance<ClientServer>().Send(returnPack);

                localClient.CloseConnect();
                //战斗日志可以存到本地去
                Saber.SaberDebug.Log(ToolUtility.ArrayToLog(mainPack.Info.ToArray()));
            }

            //准备修改
            //if (localServer.AllProcessNameDict.ContainsKey(processId))
            //{
            //    localServer.AllProcessNameDict[processId].Kill();
            //    localServer.AllProcessNameDict.Remove(processId);
            //    WriteLineUtility.WriteLine($"正在关闭{processId}服务器实例");
            //}else
            //    WriteLineUtility.WriteLine($"并不存在{processId}服务器实例",ConsoleColor.Red);

            return null;
        }

        public MainPack UpdateResources(LocalServer localServer, LocalClient localClient, MainPack mainPack)
        {
            int processId = Convert.ToInt32(mainPack.Word);
            switch (mainPack.ReturnCode)
            {
                case ReturnCode.Success:
                    localServer.ResourceHelper.FinishUpdatedResource(processId);
                    break;
                case ReturnCode.Fail:
                    Saber.SaberDebug.LogError("更新资源出现错误，请检查日志！！！");
                    localServer.ResourceHelper.BreakUpdateResource();
                    break;
            }
            try
            {
                localClient.CloseConnect();
            }
            catch
            {

            }
            return null;
        }
    }
}
