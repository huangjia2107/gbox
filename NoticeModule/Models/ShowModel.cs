using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoticeModule.Models
{
    public class ShowModel
    {
        protected ShowModel() //构造函数
        { }

        public static ShowModel CreateNewModel()
        {
            return new ShowModel();
        }

        public static ShowModel CreateModel(string id,string picsrc, string title)
        {
            return new ShowModel
            { 
                ID=id,
                PicSrc=picsrc,
                Title=title
            };
        }

        public string ID { get; set; }
        public string PicSrc { get; set; }
        public string Title { get; set; }
        
    }
}
