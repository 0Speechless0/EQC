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
    public class EADCommitteeController : Controller
    {//工程採購評選委員分析
        EADCommitteeService iService = new EADCommitteeService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }
        //工程清單
        public virtual JsonResult GetEngList(string name, byte kind, int sYear, int eYear)
        {
            List<EADCommitteeEngVModel> engList = iService.GetEngList<EADCommitteeEngVModel>(name, kind, sYear, eYear);
            return Json(new
            {
                items = engList
            });
        }
        //委員清單
        public virtual JsonResult GetList(List<string> units, int sYear, int eYear, string committee)
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

            List<EADCommitteeVModel> engList = iService.GetList<EADCommitteeVModel>(unitList, sYear, eYear, committee);
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