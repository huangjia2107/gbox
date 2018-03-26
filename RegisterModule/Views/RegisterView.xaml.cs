using System.Windows.Controls;

using RegisterModule.ViewModels;
using System.ComponentModel.Composition;

namespace RegisterModule.Views
{
    /// <summary>
    /// RegisterView.xaml 的交互逻辑
    /// </summary> 
    [Export(typeof(RegisterView))]
    public partial class RegisterView : UserControl
    {
        public RegisterView()
        {
            InitializeComponent(); 
        }

        [Import]
        RegisterViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
