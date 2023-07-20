using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace QuanLyBanHang.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
    
           protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (Session["USER_SESSION"] == null)

            {
                filterContext.Result = new RedirectToRouteResult(new
                    RouteValueDictionary(new { controller = "Login", action = "Index" }));
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
