using ResetModule.ViewModels;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace ResetModule.Views
{
    /// <summary>
    /// FindView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(ResetView))]
    public partial class ResetView : UserControl
    {
        public ResetView()
        {
            InitializeComponent();  
        }

        [Import]
        ResetViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
