using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyBanHang.Models;
using System.Configuration;
using System.IO;
using System.Web.Script.Serialization;
using System.Data.Entity;
using System.Globalization;

namespace QuanLyBanHang.Areas.Admin.Controllers
{
    public class BanHangController : BaseloginController
    {
        // GET: Admin/BanHang
        QL_BanLeEntities db = new QL_BanLeEntities();
        private const string IDetailSession = "IDetailSession";

        public ActionResult Index()
        {
            var ListProducts = from lc in db.HangHoa select lc;
            ViewBag.Products = ListProducts;
            var Idetail = Session[IDetailSession];

            var list = new List<InvoiceDetail>();

            if (Idetail != null)
            {
                list = (List<InvoiceDetail>)Idetail;
            }
            return View(list);
        }
        [HttpPost]
        public ActionResult CreateClient(KhachHang kh, HttpPostedFileBase HinhAnh)
        {
            SetViewBag();
            db.KhachHang.Add(kh);
            db.SaveChanges();
            if (HinhAnh != null && HinhAnh.ContentLength > 0)
            {
                int id = int.Parse(db.KhachHang.ToList().Last().ID.ToString());

                string _FileName = "";
                int Index = HinhAnh.FileName.IndexOf(".");
                _FileName = "KH" + id.ToString() + "." + HinhAnh.FileName.Substring(Index + 1);
                string _path = Path.Combine(Server.MapPath("~/Upload/Client"), _FileName);
                HinhAnh.SaveAs(_path);

                KhachHang uhh = db.KhachHang.FirstOrDefault(x => x.ID == id);
                uhh.HinhAnh = _FileName;
                db.SaveChanges();
            }
            else
            {
                kh.HinhAnh = "none.jpg";
                db.Entry(kh).State = EntityState.Modified;
                db.SaveChanges();
            }


            return RedirectToAction("Payment");
        }

        public void SetViewBag(int? selectedid = null)
        {
            ViewBag.NhomKH = new SelectList(db.NhomKhachHang.ToList(), "ID", "TenNhomKH", selectedid);
        }

        public void SetViewBagNameCustomer(int? selectedid = null)
        {
            ViewBag.TenKhachhang = new SelectList(db.KhachHang.ToList(), "ID", "TenKhachHang", selectedid);
        }

        public ActionResult AddItem(int productID,int quantity)
        {
            var product = db.HangHoa.FirstOrDefault(c => c.ID == productID);
            var Idetail = Session[IDetailSession];
            if (Idetail != null)
            {
                var list = (List<InvoiceDetail>)Idetail;
                if (list.Exists(x => x.Product.ID == productID))
                {

                    foreach (var item in list)
                    {
                        if (item.Product.ID == productID)
                        {
                            item.Quantity += quantity;
                        }
                    }
                }
                else
                {
                    //tạo mới đối tượng cart item
                    var item = new InvoiceDetail();
                    item.Product = product;
                    item.Quantity = quantity;
                    list.Add(item);
                }
                //Gán vào session
                Session[IDetailSession] = list;
            }
            else
            {
                //tạo mới đối tượng cart item
                var item = new InvoiceDetail();
                item.Product = product;
                item.Quantity = quantity;
                var list = new List<InvoiceDetail>();
                list.Add(item);
                //Gán vào session
                Session[IDetailSession] = list;
            }
            return RedirectToAction("Index");
        }
        public ActionResult Product()
        {
            var ListProducts = from lc in db.HangHoa select lc;
            ViewBag.Products = ListProducts;
            return View();
        }
        [HttpGet]
        public ActionResult Payment()
        {
            SetViewBag();
            SetViewBagNameCustomer();
            var ListClients = from lc in db.KhachHang select lc;
            ViewBag.Clients = ListClients;
            var Idetail = Session[IDetailSession];
            var list = new List<InvoiceDetail>();
            if (Idetail != null)
            {
                list = (List<InvoiceDetail>)Idetail;
            }
            return View(list);
        }

