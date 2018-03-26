using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Prism.ViewModel;
using CardModule.Models;

namespace CardModule.ViewModels
{
    public class MsgViewModel : NotificationObject
    {
        #region 变量

        readonly MsgModel _msgModel;

        #endregion

        #region 构造函数

        public MsgViewModel(MsgModel msgModel)
        {
            _msgModel = msgModel;
        }

        #endregion

        #region 绑定的属性

        public string MsgInfo
        {
            get { return _msgModel.MsgInfo; }
            set
            {
                if (value == _msgModel.MsgInfo)
                    return;

                _msgModel.MsgInfo = value;
                base.RaisePropertyChanged("MsgInfo");
            }
        } 

        public string PathData
        {
            get { return _msgModel.PathData; }
            set
            {
                if (value == _msgModel.PathData)
                    return;

                _msgModel.PathData = value;
                base.RaisePropertyChanged("PathData");
            }
        }

        public string MsgType
        {
            get { return _msgModel.MsgType; }
            set
            {
                if (value == _msgModel.MsgType)
                    return;

                _msgModel.MsgType = value;
                base.RaisePropertyChanged("MsgType");
            }
        }

        public string MsgTitle
        {
            get { return _msgModel.MsgTitle; }
            set
            {
                if (value == _msgModel.MsgTitle)
                    return;

                _msgModel.MsgTitle = value;
                base.RaisePropertyChanged("MsgTitle");
            }
        }

        public string MsgContent
        {
            get { return _msgModel.MsgContent; }
            set
            {
                if (value == _msgModel.MsgContent)
                    return;

                _msgModel.MsgContent = value;
                base.RaisePropertyChanged("MsgContent");
            }
        }

        public string MsgPublish
        {
            get { return _msgModel.MsgPublish; }
            set
            {
                if (value == _msgModel.MsgPublish)
                    return;

                _msgModel.MsgPublish = value;
                base.RaisePropertyChanged("MsgPublish");
            }
        }

        public string SecondRow
        {
            get { return _msgModel.SecondRow; }
            set
            {
                if (value == _msgModel.SecondRow)
                    return;

                _msgModel.SecondRow = value;
                base.RaisePropertyChanged("SecondRow");
            }
        }

        public string ThirdRow
        {
            get { return _msgModel.ThirdRow; }
            set
            {
                if (value == _msgModel.ThirdRow)
                    return;

                _msgModel.ThirdRow = value;
                base.RaisePropertyChanged("ThirdRow");
            }
        }

        #endregion
    }
}
