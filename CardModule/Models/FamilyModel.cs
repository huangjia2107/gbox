using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardModule.Models
{
    public class FamilyModel
    {
        protected FamilyModel() //构造函数
        { }

        public static FamilyModel CreateNewModel()
        {
            return new FamilyModel();
        }

        public static FamilyModel CreateModel(string familyid, string familycard, string familyname, string familyimg,string familygroup, bool familystatus, bool menuisenabled)
        {
            return new FamilyModel
            {
                FriendID=familyid,
                FriendCard=familycard,
                FriendName=familyname,
                FriendImg=familyimg,
                FriendGroup=familygroup,
                FriendStatus=familystatus,
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
