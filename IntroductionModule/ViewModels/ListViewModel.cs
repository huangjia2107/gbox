using Microsoft.Practices.Prism.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntroductionModule
{
    public class ListViewModel : NotificationObject
    {
        #region 变量

        readonly ListModel _listModel;

        #endregion

        #region 构造函数

        public ListViewModel(ListModel listModel)
        {
            _listModel = listModel;
        }

        #endregion

        #region 绑定的属性 

        public string ID
        {
            get { return _listModel.ID; }
            set
            {
                if (value == _listModel.ID)
                    return;

                _listModel.ID = value;
                base.RaisePropertyChanged("ID");
            }
        }

        public string GameName
        {
            get { return _listModel.GameName; }
            set
            {
                if (value == _listModel.GameName)
                    return;

                _listModel.GameName = value;
                base.RaisePropertyChanged("GameName");
            }
        }

        public string DownName
        {
            get { return _listModel.DownName; }
            set
            {
                if (value == _listModel.DownName)
                    return;

                _listModel.DownName = value;
                base.RaisePropertyChanged("DownName");
            }
        }

        public string DownSize
        {
            get { return _listModel.DownSize; }
            set
            {
                if (value == _listModel.DownSize)
                    return;

                _listModel.DownSize = value;
                base.RaisePropertyChanged("DownSize");
            }
        }

        public string DownProgress
        {
            get { return _listModel.DownProgress; }
            set
            {
                if (value == _listModel.DownProgress)
                    return;

                _listModel.DownProgress = value;
                base.RaisePropertyChanged("DownProgress");
            }
        }

        public string DownSpeed
        {
            get { return _listModel.DownSpeed; }
            set
            {
                if (value == _listModel.DownSpeed)
                    return;

                _listModel.DownSpeed = value;
                base.RaisePropertyChanged("DownSpeed");
            }
        }

        #endregion
    }
}
