using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Prism.Events;

namespace MessageModule.SendMsg
{
    /// <summary>
    /// 用于将各模块要发送的信息广播到主窗口后台，集中发向服务器
    /// </summary>
    public class SendMsgEvent : CompositePresentationEvent<string>
    {
    }
}
