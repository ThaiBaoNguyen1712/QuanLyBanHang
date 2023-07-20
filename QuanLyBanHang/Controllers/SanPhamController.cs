using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyBanHang.Models;
using System.IO;

namespace QuanLyBanHang.Controllers
{
    public class SanPhamController : System.Web.Mvc.Controller
    {
        // GET: SanPham
        QL_BanLeEntities db = new QL_BanLeEntities();
        [HttpPost]
        public ActionResult Index(string Search)
        {

            int id;
            bool isNumeric = int.TryParse(Search, out id);
            var products = db.HangHoa.Where(h => h.ID == id
                                                  || h.TenHang.Contains(Search)
                                                  || h.NhomHangHoa.TenNhomHH.Contains(Search)||h.GhiChu.Contains(Search))
                                          .ToList();
         
            if (!string.IsNullOrEmpty(Search))
            {
                ViewBag.SearchResult = products;
               
            }
            else
            {
                ViewBag.SearchResult = null;
            }    
            ViewBag.Search = Search;
        
            return View();
        }
        public ActionResult Detail(int id)
        {
            HangHoa hh = db.HangHoa.FirstOrDefault(c => c.ID == id);
            ThuongHieu brand = db.ThuongHieu.FirstOrDefault(b => b.ID == hh.NhomHang);
            ViewBag.Title = hh.TenHang;
            ViewBag.ThuongHieu = brand.TenThuongHieu;

            return View(hh);
        }

        public void SetViewbagType(int? selectedid = null)
        {
            ViewBag.NhomHang = new SelectList(db.NhomHangHoa.ToList(), "ID", "TenNhomHH", selectedid);
        }
        public void SetViewBagBrand(int? selectedid = null)
        {
            ViewBag.ThuongHieu = new SelectList(db.ThuongHieu.ToList(), "ID", "TenThuongHieu", selectedid);
        }
     
   
                
        public ActionResult MayBo()
        {
            var Products = from l in db.HangHoa where l.NhomHangHoa.TenNhomHH == "Máy bộ" select l ;
            SetViewBagBrand();
            SetViewbagType();
            return View(Products);
        }
        public ActionResult Laptop()
        {
            var Products = from l in db.HangHoa where l.NhomHangHoa.TenNhomHH == "Laptop" select l;
            SetViewBagBrand();
            SetViewbagType();
            return View(Products);
        }
        public ActionResult Combo()
        {
            var Products = from l in db.HangHoa where l.NhomHangHoa.TenNhomHH == "Combo" select l;
            SetViewBagBrand();
            SetViewbagType();
            return View(Products);
        }
        public ActionResult PhuKien()
        {
            var Products = from l in db.HangHoa where l.NhomHangHoa.TenNhomHH == "Phụ kiện" select l;
            SetViewBagBrand();
            SetViewbagType();
            return View(Products);
        }
        public ActionResult LikeNew()
        {
            var Products = from l in db.HangHoa where l.NhomHangHoa.TenNhomHH == "LikeNew" select l;
            SetViewBagBrand();
            SetViewbagType();
            return View(Products);
        }
        public ActionResult Old()
        {
            var Products = from l in db.HangHoa where l.NhomHangHoa.TenNhomHH == "Old" select l;
            SetViewBagBrand();
            SetViewbagType();
            return View(Products);
        }
    }
}