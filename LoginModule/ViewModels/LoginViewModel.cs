using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Practices.Prism.Regions;
using LoginModule.Models;
using LoginModule.DataAccess;
using System.Collections.ObjectModel;
using System.Windows;
using System.Collections.Specialized;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using MessageModule.ReceiveMsg;
using MessageModule.SendMsg;
using XWT = Xceed.Wpf.Toolkit;
using ToolClass.StoryBoard;
using System.ComponentModel.Composition;
using MessageModule.MessageTypes;

namespace LoginModule.ViewModels
{
    [Export]
    public class LoginViewModel : NotificationObject
    {
        #region 变量

        private IRegionManager regionManager;
        private IEventAggregator receive_Aggregator;
        private IEventAggregator send_Aggregator;
        private SubscriptionToken subscriptionToken;

        UserModel _userModel;
        readonly UserAccess _userAccess;

        #endregion

        #region 构造函数

        [ImportingConstructor]
        public LoginViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.receive_Aggregator = eventAggregator;
            this.send_Aggregator = eventAggregator;

            _userModel = UserModel.CreateNewModel();
            _userAccess = new UserAccess();
            _userAccess.UserAdded += this.OnUserAddedToModel;
            _userAccess.UserDeleted += this.OnUserDeletedToModel;

            RequestEvent(); //订阅接收信息事件
        }

        #endregion

        #region IEventAggregator订阅接收信息事件((服务器->客户端)主程序->各模块)

        public void RequestEvent()
        {
            ReceiveMsgEvent receiveMsgEvent = receive_Aggregator.GetEvent<ReceiveMsgEvent>();

            if (subscriptionToken != null)
                receiveMsgEvent.Unsubscribe(subscriptionToken);

            subscriptionToken = receiveMsgEvent.Subscribe(OnReceiveMsg, ThreadOption.UIThread, false, Filter);
        }

        public void OnReceiveMsg(ReceiveMsgOrder receiveMsgOrder)
        {
            if (receiveMsgOrder.Sign == 1) //登陆成功
            {
                //ID+账号+昵称+头像
                string[] UserMsg = receiveMsgOrder.MsgContent.Split(MessageTypes.NSP.ToCharArray());
                //此处不做窗口跳转处理，由PreViewModel.cs处理

                //存储用户
                _userModel = UserModel.CreateModel(UserMsg[1], UserMsg[2], UserMsg[3], this.PassWord, Check, DateTime.Now);

                if (_userAccess.IsAlreadyExists(_userModel))
                    _userAccess.UpdateUser(_userModel);
                else
                    _userAccess.AddUser(_userModel);

                _userAccess.SortUser();

                return;
            }

            StoryboardManager.StopStoryboard("Story_Login"); 
            XWT.MessageBox.Show(receiveMsgOrder.MsgContent, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
             
            //MsgEvent msgEvent = eventAggregator.GetEvent<MsgEvent>();

            //if (subscriptionToken != null)
            //    msgEvent.Unsubscribe(subscriptionToken);
        }

        public bool Filter(ReceiveMsgOrder receiveMsgOrder)
        {
            return receiveMsgOrder.ModuleType == MessageTypes.LOG;
        }

        #endregion

        #region 绑定的属性

        string cardword;
        bool Check=false; //是否记住密码
        string PassWord="";
        ObservableCollection<UserViewModel> _allUsers { get; set; }

        public string CardWord
        {
            get { return cardword; }
            set
            {
                if (value == cardword)
                    return;

                cardword = value;

                base.RaisePropertyChanged("CardWord");
            }
        }
       
        public ObservableCollection<UserViewModel> AllUsers
        {
            get
            {
                if (_allUsers == null)
                {
                    List<UserViewModel> all = (from user in _userAccess.GetUser()
                                               select new UserViewModel(user, _userAccess)).ToList();

                    foreach (UserViewModel vm in all) 

                    _allUsers = new ObservableCollection<UserViewModel>(all); 
                }
                return _allUsers;
            }
        }

        #endregion

        #region 绑定的命令

        DelegateCommand<PasswordBox> loginCommand { get; set; }
        DelegateCommand<string> deleteCommand { get; set; }
        DelegateCommand<bool?> rememberCommand { get; set; }
        DelegateCommand addCommand { get; set; }

        #region 登陆命令

        public ICommand LoginCommand
        {
            get
            {
                if (loginCommand == null)
                    loginCommand = new DelegateCommand<PasswordBox>(OnLoginExcute);

                return loginCommand;
            }
        }

        void OnLoginExcute(PasswordBox pass)
        {
            if (IsStringMissing(pass.Password) || IsStringMissing(this.CardWord))
            {
                XWT.MessageBox.Show("账号或密码不能为空！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //启动动画
            StoryboardManager.PlayStoryboard("Story_Login", OnCompletedExcute,null);

            if (this.Check)
                this.PassWord = pass.Password;
            else
                this.PassWord = "";

            // 信息发送==>事件发布方
            send_Aggregator.GetEvent<SendMsgEvent>().Publish(MessageTypes.LOG + this.CardWord + MessageTypes.NSP + pass.Password);
            
            
            //ReceiveMsgOrder receiveMsgOrder = new ReceiveMsgOrder() { ModuleType="LOG",Sign=1,MsgContent="fff"}; 
            ////广播
            //receive_Aggregator.GetEvent<ReceiveMsgEvent>().Publish(receiveMsgOrder);
        }

        bool IsStringMissing(string value)
        {
            return String.IsNullOrEmpty(value) || value.Trim() == String.Empty;
        }

        #region 动画完成后执行的方法

        void OnCompletedExcute(object state)
        {
            XWT.MessageBox.Show("连接或发送超时,请检查网络状态！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        #endregion

        #endregion       

        #region 删除用户记录命令

        public ICommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                    deleteCommand = new DelegateCommand<string>((card) =>
                {
                    MessageBoxResult dr;
                    dr = XWT.MessageBox.Show("确认删除该账号相关记录？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    switch (dr)
                    {
                        case MessageBoxResult.Yes:

                            _userAccess.DeleteUser(card);

                            break;

                        case MessageBoxResult.No:
                            ; break;
                    }
                });

                return deleteCommand;
            }
        }

        #endregion

        #region 记住密码命令

        public ICommand RememberCommand
        {
            get 
            {
                if (rememberCommand == null)
                    rememberCommand = new DelegateCommand<bool?>((check)=>
                {
                    if (check==true)
                        this.Check = true;
                    else
                        this.Check = false;
                });
                return rememberCommand;
            }
        }

        #endregion

        #region 事件

        void OnUserAddedToModel(object sender, UserAddedEventArgs e)
        {
            var viewModel = new UserViewModel(e.NewUserModel, _userAccess);
            this.AllUsers.Add(viewModel);
        }

        void OnUserDeletedToModel(object sender, UserDeletedEventArgs e)
        {
            foreach (UserViewModel model in this.AllUsers)
            {
                if (model.CardWord == e.CardWord)
                {
                    this.AllUsers.Remove(model);
                    break;
                }
            }
        }

        #endregion

        #endregion
    }
}
