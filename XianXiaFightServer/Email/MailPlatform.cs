using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using XianXiaFightGameServer.Tool;

namespace XianXiaFightGameServer.Email
{
    internal class MailPlatform{
    
        private List<(MailAddress, string)>? _mailinformation;
        public MailPlatform(IList<(MailAddress, string)>? pairs)
        {
            _mailinformation = (List<(MailAddress, string)>?)pairs ?? throw new Exception("错误的初始化");
        }
        public void SendMail(MailMessage message)
        {
            var info = _mailinformation?.OrderBy(s => Guid.NewGuid()).FirstOrDefault() ?? throw new Exception("未初始化任何设置");
            message.From = info.Item1;
            var client = new SmtpClient()
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(info.Item1.Address, info.Item2),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = "smtp." + info.Item1.Host
            };
            client.Send(message);
        }
        public void SendMailAsync(MailMessage message, SendCompletedEventHandler CompletedMethod, object args)
        {
            var info = _mailinformation?.OrderBy(s => Guid.NewGuid()).FirstOrDefault() ?? throw new Exception("未初始化任何设置");
            message.From = info.Item1;
           
            var client = new SmtpClient()
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential(info.Item1.Address, info.Item2),
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = "smtp." + info.Item1.Host
            };
            if(CompletedMethod!=null)
                client.SendCompleted += new SendCompletedEventHandler(CompletedMethod);
            client.SendAsync(message, args);
        }

    }
}