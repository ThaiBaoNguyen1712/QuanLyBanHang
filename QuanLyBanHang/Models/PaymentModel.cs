using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuanLyBanHang.Models;

namespace QuanLyBanHang.common
{
    public class PaymentModel
    {

        public List<CartItem> CartItems { get; set; }
        public List<KhachHang> CustomerList { get; set; }
    }
}