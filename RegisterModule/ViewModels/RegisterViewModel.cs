using System;
using System.ComponentModel;

using System.Text.RegularExpressions;
using System.Diagnostics;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows.Shapes;
using System.Windows.Media;
using Microsoft.Practices.Prism.Events;
using MessageModule.ReceiveMsg;
using MessageModule.SendMsg;
using XWT=Xceed.Wpf.Toolkit;
using System.ComponentModel.Composition;
using MessageModule.MessageTypes;

namespace RegisterModule.ViewModels
{
    [Export]
    public class RegisterViewModel : NotificationObject, IDataErrorInfo
    {
        #region 变量

        IRegionManager regionManager;
        IEventAggregator receive_Aggregator;
        IEventAggregator send_Aggregator;
        SubscriptionToken subscriptionToken;

        #endregion

        #region 构造函数

        [ImportingConstructor]
        public RegisterViewModel(IRegionManager regionManager,IEventAggregator eventAggregator)
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
            XWT.MessageBox.Show(receiveMsgOrder.MsgContent,"提示",MessageBoxButton.OK,MessageBoxImage.Information);

            //MsgEvent msgEvent = eventAggregator.GetEvent<MsgEvent>();

            //if (subscriptionToken != null)
            //    msgEvent.Unsubscribe(subscriptionToken);
        }

        public bool Filter(ReceiveMsgOrder receiveMsgOrder)
        {
            return receiveMsgOrder.ModuleType == MessageTypes.REG;
        }

        #endregion

        #region 绑定的属性

        string cardword;
        string username;
        string newpsw;
        string conpsw;
        string usermail;
        string num;    
        string imageurl;
        
        public string CardWord
        {
            get { return cardword; }
            set
            {
                if (value == cardword)
                    return;

                cardword = value;
                base.RaisePropertyChanged("CardWord");

                registerCommand.RaiseCanExecuteChanged();
            }
        }

        public string UserName
        {
            get { return username; }
            set
            {
                if (value == username)
                    return;

                username = value;
                base.RaisePropertyChanged("UserName");

                registerCommand.RaiseCanExecuteChanged();
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

                registerCommand.RaiseCanExecuteChanged();
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

                registerCommand.RaiseCanExecuteChanged();
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
                registerCommand.RaiseCanExecuteChanged();
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

                registerCommand.RaiseCanExecuteChanged();
            }
        }

        public string ImageUrl
        {
            get { return imageurl; }
            set
            {
                if (value == imageurl)
                    return;

                imageurl = value;
                base.RaisePropertyChanged("ImageUrl");

                registerCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region 绑定的命令

        DelegateCommand<object> registerCommand { get; set; }
        DelegateCommand<string> applyCommand { get; set; }
        DelegateCommand<Popup> changeCommand { get; set; }
        DelegateCommand<Rectangle> selectCommand { get; set; }

        #region 注册命令

        public ICommand RegisterCommand
        {
            get
            {
                if (registerCommand == null)
                    registerCommand = new DelegateCommand<object>(OnRegisterExcute, this.CanRegister);

                return registerCommand;
            }
        }

        bool CanRegister(object sender)
        {
            if (!this.IsValid)
                return false;
 
            return true;
        }

        void OnRegisterExcute(object sender)
        {
            //MessageBox.Show("注册中...");

            // 信息发送==事件发布方
            send_Aggregator.GetEvent<SendMsgEvent>().Publish("REG" + this.CardWord + "$" +this.UserName+"$"+this.NewPsw+"$"+this.ImageUrl+"$"+ this.UserMail+"$"+this.Num); 
        }

        #endregion

        #region 申请注册码命令

        public ICommand ApplyCommand
        {
            get
            {
                if (applyCommand == null)
                    applyCommand = new DelegateCommand<string>(OnApplyExcute,CanApply);

                return applyCommand;
            }
        }

        void OnApplyExcute(string email)
        {
            //MessageBox.Show("注册邮箱："+email);
            
            // 信息发送==事件发布方
            send_Aggregator.GetEvent<SendMsgEvent>().Publish(MessageTypes.RNU + email); 
        }

        bool CanApply(string email)
        {
            if (GetValidationError("UserMail") != null)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region 切换头像命令

        public ICommand ChangeCommand
        {
            get
            {
                if (changeCommand == null)
                    changeCommand = new DelegateCommand<Popup>((popup) =>
                    {
                        popup.IsOpen = true;
                    });

                return changeCommand;
            }
        }

        #endregion

        #region 选择头像

        public ICommand SelectCommand
        {
            get
            {
                if (selectCommand == null)
                    selectCommand = new DelegateCommand<Rectangle>(OnSelectExcute);

                return selectCommand;
            }
        }

        void OnSelectExcute(Rectangle rect)
        {
            ImageBrush imgBrush = rect.Fill as ImageBrush;
            ImageUrl = imgBrush.ImageSource.ToString();
        }

        #endregion

        #endregion

        #region IDataErrorInfo 成员

        static readonly string[] ValidatedProperties = { "CardWord","UserName", "NewPsw", "ConPsw", "UserMail", "Num" };

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
                    if (GetValidationError(porperty) != null || IsStringMissing(this.ImageUrl))
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
                    msg = ValidateCard();
                    break;

                case "UserName":
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

        string ValidateCard()
        {
            if (IsStringMissing(this.CardWord))
                return "账号不能为空";

            if (!Regex.IsMatch(this.CardWord, @"^[A-Za-z0-9]{6,12}$"))
                return "6-12位,字母或数字";

            return null;
        }

        string ValidateName()
        {
            if (IsStringMissing(this.UserName))
                return "昵称不能为空";

            if (!Regex.IsMatch(this.UserName, @"^[\w ]{4,12}$"))
                return "4-12位,字符";

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
