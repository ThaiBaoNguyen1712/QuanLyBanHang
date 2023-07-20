using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyBanHang.Models;


namespace QuanLyBanHang.Areas.Admin.Controllers
{
    public class KiemKhoController : BaseloginController
    {
        // GET: Admin/KiemKho
        QL_BanLeEntities db = new QL_BanLeEntities();
        public ActionResult Index()
        {
            var products = from l in db.HangHoa select l;
            return View(products);
        }
     
        public ActionResult Import(int id)
        {
            HangHoa hh = db.HangHoa.FirstOrDefault(x => x.ID == id);
            return View(hh);
        }
        [HttpPost]
        public ActionResult Import(HangHoa hh, int Nhap)
        {
            HangHoa uhh = db.HangHoa.FirstOrDefault(x => x.ID == hh.ID);
            if (uhh != null)
            {
                uhh.SoLuong += Nhap;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                // Xử lý khi không tìm thấy đối tượng HangHoa
                // Ví dụ: Hiển thị thông báo lỗi hoặc chuyển hướng đến trang không tìm thấy
                return HttpNotFound();
            }

        }

        public ActionResult Export(int id)
        {
            HangHoa hh = db.HangHoa.FirstOrDefault(x => x.ID == id);
            return View(hh);
        }
        [HttpPost]
        public ActionResult Export(HangHoa hh,int Xuat)
        {
            HangHoa uhh = db.HangHoa.FirstOrDefault(x => x.ID == hh.ID);
            if (uhh != null)
            {
                uhh.SoLuong -= Xuat;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                // Xử lý khi không tìm thấy đối tượng HangHoa
                // Ví dụ: Hiển thị thông báo lỗi hoặc chuyển hướng đến trang không tìm thấy
                return HttpNotFound();
            }
        }
    }
}