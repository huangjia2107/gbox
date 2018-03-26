using MessageModule.MessageTypes;
using MessageModule.SendMsg;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.ViewModel;
using MyGamesModule.DataAccess;
using MyGamesModule.Models;
using MyGamesModule.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using Xceed.Wpf.Toolkit;

namespace MyGamesModule.ViewModels
{
    public class GameViewModel : NotificationObject
    {
        #region 变量

        readonly GameModel _gameModel;
        IModuleManager moduleManager;
        MyGamesViewModel myGamesVuewModel;
        IEventAggregator send_Aggregator;

        #endregion

        #region 构造函数

        public GameViewModel(GameModel gameModel, IModuleManager moduleManager,IEventAggregator eventAggregator, MyGamesViewModel myGamesVuewModel)
        {
            _gameModel=gameModel;
            this.moduleManager = moduleManager;
            this.send_Aggregator = eventAggregator;
            this.myGamesVuewModel = myGamesVuewModel;
        }

        #endregion

        #region 绑定的属性

        public string ID
        {
            get { return _gameModel.ID; }
            set
            {
                if (value == _gameModel.ID)
                    return;

                _gameModel.ID = value;
                base.RaisePropertyChanged("ID");
            }
        }

        public string GameName
        {
            get { return _gameModel.GameName; }
            set
            {
                if (value == _gameModel.GameName)
                    return;

                _gameModel.GameName = value;
                base.RaisePropertyChanged("GameName");
            }
        }

        public string ModuleName
        {
            get { return _gameModel.ModuleName; }
            set
            {
                if (value == _gameModel.ModuleName)
                    return;

                _gameModel.ModuleName = value;
                base.RaisePropertyChanged("ModuleName");
            }
        }

        public string Icon
        {
            get { return _gameModel.Icon; }
            set
            {
                if (value == _gameModel.Icon)
                    return;

                _gameModel.Icon = value;
                base.RaisePropertyChanged("Icon");
            }
        }


        bool isEnabled = true;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set 
            {
                if (value == isEnabled)
                    return;

                isEnabled = value;
                base.RaisePropertyChanged("IsEnabled");
            }
        }

        #endregion

        #region 绑定的命令

        #region 删除游戏命令

        DelegateCommand deleteCommand { get; set; }

        public ICommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                    deleteCommand = new DelegateCommand(() =>
                    {
                        foreach (GameViewModel model in myGamesVuewModel.AllGames)
                        {
                            if (model.ID == this.ID)
                            {
                                myGamesVuewModel.AllGames.Remove(model);
                                break;
                            }
                        }

                        try
                        {
                            GameAccess.DeleteGame(this.ModuleName);
                        }
                        catch { }

                    });

                return deleteCommand;
            }
        }

        #endregion 

        #region 打开游戏命令

        DelegateCommand<Border> openCommand { get; set; }

        public ICommand OpenCommand
        {
            get
            {
                if (openCommand == null)
                    openCommand = new DelegateCommand<Border>((border) =>
                    {
                        this.IsEnabled = false;
                        try
                        {
                            //this.send_Aggregator.GetEvent<SendMsgEvent>().Publish(MessageTypes.GAM + this.ID + MessageTypes.NSP + GameAccess.GetGameTime(this.ModuleName));

                            OpenViewModel openViewModel = new OpenViewModel(this.moduleManager, border);

                            OpenWindow openWindow = new OpenWindow(openViewModel);

                            //设置owner
                            HwndSource winformWindow = (System.Windows.Interop.HwndSource.FromDependencyObject(border) as System.Windows.Interop.HwndSource);
                            if (winformWindow != null)
                                new WindowInteropHelper(openWindow) { Owner = winformWindow.Handle };

                            openWindow.Show();
                        }
                        catch { this.IsEnabled = true; }
                    });

                return openCommand;
            }
        }

        #endregion

        #endregion
    }
}
