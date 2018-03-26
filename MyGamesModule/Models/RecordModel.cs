using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGamesModule.Models
{
    public class RecordModel
    {
        protected RecordModel() //构造函数
        { }

        public static RecordModel CreateNewModel()
        {
            return new RecordModel();
        }

        public static RecordModel CreateModel(string filename,string gametime)
        {
            return new RecordModel
            {
                FileName = filename,
                GameTime=gametime
            };
        }

        public string FileName { get; set; }
        public string GameTime { get; set; }
    }
}