        [HttpPost]
        public ActionResult Payment(HoaDon hd, string shipName, string mobile, string address, string otherEmail)
        {
           hd.CreatedDate = DateTime.Now;
            hd.ShipAddress = address;
            hd.ShipMobile = mobile;
            hd.ShipName = shipName;
            hd.ShipEmail = otherEmail;
            try
            {

                hd.CreatedDate = DateTime.Now;
                hd.TrangThai = "Thành công";
                if(hd.ID_KhachHang==null)
                {
                    return RedirectToAction("UnSuccess");
                }
                if (enoughQuantity())
                {   //Thêm Order   
                    db.HoaDon.Add(hd);
                    db.SaveChanges();
                }
                else
                {
                    return RedirectToAction("Error");
                }
                         
                
                var id = hd.ID;

                var cart = (List<InvoiceDetail>)Session[IDetailSession];

                decimal total = 0;
                foreach (var item in cart)
                {

                    //Khi hóa đơn được tạo thành công đồng nghĩa với việc sẽ giảm bớt số lượng hàng hóa trong kho đi
                    var hangHoa = db.HangHoa.FirstOrDefault(h => h.ID == item.Product.ID);
                  
                     hangHoa.SoLuong -= item.Quantity;       
                    var orderDetail = new ChiTietHoaDon();
                    orderDetail.ID_SP = item.Product.ID;
                    orderDetail.ID_HD = id;
                    orderDetail.Gia = item.Product.GiaBan;
                    orderDetail.SoLuong = item.Quantity;
                    db.ChiTietHoaDon.Add(orderDetail);
                    db.SaveChanges();
                    total += (item.Product.GiaBan.GetValueOrDefault(0) * item.Quantity);

                }
                ViewBag.Total = total;
                //send mail cho khách hàng
                var strSanPham = "";
                foreach (var sp in cart)
                {
                    strSanPham += "<tr>";
                    strSanPham += "<td>" + sp.Product.TenHang + "</td>";
                    strSanPham += "<td>" + sp.Product.SoLuong + "</td>";
                    strSanPham += "<td>" + ((decimal)sp.Product.GiaBan).ToString("N0") + "</td>";
                    strSanPham += "</tr>";
                }
                string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/templates/send1.html"));
                contentCustomer = contentCustomer.Replace("{{MaDon}}", hd.ID.ToString());
                contentCustomer = contentCustomer.Replace("{{SanPham}}", strSanPham);
                contentCustomer = contentCustomer.Replace("{{NgayDat}}", DateTime.Now.ToString("dd/MM/yyyy"));
                contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", shipName);
                contentCustomer = contentCustomer.Replace("{{Phone}}", mobile);
                contentCustomer = contentCustomer.Replace("{{Email}}", otherEmail);
                contentCustomer = contentCustomer.Replace("{{Address}}", address);
                contentCustomer = contentCustomer.Replace("{{TongTien}}", total.ToString("N0"));
                QuanLyBanHang.common.SendEmailCus.SendEmail("ShopOnline", "Đơn hàng" + hd.ID, contentCustomer.ToString(), otherEmail);

                Session[IDetailSession] = null;
                return RedirectToAction("OrderDetails", "BanHang", new { id = id });
            }
            catch (Exception ex)
            {
                //ghi log
                return Redirect("/Admin/BanHang/UnSuccess");
            }
        }

        public bool enoughQuantity()
        {
            var cart = (List<InvoiceDetail>)Session[IDetailSession];
            foreach (var item in cart)
            {
                var hangHoa = db.HangHoa.FirstOrDefault(h => h.ID == item.Product.ID);
                if (hangHoa != null && hangHoa.SoLuong >= item.Quantity)
                {
                    return true;

                }
            }
            return false;
        }
        public ActionResult OrderDetails(int id)
        {
            var IDetails = db.ChiTietHoaDon.Where(x => x.ID_HD == id);
          
            var Invoice = db.HoaDon.Where(x => x.ID == id);

            var Total =(decimal)IDetails.Sum(x => x.Gia);

            ViewBag.IDetails = IDetails;
            ViewBag.Invoice = Invoice;
            ViewBag.Total =Total.ToString("N0");
            return View();
        }
        public JsonResult DeleteAll()
        {
            Session[IDetailSession] = null;
            return Json(new
            {
                status = true
            });
        }

        public ActionResult Success()
        {
            return View();
        }
        public ActionResult UnSuccess()
        {
            return View();
        }
        public ActionResult Error()
        {
            return View();
        }
        public JsonResult Delete(long id)
        {
            var sessionCart = (List<InvoiceDetail>)Session[IDetailSession];
            sessionCart.RemoveAll(x => x.Product.ID == id);
            Session[IDetailSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }
        public JsonResult Update(string cartModel)
        {
            var jsonCart = new JavaScriptSerializer().Deserialize<List<InvoiceDetail>>(cartModel);
            var sessionCart = (List<InvoiceDetail>)Session[IDetailSession];

            foreach (var item in sessionCart)
            {
                var jsonItem = jsonCart.SingleOrDefault(x => x.Product.ID == item.Product.ID);
                if (jsonItem != null)
                {
                    item.Quantity = jsonItem.Quantity;
                }
            }
            Session[IDetailSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }

    }
}