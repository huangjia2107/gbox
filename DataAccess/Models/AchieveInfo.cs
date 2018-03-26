using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Models
{
    [Serializable]
    public class AchieveInfo
    {
        public string UserLevel { get; set; }
        public string UserRank { get; set; }
        public string UserAllScore { get; set; }
        public string UserSingleScore { get; set; }
    }
}
