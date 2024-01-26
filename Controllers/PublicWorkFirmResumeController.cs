using EQC.Common;

using EQC.Services;
using System;
using EQC.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.EDMXModel;

namespace EQC.Controllers
{
    [SessionFilter]

    public class PublicWorkFirmResumeController : Controller
    {
        private PublicWorkFirmResumeService PublicWorkFirmResumeService = new PublicWorkFirmResumeService();
        public ActionResult Index()
        {
            return View("index");
        }
        public JsonResult GetList(int page, int per_page, string keyWord)
        {
            List<PublicWorkFirmResumeModel> list = PublicWorkFirmResumeService.GetList(page - 1, per_page, keyWord);
            Object totalRows = PublicWorkFirmResumeService.GetListCount("PublicWorkFirmResume", keyWord, new string[] { "CorporationName" });
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

                PublicWorkFirmResumeService.importExcel(processer);

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
                var map = PublicWorkFirmResumeService.getExcelImportFields();
                return Json(new { status = "success", data = map }, JsonRequestBehavior.AllowGet);
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
                DateTime time = PublicWorkFirmResumeService.getLastUpdateTime("PublicWorkFirmResume");
                return Json(new { status = "success", lastUpdateTime = time }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Update(PublicWorkFirmResume m)
        {
            PublicWorkFirmResumeService.update(m, (context) => context.PublicWorkFirmResume.Find(m.Seq));
            return Json(true);
        }
        public JsonResult Delete(int id)
        {
            PublicWorkFirmResumeService.delete((context) => context.PublicWorkFirmResume.Find(id));
            return Json(true);
        }
    }
}
