using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Input;

namespace G_Box.OtherClass
{
    class ListBoxPanel : Canvas {

        private Size extent = new Size();
        private Size viewport = new Size();
        private TranslateTransform translate = new TranslateTransform();
        private double x;
        private TranslateTransform transform;
        private int page;
        private int index;

		public ListBoxPanel() {
			this.RenderTransform = translate;
			this.MouseDown += new MouseButtonEventHandler(Panel_MouseDown);
			this.MouseUp += new MouseButtonEventHandler(Panel_MouseUp);
			this.RenderTransform = (transform = new TranslateTransform());
		}

		private void Panel_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			this.CaptureMouse();
            this.Cursor = Cursors.Hand;
			Point p = e.GetPosition(this);
			x = p.X;
		}

		private void Panel_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            this.Cursor = Cursors.Arrow;
			if (this.IsMouseCaptured) {
				Point p = e.GetPosition(this);
				var offsetX = p.X - x;
				if (offsetX > 10) {
					if (index > 0) { --index; }
					Go();
				} if (offsetX < -10) {
					if (index < page - 1) { ++index; }
					Go();
				}
				this.ReleaseMouseCapture();
			}
		}
		private void Go() {
			DoubleAnimation a = new DoubleAnimation(-index * 380, TimeSpan.FromMilliseconds(700));
			a.AccelerationRatio = .3;
			a.DecelerationRatio = .3;
			transform.BeginAnimation(TranslateTransform.XProperty, a);
		}

        protected override Size MeasureOverride(Size constraint)
        {
            double dWidth = Math.Floor(constraint.Width / 380.00);
            double dHeight = Math.Floor(constraint.Height / 250.00);
            Size s = new Size(Math.Ceiling(InternalChildren.Count / (dWidth * dHeight)) * constraint.Width, constraint.Height);

            Size extentTmp = new Size(s.Width * this.InternalChildren.Count, constraint.Height);
            foreach (UIElement each in InternalChildren)
            {
                each.Measure(new Size(380, 250));
            }
            if (extentTmp != extent)
            {
                extent = s;
            }
            if (viewport != constraint)
            {
                viewport = constraint;
            }
            return s;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            int count = (int)Math.Floor(viewport.Width / 380.00);
            page = (int)Math.Ceiling((decimal)InternalChildren.Count / count);
            int temp = 0;
            int n = 1;   // 呈列一排
            int countView = 0;
            try
            {
                for (int i = 0; i < InternalChildren.Count; i++)
                {
                    this.InternalChildren[i].Arrange(new Rect((380 * (i - countView * count)) + (viewport.Width * countView) + ((viewport.Width - count * 380) / 2), 0, 380, 250));
                    temp++;
                    if (temp > count)
                    {
                        for (int j = i; j < n * count; j++)
                        {
                            this.InternalChildren[j].Arrange(new Rect(380 * (j - count - countView * count) + (viewport.Width * countView) + ((viewport.Width - count * 380) / 2), 250, 380, 250));
                            i = j;
                        }
                        countView++;
                        n += 1;
                        temp = 0;
                    }
                }
            }
            catch (ArgumentOutOfRangeException) { }
            return arrangeSize;
        }		
	}
}
