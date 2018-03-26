using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Configure.Models
{
    public class VersionModel
    {
        protected VersionModel() //构造函数
        { }

        public static VersionModel CreateNewModel()
        {
            return new VersionModel();
        }

        public static VersionModel CreateModel(string updatetime, string appvs)
        {
            return new VersionModel
            {
                UpdateTime=updatetime,
                AppVs=appvs
            };
        }

        public string UpdateTime { get; set; }
        public string AppVs { get; set; } 
    }
}
