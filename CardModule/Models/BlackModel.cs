using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardModule.Models
{
    public class BlackModel
    {
        protected BlackModel() //构造函数
        { }

        public static BlackModel CreateNewModel()
        {
            return new BlackModel();
        }

        public static BlackModel CreateModel(string blackid, string blackcard, string blackname, string blackimg, string blackgroup, bool blackstatus, bool menuisenabled)
        {
            return new BlackModel
            {
                FriendID=blackid,
                FriendCard=blackcard,
                FriendName=blackname,
                FriendImg=blackimg,
                FriendGroup=blackgroup,
                FriendStatus=blackstatus,
                MenuIsEnabled=menuisenabled
            };
        }

        public string FriendID { get; set; }
        public string FriendCard { get; set; }
        public string FriendName { get; set; }
        public string FriendImg { get; set; }
        public string FriendGroup { get; set; }
        public bool FriendStatus { get; set; }
        public bool MenuIsEnabled { get; set; }
    }
}
