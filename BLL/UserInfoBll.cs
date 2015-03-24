using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using Model;

namespace BLL
{
    public class UserInfoBll
    {
        private readonly CommonDal<UserInfo> _dal = new CommonDal<UserInfo>();

        public int Insert()
        {
            return _dal.Insert(new UserInfo());
        }
    }
}
