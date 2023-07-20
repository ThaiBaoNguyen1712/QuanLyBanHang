using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyBanHang.common
{
    [Serializable]
    public class UserLogin
    {
        public string UserName { set; get; }
        public string GroupID { set; get; }
    }
}