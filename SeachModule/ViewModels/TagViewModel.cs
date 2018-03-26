using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SeachModule.Models;
using SeachModule.DataAccess;
using Microsoft.Practices.Prism.ViewModel;

namespace SeachModule.ViewModels
{
    public class TagViewModel : NotificationObject
    {
        #region 变量

        readonly TagModel _tagModel;
        readonly TagAccess _tagAccess;

        #endregion

        #region 构造函数

        public TagViewModel(TagModel tagModel, TagAccess tagAccess)
        {
            _tagModel =tagModel;
            _tagAccess = tagAccess;
        }

        #endregion

        #region 绑定的属性

        public string ID
        {
            get { return _tagModel.ID; }
            set
            {
                if (value == _tagModel.ID)
                    return;

                _tagModel.ID = value;

                base.RaisePropertyChanged("ID");
            }
        }

        public string Title
        {
            get { return _tagModel.Title; }
            set
            {
                if (value == _tagModel.Title)
                    return;

                _tagModel.Title = value;

                base.RaisePropertyChanged("Title");
            }
        }

        public string FontColor
        {
            get { return _tagModel.FontColor; }
            set
            {
                if (value == _tagModel.FontColor)
                    return;

                _tagModel.FontColor = value;

                base.RaisePropertyChanged("FontColor");
            }
        }

        public double Size
        {
            get { return _tagModel.Size; }
            set
            {
                if (value == _tagModel.Size)
                    return;

                _tagModel.Size = value;

                base.RaisePropertyChanged("Size");
            }
        }

        #endregion
    }
}
