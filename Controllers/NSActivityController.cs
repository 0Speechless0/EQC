using EQC.Common;
using EQC.EDMXModel;
using EQC.Models;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class NSActivityController : Controller
    {
        NSActivityService NSActivityService = new NSActivityService();

        public ActionResult Index()
        {

            return View("Index");
        }
        public JsonResult GetList(int page, int per_page, string keyWord)
        {
            List<NSActivityModel> list = NSActivityService.GetList(page - 1, per_page, keyWord);
            Object totalRows = NSActivityService.GetListCount("NationalSupervisedActivity", keyWord, new string[] { "ConstructionName" });
            int rows;
            if (totalRows == null)
            {
                rows = 0;
            }
            else
            {
                rows = (int)totalRows;
            }
            return Json(new
            {
                l = list,
                t = rows
            }, JsonRequestBehavior.AllowGet);

        }


        // POST api/<controller>
        [HttpPost]
        public JsonResult Add(NSActivityModel collection)
        {
            try
            {
                int newId = NSActivityService.Add(collection);
                return Json(new { status = "success", newId = newId }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public JsonResult excelUpload()
        {
            try
            {
                var file = Request.Files[0];
                if (file.ContentLength == 0)
                {
                    return Json(new
                    {
                        status = "failed",
                        message = "no Content"
                    }, JsonRequestBehavior.AllowGet);
                };
                ExcelProcesser processer = new ExcelProcesser(file, file.FileName);

                NSActivityService.importExcel(processer);

            }
            catch (Exception e)
            {
                return Json(new
                {
                    status = "failed",
                    message = e.Message
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                status = "success"
            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult getDemandFields()
        {
            try
            {
                var map = NSActivityService.getExcelImportFields();
                return Json(new { status = "success", data = map}, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult getLastUpdateTime()
        {
            try
            {
                DateTime time = NSActivityService.getLastUpdateTime("NationalSupervisedActivity");
                return Json(new { status = "success", lastUpdateTime = time }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Update(NationalSupervisedActivity m)
        {
            NSActivityService.update(m, (context) => context.NationalSupervisedActivity.Find(m.Seq));
            return Json(true);
        }
        public JsonResult Delete(int id)
        {
            NSActivityService.delete( (context) => context.NationalSupervisedActivity.Find(id));
            return Json(true);
        }

    }
}