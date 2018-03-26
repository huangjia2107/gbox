using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Practices.Prism.Events;

namespace MessageModule.ModuleMsg
{
    /// <summary>
    /// 用于 界面切换命令、游戏ID 的传输
    /// </summary>
    public class ModuleMsgEvent : CompositePresentationEvent<ModuleMsgOrder>
    {

    }
}
