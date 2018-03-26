using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Prism.Events;

namespace MessageModule.AddGameMsg
{
    /// <summary>
    /// 用于游戏添加  传输游戏ID（下载模块-->我的游戏模块）
    /// </summary>
    public class AddGameMsgEvent : CompositePresentationEvent<AddGameMsgOrder>
    {
    }
}
