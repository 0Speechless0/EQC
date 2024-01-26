using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EADRiskController : Controller
    {//水利工程履約風險分析
        EADRiskService iService = new EADRiskService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }
        //工程清單
        public JsonResult GetList(List<string> units, int sYear, int eYear, int mode, int pageRecordCount, int pageIndex)
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
            //
            int total = 0;
            List<EADRiskEngVModel> engList = new List<EADRiskEngVModel>();
            switch (mode)
            {
                case 1://巨額工程落後2個百分比
                    total = iService.GetA1Count(unitList, sYear, eYear);
                    engList = iService.GetA1(unitList, sYear, eYear, pageRecordCount, pageIndex);
                    break;
                case 2://停工案件
                    total = iService.GetA2Count(unitList, sYear, eYear);
                    engList = iService.GetA2(unitList, sYear, eYear, pageRecordCount, pageIndex);
                    break;
                case 3://完工3個月未完成驗收工程
                    total = iService.GetA3Count(unitList, sYear, eYear);
                    engList = iService.GetA3(unitList, sYear, eYear, pageRecordCount, pageIndex);
                    break;
                case 4://完工4個月未完成驗收工程
                    total = iService.GetA4Count(unitList, sYear, eYear);
                    engList = iService.GetA4(unitList, sYear, eYear, pageRecordCount, pageIndex);
                    break;
                case 5://完工5個月未完成驗收工程
                    total = iService.GetA5Count(unitList, sYear, eYear);
                    engList = iService.GetA5(unitList, sYear, eYear, pageRecordCount, pageIndex);
                    break;
                case 6://超過4個月未估驗工程
                    total = iService.GetA6Count(unitList, sYear, eYear);
                    engList = iService.GetA6(unitList, sYear, eYear, pageRecordCount, pageIndex);
                    break;
                case 7://超過5個月未估驗工程
                    total = iService.GetA7Count(unitList, sYear, eYear);
                    engList = iService.GetA7(unitList, sYear, eYear, pageRecordCount, pageIndex);
                    break;
                case 8://終止解約工程
                    total = iService.GetA8Count(unitList, sYear, eYear);
                    engList = iService.GetA8(unitList, sYear, eYear, pageRecordCount, pageIndex);
                    break;
                case 9://5千萬以上未達2億元(落後8%以上)
                    total = iService.GetA9Count(unitList, sYear, eYear);
                    engList = iService.GetA9(unitList, sYear, eYear, pageRecordCount, pageIndex);
                    break;
                case 10://未達查核金額工程(落後8%以上)
                    total = iService.GetA10Count(unitList, sYear, eYear);
                    engList = iService.GetA10(unitList, sYear, eYear, pageRecordCount, pageIndex);
                    break;
            }
            return Json(new
            {
                total = total,
                items = engList
            });
        }

        //下載
        public ActionResult Download(List<string> units, int sYear, int eYear, int mode)
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
            //unitList = "";
            //
            string title = "";
            List<EADRiskEngVModel> engList = new List<EADRiskEngVModel>();
            switch (mode)
            {
                case 1:
                    title = "巨額工程落後2個百分比";
                    engList = iService.GetA1(unitList, sYear, eYear, 9999, 1);
                    break;
                case 2:
                    title = "停工案件";
                    engList = iService.GetA2(unitList, sYear, eYear, 9999, 1);
                    break;
                case 3:
                    title = "完工3個月未完成驗收工程";
                    engList = iService.GetA3(unitList, sYear, eYear, 9999, 1);
                    break;
                case 4:
                    title = "完工4個月未完成驗收工程";
                    engList = iService.GetA4(unitList, sYear, eYear, 9999, 1);
                    break;
                case 5:
                    title = "完工5個月未完成驗收工程";
                    engList = iService.GetA5(unitList, sYear, eYear, 9999, 1);
                    break;
                case 6:
                    title = "超過4個月未估驗工程";
                    engList = iService.GetA6(unitList, sYear, eYear, 9999, 1);
                    break;
                case 7:
                    title = "超過5個月未估驗工程";
                    engList = iService.GetA7(unitList, sYear, eYear, 9999, 1);
                    break;
                case 8:
                    title = "終止解約工程";
                    engList = iService.GetA8(unitList, sYear, eYear, 9999, 1);
                    break;
                case 9:
                    title = "5千萬以上未達2億元(落後8%個百分比以上)";
                    engList = iService.GetA9(unitList, sYear, eYear, 9999, 1);
                    break;
                case 10:
                    title = "未達查核金額工程(落後8%個百分比以上)";
                    engList = iService.GetA10(unitList, sYear, eYear, 9999, 1);
                    break;
            }
            return CreateExcel(engList, title);

            return Json(new
            {
                result = -1,
                message = "請求錯誤"
            });
        }
        private ActionResult CreateExcel(List<EADRiskEngVModel> engList, string title)
        {
            string filename = CopyTemplateFile("水利工程履約風險分析.xlsx", ".xlsx");
            Dictionary<string, Worksheet> dict = new Dictionary<string, Worksheet>();
            Microsoft.Office.Interop.Excel.Application appExcel = null;
            Workbook workbook = null;
            //開啟 Excel 檔案
            try
            {
                appExcel = new Microsoft.Office.Interop.Excel.Application();
                workbook = appExcel.Workbooks.Open(filename);

                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    fillSheet(worksheet, engList);
                    break;
                }

                workbook.Save();
                workbook.Close();
                appExcel.Quit();

                return DownloadFile(filename, title+".xlsx");
            }
            catch (Exception e)
            {
                if (workbook != null) workbook.Close(false);
                if (appExcel != null) appExcel.Quit();
                return Json(new
                {
                    result = -1,
                    message = "Excel 製表失敗"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        private void fillSheet(Worksheet sheet, List<EADRiskEngVModel> engList)
        {
            if (sheet == null) return;

            int row = 2;
            foreach (EADRiskEngVModel m in engList)
            {
                sheet.Cells[row, 1] = m.BelongPrj;
                sheet.Cells[row, 2] = m.ExecUnitName;
                sheet.Cells[row, 3] = m.TenderName;
                sheet.Cells[row, 4] = m.BidAmount;
                sheet.Cells[row, 5] = m.PDAccuScheProgress;
                sheet.Cells[row, 6] = m.PDAccuActualProgress;
                sheet.Cells[row, 7] = m.DiffProgress;
                sheet.Cells[row, 8] = m.BDAnalysis;
                sheet.Cells[row, 9] = m.BDSolution;
                row++;
            }
        }
        private ActionResult DownloadFile(string fullPath, string fileExt)
        {
            if (System.IO.File.Exists(fullPath))
            {
                Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", "水利工程履約風險分析-" + fileExt);
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }
        private string CopyTemplateFile(string filename, string extFile)
        {
            string tempFile = Utils.GetTempFile(extFile);
            string srcFile = Path.Combine(Utils.GetTemplateFilePath(), filename);
            System.IO.File.Copy(srcFile, tempFile);
            return tempFile;
        }

        //標案年分
        public JsonResult GetYearOptions()
        {
            List<EngYearVModel> items = iService.GetEngYearList();
            return Json(items);
        }
    }
}