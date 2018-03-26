using LoginModule.ViewModels;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using ToolClass.StoryBoard;

namespace LoginModule.Views
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(LoginView))]
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent(); 

            StoryboardManager.SetID((Storyboard)this.Resources["Story_Login"], "Story_Login"); 
        }

        [Import]
        LoginViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
