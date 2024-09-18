using EQC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EQC.EDMXModel;
namespace EQC.Common
{
    public class NotificationProcesser
    {
        public static List<Notification> notifications = new List<Notification>() { new Notification {Title ="test",CreateTime =DateTime.Now, EmitContent="testtst....." } };
        public static Dictionary<int, UserNotification> userNotiy = new Dictionary<int, UserNotification>();

        public static void NotifyUser(int userSeq)
        {
            userNotiy.TryGetValue(userSeq, out UserNotification notify);
            if(notify == null)
            {
                userNotiy.Add(userSeq,
                new UserNotification { NotifyTime = DateTime.Now, UserMainSeq = userSeq });
            }
            else
            {
                notify.NotifyTime = DateTime.Now;
            }

        }
        public static List<Notification> GetNotifications(UserInfo userInfo)
        {

            using(var context = new EQC_NEW_Entities())
            {
                var userNotiy = context.UserNotification.Where(r => r.UserMainSeq == userInfo.Seq).FirstOrDefault();
                //context.Notification.RemoveRange(context.Notification.Where(r => r.ExpireTime < DateTime.Now));
                context.SaveChanges();
                return context
                    .Notification
                    .OrderByDescending(r => r.CreateTime)
                    .ToList()
                    .Where(r =>
                    (r.Role == userInfo.RoleSeq || r.Role == null) &&
                    (r.Unit == userInfo.UnitSeq1 || r.Unit == null) &&
                    ( (r.CreateTime > userNotiy?.NotifyTime || userNotiy == null) &&  r.ExpireTime > DateTime.Now) 
                ).ToList();
            }

        }
    }
}