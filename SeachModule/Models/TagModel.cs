using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeachModule.Models
{
    public class TagModel
    {
        protected TagModel() //构造函数
        { }

        public static TagModel CreateNewModel()
        {
            return new TagModel();
        }

        public static TagModel CreateModel(string id,string title, string fontcolor, double size)
        {
            return new TagModel
            {
                ID=id,
                Title=title,
                FontColor=fontcolor,
                Size=size
            };
        }

        public string ID { get; set; }
        public string Title { get; set; }
        public string FontColor { get; set; }
        public double Size { get; set; }
    }
}
