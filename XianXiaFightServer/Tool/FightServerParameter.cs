using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace XianXiaFightGameServer.Tool
{
    [DataContractAttribute]
    public class FightServerParameter
    {
        [DataMemberAttribute]
        public string ServerIP;
        [DataMemberAttribute]
        public ushort ServerPort;
        [DataMemberAttribute]
        public string SelfIP;
        [DataMemberAttribute]
        public ushort SelfPort;
        [DataMemberAttribute]
        public ushort[] FightAllotPorts;
        [DataMemberAttribute]
        public string FightServerPath;
        [DataMemberAttribute]
        public string UpdateResourcePath;
        [DataMemberAttribute]
        public int PreparaProcessNum = 1;
        [DataMemberAttribute]
        public (string, string)[] MailUserAndPermission;
        [DataMemberAttribute]
        public string[] TargetMails;
        public FightServerParameter()
        {

        }

        public FightServerParameter(string serverIP, ushort serverPort, ushort selfPort, ushort[] fightAllotPorts, string fightServerPath, string selfIP, int preparaProcessNum, (string, string)[] mailUserAndPermission, string[] targetMails, string updateResourcePath)
        {
            ServerIP = serverIP;
            ServerPort = serverPort;
            SelfPort = selfPort;
            FightAllotPorts = fightAllotPorts;
            FightServerPath = fightServerPath;
            SelfIP = selfIP;
            PreparaProcessNum = preparaProcessNum;
            this.MailUserAndPermission = mailUserAndPermission;
            this.TargetMails = targetMails;
            UpdateResourcePath = updateResourcePath;
        }
        public override string ToString()
        {
            StringBuilder stringBuilder=new StringBuilder();
            stringBuilder.Append("Config\n");
            stringBuilder.Append($"ServerIP:{ServerIP}\n");
            stringBuilder.Append($"ServerPort:{ServerPort}\n");
            stringBuilder.Append($"SelftIP:{SelfIP}\n");
            stringBuilder.Append($"SelftPort:{SelfPort}\n");
            stringBuilder.Append(($"FightAllotPorts:"));
            foreach (var v in FightAllotPorts)
            {
                stringBuilder.Append($"{v},");
            }
            stringBuilder.Append("\n");
            stringBuilder.Append($"FightSerPath:{FightServerPath}\n");
            stringBuilder.Append($"PreparaProcessNum:{PreparaProcessNum}\n");
            stringBuilder.Append($"UpdateResourcePath:{UpdateResourcePath}\n");

            if (MailUserAndPermission != null)
            {
                foreach (var v in MailUserAndPermission)
                {
                    stringBuilder.Append($"MailInfo:{v.Item1}\n");
                }
            }
            if (TargetMails != null)
            {
                foreach (var v in TargetMails)
                {
                    stringBuilder.Append($"TargetMail:{v}\n");
                }
            }


            return stringBuilder.ToString();
        }
    }
}
