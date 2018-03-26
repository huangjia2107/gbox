using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Models
{
    [Serializable]
    public class GameInfo
    {
        public int UserID { get; set; }

        public string GameName { get; set; }

        public string GameTable { get; set; }
    }
}
