using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Models
{
    [Serializable]
    public class Friend
    {
        public int UserID { get; set; }

        public int UserGroup { get; set; }
    }
}
