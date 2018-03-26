using doudizhu.Class;
using doudizhu.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace doudizhu.Views
{
    /// <summary>
    /// DouDiZhuView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(doudizhu))]
    public partial class doudizhu : UserControl
    {
        public doudizhu()
        {
            InitializeComponent();

            Add();
        }

        [Import]
        DDZViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }

        MyButton[,] maps = new MyButton[10, 10];
        int Amount;
        int Count;
        public void Add()
        {
            for (int i = 0; i < 10; i++)
            {
                RowDefinition rowDef = new RowDefinition();
                GridGame.RowDefinitions.Add(rowDef);

                ColumnDefinition colDef = new ColumnDefinition();
                GridGame.ColumnDefinitions.Add(colDef);
            }
            Count = 8;

            Amount = 32;
            List<int> ran = (new MyRandom(64, 2, 1, Count)).Get();
            for (int r = 0; r < 10; r++)
            {
                for (int c = 0; c < 10; c++)
                {
                    MyButton btn;

                    if (r == 0 || r == 9 || c == 0 || c == 9)
                    {
                        btn = new MyButton(0, r, c);
                        btn.Visibility = System.Windows.Visibility.Hidden;
                    }
                    else
                        btn = new MyButton((int)ran[(r - 1) * 8 + c - 1], r, c);

                    btn.Click += btn_Click;

                    maps[r, c] = btn;

                    Grid.SetColumn(btn, c);
                    Grid.SetRow(btn, r);
                    GridGame.Children.Add(btn);
                }
            }
        }
        public void Clear()
        {
            this.GridGame.Children.Clear();
            GridGame.RowDefinitions.Clear();
            GridGame.ColumnDefinitions.Clear();
        }

        MyButton btn1;
        MyButton btn2;
        int ClickCount = 0;
        void btn_Click(object sender, RoutedEventArgs e)
        {
            ++ClickCount;
            if (ClickCount == 1)
            {
                btn1 = sender as MyButton;
            }
            else if (ClickCount == 2)
            {
                btn2 = sender as MyButton;

                if (btn1 != null && btn2 != null)
                {
                    if (btn1.Flag == btn2.Flag && !(btn1.ColNum == btn2.ColNum && btn1.RowNum == btn2.RowNum))
                    {
                        if (Check(btn1, btn2))
                        {
                            btn1.Visibility = Visibility.Hidden;
                            btn2.Visibility = Visibility.Hidden;

                            maps[btn1.RowNum, btn1.ColNum].Visibility = Visibility.Hidden;
                            maps[btn2.RowNum, btn2.ColNum].Visibility = Visibility.Hidden;
                        }
                    }

                }
                btn1 = null;
                btn2 = null;
                ClickCount = 0;
            }
        }

        private bool Check(MyButton btn1, MyButton btn2)
        {
            for (int c = 0; c < 10; c++)
            {
                if (c != btn1.ColNum && !(btn1.RowNum == btn2.RowNum && btn1.ColNum == c) && maps[btn1.RowNum, c].Visibility == Visibility.Visible)   //不为A和B的障碍
                    continue;
                if (!IsLine(btn1.RowNum, btn2.RowNum, btn1.RowNum, c))  //Aa
                    continue;
                for (int c2 = 0; c2 < 10; c2++)
                {
                    if (c2 != btn2.ColNum && !(btn1.RowNum == btn2.RowNum && btn1.ColNum == c2) && maps[btn2.RowNum, c2].Visibility == Visibility.Visible)
                        continue;
                    if (!IsLine(btn1.RowNum, c, btn1.RowNum, c2))   //ab
                        continue;
                    if (!IsLine(btn2.RowNum, c2, btn2.RowNum, btn2.ColNum)) //bB
                        continue;
                    return true;
                }
                for (int r = 0; r < 10; r++)
                {
                    if (r != btn2.RowNum && !(r == btn1.RowNum && btn2.ColNum == btn1.ColNum) && maps[r, btn2.ColNum].Visibility == Visibility.Visible)
                        continue;
                    if (!IsLine(btn1.RowNum, c, r, btn2.ColNum))    //ab
                        continue;
                    if (!IsLine(r, btn2.ColNum, btn2.RowNum, btn2.ColNum))  //bB
                        continue;
                    return true;
                }
            }
            for (int r = 0; r < 10; r++)
            {
                if (r != btn1.RowNum && !(r == btn2.RowNum && btn1.ColNum == btn2.ColNum) && maps[r, btn1.RowNum].Visibility == Visibility.Visible)   //不为A和B的障碍
                    continue;
                if (!IsLine(btn1.RowNum, btn1.ColNum, r, btn1.ColNum))  //Aa
                    continue;
                for (int c2 = 0; c2 < 10; c2++)
                {
                    if (c2 != btn2.ColNum && maps[btn2.RowNum, c2].Visibility == Visibility.Visible)
                        continue;
                    if (!IsLine(r, btn1.RowNum, btn2.RowNum, c2))   //ab
                        continue;
                    if (!IsLine(btn2.RowNum, c2, btn2.RowNum, btn2.ColNum)) //bB
                        continue;
                    return true;
                }
                for (int r2 = 0; r2 < 10; r2++)
                {
                    if (r2 != btn2.RowNum && !(r2 == btn1.RowNum && btn2.ColNum == btn1.ColNum) && maps[r2, btn2.ColNum].Visibility == Visibility.Visible)
                        continue;
                    if (!IsLine(r, btn1.ColNum, r2, btn2.ColNum))    //ab
                        continue;
                    if (!IsLine(r2, btn2.ColNum, btn2.RowNum, btn2.ColNum))  //bB
                        continue;
                    return true;
                }
            }
            return false;
        }
        private bool IsLine(int r1, int c1, int r2, int c2)
        {
            int max, min;
            if (r1 == r2)   //同行
            {
                max = ((c1 > c2) ? c1 : c2) - 1;
                min = ((c1 > c2) ? c2 : c1) + 1;
                while (min <= max)
                {
                    if (maps[r1, min].Visibility == Visibility.Visible)
                        return false;
                    min++;
                }
                return true;
            }
            if (c1 == c2)   //同列
            {
                max = ((r1 > r2) ? r1 : r2) - 1;
                min = ((r1 > r2) ? r2 : r1) + 1;
                while (min <= max)
                {
                    if (maps[min, c1].Visibility == Visibility.Visible)
                        return false;
                    min++;
                }
                return true;
            }
            return false;
        } 

        private void Continue_Click_1(object sender, RoutedEventArgs e)
        {
            Clear();
            Add();
        }
    }
}
