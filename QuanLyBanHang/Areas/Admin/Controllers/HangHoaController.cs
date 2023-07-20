using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyBanHang.Models;
using System.IO;
using System.Data.Entity;

namespace QuanLyBanHang.Areas.Admin.Controllers
{
    public class HangHoaController : BaseloginController
    {
        // GET: Admin/HangHoa
        QL_BanLeEntities db = new QL_BanLeEntities();
        List<HangHoa> lhh = new List<HangHoa>();
        public ActionResult Index( string searchString, string sort)
        {

            // lấy toàn bộ liên kết 
            var products = from l in db.HangHoa select l;
            SetViewBagBrand();
            SetViewBagType();
            return View(products);
        }
        [HttpPost]
        public ActionResult Create(HangHoa hh, HttpPostedFileBase HinhAnh)
        {
            db.HangHoa.Add(hh);
            hh.Created = DateTime.Now;
            db.SaveChanges();

            if (HinhAnh != null && HinhAnh.ContentLength > 0)
            {
                int id = hh.ID; // Lấy ID của sản phẩm mới được tạo
                string _FileName = "";
                int Index = HinhAnh.FileName.LastIndexOf(".");
                _FileName = "SP" + id.ToString() + HinhAnh.FileName.Substring(Index);
                string _path = Path.Combine(Server.MapPath("~/Upload/Product"), _FileName);
                HinhAnh.SaveAs(_path);
                hh.HinhAnh = _FileName;
            }
            else
            {
                hh.HinhAnh = "none.jpg";
            }

            db.Entry(hh).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Create()
        {
            SetViewBagBrand();
            SetViewBagType();
            return View();
        }
       
        public ActionResult Update(int ID)
        {
            HangHoa hh = db.HangHoa.FirstOrDefault(c => c.ID == ID);
            SetViewBagType(hh.NhomHang);
            SetViewBagBrand(hh.ThuongHieu);
            return View(hh);
        }

        [HttpPost]
        public ActionResult Update(HangHoa hh, HttpPostedFileBase HinhAnh)
        {
            HangHoa uhh = db.HangHoa.FirstOrDefault(c => c.ID == hh.ID);
            uhh.TenHang = hh.TenHang;
            uhh.ThuongHieu = hh.ThuongHieu1.ID;// Chú ý
            uhh.NhomHang = hh.NhomHangHoa.ID;// Chú ý
            uhh.GiaBan = hh.GiaBan;
            uhh.GiaVon = hh.GiaVon;
            uhh.SoLuong = hh.SoLuong;
            uhh.ViTri = hh.ViTri;
            uhh.GhiChu = hh.GhiChu;

            if (HinhAnh != null && HinhAnh.ContentLength > 0)
            {
                int id = hh.ID;
                uhh.HinhAnh = hh.HinhAnh;
                string _FileName = "";
                int Index = HinhAnh.FileName.IndexOf(".");
                _FileName = "SP" + id.ToString() + "." + HinhAnh.FileName.Substring(Index + 1);
                string _path = Path.Combine(Server.MapPath("~/Upload/Product"), _FileName);
                HinhAnh.SaveAs(_path);
                uhh.HinhAnh = _FileName;
                
            }
           
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int ID)
        {
            HangHoa hh = db.HangHoa.FirstOrDefault(c => c.ID == ID);
            db.HangHoa.Remove(hh);
            db.SaveChanges();
           return RedirectToAction("Index");
        }

        public void SetViewBagBrand(int? selectedid = null)
        {

            ViewBag.ThuongHieu = new SelectList(db.ThuongHieu.ToList(), "ID", "TenThuongHieu", selectedid);
        }
        public void SetViewBagType(int? selectedId = null)
        {
            ViewBag.NhomHang = new SelectList(db.NhomHangHoa.ToList(), "ID", "TenNhomHH", selectedId);
        }


    }
}