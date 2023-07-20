using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyBanHang.Models;
using System.IO;
using System.Web.Script.Serialization;
using System.Configuration;
using QuanLyBanHang.common;


namespace QuanLyBanHang.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        QL_BanLeEntities db = new QL_BanLeEntities();
        private const string CartSession = "CartSession";
        public ActionResult Index()
        {
            var cart = Session[CartSession];

            var list = new List<CartItem>();

            if (cart != null)
            {
                list = (List<CartItem>)cart;
            }
            return View(list);
        }

        public ActionResult AddItem(long productId, int quantity)
        {

            var product = db.HangHoa.FirstOrDefault(c => c.ID == productId);
            var cart = Session[CartSession];
            if (cart != null)
            {
                var list = (List<CartItem>)cart;
                if (list.Exists(x => x.product.ID == productId))
                {

                    foreach (var item in list)
                    {
                        if (item.product.ID == productId)
                        {
                            item.Quantity += quantity;
                        }
                    }
                }
                else
                {
                    //tạo mới đối tượng cart item
                    var item = new CartItem();
                    item.product = product;
                    item.Quantity = quantity;
                    list.Add(item);
                }
                //Gán vào session
                Session[CartSession] = list;
            }
            else
            {
                //tạo mới đối tượng cart item
                var item = new CartItem();
                item.product = product;
                item.Quantity = quantity;
                var list = new List<CartItem>();
                list.Add(item);
                //Gán vào session
                Session[CartSession] = list;
            }
            return RedirectToAction("Index");
        }
 
        public JsonResult DeleteAll()
        {
            Session[CartSession] = null;
            return Json(new
            {
                status = true
            });
        }

        public ActionResult Checkout()
        {
            int id = (int)Session["UserID"];
            var InforCus = db.KhachHang.FirstOrDefault(x => x.UserID == id);

            ViewBag.InforCus = InforCus;
            return View();
        }
      
        public JsonResult Delete(long id)
        {
            var sessionCart = (List<CartItem>)Session[CartSession];
            sessionCart.RemoveAll(x => x.product.ID == id);
            Session[CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }
        public JsonResult Update(string cartModel)
        {
            var jsonCart = new JavaScriptSerializer().Deserialize<List<CartItem>>(cartModel);
            var sessionCart = (List<CartItem>)Session[CartSession];

            foreach (var item in sessionCart)
            {
                var jsonItem = jsonCart.SingleOrDefault(x => x.product.ID == item.product.ID);
                if (jsonItem != null)
                {
                    item.Quantity = jsonItem.Quantity;
                }
            }
            Session[CartSession] = sessionCart;
            return Json(new
            {
                status = true
            });
        }

    }
}