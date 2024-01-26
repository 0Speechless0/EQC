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
    public class UnitCitiesController : Controller
    {
        // GET: UnitCities
        private UnitCitiesService UnitCitiesService = new UnitCitiesService();
        public ActionResult Index()
        {
            return View("index");
        }
        public JsonResult GetList(int page, int per_page, string keyWord)
        {
            List<Country2WRAMapping> list = UnitCitiesService.GetList(page - 1, per_page, keyWord);
            Object totalRows = UnitCitiesService.GetListCount("Country2WRAMapping", keyWord, new string[] { "RiverBureau", "Country" });
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

                UnitCitiesService.importExcel(processer);

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
                var map = UnitCitiesService.getExcelImportFields();
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
                DateTime time = UnitCitiesService.getLastUpdateTime("UnitCities");
                return Json(new { status = "success", lastUpdateTime = time }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}