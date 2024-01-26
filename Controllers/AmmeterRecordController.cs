using EQC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using EQC.Common;
using EQC.EDMXModel;

namespace EQC.Controllers
{
    public class AmmeterRecordController : Controller
    {
        // GET: AmmeterRecord
        private AmmeterRecordService service;
        public AmmeterRecordController()
        {
            service = new AmmeterRecordService();
        }
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            ViewBag.Title = "電表紀錄";
            //ViewBag.cardClass= "card whiteBG mb-4 colorset_B";
            //ViewBag.mainLeftClass = "main-left menu_bg_B";
            //ViewBag.nvaClass = "navbar navbar-expand-lg fixed-top navbar-bg-B";
            return View();
        }

        public JsonResult GetAmmeter(int page, int perPage, int selectYear, int selectUnit)
        {
            var l = service.GetAmmeterRecord(selectYear, selectUnit)
                .ToList();

            l.Reverse();

            return Json(new
            {
                list = l
                    .getPagination(page,perPage)
                    ,
                count = l.Count,
                pageCount = l.getPageCount(perPage)


            });
        }
        
        public JsonResult NewAmmeter(int year, int unitSeq, AmmeterRecord m = null)
        {
            service.addAmmeterRecord(year, unitSeq, m);
            return Json(true);
        }
        public JsonResult RecordAmmeter(AmmeterRecord m)
        {

            service.RecordAmmeter(m);
            return Json(true);
        }

        public JsonResult EditAmmeter(AmmeterRecord m)
        {
            service.EditAmmeter(m);
            return Json(true);
        }

        public JsonResult DeleteAmmeter(int id)
        {
            service.deleteAmmeter(id);
            return Json(true);
        }
    }
}