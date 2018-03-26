using System;

using CardModule.Models;
using MessageModule.ReceiveMsg;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;
using MessageModule.SendMsg;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Diagnostics;
using XWT = Xceed.Wpf.Toolkit;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.ComponentModel.Composition;
using MessageModule.MessageTypes;
using ToolClass.Logger;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Linq;
using ToolClass.String;
using MessageModule.RegionTypes;

namespace CardModule.ViewModels
{
    [Export]
    public class CardViewModel : NotificationObject, IDataErrorInfo
    {
        #region 变量

        IRegionManager regionManager;
        IEventAggregator receive_Aggregator;
        IEventAggregator send_Aggregator;
        SubscriptionToken subscriptionToken;

        AchieveModel _achieveModel;
        FriendModel _friendModel;
        FamilyModel _familyModel;
        BlackModel _blackModel;
        ResultModel _resultModel;
        MsgModel _msgModel;

        /// <summary>
        /// 指定分页类型
        /// </summary>
        enum Paging { ACH, MSG };

        /// <summary>
        /// log对象，指向日志级别
        /// </summary>
        private readonly ILogger ilogger;

        /// <summary>
        /// 存放所有成就
        /// </summary>
        List<AchieveViewModel> achieveList = null;

        /// <summary>
        /// 存放所有消息
        /// </summary>
        List<MsgViewModel> msgList = null;

        #endregion

        #region 构造函数

        [ImportingConstructor]
        public CardViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.receive_Aggregator = eventAggregator;
            this.send_Aggregator = eventAggregator;

            _achieveModel = AchieveModel.CreateNewModel();
            _friendModel = FriendModel.CreateNewModel();
            _familyModel = FamilyModel.CreateNewModel();
            _blackModel = BlackModel.CreateNewModel();
            _resultModel = ResultModel.CreateNewModel();
            _msgModel = MsgModel.CreateNewModel();

            //获取日志记录实例
            this.ilogger = ILogger.GetInstance();
            //实例化
            achieveList = new List<AchieveViewModel>();
            //实例化
            msgList = new List<MsgViewModel>();

            ChangePage(1, Paging.ACH, this._pagingAchieve);
            ChangePage(1, Paging.MSG, this._pagingMsg);

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
            string[] msg = null;
            if (receiveMsgOrder.MsgContent.IndexOf(MessageTypes.NSP) > -1)
                msg = receiveMsgOrder.MsgContent.Split(MessageTypes.NSP.ToCharArray());

