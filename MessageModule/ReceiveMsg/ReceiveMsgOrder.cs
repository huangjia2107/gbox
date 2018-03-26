using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageModule.ReceiveMsg
{
    public class ReceiveMsgOrder
    {
        /// <summary>
        /// 模块类型：根据不同的模块标志来选择接收属于自己模块的相应的信息，用作区别信息标志
        /// </summary>
        public string ModuleType { get; set; }

        /// <summary>
        /// 仅限于登录模块
        /// Sign=0 登录未成功.显示相应错误信息
        /// Sign=1 登陆成功,直接跳转窗体
        /// </summary>
        public int Sign { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string MsgContent { get; set; }
    }
}
