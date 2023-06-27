using Proto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XianXiaFightGameServer.FightClientToServer;
using XianXiaFightGameServer.Local;
using XianXiaFightGameServer.Tool;

namespace XianXiaFightGameServer.Controller
{
    public class ClientServerController : BaseController
    {
        public override RequestType RequestType => RequestType.Server;
        public ClientServerController(ControllerManager controllerManager) : base(controllerManager)
        {
        }
        /// <summary>
        /// 从总服务器那收到指令，开启战斗服务器实例
        /// </summary>
        /// <param name="localServer"></param>
        /// <param name="localClient"></param>
        /// <param name="mainPack"></param>
        /// <returns></returns>
        public MainPack ReadyFightAction(ClientServer clientServer, MainPack mainPack)
        {

            //mainPack.ActionCode = ActionCode.ReadyFightAction;
            //int port = clientServer.EnqueueFightData(mainPack);
            //MainPack returnPack = new MainPack();
            //returnPack.ActionCode = ActionCode.ReadyFightAction;
            //returnPack.IpAndPortPack = new IPAndPortPack();
            //returnPack.IpAndPortPack.Ip = clientServer.Self_IP;
            //returnPack.IpAndPortPack.Port = port;
            //returnPack.Word = mainPack.Word;
            //return returnPack;
            Saber.SaberDebug.Log($"[{mainPack.Word}玩家的交战信息]",ConsoleColor.Yellow);
            string s = "Player:";
            foreach(var fight in mainPack.HeroAndPosList)
            {

                foreach(var v in fight.List)
                {
                    Saber.SaberDebug.Log($"{s}英雄信息ID:{v.HeroID};LevelID:{v.LevelID};Pos:{v.Pos}");
                }
                s = "Defend";
            }
            _ = clientServer.EnqueueFightData(mainPack);
            //mainPack.ReturnCode = ReturnCode.Success;
            return null;
        }

        public MainPack UpdateResources(ClientServer clientServer, MainPack mainPack)
        {
            //开启另一个服务器实例，并但他登录后发送更新资源指令
            LocalServer.UpdateResourceHelper helper = InstanceFinder.GetInstance<LocalServer>().ResourceHelper;
            if (helper.UpdatingProcessID != 0)
                helper.StartResourceUpdate();
            else
                Saber.SaberDebug.LogWarning("服务器已经在热更资源了！！！！");
            return null;
        }
        public MainPack HeartBeat(ClientServer clientServer, MainPack mainPack)
        {
            if (clientServer.HeartTimer != null)
            {
                clientServer.HeartTimer.Stop(true);
                clientServer.HeartTimer = null;
            }
            return null;
        }
    }
}
