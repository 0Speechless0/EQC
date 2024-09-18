using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class ERAnnualFundingReviewController : Controller
    {//工程提報 - 年度經費檢討會議

        EngReportService engReportService = new EngReportService();

        public ActionResult Index()
        {
            Utils.setUserClass(this, 1);
            return View();
        }

        //工程清單
        public JsonResult GetList(int year, int unit, int subUnit, int rptType, int pageRecordCount, int pageIndex, string keyWord = null)
        {
            List<EngReportVModel> engList = new List<EngReportVModel>();
            List<EngReportEstimatedCostVModel> engEREC = new List<EngReportEstimatedCostVModel>();

            int total = engReportService.GetEngListCount(year, unit, subUnit, rptType, 4, keyWord);
            if (total > 0)
            {
                engList = engReportService.GetEngList<EngReportVModel>(year, unit, subUnit, rptType, pageRecordCount, pageIndex, 4, keyWord);

                int iCount = 0;
                foreach (EngReportVModel item in engList) 
                {
                    engEREC = engReportService.GetEngReportEstimatedCost<EngReportEstimatedCostVModel>(item.Seq, Convert.ToInt16(item.RptYear));
                    if(engEREC.Count>0) 
                    {
                        if(engList[iCount].Expenditure == null)
                            engList[iCount].Expenditure = engEREC[0].Price;

                    }
                    iCount++;
                }
            }
            return Json(new
            {
                pTotal = total,
                items = engList
            });
        }

        //更新工程提報-年度經費檢討會議
        public ActionResult UpdateEngReport(EngReportVModel m)
        {
            int state;
            var item = engReportService.UpdateEngReportForAF(m);
            if (item != null)
            {
                return Json(new
                {
                    result = item,
                    msg = "儲存成功"
                });
            }

            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }

        public ActionResult ViewEng(int seq)
        {
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("EView", "ERAnnualFundingReview");
            string tarUrl = redirectUrl.ToString() + "?id=" + seq;
            return Json(new { Url = tarUrl });
        }

        public ActionResult EView()
        {
            Utils.setUserClass(this, 2);
            List<VMenu> menu = new List<VMenu>();
            var link = new UrlHelper(Request.RequestContext).Action("Index", "ERAnnualFundingReview");
            menu.Add(new VMenu() { Name = "年度經費檢討會議", Url = link.ToString() });
            menu.Add(new VMenu() { Name = "年度經費檢討會議", Url = "" });
            ViewBag.breadcrumb = menu;
            return View("EView");
        }
    }
}