using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ConfigAccess
{
    public class ConfigData
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
                            new XElement("LoginIP", ConfigAccess.Properties.Resources.LoginIP),
                            new XComment("登陆服务器端口"),
                            new XElement("LoginPort", ConfigAccess.Properties.Resources.LoginPort),
                            new XComment("大厅服务器IP"),
                            new XElement("HallIP", ConfigAccess.Properties.Resources.HallIP),
                            new XComment("大厅服务器端口"),
                            new XElement("HallPort", ConfigAccess.Properties.Resources.HallPort),
                            new XComment("游戏下载服务器IP"),
                            new XElement("DownIP", ConfigAccess.Properties.Resources.DownIP),
                            new XComment("游戏下载服务器端口"),
                            new XElement("DownPort", ConfigAccess.Properties.Resources.DownPort),
                            new XComment("服务器更新IP"),
                            new XElement("UpdateIP", ConfigAccess.Properties.Resources.UpdateIP),
                            new XComment("服务器更新端口"),
                            new XElement("UpdatePort", ConfigAccess.Properties.Resources.UpdatePort),
                            new XComment("服务器更新路径"),
                            new XElement("UpdatePath", ConfigAccess.Properties.Resources.UpdatePath),
                            new XComment("私钥"),
                            new XElement("PrivateKey", ConfigAccess.Properties.Resources.PrivateKey)
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
                           config.Element("UpdatePath").Value,
                           config.Element("PrivateKey").Value
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

        public enum Type {login,hall,down,update};

        /// <summary>
        /// 配置服务器参数
        /// </summary> 
        public static void SetConfig(string ip,string port,Type type)
        {
            XElement xe = XElement.Load(_config_path);
            switch(type)
            {
                case Type.login:

                    var Config = from config in xe.Descendants("Server")
                                 where config.HasElements == true
                                 select config;
                    foreach (var config in Config)
                    {
                        config.Element("LoginIP").Value = ip;
                        config.Element("LoginPort").Value=port;
                    }

                    break;

                case Type.hall:

                    var Config1 = from config in xe.Descendants("Server")
                                 where config.HasElements == true
                                 select config;
                    foreach (var config in Config1)
                    {
                        config.Element("HallIP").Value = ip;
                        config.Element("HallPort").Value = port;
                    }

                    break;

                case Type.down:

                    var Config2 = from config in xe.Descendants("Server")
                                  where config.HasElements == true
                                  select config;
                    foreach (var config in Config2)
                    {
                        config.Element("DownIP").Value = ip;
                        config.Element("DownPort").Value = port;
                    }

                    break;

                case Type.update:

                    var Config3 = from config in xe.Descendants("Server")
                                 where config.HasElements == true
                                 select config;
                    foreach (var config in Config3)
                    {
                        config.Element("UpdateIP").Value = ip;
                        config.Element("UpdatePort").Value = port;
                    }

                    break;
            }

            xe.Save(_config_path);
        }

        /// <summary>
        /// 恢复服务器默认参数
        /// </summary> 
        public static void SetRestore(Type type)
        {
            XElement xe = XElement.Load(_config_path);
            switch (type)
            {
                case Type.login:

                    var Config = from config in xe.Descendants("Server")
                                 where config.HasElements == true
                                 select config;
                    foreach (var config in Config)
                    {
                        config.Element("LoginIP").Value = ConfigAccess.Properties.Resources.LoginIP;
                        config.Element("LoginPort").Value = ConfigAccess.Properties.Resources.LoginPort;
                    }

                    break;

                case Type.hall:

                    var Config1 = from config in xe.Descendants("Server")
                                  where config.HasElements == true
                                  select config;
                    foreach (var config in Config1)
                    {
                        config.Element("HallIP").Value = ConfigAccess.Properties.Resources.HallIP;
                        config.Element("HallPort").Value = ConfigAccess.Properties.Resources.HallPort;
                    }

                    break;

                case Type.down:

                    var Config2 = from config in xe.Descendants("Server")
                                  where config.HasElements == true
                                  select config;
                    foreach (var config in Config2)
                    {
                        config.Element("DownIP").Value = ConfigAccess.Properties.Resources.DownIP;
                        config.Element("DownPort").Value = ConfigAccess.Properties.Resources.DownPort;
                    }

                    break;

                case Type.update:

                    var Config3 = from config in xe.Descendants("Server")
                                  where config.HasElements == true
                                  select config;
                    foreach (var config in Config3)
                    {
                        config.Element("UpdateIP").Value = ConfigAccess.Properties.Resources.UpdateIP;
                        config.Element("UpdatePort").Value = ConfigAccess.Properties.Resources.UpdatePath;
                    }

                    break;
            }

            xe.Save(_config_path);
        }

        #endregion
    }
}
