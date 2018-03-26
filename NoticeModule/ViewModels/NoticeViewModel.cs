using System;

using NoticeModule.Models;
using NoticeModule.DataAccess;
using ToolClass.LinkedList;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Regions;
using System.Xml;
using MessageModule.ModuleMsg;
using Microsoft.Practices.Prism.Events;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace NoticeModule.ViewModels
{
    [Export]
    public class NoticeViewModel : NotificationObject
    {
        #region 变量

        ShowModel _showModel;
        ShowAccess _showAccess;
        static LoopLink<ShowModel> _loopLink;
        DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Render);

        IRegionManager regionManager;
        IEventAggregator module_Aggregator;

        ModuleMsgOrder moduleMsgOrder;

        #endregion

        #region 构造函数

        [ImportingConstructor]
        public NoticeViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.module_Aggregator = eventAggregator;

            _showAccess = new ShowAccess();
            _loopLink = ShowAccess.GetShow();

            moduleMsgOrder = new ModuleMsgOrder();
        }

        #endregion

        #region 绑定的属性

        string id;
        string imgsrc;
        string title;

        public string ID
        {
            get { return id; }
            set
            {
                if (value == id)
                    return;

                id = value;
                base.RaisePropertyChanged("ID");
            }
        }

        public string ImgSrc
        {
            get { return imgsrc; }
            set
            {
                if (value == imgsrc)
                    return;

                imgsrc = value;
                base.RaisePropertyChanged("ImgSrc");
            }
        }

        public string Title
        {
            get { return title; }
            set
            {
                if (value == title)
                    return;

                title = value;
                base.RaisePropertyChanged("Title");
            }
        }

        #endregion

        #region 绑定的命令

        //控制图片Show
        DelegateCommand loadCommand { get; set; }
        DelegateCommand nextCommand { get; set; }
        DelegateCommand lastCommand { get; set; }

        #region Picture加载,计时开始

        public ICommand LoadCommand
        {
            get
            {
                if (loadCommand == null)
                    loadCommand = new DelegateCommand(OnLoadExcute);

                return loadCommand;
            }
        }

        void OnLoadExcute()
        {
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 2);
            timer.IsEnabled = true;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {
            _showModel = _loopLink.GetNext();

            this.ID = _showModel.ID;
            this.Title = _showModel.Title;
            this.ImgSrc = _showModel.PicSrc;
        }

        #endregion

        #region 下一页

        public ICommand NextCommand
        {
            get
            {
                if (nextCommand == null)
                    nextCommand = new DelegateCommand(OnNextExcute);

                return nextCommand;
            }
        }

        void OnNextExcute()
        {
            timer.Stop();

            _showModel = _loopLink.GetNext();

            this.ID = _showModel.ID;
            this.Title = _showModel.Title;
            this.ImgSrc = _showModel.PicSrc;

            timer.Start();
        }

        #endregion

        #region 上一页

        public ICommand LastCommand
        {
            get
            {
                if (lastCommand == null)
                    lastCommand = new DelegateCommand(OnLastExcute);

                return lastCommand;
            }
        }

        void OnLastExcute()
        {
            timer.Stop();

            _showModel = _loopLink.GetPrevious();

            this.ID = _showModel.ID;
            this.Title = _showModel.Title;
            this.ImgSrc = _showModel.PicSrc;

            timer.Start();
        }

        #endregion

        DelegateCommand<Border> showCommand { get; set; }
        DelegateCommand<TextBlock> showPicCommand { get; set; }

        #region 查看命令

        public ICommand ShowCommand
        {
            get
            {
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

        public ICommand ShowPicCommand
        {
            get
            {
                if (showPicCommand == null)
                    showPicCommand = new DelegateCommand<TextBlock>((textBlock) =>
                    { 
                        moduleMsgOrder.Sign = 0; //得到ID或游戏名/切换到“介绍”界面 
                        moduleMsgOrder.GameId = textBlock.Tag.ToString(); //游戏唯一ID
                        moduleMsgOrder.GameName = textBlock.Text;

                        //将游戏ID广播出去
                        module_Aggregator.GetEvent<ModuleMsgEvent>().Publish(moduleMsgOrder);
                    });

                return showPicCommand;
            }
        }

        #endregion

        #endregion
    }
}
