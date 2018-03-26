using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Prism.ViewModel;
using CardModule.Models;

namespace CardModule.ViewModels
{
    public class FamilyViewModel : NotificationObject
    {
        #region 变量

        readonly FamilyModel _familyModel;

        #endregion

        #region 构造函数

        public FamilyViewModel(FamilyModel familyModel)
        {
            _familyModel = familyModel;
        }

        #endregion

        #region 绑定的属性

        public string FriendID
        {
            get { return _familyModel.FriendID; }
            set
            {
                if (value == _familyModel.FriendID)
                    return;

                _familyModel.FriendID = value;
                base.RaisePropertyChanged("FriendID");
            }
        }

        public string FriendCard
        {
            get { return _familyModel.FriendCard; }
            set
            {
                if (value == _familyModel.FriendCard)
                    return;

                _familyModel.FriendCard = value;
                base.RaisePropertyChanged("FriendCard");
            }
        }

        public string FriendName
        {
            get { return _familyModel.FriendName; }
            set
            {
                if (value == _familyModel.FriendName)
                    return;

                _familyModel.FriendName = value;
                base.RaisePropertyChanged("FriendName");
            }
        }

        public string FriendImg
        {
            get { return _familyModel.FriendImg; }
            set
            {
                if (value == _familyModel.FriendImg)
                    return;

                _familyModel.FriendImg = value;
                base.RaisePropertyChanged("FriendImg");
            }
        }

        public string FriendGroup
        {
            get { return _familyModel.FriendGroup; }
            set
            {
                if (value == _familyModel.FriendGroup)
                    return;

                _familyModel.FriendGroup = value;
                base.RaisePropertyChanged("FriendGroup");
            }
        }

        public bool FriendStatus
        {
            get { return _familyModel.FriendStatus; }
            set
            {
                if (value == _familyModel.FriendStatus)
                    return;

                _familyModel.FriendStatus = value;
                base.RaisePropertyChanged("FriendStatus");
            }
        }

        public bool MenuIsEnabled
        {
            get { return _familyModel.MenuIsEnabled; }
            set
            {
                if (value == _familyModel.MenuIsEnabled)
                    return;

                _familyModel.MenuIsEnabled = value;
                base.RaisePropertyChanged("MenuIsEnabled");
            }
        }

        #endregion
    }
}
