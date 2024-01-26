using EQC.Models;
using EQC.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    public class CourtVerdictController : Controller
    {
        // GET: CourtVerdict
        public ActionResult Index()
        {
            return View();
        }
        CourtVerdictService courtVerdictService = new CourtVerdictService();
        public JsonResult GetList(int page, int per_page, string coditions)
        {
            SearchCodition ob = JsonConvert.DeserializeObject<SearchCodition>(coditions);
            var list = courtVerdictService.GetList(page - 1, per_page, ob);
            Object totalRows = list.Count;
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
        public JsonResult getDemandFields()
        {
            try
            {
                var map = courtVerdictService.getFields();
                return Json(new { status = "success", data = map }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }

        }


    }
}