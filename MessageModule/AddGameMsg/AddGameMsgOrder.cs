using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageModule.AddGameMsg
{
    public class AddGameMsgOrder
    {
        /// <summary>
        /// 游戏唯一性标志ID
        /// </summary>
        public string GameId { get; set; }

        /// <summary>
        /// 游戏名称
        /// </summary>
        public string GameName { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public string GameTime { get; set; }
    }
}
