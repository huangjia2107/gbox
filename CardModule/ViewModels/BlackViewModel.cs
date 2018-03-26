using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Prism.ViewModel;
using CardModule.Models;

namespace CardModule.ViewModels
{
    public class BlackViewModel : NotificationObject
    {
        #region 变量

        readonly BlackModel _blackModel;

        #endregion

        #region 构造函数

        public BlackViewModel(BlackModel blackModel)
        {
            _blackModel = blackModel;
        }

        #endregion

        #region 绑定的属性

        public string FriendID
        {
            get { return _blackModel.FriendID; }
            set
            {
                if (value == _blackModel.FriendID)
                    return;

                _blackModel.FriendID = value;
                base.RaisePropertyChanged("FriendID");
            }
        }

        public string FriendCard
        {
            get { return _blackModel.FriendCard; }
            set
            {
                if (value == _blackModel.FriendCard)
                    return;

                _blackModel.FriendCard = value;
                base.RaisePropertyChanged("FriendCard");
            }
        }

        public string FriendName
        {
            get { return _blackModel.FriendName; }
            set
            {
                if (value == _blackModel.FriendName)
                    return;

                _blackModel.FriendName = value;
                base.RaisePropertyChanged("FriendName");
            }
        }

        public string FriendImg
        {
            get { return _blackModel.FriendImg; }
            set
            {
                if (value == _blackModel.FriendImg)
                    return;

                _blackModel.FriendImg = value;
                base.RaisePropertyChanged("FriendImg");
            }
        }

        public string FriendGroup
        {
            get { return _blackModel.FriendGroup; }
            set
            {
                if (value == _blackModel.FriendGroup)
                    return;

                _blackModel.FriendGroup = value;
                base.RaisePropertyChanged("FriendGroup");
            }
        }

        public bool FriendStatus
        {
            get { return _blackModel.FriendStatus; }
            set
            {
                if (value == _blackModel.FriendStatus)
                    return;

                _blackModel.FriendStatus = value;
                base.RaisePropertyChanged("FriendStatus");
            }
        }

        public bool MenuIsEnabled
        {
            get { return _blackModel.MenuIsEnabled; }
            set
            {
                if (value == _blackModel.MenuIsEnabled)
                    return;

                _blackModel.MenuIsEnabled = value;
                base.RaisePropertyChanged("MenuIsEnabled");
            }
        }

        #endregion
    }
}
