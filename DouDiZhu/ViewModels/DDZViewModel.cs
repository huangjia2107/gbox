using MessageModule.MessageTypes;
using MessageModule.ReceiveMsg;
using MessageModule.RegionTypes;
using MessageModule.SendMsg;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace doudizhu.ViewModels
{
    [Export]
    public class DDZViewModel
    {
        #region 变量

        IEventAggregator eventAggregator;
        IRegionManager regionManager;
        string[] UserMsg = null;

        #endregion

        #region 构造函数

        [ImportingConstructor]
        public DDZViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            //获得用户信息
            UserMsg = this.regionManager.Regions[RegionTypes.CardRegion].Context as string[];
        }

        #endregion

        #region 绑定的属性

        public string UserID
        {
            get
            {
                return UserMsg == null ? "" : UserMsg[0];
            }
        }

        public string CardWord
        {
            get 
            {
                return UserMsg == null ? "" : UserMsg[1];
            }
        }

        public string UserName
        {
            get
            {
                return UserMsg == null ? "" : UserMsg[2];
            }
        }

        public string UserImg
        {
            get
            {
                return UserMsg == null ? "" : UserMsg[3];
            }
        }

        #endregion

        #region 绑定的命令

        DelegateCommand overCommand { get; set; }

        public ICommand OverCommand
        {
            get
            {
                if (overCommand == null)
                    overCommand = new DelegateCommand(() =>
                {
                    eventAggregator.GetEvent<ReceiveMsgEvent>().Publish(
                        new ReceiveMsgOrder 
                        { 
                            ModuleType = MessageTypes.ACH, 
                            Sign = 1, 
                            MsgContent = "斗地主"+MessageTypes.NSP+"25"+MessageTypes.NSP+ "1"+MessageTypes.NSP+"2500"+MessageTypes.NSP+"300"
                        }); 
                     
                });

                return overCommand;
            }
        } 

        #endregion
    }
}
