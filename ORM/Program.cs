using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data;
using BLL;

namespace ORM
{
    internal class Program
    {
        private static bool Required { get; set; }

        private static void Main(string[] args)
        {
            var bll = new UserInfoBll();
            bll.Insert();
        }
    }
}