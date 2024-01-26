using EQC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace EQC.Common
{
    public class APITokenFilter : ActionFilterAttribute
    {
        private APIService apiService = new APIService();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            string token = filterContext.HttpContext.Request.Headers["token"];
            
            if (apiService.checkTokenVaild(token))
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                LogOut(filterContext);
            }


           
        }
        private void LogOut(ActionExecutingContext filterContext)
        {
            HttpContext context = HttpContext.Current;
            string url = string.Empty;
            RouteValueDictionary dictionary = new RouteValueDictionary(
            new
            {
                controller = "Login",
                action = "APILogout",
                returnUrl = filterContext.HttpContext.Request.RawUrl
            });

            filterContext.Result = new RedirectToRouteResult(dictionary);


            base.OnActionExecuting(filterContext);
        }
    }
}