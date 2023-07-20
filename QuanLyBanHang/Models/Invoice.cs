using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuanLyBanHang.Models
{
    [Serializable]
    public class InvoiceDetail
    {
        public HangHoa Product { get; set; }
        public int Quantity { get; set; }
  
    }


}