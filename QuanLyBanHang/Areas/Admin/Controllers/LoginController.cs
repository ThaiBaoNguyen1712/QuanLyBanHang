using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using QuanLyBanHang.Models;
using QuanLyBanHang.common;
using System.Security.Cryptography;
using System.Text;

namespace QuanLyBanHang.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        // GET: Admin/Login
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
        public void ghinhotaikhoan(string username, string password)
        {
            HttpCookie us = new HttpCookie("username");
            HttpCookie pas = new HttpCookie("password");

            us.Value = username;
            pas.Value = password;

            us.Expires = DateTime.Now.AddDays(1);
            pas.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(us);
            Response.Cookies.Add(pas);

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

                var listGroups = GetListGroupID(username);//Có thể viết dòng lệnh lấy các GroupID từ CSDL, ví dụ gán ="ADMIN", dùng List<string>

                if (listGroups.Contains("Admin"))
                {
                    Session.Add("SESSION_GROUP", listGroups);
                    Session.Add("USER_SESSION", userSession);
                    return Redirect("~/Admin/HomeAdmin");
                }
            }
            TempData["ErrorMessage"] = "Sai thông tin tài khoản hoặc mật khẩu !";
            return Redirect("~/Admin/Login");
        }
        public List<string> GetListGroupID(string userName)
        {
            // var user = db.User.Single(x => x.UserName == userName);

            var data = (from a in db.UserGroup
                        join b in db.User on a.ID equals b.GroupID
                        where b.UserName == userName

                        select new
                        {
                            UserGroupID = b.GroupID,
                            UserGroupName = a.Name
                        });

            return data.Select(x => x.UserGroupName).ToList();

        }

        public bool checkpassword(string username, string password)
        {
            //LinQ
            var f_password = GetMD5(password);
            var data = db.User.Where(s => s.UserName.Equals(username) && s.Password.Equals(f_password)).ToList();// Giải mã hóa
            if (data.Count() > 0)
                return true;
            else
                return false;


        }
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
        public ActionResult SignOut()
        {

            Session["USER_SESSION"] = null;
            Session["SESSION_GROUP"] = null;
            Response.Cookies.Clear();
            Session.Clear();
            if (Request.Cookies["username"] != null && Request.Cookies["password"] != null)
            {
                HttpCookie us = Request.Cookies["username"];
                HttpCookie ps = Request.Cookies["password"];

                ps.Expires = DateTime.Now.AddDays(-1);
                us.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(us);
                Response.Cookies.Add(ps);
            }

            return Redirect("/Admin/Login");
        }
    }
}