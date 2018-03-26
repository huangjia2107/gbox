using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input; 
using XWT = Xceed.Wpf.Toolkit;

namespace IntroductionModule.ViewModels
{ 
    public class DownViewModel
    {
        #region 变量

        IRegionManager regionManager;
        public List<ConnectParam> connectList;

        #endregion

        #region 构造函数
         
        public DownViewModel() 
        {
            connectList = new List<ConnectParam>(); 
        }

        #endregion
         
        #region 绑定的属性

        //下载列表
        ObservableCollection<ListViewModel> _allDowns { get; set; }

        public ObservableCollection<ListViewModel> AllDowns
        {
            get
            {
                if (_allDowns == null)
                {
                    _allDowns = new ObservableCollection<ListViewModel>();
                }

                return _allDowns;
            }
        }

        #endregion 

        #region 绑定的命令

        #region 关闭命令

        DelegateCommand<Window> closeCommand { get; set; }

        public ICommand CloseCommand
        {
            get
            {
                if (closeCommand == null)
                    closeCommand = new DelegateCommand<Window>((win) =>
                    {
                        if (this.AllDowns.Count == 0)
                        {
                            win.Visibility = Visibility.Hidden;
                            return;
                        }

                        MessageBoxResult dr;
                        dr = XWT.MessageBox.Show("关闭该窗口并结束下载任务？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        switch (dr)
                        {
                            case MessageBoxResult.Yes:
                                 
                                foreach (ConnectParam connectParam in connectList)
                                { 
                                    connectParam.StopTimerAndClose(); 
                                }

                                connectList.Clear();
                                this.AllDowns.Clear();

                                win.Visibility = Visibility.Hidden;

                                break;

                            case MessageBoxResult.No:
                                ; break;
                        } 
                    });

                return closeCommand;
            }
        }

        #endregion

        #region 最小化命令

        DelegateCommand<Window> minCommand { get; set; }

        public ICommand MinCommand
        {
            get
            {
                if (minCommand == null)
                    minCommand = new DelegateCommand<Window>((win) =>
                    {
                        win.WindowState = WindowState.Minimized;
                    });

                return minCommand;
            }
        }

        #endregion

        #region 拖动命令

        DelegateCommand<Window> dragCommand { get; set; }

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

        #region 取消下载命令

        DelegateCommand<TextBlock> cancelCommand { get; set; }

        public ICommand CancelCommand
        {
            get
            {
                if (cancelCommand == null)
                    cancelCommand = new DelegateCommand<TextBlock>((textBlock) =>
                { 
                    foreach (ConnectParam connectParam in connectList)
                    {
                        if(connectParam.stateObject.gameID==textBlock.Tag.ToString())
                        {
                            connectParam.StopTimerAndClose();
                            this.connectList.RemoveAll(conn => conn.stateObject.gameID == textBlock.Tag.ToString());
                            break;
                        }
                    }
                });

                return cancelCommand;
            }
        }

        #endregion

        #endregion
    }
}
