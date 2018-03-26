
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ConfigAccess
{
    public class VersionData
    {
        #region 变量

        //E:\\项目文件\\WPF\\G-Box\\G-Box\\bin\\Debug\\
        static string _config_path = AppDomain.CurrentDomain.BaseDirectory + "Config\\Version.xml";

        #endregion

        #region 方法

        /// <summary>
        /// 暴露于外部的方法
        /// </summary>
        /// <returns></returns>
        public static VersionModel GetConfig()
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
                    new XElement("Versions",
                        new XElement("Version",
                            new XComment("更新日期"),
                            new XElement("UpdateTime", ConfigAccess.Properties.Resources.UpdateTime),
                            new XComment("当前版本"),
                            new XElement("AppVs", ConfigAccess.Properties.Resources.AppVs)
                                                    )));
                xDoc.Save(_config_path);
            }
            catch { throw (new Exception()); }
        }

        /// <summary>
        /// 加载数据
        /// </summary>
        /// <returns></returns>
        static VersionModel LoadFromFile()
        {
            if (!File.Exists(_config_path))
            { 
                NewFile();
            }

            var param = from config in XElement.Load(_config_path).Descendants("Version")
                        select VersionModel.CreateModel(
                           config.Element("UpdateTime").Value,
                           config.Element("AppVs").Value
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
