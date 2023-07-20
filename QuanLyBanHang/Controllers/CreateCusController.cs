using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyBanHang.Models;
using System.IO;
using System.Data.Entity;

namespace QuanLyBanHang.Controllers
{
    public class CreateCusController : System.Web.Mvc.Controller
    {
        // GET: CreateCus
        QL_BanLeEntities db = new QL_BanLeEntities();
        List<KhachHang> lkh = new List<KhachHang>();
        // GET: Admin/KhachHang
        public ActionResult Index()
        {
            var Clients = from l in db.KhachHang select l;
            SetViewBag();

            return View(Clients);
        }

        public ActionResult Create(int idUser)
        {

            var IDuser = db.User.FirstOrDefault(x => x.ID == idUser);
            ViewBag.IDuser = idUser;
            SetViewBag();
            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(KhachHang kh, User us,HttpPostedFileBase HinhAnh)
        {
            int idUser = (int)Session["UserID"];
            kh.UserID = idUser;
           
            db.KhachHang.Add(kh);
            db.SaveChanges();

            User existingUser = db.User.Find(kh.UserID); // Tìm User có ID tương ứng với khóa ngoại kh.UserID
            if (existingUser != null)
            {
                existingUser.Name = kh.TenKhachHang; // Cập nhật thông tin Name trong User
                db.SaveChanges();
            }

         
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


            return RedirectToAction("Index","Home");
        }

        public ActionResult Update()
        {
            int? idUser = Session["UserId"] as int?;
            if (idUser.HasValue && idUser.Value != 0)
            {
                KhachHang kh = db.KhachHang.FirstOrDefault(x => x.UserID == idUser);
                if(kh !=null)
                {
                    SetViewBag(kh.NhomKH);
                    return View(kh);
                }
            }
            return RedirectToAction("Index");
        
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
            return RedirectToAction("Index","Home");
        }
      
        public void SetViewBag(int? selectedid = null)
        {
            ViewBag.NhomKH = new SelectList(db.NhomKhachHang.ToList(), "ID", "TenNhomKH", selectedid);
        }

    }
}