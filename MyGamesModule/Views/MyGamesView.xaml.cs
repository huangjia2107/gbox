using System.Windows.Controls;

using MyGamesModule.ViewModels;
using System.ComponentModel.Composition;

namespace MyGamesModule.Views
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(MyGamesView))]
    public partial class MyGamesView : UserControl
    {
        public MyGamesView()
        {
            InitializeComponent(); 
        }

        [Import]
        MyGamesViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
