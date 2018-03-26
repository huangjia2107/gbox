using CardModule.ViewModels;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace CardModule.Views
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(CardView))]
    public partial class CardView : UserControl
    {
        public CardView()
        {
            InitializeComponent(); 
        }

        [Import]
        CardViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
