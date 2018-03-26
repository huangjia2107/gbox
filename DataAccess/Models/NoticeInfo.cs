using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Models
{
    [Serializable]
    public class NoticeInfo
    {
        public string MsgID { get; set; }
        public string MsgType = "M";
        public string MsgTitle { get;set; }
        public string MsgContent { get; set; }
        public string MsgPublish = "系统";
    }
}
