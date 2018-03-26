using MessageModule.RegionTypes;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using XWT = Xceed.Wpf.Toolkit;

namespace MyGamesModule.ViewModels
{
    public class OpenViewModel : NotificationObject
    {
        #region 变量
         
        IModuleManager moduleManager;
        Border border=null;

        #endregion

        #region 构造函数

        [ImportingConstructor]
        public OpenViewModel(IModuleManager moduleManager,Border border)
        {
            this.moduleManager = moduleManager;
            this.border = border;
            this.Title =  border.ToolTip.ToString();

            RegionTypes.ModuleRegion = border.Tag.ToString();
            this.moduleManager.LoadModule(border.Tag.ToString());
        } 

        #endregion

        #region 绑定的属性 

        string title; 

        public string Title
        {
            get { return title; }
            set
            {
                if (value == title)
                    return;

                title = value;
                base.RaisePropertyChanged("Title");
            }
        } 

        #endregion

        #region 绑定的命令

        DelegateCommand<Window> minedCommand { get; set; }
        DelegateCommand<Window> closeCommand { get; set; }
        DelegateCommand closingCommand { get; set; }
        DelegateCommand<Window> dragCommand { get; set; }

        public ICommand MinedCommand
        {
            get
            {
                if (minedCommand == null)
                    minedCommand = new DelegateCommand<Window>((win) =>
                    {
                        win.WindowState = WindowState.Minimized;
                    });

                return minedCommand;
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                if (closeCommand == null)
                    closeCommand = new DelegateCommand<Window>((win) =>
                    {
                        MessageBoxResult dr;
                        dr = XWT.MessageBox.Show("确认退出该游戏？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        switch (dr)
                        {
                            case MessageBoxResult.Yes:

                                if (win.Visibility == Visibility.Visible)
                                {
                                    win.Close();
                                }

                                break;

                            case MessageBoxResult.No:
                                ; break;
                        }
                    });

                return closeCommand;
            }
        }

        public ICommand ClosingCommand
        {
            get
            {
                if (closingCommand == null)
                    closingCommand = new DelegateCommand(() =>
                    {
                        if (this.border != null)
                            border.IsEnabled = true;
                    });

                return closingCommand;
            }
        }

        public ICommand DragCommand
        {
            get
            {
                if (dragCommand == null)
                    dragCommand = new DelegateCommand<Window>((win) =>
                    {
                        win.DragMove();
                    });

                return dragCommand;
            }
        }

        #endregion
    }
}
