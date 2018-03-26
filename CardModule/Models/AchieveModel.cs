using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardModule.Models
{
    public class AchieveModel
    {
        protected AchieveModel() //构造函数
        { }

        public static AchieveModel CreateNewModel()
        {
            return new AchieveModel();
        }

        public static AchieveModel CreateModel(string gamename, string level, string rank,string total,string single)
        {
            return new AchieveModel
            {
                GameName=gamename,
                Level=level,
                Rank=rank,
                Total=total,
                Single=single
            };
        }

        public string GameName { get; set; }
        public string Level { get; set; }
        public string Rank { get; set; }
        public string Total { get; set; }
        public string Single { get; set; }
    }
}
