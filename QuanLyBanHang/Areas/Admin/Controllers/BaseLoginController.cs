using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QuanLyBanHang.Areas.Admin.Controllers
{
    public class BaseloginController : Controller
    {
        // GET: Admin/Baselogin
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (Session["USER_SESSION"] == null || Session["SESSION_GROUP"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Login", action = "Index", Area = "Admin" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }
}