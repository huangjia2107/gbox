using LoginModule.Models;
using LoginModule.DataAccess;
using Microsoft.Practices.Prism.ViewModel;

namespace LoginModule.ViewModels
{
    public class UserViewModel : NotificationObject
    {
        #region 变量

        readonly UserModel _userModel;
        readonly UserAccess _userAccess;

        #endregion

        #region 构造函数

        public UserViewModel(UserModel userModel, UserAccess userAccess)
        {
            _userModel =userModel;
            _userAccess = userAccess;
        }

        #endregion

        #region 绑定的属性

        public string ImageUrl
        {
            get { return _userModel.ImageUrl; }
            set
            {
                if (value == _userModel.ImageUrl)
                    return;

                _userModel.ImageUrl = value;

                base.RaisePropertyChanged("ImageUrl");
            }
        }

        public string CardWord
        {
            get { return _userModel.CardWord; }
            set
            {
                if (value == _userModel.CardWord)
                    return;

                _userModel.CardWord = value;

                base.RaisePropertyChanged("CardWord");
            }
        }

        public string UserName
        {
            get { return _userModel.UserName; }
            set
            {
                if (value == _userModel.UserName)
                    return;

                _userModel.UserName = value;

                base.RaisePropertyChanged("UserName");
            }
        }

        public string Password
        {
            get { return _userModel.Password; }
            set
            {
                if (value == _userModel.Password)
                    return;

                _userModel.Password = value;

                base.RaisePropertyChanged("Password");
            }
        }

        public bool IsRemPass
        {
            get { return _userModel.IsRemPass; }
            set
            {
                if (value == _userModel.IsRemPass)
                    return;

                _userModel.IsRemPass = value;

                base.RaisePropertyChanged("IsRemPass");
            }
        }

        public System.DateTime NowTime
        {
            get { return _userModel.NowTime; }
            set
            {
                if (value == _userModel.NowTime)
                    return;

                _userModel.NowTime = value;

                base.RaisePropertyChanged("NowTime");
            }
        }

        #endregion
    }
}
