using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyBanHang.Models
{
    [Serializable]
    public class CartItem
    {
        public HangHoa product { set; get; }
        public int Quantity { set; get; }

    }
}