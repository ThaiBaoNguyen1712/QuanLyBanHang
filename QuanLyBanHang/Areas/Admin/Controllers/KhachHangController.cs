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
    public class KhachHangController : BaseloginController
    {
        QL_BanLeEntities db = new QL_BanLeEntities();
        List<KhachHang> lkh = new List<KhachHang>();
        // GET: Admin/KhachHang
        public ActionResult Index(string searchString,string sort)
        {
            var Clients = from l in db.KhachHang select l;
           SetViewBag();
           
            return View(Clients);
        }

        public ActionResult Create()
        {
            SetViewBag();
            return View();
        }

        [HttpPost]
        public ActionResult Create(KhachHang kh, HttpPostedFileBase HinhAnh)
        {
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


            return RedirectToAction("Index");
        }

        public ActionResult Update(int ID)
        {
            KhachHang kh = db.KhachHang.FirstOrDefault(x => x.ID == ID);
            SetViewBag(kh.NhomKH);
            return View(kh);
        }

        [HttpPost]
        public ActionResult Update(KhachHang kh, HttpPostedFileBase HinhAnh)
        {
            KhachHang ukh = db.KhachHang.FirstOrDefault(c => c.ID == kh.ID);
            ukh.TenKhachHang = kh.TenKhachHang;
            ukh.DienThoai = kh.DienThoai;
            ukh.NgaySinh = kh.NgaySinh;
            ukh.GioiTinh = kh.GioiTinh;
            ukh.DiaChi = kh.DiaChi;
            ukh.MaSoThue = kh.MaSoThue;
            ukh.Email = kh.Email;
            ukh.Facebook = kh.Facebook;
            ukh.GhiChu = kh.GhiChu;
            ukh.NhomKH = kh.NhomKhachHang.ID;// Chú ý phần này
            
             if (HinhAnh != null && HinhAnh.ContentLength > 0)
            {
                int id = kh.ID;

                string _FileName = "";
                int Index = HinhAnh.FileName.IndexOf(".");
                _FileName = "KH" + id.ToString() + "." + HinhAnh.FileName.Substring(Index + 1);
                string _path = Path.Combine(Server.MapPath("~/Upload/Client"), _FileName);
                HinhAnh.SaveAs(_path);
                KhachHang uhh = db.KhachHang.FirstOrDefault(x => x.ID == id);
                uhh.HinhAnh = _FileName;
             
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult Delete(int ID)
        {
            KhachHang KH = db.KhachHang.FirstOrDefault(x => x.ID == ID);
            db.KhachHang.Remove(KH);
            db.SaveChanges();
            return RedirectToAction("Index");

        }
        public void SetViewBag(int? selectedid = null)
        {
            ViewBag.NhomKH = new SelectList(db.NhomKhachHang.ToList(), "ID", "TenNhomKH", selectedid);
        }
        
    }
}