using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NoticeModule.Models;
using ToolClass.LinkedList;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace NoticeModule.DataAccess
{
    public class ShowAccess
    {
        #region 变量

        //E:\\项目文件\\WPF\\G-Box\\G-Box\\bin\\Debug\\
        static string _games_path = AppDomain.CurrentDomain.BaseDirectory + "Modules\\Games.dll";

        static LoopLink<ShowModel> _loopLinks;

        #endregion

        #region 构造函数

        public ShowAccess()
        {
            _loopLinks = new LoopLink<ShowModel>();
            LoadFromShow();
        }

        #endregion

        #region 方法

        static void LoadFromShow()
        {
            if (!File.Exists(_games_path))
            {
                _loopLinks= null;
                return;
            }

            Assembly myAssembly = Assembly.LoadFrom("Modules\\Games.dll");
            using (Stream stream = myAssembly.GetManifestResourceStream("Games.XmlFile.PicFile.xml"))
            using (XmlReader xmlReader = new XmlTextReader(stream))
            {
                var param = from file in XDocument.Load(xmlReader).Element("Pics").Elements("Pic")
                        select ShowModel.CreateModel( 
                             file.Attribute("id").Value,
                             file.Attribute("picsrc").Value,
                             file.Attribute("title").Value
                            );

                try
                {
                    foreach (var p in param)
                    {
                        _loopLinks.Add(p);
                    }
                }
                catch { }
            }
        }

        public static LoopLink<ShowModel> GetShow()
        {
            return _loopLinks;
        }

        #endregion
    }
}
