using EQC.Models;
using EQC.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.ViewModel;
using EQC.Common;

namespace EQC.Controllers
{
    public class ThsrController : Controller
    {
        // GET: Thsr
        public ActionResult Index()
        {
            return View();
        }

        ThsrService thsrService = new ThsrService();

        public JsonResult GetList(string coditions, int page = 0, int per_page = -1)
        {
            THSR ob = JsonConvert.DeserializeObject<THSR>(coditions);
            int rowCount = (int)thsrService.GetCount(ob);
            List<THSR> list =  per_page > 0 ?
             
                     thsrService.GetList(page - 1, per_page, ob) :
                     thsrService.GetList(0, rowCount, ob);


            var startOptionList = thsrService.getStartStationList(ob.Direction).SortListByMap(r => r, Order.TrainOrderMap, true);
             var endOptionList = 
                thsrService.getEndingStationList(ob.Direction, ob.StartStationName).SortListByMap(r => r, Order.TrainOrderMap, true) ;

            return Json(new
            {
                l = list,
                t = rowCount,
                startOptions = startOptionList,
                endOptions = endOptionList

            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult getDemandFields()
        {
            try
            {
                var map = thsrService.getFields();
                return Json(new { status = "success", data = map }, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}