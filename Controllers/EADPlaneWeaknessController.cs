using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EADPlaneWeaknessController : Controller
    {//品質管制弱面追蹤與分析
        EADPlaneWeaknessService iService = new EADPlaneWeaknessService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }
        //工程清單
        public virtual JsonResult GetList(List<string> units, int sYear, int eYear, List<string> places, bool isSupervise, string pw, string minBid, string maxBid)
        {
            string unitList = "";
            if (units != null)
            {
                string sp = "";
                foreach (string item in units)
                {
                    unitList += String.Format("{0}'{1}'", sp, item);
                    sp = ",";
                }
            }

            List<EADPlaneWeaknessEngVModel> engList = iService.GetList<EADPlaneWeaknessEngVModel>(unitList, sYear, eYear, places, isSupervise, pw, minBid, maxBid);
            return Json(new
            {
                items = engList
            });
        }
        //標案年分
        public JsonResult GetYearOptions()
        {
            List<EngYearVModel> items = iService.GetEngYearList();//s20230726
            return Json(items);
        }
    }
}