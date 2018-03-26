using LogolModule.ViewModels;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace LogolModule.Views
{
    /// <summary>
    /// LogolView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(LogolView))]
    public partial class LogolView : UserControl
    {
        public LogolView()
        {
            InitializeComponent(); 
        }

        [Import]
        LogolViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
