using EQC.Common;
using EQC.EDMXModel;
using EQC.Models;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class GravelFieldCoordController : Controller
    {
        // GET: Gravelfieldcoord
        private GravelFieldCoordService GravelFieldCoordService = new GravelFieldCoordService();
        public ActionResult Index()
        {
            return View("index");
        }
        public JsonResult GetList(int page, int per_page, string keyWord)
        {
            List<GravelFieldCoordModel> list = GravelFieldCoordService.GetList(page - 1, per_page, keyWord);
            Object totalRows = GravelFieldCoordService.GetListCount("Gravelfieldcoord", keyWord, new string[] { "vendorname" });
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

                GravelFieldCoordService.importExcel(processer);

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
                var map = GravelFieldCoordService.getExcelImportFields();
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
                DateTime time = GravelFieldCoordService.getLastUpdateTime("Gravelfieldcoord");
                return Json(new { status = "success", lastUpdateTime = time }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Update(Gravelfieldcoord m)
        {
            GravelFieldCoordService.update(m, (context) => context.Gravelfieldcoord.Find(m.Seq));
            return Json(true);
        }
        public JsonResult Delete(int id)
        {
            GravelFieldCoordService.delete( (context) => context.Gravelfieldcoord.Find(id));
            return Json(true);
        }
    }
}