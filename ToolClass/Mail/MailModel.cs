using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolClass.Mail
{
    class MailModel
    {
        protected MailModel() //构造函数
        { }

        public static MailModel CreateNewModel()
        {
            return new MailModel();
        }

        public static MailModel CreateModel(string from, string fromname, string username, string password, string mailserver)
        {
            return new MailModel
            {
                From=from,
                FromName=fromname,
                UserName=username,
                PassWord=password,
                MailServer=mailserver
            };
        }

        public string From { get; set; }
        public string FromName { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string MailServer { get; set; }
    }
}
