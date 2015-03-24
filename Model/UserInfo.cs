using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class UserInfo
    {
        [Primary]
        [Identity]
        public long Id { get; set; }
        
        public string Name { get; set; }
        
    }
}
