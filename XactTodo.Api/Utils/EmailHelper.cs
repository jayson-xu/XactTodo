using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace XactTodo.Api.Utils
{
    /// <summary>
    /// 邮件发送辅助类
    /// </summary>
    public class EmailHelper
    {
        /// <summary>
        /// 发送电子邮件
        /// </summary>
        /// <param name="smtpHost">SMTP服务器</param>
        /// <param name="enableSsl">是否启用SSL加密</param>
        /// <param name="userName">登录帐号</param>
        /// <param name="pwd">登录密码</param>
        /// <param name="recipientName">发件人昵称</param>
        /// <param name="senderMailbox">发件人</param>
        /// <param name="recipientMailbox">收件人</param>
        /// <param name="subject">主题</param>
        /// <param name="body">正文</param>
        public static async Task SendMailAsync(string smtpHost, bool enableSsl, string userName, string pwd, string senderMailbox, string recipientName, string recipientMailbox, string subject, string body)
        {
            var smtpClient = new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,//指定电子邮件发送方式
                Host = smtpHost,//指定SMTP服务器
                Credentials = new NetworkCredential(userName, pwd),//用户名和密码
                EnableSsl = enableSsl
            };
            var fromAddress = new MailAddress(senderMailbox);
            var toAddress = new MailAddress(recipientMailbox, recipientName);
            var mailMessage = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,//主题
                Body = body,//内容
                BodyEncoding = Encoding.UTF8,//正文编码
                IsBodyHtml = true,//设置为HTML格式
                Priority = MailPriority.Normal//优先级
            };
            await smtpClient.SendMailAsync(mailMessage);
        }

    }
}
