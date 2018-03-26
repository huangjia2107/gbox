using System.Windows.Controls;
using IntroductionModule.ViewModels;
using System.ComponentModel.Composition;

namespace IntroductionModule.Views
{
    /// <summary>
    /// IntrductionView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IntroductionView))]
    public partial class IntroductionView : UserControl
    {
        public IntroductionView()
        {
            InitializeComponent(); 
        }

        [Import]
        IntroductionViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
