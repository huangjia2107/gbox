using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Prism.ViewModel;
using CardModule.Models;

namespace CardModule.ViewModels
{
    public class ResultViewModel: NotificationObject
    {
        #region 变量

        readonly ResultModel _resultModel;

        #endregion

        #region 构造函数

        public ResultViewModel(ResultModel resultModel)
        {
            _resultModel = resultModel;
        }

        #endregion

        #region 绑定的属性

        public string ResultID
        {
            get { return _resultModel.ResultID; }
            set
            {
                if (value == _resultModel.ResultID)
                    return;

                _resultModel.ResultID = value;
                base.RaisePropertyChanged("ResultID");
            }
        }

        public string ResultCard
        {
            get { return _resultModel.ResultCard; }
            set
            {
                if (value == _resultModel.ResultCard)
                    return;

                _resultModel.ResultCard = value;
                base.RaisePropertyChanged("ResultCard");
            }
        }

        public string ResultName
        {
            get { return _resultModel.ResultName; }
            set
            {
                if (value == _resultModel.ResultName)
                    return;

                _resultModel.ResultName = value;
                base.RaisePropertyChanged("ResultName");
            }
        }

        public string ResultImg
        {
            get { return _resultModel.ResultImg; }
            set
            {
                if (value == _resultModel.ResultImg)
                    return;

                _resultModel.ResultImg = value;
                base.RaisePropertyChanged("ResultImg");
            }
        }

        public bool ResultIsCanAdd
        {
            get { return _resultModel.ResultIsCanAdd; }
            set
            {
                if (value == _resultModel.ResultIsCanAdd)
                    return;

                _resultModel.ResultIsCanAdd = value;
                base.RaisePropertyChanged("ResultIsCanAdd");
            }
        }

        public bool ResultIsOnline
        {
            get { return _resultModel.ResultIsOnline; }
            set
            {
                if (value == _resultModel.ResultIsOnline)
                    return;

                _resultModel.ResultIsOnline = value;
                base.RaisePropertyChanged("ResultIsOnline");
            }
        } 

        #endregion
    }
}
