using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyBanHang.Models;
using System.Globalization;


namespace QuanLyBanHang.Areas.Admin.Controllers
{
    public class HomeAdminController : BaseloginController
    {
        // GET: Admin/HomeAdmin
        QL_BanLeEntities db = new QL_BanLeEntities();
        public ActionResult Index()
        {
            var SumInvoice = db.HoaDon.Count();
            //var SumCustomer = db.KhachHang.Count();
            var SumProduct = db.HangHoa.Count();
            var ListProduct = db.HangHoa.Where(s => s.SoLuong < 5).Select(s => s.TenHang);
            var Invoiceneed = db.HoaDon.Where(s => s.TrangThai == "Chờ xác nhận").Count();
            var SumAccount = db.User.Count();
            var ViewCount = db.View.Where(x => x.ID == 1).Select(x => x.Seen).FirstOrDefault();
            //Lấy top 3 sản phẩm bán chạy nhất bằng cách đếm tổng số lượng trong bảng CTHD vả lấy tên của 
            //bảng HangHoa
            var HotProduct = db.ChiTietHoaDon
                    .Join(db.HangHoa, cthd => cthd.ID_SP, hh => hh.ID, (cthd, hh) => new { cthd, hh })
                    .GroupBy(x => x.hh.TenHang)
                    .Select(g => new { TenHang = g.Key, TotalQuantity = g.Sum(x => x.cthd.SoLuong) })
                    .OrderByDescending(x => x.TotalQuantity)
                    .Take(3)
                    .Select(x => x.TenHang)
                    .ToList();


            ViewBag.SumInvoice = SumInvoice;
           // ViewBag.SumCustomer = SumCustomer;
            ViewBag.SumProduct = SumProduct;
            ViewBag.ListProductneed = ListProduct.ToList();
            ViewBag.Invoiceneed = Invoiceneed;
            ViewBag.SumAccount = SumAccount;
            ViewBag.SumRevenue = Tongdoanhthu();
            ViewBag.PageView = ViewCount;//Lấy số lượng người truy cập từ Application đã được tạo
            ViewBag.Online = HttpContext.Application["Online"].ToString();//Lấy số lượng người Online từ Application đã được tạo
            ViewBag.HotSale = HotProduct.ToList();
            return View();
        }
        public string Tongdoanhthu()
        {
            var Hd = db.HoaDon.Where(x => x.TrangThai == "Thành công").Select(x => x.ID);
            decimal sumRevenue = (decimal)db.ChiTietHoaDon.Where(x => Hd.Contains(x.ID_HD)).Sum(x => x.SoLuong * x.Gia);
            

            CultureInfo cultureInfo = new CultureInfo("en-US");
            string formattedRevenue = sumRevenue.ToString("N", cultureInfo);

            return formattedRevenue;
        }

        public ActionResult Chude()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult HoTro()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}