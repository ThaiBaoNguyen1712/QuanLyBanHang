using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyBanHang.Models;
using QuanLyBanHang.common;
using System.Globalization;

namespace QuanLyBanHang.Controllers
{
    public class PaymentController :BaseController
    {
        // GET: Payment
        QL_BanLeEntities db = new QL_BanLeEntities();
        private const string CartSession = "CartSession";
        public  ActionResult Index()
        {

            int? idUser = Session["UserId"] as int?;
        

            if (idUser.HasValue && idUser.Value != 0)
            {

              
            var InforCus = new List<KhachHang> { db.KhachHang.FirstOrDefault(x => x.UserID == idUser) };
            ViewBag.InforCus = InforCus;

            var cart = Session[CartSession];
            var list = new List<CartItem>();
            if (cart != null)
            {
                list = (List<CartItem>)cart;

            }
            return View(list);
            }
            return RedirectToAction("Index", "CreateCus");
        }
       

        [HttpPost]
        public ActionResult Payment( string shipName, string mobile, string address, string email)
        {
            var order = new HoaDon();
            order.CreatedDate = DateTime.Now;
            order.ID_KhachHang = getIDCus();
            order.ShipAddress = address;
            order.ShipMobile = mobile;
            order.ShipName = shipName;
            order.ShipEmail = email;
            order.TrangThai = "Chờ xác nhận";

            try
            {
                //Thêm Order
                db.HoaDon.Add(order);
                db.SaveChanges();
                var id = order.ID;

                var cart = (List<CartItem>)Session[CartSession];

                decimal total = 0;
                foreach (var item in cart)
                {
                    var orderDetail = new ChiTietHoaDon();
                    orderDetail.ID_SP = item.product.ID;
                    orderDetail.ID_HD = id;
                    orderDetail.Gia = item.product.GiaBan;
                    orderDetail.SoLuong = item.Quantity;
                    db.ChiTietHoaDon.Add(orderDetail);
                    db.SaveChanges();
                    total += (item.product.GiaBan.GetValueOrDefault(0) * item.Quantity);
                    
                }
                ViewBag.Total = total;
                //send mail cho khách hàng
                var strSanPham = "";
                foreach(var sp in cart)
                {
                    strSanPham += "<tr>";
                    strSanPham += "<td>"+ sp.product.TenHang+"</td>";
                    strSanPham += "<td>" + sp.product.SoLuong + "</td>";
                    strSanPham += "<td>" + ((decimal)sp.product.GiaBan).ToString("N0") + "</td>";
                    strSanPham += "</tr>";
                }
                string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send2.html"));
                contentCustomer = contentCustomer.Replace("{{MaDon}}", order.ID.ToString());
                contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", shipName);
                contentCustomer = contentCustomer.Replace("{{Phone}}", mobile);
                contentCustomer = contentCustomer.Replace("{{Email}}", email);
                contentCustomer = contentCustomer.Replace("{{Address}}", address);
                contentCustomer = contentCustomer.Replace("{{TongTien}}", total.ToString("N0"));
                QuanLyBanHang.common.SendEmailCus.SendEmail("ShopOnline", "Đơn hàng" + order.ID,contentCustomer.ToString(), email);
              
             
                Session[CartSession] = null;
                return RedirectToAction("Success", "Payment");
            }
            catch (Exception ex)
            {
                //ghi log
                return Redirect("/Payment/UnSuccess");
            }
          
        }
        
        public int getIDCus()
        {
            int userID =(int)Session["UserID"];
            var khachhang = db.KhachHang.FirstOrDefault(x => x.UserID == userID);
            if(khachhang !=null)
            {
                int IDCus = khachhang.ID;
                return IDCus;
            }
            return 0;
        }
        public ActionResult Success()
        {
            return View();
        }
        public ActionResult UnSuccess()
        {
            return View();
        }

    }
}