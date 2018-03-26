using NoticeModule.ViewModels;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace NoticeModule.Views
{
    /// <summary>
    /// PictureView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(NoticeView))]
    public partial class NoticeView : UserControl
    {
        public NoticeView()
        {
            InitializeComponent(); 
        }

        [Import]
        NoticeViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
