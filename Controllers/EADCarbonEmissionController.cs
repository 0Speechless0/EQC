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
    public class EADCarbonEmissionController : Controller
    {//水利工程淨零碳排分析
        EADCarbonEmissionService iService = new EADCarbonEmissionService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }
        
        //工程清單
        public virtual JsonResult GetList(List<string> units, int sYear, int eYear, int pageRecordCount, int pageIndex, string refCodeKeyWord ="")
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
            decimal co2Total = 0;
            int total = iService.GetListCount(unitList, sYear, eYear, refCodeKeyWord, ref co2Total);
            List<EADCarbonEmissionVModel> engList = iService.GetList<EADCarbonEmissionVModel>(unitList, sYear, eYear, refCodeKeyWord, pageRecordCount, pageIndex);
            return Json(new
            {
                total = total,
                co2Total = co2Total,
                items = engList
            });
        }
        //標案年分
        public JsonResult GetYearOptions()
        {
            List<EngYearVModel> items = new TenderPlanService().GetEngYearList();
            return Json(items);
        }
        //碳排清單
        public JsonResult GetCEList(int id, string keyword, string refCodeKeyWord = "")
        {
            List<CarbonEmissionPayItemVModel> ceList = iService.GetCEList<CarbonEmissionPayItemVModel>(id, keyword, refCodeKeyWord);
            if (ceList.Count == 0 && string.IsNullOrEmpty(keyword))
            {//初始化資料
                return Json(new
                {
                    result = -1,
                    msg = "無資料"
                });
            }
            decimal? co2Total = null;
            decimal? co2ItemTotal = null;
            new CarbonEmissionPayItemService().CalCarbonTotal(id, ref co2Total, ref co2ItemTotal);
            return Json(new
            {
                result = 0,
                items = ceList,
                co2Total = co2Total,
                co2ItemTotal = co2ItemTotal
            });
        }
    }
}