using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using QuanLyBanHang.Models;

namespace QuanLyBanHang
{
    public class MvcApplication : System.Web.HttpApplication
    {
        QL_BanLeEntities db = new QL_BanLeEntities();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Application["Online"] = 0;
            Application["PageView"] = 0;
        }
        protected void Session_Start()
        {
            Application.Lock();//Đồng bộ hóa
            Application["PageView"] = (int)Application["PageView"] + 1;
            View viewUp = db.View.FirstOrDefault(x => x.ID == 1);
               //Lưu lượt xem tổng vào Database
            if (viewUp != null)
            {
                viewUp.Seen = viewUp.Seen + 1;
                db.SaveChanges();
            }
            Application["Online"] = (int)Application["Online"] + 1;
            Application.UnLock();
        }
        protected void Session_End()
        {
            Application.Lock();//Đồng bộ hóa
            Application["Online"] = (int)Application["Online"] - 1;
            Application.UnLock();
            
        }
    }
}
