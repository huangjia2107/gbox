using System;
using System.Windows;
using System.Windows.Controls;

namespace Resources.Controls
{
    public class PicBtn:Button
    {
        public PicBtn()
        {
            DefaultStyleKey=typeof(PicBtn);
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(PicBtn), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
    }
}
