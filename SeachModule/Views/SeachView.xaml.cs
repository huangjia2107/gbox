using SeachModule.ViewModels;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace SeachModule.Views
{
    /// <summary>
    /// SeachView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(SeachView))]
    public partial class SeachView : UserControl
    {
        public SeachView()
        {
            InitializeComponent(); 
        }

        [Import]
        SeachViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
