using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageModule.ModuleMsg
{
    public class ModuleMsgOrder
    {
        /// <summary>
        /// Sidn=0 发送ID/界面切换到“介绍”界面
        /// Sign=1 界面切换到“原”界面
        /// </summary>
        public int Sign { get; set; }

        /// <summary>
        /// 游戏唯一性标志ID
        /// </summary>
        public string GameId { get; set; }

        /// <summary>
        /// 游戏名称
        /// </summary>
        public string GameName { get; set; }
    }
}
