using EQC.Common;
using EQC.Detection;
using EQC.EDMXModel;
using EQC.Scheduler;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace EQC
{
    public class MvcApplication : System.Web.HttpApplication
    {


        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError().InnerException ;
            if (exception != null)
            {
                APIDetection.SetError(Thread.CurrentThread.ManagedThreadId, exception.Message);

                BaseService.log.Info("Err: " + exception.Message+ "Source:" + exception.Source);
            }
        }
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            DbContext.changeTextSetter =
                (changeText, changeTable)
                =>
                {
                    var record = Detection.APIDetection.GetActionRecord(Thread.CurrentThread.ManagedThreadId);
                    record.ChangeText = changeText;
                    record.ActionTable = changeTable;
                };
            SqlDB.changeTextSetter =
                (changeText, changeTable)
            =>
            {
                var record = Detection.APIDetection.GetActionRecord(Thread.CurrentThread.ManagedThreadId);
                record.ChangeText = changeText;
                record.ActionTable = changeTable;
            };

            // log4net
            string log4netPath = Server.MapPath("~/log4net.config");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(log4netPath));
            Utils.rootPath =  HttpContext.Current.Server.MapPath("~");
            EmailForDepUser emailForDepUserScheduler = new EmailForDepUser();
            HttpConfiguration config = GlobalConfiguration.Configuration;
            config.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Serialize;

            config.Formatters.JsonFormatter.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
        }

        protected void Application_EndRequest()
        {
        
            var record = Detection.APIDetection.GetActionRecord(Thread.CurrentThread.ManagedThreadId);
            if (record.CreateTime != null)
                Detection.APIDetection.FinishAction(Thread.CurrentThread.ManagedThreadId, record.UserMainSeq);
        }
        protected void Application_BeginRequest()
        {
      
            if (Request.Headers.AllKeys.Contains("Origin", StringComparer.OrdinalIgnoreCase) &&
                 Request.HttpMethod == "OPTIONS")
            {
                Response.StatusCode = 204;
                Response.Flush();
            }

        }
    }
}
