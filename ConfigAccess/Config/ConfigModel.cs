using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigAccess
{
    public class ConfigModel
    {
        protected ConfigModel() //构造函数
        { }

        public static ConfigModel CreateNewModel()
        {
            return new ConfigModel();
        }

        public static ConfigModel CreateModel(string loginip, string loginport, string hallip, string hallport,string downip,string downport, string updateip, string updateport,string updatepath, string privatekey)
        {
            return new ConfigModel
            {
                LoginIP = loginip,
                LoginPort = loginport,
                HallIP = hallip,
                HallPort = hallport,
                DownIP=downip,
                DownPort=downport,
                UpdateIP = updateip,
                UpdatePort = updateport,
                UpdatePath=updatepath,
                PrivateKey = privatekey
            };
        }

        public string LoginIP { get; set; }
        public string LoginPort { get; set; }
        public string HallIP { get; set; }
        public string HallPort { get; set; }
        public string DownIP { get; set; }
        public string DownPort { get; set; }
        public string UpdateIP { get; set; }
        public string UpdatePort { get; set; }
        public string UpdatePath { get; set; }
        public string PrivateKey { get; set; }
    }
}
