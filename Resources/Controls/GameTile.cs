using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Resources.Controls
{
    public class GameTile:Button
    {
        public GameTile()
        {
            DefaultStyleKey=typeof(GameTile);
        }

        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(GameTile), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleHeightProperty = DependencyProperty.Register("TitleHeight", typeof(double), typeof(GameTile), new PropertyMetadata(default(double)));

        public double TitleHeight
        {
            get { return (double)GetValue(TitleHeightProperty); }
            set { SetValue(TitleHeightProperty, value); }
        }

        public static readonly DependencyProperty TitleMarginProperty = DependencyProperty.Register("TitleMargin", typeof(Thickness), typeof(GameTile), new PropertyMetadata(default(Thickness)));

        public Thickness TitleMargin
        {
            get { return (Thickness)GetValue(TitleMarginProperty); }
            set { SetValue(TitleMarginProperty,value); }
        }

        public static readonly DependencyProperty ImgSourceProperty = DependencyProperty.Register("ImgSource", typeof(ImageSource), typeof(GameTile), new PropertyMetadata(default(string)));

        public ImageSource ImgSource
        {
            get { return (ImageSource)GetValue(ImgSourceProperty); }
            set { SetValue(ImgSourceProperty, value); }
        }
    }
}
