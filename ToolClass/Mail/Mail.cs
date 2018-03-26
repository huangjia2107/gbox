using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ToolClass.Mail
{
    public class Mail
    {
        [DllImport("jmail.dll")]
        static extern int DllRegisterServer(); //注册

        [DllImport("jmail.dll")]
        static extern int DllUnregisterServer(); //取消注册

        MailModel mailModel = null;

        private static Mail instance = null;

        private Mail() { RegDll(); mailModel = MailAccess.GetMail(); }

        /// <summary>
        /// 注册文件
        /// </summary> 
        private bool RegDll()
        {
            RegistryKey rkTest = Registry.ClassesRoot.OpenSubKey("CLSID\\{AED3A6B0-2171-11D2-B77C-0008C73ACA8F}\\");
            if (rkTest == null)
            {
                if (DllRegisterServer() >= 0)
                {
                    return true; //注册成功
                }

                return false; //注册失败
            }

            return true; //已经被注册
        }

        /// <summary>
        /// 取消注册
        /// </summary> 
        private bool UnRegDll()
        {
            RegistryKey rkTest = Registry.ClassesRoot.OpenSubKey("CLSID\\{AED3A6B0-2171-11D2-B77C-0008C73ACA8F}\\");
            if (rkTest != null)
            {
                if (DllUnregisterServer() >= 0)
                {
                    return true; //取消注册成功
                }

                return false; //取消注册失败
            }

            return true; //没注册
        }

        /// <summary>
        /// 获取单例
        /// </summary> 
        public static Mail GetInstance()
        {
            if (instance == null)
                instance = new Mail();
            return instance;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        public bool SendMail(string subject,string mailto,string body)
        {
            jmail.Message jmessage = new jmail.Message();

            try
            {
                jmessage.Silent = true;
                jmessage.Charset = "GB2312";
                jmessage.From = mailModel.From;
                jmessage.FromName = mailModel.FromName;
               // jmessage.ReplyTo = "replayto@benq.com";
                jmessage.Subject = subject;
                jmessage.AddRecipient(mailto, "", "");
                jmessage.Body = body;
                jmessage.MailServerUserName = mailModel.UserName;
                jmessage.MailServerPassWord = mailModel.PassWord;
                if (jmessage.Send(mailModel.MailServer, false))
                {
                    return true;
                } 
            }
            catch { return false; }
            finally { jmessage.Close(); }

            return false;

        }
    }
}
