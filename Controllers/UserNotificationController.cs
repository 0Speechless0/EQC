using EQC.Common;
using EQC.EDMXModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class UserNotificationController : MyController
    {
        // GET: Notification
        public ActionResult Index()
        {
            return View();
        }


        public void Submit(string content, string title, DateTime expire_time, int? roleSeq = null, int? unitSeq = null)
        {
            using(var context = new EQC_NEW_Entities())
            {
                var user = new SessionManager().GetUser();
                
                
                context.Notification.Add(new Notification
                {
                    CreateTime = DateTime.Now,
                    Role = roleSeq,
                    Unit = unitSeq,
                    EmitContent = content.Replace("<table>", "<table class=\"table\">"),
                    Title = title,
                    ExpireTime = expire_time.AddDays(1),
                    CreateUser = user?.Seq  ?? 0 
                });
                context.SaveChanges();

            }

            ResponseJson(true);
        }
        public void Know()
        {
            var user = new SessionManager().GetUser();
            using (var context = new EQC_NEW_Entities())
            {

                var notify = context.UserNotification
                     .Where(r => r.UserMainSeq == user.Seq).FirstOrDefault();
                if (notify == null)
                {
                    context.UserNotification.Add(
                        new UserNotification { NotifyTime = DateTime.Now, UserMainSeq = user.Seq });
                }
                else
                {
                    notify.NotifyTime = DateTime.Now;
                }
                context.SaveChanges();
            }

            ResponseJson(true);
        }

        public void GetList(int page, int perPage)
        {
            using(var context = new EQC_NEW_Entities())
            {

                var list = context.Notification.OrderByDescending(r => r.CreateTime).ToList();
                var pagination = list.getPagination(page, perPage);

                pagination.ForEach(e => e.ReadedCount = context.UserNotification
                    .Where(r => r.NotifyTime > e.CreateTime).Count()
                );

                ResponseJson(
                    new
                    {
                        list = pagination,
                        count = list.Count,
                    }
                    );
            }

        }


        public void Delete(int seq)
        {
            using (var context = new EQC_NEW_Entities())
            {
                context.Notification.Remove(context.Notification.Find(seq));
                context.SaveChanges();
            }

            ResponseJson(true);
        }
    }
}