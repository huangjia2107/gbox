
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;
using Microsoft.Practices.Prism.Events;
using MessageModule.ModuleMsg;
using Microsoft.Practices.Prism.ViewModel;
using IntroductionModule.Models;
using IntroductionModule.DataAccess;
using MessageModule.SendMsg;
using System.ComponentModel.Composition;
using MessageModule.MessageTypes;
using Configure.Models;
using Configure;
using System.Collections.ObjectModel;
using XWT = Xceed.Wpf.Toolkit;
using System.Windows.Controls.Primitives;
using IntroductionModule.Views;
using System.Windows.Controls;
using System.Windows.Interop;
using MessageModule.ReceiveMsg;
using MessageModule.AddGameMsg;

namespace IntroductionModule.ViewModels
{
    [Export]
    public class IntroductionViewModel : NotificationObject
    {
        #region 变量

        GameModel _gameModel;
        ListModel _listModel;
        DownView downView;

        IRegionManager regionManager;
        IEventAggregator send_Aggregator;
        IEventAggregator module_Aggregator; 
        SubscriptionToken subscriptionToken;

        ModuleMsgOrder moduleMsgOrder;


        #endregion

        #region 构造函数

        [ImportingConstructor]
        public IntroductionViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.module_Aggregator = eventAggregator;
            this.send_Aggregator = eventAggregator; 

            moduleMsgOrder = new ModuleMsgOrder();

            _gameModel = GameModel.CreateNewModel();
            _listModel = ListModel.CreateNewModel();
            downView = new DownView(new DownViewModel(), eventAggregator);

            RequestEvent(); 
        }

        #endregion

        #region IEventAggregator订阅接收信息事件(来自：所有游戏/最近发布/一周排行/热门搜索/图片展示)

        public void RequestEvent()
        {
            ModuleMsgEvent moduleMsgEvent = module_Aggregator.GetEvent<ModuleMsgEvent>();

            if (subscriptionToken != null)
                moduleMsgEvent.Unsubscribe(subscriptionToken);

            subscriptionToken = moduleMsgEvent.Subscribe(OnModuleMsg, ThreadOption.UIThread, false, Filter);
        }

        public void OnModuleMsg(ModuleMsgOrder _moduleMsgOrder)
        {
            if (_moduleMsgOrder.GameId == null) //搜索时，游戏ID为null
            {
                if (GameAccess.GetGameByName(_moduleMsgOrder.GameName) == null)
                {
                    XWT.MessageBox.Show("抱歉,不存在当前游戏.");
                    return;
                }

                _gameModel = GameAccess.GetGameByName(_moduleMsgOrder.GameName);
            }
            else
            {
                if (GameAccess.GetGameByID(_moduleMsgOrder.GameId) == null)
                {
                    XWT.MessageBox.Show("抱歉,不存在当前游戏.");
                    return;
                }

                _gameModel = GameAccess.GetGameByID(_moduleMsgOrder.GameId);
            }

            this.ID = _gameModel.ID;
            this.GameName = _gameModel.GameName;
            this.GameType = _gameModel.GameType;
            this.Icon = _gameModel.Icon;
            this.PublishData = _gameModel.PublishData;
            this.IsEnabled = _gameModel.IsEnabled;
            this.ImgSrc1 = _gameModel.ImgSrc1;
            this.ImgSrc2 = _gameModel.ImgSrc2;
            this.GameDetail = _gameModel.GameDetail;

            //MsgEvent msgEvent = eventAggregator.GetEvent<MsgEvent>();

            //if (subscriptionToken != null)
            //    msgEvent.Unsubscribe(subscriptionToken);
        }

        public bool Filter(ModuleMsgOrder _moduleMsgOrder)
        {
            return _moduleMsgOrder.Sign == 0; //得到ID或游戏名/切换到“介绍”界面 
        }

        #endregion

        #region 绑定的属性

        #region 游戏数据

        string ID;
        string gameName;
        string gameType;
        string publishData;
        string icon;
        bool isenabled;
        string imgSrc1;
        string imgSrc2;
        string gameDetail;

        public string GameName
        {
            get { return gameName; }
            set
            {
                if (value == gameName)
                    return;

                gameName = value;
                base.RaisePropertyChanged("GameName");
            }
        }

        public string GameType
        {
            get { return gameType; }
            set
            {
                if (value == gameType)
                    return;

                gameType = value;
                base.RaisePropertyChanged("GameType");
            }
        }

        public string PublishData
        {
            get { return publishData; }
            set
            {
                if (value == publishData)
                    return;

                publishData = value;
                base.RaisePropertyChanged("PublishData");
            }
        }

        public string Icon
        {
            get { return icon; }
            set
            {
                if (value == icon)
                    return;

                icon = value;
                base.RaisePropertyChanged("Icon");
            }
        }

        public bool IsEnabled
        {
            get { return isenabled; }
            set
            {
                if (value == isenabled)
                    return;

                isenabled = value;
                base.RaisePropertyChanged("IsEnabled");
            }
        }

        public string ImgSrc1
        {
            get { return imgSrc1; }
            set
            {
                if (value == imgSrc1)
                    return;

                imgSrc1 = value;
                base.RaisePropertyChanged("ImgSrc1");
            }
        }

        public string ImgSrc2
        {
            get { return imgSrc2; }
            set
            {
                if (value == imgSrc2)
                    return;

                imgSrc2 = value;
                base.RaisePropertyChanged("ImgSrc2");
            }
        }

        public string GameDetail
        {
            get { return gameDetail; }
            set
            {
                if (value == gameDetail)
                    return;

                gameDetail = value;
                base.RaisePropertyChanged("GameDetail");
            }
        }

        #endregion 

        #endregion

        #region 绑定的命令 

        #region 返回命令

        DelegateCommand backCommand { get; set; }

        public ICommand BackCommand
        {
            get
            {
                if (backCommand == null)
                    backCommand = new DelegateCommand(() =>
                {
                    moduleMsgOrder.GameName = null;
                    moduleMsgOrder.GameId = null;
                    moduleMsgOrder.Sign = 1; //切换到“原”页面 

                    module_Aggregator.GetEvent<ModuleMsgEvent>().Publish(moduleMsgOrder);
                });

                return backCommand;
            }
        }

        #endregion

        #region 添加游戏命令

        DelegateCommand<UserControl> addGameCommand { get; set; }

        public ICommand AddGameCommand
        {
            get
            {
                if (addGameCommand == null)
                    addGameCommand = new DelegateCommand<UserControl>((userControl) =>
                    {
                        try
                        { 
                            //设置owner
                            HwndSource winformWindow = (System.Windows.Interop.HwndSource.FromDependencyObject(userControl) as System.Windows.Interop.HwndSource);
                            if (winformWindow != null)
                                new WindowInteropHelper(downView) { Owner = winformWindow.Handle };

                            bool IsCanExcute = true;

                            
                            _listModel = ListModel.CreateModel(this.ID,this.GameName, "准备中.", "0MB", "0", "0KB/S");
                            ListViewModel listViewModel = new ListViewModel(_listModel);

                            downView.BeginDown(out IsCanExcute,this.ID, listViewModel);

                            if (IsCanExcute)
                            {
                                downView.Show();
                                this.IsEnabled = false;
                            }
                        }
                        catch { this.IsEnabled = true; }
                    });

                return addGameCommand;
            }
        }

        #endregion 

        #endregion
    }
}
