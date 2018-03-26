using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IntroductionModule.Models;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace IntroductionModule.DataAccess
{
    public class GameAccess
    {
        #region 变量

        //E:\\项目文件\\WPF\\G-Box\\G-Box\\bin\\Debug\\
        static string currentPath = AppDomain.CurrentDomain.BaseDirectory;

        static string _games_path = currentPath + "Modules\\Games.dll";
        static string _local_games_path = currentPath + "Games";
        static string _file_path = currentPath + "Games\\mygame.xml";

        #endregion

        #region 方法

        /// <summary>
        /// 通过游戏名字获取
        /// </summary> 
        static GameModel LoadByName(string gamename)
        {
            if (!File.Exists(_games_path))
                return null;

            Assembly myAssembly = Assembly.LoadFrom("Modules\\Games.dll");
            using (Stream stream = myAssembly.GetManifestResourceStream("Games.XmlFile.GamesFile.xml"))
            using (XmlReader xmlReader = new XmlTextReader(stream))
            {
                var param = from file in XDocument.Load(xmlReader).Element("Games").Elements("Game")
                            where (string)file.Attribute("title") == gamename
                            select GameModel.CreateModel(
                               file.Attribute("id").Value,
                               file.Attribute("title").Value,
                               file.Attribute("type").Value,
                               file.Attribute("data").Value,
                               file.Attribute("icon").Value,
                               !IsExists(file.Attribute("file").Value.ToLower()),
                               file.Attribute("picpath1").Value,
                               file.Attribute("picpath2").Value,
                               file.Attribute("detail").Value
                               );
                try
                {
                    foreach (var p in param)
                    {
                        return p;
                    }
                }
                catch { }

                return null;
            }
        }

        /// <summary>
        /// 通过游戏ID获取
        /// </summary> 
        static GameModel LoadByID(string id)
        {
            if (!File.Exists(_games_path))
                return null;

            Assembly myAssembly = Assembly.LoadFrom("Modules\\Games.dll");
            using (Stream stream = myAssembly.GetManifestResourceStream("Games.XmlFile.GamesFile.xml"))
            using (XmlReader xmlReader = new XmlTextReader(stream))
            {
                var param = from file in XDocument.Load(xmlReader).Element("Games").Elements("Game")
                            where (string)file.Attribute("id") == id
                            select GameModel.CreateModel(
                               file.Attribute("id").Value,
                               file.Attribute("title").Value,
                               file.Attribute("type").Value,
                               file.Attribute("data").Value,
                               file.Attribute("icon").Value,
                               !IsExists(file.Attribute("file").Value.ToLower()),
                               file.Attribute("picpath1").Value,
                               file.Attribute("picpath2").Value,
                               file.Attribute("detail").Value
                               );
                try
                {
                    foreach (var p in param)
                    {
                        return p;
                    }
                }
                catch { }

                return null;
            }
        }

        /// <summary>
        /// 查看游戏文件是否存在于本地
        /// </summary>
        /// <param name="gamename"></param>
        /// <returns></returns>
        static bool IsExists(string filename)
        {
            if (File.Exists(_file_path))
            {
                XElement xDoc = XElement.Load(_file_path);

                if (xDoc.Elements("Game").ToList().Count != 0)
                {
                    var AllUser = from aName in xDoc.Elements("Game")
                                  where (string)aName.Attribute("file") == filename
                                  select aName;

                    if (AllUser.ToList().Count!= 0)
                        return true;
                    else
                        return false; //文件不存在
                }

                return false;
            }
            else
            {
                CreateFile();
                return false; //文件不存在
            }
        }

        /// <summary>
        /// 创建配置文件
        /// </summary>
        static bool CreateFile()
        {
            try
            {
                XDocument xDoc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XComment("配置文件,请勿随意修改！"),
                    new XElement("Games"));

                xDoc.Save(_file_path);

                return true;
            }
            catch { }

            return false;
        }

        public static GameModel GetGameByName(string gamename)
        {
            return LoadByName(gamename);
        }

        public static GameModel GetGameByID(string id)
        {
            return LoadByID(id);
        }

        #endregion
    }
}
