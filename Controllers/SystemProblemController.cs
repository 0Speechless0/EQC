using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.EDMXModel;
using EQC.Common;
using System.IO;
using EQC.ViewModel.Common;

namespace EQC.Controllers
{
    [SessionFilter]
    public class SystemProblemController : MyController
    {
        // GET: SystemProblem
        public ActionResult Index()
        {
            ViewBag.LeftMenuDisplay = false;
            ViewBag.funcPage = true;
            return View();
        }

        public ActionResult AdminIndex()
        {

            return View();
        }
        public void GetProcessingList(int type, int page = 1, int perPage = 0)
        {
            var userSeq = Utils.getUserSeq();
            using (var context = new EQC_NEW_Entities(false))
            {
                List<SystemProblem> processingList = new List<SystemProblem>();

                SystemProblem.SetFileRoot(HttpContext.Server.MapPath("~/FileUploads/SystemProblem"));
                var list = context.Menu.ToList();
                processingList =
                    (type == 1 ?
                        context.SystemProblem
                            .Include("EngMain")
                            .Include("SystemProblemType")
                            .Include("UserMain") :
                        context.SystemProblem.Include("SystemProblemType"))

                    .OrderByDescending(r => r.CreateTIme)
                    .Where(r => type == 1 || r.UserMainSeq == userSeq)
                    .Where(r => r.Anwser == null || (type == 1 && r.AdminCheck == null))
                    .ToList();


                processingList.ForEach(e =>
                {
                    if (e.EngMain != null) e.EngMain.SystemProblem = null;
                    if (e.SystemProblemType != null) e.SystemProblemType.SystemProblem = null;
                    if (e.UserMain != null) e.UserMain.SystemProblem = null;

                });


                ResponseJson(new
                {
                    list = processingList.getPagination(page, perPage),
                    count = processingList.Count()
                }, "yyyy-MM-dd HH:mm:ss"); ;
            }

        }

        public void AdminCheck(int id)
        {
            using(var context = new EQC_NEW_Entities() )
            {
                var target = context.SystemProblem.Find(id);
                target.AdminCheck = true;
                context.SaveChanges();
            }
            ResponseJson(true);
        }
        public void GetDoneList(int type, int page = 1, int perPage = 0)
        {
            var userSeq = Utils.getUserSeq();
            using (var context = new EQC_NEW_Entities(false))
            {

                List<SystemProblem> doneList = new List<SystemProblem>();
                doneList =
                    context.SystemProblem
                    .Include("SystemProblemType")
                    .OrderByDescending(r => r.CreateTIme)

                    .Where(r => type == 1 || r.UserMainSeq == userSeq)
                    .Where(r => r.AdminCheck != null || type == 0 )
                    .Where(r => r.Anwser != null  ).ToList();
                doneList.ForEach(e =>
                {
                    if (e.SystemProblemType != null) e.SystemProblemType.SystemProblem = null;

                });
                ResponseJson(new
                {
                    list= doneList.getPagination(page, perPage),
                    count = doneList.Count()
                }, "yyyy-MM-dd HH:mm:ss"); ;
            }

        }

        public void GetOneProblem(int Seq)
        {
            using(var context = new EQC_NEW_Entities(false))
            {
                var target = context.SystemProblem.Find(Seq);
                SystemProblem.SetFileRoot(HttpContext.Server.MapPath("~/FileUploads/SystemProblem"));
                context.Entry(target).Reference("EngMain").Load();
                context.Entry(target).Reference("Menu").Load();
                context.Entry(target).Reference("SystemProblemType").Load();

                ResponseJson(target);
            }
        }
        public void UserSave(SystemProblem systemProblem, UserMain userMain)
        {
   
            using(var context = new EQC_NEW_Entities() )
            {
                systemProblem.CreateTIme = DateTime.Now; 
                context.SystemProblem.Add(systemProblem);


                var usrtarget = context.UserMain.Find(userMain.Seq); 
                if(usrtarget != null)
                {
                    usrtarget.Tel = userMain.Tel;
                    usrtarget.TelExt = userMain.TelExt;
                    usrtarget.Email = userMain.Email;
                    usrtarget.Mobile = userMain.Mobile;
                    usrtarget.ModifyTime = DateTime.Now;
                }
                context.SaveChanges();
            }
            ResponseJson(systemProblem.Seq);
        }

        public void AdimResponse(SystemProblem systemProblem)
        {
            
            using (var context = new EQC_NEW_Entities())
            {
                var target = context.SystemProblem.Find(systemProblem.Seq);
                target.ModifyTime = DateTime.Now;
                target.Anwser = systemProblem.Anwser;
                context.SaveChanges();
            }
            ResponseJson(true);
        }
        public void UploadUserFile(int id)
        {
            var files = Request.Files.GetMultiple("files");
            foreach (var file in files)
                $"SystemProblem/{id}".UploadFileToFolder(file);
            ResponseJson(true);
        }

        public void AdminEdit(SystemProblem systemProblem)
        {
            using (var context = new EQC_NEW_Entities())
            {
                var target = context.SystemProblem.Find(systemProblem.Seq);
                if(target != null)
                {
                    context.Entry(target).CurrentValues.SetValues(systemProblem);
                    context.SaveChanges();
                }
            }
            ResponseJson(true);
        }

        public void Delete(int id)
        {
            using(var context = new EQC_NEW_Entities() )
            {
                context.Entry(
                    context.SystemProblem.Find(id)
                    ).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
            }
        }
        public void Download(int id)
        {
            var fileName = $"system_problem_temp{DateTime.Now.ToString("yyyyMMddHHmmss")}.zip";
            var zipFileStream = $"SystemProblem/{id}".GetDownloadListByFolder(true)
                .downloadFilesByZip(Path.GetTempPath(), fileName)
                .GetFileStream(Path.Combine(Path.GetTempPath(), fileName) );

            using(var ms = new MemoryStream() )
            {
                zipFileStream.CopyTo(ms);
                DownloadFile(ms, fileName);
                ms.Dispose();
                zipFileStream.Close();
            }
            System.IO.File.Delete(Path.Combine(Path.GetTempPath(), fileName));

        }

        public void GetTypeOtion()
        {
            using(var context = new EQC_NEW_Entities())
            {
                var option = context.SystemProblemType.Select(r => new SelectVM
                {
                    Text = r.Name,
                    Value = r.Seq.ToString()
                });
                ResponseJson(option);
            }
        }
    }
}