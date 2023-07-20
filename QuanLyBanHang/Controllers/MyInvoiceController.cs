using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyBanHang.Models;

namespace QuanLyBanHang.Controllers
{
    public class MyInvoiceController : BaseController
    {
        // GET: MyInvoice
        QL_BanLeEntities db = new QL_BanLeEntities();
        public ActionResult Index()
        {
            int id = getIDcus();
            if(getIDcus()==0)
            {
                return RedirectToAction("Index", "Login");
            }
            List<decimal> totalAmountList = new List<decimal>();
            var HD = db.HoaDon.Where(x => x.ID_KhachHang == id && x.TrangThai == "Thành công").ToList();
            var HDHT = db.HoaDon.Where(x => x.ID_KhachHang == id && x.TrangThai == "Chờ xác nhận").ToList();

            if (HD.Count > 0)
            {
                var HDIDs = HD.Select(h => h.ID).ToList();
                var IDetails = db.ChiTietHoaDon.Where(x => HDIDs.Contains(x.ID_HD)).ToList();
                ViewBag.HoaDonList = HD;
                ViewBag.ChiTietHoaDonList = IDetails;


                foreach (var detail in IDetails)
                {
                    decimal totalAmount = (decimal)(detail.SoLuong * detail.Gia.GetValueOrDefault(0)); // Tính tổng giá trị cho dòng hiện tại

                }

                ViewBag.TotalAmount = totalAmountList; // Thêm tổng tiền vào ViewBag
            }
            else
            {
                ViewBag.HoaDonList = null;
                ViewBag.ChiTietHoaDonList = null;
            }

            if (HDHT.Count > 0)
            {
                var HDHTIDs = HDHT.Select(h => h.ID).ToList();
                var IDetailsHT = db.ChiTietHoaDon.Where(x => HDHTIDs.Contains(x.ID_HD)).ToList();
                ViewBag.HoaDonListHT = HDHT;
                ViewBag.ChiTietHoaDonListHT = IDetailsHT;

            }
            else
            {
                ViewBag.HoaDonListHT = null;
                ViewBag.ChiTietHoaDonListHT = null;
            }

            return View();
        }
        public int getIDcus()
        {
            if (Session["UserID"] != null)
            {
                int userID = (int)Session["UserID"];
                var khachhang = db.KhachHang.FirstOrDefault(x => x.UserID == userID);
                if (khachhang != null)
                {
                    int IDcus = khachhang.ID;
                    return IDcus;
                }
            }
             
            return 0;
        }
        public ActionResult Delete(int id)
        {
            var hd = db.HoaDon.FirstOrDefault(x => x.ID == id);
            var cthd = db.ChiTietHoaDon.Where(x => x.ID_HD == id);
            foreach(var item in cthd)
            {
                db.ChiTietHoaDon.Remove(item);
                
            }
            db.HoaDon.Remove(hd);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}