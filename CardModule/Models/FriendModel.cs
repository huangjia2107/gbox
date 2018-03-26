using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardModule.Models
{
    public class FriendModel
    {
        protected FriendModel() //构造函数
        { }

        public static FriendModel CreateNewModel()
        {
            return new FriendModel();
        }

        public static FriendModel CreateModel(string friendid, string friendcard, string friendname, string friendimg,string friendgroup, bool friendstatus, bool menuisenabled)
        {
            return new FriendModel
            {
                FriendID=friendid,
                FriendCard=friendcard,
                FriendName=friendname,
                FriendImg=friendimg,
                FriendGroup=friendgroup,
                FriendStatus=friendstatus,
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
