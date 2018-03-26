using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardModule.Models
{
    public class ResultModel
    {
        protected ResultModel() //构造函数
        { }

        public static ResultModel CreateNewModel()
        {
            return new ResultModel();
        }

        public static ResultModel CreateModel(string friendid,string friendcard, string friendname, string friendimg,bool IsCanAdd,bool IsOnline)
        {
            return new ResultModel
            {
                ResultID=friendid,
                ResultCard=friendcard,
                ResultName=friendname,
                ResultImg=friendimg,
                ResultIsCanAdd=IsCanAdd,
                ResultIsOnline=IsOnline
            };
        }

        public string ResultID { get; set; }
        public string ResultCard { get; set; }
        public string ResultName { get; set; }
        public string ResultImg { get; set; }
        public bool ResultIsCanAdd { get; set; }
        public bool ResultIsOnline { get; set; } 
    }
}
