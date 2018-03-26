using System;
using System.Collections.Generic;
using System.Linq;

using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.ViewModel;
using SeachModule.Models;
using SeachModule.DataAccess;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Input;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Events;
using MessageModule.ModuleMsg;
using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Xml;

namespace SeachModule.ViewModels
{
    [Export]
    public class SeachViewModel : NotificationObject
    {
        #region 变量

        TagModel _tagModel;
        readonly TagAccess _tagAccess;

        IRegionManager regionManager;
        IEventAggregator module_Aggregator;

        ModuleMsgOrder moduleMsgOrder;

        ObservableCollection<TagViewModel> _allTags { get; set; }

        #endregion

        #region 构造函数

        [ImportingConstructor]
        public SeachViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.module_Aggregator = eventAggregator;

            moduleMsgOrder = new ModuleMsgOrder();

            _tagModel = TagModel.CreateNewModel();
            _tagAccess = new TagAccess();
        }

        #endregion

        #region 绑定的属性

        string seachText;

        public string SeachText
        {
            get { return seachText; }
            set
            {
                if (value == seachText)
                    return;

                seachText = value;
                base.RaisePropertyChanged("SeachText");
            }
        }

        public ObservableCollection<TagViewModel> AllTags
        {
            get
            {
                if (_allTags == null)
                {
                    List<TagViewModel> all = (from tag in _tagAccess.GetTag()
                                               select new TagViewModel(tag, _tagAccess)).ToList();

                    //foreach (TagViewModel vm in all)
                    //    vm.RequestDelete += this.OnUserViewModelRequestDelete;

                    _allTags = new ObservableCollection<TagViewModel>(all);
                    //_allTags.CollectionChanged += this.OnCollectionChanged;
                }
                return _allTags;
            }
        }

        #endregion

        #region 绑定的命令

        DelegateCommand seachCommand { get; set; }
        DelegateCommand<TextBlock> showCommand { get; set; }

        #region 搜索命令

        public ICommand SeachCommand
        {
            get
            {
                if (seachCommand == null)
                    seachCommand = new DelegateCommand(OnSeachExcute, () => { return true; });

                return seachCommand;
            }              
        }

        void OnSeachExcute()
        {
            if (IsStringMissing(this.SeachText))
                return;

            moduleMsgOrder.Sign = 0; //得到ID/切换到“介绍”界面 
            moduleMsgOrder.GameId = null;
            moduleMsgOrder.GameName = this.SeachText;

            //将游戏ID广播出去
            module_Aggregator.GetEvent<ModuleMsgEvent>().Publish(moduleMsgOrder);
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

        #endregion

        public ICommand ShowCommand
        {
            get
            {
                if (showCommand == null)
                    showCommand = new DelegateCommand<TextBlock>((textBlock) =>
                    {  
                        moduleMsgOrder.Sign = 0; //得到ID/切换到“介绍”界面 
                        moduleMsgOrder.GameId = textBlock.Tag.ToString(); //游戏唯一ID
                        moduleMsgOrder.GameName = textBlock.Text;

                        //将游戏ID广播出去
                        module_Aggregator.GetEvent<ModuleMsgEvent>().Publish(moduleMsgOrder);
                    }
                );

                return showCommand;
            }
        }

        #endregion
    }
}
