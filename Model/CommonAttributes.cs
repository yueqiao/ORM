using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    /// <summary>
    /// 不能为空
    /// </summary>
    public class Required : Attribute
    {
        public bool IsRequired { get; private set; }

        public Required()
        {
            this.IsRequired = true;
        }
    }

    /// <summary>
    /// 主键
    /// </summary>
    public class Primary : Attribute
    {
        public bool IsPrimary { get; private set; }

        public Primary()
        {
            this.IsPrimary = true;
        }
    }

    /// <summary>
    /// 标识列
    /// </summary>
    public class Identity : Attribute
    {
        public bool IsIdentity { get; private set; }

        public Identity()
        {
            this.IsIdentity = true;
        }
    }
}