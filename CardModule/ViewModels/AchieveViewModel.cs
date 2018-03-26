using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CardModule.Models;
using Microsoft.Practices.Prism.ViewModel;

namespace CardModule.ViewModels
{
    public class AchieveViewModel : NotificationObject
    {
        #region 变量

        readonly AchieveModel _achieveModel;

        #endregion

        #region 构造函数

        public AchieveViewModel(AchieveModel achieveModel)
        {
            _achieveModel = achieveModel;
        }

        #endregion

        #region 绑定的属性

        public string GameName
        { 
            get { return _achieveModel.GameName; }
            set 
            {
                if (value == _achieveModel.GameName)
                    return;

                _achieveModel.GameName = value;
                base.RaisePropertyChanged("GameName");
            }
        }

        public string Level
        {
            get { return _achieveModel.Level; }
            set
            {
                if (value == _achieveModel.Level)
                    return;

                _achieveModel.Level = value;
                base.RaisePropertyChanged("Level");
            }
        }

        public string Rank
        {
            get { return _achieveModel.Rank; }
            set
            {
                if (value == _achieveModel.Rank)
                    return;

                _achieveModel.Rank = value;
                base.RaisePropertyChanged("Rank");
            }
        }

        public string Total
        {
            get { return _achieveModel.Total; }
            set
            {
                if (value == _achieveModel.Total)
                    return;

                _achieveModel.Total = value;
                base.RaisePropertyChanged("Total");
            }
        }

        public string Single
        {
            get { return _achieveModel.Single; }
            set
            {
                if (value == _achieveModel.Single)
                    return;

                _achieveModel.Single = value;
                base.RaisePropertyChanged("Single");
            }
        }

        #endregion
    }
}