            // ilogger.Logger(string.Format("接收到的数据:{0}-{1}", receiveMsgOrder.ModuleType, receiveMsgOrder.MsgContent));
            if (receiveMsgOrder.Sign == 0)
            {
                XWT.MessageBox.Show(receiveMsgOrder.MsgContent, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            switch (receiveMsgOrder.ModuleType)
            {
                #region 返回个人信息_PER
                case MessageTypes.PER:

                    this.UserID = msg[0];
                    this.CardWord = msg[1];
                    this.UserName = msg[2];
                    this.ImgSrc = msg[3];

                    //存储用户信息
                    this.regionManager.Regions[RegionTypes.CardRegion].Context = msg;

                    break;
                #endregion

                #region 返回游戏成就_ACH
                case MessageTypes.ACH:
                     
                    //更新
                    foreach (AchieveViewModel viewmodel in achieveList)
                    {
                        if (viewmodel.GameName == msg[0])
                        {
                            viewmodel.Level = msg[1];
                            viewmodel.Rank = msg[2];
                            viewmodel.Total = msg[3];
                            viewmodel.Single = msg[4];

                            return;
                        }
                    }

                    //添加
                    _achieveModel = AchieveModel.CreateModel(msg[0], msg[1], msg[2], msg[3], msg[4]);
                    achieveList.Add(new AchieveViewModel(_achieveModel));
                    //this.AllAchieves.Add(new AchieveViewModel(_achieveModel));

                    this.Achieve++;
                    ChangePage(this._pagingAchieve.CurrentPage == 0 ? 1 : this._pagingAchieve.CurrentPage, Paging.ACH, this._pagingAchieve);

                    break;
                #endregion

                #region 返回消息_MSG
                case MessageTypes.MSG:

                    string msginfo = msg[0] + "," + GetString.GetRandomCode(GetString.Code.StrAndNum, 4);

                    _msgModel = MsgModel.CreateModel(msginfo, msg[0], msg[1], msg[2], msg[3]);
                    msgList.Add(new MsgViewModel(_msgModel));

                    this.Message++;
                    ChangePage(this._pagingMsg.CurrentPage == 0 ? 1 : this._pagingMsg.CurrentPage, Paging.MSG, this._pagingMsg);

                    break;
                #endregion

                #region 修改密码返回的信息_SET
                case MessageTypes.SET:

                    this.PassWord = "";
                    this.NewPassWord = "";
                    this.ConPassWord = "";

                    XWT.MessageBox.Show(receiveMsgOrder.MsgContent, "提示", MessageBoxButton.OK, MessageBoxImage.Information);

                    break;
                #endregion

                #region 返回好友信息_FRI
                case MessageTypes.FRI:

                    switch (msg[5])
                    {
                        case MessageTypes.Friend:

                            _friendModel = FriendModel.CreateModel(msg[0], msg[1], msg[2], msg[3], msg[5], msg[4] == MessageTypes.N ? true : false, msg[4] == MessageTypes.N ? false : true);
                            this.AllFriends.Add(new FriendViewModel(_friendModel));

                            this.Friend++;

                            break;

                        case MessageTypes.Family:

                            _familyModel = FamilyModel.CreateModel(msg[0], msg[1], msg[2], msg[3], msg[5], msg[4] == MessageTypes.N ? true : false, msg[4] == MessageTypes.N ? false : true);
                            this.AllFamilys.Add(new FamilyViewModel(_familyModel));

                            this.Friend++;

                            break;

                        case MessageTypes.Black:

                            _blackModel = BlackModel.CreateModel(msg[0], msg[1], msg[2], msg[3], msg[5], msg[4] == MessageTypes.N ? true : false, false);
                            this.AllBlacks.Add(new BlackViewModel(_blackModel));

                            this.Friend++;

                            break;
                    }

                    break;
                #endregion

                #region 返回查找好友结果_SFR
                case MessageTypes.SFR:

                    _resultModel = ResultModel.CreateModel(msg[0], msg[1], msg[2], msg[3],
                        msg[1] == this.CardWord ? false : true, msg[4] == MessageTypes.N ? true : false);
                    this.AllResults.Add(new ResultViewModel(_resultModel));

                    break;
                #endregion

                #region 好友上线通知_UPL
                case MessageTypes.UPL:

                    foreach (FriendViewModel friend in this.AllFriends.Where(f => f.FriendID == receiveMsgOrder.MsgContent))
                    {
                        friend.FriendStatus = false;
                        friend.MenuIsEnabled = true;
                    }

                    foreach (FamilyViewModel family in this.AllFamilys.Where(f => f.FriendID == receiveMsgOrder.MsgContent))
                    {
                        family.FriendStatus = false;
                        family.MenuIsEnabled = true;
                    }

                    break;
                #endregion

                #region 好友下线通知_OFF
                case MessageTypes.OFF:

                    foreach (FriendViewModel friend in this.AllFriends.Where(f => f.FriendID == receiveMsgOrder.MsgContent))
                    {
                        friend.FriendStatus = true;
                        friend.MenuIsEnabled = false;
                    }

                    foreach (FamilyViewModel family in this.AllFamilys.Where(f => f.FriendID == receiveMsgOrder.MsgContent))
                    {
                        family.FriendStatus = true;
                        family.MenuIsEnabled = false;
                    }

                    break;
                #endregion

                #region 询问好友添加请求_ASK
                case MessageTypes.ASK:

                    string msginfo1 = MessageTypes.F + "," + GetString.GetRandomCode(GetString.Code.StrAndNum, 4) + "," + msg[0];

                    _msgModel = MsgModel.CreateModel(msginfo1, MessageTypes.F, "请求添加您为好友.", "", msg[1]);
                    msgList.Add(new MsgViewModel(_msgModel));

                    this.Message++;
                    ChangePage(this._pagingMsg.CurrentPage == 0 ? 1 : this._pagingMsg.CurrentPage, Paging.MSG, this._pagingMsg);

                    break;
                #endregion

                #region 返回查找好友资料进行添加_AGR
                case MessageTypes.AGR:

                    //添加信息提示
                    string msginfo2 = MessageTypes.T + "," + GetString.GetRandomCode(GetString.Code.StrAndNum, 4);

                    _msgModel = MsgModel.CreateModel(msginfo2, MessageTypes.T, "对方同意您的添加请求.", "", msg[1]);
                    msgList.Add(new MsgViewModel(_msgModel));

                    this.Message++;
                    ChangePage(this._pagingMsg.CurrentPage == 0 ? 1 : this._pagingMsg.CurrentPage, Paging.MSG, this._pagingMsg);

                    //添加好友
                    _friendModel = FriendModel.CreateModel(msg[0], msg[1], msg[2], msg[3], msg[5], msg[4] == MessageTypes.N ? true : false, msg[4] == MessageTypes.N ? false : true);
                    this.AllFriends.Add(new FriendViewModel(_friendModel));

                    this.Friend++;

                    //添加信息提示 
                    string msginfo3 = MessageTypes.T + "," + GetString.GetRandomCode(GetString.Code.StrAndNum, 4);

                    _msgModel = MsgModel.CreateModel(msginfo3, MessageTypes.T, "新的好友添加成功.", "", "系统");
                    msgList.Add(new MsgViewModel(_msgModel));

                    this.Message++;
                    ChangePage(this._pagingMsg.CurrentPage == 0 ? 1 : this._pagingMsg.CurrentPage, Paging.MSG, this._pagingMsg);

                    break;
                #endregion

                #region 返回查找好友资料进行添加_AFR
                case MessageTypes.AFR:

                    //添加好友信息
                    _friendModel = FriendModel.CreateModel(msg[0], msg[1], msg[2], msg[3], msg[5], msg[4] == MessageTypes.N ? true : false, msg[4] == MessageTypes.N ? false : true);
                    this.AllFriends.Add(new FriendViewModel(_friendModel));

                    this.Friend++;

                    //添加信息提示
                    string msginfo4 = MessageTypes.T + "," + GetString.GetRandomCode(GetString.Code.StrAndNum, 4);

                    _msgModel = MsgModel.CreateModel(msginfo4, MessageTypes.T, "新的好友添加成功.", "", "系统");
                    msgList.Add(new MsgViewModel(_msgModel));

                    this.Message++;
                    ChangePage(this._pagingMsg.CurrentPage == 0 ? 1 : this._pagingMsg.CurrentPage, Paging.MSG, this._pagingMsg);

                    break;
                #endregion

                #region 对方将自己删除，此处也将对方删除_DFR
                case MessageTypes.DFR:

                    foreach (FriendViewModel friend in this.AllFriends)
                    {
                        if (friend.FriendID == receiveMsgOrder.MsgContent)
                        {
                            this.AllFriends.Remove(friend);
                            break;
                        }
                    }

                    if (this.Friend > 0)
                        this.Friend--;

                    break;
                #endregion

                #region 返回移动好友结果，本地进行移动_MFR
                case MessageTypes.MFR:

                    switch (msg[1])
                    {
                        #region 该好友原来在“我的好友”组
                        case MessageTypes.Friend:

                            foreach (FriendViewModel friend in this.AllFriends)
                            {
                                if (friend.FriendID == msg[0])
                                {
                                    //从该组移除
                                    this.AllFriends.Remove(friend);

                                    switch (msg[2])
                                    {
                                        //现移动到“我的家族”
                                        case MessageTypes.Family:

                                            _familyModel = FamilyModel.CreateModel(friend.FriendID, friend.FriendCard, friend.FriendName, friend.FriendImg, MessageTypes.Family, friend.FriendStatus, friend.MenuIsEnabled);
                                            this.AllFamilys.Add(new FamilyViewModel(_familyModel));

                                            break;
                                        //现移动到“黑名单”
                                        case MessageTypes.Black:

                                            _blackModel = BlackModel.CreateModel(friend.FriendID, friend.FriendCard, friend.FriendName, friend.FriendImg, MessageTypes.Black, friend.FriendStatus, false);
                                            this.AllBlacks.Add(new BlackViewModel(_blackModel));

                                            break;
                                    }

                                    break;
                                }
                            }

                            break;
                        #endregion

                        #region 该好友原来在“我的家族”组
                        case MessageTypes.Family:

                            foreach (FamilyViewModel family in this.AllFamilys)
                            {
                                if (family.FriendID == msg[0])
                                {
                                    //从该组移除
                                    this.AllFamilys.Remove(family);

                                    switch (msg[2])
                                    {
                                        //现移动到“我的好友”
                                        case MessageTypes.Friend:

                                            _friendModel = FriendModel.CreateModel(family.FriendID, family.FriendCard, family.FriendName, family.FriendImg, MessageTypes.Friend, family.FriendStatus, family.MenuIsEnabled);
                                            this.AllFriends.Add(new FriendViewModel(_friendModel));

                                            break;
                                        //现移动到“黑名单”
                                        case MessageTypes.Black:

                                            _blackModel = BlackModel.CreateModel(family.FriendID, family.FriendCard, family.FriendName, family.FriendImg, MessageTypes.Black, family.FriendStatus, false);
                                            this.AllBlacks.Add(new BlackViewModel(_blackModel));

                                            break;
                                    }

                                    break;
                                }
                            }
                            break;
                        #endregion

                        #region 该好友原来在“黑名单”组
                        case MessageTypes.Black:

                            foreach (BlackViewModel black in this.AllBlacks)
                            {
                                if (black.FriendID == msg[0])
                                {
                                    //从该组移除
                                    this.AllBlacks.Remove(black);

                                    switch (msg[2])
                                    {
                                        //现移动到“我的好友”
                                        case MessageTypes.Friend:

                                            _friendModel = FriendModel.CreateModel(black.FriendID, black.FriendCard, black.FriendName, black.FriendImg, MessageTypes.Friend, black.FriendStatus, !black.FriendStatus);
                                            this.AllFriends.Add(new FriendViewModel(_friendModel));

                                            break;
                                        //现移动到“我的家族”
                                        case MessageTypes.Family:

                                            _familyModel = FamilyModel.CreateModel(black.FriendID, black.FriendCard, black.FriendName, black.FriendImg, MessageTypes.Friend, black.FriendStatus, !black.FriendStatus);
                                            this.AllFamilys.Add(new FamilyViewModel(_familyModel));

                                            break;
                                    }

                                    break;
                                }
                            }
                            break;
                        #endregion
                    }

                    break;
                #endregion
            }

            //MessageBox.Show(receiveMsgOrder.MsgContent);

            //ReceiveMsgEvent msgEvent = receive_Aggregator.GetEvent<ReceiveMsgEvent>();

            //if (subscriptionToken != null)
            //    msgEvent.Unsubscribe(subscriptionToken);
        }

        public bool Filter(ReceiveMsgOrder receiveMsgOrder)
        {
            return receiveMsgOrder.ModuleType == MessageTypes.ACH || receiveMsgOrder.ModuleType == MessageTypes.MSG || receiveMsgOrder.ModuleType == MessageTypes.PER
                || receiveMsgOrder.ModuleType == MessageTypes.FRI || receiveMsgOrder.ModuleType == MessageTypes.SET || receiveMsgOrder.ModuleType == MessageTypes.SFR
                || receiveMsgOrder.ModuleType == MessageTypes.AFR || receiveMsgOrder.ModuleType == MessageTypes.DFR || receiveMsgOrder.ModuleType == MessageTypes.MFR
                || receiveMsgOrder.ModuleType == MessageTypes.UPL || receiveMsgOrder.ModuleType == MessageTypes.OFF || receiveMsgOrder.ModuleType == MessageTypes.AGR
                || receiveMsgOrder.ModuleType == MessageTypes.ASK;
        }

        #endregion

        #region 绑定的属性

        #region 分页

        /// <summary>
        /// 成就分页类
        /// </summary>
        PagingViewModel _pagingAchieve = new PagingViewModel();
        public PagingViewModel pagingAchieve
        {
            get
            {
                if (_pagingAchieve == null)
                    _pagingAchieve = new PagingViewModel();

                return _pagingAchieve;
            }
        }

        /// <summary>
        /// 消息分页类
        /// </summary>
        PagingViewModel _pagingMsg = new PagingViewModel(5);
        public PagingViewModel pagingMsg
        {
            get
            {
                if (_pagingMsg == null)
                    _pagingMsg = new PagingViewModel(5);

                return _pagingMsg;
            }
        }

        #endregion

        #region 个人信息

        string userID;
        string cardWord;
        string userName;
        string imgSrc;
        string passWord;
        string newPassWord;
        string conPassWord;

        public string UserID
        {
            get { return userID; }
            set
            {
                if (value == userID)
                    return;

                userID = value;

                base.RaisePropertyChanged("UserID");
            }
        }

        public string CardWord
        {
            get { return cardWord; }
            set
            {
                if (value == cardWord)
                    return;

                cardWord = value;

                base.RaisePropertyChanged("CardWord");
            }
        }

        public string UserName
        {
            get { return userName; }
            set
            {
                if (value == userName)
                    return;

                userName = value;

                base.RaisePropertyChanged("UserName");
            }
        }

        public string ImgSrc
        {
            get { return imgSrc; }
            set
            {
                if (value == imgSrc)
                    return;

                imgSrc = value;

                base.RaisePropertyChanged("ImgSrc");
            }
        }

        public string PassWord
        {
            get { return passWord; }
            set
            {
                if (value == passWord)
                    return;

                passWord = value;

                base.RaisePropertyChanged("PassWord");

                setPswCommand.RaiseCanExecuteChanged();
            }
        }

        public string NewPassWord
        {
            get { return newPassWord; }
            set
            {
                if (value == newPassWord)
                    return;

                newPassWord = value;

                base.RaisePropertyChanged("NewPassWord");

                setPswCommand.RaiseCanExecuteChanged();
            }
        }

        public string ConPassWord
        {
            get { return conPassWord; }
            set
            {
                if (value == conPassWord)
                    return;

                conPassWord = value;

                base.RaisePropertyChanged("ConPassWord");

                setPswCommand.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region 成就

        int achieve = 0;
        ObservableCollection<AchieveViewModel> _allAchieves { get; set; }

        public int Achieve
        {
            get { return achieve; }
            set
            {
                if (value == achieve)
                    return;

                achieve = value;

                base.RaisePropertyChanged("Achieve");
            }
        }

        public ObservableCollection<AchieveViewModel> AllAchieves
        {
            get { return _allAchieves; }
            set
            {
                _allAchieves = value;
                base.RaisePropertyChanged("AllAchieves");
            }
        }

        #endregion

        #region 消息

        int message = 0;

        public int Message
        {
            get { return message; }
            set
            {
                if (value == message)
                    return;

                message = value;

                base.RaisePropertyChanged("Message");
            }
        }

        //消息列表
        ObservableCollection<MsgViewModel> _allMsgs { get; set; }

        public ObservableCollection<MsgViewModel> AllMsgs
        {
            get { return _allMsgs; }
            set
            {
                _allMsgs = value;
                base.RaisePropertyChanged("AllMsgs");
            }
        }

        #endregion

        #region 好友列表

        int friend = 0;
        public int Friend
        {
            get { return friend; }
            set
            {
                if (value == friend)
                    return;

                friend = value;

                base.RaisePropertyChanged("Friend");
            }
        }

        //我的好友
        ObservableCollection<FriendViewModel> _allFriends { get; set; }

        public ObservableCollection<FriendViewModel> AllFriends
        {
            get
            {
                if (_allFriends == null)
                {
                    _allFriends = new ObservableCollection<FriendViewModel>();
                }

                return _allFriends;
            }
        }

        //我的家族
        ObservableCollection<FamilyViewModel> _allFamilys { get; set; }

        public ObservableCollection<FamilyViewModel> AllFamilys
        {
            get
            {
                if (_allFamilys == null)
                {
                    _allFamilys = new ObservableCollection<FamilyViewModel>();
                }

                return _allFamilys;
            }
        }

        //黑名单
        ObservableCollection<BlackViewModel> _allBlacks { get; set; }

        public ObservableCollection<BlackViewModel> AllBlacks
        {
            get
            {
                if (_allBlacks == null)
                {
                    _allBlacks = new ObservableCollection<BlackViewModel>();
                }

                return _allBlacks;
            }
        }

        #endregion

        #region 查找好友结果

        //结果列表
        ObservableCollection<ResultViewModel> _allResults { get; set; }

        public ObservableCollection<ResultViewModel> AllResults
        {
            get
            {
                if (_allResults == null)
                {
                    _allResults = new ObservableCollection<ResultViewModel>();
                }

                return _allResults;
            }
        }

        #endregion

        #endregion

        #region 绑定的命令

        #region 打开或关闭Popup

        DelegateCommand<Popup> openPOPCommand { get; set; }

        public ICommand OpenPOPCommand
        {
            get
            {
                if (openPOPCommand == null)
                    openPOPCommand = new DelegateCommand<Popup>((p) =>
                    {
                        if (!p.IsOpen)
                            p.IsOpen = true;
                        else
                            p.IsOpen = false;
                    });

                return openPOPCommand;
            }
        }

        #endregion

        #region 修改密码命令

        DelegateCommand setPswCommand { get; set; }

        public ICommand SetPswCommand
        {
            get
            {
                if (setPswCommand == null)
                    setPswCommand = new DelegateCommand(() =>
                    {
                        send_Aggregator.GetEvent<SendMsgEvent>().Publish(MessageTypes.SET + this.CardWord + MessageTypes.NSP + this.PassWord + MessageTypes.NSP + this.NewPassWord);
                    }, CanSet);

                return setPswCommand;
            }
        }

        bool CanSet()
        {
            if (!this.IsValid)
                return false;

            return true;
        }

        #endregion

        #region 刷新游戏数据

        DelegateCommand flushACHCommand { get; set; }

        public ICommand FlushACHCommand
        {
            get
            {
                if (flushACHCommand == null)
                    flushACHCommand = new DelegateCommand(() =>
                    {
                        this.Achieve = 0;
                        this.AllAchieves.Clear();
                        this.AllAchieves = null;
                        this.achieveList.Clear();

                        send_Aggregator.GetEvent<SendMsgEvent>().Publish(MessageTypes.ACH + this.UserID);
                    });

                return flushACHCommand;
            }
        }

        #endregion

        #region 获取最新消息

        DelegateCommand flushMSGCommand { get; set; }

        public ICommand FlushMSGCommand
        {
            get
            {
                if (flushMSGCommand == null)
                    flushMSGCommand = new DelegateCommand(() =>
                {
                    this.Message = 0;
                    this.AllMsgs.Clear();
                    this.AllMsgs = null;
                    this.msgList.Clear();

                    send_Aggregator.GetEvent<SendMsgEvent>().Publish(MessageTypes.MSG + this.UserID);
                });

                return flushMSGCommand;
            }
        }

        #endregion

        #region 刷新好友列表

        DelegateCommand flushFRICommand { get; set; }

        public ICommand FlushFRICommand
        {
            get
            {
                if (flushFRICommand == null)
                    flushFRICommand = new DelegateCommand(() =>
                {
                    this.Friend = 0;
                    this.AllFriends.Clear();
                    this.AllFamilys.Clear();
                    this.AllBlacks.Clear();

                    send_Aggregator.GetEvent<SendMsgEvent>().Publish(MessageTypes.FRI + this.UserID);
                });

                return flushFRICommand;
            }
        }

        #endregion

        #region 查找好友

        DelegateCommand<string> seachFRICommand { get; set; }

        public ICommand SeachFRICommand
        {
            get
            {
                if (seachFRICommand == null)
                    seachFRICommand = new DelegateCommand<string>((msg) =>
                    {
                        this.AllResults.Clear();

                        if (msg.Trim() != null || msg.Trim() != "")
                            send_Aggregator.GetEvent<SendMsgEvent>().Publish(MessageTypes.SFR + msg);
                    });

                return seachFRICommand;
            }
        }

        #endregion

        #region 添加好友

        DelegateCommand<Rectangle> addCommand { get; set; }

        public ICommand AddCommand
        {
            get
            {
                if (addCommand == null)
                    addCommand = new DelegateCommand<Rectangle>((rect) =>
                   {
                       foreach (FriendViewModel friend in this.AllFriends.Where(f => f.FriendID == rect.Tag.ToString()))
                       {
                           XWT.MessageBox.Show("该用户已经是您的好友.");
                           return;
                       }

                       foreach (FamilyViewModel family in this.AllFamilys.Where(f => f.FriendID == rect.Tag.ToString()))
                       {
                           XWT.MessageBox.Show("该用户已经是您的好友.");
                           return;
                       }

                       foreach (BlackViewModel black in this.AllBlacks.Where(b => b.FriendID == rect.Tag.ToString()))
                       {
                           XWT.MessageBox.Show("该用户已经是您的好友.");
                           return;
                       }

                       send_Aggregator.GetEvent<SendMsgEvent>().Publish(MessageTypes.AFR + this.UserID + MessageTypes.NSP + this.CardWord + MessageTypes.NSP + rect.Tag.ToString());

                       XWT.MessageBox.Show("您的好友请求已发送.");

                       this.AllResults.Clear();
                   });

                return addCommand;
            }
        }

        #endregion

        #region 删除好友

        DelegateCommand<string> deleteFRICommand { get; set; }

        public ICommand DeleteFRICommand
        {
            get
            {
                if (deleteFRICommand == null)
                    deleteFRICommand = new DelegateCommand<string>((friendID) =>
                {

                    MessageBoxResult dr;
                    dr = XWT.MessageBox.Show("删除好友后您也会从对方好友中被移除\n\r确认删除?", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    switch (dr)
                    {
                        case MessageBoxResult.OK:

                            send_Aggregator.GetEvent<SendMsgEvent>().Publish(MessageTypes.DFR + this.UserID + MessageTypes.NSP + friendID);

                            // this.AllFriends.Remove(friend);   
                            //  this.Friend--; 

                            break;

                        case MessageBoxResult.Cancel:

                            break;
                    }

                });

                return deleteFRICommand;
            }
        }

        #endregion

        #region 移动好友

        DelegateCommand<Rectangle> moveToFriCommand { get; set; }
        DelegateCommand<Rectangle> moveToFamCommand { get; set; }
        DelegateCommand<Rectangle> moveToBlaCommand { get; set; }

        //移动到我的好友
        public ICommand MoveToFriCommand
        {
            get
            {
                if (moveToFriCommand == null)
                    moveToFriCommand = new DelegateCommand<Rectangle>((rect) =>
                    {
                        if (rect.Tag.ToString() == MessageTypes.Friend)
                            return;
                        //MessageBox.Show("FriendID:"+rect.ToolTip.ToString()+"\n\rFriendGroup:"+rect.Tag.ToString());
                        send_Aggregator.GetEvent<SendMsgEvent>().Publish(MessageTypes.MFR + this.UserID + MessageTypes.NSP + rect.ToolTip.ToString() + MessageTypes.NSP + rect.Tag.ToString() + MessageTypes.NSP + MessageTypes.Friend);
                    });

                return moveToFriCommand;
            }
        }

        //移动到我的家族
        public ICommand MoveToFamCommand
        {
            get
            {
                if (moveToFamCommand == null)
                    moveToFamCommand = new DelegateCommand<Rectangle>((rect) =>
                    {
                        if (rect.Tag.ToString() == MessageTypes.Family)
                            return;
                        // MessageBox.Show("FriendID:" + rect.ToolTip.ToString() + "\n\rFriendGroup:" + rect.Tag.ToString());
                        send_Aggregator.GetEvent<SendMsgEvent>().Publish(MessageTypes.MFR + this.UserID + MessageTypes.NSP + rect.ToolTip.ToString() + MessageTypes.NSP + rect.Tag.ToString() + MessageTypes.NSP + MessageTypes.Family);
                    });

                return moveToFamCommand;
            }
        }

        //移动到黑名单
        public ICommand MoveToBlaCommand
        {
            get
            {
                if (moveToBlaCommand == null)
                    moveToBlaCommand = new DelegateCommand<Rectangle>((rect) =>
                    {
                        if (rect.Tag.ToString() == MessageTypes.Black)
                            return;
                        //MessageBox.Show("FriendID:" + rect.ToolTip.ToString() + "\n\rFriendGroup:" + rect.Tag.ToString());
                        send_Aggregator.GetEvent<SendMsgEvent>().Publish(MessageTypes.MFR + this.UserID + MessageTypes.NSP + rect.ToolTip.ToString() + MessageTypes.NSP + rect.Tag.ToString() + MessageTypes.NSP + MessageTypes.Black);
                    });

                return moveToBlaCommand;
            }
        }

        #endregion

        #region 成就列表翻页控制

        DelegateCommand ach_fpageCommand { get; set; }
        DelegateCommand ach_ppageCommand { get; set; }
        DelegateCommand ach_npageCommand { get; set; }
        DelegateCommand ach_lpageCommand { get; set; }

        //首页
        public ICommand ACH_FPageCommand
        {
            get
            {
                if (ach_fpageCommand == null)
                    ach_fpageCommand = new DelegateCommand(() =>
                    {
                        ChangePage(1, Paging.ACH, this._pagingAchieve);
                    });

                return ach_fpageCommand;
            }
        }

        //上一页
        public ICommand ACH_PPageCommand
        {
            get
            {
                if (ach_ppageCommand == null)
                    ach_ppageCommand = new DelegateCommand(() =>
                    {
                        if (this._pagingAchieve.CurrentPage > 1)
                            ChangePage(--this._pagingAchieve.CurrentPage, Paging.ACH, this._pagingAchieve);
                    });

                return ach_ppageCommand;
            }
        }

        //下一页
        public ICommand ACH_NPageCommand
        {
            get
            {
                if (ach_npageCommand == null)
                    ach_npageCommand = new DelegateCommand(() =>
                    {
                        if (this._pagingAchieve.CurrentPage < this._pagingAchieve.ToltalPage)
                            ChangePage(++this._pagingAchieve.CurrentPage, Paging.ACH, this._pagingAchieve);
                    });

                return ach_npageCommand;
            }
        }

        //末页
        public ICommand ACH_LPageCommand
        {
            get
            {
                if (ach_lpageCommand == null)
                    ach_lpageCommand = new DelegateCommand(() =>
                    {
                        ChangePage(this._pagingAchieve.ToltalPage, Paging.ACH, this._pagingAchieve);
                    });

                return ach_lpageCommand;
            }
        }

        #endregion

        #region 消息列表翻页控制

        DelegateCommand msg_fpageCommand { get; set; }
        DelegateCommand msg_ppageCommand { get; set; }
        DelegateCommand msg_npageCommand { get; set; }
        DelegateCommand msg_lpageCommand { get; set; }

        //首页
        public ICommand MSG_FPageCommand
        {
            get
            {
                if (msg_fpageCommand == null)
                    msg_fpageCommand = new DelegateCommand(() =>
                    {
                        ChangePage(1, Paging.MSG, this._pagingMsg);
                    });

                return msg_fpageCommand;
            }
        }

        //上一页
        public ICommand MSG_PPageCommand
        {
            get
            {
                if (msg_ppageCommand == null)
                    msg_ppageCommand = new DelegateCommand(() =>
                    {
                        if (this._pagingMsg.CurrentPage > 1)
                            ChangePage(--this._pagingMsg.CurrentPage, Paging.MSG, this._pagingMsg);
                    });

                return msg_ppageCommand;
            }
        }

        //下一页
        public ICommand MSG_NPageCommand
        {
            get
            {
                if (msg_npageCommand == null)
                    msg_npageCommand = new DelegateCommand(() =>
                    {
                        if (this._pagingMsg.CurrentPage < this._pagingMsg.ToltalPage)
                            ChangePage(++this._pagingMsg.CurrentPage, Paging.MSG, this._pagingMsg);
                    });

                return msg_npageCommand;
            }
        }

        //末页
        public ICommand MSG_LPageCommand
        {
            get
            {
                if (msg_lpageCommand == null)
                    msg_lpageCommand = new DelegateCommand(() =>
                    {
                        ChangePage(this._pagingMsg.ToltalPage, Paging.MSG, this._pagingMsg);
                    });

                return msg_lpageCommand;
            }
        }

        #endregion

        #region 忽略消息

        DelegateCommand<object> ignoreMsgCommand { get; set; }

        public ICommand IgnoreMsgCommand
        {
            get
            {
                if (ignoreMsgCommand == null)
                    ignoreMsgCommand = new DelegateCommand<object>((info) =>
                {
                    string msginfo = info as string;
                    string[] msg = msginfo.Split(',');
                    switch (msg[0])
                    {
                        case MessageTypes.F:
                            send_Aggregator.GetEvent<SendMsgEvent>().Publish(MessageTypes.NGR + this.UserID + MessageTypes.NSP + msg[2]);

                            msgList.RemoveAll((msgViewModel) => msgViewModel.MsgInfo.Split(',')[1] == msg[1]);

                            break;

                        default:

                            msgList.RemoveAll((msgViewModel) => msgViewModel.MsgInfo.Split(',')[1] == msg[1]);

                            break;
                    }

                    if (this.Message > 0)
                        this.Message--;

                    ChangePage(this._pagingMsg.CurrentPage == 0 ? 1 : this._pagingMsg.CurrentPage, Paging.MSG, this._pagingMsg);

                });

                return ignoreMsgCommand;
            }
        }

        #endregion

        #region 同意好友添加请求

        DelegateCommand<object> agreeAddCommand { get; set; }

        public ICommand AgreeAddCommand
        {
            get
            {
                if (agreeAddCommand == null)
                    agreeAddCommand = new DelegateCommand<object>((info) =>
                {
                    string msginfo = info as string;
                    string[] msg = msginfo.Split(',');

                    send_Aggregator.GetEvent<SendMsgEvent>().Publish(MessageTypes.AGR + this.UserID + MessageTypes.NSP + msg[2]);

                    msgList.RemoveAll((msgViewModel) => msgViewModel.MsgInfo.Split(',')[1] == msg[1]);

                    if (this.Message > 0)
                        this.Message--;

                    ChangePage(this._pagingMsg.CurrentPage == 0 ? 1 : this._pagingMsg.CurrentPage, Paging.MSG, this._pagingMsg);

                });

                return agreeAddCommand;
            }
        }

        #endregion

        #region 查看消息

        DelegateCommand<object> scanMsgCommand { get; set; }

        public ICommand ScanMsgCommand
        {
            get
            {
                if (scanMsgCommand == null)
                    scanMsgCommand = new DelegateCommand<object>((code) =>
                {
                    string msgcode = code as string;
                    MessageBox.Show(msgcode);
                });

                return scanMsgCommand;
            }
        }

        #endregion

        #endregion

        #region 分页方法

        /// <summary>
        /// 分页调用
        /// </summary>
        /// <param name="currentpage">当前页</param>
        /// <param name="pagingType">分页类型：成就或信息</param>
        /// <param name="pageViewModel">分页ViewModel</param>
        private void ChangePage(int currentpage, Paging pagingType, PagingViewModel pageViewModel)
        {
            switch (pagingType)
            {
                case Paging.ACH:

                    AchievePaging(currentpage, pageViewModel);
                    break;

                case Paging.MSG:

                    MsgPaging(currentpage, pageViewModel);
                    break;
            }
        }

        /// <summary>
        /// 执行成就分页
        /// </summary> 
        private void AchievePaging(int currentpage, PagingViewModel pageViewModel)
        {
            if (achieveList != null)
            {
                if (achieveList.Count % pageViewModel.pagecount == 0)
                {
                    pageViewModel.ToltalPage = achieveList.Count / pageViewModel.pagecount;
                    pageViewModel.PageAndToltal = 0 + "/" + pageViewModel.ToltalPage;
                }
                else
                {
                    pageViewModel.CurrentPage = currentpage;
                    pageViewModel.ToltalPage = achieveList.Count / pageViewModel.pagecount + 1;
                    pageViewModel.PageAndToltal = currentpage + "/" + pageViewModel.ToltalPage;
                }

                if (achieveList.Count < pageViewModel.pagecount)
                    pageViewModel.SumAndToltal = "记录:" + achieveList.Count + "/" + achieveList.Count;
                else
                    pageViewModel.SumAndToltal = "记录:" + pageViewModel.pagecount + "/" + achieveList.Count;

                this.AllAchieves = new ObservableCollection<AchieveViewModel>(achieveList.Take(pageViewModel.pagecount * currentpage).Skip(pageViewModel.pagecount * (currentpage - 1)));
            }
        }

        /// <summary>
        /// 执行消息分页
        /// </summary> 
        private void MsgPaging(int currentpage, PagingViewModel pageViewModel)
        {
            if (msgList != null)
            {
                if (msgList.Count % pageViewModel.pagecount == 0)
                {
                    pageViewModel.ToltalPage = msgList.Count / pageViewModel.pagecount;
                    pageViewModel.PageAndToltal = 0 + "/" + pageViewModel.ToltalPage;
                }
                else
                {
                    pageViewModel.CurrentPage = currentpage;
                    pageViewModel.ToltalPage = msgList.Count / pageViewModel.pagecount + 1;
                    pageViewModel.PageAndToltal = currentpage + "/" + pageViewModel.ToltalPage;
                }

                if (msgList.Count < pageViewModel.pagecount)
                    pageViewModel.SumAndToltal = "记录:" + msgList.Count + "/" + msgList.Count;
                else
                    pageViewModel.SumAndToltal = "记录:" + pageViewModel.pagecount + "/" + msgList.Count;

                this.AllMsgs = new ObservableCollection<MsgViewModel>(msgList.Take(pageViewModel.pagecount * currentpage).Skip(pageViewModel.pagecount * (currentpage - 1)));
            }
        }

        #endregion

        #region IDataErrorInfo 成员

        static readonly string[] ValidatedProperties = { "PassWord", "NewPassWord", "ConPassWord" };

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
                case "PassWord":
                    msg = ValidatePassword();
                    break;

                case "NewPassWord":
                    msg = ValidateNewPassword();
                    break;

                case "ConPassWord":
                    msg = ValidateConPassword();
                    break;

                default:
                    Debug.Fail("未能识别信息: " + propertyName);
                    break;
            }
            return msg;
        }

        #region IDataErrorInfo 方法


        string ValidatePassword()
        {
            if (IsStringMissing(this.PassWord))
                return "密码不能为空";

            return null;
        }

        string ValidateNewPassword()
        {
            if (IsStringMissing(this.NewPassWord))
                return "密码不能为空";

            if (!Regex.IsMatch(this.NewPassWord, @"^[A-Za-z0-9]{6,12}$"))
                return "6-12位,字母或数字";

            return null;
        }

        string ValidateConPassword()
        {
            if (IsStringMissing(this.ConPassWord))
                return "确认密码不能为空";

            if (this.NewPassWord != this.ConPassWord)
                return "前后输入密码不一致";

            return null;
        }

        #endregion

        #endregion // IDataErrorInfo Members
    }
}
