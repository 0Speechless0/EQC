using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class SIRIApproveController : SamplingInspectionRecImproveController
    {
        private ConstCheckRecImproveApproveService constCheckRecImproveApproveService = new ConstCheckRecImproveApproveService();
        override public ActionResult Index()
        {
            //ViewBag.Title = "抽查缺失改善";
            Utils.setUserClass(this);
            return View();
        }

        override public ActionResult EditEng(int seq)
        {
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Edit", "SIRIApprove");
            string tarUrl = redirectUrl.ToString() + "?id=" + seq;
            return Json(new { Url = tarUrl });
        }
        override public ActionResult Edit()
        {
            //ViewBag.Title = "監造計畫-編輯";
            Utils.setUserClass(this);
            List<VMenu> menu = new List<VMenu>();
            var link = new UrlHelper(Request.RequestContext).Action("Index", "SIRIApprove");
            menu.Add(new VMenu() { Name = "缺失改善審查", Url = link.ToString() });
            menu.Add(new VMenu() { Name = "缺失改善審查-編輯", Url = "" });
            ViewBag.breadcrumb = menu;

            return View();
        }

        //標案年分
        override public JsonResult GetYearOptions()
        {
            List<EngYearVModel> years = constCheckRecImproveApproveService.GetEngYearList();
            return Json(years);
        }
        //依年分取執行機關
        override public JsonResult GetUnitOptions(string year)
        {
            List<EngExecUnitsVModel> items = constCheckRecImproveApproveService.GetEngExecUnitList(year);
            return Json(items);
        }
        //依年分,機關取執行單位
        override public JsonResult GetSubUnitOptions(string year, int parentSeq)
        {
            List<EngExecUnitsVModel> items = constCheckRecImproveApproveService.GetEngExecSubUnitList(year, parentSeq);
            EngExecUnitsVModel m = new EngExecUnitsVModel();
            m.UnitSeq = -1;
            m.UnitName = "全部單位";
            items.Insert(0, m);
            return Json(items);
        }

        //工程名稱清單
        override public JsonResult GetEngNameList(string year, int unit, int subUnit, int engMain)
        {
            List<EngNameOptionsVModel> engNames = new List<EngNameOptionsVModel>();
            engNames = constCheckRecImproveApproveService.GetEngCreatedNameList<EngNameOptionsVModel>(year, unit, subUnit);
            engNames.Sort((x, y) => x.CompareTo(y));
            return Json(new
            {
                engNameItems = engNames
            });
        }
        //分項工程清單
        override public JsonResult GetSubEngNameList(int engMain)
        {
            List<EngConstructionOptionsVModel> subEngNames = constCheckRecImproveApproveService.GetSubEngList<EngConstructionOptionsVModel>(engMain);
            EngConstructionOptionsVModel m = new EngConstructionOptionsVModel();
            m.Seq = -1;
            m.ItemName = "全部分項工程";
            subEngNames.Insert(0, m);
            //    engNames.Sort((x, y) => x.CompareTo(y));
            return Json(subEngNames);
        }

        //分項工程清單
        public JsonResult GetList(string year, int unit, int subUnit, int engMain, int subEngMain, int pageRecordCount, int pageIndex)
        {
            List<EngConstructionListVModel> engList = new List<EngConstructionListVModel>();
            int total = constCheckRecImproveApproveService.GetEngCreatedListCount(year, unit, subUnit, engMain, subEngMain);
            if (total > 0)
            {
                engList = constCheckRecImproveApproveService.GetEngCreatedList<EngConstructionListVModel>(year, unit, subUnit, engMain, subEngMain, pageRecordCount, pageIndex);
            }
            return Json(new
            {
                pTotal = total,
                items = engList,
            });
        }
        public JsonResult GetListLightly(int engMain, int subEngMain, int pageRecordCount, int pageIndex)
        {
            List<EngConstructionListVModel> engList = new List<EngConstructionListVModel>();
            int total = constCheckRecImproveApproveService.GetEngCreatedListCount(engMain, subEngMain);
            if(total > 0)
            {
                engList = constCheckRecImproveApproveService.GetEngCreatedList<EngConstructionListVModel>(engMain, subEngMain, pageRecordCount, pageIndex);
            }
            return Json(new
            {
                pTotal = total,
                items = engList,
            });
        }
        //for edit ===========================================================
        //已有檢驗單之檢驗項目
        override public JsonResult GetRecCheckTypeOption(int id)
        {
            List<SelectIntOptionModel> items = constCheckRecImproveApproveService.GetRecCheckTypeOption<SelectIntOptionModel>(id);
            return Json(items);
        }
        //檢驗單清單 單一項目
        override public JsonResult GetRecOptionByCheckType(int constructionSeq, int checkTypeSeq)
        {
            List<ConstCheckRecOptionVModel> items = constCheckRecImproveApproveService.GetListByCheckType<ConstCheckRecOptionVModel>(constructionSeq, checkTypeSeq);
            return Json(items);
        }
        //審核更新 不符合事項報告
        public JsonResult UpdateReportApprove(ConstCheckRecImproveVModel report)
        {
            report.updateDate();
            if (constCheckRecImproveApproveService.Update(report)==0)
            {
                return Json(new
                {
                    result = -1,
                    message = "儲存失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    formConfirm = report.FormConfirm,
                    message = "儲存成功"
                });
            }
        }
        //審核更新 NCR
        public JsonResult UpdateNCRApprove(NcrVModel ncr)
        {
            ncr.updateDate();
            if (new NcrService().UpdateApprove(ncr)==0)
            {
                return Json(new
                {
                    result = -1,
                    message = "儲存失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    formConfirm = ncr.FormConfirm,
                    message = "儲存成功"
                });
            }
        }
    }
}