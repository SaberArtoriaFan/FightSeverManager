using Proto;
using Saber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XianXiaFightGameServer.Tool;
using XianXiaFightServer.Tool;

namespace XianXiaFightGameServer.FightClientToServer
{
    public static class ClientServerRequest
    {
        public static void Login()
        {
            MainPack mainPack = new MainPack();
            mainPack.ActionCode = ActionCode.Login;
            mainPack.Word = "2020511";
            InstanceFinder.GetInstance<ClientServer>().Send(mainPack);
        }
        public static void SendHeartbeat(float heartBeatTime)
        {
            MainPack mainPack = new MainPack();
            mainPack.ActionCode = ActionCode.HeartBeat;
            //string time= DateTime.Now.ToString("G");
            //mainPack.Word = time;

            TimerSystem.Timer timer = InstanceFinder.GetInstance<TimerSystem>().AddTimer(() =>
            {
                SaberDebug.LogError($"失去服务器心跳回馈！！！");
                InstanceFinder.GetInstance<ClientServer>().ReConnect();
            }, heartBeatTime);
            //SaberDebug.Log($"发送心跳包,时间{time}");
            InstanceFinder.GetInstance<ClientServer>().SendHeartBeat(mainPack, timer);
        }

        //public static void SendFightResult()
        //{

        //}
    }
}
