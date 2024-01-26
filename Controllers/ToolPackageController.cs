using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.EDMXModel;
using EQC.Common;
using Newtonsoft.Json;

namespace EQC.Controllers
{
    public class ToolPackageController : Controller
    {

        public ActionResult Index()
        {
            Utils.setUserClass(this);

            ViewBag.Title = "工具包";
            return View();
        }
        // GET: ToolPackage
        public JsonResult GetList(int engSeq)
        {
            using(var context = new EQC_NEW_Entities())
            {
                context.Configuration.LazyLoadingEnabled = false;
                var list = context.ToolPackage.Where(row => row.EngSeq == engSeq).OrderByDescending(row => row.CreateTime).ToList();
                return Json(list);
            }
        }

        public JsonResult UploadFile(HttpPostedFileBase file, string m)
        {
            ToolPackage arg =  JsonConvert.DeserializeObject<ToolPackage>(m);

            string GUID = Guid.NewGuid().ToString("B").ToUpper();
            $"ToolPackage/{arg.EngSeq}".UploadFileToFolder(file, GUID + file.FileName);
            using (var context = new EQC_NEW_Entities())
            {
                var packageItem = context.ToolPackage.Find(GUID + file.FileName);
                if(packageItem != null)
                {

                    packageItem.ModifyTime = DateTime.Now;
                    packageItem.FileName = GUID + file.FileName;
                    packageItem.Stage = (byte)arg.Stage;
                    packageItem.Description = arg.Description;
                }
                else if (context.ToolPackage.Find(GUID + file.FileName) == null)
                {
                    context.ToolPackage.Add(new ToolPackage
                    {
                        EngSeq = arg.EngSeq,
                        CreateTime = DateTime.Now,
                        FileName = GUID + file.FileName,
                        Stage = (byte)arg.Stage,
                        Description = arg.Description

                    });
                }
                context.SaveChanges();
            }

            return Json(true);
        }

        public JsonResult Delete(string fileName,int engSeq)
        {
            using (var context = new EQC_NEW_Entities())
            {

                context.Entry(context.ToolPackage.Find(fileName)).State = System.Data.Entity.EntityState.Deleted;
                context.SaveChanges();
                $"ToolPackage/{engSeq}".RemoveFile(fileName);
            }
            return Json(true);
        }
        public JsonResult Update(ToolPackage m)
        {
            using (var context = new EQC_NEW_Entities())
            {
                var packageItem = context.ToolPackage.Find(m.FileName);

                packageItem.Description = m.Description;
                packageItem.Stage = m.Stage;
                packageItem.ModifyTime = DateTime.Now;
                context.SaveChanges();
            }
            return Json(true);
        }


    }
}