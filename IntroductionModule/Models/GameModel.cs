using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntroductionModule.Models
{
    public class GameModel
    {
        protected GameModel() //构造函数
        { }

        public static GameModel CreateNewModel()
        {
            return new GameModel();
        }

        public static GameModel CreateModel(string id,string gamename, string gametype, string publishdata,string icon,bool isenabled,string imgsrc1,string imgsrc2,string gamedetail)
        {
            return new GameModel
            {
                ID=id,
                GameName=gamename, 
                GameType=gametype,
                PublishData=publishdata,
                Icon=icon,
                IsEnabled=isenabled,
                ImgSrc1=imgsrc1,
                ImgSrc2=imgsrc2,
                GameDetail=gamedetail
            };
        }

        public string ID { get; set; }
        public string GameName { get; set; } 
        public string GameType { get; set; }
        public string PublishData { get; set; }
        public string Icon { get; set; }
        public bool IsEnabled { get; set; }
        public string ImgSrc1 { get; set; }
        public string ImgSrc2 { get; set; }
        public string GameDetail { get; set; }
    }
}
