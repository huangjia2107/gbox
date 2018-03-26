using System;

using Microsoft.Practices.Prism.ViewModel;
using System.ComponentModel;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;
using System.Windows;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Practices.Prism.Events;
using MessageModule.ReceiveMsg;
using MessageModule.SendMsg;
using XWT = Xceed.Wpf.Toolkit;
using System.ComponentModel.Composition;
using MessageModule.MessageTypes;

namespace ResetModule.ViewModels
{
    [Export]
    public class ResetViewModel : NotificationObject, IDataErrorInfo
    {
        #region 变量

        IRegionManager regionManager;
        IEventAggregator receive_Aggregator;
        IEventAggregator send_Aggregator;
        SubscriptionToken subscriptionToken;

        #endregion

        #region 构造函数

        [ImportingConstructor]
        public ResetViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.receive_Aggregator = eventAggregator;
            this.send_Aggregator = eventAggregator;

            RequestEvent();
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
            XWT.MessageBox.Show(receiveMsgOrder.MsgContent, "提示", MessageBoxButton.OK, MessageBoxImage.Information);

            //MsgEvent msgEvent = eventAggregator.GetEvent<MsgEvent>();

            //if (subscriptionToken != null)
            //    msgEvent.Unsubscribe(subscriptionToken);
        }

        public bool Filter(ReceiveMsgOrder receiveMsgOrder)
        {
            return receiveMsgOrder.ModuleType == MessageTypes.RES;
        }

        #endregion

        #region 绑定的属性

        string cardword;
        string usermail;
        string newpsw;
        string conpsw;
        string num;

        public string CardWord
        {
            get { return cardword; }
            set
            {
                if (value == cardword)
                    return;

                cardword = value;
                base.RaisePropertyChanged("CardWord");

                applyCommand.RaiseCanExecuteChanged();
                submitCommand.RaiseCanExecuteChanged();
            }
        }

        public string NewPsw
        {
            get { return newpsw; }
            set
            {
                if (value == newpsw)
                    return;

                newpsw = value;
                base.RaisePropertyChanged("NewPsw");

                submitCommand.RaiseCanExecuteChanged();
            }
        }

        public string ConPsw
        {
            get { return conpsw; }
            set
            {
                if (value == conpsw)
                    return;

                conpsw = value;
                base.RaisePropertyChanged("ConPsw");

                submitCommand.RaiseCanExecuteChanged();
            }
        }

        public string UserMail
        {
            get { return usermail; }
            set
            {
                if (value == usermail)
                    return;

                usermail = value;
                base.RaisePropertyChanged("UserMail");

                applyCommand.RaiseCanExecuteChanged();
                submitCommand.RaiseCanExecuteChanged();
            }
        }

        public string Num
        {
            get { return num; }
            set
            {
                if (value == num)
                    return;

                num = value;
                base.RaisePropertyChanged("Num");

                submitCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region 绑定的命令

        DelegateCommand<object> submitCommand { get; set; }
        DelegateCommand<string> applyCommand { get; set; }

        #region 提交命令

        public ICommand SubmitCommand
        {
            get
            {
                if (submitCommand == null)
                    submitCommand = new DelegateCommand<object>(OnFindExcute, this.CanSubmit);

                return submitCommand;
            }
        }

        void OnFindExcute(object sender)
        { 
            // 信息发送==事件发布方
            send_Aggregator.GetEvent<SendMsgEvent>().Publish(MessageTypes.RES + this.CardWord + MessageTypes.NSP + this.NewPsw + MessageTypes.NSP + this.UserMail + MessageTypes.NSP + this.Num);
        }

        bool CanSubmit(object sender)
        {
            if (!this.IsValid)
                return false;

            return true;
        }

        #endregion

        #region 申请注册码命令

        public ICommand ApplyCommand
        {
            get
            {
                if (applyCommand == null)
                    applyCommand = new DelegateCommand<string>(OnApplyExcute, CanApply);

                return applyCommand;
            }
        }

        void OnApplyExcute(string email)
        { 
            // 信息发送==事件发布方
            send_Aggregator.GetEvent<SendMsgEvent>().Publish(MessageTypes.VNU + this.CardWord+MessageTypes.NSP+email);
        }

        bool CanApply(string email)
        {
            if (GetValidationError("UserMail") != null || GetValidationError("CardWord") != null)
            {
                return false;
            }

            return true;
        }

        #endregion

        #endregion

        #region IDataErrorInfo 成员

        static readonly string[] ValidatedProperties = { "CardWord", "NewPsw", "ConPsw", "UserMail", "Num" };

        string IDataErrorInfo.Error
        {
            get { return null; }
        }

        string IDataErrorInfo.this[string propertyName]
        {
            get { return GetValidationError(propertyName); }
        }

        /// <summary>
        /// 判断值是否为空或Null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static bool IsStringMissing(string value)
        {
            return String.IsNullOrEmpty(value) || value.Trim() == String.Empty;
        }

        /// <summary>
        /// 获取注册信息的有效性
        /// </summary>
        bool IsValid
        {
            get
            {
                foreach (string porperty in ValidatedProperties)
                {
                    if (GetValidationError(porperty) != null)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        /// <summary>
        /// 获取验证信息
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        string GetValidationError(string propertyName)
        {
            if (Array.IndexOf(ValidatedProperties, propertyName) < 0)
                return null;

            string msg = null;
            switch (propertyName)
            {
                case "CardWord":
                    msg = ValidateName();
                    break;

                case "NewPsw":
                    msg = ValidatePassword();
                    break;

                case "ConPsw":
                    msg = ValidateConPassword();
                    break;

                case "UserMail":
                    msg = ValidateEmail();
                    break;

                case "Num":
                    msg = ValidateNum();
                    break;

                default:
                    Debug.Fail("未能识别信息: " + propertyName);
                    break;
            }
            return msg;
        }

        #region IDataErrorInfo 方法

        string ValidateName()
        {
            if (IsStringMissing(this.CardWord))
                return "账号不能为空";

            if (!Regex.IsMatch(this.CardWord, @"^[A-Za-z0-9]{6,12}$"))
                return "6-12位,字母或数字";

            return null;
        }

        string ValidatePassword()
        {
            if (IsStringMissing(this.NewPsw))
                return "密码不能为空";

            if (!Regex.IsMatch(this.NewPsw, @"^[A-Za-z0-9]{6,12}$"))
                return "6-12位,字母或数字";

            return null;
        }

        string ValidateConPassword()
        {
            if (IsStringMissing(this.ConPsw))
                return "确认密码不能为空";

            if (this.NewPsw != this.ConPsw)
                return "前后输入密码不一致";

            return null;
        }

        string ValidateEmail()
        {
            if (IsStringMissing(this.UserMail))
                return "邮箱地址不能为空";

            if (!Regex.IsMatch(this.UserMail, @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$"))
                return "邮箱地址不正确";

            return null;
        }

        string ValidateNum()
        {
            if (IsStringMissing(this.Num))
                return "注册码不能为空";

            if (!Regex.IsMatch(this.Num, @"[A-Za-z0-9]+") || this.Num.Trim().Length != 4)
                return "4位,字母或数字";

            return null;
        }

        #endregion

        #endregion // IDataErrorInfo Members
    }
}
