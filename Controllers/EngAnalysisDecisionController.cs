using EQC.Common;
using EQC.EDMXModel;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EngAnalysisDecisionController : Controller
    {//工程分析決策
        EngAnalysisDecisionService iService = new EngAnalysisDecisionService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            new SessionManager().currentSystemSeq = "9";
            return View("Index");
        }
        public ActionResult UnitEngList(string id)
        {
            Utils.setUserClass(this);
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("GovEngList", "EngAnalysisDecision");
            string tarUrl = redirectUrl.ToString() + "?m=" + (id== "縣市政府" ? "1" : "2");
            return Json(new { Url = tarUrl });
            return View("UnitEngList");
        }
        public ActionResult GovEngList(string m)
        {
            Utils.setUserClass(this);
            return View("GovEngList");
        }

        //其他補助&縣市政府 各單位在建工程件數
        public JsonResult GetEngCntForGovUnitList(int m)
        {
            List<EADEngCountVModel> engItems = iService.GetEngCntForGovUnit<EADEngCountVModel>(m);
            return Json(new
            {
                items = engItems
            });
        }
        //各單位在建工程件數 for 單位
        public JsonResult GetEngListForGovUnit(int m, int mode, string unit)
        {
            List<EADPrjXMLVModel> engItems;
            if (mode == 1)
                engItems = iService.GetBehindEngGovList<EADPrjXMLVModel>(m, unit);
            else
                engItems = iService.GetConstructionEngGovList<EADPrjXMLVModel>(m, unit);
            return Json(new
            {
                items = engItems
            });
        }

        //長官需要知道的事
        public JsonResult GetImportantEventSta()
        {
            List<EADEngStaVModel> engItems = iService.GetImportantEventSta<EADEngStaVModel>();
            return Json(new
            {
                items = engItems
            });
        }
        //長官需要知道的事 工程清單 for 項目
        public JsonResult GetImportantEventList(int mode)
        {
            List<EADPrjXMLVModel> engItems = iService.GetImportantEventList<EADPrjXMLVModel>(mode);
            var roleSeq = new SessionManager().GetUser().RoleSeq;
            return Json(new
            {
                items = engItems,
                roleSeq = roleSeq
            }); ;
        }

        //經費等級
        public JsonResult GetEngFeeLevelList(int mode)
        {
            List<EADPrjXMLVModel> engItems = iService.GetEngFeeLevelList<EADPrjXMLVModel>(mode);
            return Json(new
            {
                items = engItems
            });
        }
        public JsonResult GetEngFeeLevelSta()
        {
            List<EADEngStaVModel> engItems = iService.GetEngFeeLevelSta<EADEngStaVModel>();
            return Json(new
            {
                items = engItems
            });
        }
        public JsonResult GetEngFeeLevelOtherList(int mode)
        {
            List<EADPrjXMLVModel> engItems = iService.GetEngFeeLevelOtherList<EADPrjXMLVModel>(mode);
            return Json(new
            {
                items = engItems
            });
        }
        public JsonResult GetEngFeeLevelOtherSta()
        {
            List<EADEngStaVModel> engItems = iService.GetEngFeeLevelOtherSta<EADEngStaVModel>();
            return Json(new
            {
                items = engItems
            });
        }

        //各單位在建工程件數
        public JsonResult GetEngCntForUnitList()
        {
            List<EADEngCountVModel> engItems = iService.GetEngCntForUnit<EADEngCountVModel>();
            return Json(new
            {
                items = engItems
            });
        }
        //各單位在建工程件數 for 單位
        public JsonResult GetEngListForUnit(int mode, string unit)
        {
            List<EADPrjXMLVModel> engItems;
            if (mode == 1)
                engItems = iService.GetBehindEngList<EADPrjXMLVModel>(unit);
            else
                engItems = iService.GetConstructionEngList<EADPrjXMLVModel>(unit);
            return Json(new
            {
                items = engItems
            });
        }

        //工程標案清單
        public virtual JsonResult GetStateList()
        {
            List<SelectOptionModel> engList = iService.GetEngCreatedListState<SelectOptionModel>();
            return Json(new
            {
                items = engList
            });
        }

        public virtual JsonResult GetList(int pageRecordCount, int pageIndex, string state, string unitKeyword, string engKeyword)
        {
            if (string.IsNullOrEmpty(state))
                state = "";
            if (string.IsNullOrEmpty(unitKeyword))
                unitKeyword = "";
            else
                unitKeyword = string.Format("%{0}%", unitKeyword);
            if (string.IsNullOrEmpty(engKeyword))
                engKeyword = "";
            else
                engKeyword = string.Format("%{0}%", engKeyword);

            List<EADPrjXML2VModel> engList = new List<EADPrjXML2VModel>();
            int total = iService.GetEngCreatedListCount(state, unitKeyword, engKeyword);
            if (total > 0)
            {
                engList = iService.GetEngCreatedList<EADPrjXML2VModel>(pageRecordCount, pageIndex, state, unitKeyword, engKeyword);
            }
            return Json(new
            {
                pTotal = total,
                items = engList
            });
        }
        public JsonResult setImportListTag(int prjXMLSeq, int mode)
        {
            using(var context = new EQC_NEW_Entities() )
            {
                var exist = context.PrjXMLTag.Find(prjXMLSeq);
                if (exist != null)
                {
                    exist.ExcludeControlCode = exist.ExcludeControlCode ^ (int)Math.Pow(2, mode);
                }
                else 
                {
 
                    context.PrjXMLTag.Add(new PrjXMLTag { 
                        PrjXMLSeq = prjXMLSeq,
                        ExcludeControlCode = (int)Math.Pow(2, mode),
                        CarbonReductionCalTag = true
                    });
                }
                context.SaveChanges();
            }
            return Json(true);
        }
    }
}