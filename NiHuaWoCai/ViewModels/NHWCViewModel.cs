using MessageModule.RegionTypes;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace nihuawocai.ViewModels
{
    [Export]
    public class NHWCViewModel
    {
        #region 变量

        IRegionManager regionManager;
        string[] UserMsg = null;

        #endregion

        #region 构造函数

        [ImportingConstructor]
        public NHWCViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            //获得用户信息
            UserMsg = this.regionManager.Regions[RegionTypes.CardRegion].Context as string[];
        }

        #endregion

        #region 绑定的属性

        public string UserID
        {
            get
            {
                return UserMsg == null ? "" : UserMsg[0];
            }
        }

        public string CardWord
        {
            get 
            {
                return UserMsg == null ? "" : UserMsg[1];
            }
        }

        public string UserName
        {
            get
            {
                return UserMsg == null ? "" : UserMsg[2];
            }
        }

        public string UserImg
        {
            get
            {
                return UserMsg == null ? "" : UserMsg[3];
            }
        }

        #endregion
    }
}
