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
    public class EADManufacturerController : Controller
    {//廠商履歷評估分析
        EADManufacturerService iService = new EADManufacturerService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }
        //搜索廠商
        public virtual JsonResult Search(string key)
        {
            string manufacturer = iService.GetManufacturer(key);
            if (String.IsNullOrEmpty(manufacturer))
            {
                return Json(new
                {
                    result = -1,
                    msg = "查無廠商"
                });
            } else {
                return Json(new
                {
                    result = 0,
                    m = manufacturer
                });
            }
        }
        //工程清單
        public virtual JsonResult GetEngList(string m)
        {
            List<EADManufacturerEngVModel> engList = iService.GetEngList<EADManufacturerEngVModel>(m);
            return Json(new
            {
                items = engList
            });
        }
        //履約計分
        public virtual JsonResult GetEngScoreList(string m)
        {
            List<EADManufacturerEngScoreVModel> engList = iService.GetEngScoreList<EADManufacturerEngScoreVModel>(m);
            return Json(new
            {
                items = engList
            });
        }
        //重大事故
        public virtual JsonResult GetEngSafetyList(string m)
        {
            List<EADManufacturerEngSafetyVModel> engList = iService.GetEngSafetyList<EADManufacturerEngSafetyVModel>(m);
            return Json(new
            {
                items = engList
            });
        }
        //司法院裁判書
        public virtual JsonResult GetEngVerdictList(string m)
        {
            List<EADManufacturerEngVerdictVModel> engList = iService.GetEngVerdictList<EADManufacturerEngVerdictVModel>(m);
            return Json(new
            {
                items = engList
            });
        }
    }
}