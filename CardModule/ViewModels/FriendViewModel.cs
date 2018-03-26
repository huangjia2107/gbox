using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using CardModule.Models;
using Microsoft.Practices.Prism.ViewModel;

namespace CardModule.ViewModels
{
    public class FriendViewModel : NotificationObject
    {
        #region 变量

        readonly FriendModel _friendModel;

        #endregion

        #region 构造函数

        public FriendViewModel(FriendModel friendModel)
        {
            _friendModel = friendModel;
        }

        #endregion

        #region 绑定的属性

        public string FriendID
        {
            get { return _friendModel.FriendID; }
            set
            {
                if (value == _friendModel.FriendID)
                    return;

                _friendModel.FriendID = value;
                base.RaisePropertyChanged("FriendID");
            }
        }

        public string FriendCard
        {
            get { return _friendModel.FriendCard; }
            set
            {
                if (value == _friendModel.FriendCard)
                    return;

                _friendModel.FriendCard = value;
                base.RaisePropertyChanged("FriendCard");
            }
        }

        public string FriendName
        {
            get { return _friendModel.FriendName; }
            set
            {
                if (value == _friendModel.FriendName)
                    return;

                _friendModel.FriendName = value;
                base.RaisePropertyChanged("FriendName");
            }
        }

        public string FriendImg
        {
            get { return _friendModel.FriendImg; }
            set
            {
                if (value == _friendModel.FriendImg)
                    return;

                _friendModel.FriendImg = value;
                base.RaisePropertyChanged("FriendImg");
            }
        }

        public string FriendGroup
        {
            get { return _friendModel.FriendGroup; }
            set
            {
                if (value == _friendModel.FriendGroup)
                    return;

                _friendModel.FriendGroup = value;
                base.RaisePropertyChanged("FriendGroup");
            }
        }

        public bool FriendStatus
        {
            get { return _friendModel.FriendStatus; }
            set
            {
                if (value == _friendModel.FriendStatus)
                    return;

                _friendModel.FriendStatus = value;
                base.RaisePropertyChanged("FriendStatus");
            }
        }

        public bool MenuIsEnabled
        {
            get { return _friendModel.MenuIsEnabled; }
            set
            {
                if (value == _friendModel.MenuIsEnabled)
                    return;

                _friendModel.MenuIsEnabled = value;
                base.RaisePropertyChanged("MenuIsEnabled");
            }
        }

        #endregion
    }
}
