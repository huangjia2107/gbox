using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Models
{
    public class FriendInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 用户1ID
        /// </summary>
        public int OneID { get; set; }

        /// <summary>
        /// 用户1组ID
        /// </summary>
        public int OneGroup { get; set; }

        /// <summary>
        /// 用户2ID
        /// </summary>
        public int OtherID { get; set; }

        /// <summary>
        /// 用户2组ID
        /// </summary>
        public int OtherGroup { get; set; }

        
    }
}
