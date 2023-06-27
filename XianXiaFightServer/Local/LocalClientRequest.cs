using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XianXiaFightGameServer.Local
{
    internal static class LocalClientRequest
    {
        internal static MainPack LocalClientIsLogin()
        {
            MainPack mainPack = new MainPack();
            mainPack.ActionCode = ActionCode.Login;
            mainPack.ReturnCode = ReturnCode.Success;
            return mainPack;
        }
        internal static MainPack Request_ReadyFightAction(MainPack mainPack)
        {
            mainPack.ActionCode = ActionCode.ReadyFightAction;
            return mainPack;
        }
    }
}
