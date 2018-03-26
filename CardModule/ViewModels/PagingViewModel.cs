using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Prism.ViewModel;
using CardModule.Models;

namespace CardModule.ViewModels
{
    public class PagingViewModel : NotificationObject
    {
        #region 变量

        public int pagecount = 3; //单页记录数

        #endregion

        #region 构造函数

        /// <summary>
        ///  带参构造
        /// </summary>
        /// <param name="pagecount">单页记录数</param>
        public PagingViewModel(int pagecount)
        {
            this.pagecount = pagecount;
        }

        /// <summary>
        /// 无参构造
        /// </summary>
        public PagingViewModel() { }

        #endregion

        #region 绑定的属性

        private string _sumAndToltal;  //...单页记录数/总记录数
        private string _pageAndTotal;  //...当前页/总页数
        private int _currentPage; //...当前页
        private int _toltalPage; //...总页数

        public string SumAndToltal
        {
            get { return _sumAndToltal; }
            set
            {
                if (value == _sumAndToltal)
                    return;

                _sumAndToltal = value;
                base.RaisePropertyChanged("SumAndToltal");
            }
        }

        public string PageAndToltal
        {
            get { return _pageAndTotal; }
            set
            {
                if (value == _pageAndTotal)
                    return;

                _pageAndTotal = value;
                base.RaisePropertyChanged("PageAndToltal");
            }
        }

        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (value == _currentPage)
                    return;

                _currentPage = value;
                base.RaisePropertyChanged("CurrentPage");
            }
        }

        public int ToltalPage
        {
            get { return _toltalPage; }
            set
            {
                if (value == _toltalPage)
                    return;

                _toltalPage = value;
                base.RaisePropertyChanged("ToltalPage");
            }
        }

        #endregion
    }
}
