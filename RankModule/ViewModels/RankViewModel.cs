
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Events;
using MessageModule.ModuleMsg;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;
using System.Xml;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace RankModule.ViewModels
{
    [Export]
    public class RankViewModel
    {
        #region 变量

        IRegionManager regionManager;
        IEventAggregator module_Aggregator;

        ModuleMsgOrder moduleMsgOrder;

        #endregion

        #region 构造函数

        [ImportingConstructor]
        public RankViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.module_Aggregator = eventAggregator;

            moduleMsgOrder = new ModuleMsgOrder();
        }

        #endregion

        #region 绑定的命令

        DelegateCommand<TextBlock> showCommand { get; set; }

        #region 查看命令

        public ICommand ShowCommand
        {
            get
            {
                if (showCommand == null)
                    showCommand = new DelegateCommand<TextBlock>((textBlock) =>
                    {
                        XmlAttribute x_id = textBlock.Tag as XmlAttribute; 

                        moduleMsgOrder.Sign = 0; //得到ID/切换到“介绍”界面 
                        moduleMsgOrder.GameId = x_id.FirstChild.Value; //游戏唯一ID
                        moduleMsgOrder.GameName = textBlock.Text;

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
