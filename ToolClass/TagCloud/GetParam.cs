using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolClass.TagCloud
{
    public class GetParam
    {
        public static string GetColor(int i)
        {
            if (i < 0 || i > 6)
                return "#000000";

            string[] colors = { "#FF0000", "#FF6600", "#FF9933", "#0000FF", "#000066", "#000000" };
            return colors[--i];
        }

        public static double GetSize(int i)
        {
            if (i < 0 || i > 6)
                return 8;

            double[] sizes = { 28, 24, 20, 16, 12, 8 };
            return sizes[--i];
        }
    }
}
