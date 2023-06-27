using Saber;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using XianXiaFightGameServer.Tool;

namespace XianXiaFightGameServer.Email
{
   public class MailUtility
    {
        public static void InitMailPlatform()
        {
            if (InstanceFinder.GetInstance<MailPlatform>() != null) return;
            var mails = JsonUtility.FightServerParameter.MailUserAndPermission;
            var targets = JsonUtility.FightServerParameter.TargetMails;
            if (mails != null && mails.Length > 0 && targets != null && targets.Length > 0)
            {
                var list = new List<(MailAddress, string)>();
                foreach (var v in mails)
                    list.Add(new(new MailAddress(v.Item1), v.Item2));
                MailPlatform mailPlatform = new MailPlatform(list);//邮箱
                InstanceFinder.Register(mailPlatform);
                SaberDebug.Log("初始化邮箱系统！！");
            }
        }
        public static void SendToDefault(string subobject, string body)
        {
            if (JsonUtility.FightServerParameter.TargetMails == null||JsonUtility.FightServerParameter.TargetMails.Length<=0) { SaberDebug.LogWarning("没有设置邮件发送对象！！！");return; }

            MailBuilder mailBuilder = new MailBuilder();
            //标题
            mailBuilder.Body = body;
            //内容
            subobject = subobject+DateTime.Now.ToString("G");
            mailBuilder.Subject =subobject;
            //目标
            foreach (var v in JsonUtility.FightServerParameter.TargetMails)
                mailBuilder.Address.Add(v);
            InstanceFinder.GetInstance<MailPlatform>().SendMailAsync(mailBuilder.Build(), null, mailBuilder);
        }
        public static void SendTo(string subobject, string body,List<string> list)
        {
            if (list==null ||list.Count<=0) { SaberDebug.LogWarning("没有设置邮件发送对象！！！"); return; }

            MailBuilder mailBuilder = new MailBuilder();
            //标题
            mailBuilder.Body = body;
            //内容
            subobject = subobject + DateTime.Now.ToString("G");
            mailBuilder.Subject = subobject;
            //目标
            foreach (var v in list)
                mailBuilder.Address.Add(v);
            InstanceFinder.GetInstance<MailPlatform>().SendMailAsync(mailBuilder.Build(), null, mailBuilder);
        }
    }
}
