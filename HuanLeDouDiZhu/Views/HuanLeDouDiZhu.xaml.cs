using huanledoudizhu.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace huanledoudizhu.Views
{
    /// <summary>
    /// HuanLeDouDiZhuView.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(huanledoudizhu))]
    public partial class huanledoudizhu : UserControl
    {
        public huanledoudizhu()
        {
            InitializeComponent();
        }

        [Import]
        HLDDZViewModel ViewModel
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
