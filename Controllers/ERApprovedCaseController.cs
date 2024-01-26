using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using EQC.ViewModel.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class ERApprovedCaseController : Controller
    {//工程提報 - 核定案件

        EngReportService engReportService = new EngReportService();

        public ActionResult Index()
        {
            Utils.setUserClass(this, 1);
            return View();
        }

        //工程清單
        public JsonResult GetList(int year, int unit, int subUnit, int rptType, int pageRecordCount, int pageIndex)
        {
            List<EngReportVModel> engList = new List<EngReportVModel>();
            int total = engReportService.GetEngListCount(year, unit, subUnit, rptType, 5);
            if (total > 0)
            {
                engList = engReportService.GetEngList<EngReportVModel>(year, unit, subUnit, rptType, pageRecordCount, pageIndex, 5);
            }
            return Json(new
            {
                pTotal = total,
                items = engList
            });
        }

        //更新工程 FOR 轉入核定案件
        public ActionResult UpdateEngReport(EngReportVModel m)
        {
            if (!String.IsNullOrEmpty(m.Seq.ToString()) && !String.IsNullOrEmpty(m.EngNo))
            {
                int state;
                state = engReportService.UpdateEngReportForAC(m);
                if (state == 0)
                {
                    return Json(new
                    {
                        result = 0,
                        msg = "儲存成功"
                    });
                }
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                });
            }

            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }

        //更新工程 FOR 轉入核定案件 - 轉入建立案件（單件）
        public ActionResult UpdateEngReportTransferRecord(EngReportVModel m)
        {
            if (!String.IsNullOrEmpty(m.Seq.ToString()) && !String.IsNullOrEmpty(m.EngNo))
            {
                int state;
                state = engReportService.UpdateEngReportForACT(m.Seq);
                if (state == 0)
                {
                    return Json(new
                    {
                        result = 0,
                        msg = "儲存成功"
                    });
                }
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                });
            }

            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }

        //更新工程 FOR 轉入核定案件 - 轉入建立案件
        public ActionResult UpdateEngReportTransfer(List<EngReportVModel> list)
        {
            try
            {
                int state = -1;
                foreach (var item in list)
                {
                    if (item.IsCheck)
                    {
                        if (string.IsNullOrEmpty(item.EngNo))
                        {
                            return Json(new
                            {
                                result = -1,
                                msg = "轉入失敗，勾選的案件未填寫工程案號。"
                            });
                        }

                        state = engReportService.UpdateEngReportForACT(item.Seq);
                        if (state < 0)
                        {
                            return Json(new
                            {
                                result = -1,
                                msg = "轉入失敗"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    result = -1,
                    msg = "轉入失敗"
                });
            }

            return Json(new
            {
                result = 0,
                msg = "轉入成功"
            });
        }

        public ActionResult ViewEng(int seq)
        {
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("EView", "ERApprovedCase");
            string tarUrl = redirectUrl.ToString() + "?id=" + seq;
            return Json(new { Url = tarUrl });
        }

        public ActionResult EView()
        {
            Utils.setUserClass(this, 2);
            List<VMenu> menu = new List<VMenu>();
            var link = new UrlHelper(Request.RequestContext).Action("Index", "ERApprovedCase");
            menu.Add(new VMenu() { Name = "核定案件", Url = link.ToString() });
            menu.Add(new VMenu() { Name = "核定案件", Url = "" });
            ViewBag.breadcrumb = menu;
            return View("EView");
        }

    }
}