using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntroductionModule
{
    public class ListModel
    {
        protected ListModel() //构造函数
        { }

        public static ListModel CreateNewModel()
        {
            return new ListModel();
        }

        public static ListModel CreateModel(string id,string gamename,string downname, string downsize, string downprogress,string downspeed)
        {
            return new ListModel
            { 
                ID=id,
                GameName=gamename,
                DownName=downname,
                DownSize=downsize,
                DownProgress=downprogress,
                DownSpeed=downspeed
            };
        }

        public string ID { get; set; }
        public string GameName { get; set; }
        public string DownName { get; set; }
        public string DownSize { get; set; }
        public string DownProgress { get; set; }
        public string DownSpeed { get; set; } 
    }
}
