using System.Windows.Controls;

using RankModule.ViewModels;
using System.ComponentModel.Composition;

namespace RankModule.Views
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(RankView))]
    public partial class RankView : UserControl
    {
        public RankView()
        {
            InitializeComponent(); 
        }

        [Import]
        RankViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
