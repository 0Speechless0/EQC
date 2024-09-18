using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using EQC.Common;
using EQC.EDMXModel;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class CEFactorController : Controller
    {//碳排係數維護
        CarbonEmissionFactorService iService = new CarbonEmissionFactorService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Edit");
        }
        public ActionResult CCT()
        {
            Utils.setUserClass(this);
            return View("CCT");
        }

        //**** 碳係數指引維護 ***** shioulo 20230202
        //清單
        public JsonResult GetSubRecords(int pageRecordCount, int pageIndex, int id)
        {
            List<CarbonEmissionFactorDetailModel> lists = new List<CarbonEmissionFactorDetailModel>();
            CarbonEmissionFactorDetailModel unitItem = new CarbonEmissionFactorDetailModel() { Seq=-1};
            int total = iService.GetSubListCount(id);
            if (total > 0)
            {
                lists = iService.GetSubList<CarbonEmissionFactorDetailModel>(pageRecordCount, pageIndex, id);
                List<CarbonEmissionFactorDetailModel> ulists = iService.GetSubUnitItem<CarbonEmissionFactorDetailModel>(id);
                if(ulists.Count == 1)
                {
                    unitItem = ulists[0];
                }
            }
            return Json(new
            {
                result = 0,
                pTotal = total,
                items = lists,
                unitItem = unitItem
            });
        }
        //更新
        public JsonResult UpdateSubRecord(CarbonEmissionFactorDetailModel m)
        {
            int state;
            if (m.Seq == -1)
                state = iService.AddSubRecord(m);
            else
                state = iService.UpdateSubRecord(m);
            if (state == 0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "儲存成功"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }
        //刪除
        public JsonResult DelSubRecord(CarbonEmissionFactorDetailModel m)
        {
            int state = iService.DelSubRecord(m);
            if (state == 0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "刪除成功"
                }); ;
            }
            return Json(new
            {
                result = -1,
                msg = "刪除失敗"
            });
        }
        public JsonResult NewSubRecord()
        {
            return Json(new
            {
                result = 0,
                item = new CarbonEmissionFactorDetailModel() { Seq = -1 }
            });
        }

        //******* ******
        //水利署碳報表 s20230529

        public ActionResult GetCarbonReoprt(int year, int unit, int subUnit)
        {
            var list = iService.GetCarbonReoprtGroup<CarbonEmissionReportVModel>(year, unit, subUnit);
            return Json(new
            {
                result = 0,
                items = iService.GetCarbonReoprtGroup<CarbonEmissionReportVModel>(year, unit, subUnit)
            });
        }
        public ActionResult GetCarbonReoprtDetail(int year, int unit, int subUnit, bool award = false)
        {
            return Json(new
            {
                result = 0,
                items = iService.GetCarbonReoprtDetail<CarbonEmissionReportVModel>(year, unit, subUnit, award)
            });
        }

        //碳報表-總表
        public ActionResult dnCReportG(int year, int unit, int subUnit)
        {
            string filename = Utils.CopyTemplateFile("統計報表-工程碳排量資料表_總表V2.xlsx", ".xlsx");
            Dictionary<string, Microsoft.Office.Interop.Excel.Worksheet> dict = new Dictionary<string, Microsoft.Office.Interop.Excel.Worksheet>();
            Microsoft.Office.Interop.Excel.Application appExcel = null;
            Microsoft.Office.Interop.Excel.Workbook workbook = null;
            //開啟 Excel 檔案
            try
            {
                appExcel = new Microsoft.Office.Interop.Excel.Application();
                workbook = appExcel.Workbooks.Open(filename);

                foreach (Microsoft.Office.Interop.Excel.Worksheet worksheet in workbook.Worksheets)
                {
                    dict.Add(worksheet.Name, worksheet);
                }
                Microsoft.Office.Interop.Excel.Worksheet sheet = dict["總表"];
                List<CarbonEmissionReportVModel> items = iService.GetCarbonReoprtGroup<CarbonEmissionReportVModel>(year, unit, subUnit);
                List<CarbonEmissionReportVModel> dItems = 
                    iService.GetCarbonReoprtDetail<CarbonEmissionReportVModel>(year, unit, subUnit)
                    .OrderBy(r => r.OrderNo).ToList();

                //sheet.Cells[1, 3] = String.Format("附件1-{0}年度工程碳排量資料表總表", year);
                sheet.Cells[1, 6] = String.Format("附件1-{0}年度工程碳排量資料表總表", year);
                //sheet.Cells[1, 13] = String.Format("資料統計至{0}.{1}.{2}", DateTime.Now.Year-1911, DateTime.Now.Month, DateTime.Now.Day);
                sheet.Cells[1, 15] = String.Format("{0}.{1}.{2}", DateTime.Now.Year-1911, DateTime.Now.Month, DateTime.Now.Day);
                int row = 3, inx = 1;
                Microsoft.Office.Interop.Excel.Range excelRange = sheet.Rows[4];
                for (int i = 1; i < items.Count; i++)
                {
                    excelRange.Insert(Microsoft.Office.Interop.Excel.XlDirection.xlDown, Microsoft.Office.Interop.Excel.XlInsertFormatOrigin.xlFormatFromLeftOrAbove);
                }
                CarbonEmissionReportVModel total = new CarbonEmissionReportVModel() 
                {
                    engCnt = 0,
                    TotalBudget = 0,
                    CarbonDemandQuantity = 0,
                    ApprovedCarbonQuantity = 0,
                    awardCnt = 0,
                    AwardTotalBudget = 0,
                    Co2Total = 0,
                    GreenFunding = 0,
                    greenFundingRate = 0,
                    F1108Area = 0,
                    F1109Length = 0,
                    co2TotalRate = 0,
                    Co2TotalItem = 0,
                    Co2TotalItemAll = 0
                };
                decimal Total02931 = 0;
                decimal Total02932 = 0;

                foreach (CarbonEmissionReportVModel m in items)
                {
                    sheet.Cells[row, 1] = inx;
                    sheet.Cells[row, 2] = m.execUnitName;
                    sheet.Cells[row, 3] = m.engCnt;
                    sheet.Cells[row, 4] = m.TotalBudget;
                    sheet.Cells[row, 5] = m.CarbonDemandQuantity;
                    sheet.Cells[row, 6] = m.ApprovedCarbonQuantity;
                    sheet.Cells[row, 7] = m.awardCnt;
                    sheet.Cells[row, 8] = m.Co2Total;
                    sheet.Cells[row, 9] = m.GreenFunding;
                    sheet.Cells[row, 10] = m.greenFundingRate ;
                    sheet.Cells[row, 11] = m.Tree02931Total;
                    sheet.Cells[row, 12] = m.Tree02932Total;
                    sheet.Cells[row, 13] = m.F1108Area;
                    sheet.Cells[row, 14] = m.F1109Length;
                    sheet.Cells[row, 15] = m.co2TotalRate ;
                    //
                    Total02931 += m.Tree02931Total ?? 0;
                    Total02932 += m.Tree02932Total ?? 0;
                    total.Co2TotalItem += (m.Co2TotalItem ?? 0);
                    total.AwardTotalBudget += m.AwardTotalBudget ?? 0;
                    total.engCnt += m.engCnt.HasValue ? m.engCnt : 0;
                    total._TotalBudget += m._TotalBudget.HasValue ? m._TotalBudget : 0;
                    total._CarbonDemandQuantity += m._CarbonDemandQuantity.HasValue ? m._CarbonDemandQuantity : 0;
                    total._ApprovedCarbonQuantity += m._ApprovedCarbonQuantity.HasValue ? m._ApprovedCarbonQuantity : 0;
                    total.awardCnt += m.awardCnt.HasValue ? m.awardCnt : 0;
                    total._Co2Total += m._Co2Total.HasValue ? m._Co2Total : 0;
                    total.GreenFunding += m.GreenFunding.HasValue ? m.GreenFunding : 0;
                    total.greenFundingRate += m.greenFundingRate.HasValue ? m.greenFundingRate : 0;
                    total.F1108Area += m.F1108Area.HasValue ? m.F1108Area : 0;
                    total.F1109Length += m.F1109Length.HasValue ? m.F1109Length : 0;
                    total.Co2TotalItemAll += m.Co2TotalItemAll ;

                    inx++;
                    row++;
                }
                //
                sheet.Cells[row, 3] = total.engCnt;
                sheet.Cells[row, 4] = total.TotalBudget;
                sheet.Cells[row, 5] = total.CarbonDemandQuantity;
                sheet.Cells[row, 6] = total.ApprovedCarbonQuantity;
                sheet.Cells[row, 7] = total.awardCnt;
                sheet.Cells[row, 8] = total.Co2Total;
                sheet.Cells[row, 9] = total.GreenFunding;
                sheet.Cells[row, 10] = total.AwardTotalBudget > 0 ? 
                    (total.GreenFunding*1000 / total.AwardTotalBudget) *100 + "%" : "";
                sheet.Cells[row, 11] = Total02931;
                sheet.Cells[row, 12] = Total02932;
                sheet.Cells[row, 13] = total.F1108Area;
                sheet.Cells[row, 14] = total.F1109Length;
                sheet.Cells[row, 15] =
                total.Co2TotalItemAll > 0 ?  (total.Co2TotalItem / total.Co2TotalItemAll) *100 + "%" : "";

                //
                sheet = dict["明細表"];
                sheet.Cells[1, 4] = String.Format("附件1-{0}年度工程碳排量資料表個案明細表", year);
                sheet.Cells[1, 15] = String.Format("資料統計至{0}.{1}.{2}", DateTime.Now.Year - 1911, DateTime.Now.Month, DateTime.Now.Day);
                string execUnitName = "";
                int bkChange = 0, gInx = 0, pRow = 3;
                row = 3;
                inx = 1;

                excelRange = sheet.Rows[3];
                for (int i = 1; i < dItems.Count; i++)
                {
                    excelRange.Insert(Microsoft.Office.Interop.Excel.XlDirection.xlDown, Microsoft.Office.Interop.Excel.XlInsertFormatOrigin.xlFormatFromRightOrBelow);
                }
                foreach (CarbonEmissionReportVModel m in dItems)
                {
                    if (m.execUnitName != execUnitName)
                    {
                        inx = 1;
                        gInx++;
                        //sheet.Cells[row, 2] = m.execUnitName;
                        sheet.Cells[row, 1] = m.execUnitName;
                        if (execUnitName != "")
                        {
                            excelRange = sheet.Range[sheet.Cells[pRow, 1], sheet.Cells[row-1, 1]];
                            excelRange.Merge(0);
                            excelRange = sheet.Range[sheet.Cells[row, 1], sheet.Cells[row, 18]];
                            //excelRange.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlDouble;
                            //excelRange.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlMedium;
                            //excelRange.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).ColorIndex = 1;
                        }
                        pRow = row;
                        execUnitName = m.execUnitName;
                    }
                    //sheet.Cells[row, 1] = String.Format("{0}-{1}",gInx, inx);
                    sheet.Cells[row, 2] = String.Format(" {0}－{1}",gInx, inx);
                    //sheet.Cells[row, 2] = m.execUnitName;
                    sheet.Cells[row, 3] = m.EngName;
                    sheet.Cells[row, 4] = m.TotalBudget;
                    sheet.Cells[row, 5] = m.CarbonDemandQuantity;
                    sheet.Cells[row, 6] = m.ApprovedCarbonQuantity;
                    sheet.Cells[row, 7] = m.Co2Total;
                    sheet.Cells[row, 8] = m.awardStatus;
                    excelRange = sheet.Range[sheet.Cells[row, 8], sheet.Cells[row, 8]];
                    if (m.awardStatus == "是") excelRange.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue);
                    sheet.Cells[row, 9] = m.GreenFunding;
                    sheet.Cells[row, 10] = ( m.greenFundingRate ?? 0)  + "%";
                    sheet.Cells[row, 11] = m.Tree02931;
                    sheet.Cells[row, 12] = m.Tree02932;

                    //sheet.Cells[row, 15] = m.F1108Area;
                    //sheet.Cells[row, 16] = m.F1109Length;                    
                    sheet.Cells[row, 13] = m.F1108Area;
                    sheet.Cells[row, 14] = m.F1109Length;
                    sheet.Cells[row, 15] = m.GreenFundingValue;
                    sheet.Cells[row, 16] = m.ReductionStrategy;
                    //sheet.Cells[row, 17] = m.co2TotalRate;
                    sheet.Cells[row, 17] = m.co2TotalRate + "%";
                    sheet.Cells[row, 18] = decimal.Round( m.CarbonReduction, 2);
                    sheet.Cells[row, 19] = m.Remark;
                    sheet.Cells[row, 20] = m.DredgingEng ? "是" : "否";
                    excelRange = sheet.Range[sheet.Cells[row, 20], sheet.Cells[row, 20]];
                    if (m.DredgingEng) excelRange.Font.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue);

                    inx++;
                    row++;
                }
                excelRange = sheet.Range[sheet.Cells[pRow, 1], sheet.Cells[row - 1, 1]];
                excelRange.Merge(0);
                excelRange = sheet.Range[sheet.Cells[row, 1], sheet.Cells[row, 18]];
                excelRange.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlDouble;
                //excelRange.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlHairline;
                //excelRange.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).ColorIndex = 1;

                workbook.Save();
                workbook.Close();
                appExcel.Quit();

                Stream iStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", String.Format("工程碳排量資料表_總表-[{0}].xlsx", DateTime.Now.ToString("yyyy-MM-dd")));
                //return DownloadFile(filename, eng.EngNo);
            }
            catch (Exception e)
            {
                if (workbook != null) workbook.Close(false);
                if (appExcel != null) appExcel.Quit();
                return Json(new
                {
                    result = -1,
                    message = "Excel 製表失敗\n" + e.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        //水利署碳排管制表
        //標案年分
        public JsonResult GetYearOptions()
        {
            List<EngYearVModel> items = new TenderPlanService().GetEngYearListByAwardDate();

            return Json(items);
        }
        //依年分取執行機關
        public JsonResult GetUnitOptions(string year)
        {
            List<EngExecUnitsVModel> items = new TenderPlanService().GetEngExecUnitListAwardDate(year);
            return Json(items);
        }
        public ActionResult GetCCTList(int year, int uId)
        { //s20230506
            return Json(new
            {
                result = 0,
                items = iService.GetCarbonControlTable<CarbonEmissionCTVModel>(year, uId)
            });
        }
        public ActionResult dnCCT(int year, int uId)
        {
            string filename = Utils.CopyTemplateFile("水利署碳排管制表.xlsx", ".xlsx");
            Dictionary<string, Microsoft.Office.Interop.Excel.Worksheet> dict = new Dictionary<string, Microsoft.Office.Interop.Excel.Worksheet>();
            Microsoft.Office.Interop.Excel.Application appExcel = null;
            Microsoft.Office.Interop.Excel.Workbook workbook = null;
            //開啟 Excel 檔案
            try
            {
                appExcel = new Microsoft.Office.Interop.Excel.Application();
                workbook = appExcel.Workbooks.Open(filename);

                foreach (Microsoft.Office.Interop.Excel.Worksheet worksheet in workbook.Worksheets)
                {
                    dict.Add(worksheet.Name, worksheet);
                }
                Microsoft.Office.Interop.Excel.Worksheet sheet = dict["總表"];
                List<CarbonEmissionCTVModel> items = iService.GetCarbonControlTable<CarbonEmissionCTVModel>(year, uId);
                int row = 2, bkChange=0, inx = 1;
                string execUnitName = "";
                Microsoft.Office.Interop.Excel.Range excelRange;
                foreach (CarbonEmissionCTVModel m in items)
                {
                    if(m.execUnitName != execUnitName)
                    {
                        inx = 1;
                        execUnitName = m.execUnitName;
                        bkChange++;
                        excelRange = sheet.Range[sheet.Cells[row, 1], sheet.Cells[row, 13]];
                        excelRange.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlMedium;
                        excelRange.Borders.get_Item(Microsoft.Office.Interop.Excel.XlBordersIndex.xlEdgeTop).ColorIndex = 1;
                    }
                    sheet.Cells[row, 1] = inx;
                    sheet.Cells[row, 2] = m.execUnitName;
                    sheet.Cells[row, 3] = m.EngNo;
                    sheet.Cells[row, 4] = m.EngName;
                    sheet.Cells[row, 5] = m.TenderNo;
                    sheet.Cells[row, 6] = m.TenderName;
                    sheet.Cells[row, 7] = m.awardStatus;
                    sheet.Cells[row, 8] = m.createEng;
                    sheet.Cells[row, 9] = m.pccesXML;
                    sheet.Cells[row, 10] = m.detachableRate;
                    sheet.Cells[row, 11] = (m.engMaterialDeviceCount == 0 || m.engMaterialDeviceSummaryCount > 0 ? 1 : 0);// m.engMaterialDevice;
                    sheet.Cells[row, 12] = m.supDaily;
                    sheet.Cells[row, 13] = m.checkRec;
                    if (bkChange % 2 == 0)
                    {
                        for (int i = 1; i < 14; i++) sheet.Cells[row, i].Interior.Color = System.Drawing.Color.Linen;
                    } else
                    {
                        for (int i = 1; i < 14; i++) sheet.Cells[row, i].Interior.Color = System.Drawing.Color.AliceBlue;
                    }
                    inx++;
                    row++;
                }
                
                row = 2;
                sheet = dict["統計"];
                items = iService.GetCarbonControlTableSta<CarbonEmissionCTVModel>();
                CarbonEmissionCTVModel total = new CarbonEmissionCTVModel()
                {
                    createEng = 0,
                    pccesXML = 0,
                    detachableRateCnt = 0,
                    engMaterialDevice = 0,
                    supDaily = 0,
                    checkRec = 0
                };
                foreach (CarbonEmissionCTVModel m in items)
                {
                    sheet.Cells[row, 1] = m.execUnitName;
                    sheet.Cells[row, 2] = m.createEng;
                    sheet.Cells[row, 3] = m.pccesXML;
                    sheet.Cells[row, 4] = m.detachableRateCnt;
                    sheet.Cells[row, 5] = (m.engMaterialDeviceCount == 0 || m.engMaterialDeviceSummaryCount > 0 ? 1 : 0);
                    sheet.Cells[row, 6] = m.supDaily;
                    sheet.Cells[row, 7] = m.checkRec;
                    //
                    total.createEng += m.createEng;
                    total.pccesXML += m.pccesXML;
                    total.detachableRateCnt += m.detachableRateCnt;
                    total.engMaterialDevice += (m.engMaterialDeviceCount == 0 || m.engMaterialDeviceSummaryCount > 0 ? 1 : 0);
                    total.supDaily += m.supDaily;
                    total.checkRec += m.checkRec;
                    row++;
                }
                sheet.Cells[row, 1] = "總計";
                sheet.Cells[row, 2] = total.createEng;
                sheet.Cells[row, 3] = total.pccesXML;
                sheet.Cells[row, 4] = total.detachableRateCnt;
                sheet.Cells[row, 5] = total.engMaterialDevice;
                sheet.Cells[row, 6] = total.supDaily;
                sheet.Cells[row, 7] = total.checkRec;
                
                excelRange = sheet.Range[sheet.Cells[2, 1], sheet.Cells[row-1, 7]];
                excelRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                excelRange.Borders.ColorIndex = 1;
                excelRange = sheet.Range[sheet.Cells[row, 1], sheet.Cells[row, 7]];
                excelRange.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlMedium;
                excelRange.Borders.ColorIndex = 1;

                workbook.Save();
                workbook.Close();
                appExcel.Quit();

                Stream iStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", String.Format("水利署碳排管制表-[{0}].xlsx", DateTime.Now.ToString("yyyy-MM-dd")));
                //return DownloadFile(filename, eng.EngNo);
            }
            catch (Exception e)
            {
                if (workbook != null) workbook.Close(false);
                if (appExcel != null) appExcel.Quit();
                return Json(new
                {
                    result = -1,
                    message = "Excel 製表失敗\n" + e.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        private void setBK(Microsoft.Office.Interop.Excel.Worksheet sheet, int row)
        {
                for (int i = 1; i < 14; i++) sheet.Cells[row, i].Interior.Color = System.Drawing.Color.Linen;
        }
        
        //清單
        public JsonResult GetRecords(int pageRecordCount, int pageIndex, string keyWord)
        {
            List<CarbonEmissionFactorModel> lists = new List<CarbonEmissionFactorModel>();
            int total = iService.GetListCount(keyWord);
            if (total > 0)
            {
                lists = iService.GetList<CarbonEmissionFactorModel>(pageRecordCount, pageIndex, keyWord);
            }
            return Json(new
            {
                result = 0,
                pTotal = total,
                items = lists
            });
        }
        public JsonResult NewRecord()
        {
            return Json(new
            {
                result = 0,
                item = new CarbonEmissionFactorModel() { Seq = -1, Kind = 2, IsEnabled = true}
            });
        }
        public JsonResult UpdateRecords(CarbonEmissionFactorModel m)
        {
            int state;

            if (m.Code.Length > 9) {
                if (m.SubCode == null) m.SubCode = "";
                char[] kc = m.Code.Substring(m.Code.Length - 10, 10).ToCharArray();
                for (int j = 6; j < 10; j++)
                {
                    if (m.SubCode.IndexOf(j.ToString()) == -1)
                    {
                        kc[j - 1] = '_';
                    }
                }
                m.KeyCode1 = string.Join("", kc);
                kc[9] = '_';
                m.KeyCode2 = string.Join("", kc);
                if (!String.IsNullOrEmpty(m.Memo) && m.Memo.IndexOf("不分類") == 0)
                {
                    //m.KeyCode3 = String.Format("{0}____{1}", m.KeyCode1.Substring(0, 5), m.KeyCode1.Substring(9, 1));
                    //shioulo 20230107 取消第10碼
                    m.KeyCode3 = String.Format("{0}_____", m.KeyCode1.Substring(0, 5));
                }
                else
                    m.KeyCode3 = "-1";
            } else { 
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                });
            }

            if (m.Seq == -1)
                state = iService.AddRecord(m);
            else
                state = iService.UpdateRecord(m);
            if (state == 0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "儲存成功"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }
        public JsonResult DelRecord(int id)
        {
            int state = iService.DelRecord(id);
            if (state == 0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "刪除成功"
                }); ;
            }
            return Json(new
            {
                result = -1,
                msg = "刪除失敗"
            });
        }
        //匯入 excel
        public JsonResult Upload()
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                string fileName;
                try
                {
                    fileName = SaveFile(file);
                } catch (Exception e) {
                    System.Diagnostics.Debug.WriteLine("上傳檔案儲存失敗: " + e.Message);
                    return Json(new
                    {
                        result = -1,
                        message = "上傳檔案儲存失敗"
                    });
                }
                List<CarbonEmissionFactorModel> items = new List<CarbonEmissionFactorModel>();
                try
                {
                    readExcelData(fileName, items);
                } catch (Exception e) {
                    System.Diagnostics.Debug.WriteLine("Excel解析發生錯誤: " + e.Message);
                    return Json(new
                    {
                        result = -1,
                        message = "Excel解析發生錯誤"
                    });
                }

                int iCnt = 0, uCnt = 0;
                string errCnt = "";
                iService.ImportData(items, ref iCnt, ref uCnt, ref errCnt);
                if (errCnt != "") errCnt = "\n失敗:" + errCnt;
                return Json(new
                {
                    result = 0,
                    message = String.Format("讀入{0}筆, 新增:{1} 更新:{2} {3}", items.Count, iCnt, uCnt, errCnt)
                });
                return Json(new
                {
                    result = 0,
                    message = String.Format("共匯入{0}筆資料", items.Count)
                });
                
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }
        private void readExcelData(string fileName, List<CarbonEmissionFactorModel> items)
        {
            int inx;
            decimal kgCO2e;
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fileName, false))
            {
                var sheets = doc.WorkbookPart.WorksheetParts;
                SharedStringTable strTable = doc.WorkbookPart.SharedStringTablePart.SharedStringTable;//取得共用字串表
                foreach (var wp in sheets)
                {
                    Worksheet sheet = wp.Worksheet;
                    List<Row> rows = sheet.Descendants<Row>().ToList();
                    for(int i=2; i<rows.Count; i++) 
                    {
                        List<Cell> cells = rows[i].Descendants<Cell>().ToList();
                        if (cells.Count >= 7)
                        {
                            string str = Utils.oxCellStr(cells[0], strTable);
                            if (int.TryParse(str, out inx))
                            {
                                if (decimal.TryParse(Utils.oxCellStr(cells[2], strTable), out kgCO2e))
                                {
                                    CarbonEmissionFactorModel m = new CarbonEmissionFactorModel() { Seq = -1, Kind = 1, IsEnabled = true };
                                    m.Item = Utils.oxCellStr(cells[1], strTable);
                                    m.KgCo2e = kgCO2e;
                                    m.Unit = Utils.oxCellStr(cells[3], strTable);
                                    m.Code = Utils.oxCellStr(cells[4], strTable);
                                    m.SubCode = Utils.oxCellStr(cells[6], strTable);
                                    if (m.SubCode == null) m.SubCode = "";
                                    m.Memo = Utils.oxCellStr(cells[7], strTable);
                                    if(m.Code.Length>9)
                                    {
                                        char[] kc = m.Code.Substring(m.Code.Length - 10, 10).ToCharArray();
                                        for(int j=6; j<10; j++)
                                        {
                                            if(m.SubCode.IndexOf(j.ToString()) == -1)
                                            {
                                                kc[j-1] = '_';
                                            }
                                        }
                                        m.KeyCode1 = string.Join("",kc);
                                        kc[9] = '_';
                                        m.KeyCode2 = string.Join("", kc);
                                        if (!String.IsNullOrEmpty(m.Memo) && m.Memo.IndexOf("不分類") == 0)
                                        {
                                            //m.KeyCode3 = String.Format("{0}____{1}", m.KeyCode1.Substring(0, 5), m.KeyCode1.Substring(9, 1));
                                            //shioulo 20230107 取消第10碼
                                            m.KeyCode3 = String.Format("{0}_____", m.KeyCode1.Substring(0, 5));
                                        }
                                        else
                                            m.KeyCode3 = "-1";
                                    } else
                                    {
                                        m.KeyCode1 = m.Code;
                                        m.KeyCode2 = "-1";
                                        m.KeyCode3 = "-1";
                                    }
                                    items.Add(m);
                                }
                            }
                            else
                                break;
                        }
                    }
                }
                doc.Close();
            }
        }
        private string SaveFile(HttpPostedFileBase file)
        {
            string filePath = Path.GetTempPath();
            string originFileName = file.FileName.ToString().Trim();
            int inx = originFileName.LastIndexOf(".");
            string uniqueFileName = String.Format("{0}{1}", Guid.NewGuid(), originFileName.Substring(inx));
            string fullPath = Path.Combine(filePath, uniqueFileName);
            file.SaveAs(fullPath);

            return fullPath;
        }
    }
}