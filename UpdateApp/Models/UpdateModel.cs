using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UpdateApp
{
    public class UpdateModel
    {
        protected UpdateModel() //构造函数
        { }

        public static UpdateModel CreateNewModel()
        {
            return new UpdateModel();
        }

        public static UpdateModel CreateModel(string filename, string movetopath)
        {
            return new UpdateModel
            {
                FileName=filename,
                MoveToPath=movetopath
            };
        }

        public string FileName { get; set; }
        public string MoveToPath { get; set; } 
    }
}
