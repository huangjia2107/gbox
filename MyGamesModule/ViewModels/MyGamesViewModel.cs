
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using MessageModule.ReceiveMsg;
using System.ComponentModel.Composition;
using MessageModule.MessageTypes;
using Microsoft.Practices.Prism.Modularity;
using System.Collections.ObjectModel;
using MyGamesModule.DataAccess;
using MyGamesModule.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Controls;
using System.Windows.Input;
using MyGamesModule.Views;
using System.Windows.Interop;
using MessageModule.RegionTypes;
using MessageModule.AddGameMsg;
using Xceed.Wpf.Toolkit;
using MessageModule.SendMsg;

namespace MyGamesModule.ViewModels
{
    [Export]
    public class MyGamesViewModel
    {
        #region 变量

        IModuleManager moduleManager;
        IRegionManager regionManager;
        IEventAggregator addgame_Aggregator;
        IEventAggregator send_Aggregator;
        SubscriptionToken subscriptionToken;

        #endregion

        #region 构造函数

        [ImportingConstructor]
        public MyGamesViewModel(IModuleManager moduleManager,IRegionManager regionManager,IEventAggregator eventAggregator)
        {
            this.moduleManager = moduleManager;
            this.regionManager = regionManager;
            this.addgame_Aggregator = eventAggregator;
            this.send_Aggregator = eventAggregator;

            RequestEvent(); //订阅接收信息事件
        }

        #endregion

        #region IEventAggregator订阅接收信息事件

        public void RequestEvent()
        {
            AddGameMsgEvent addGameMsgEvent = addgame_Aggregator.GetEvent<AddGameMsgEvent>();

            if (subscriptionToken != null)
                addGameMsgEvent.Unsubscribe(subscriptionToken);

            subscriptionToken = addGameMsgEvent.Subscribe(OnReceiveMsg, ThreadOption.UIThread, false, Filter);
        }

        public void OnReceiveMsg(AddGameMsgOrder addGameMsgOrder)
        {  
            //根据id添加游戏
            this.AllGames.Add(new GameViewModel(GameAccess.GetGame(addGameMsgOrder.GameId,addGameMsgOrder.GameTime), this.moduleManager, this.addgame_Aggregator,this));

            send_Aggregator.GetEvent<SendMsgEvent>().Publish(MessageTypes.AGA + addGameMsgOrder.GameId);

            //ReceiveMsgEvent msgEvent = receive_Aggregator.GetEvent<ReceiveMsgEvent>();

            //if (subscriptionToken != null)
            //    msgEvent.Unsubscribe(subscriptionToken);
        }

        public bool Filter(AddGameMsgOrder addGameMsgOrder)
        {
            return true;
        }

        #endregion

        #region 绑定的属性

        //我的游戏列表
        ObservableCollection<GameViewModel> _allGames { get; set; }

        public ObservableCollection<GameViewModel> AllGames
        {
            get
            {
                if (_allGames == null)
                {
                    List<GameViewModel> all = (from game in GameAccess.GetGame()
                                               select new GameViewModel(game, this.moduleManager, this.addgame_Aggregator,this)).ToList();
                    _allGames = new ObservableCollection<GameViewModel>(all);
                }

                return _allGames;
            }
        }

        #endregion 

        #region 刷新

        DelegateCommand flushCommand { get; set; }

        public ICommand FlushCommand
        {
            get
            {
                if (flushCommand == null)
                    flushCommand = new DelegateCommand(() =>
                {
                    this.AllGames.Clear();

                    foreach (GameModel model in GameAccess.GetGame())
                    {
                        AllGames.Add(new GameViewModel(model, this.moduleManager,this.addgame_Aggregator, this));
                    } 
                });

                return flushCommand;
            }
        }

        #endregion
    }
}
