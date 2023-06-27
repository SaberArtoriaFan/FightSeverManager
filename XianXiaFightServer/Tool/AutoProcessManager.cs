using Proto;
using Saber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using XianXiaFightGameServer.FightClientToServer;
using XianXiaFightGameServer.Local;
using XianXiaFightGameServer.Tool;

namespace XianXiaFightGameServer.Tool
{
    public class AutoProcessManager
    {
        ClientServer clientServer;
        LocalServer localServer;
        public AutoProcessManager()
        {
            Init();
        }
        public void Init()
        {
            clientServer = InstanceFinder.GetInstance<ClientServer>();
            localServer = InstanceFinder.GetInstance<LocalServer>();
        }
        public void ShowCouts()
        {
            Saber.SaberDebug.Log($"p:{localServer.WaitForFightProcess.Count}   D:{clientServer.GetFightDataNum()}");

        }
        public void AutoReayFight()
        {
            while (localServer.WaitForFightProcess.Count > 0 && clientServer.GetFightDataNum() > 0)
            {
                LocalClient c = localServer.WaitForFightProcess.Dequeue();
                MainPack fightPack = clientServer.DequeueFightData();
                c.Port = (ushort)fightPack.IpAndPortPack.Port;
                c.FightInfo = fightPack.Info.ToArray();
                fightPack.ActionCode = ActionCode.ReadyFightAction;
                Saber.SaberDebug.Log($"{c.ProcessId}号服务器实例登录，领取了{fightPack.Word}玩家的战斗任务，IP:{fightPack.IpAndPortPack.Ip},Port:{fightPack.IpAndPortPack.Port}");
                c.PlayerID = fightPack.Word;
                c.Send(fightPack);
            }
        }
    }
}
