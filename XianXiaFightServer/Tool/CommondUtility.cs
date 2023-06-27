using Proto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XianXiaFightGameServer.FightClientToServer;
using XianXiaFightGameServer.Local;
using XianXiaFightGameServer.Log;
using XianXiaFightServer.Tool;
using Saber;
using Type = System.Type;
using System.Net.Mail;
using XianXiaFightGameServer.Email;

namespace XianXiaFightGameServer.Tool
{
    internal static class CommondUtility
    {
        
        public static void ReadLine()
        {
            string s= Console.ReadLine();

            //InstanceFinder.GetInstance<LogManager>().Reocrd(s);
            if (s.StartsWith("-")) s = s.Remove(0, 1);
            else return;
            int index = s.IndexOf(" ");
            if(index == -1) index=s.Length;
            string method = s.Substring(0, index);
            Type type=typeof(CommondUtility);
            MethodInfo methodInfo = type.GetMethod(method,BindingFlags.IgnoreCase
                    | BindingFlags.NonPublic
                    | BindingFlags.Static);
            if (methodInfo == null) { SaberDebug.LogError("Is not a validateCommond!");return; }
            if (index >= s.Length) index = s.Length - 1;
            SaberDebug.Log(methodInfo.Name);
            //if(index+1<0) { SaberDebug.LogError("Is not a validateCommond!"); return; }
            methodInfo.Invoke(null, new object[] { s.Substring(index+1) });
        }
        #region 调试指令

        private static void InitServer(string s)
        {
            /// <summary>
            ///  判断IP地址
            /// </summary>
            /// <param name="ipAddress"></param>
            /// <returns></returns>
            static bool ValidateIPAddress(string ipAddress)
            {
                Regex validipregex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
                return (ipAddress != "" && validipregex.IsMatch(ipAddress.Trim())) ? true : false;
            }
            ClientServer clientServer = InstanceFinder.GetInstance<ClientServer>();
            if (clientServer == null || clientServer.IsActive == true)
            {
                Saber.SaberDebug.LogWarning("ClientServer is Active,Cant Init Twice");
                return;
            }
            try
            {
                s = s.Replace(" ", "");
                if(string.IsNullOrEmpty(s))
                {
                    clientServer.InitSocket(JsonUtility.FightServerParameter.ServerIP, JsonUtility.FightServerParameter.ServerPort);
                    return;
                }
                int index = s.IndexOf(",");
                string ip = s.Substring(0, index);
                ushort port = Convert.ToUInt16(s.Substring(index + 1));
                if (ValidateIPAddress(ip))
                {
                    clientServer.InitSocket(ip, port);
                }
                else
                {
                    Saber.SaberDebug.LogWarning($"{ip} is not a validateIPAddress！", ConsoleColor.Red);
                }
            }
            catch(Exception ex)
            {
                Saber.SaberDebug.LogError($"InitServer fail{ex.Message}", ConsoleColor.Red);
            }

        } 
        private static void Update(string s)
        {
            //开启另一个服务器实例，并但他登录后发送更新资源指令
            LocalServer.UpdateResourceHelper helper = InstanceFinder.GetInstance<LocalServer>().ResourceHelper;
            if (helper.UpdatingProcessID == 0)
                helper.StartResourceUpdate();
            else
                Saber.SaberDebug.LogWarning("服务器已经在热更资源了！！！！");
        }
        private static void CLS(string s)
        {
            Console.Clear();
        }
        private static void Kill(string s)
        {
            int id= Convert.ToInt32(s);
            if(InstanceFinder.GetInstance<LocalServer>().AllProcessNameDict.TryGetValue(id, out var process)) 
            { 
                process.Kill();
            }
        }

        private static void Test(string s)
        {
            //InstanceFinder.GetInstance<LogManager>().OutPutJson();
            MainPack mainPack = new MainPack();
            mainPack.Word = "User";
            mainPack.ActionCode = ActionCode.ReadyFightAction;
            HeroAndPosPack mainPosPack = new HeroAndPosPack();
            NGridPack nGridPack = new NGridPack();
            nGridPack.HeroID = 1;
            nGridPack.LevelID = 1;
            nGridPack.Pos = 1;
            mainPosPack.List.Add(nGridPack);
            mainPack.HeroAndPosList.Add(mainPosPack);
            InstanceFinder.GetInstance<ClientServer>().EnqueueFightData(mainPack);
        }
        private static void Breake(string s)
        {
            //InstanceFinder.GetInstance<LogManager>().OutPutJson();
            MainPack returnPack = new MainPack();
            returnPack.ActionCode = ActionCode.BreakFight;
            returnPack.ReturnCode = ReturnCode.Success ;
            returnPack.Word = "User";
            returnPack.Info.Add("10001");
            InstanceFinder.GetInstance<ClientServer>().Send(returnPack);
        }
        private static void Login(string s)
        {
            ClientServerRequest.Login();
        }
        private static void CheckConfiguration(string s)
        {
            JsonUtility.InitConfiguration();
        }
        private static void SLFC(string s)
        {
            InstanceFinder.GetInstance<AutoProcessManager>().ShowCouts();
        }
        private static void SetLogOutPut(string s)
        {

        }
        private static void ReConnect(string s)
        {
            InstanceFinder.GetInstance<ClientServer>().ReConnect();
        }
        private static void Send(string s)
        {
            //int toIndex = s.IndexOf("To");
            //if ( toIndex < 0) { SaberDebug.LogWarning("请使用正确格式{-Send To{目标地址} }"); };
            InstanceFinder.GetInstance<TimerSystem>().AddTimer(() => {
                SaberDebug.Log("送信成功");
                MailUtility.SendTo("sblq_", "lqsb", new List<string>() {  "2565190116@qq.com" });
            }, 6, true);

        }
        #endregion
    }
}
