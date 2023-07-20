using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyBanHang.Models;


namespace QuanLyBanHang.Areas.Admin.Controllers
{
    public class InvoiceController : BaseloginController
    {
        // GET: Admin/Invoice
        QL_BanLeEntities db = new QL_BanLeEntities();
        private const string IDetailSession = "IDetailSession";
        public ActionResult Index()
        {
           
            return View();
        }
        public ActionResult Finished()
        {
            List<HoaDon> list = new List<HoaDon>(db.HoaDon.Where(x => x.TrangThai == "Thành công"));
             return View(list);
        }
        public ActionResult UnFinished()
        {
            List<HoaDon> list = new List<HoaDon>(db.HoaDon.Where(x => x.TrangThai == "Chờ xác nhận"));
            return View(list);
        }
        public ActionResult Detail(int id)
        {
            var hd = db.HoaDon.Where(x => x.ID == id);

            var CTHD = db.ChiTietHoaDon.Where(x => x.ID_HD == id);

            var Total = CTHD.Sum(x => x.Gia);

            ViewBag.HoaDon = hd;
            ViewBag.ChiTietHoaDon = CTHD;
            ViewBag.Total = Total;
            
            return View();
        }

        public ActionResult DetailUnFinished(int id)
        {
            var hd = db.HoaDon.Where(x => x.ID == id);
            if(hd ==null)
            {
                return RedirectToAction("UnFinished");
            }
            var CTHD = db.ChiTietHoaDon.Where(x => x.ID_HD == id);
            var Total = CTHD.Sum(x => x.Gia);

            ViewBag.HoaDon = hd;
            ViewBag.ChiTietHoaDon = CTHD;
            ViewBag.Total = Total;

            return View(hd.FirstOrDefault());
        }

        [HttpPost]
        public ActionResult DetailUnFinished(HoaDon hd)
        {
            var cthdQuantities = db.ChiTietHoaDon
                .Where(x => x.ID_HD == hd.ID)
                .GroupBy(x => x.ID_SP)
                .Select(g => new
                {
                    ID_SP = g.Key,
                    TotalQuantity = g.Sum(x => x.SoLuong)
                })
                .ToList();

            foreach (var item in cthdQuantities)
            {
                var hanghoa = db.HangHoa.FirstOrDefault(x => x.ID == item.ID_SP);
                if (hanghoa == null || item.TotalQuantity >  hanghoa.SoLuong)
                {
                    return RedirectToAction("Error");
                }
                else
                {
                    hanghoa.SoLuong -= item.TotalQuantity;
                }
            }

            var updatedHd = db.HoaDon.FirstOrDefault(x => x.ID == hd.ID);
            updatedHd.TrangThai = "Thành công";
            db.SaveChanges();

            return RedirectToAction("UnFinished");
        }


        public ActionResult DeleteU(int id)
        {
            HoaDon hd = db.HoaDon.FirstOrDefault(x => x.ID == id);

            if (hd != null)
            {
                var cthdList = db.ChiTietHoaDon.Where(x => x.ID_HD == id).ToList();
                 foreach (var cthd in cthdList)
                {
                    db.ChiTietHoaDon.Remove(cthd);
                }
                  db.HoaDon.Remove(hd);
                  db.SaveChanges();
            }

            TempData.Remove("IDHD");
            return RedirectToAction("UnFinished");
        }
        public ActionResult Delete(int id)
        {
            HoaDon hd = db.HoaDon.FirstOrDefault(x => x.ID == id);

            if (hd != null)
            {
                var cthdList = db.ChiTietHoaDon.Where(x => x.ID_HD == id).ToList();
                foreach (var cthd in cthdList)
                {
                    db.ChiTietHoaDon.Remove(cthd);
                }
                db.HoaDon.Remove(hd);
                db.SaveChanges();
            }

            TempData.Remove("IDHD");
            return RedirectToAction("Finished");
        }
        public ActionResult Error()
        {
            return View();
        }
    }
}