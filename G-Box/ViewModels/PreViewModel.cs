
using System.Windows.Input;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Events;
using MessageModule.ReceiveMsg;
using System;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Modularity;
using G_Box.OtherClass;
using MessageModule.MessageTypes;
using ToolClass.StoryBoard;
using XWT = Xceed.Wpf.Toolkit;

namespace G_Box.ViewModels
{
    [Export]
    public class PreViewModel
    {
        #region 变量

        IModuleManager moduleManager;
        IEventAggregator receive_Aggregator;
        SubscriptionToken subscriptionToken;

        #endregion

        #region 构造函数

        [ImportingConstructor]
        public PreViewModel(IModuleManager moduleManager, IEventAggregator eventAggregator)
        {
            this.moduleManager = moduleManager;
            this.receive_Aggregator = eventAggregator;

            RequestEvent(); //订阅接收信息事件
        }

        #endregion

        #region IEventAggregator订阅接收信息事件

        public void RequestEvent()
        {
            ReceiveMsgEvent receiveMsgEvent = receive_Aggregator.GetEvent<ReceiveMsgEvent>();

            if (subscriptionToken != null)
                receiveMsgEvent.Unsubscribe(subscriptionToken);

            subscriptionToken = receiveMsgEvent.Subscribe(OnReceiveMsg, ThreadOption.UIThread, false, Filter);
        }

        public void OnReceiveMsg(ReceiveMsgOrder receiveMsgOrder)
        {
            if (receiveMsgOrder.Sign == 1)
            {
                this.OnRequestClose();
                StoryboardManager.StopStoryboard("Story_Login");
                App.Current.MainWindow.Show();

                return;
            }

            StoryboardManager.StopStoryboard("Story_Login");
            XWT.MessageBox.Show(receiveMsgOrder.MsgContent, "提示", MessageBoxButton.OK, MessageBoxImage.Information);

            //MessageBox.Show(receiveMsgOrder.MsgContent);

            //ReceiveMsgEvent msgEvent = receive_Aggregator.GetEvent<ReceiveMsgEvent>();

            //if (subscriptionToken != null)
            //    msgEvent.Unsubscribe(subscriptionToken);
        }

        public bool Filter(ReceiveMsgOrder receiveMsgOrder)
        {
            return receiveMsgOrder.ModuleType == MessageTypes.CON;
        }

        public event EventHandler RequestClose;

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        #endregion

        #region 绑定的命令

        DelegateCommand<Window> dragCommand { get; set; }
        DelegateCommand closeCommand { get; set; }

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

        public ICommand CloseCommand
        {
            get
            {
                if (closeCommand == null)
                    closeCommand = new DelegateCommand(() =>
                {
                    Environment.Exit(0);
                });

                return closeCommand;
            }
        }

        #endregion
    }
}
