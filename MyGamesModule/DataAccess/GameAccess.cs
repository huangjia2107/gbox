using MyGamesModule.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MyGamesModule.DataAccess
{
    public class GameAccess
    {
        #region 变量

        //E:\\项目文件\\WPF\\G-Box\\G-Box\\bin\\Debug\\
        static string currentPath = AppDomain.CurrentDomain.BaseDirectory;

        static string _games_path = currentPath + "Modules\\Games.dll";
        static string _local_games_path = currentPath + "Games";
        static string _file_path = currentPath + "Games" + "\\mygame.xml";
         
        #endregion

        #region 私有方法

        /// <summary>
        /// 从game.dll文件中，通过文件名字获取
        /// </summary> 
        static GameModel LoadByName(string filename)
        {
            if (!File.Exists(_games_path))
                return null;

            Assembly myAssembly = Assembly.LoadFrom("Modules\\Games.dll");
            using (Stream stream = myAssembly.GetManifestResourceStream("Games.XmlFile.GamesFile.xml"))
            using (XmlReader xmlReader = new XmlTextReader(stream))
            {
                var param = from file in XDocument.Load(xmlReader).Element("Games").Elements("Game")
                            where file.Attribute("file").Value.ToLower() == filename.ToLower()
                            select GameModel.CreateModel(
                               file.Attribute("id").Value,
                               file.Attribute("title").Value,
                               file.Attribute("file").Value.Substring(0, file.Attribute("file").Value.Length - 4).ToLower(),
                               file.Attribute("icon").Value
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
        /// 从game.dll文件中，通过游戏ID获取
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
                               file.Attribute("file").Value.Substring(0, file.Attribute("file").Value.Length - 4).ToLower(),
                               file.Attribute("icon").Value
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
        /// 创建配置文件
        /// </summary>
        static void CreateFile()
        {
            try
            {
                XDocument xDoc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XComment("配置文件,请勿随意修改！"),
                    new XElement("Games"));

                xDoc.Save(_file_path);
            }
            catch { throw (new Exception()); }
        }

        /// <summary>
        /// 通过配置文件获取所有游戏信息
        /// </summary>
        /// <returns></returns>
        static List<RecordModel> GetGameByFile()
        {
            if (!File.Exists(_file_path))
                CreateFile();
             
            return (from file in XDocument.Load(_file_path).Element("Games").Elements("Game")
                    select RecordModel.CreateModel(
                       file.Attribute("file").Value,
                       file.Attribute("time").Value
                       )
                    ).ToList();
        }

        /// <summary>
        /// 从配置文件中删除游戏记录
        /// </summary> 
        static void DeleteByName(string gamename)
        {
            try
            {
                XElement xDoc = XElement.Load(_file_path);
                var AllGame = from aName in xDoc.Elements("Game")
                              where (string)aName.Attribute("file") == gamename + ".dll"
                              select aName;

                AllGame.Remove();
                //foreach (var aname in AllGame)
                //{
                //    aname.RemoveAll();
                //}

                xDoc.Save(_file_path);
            }
            catch { }

            try
            {
               // File.Delete(_local_games_path+"\\"+gamename + ".dll");
                System.Diagnostics.Process.Start("del /f " + _local_games_path + "\\" + gamename + ".dll");
            }
            catch { }
        }

        /// <summary>
        /// 添加游戏到配置文件记录
        /// </summary> 
        static bool AddToFile(string gamename,string gametime)
        {
            try
            {
                XElement xDoc = XElement.Load(_file_path);
                XElement xel = new XElement("Game", new XAttribute("file", gamename+".dll"),
                                                    new XAttribute("time",gametime));

                xDoc.Add(xel);
                xDoc.Save(_file_path);

                return true;
            }
            catch { }

            return false;
        }

        static string GetGameTimeByFile(string gamename)
        {
            try
            {
                XElement xDoc = XElement.Load(_file_path);
                var AllGame = from aName in xDoc.Elements("Game")
                              where (string)aName.Attribute("file") == gamename + ".dll"
                              select aName;

                foreach (var aname in AllGame)
                {
                    return aname.Attribute("time").Value;
                }

                return null;
            }
            catch { return null; }
        }

        #endregion

        #region 公有方法

        /// <summary>
        /// 返回所有游戏集合
        /// </summary>
        /// <returns></returns>
        public static List<GameModel> GetGame()
        {
            List<GameModel> listModel = new List<GameModel>();

            foreach (RecordModel model in GetGameByFile())
            {
                listModel.Add(LoadByName(model.FileName));
            }

            listModel.RemoveAll(game => game == null);

            return listModel;
        }

        /// <summary>
        /// 根据ID获取某一游戏
        /// </summary>  
        public static GameModel GetGame(string id,string gametime)
        {
            GameModel gameModel = LoadByID(id);

            if (gameModel != null)
            {
                AddToFile(gameModel.ModuleName,gametime);
            }
            return LoadByID(id);
        }

        /// <summary>
        /// 删除某一游戏
        /// </summary> 
        public static void DeleteGame(string gamename)
        {
            DeleteByName(gamename);
        }

        /// <summary>
        /// 获取版本时间
        /// </summary> 
        public static string GetGameTime(string gamename)
        {
            return GetGameTimeByFile(gamename);
        }

        #endregion
    }
}
