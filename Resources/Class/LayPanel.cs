using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Input;

namespace ListBox_ImagePanel
{
    class LayPanel:Canvas
    {
        private Size viewport = new Size();
        int space = 5;
        double item_w = 50.00;
        double item_h = 50.00;

        public LayPanel() {
			this.Background = Brushes.White;
		}

		protected override Size MeasureOverride(Size constraint) {
			foreach (UIElement each in InternalChildren) {
                each.Measure(constraint);
			}

			if (viewport != constraint) {
				viewport = constraint;
			}
            return new Size(item_w, item_h);
		}

		protected override Size ArrangeOverride(Size arrangeSize) {
            int row_count = (int)Math.Floor(viewport.Width / item_w);  //一行的item个数
            int colmn_count = (int)Math.Floor(viewport.Height / item_h);
			int temp = 0;
			int row = 0;
			try {
				for (int i = 0; i < row_count; i++) 
                {
                    if (i > InternalChildren.Count) 
                        break;
                    this.InternalChildren[i].Arrange(new Rect((item_w + space) * i + space, space, item_w, item_h));
					temp++;
					if (temp == row_count)
                    {
                        row++;
                        layPanel(++i,row_count,InternalChildren.Count,row);  //递归
					}
				}
			} catch (ArgumentOutOfRangeException) { }

            Size s = new Size(row_count * (item_h + space) + space, colmn_count * (item_w+space) + space);
            if (arrangeSize != s)
                arrangeSize = s;
			
            return arrangeSize;
		}

        protected void layPanel(int i,int row_count,int all_count,int row)
        {
            int temp = 0;
            for (int j = i; j < row_count*(row+1); j++)
            {
                if (j > all_count)
                    break;
                this.InternalChildren[j].Arrange(new Rect((item_w + space) * temp + space, row * (item_w + space) + space, item_w, item_h));
                temp++;
                if (temp == row_count)
                {
                    row++;
                    layPanel(++j,row_count, all_count,row);
                }
            }
        }
    }
}
