using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Models
{
    [Serializable]
    public class UserInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string CardWord { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string UserPicture { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string UserMail { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime RegTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string UserStatus { get; set; }
    }
}
