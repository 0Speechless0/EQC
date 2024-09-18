using EQC.Common;
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


        public void DownloadUserRecord(string from, int days)  
        {
            using(var context =new EQC_NEW_Entities(false))
            {
                DateTime dateTimeStart = DateTime.Parse(from).AddDays(1);
                DateTime dateTimeEnd = DateTime.Parse(from).AddDays(1).AddDays(-days);
                var loginRecordService = new RecordService(context);
                var list = context.UserLoginRecord
                    .Include("UserMain.UserUnitPosition.Unit")
                    .Include("UserMain.UserUnitPosition.Position")
                    .Include("UserMain.UserUnitPosition.Role")
                    .Where(r => r.CreateTime <= dateTimeStart && r.CreateTime >= dateTimeEnd)
                    .OrderByDescending(r => r.CreateTime)
                    .ToList()

                    .Select(r => new
                    {
                        Position = r.UserMain.UserUnitPosition.FirstOrDefault()?.Position?.Name,
                        DisplayName = r.UserMain.DisplayName,
                        UserNo = r.UserMain.UserNo,
                        OriginIP = r.OriginIP,
                        CreateTime = r.CreateTime,
                        Unit = loginRecordService.GetLoginRecordUnitStr(r.UserMain.UserUnitPosition.FirstOrDefault()?.Unit),
                        RoleName = r.UserMain.UserUnitPosition.FirstOrDefault()?.Role.FirstOrDefault()?.Name
                    }); 
                var p = new ExcelProcesser(0, (wookBook) => {
                    var sheet = wookBook.GetSheetAt(0);
                    var row = sheet.CreateRow(0);
                    new string[] {
                    "登入帳號",
                    "名稱",
                    "所屬單位",
                    "角色",
                    "連線位置",
                    "登入時間",
                }.ToList()
                    .ForEach(colName =>
                    {
                        row.CreateCell(row.Cells.Count).SetCellValue(colName);
                    });
                });
                p.insertOneCol(list.Select(r => r.UserNo), 0);
                p.insertOneCol(list.Select(r => r.DisplayName), 1);
                p.insertOneCol(list.Select(r => r.Unit), 2);
                p.insertOneCol(list.Select(r => $"{r.RoleName}"), 3);
                p.insertOneCol(list.Select(r => r.OriginIP), 4);
                p.insertOneCol(list.Select(r => r.CreateTime), 5);
                DownloadFile(p.getTemplateStream(), "使用者登入紀錄.xlsx");
            }

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