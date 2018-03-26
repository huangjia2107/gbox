using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SeachModule.Models;
using System.Reflection;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using ToolClass.TagCloud;

namespace SeachModule.DataAccess
{
    public class TagAccess
    {
        #region 变量

        //E:\\项目文件\\WPF\\G-Box\\G-Box\\bin\\Debug\\
        static string _games_path = AppDomain.CurrentDomain.BaseDirectory + "Modules\\Games.dll";

        readonly List<TagModel> _tagModels;

        #endregion

        #region 构造函数

        public TagAccess()
        {
            _tagModels = LoadFromTag();
        }

        #endregion

        #region 方法

        static List<TagModel> LoadFromTag()
        {
            if (!File.Exists(_games_path))
                return null;

            Assembly myAssembly = Assembly.LoadFrom("Modules\\Games.dll");
            using (Stream stream = myAssembly.GetManifestResourceStream("Games.XmlFile.TagsFile.xml"))
            using (XmlReader xmlReader = new XmlTextReader(stream))
            {
                return (from file in XDocument.Load(xmlReader).Element("Tags").Elements("Tag")
                        select TagModel.CreateModel(
                            file.Attribute("id").Value,
                            file.Attribute("title").Value,
                            GetParam.GetColor((int)file.Attribute("weight")),
                            GetParam.GetSize((int)file.Attribute("weight"))
                            )
                   ).ToList();
            }
        }

        public List<TagModel> GetTag()
        {
            return new List<TagModel>(_tagModels);
        }

        #endregion
    }
}
