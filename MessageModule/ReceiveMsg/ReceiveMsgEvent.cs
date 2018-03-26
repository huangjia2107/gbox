using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Prism.Events;

namespace MessageModule.ReceiveMsg
{
    /// <summary>
    /// 用于将服务器发过来的信息广播给各个模块
    /// </summary>
    public class ReceiveMsgEvent:CompositePresentationEvent<ReceiveMsgOrder>
    {

    }
}
