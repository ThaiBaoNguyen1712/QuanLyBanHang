using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using QuanLyBanHang.Models;
using QuanLyBanHang.common;
using System.Text;

namespace QuanLyBanHang.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        QL_BanLeEntities db = new QL_BanLeEntities();
        public ActionResult Index()
        {
            if (Request.Cookies["username"] != null && Request.Cookies["password"] != null)
            {
                ViewBag.username = Request.Cookies["username"].Value;
                ViewBag.password = Request.Cookies["password"].Value;
            }
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }
     
            return View();
        }

        [HttpPost]
        public ActionResult kiemtradangnhap(string username, string password)
        {
            if (Request.Cookies["username"] != null && Request.Cookies["username"] != null)
            {
                username = Request.Cookies["username"].Value;
                password = Request.Cookies["password"].Value;
            }

            if (checkpassword(username, password))
            {
                var userSession = new UserLogin();
                userSession.UserName = username;
                Session.Add("USER_SESSION", userSession);
                int userId = GetUserIdByUsername(username);
                Session["UserId"] = userId;

                //if (ghinho == "on")//Ghi nhớ
                //    ghinhotaikhoan(username, password);

                if (checkInfo(username))
                {
                   
                    return Redirect("~/Cart/Index");
                    
                }
                else
                    return RedirectToAction("Index", "CreateCus");

            }
            TempData["ErrorMessage"] = "Sai thông tin tài khoản hoặc mật khẩu!";
            return Redirect("~/Login/Index");
        }

        public int GetUserIdByUsername(string username)
        {
            var user = db.User.FirstOrDefault(x => x.UserName == username);
            if (user != null)
            {
                return user.ID; // Giả sử ID của người dùng được lưu trong thuộc tính Id
            }
            return 0; // Trả về giá trị mặc định (hoặc giá trị thích hợp) nếu không tìm thấy người dùng
        }

        public bool checkpassword(string username, string password)
        {
            //LinQ
            //var ketqua = from u in db.User
            //             where u.UserName == username && u.Password == password
            //             select u;//Cách 1
            var f_password = GetMD5(password);
            var data = db.User.Where(s => s.UserName.Equals(username) && s.Password.Equals(f_password)).ToList();// Giải mã hóa
            if(data.Count>0)
                 return true;
            else
                return false;


        }
        //Tạo mã hóa string MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }
        public ActionResult Register()
        {
            
            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }
            return View();
        }
        [HttpPost]
        public ActionResult Register(string UserName,string PassWord,string rePassWord)
        {
           
            var existingUser = db.User.FirstOrDefault(x => x.UserName == UserName);
            //Kiểm tra tên tài khoản đã tồn tại hay chưa
            if (existingUser == null)
            {
                // check Mật khẩu và mật khẩu nhập lại
                if (PassWord == rePassWord)
                {
                    var user = new User();
                    user.UserName = UserName;
                    user.Password = GetMD5(PassWord);//Lưu mật khẩu dưới dạng mã hóa
                    user.GroupID = 2;

                    db.User.Add(user);
                    db.SaveChanges();
                    Session["UserID"] = user.ID;
                    Session["USER_SESSION"] = Session["UserID"];
             
                    return RedirectToAction("Index", "CreateCus");
                }
                else
                {
                    TempData["ErrorMessage"] = "Mật khẩu không trùng khớp!";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Tên tài khoản đã tồn tại!";
            }
           
            return Redirect("/Login/Register");
        }
        public bool checkInfo(string username)
        {
            var CheckName = db.User.FirstOrDefault(x => x.UserName ==username && x.Name !=null);
            if (CheckName !=null)
                return true;
            
            else
                return false;
        }
        public ActionResult SignOut()
        {

            Session["USER_SESSION"] = null;
            Session["SESSION_GROUP"] = null;
            Session.Remove("idUser");
            Session.Remove("UserID");
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            if (Request.Cookies["username"] != null && Request.Cookies["password"] != null)
            {
                HttpCookie us = Request.Cookies["username"];
                HttpCookie ps = Request.Cookies["password"];

                ps.Expires = DateTime.Now.AddDays(-1);
                us.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(us);
                Response.Cookies.Add(ps);
            }

            return RedirectToAction("Index");
        }

        //ChangePassWord
        public ActionResult ChangePassWord()
        {
            if(Session["UserID"]!=null)
            {
                int id = (int)Session["UserID"];
                User us = db.User.FirstOrDefault(x => x.ID == id);
                ViewBag.User = us;
                if (TempData.ContainsKey("ErrorMessage"))
                {
                    ViewBag.ErrorMessage = TempData["ErrorMessage"];
                }
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
         
    
        }
        [HttpPost]
        public ActionResult ChangePassWord(string UserName, string PassWord, string NewPassWord, string reNewPassWord)
        {
            int id = (int)Session["UserID"];
            var f_password = GetMD5(PassWord);
            var user = db.User.FirstOrDefault(s => s.UserName == UserName && s.Password == f_password);

            if (user != null)
            {
                if (NewPassWord == reNewPassWord)
                {
                    user.Password = GetMD5(NewPassWord);
                    db.SaveChanges();
                    TempData["ErrorMessage"] = "Thay đổi mật khẩu thành công!";
                   
                }
                else
                    TempData["ErrorMessage"] = "Nhập lại mật khẩu mới không trùng khớp !";
                   
            }
            else
            {
                TempData["ErrorMessage"] = "Mật khẩu không đúng!";
                
            }

            return RedirectToAction("ChangePassWord");
        }
    }
}