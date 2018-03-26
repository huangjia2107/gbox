using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;
using System.Windows;
using XWT = Xceed.Wpf.Toolkit;
using System.ComponentModel.Composition;

namespace LogolModule.ViewModels
{
    [Export]
    public class LogolViewModel
    {
        #region 变量

        IRegionManager regionManager;

        #endregion

        #region 构造函数

        [ImportingConstructor]
        public LogolViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        #endregion

        #region 绑定的命令

        DelegateCommand closeCommand { get; set; }

        public ICommand CloseCommand
        {
            get
            {
                if (closeCommand == null)
                    closeCommand = new DelegateCommand(OnCloseExcute);

                return closeCommand;
            }
        }

        void OnCloseExcute()
        {
            MessageBoxResult dr;
            dr =XWT.MessageBox.Show("确认退出系统？","提示",MessageBoxButton.YesNo,MessageBoxImage.Question);
            switch (dr)
            {
                case MessageBoxResult.Yes:

                    Application.Current.Shutdown();

                    break;

                case MessageBoxResult.No:
                    ;break;
            }
        }

        #endregion
    }
}
