using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ToolClass.Mail
{
    class MailAccess
    {
        #region 变量

        //E:\\项目文件\\WPF\\G-Box\\G-Box\\bin\\Debug\\
        static string _config_path = AppDomain.CurrentDomain.BaseDirectory + "Config\\Mail.xml";

        #endregion

        #region 方法

        /// <summary>
        /// 暴露于外部的方法
        /// </summary>
        /// <returns></returns>
        public static MailModel GetMail()
        {
            return LoadFromFile();
        }

        /// <summary>
        /// 新建配置文件
        /// </summary>
        static void NewFile()
        {
            try
            {
                XDocument xDoc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XComment("程序配置文件,请勿随意修改！"),
                    new XElement("Mails",
                        new XElement("Mail",
                            new XComment("发件人邮件地址"),
                            new XElement("From", ToolClass.Properties.Resource.From),
                            new XComment("发送人姓名"),
                            new XElement("FromName", ToolClass.Properties.Resource.FromName),
                            new XComment("发送者邮箱用户名"),
                            new XElement("UserName", ToolClass.Properties.Resource.UserName),
                            new XComment("发送者邮箱密码"),
                            new XElement("PassWord", ToolClass.Properties.Resource.PassWord),
                            new XComment("发送者邮箱服务器地址"),
                            new XElement("MailServer", ToolClass.Properties.Resource.MailServer)
                                                    )));
                xDoc.Save(_config_path);
            }
            catch { throw (new Exception()); }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <returns></returns>
        static MailModel LoadFromFile()
        {
            if (!File.Exists(_config_path))
            {
                NewFile();
            }

            var param = from config in XElement.Load(_config_path).Descendants("Mail")
                        select MailModel.CreateModel(
                           config.Element("From").Value,
                           config.Element("FromName").Value,
                           config.Element("UserName").Value,
                           config.Element("PassWord").Value,
                           config.Element("MailServer").Value
                           );
            try
            {
                foreach (var p in param)
                {
                    return p;
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        #endregion
    }
}
