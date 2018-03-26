using System;
using System.Windows;

using G_Box.ViewModels;
using System.ComponentModel.Composition;

namespace G_Box
{
    /// <summary>
    /// Shell.xaml 的交互逻辑
    /// </summary>
    [Export]
    public partial class PreWindow : Window
    {
        public PreWindow()
        {          
            InitializeComponent();
        }

        [Import]
        PreViewModel ViewModel
        {
            set
            {
                this.DataContext = value;

                EventHandler close = null;
                close = delegate
                {
                    value.RequestClose -= close;
                    this.Hide();
                };
                value.RequestClose += close;
            }
        }
    }
}
