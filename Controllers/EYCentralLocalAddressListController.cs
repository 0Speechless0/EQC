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
    public class EYCentralLocalAddressListController : Controller
    {
        // GET: EYCentralLocalAddressList
        private EYCentralLocalAddressListService EYCentralLocalAddressListService = new EYCentralLocalAddressListService();
        public ActionResult Index()
        {
            return View("index");
        }
        public JsonResult GetList(int page, int per_page, string keyWord, string strOption)
        {
            List<EYCentralLocalAddressListModel> list = EYCentralLocalAddressListService.GetList(page - 1, per_page, keyWord, strOption);
            Object totalRows = EYCentralLocalAddressListService.GetListCount("EYCentralLocalAddressList", keyWord, new string[] { "OrganizerName" });
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
        public JsonResult GetOrganizerList()
        {
            List<string> list = EYCentralLocalAddressListService.getOrganizerArea();
            return Json(new
            {
                l = list
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

                EYCentralLocalAddressListService.importExcel(processer);

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
                var map = EYCentralLocalAddressListService.getExcelImportFields();
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
                DateTime time = EYCentralLocalAddressListService.getLastUpdateTime("EYCentralLocalAddressList");
                return Json(new { status = "success", lastUpdateTime = time }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Update(EYCentralLocalAddressList m)
        {
            EYCentralLocalAddressListService.update(m, (context) => context.EYCentralLocalAddressList.Find(m.Seq));
            return Json(true);
        }
        public JsonResult Delete(int id)
        {
            EYCentralLocalAddressListService.delete( (context) => context.EYCentralLocalAddressList.Find(id));
            return Json(true);
        }
    }
}
