using EQC.Services;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace EQC.Common
{
    public class CalUtils : Controller
    {//工程月曆顯示 s20230526
        //
        public JsonResult GetExtensionList(int id)
        {
            return Json(new EPCCalendarService().GetExtensionByPrjSeq<EPCReportExtensionVModel>(id));
        }
        //停復工清單
        public JsonResult GetWorkList(int id)
        {
            return Json(new EPCCalendarService().GetWorkByPrjSeq<EPCReportWorkVModel>(id));
        }
        //核定公文下載
        public ActionResult DownloadFile(int id, int mode, string fn)
        {
            string filePath = Utils.GetTenderFolder(id);

            string uniqueFileName = fn;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
                if (System.IO.File.Exists(fullPath))
                {
                    int inx = uniqueFileName.LastIndexOf(".");
                    Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", String.Format("{0}-核定公文檔案.{1}", mode == 1 ? "停工" : "復工", uniqueFileName.Substring(inx)));
                }
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }
        //核定公文
        public string SaveFile(HttpPostedFileBase file, int engMainSeq, string fileHeader)
        {
            string filePath = Utils.GetTenderFolder(engMainSeq);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string originFileName = file.FileName.ToString().Trim();
            int inx = originFileName.LastIndexOf(".");
            string uniqueFileName = String.Format("{0}{1}{2}", fileHeader, Guid.NewGuid(), originFileName.Substring(inx));

            string fullPath = Path.Combine(filePath, uniqueFileName);
            file.SaveAs(fullPath);

            return uniqueFileName;
        }
        public void DelFile(int engMainSeq, string fileName)
        {
            try
            {
                string filePath = Utils.GetTenderFolder(engMainSeq);
                if (Directory.Exists(filePath))
                {
                    if (fileName != null && fileName.Length > 0)
                    {
                        System.IO.File.Delete(Path.Combine(filePath, fileName));
                    }
                }
            }
            catch { }
        }
        public JsonResult GetCalDayInfo(int id, DateTime fromDate, int mode)
        {
            List<EPCTendeVModel> tenders = new EPCProgressManageService().GetEngLinkTenderBySeq<EPCTendeVModel>(id);
            if (tenders.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程錯誤"
                });
            }
            EPCTendeVModel tender = tenders[0];
            if (!tender.StartDate.HasValue || !tender.SchCompDate.HasValue)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程日期錯誤"
                });
            }
            //日期範圍
            DateTime startDate = tender.StartDate.Value;
            if (!String.IsNullOrEmpty(tender.ActualStartDate))//實際開工日期
            {
                DateTime? dt = Utils.StringChs2DateToDateTime(tender.ActualStartDate);
                if (dt != null) startDate = dt.Value;
            }
            if (fromDate > startDate) startDate = fromDate;

            DateTime monthEndDate = fromDate.AddMonths(1).AddDays(-1);//月底
            DateTime endDate = monthEndDate;
            if (endDate > tender.SchCompDate.Value) endDate = tender.SchCompDate.Value; //預計完工日期

            DateTime nowDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));//今日
            if (endDate >= nowDate) endDate = nowDate.AddDays(-1);

            Dictionary<string, EPCCalInfoVModel> cals = new Dictionary<string, EPCCalInfoVModel>();
            //未填寫
            DateTime bDate = startDate;
            while (bDate <= endDate)
            {
                cals.Add(bDate.ToString("yyyy-MM-dd"), new EPCCalInfoVModel() { Mode = 0, ItemDate = bDate });
                bDate = bDate.AddDays(1);
            }
            //停工
            List<EPCReportWorkVModel> stopList = new EPCCalendarService().GetWorkByPrjSeq<EPCReportWorkVModel>(id);
            foreach (EPCReportWorkVModel m in stopList)
            {
                bDate = m.SStopWorkDate;
                DateTime eDate = m.BackWorkDate.HasValue ? m.BackWorkDate.Value.AddDays(-1) : m.EStopWorkDate;//s20230526
                while (bDate <= eDate)
                {
                    string key = bDate.ToString("yyyy-MM-dd");
                    if (cals.ContainsKey(key))
                    {
                        cals[key].Mode = 1;
                    }
                    else if (bDate <= monthEndDate)
                    {
                        cals.Add(key, new EPCCalInfoVModel() { Mode = 1, ItemDate = bDate });
                    }
                    bDate = bDate.AddDays(1);
                }
            }
            //已填寫
            List<EPCCalInfoVModel> fillList = new SupDailyReportService().GetCalendarInfo<EPCCalInfoVModel>(id, fromDate, mode);
            foreach (EPCCalInfoVModel m in fillList)
            {
                if (cals.ContainsKey(m.DateStr))
                {
                    cals[m.DateStr].Mode = 2;
                }
                else
                {
                    cals.Add(m.ItemDate.ToString("yyyy-MM-dd"), new EPCCalInfoVModel() { Mode = 2, ItemDate = m.ItemDate });
                }
            }
            List<EPCCalInfoVModel> calList = new List<EPCCalInfoVModel>();
            foreach (string key in cals.Keys)
            {
                calList.Add(cals[key]);
            }
            UserInfo userInfo = Utils.getUserInfo();
            bool admin = userInfo.IsAdmin || userInfo.IsDepartmentAdmin || userInfo.IsDepartmentUser;//s20230417
            return Json(new
            {
                result = 0,
                items = calList,
                admin = admin
            });
        }
    }
}