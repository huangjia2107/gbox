using System.Windows.Controls;
using AllGamesModule.ViewModels;
using System.ComponentModel.Composition;

namespace AllGamesModule.Views
{
    /// <summary>
    /// AllGamesView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(AllGamesView))]
    public partial class AllGamesView : UserControl
    {
        public AllGamesView()
        {
            InitializeComponent(); 
        }

        [Import]
        AllGamesViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
