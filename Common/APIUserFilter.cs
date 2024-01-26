using EQC.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EQC.Controllers;
using EQC.Detection;

namespace EQC.Common
{
    public class APIUserFilter : ActionFilterAttribute
    {
        private APIUserService service = new APIUserService();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            string token = filterContext.HttpContext.Request.Headers["token"];
            string adminPassword = filterContext.HttpContext.Request.Headers["adminPassword"];
            string actionName =  filterContext.RouteData.Values["action"].ToString();
            filterContext.ActionDescriptor.Record(new UserInfo() { Seq = -1 } );
            if (adminPassword == ConfigurationManager.AppSettings["APIUserControlPassword"].ToString() )
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                if (service.checkActionPermission(actionName))
                {
                    base.OnActionExecuting(filterContext);
                }
                else
                {

                    Reject(filterContext);
                }


            }




        }
        private void Reject(ActionExecutingContext filterContext)
        {
            HttpContext context = HttpContext.Current;
            string url = string.Empty;
            RouteValueDictionary dictionary = new RouteValueDictionary(
            new
            {
                controller = "Login",
                action = "Reject",
                message = "沒有權限",
                returnUrl = filterContext.HttpContext.Request.RawUrl
            });

            filterContext.Result = new RedirectToRouteResult(dictionary);

            base.OnActionExecuting(filterContext);
        }

    }
}