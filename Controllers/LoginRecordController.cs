using EQC.EDMXModel;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    public class LoginRecordController : MyController
    {

        public ActionResult Index()
        {
            return View();
        }
        // GET: LoginRecord
        public void GetList(string  from, int days)
        {
            
            using (var context = new EQC_NEW_Entities(false))
            {
                DateTime dateTimeStart = DateTime.Parse(from).AddDays(1);
                DateTime dateTimeEnd = DateTime.Parse(from).AddDays(1).AddDays(-days);
                context.Configuration.LazyLoadingEnabled = false;
                var list = context.UserLoginRecord
                    .Include("UserMain.UserUnitPosition.Unit")
                    .Include("UserMain.UserUnitPosition.Position")
                    .Where(r => r.CreateTime <= dateTimeStart && r.CreateTime >= dateTimeEnd)
                    .OrderByDescending(r => r.CreateTime)
                    .ToList();
                var loginRecordService = new RecordService(context);
                var result = list.Select(r => new
                {
                    Position = r.UserMain.UserUnitPosition.FirstOrDefault()?.Position?.Name,
                    DisplayName = r.UserMain.DisplayName,
                    UserNo = r.UserMain.UserNo,
                    OriginIP = r.OriginIP,
                    CreateTime = r.CreateTime,
                    Unit = loginRecordService.GetLoginRecordUnitStr(r.UserMain.UserUnitPosition.FirstOrDefault()?.Unit)

                }).ToList(); 
                ResponseJson(new { 
                    list = result
                }, "yyyy-MM-dd hh:mm:ss");
            }
        }


    }
}