
using MessageModule.ModuleMsg;
using MessageModule.SendMsg;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Windows;
using System.Windows.Input;
using XWT = Xceed.Wpf.Toolkit;

namespace G_Box.ViewModels
{
    [Export]
    public class ShellViewModel : NotificationObject
    {
        #region 变量

        IRegionManager regionManager;
        IEventAggregator module_Aggregator; 
        SubscriptionToken subscriptionToken;
         
        #endregion

        #region 构造函数

        [ImportingConstructor]
        public ShellViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.module_Aggregator = eventAggregator; 

            RequestEvent();
        }

        #endregion

        #region IEventAggregator订阅接收信息事件(来自：游戏简介)

        public void RequestEvent()
        {
            ModuleMsgEvent moduleMsgEvent = module_Aggregator.GetEvent<ModuleMsgEvent>();

            if (subscriptionToken != null)
                moduleMsgEvent.Unsubscribe(subscriptionToken);

            subscriptionToken = moduleMsgEvent.Subscribe(OnModuleMsg, ThreadOption.UIThread, false, (s) => { return true; });
        }

        public void OnModuleMsg(ModuleMsgOrder _moduleMsgOrder)
        {
            switch (_moduleMsgOrder.Sign)
            {
                case 0: //得到ID/切换到“介绍”界面 
                    this.UpRow = 2;
                    this.DownRow = 1;
                    break;

               case 1: //切换回“原”界面
                    this.UpRow = 1;
                    this.DownRow = 2;
                    break;
            }
            
            //MsgEvent msgEvent = eventAggregator.GetEvent<MsgEvent>();

            //if (subscriptionToken != null)
            //    msgEvent.Unsubscribe(subscriptionToken);
        }

        #endregion

        #region 绑定的属性

        int upRow = 1;
        int downRow = 2;

        public int UpRow
        {
            get { return upRow; }
            set
            {
                if (value == upRow)
                    return;

                upRow = value;
                base.RaisePropertyChanged("UpRow");
            }
        }

        public int DownRow
        {
            get { return downRow; }
            set
            {
                if (value == downRow)
                    return;

                downRow = value;
                base.RaisePropertyChanged("DownRow");
            }
        }

        #endregion

        #region 绑定的命令

        DelegateCommand minedCommand { get; set; }
        DelegateCommand closeCommand { get; set; }
        DelegateCommand<Window> dragCommand { get; set; }

        public ICommand MinedCommand
        {
            get
            {
                if (minedCommand == null)
                    minedCommand = new DelegateCommand(() =>
                {
                    App.Current.MainWindow.WindowState = WindowState.Minimized;
                });

                return minedCommand;
            }
        }

        public ICommand CloseCommand
        {
            get
            {
                if (closeCommand == null)
                    closeCommand = new DelegateCommand(() =>
                {
                    MessageBoxResult dr;
                    dr = XWT.MessageBox.Show("确认退出系统？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    switch (dr)
                    {
                        case MessageBoxResult.Yes:  

                            Application.Current.Shutdown();

                            break;

                        case MessageBoxResult.No:
                            ; break;
                    }
                });

                return closeCommand;
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
