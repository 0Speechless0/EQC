using EQC.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Models;
using System.Data;
using EQC.Common;

namespace EQC.Controllers
{
    [SessionFilter]

    public class WeakTenderPlanEditController : Controller
    {
        // GET: BackendTenderPlan
        WeakTenderPlanEditService weakTenderPlanEditService = new WeakTenderPlanEditService();
        public ActionResult Index()
        {
            return View("Index");
        }

        public JsonResult Add(FormCollection collection)
        {
            try
            {
                int affect = weakTenderPlanEditService.setMajorEng(collection["tenderNo"], collection["execUnit"]);
                if (affect == 0) throw new Exception();
            }
            catch(Exception e)
            {
                return Json(new { status = e.Message  });
            }

            return Json(new { status = "success" });
        }

        public JsonResult getUnitListByTenderYear(string tenderYear)
        {
            List<object> list ;
            try
            {
                 list = weakTenderPlanEditService.getUnitListByTenderYear(tenderYear);

            }
            catch(Exception e)
            {
                return Json(new { success = "failed" });
            }
            return Json(new { data = list, status = "success" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getEngList(int unitSeq, int year)
        {

            List<string> list ;
            try
            {
                list = weakTenderPlanEditService.getEngList(unitSeq, year);
              
            }
            catch (Exception e)
            {
                return Json(new { success = "failed" });

            }
            return Json(new { data= list, status = "success" }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult getTenderYearList()
        {
            List<string> list;
            try
            {
                list = weakTenderPlanEditService.getTenderYear();

            }
            catch (Exception e)
            {
                return Json(new { success = "failed" });
            }
            return Json(new { data = list, status = "success" });
        }
        public JsonResult getMajorEng(int page, int per_page)
        {



            List<object> list;
            Object totalRows = weakTenderPlanEditService.GetCount();
            int rows;
            if (totalRows == null)
            {
                rows = 0;
            }
            else
            {
                rows = (int)totalRows;
            }
            try
            {
                list = weakTenderPlanEditService.getMajorEng(page - 1, per_page);

            }
            catch (Exception e)
            {
                return Json(new { success = "failed" });

            }
            return Json(new { l = list, t = rows ,status = "success" }, JsonRequestBehavior.AllowGet);
        }

        [HttpDelete]
        public JsonResult Delete(int id)
        {

            try
            {
                weakTenderPlanEditService.deleteByTenderNo(id);
            }
            catch(Exception e)
            {
                return Json(new { status = "failed" });
            }

            return Json(new { status = "success" });
        }
    } 
}