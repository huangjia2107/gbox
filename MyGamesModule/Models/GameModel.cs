using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGamesModule.Models
{
    public class GameModel
    {
        protected GameModel() //构造函数
        { }

        public static GameModel CreateNewModel()
        {
            return new GameModel();
        }

        public static GameModel CreateModel(string id, string gamename,string modulename ,string icon)
        {
            return new GameModel
            {
                ID = id,
                GameName = gamename, 
                ModuleName=modulename,
                Icon = icon 
            };
        }

        public string ID { get; set; }
        public string GameName { get; set; }
        public string ModuleName { get; set; }
        public string Icon { get; set; }
    }
}
