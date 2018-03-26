
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Configure.Models;

namespace Configure
{
    public class ConfigAccess
    {
        #region 变量

        //E:\\项目文件\\WPF\\G-Box\\G-Box\\bin\\Debug\\
        static string _config_path = AppDomain.CurrentDomain.BaseDirectory + "Config\\SvConfig.xml";

        #endregion

        #region 方法

        /// <summary>
        /// 暴露于外部的方法
        /// </summary>
        /// <returns></returns>
        public static ConfigModel GetConfig()
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
                    new XElement("Servers",
                        new XElement("Server",
                            new XComment("登陆服务器IP！"),
                            new XElement("LoginIP", Configure.Properties.Resources.LoginIP),
                            new XComment("登陆服务器端口"),
                            new XElement("LoginPort", Configure.Properties.Resources.LoginPort),
                            new XComment("大厅服务器IP"),
                            new XElement("HallIP", Configure.Properties.Resources.HallIP),
                            new XComment("大厅服务器端口"),
                            new XElement("HallPort", Configure.Properties.Resources.HallPort),
                            new XComment("游戏下载服务器IP"),
                            new XElement("DownIP", Configure.Properties.Resources.DownIP),
                            new XComment("游戏下载服务器端口"),
                            new XElement("DownPort", Configure.Properties.Resources.DownPort),
                            new XComment("服务器更新IP"),
                            new XElement("UpdateIP", Configure.Properties.Resources.UpdateIP),
                            new XComment("服务器更新端口"),
                            new XElement("UpdatePort", Configure.Properties.Resources.UpdatePort),
                            new XComment("公钥"),
                            new XElement("PublicKey", Configure.Properties.Resources.PublicKey)
                                                    )));
                xDoc.Save(_config_path);
            }
            catch { throw (new Exception()); }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <returns></returns>
        static ConfigModel LoadFromFile()
        {
            if (!File.Exists(_config_path))
            {
                NewFile();
            }

            var param = from config in XElement.Load(_config_path).Descendants("Server")
                        select ConfigModel.CreateModel(
                           config.Element("LoginIP").Value,
                           config.Element("LoginPort").Value,
                           config.Element("HallIP").Value,
                           config.Element("HallPort").Value,
                           config.Element("DownIP").Value,
                           config.Element("DownPort").Value,
                           config.Element("UpdateIP").Value,
                           config.Element("UpdatePort").Value,
                           config.Element("PublicKey").Value
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
