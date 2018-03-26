using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace doudizhu.Class
{
    public class MyButton : Button
    {
        //图片的名称，用于判断两个按钮是否为同一个
        public int Flag { get; set; }

        //Button所在行
        public int RowNum { get; set; }

        //所在列
        public int ColNum { get; set; }

        public MyButton(int flag, int romNum, int colNum)
        {
            this.Flag = flag;
            this.RowNum = romNum;
            this.ColNum = colNum;

            if (flag != 0)
            { 
                Image img = new Image();
            img.Source = new BitmapImage(new Uri(@"pack://application:,,,/doudizhu;component/Images/" + flag + ".png", UriKind.RelativeOrAbsolute));
                this.Content = img;
            }
        }
    }
}
