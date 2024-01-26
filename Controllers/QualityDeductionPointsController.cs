using EQC.Common;
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
    public class QualityDeductionPointsController : Controller
    {
        private QualityDeductionPointsService QualityDeductionPointsService = new QualityDeductionPointsService();
        public ActionResult Index()
        {
            return View("index");
        }
        public JsonResult GetList(int page, int per_page,string keyWord)
        {
            List<QualityDeductionPoints> list = QualityDeductionPointsService.GetList(page - 1, per_page, keyWord);
            Object totalRows = QualityDeductionPointsService.GetListCount("QualityDeductionPoints", keyWord, new string[] { "MissingNo" });
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

                QualityDeductionPointsService.importExcel(processer);

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
                var map = QualityDeductionPointsService.getExcelImportFields();
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
                DateTime time = QualityDeductionPointsService.getLastUpdateTime("QualityDeductionPoints");
                return Json(new { status = "success", lastUpdateTime = time }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult Add(QualityDeductionPoints value)
        {
            try
            {
                QualityDeductionPointsService.Add(value);
                return Json(new { status = "success" });
            }
            catch(Exception e)
            {
                return Json(new { status = "failed" });
            }

        }
        public JsonResult Update(QualityDeductionPoints m)
        {
            try
            {
                QualityDeductionPointsService.Update(m);
                return Json(true );
            }
            catch(Exception e)
            {
                return Json(false);
            }

        }
        public JsonResult Delete(int id)
        {
            try
            {
                QualityDeductionPointsService.Delete(id);
                return Json(true);
            }
            catch(Exception e)
            {
                return Json(false);
            }


        }
    }
}