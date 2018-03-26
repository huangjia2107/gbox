
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;
using System.Xml;
using Microsoft.Practices.Prism.Events;
using MessageModule.ModuleMsg;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace AllGamesModule.ViewModels
{
    [Export]
    public class AllGamesViewModel
    {
        #region 变量

        IRegionManager regionManager; 
        IEventAggregator module_Aggregator;

        ModuleMsgOrder moduleMsgOrder;

        #endregion

        #region 构造函数

        [ImportingConstructor]
        public AllGamesViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.module_Aggregator = eventAggregator;

            moduleMsgOrder = new ModuleMsgOrder();
        }

        #endregion

        #region 绑定的命令

        DelegateCommand<Border> showCommand { get; set; }

        #region 查看命令

        public ICommand ShowCommand
        {
            get {
                if (showCommand == null)
                    showCommand = new DelegateCommand<Border>((border) =>
                    {
                        XmlAttribute x_id = border.Tag as XmlAttribute;
                        XmlAttribute x_name = border.ToolTip as XmlAttribute;

                        moduleMsgOrder.Sign = 0; //得到ID/切换到“介绍”界面 
                        moduleMsgOrder.GameId = x_id.FirstChild.Value; //游戏唯一ID
                        moduleMsgOrder.GameName = x_name.FirstChild.Value;

                        //将游戏ID广播出去
                        module_Aggregator.GetEvent<ModuleMsgEvent>().Publish(moduleMsgOrder);
                    }
                );

                return showCommand;
            }
        }

        #endregion

        #endregion
    }
}
