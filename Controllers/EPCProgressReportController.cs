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
    public class EPCProgressReportController : Controller
    {//工程管理 - 進度報表
        EPCProgressReportService iService = new EPCProgressReportService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }
        //完成進度
        public JsonResult GetChartProgress(int id)
        {
            List<EPCTendeVModel> tenders = new EPCProgressManageService().GetEngLinkTenderBySeq<EPCTendeVModel>(id);//20220602
            //
            if (tenders.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "查無資料或開工日期錯誤"
                });

            }
            EPCTendeVModel tender = tenders[0];
            if (!tender.StartDate.HasValue || !tender.SchCompDate.HasValue)
            {
                return Json(new
                {
                    result = -1,
                    msg = "開工日期或預定完工日期錯誤"
                });

            }
            List<EPCProgressRSchVModel> schProgress = iService.GetSchProgress<EPCProgressRSchVModel>(id);
            List<EPCProgressRSchVModel> comProgress = iService.GetComProgress<EPCProgressRSchVModel>(id, tender.StartDate.Value);
            int days;
            decimal todayProgress = 0;
            decimal todayComProgress = 0;
            if (schProgress.Count > 0)
            {
                schProgress[0].dayProgress = schProgress[0].rate;
                for (int i = 1; i < schProgress.Count; i++)
                {
                    EPCProgressRSchVModel preM = schProgress[i - 1];
                    EPCProgressRSchVModel m = schProgress[i];
                    days = m.itemDate.Subtract(preM.itemDate).Days;
                    m.dayProgress = (m.rate - preM.rate) / (days);
                }
            }
            DateTime today = DateTime.Now.Date;//s20230625
            DateTime sDate = tender.StartDate.Value;
            decimal dayProgress = schProgress[0].dayProgress;
            DateTime maxDate = tender.SchCompDate.Value;
            if (comProgress.Count > 0 && comProgress[comProgress.Count-1].itemDate > maxDate)
            {
                maxDate = comProgress[comProgress.Count - 1].itemDate;
            }
            days = maxDate.Subtract(tender.StartDate.Value).Days + 1;
            ChartVModel progress = new ChartVModel();
            progress.series.Add(new ChartSeriesVModel() { name = "預定累計完成", color="blue" });//s20230623
            progress.series.Add(new ChartSeriesVModel() { name = "累計完成", color = "red" });//s20230623
            for (int i = 0; i < days; i++)
            {
                progress.categories.Add(sDate.ToString("yyyy-M-d"));
                //預定累計完成
                todayProgress += dayProgress;
                progress.series[0].data.Add(todayProgress);
                if (schProgress.Count > 0 && sDate == schProgress[0].itemDate)
                {
                    schProgress.RemoveAt(0);
                    if (schProgress.Count > 0)
                        dayProgress = schProgress[0].dayProgress;
                    else
                        dayProgress = 0;
                }
                //累計完成
                if (comProgress.Count > 0 && sDate == comProgress[0].itemDate)
                {
                    todayComProgress += comProgress[0].rate;
                    comProgress.RemoveAt(0);
                }
                if (today.Subtract(sDate).Days >= 0)
                {//今日以前才呈現 s20230625
                    progress.series[1].data.Add(todayComProgress);
                }

                sDate = sDate.AddDays(1);
            }

            List<SchProgressHeaderHistoryProgressModel> initProgress = iService.GetIinitSchProgress<SchProgressHeaderHistoryProgressModel>(id);
            if (initProgress.Count > 0)
            {
                progress.series.Insert(0, new ChartSeriesVModel() { name = "初始預定累計完成", color= "green" });//s20230623
                foreach (SchProgressHeaderHistoryProgressModel m in initProgress)
                {
                    progress.series[0].data.Add(m.SchProgress);
                }
            }
            //
            return Json(new
            {
                result = 0,
                item = progress
            });
        }
        public JsonResult GetChart2(int id)
        {
            List<EPCProgressReportChart2VModel> items = iService.GetChart2<EPCProgressReportChart2VModel>(id);
            //s20230228
            List<EPCProgressReportChart2VModel> progress = iService.GetEngProgress<EPCProgressReportChart2VModel>(id, DateTime.Now);
            //
            foreach(EPCProgressReportChart2VModel m in items)
            {//s20230426
                foreach(EPCProgressReportChart2VModel p in progress)
                {
                    if (m.OrderNo == p.OrderNo && m.Description == p.Description)
                    {
                        m.SchProgress = p.SchProgress;
                        m.ActualProgress = p.ActualProgress;
                        progress.Remove(p);
                        break;
                    }
                    
                }
            }
            return Json(new
            {
                result = 0,
                items = items
            });
        }
        //機具出工
        public JsonResult GetChartEquipmen(int id)
        {
            List<EPCTendeVModel> tenders = new EPCProgressManageService().GetEngLinkTenderBySeq<EPCTendeVModel>(id);//20220602
            //
            if (tenders.Count == 0 || !tenders[0].StartDate.HasValue)
            {
                return Json(new
                {
                    result = -1,
                    msg = "查無資料或開工日期錯誤"
                });

            }
            EPCTendeVModel tender = tenders[0];

            List<EPCProgressRChartItemVModel> items = iService.GetChart3Equipment<EPCProgressRChartItemVModel>(id, tender.StartDate.Value);
            string iName = "";
            DateTime maxDate = DateTime.Parse("1911/1/1");
            ChartVModel equipmen = new ChartVModel();
            foreach (EPCProgressRChartItemVModel m in items)
            {
                if(m.itemName != iName)
                {
                    iName = m.itemName;
                    equipmen.seriesName.Add(iName);
                    equipmen.series.Add(new ChartSeriesVModel() { name= iName });
                }
                if(maxDate < m.ItemDate)
                {
                    maxDate = m.ItemDate;
                }
            }
            DateTime sDate = tender.StartDate.Value;
            int days = maxDate.Subtract(tender.StartDate.Value).Days;
            foreach (ChartSeriesVModel m in equipmen.series)
            {
                for (int i = 0; i < days; i++)
                {
                    equipmen.categories.Add(sDate.ToString("yyyy-M-d"));
                    sDate = sDate.AddDays(1);
                    m.data.Add(0);
                }
            }
            foreach (EPCProgressRChartItemVModel m in items)
            {
                int inx = equipmen.seriesName.IndexOf(m.itemName);
                if(inx != -1)
                {
                    days = m.ItemDate.Subtract(tender.StartDate.Value).Days;
                    if(days > -1 && days < equipmen.series[inx].data.Count)
                    equipmen.series[inx].data[days] = m.quantity;
                }
            }
            //
            return Json(new
            {
                result = 0,
                item = equipmen
            });
        }
        //機具碳排量 s20230502
        public JsonResult GetChartEquipmenKgCo2e(int id)
        {
            List<EPCTendeVModel> tenders = new EPCProgressManageService().GetEngLinkTenderBySeq<EPCTendeVModel>(id);//20220602
            //
            if (tenders.Count == 0 || !tenders[0].StartDate.HasValue)
            {
                return Json(new
                {
                    result = -1,
                    msg = "查無資料或開工日期錯誤"
                });

            }
            EPCTendeVModel tender = tenders[0];

            List<EPCProgressRChartItemVModel> items = iService.GetChart4Equipment<EPCProgressRChartItemVModel>(id, tender.StartDate.Value);
            string iName = "";
            DateTime maxDate = DateTime.Parse("1911/1/1");
            ChartVModel equipmen = new ChartVModel();
            foreach (EPCProgressRChartItemVModel m in items)
            {
                if (m.itemName != iName)
                {
                    iName = m.itemName;
                    equipmen.seriesName.Add(iName);
                    equipmen.series.Add(new ChartSeriesVModel() { name = iName });
                }
                if (maxDate < m.ItemDate)
                {
                    maxDate = m.ItemDate;
                }
            }
            DateTime sDate = tender.StartDate.Value;
            int days = maxDate.Subtract(tender.StartDate.Value).Days;
            foreach (ChartSeriesVModel m in equipmen.series)
            {
                for (int i = 0; i < days; i++)
                {
                    equipmen.categories.Add(sDate.ToString("yyyy-M-d"));
                    sDate = sDate.AddDays(1);
                    m.data.Add(0);
                }
            }
            foreach (EPCProgressRChartItemVModel m in items)
            {
                int inx = equipmen.seriesName.IndexOf(m.itemName);
                if (inx != -1)
                {
                    days = m.ItemDate.Subtract(tender.StartDate.Value).Days;
                    if (days > -1 && days < equipmen.series[inx].data.Count)
                        equipmen.series[inx].data[days] = m.quantity;
                }
            }
            //
            return Json(new
            {
                result = 0,
                item = equipmen
            });
        }
        //人員出工
        public JsonResult GetChartPerson(int id)
        {
            List<EPCTendeVModel> tenders = new EPCProgressManageService().GetEngLinkTenderBySeq<EPCTendeVModel>(id);//20220602
            //
            if (tenders.Count == 0 || !tenders[0].StartDate.HasValue)
            {
                return Json(new
                {
                    result = -1,
                    msg = "查無資料或開工日期錯誤"
                });

            }
            EPCTendeVModel tender = tenders[0];

            List<EPCProgressRChartItemVModel> items = iService.GetChart3Person<EPCProgressRChartItemVModel>(id, tender.StartDate.Value);
            string iName = "";
            DateTime maxDate = DateTime.Parse("1911/1/1");
            ChartVModel person = new ChartVModel();
            foreach (EPCProgressRChartItemVModel m in items)
            {
                if (m.itemName != iName)
                {
                    iName = m.itemName;
                    person.seriesName.Add(iName);
                    person.series.Add(new ChartSeriesVModel() { name = iName });
                }
                if (maxDate < m.ItemDate)
                {
                    maxDate = m.ItemDate;
                }
            }
            DateTime sDate = tender.StartDate.Value;
            int days = maxDate.Subtract(tender.StartDate.Value).Days;
            foreach (ChartSeriesVModel m in person.series)
            {
                for (int i = 0; i < days; i++)
                {
                    person.categories.Add(sDate.ToString("yyyy-M-d"));
                    sDate = sDate.AddDays(1);
                    m.data.Add(0);
                }
            }
            foreach (EPCProgressRChartItemVModel m in items)
            {
                int inx = person.seriesName.IndexOf(m.itemName);
                if (inx != -1)
                {
                    days = m.ItemDate.Subtract(tender.StartDate.Value).Days;
                    if(days > -1 && days < person.series[inx].data.Count)
                        person.series[inx].data[days] = m.quantity;
                }
            }
            //
            return Json(new
            {
                result = 0,
                item = person
            });
        }
        //工程標案
        public JsonResult GetTrender(int id)
        {
            List<EPCTendeVModel> tender = new EPCProgressManageService().GetEngLinkTenderBySeq<EPCTendeVModel>(id);//20220602
            //
            if (tender.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "查無資料"
                });

            }
            else
            {
                return Json(new
                {
                    result = 0,
                    item = tender[0],
                    stopDays = iService.GetStopWorkDays(id)
                });
            }
        }
    }
}