using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using EQC.Common;
using EQC.Models;
using EQC.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{//Materials Machine Fuel Maintenance
    [SessionFilter]
    public class MMFMaintenanceController : Controller
    {//材料機具燃料維護
        CarbonEmissionMaterialService materialService = new CarbonEmissionMaterialService();
        CarbonEmissionMachineService machineService = new CarbonEmissionMachineService();
        CarbonEmissionFuelService fuelService = new CarbonEmissionFuelService();
        CarbonEmissionCustomizeService customizeService = new CarbonEmissionCustomizeService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }

        // ** 自定義 維護 **
        //清單
        public JsonResult GetCustomizeRecords(int pageRecordCount, int pageIndex, string keyWord)
        {
            List<CarbonEmissionCustomizeModel> lists = new List<CarbonEmissionCustomizeModel>();
            int total = customizeService.GetListCount(keyWord);
            if (total > 0)
            {
                lists = customizeService.GetList<CarbonEmissionCustomizeModel>(pageRecordCount, pageIndex, keyWord);
            }
            List<DateTimeVModel> lastDT = customizeService.GetLastDateTime();
            return Json(new
            {
                result = 0,
                pTotal = total,
                items = lists,
                lastUpdate = (lastDT.Count == 0 ? "" : lastDT[0].dateTimeStr)
            });
        }
        //更新
        public JsonResult UpdateCustomizeRecord(CarbonEmissionCustomizeModel m)
        {
            if (m.KgCo2e != null && !String.IsNullOrEmpty(m.ItemCode) && !String.IsNullOrEmpty(m.NameSpec) && !String.IsNullOrEmpty(m.Unit))
            {
                int state;
                if (m.Seq == -1)
                    state = customizeService.AddRecord(m);
                else
                    state = customizeService.UpdateRecord(m);
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
        //刪除
        public JsonResult DelCustomizeRecord(int id)
        {
            int state = customizeService.DelRecord(id);
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
        public JsonResult NewCustomizeRecord()
        {
            return Json(new
            {
                result = 0,
                item = new CarbonEmissionCustomizeModel() { Seq = -1, NameSpec = "" }
            });
        }
        //碳排表-自定義
        public ActionResult dnCustomize()
        {
            string filename = Utils.CopyTemplateFile("材料機具燃料維護-自定義.xlsx", ".xlsx");
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
                Microsoft.Office.Interop.Excel.Worksheet sheet = dict["自定義"];
                List<CarbonEmissionCustomizeModel> items = customizeService.GetList<CarbonEmissionCustomizeModel>(99999, 1, "");
                int row = 3, bkChange = 0, inx = 1;
                //string execUnitName = "";
                Microsoft.Office.Interop.Excel.Range excelRange;
                for (int i = 1; i < items.Count; i++)
                {
                    sheet.Rows[4].Insert();
                }
                foreach (CarbonEmissionCustomizeModel m in items)
                {
                    sheet.Cells[row, 1] = inx;
                    sheet.Cells[row, 2] = m.CreateUnit;
                    sheet.Cells[row, 3] = m.ItemCode;
                    sheet.Cells[row, 4] = m.NameSpec;
                    sheet.Cells[row, 5] = m.KgCo2e;
                    sheet.Cells[row, 6] = m.Unit;
                    sheet.Cells[row, 7] = m.Memo;

                    excelRange = sheet.Range[sheet.Cells[row, 1], sheet.Cells[row, 5]];
                    excelRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    excelRange.Borders.ColorIndex = 1;
                    inx++;
                    row++;
                }

                workbook.Save();
                workbook.Close();
                appExcel.Quit();

                Stream iStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", String.Format("經濟部水利署-自定義單位碳排量-[{0}].xlsx", DateTime.Now.ToString("yyyy-MM-dd")));
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

        // ** 材料 維護 **
        //匯入 材料excel
        public JsonResult MaterialUpload()
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                string fileName;
                try
                {
                    fileName = SaveFile(file);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("上傳檔案儲存失敗: " + e.Message);
                    return Json(new
                    {
                        result = -1,
                        message = "上傳檔案儲存失敗"
                    });
                }
                List<CarbonEmissionMaterialModel> items = new List<CarbonEmissionMaterialModel>();
                try
                {
                    readMaterialExcelData(fileName, items);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Excel解析發生錯誤: " + e.Message);
                    return Json(new
                    {
                        result = -1,
                        message = "Excel解析發生錯誤"
                    });
                }

                int iCnt = 0, uCnt = 0;
                string errCnt = "";
                materialService.ImportData(items, ref iCnt, ref uCnt, ref errCnt);
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
        private void readMaterialExcelData(string fileName, List<CarbonEmissionMaterialModel> items)
        {
            int inx;
            decimal kgCO2e;
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fileName, false))
            {
                var workbookPart = doc.WorkbookPart;
                string relationshipId = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name.Equals("材料"))?.Id;
                Worksheet sheet = ((WorksheetPart)workbookPart.GetPartById(relationshipId)).Worksheet;
                if (sheet != null)
                {
                    SharedStringTable strTable = doc.WorkbookPart.SharedStringTablePart.SharedStringTable;//取得共用字串表
                    
                    List<Row> rows = sheet.Descendants<Row>().ToList();
                    for (int i = 2; i < rows.Count; i++)
                    {
                        List<Cell> cells = rows[i].Descendants<Cell>().ToList();
                        if (cells.Count >= 6)
                        {
                            string str = Utils.oxCellStr(cells[0], strTable);
                            if (int.TryParse(str, out inx))
                            {
                                if (decimal.TryParse(Utils.oxCellStr(cells[3], strTable), NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out kgCO2e))
                                {
                                    string itemKind = Utils.oxCellStr(cells[1], strTable);
                                    string itemName = Utils.oxCellStr(cells[2], strTable);
                                    string itemUnit = Utils.oxCellStr(cells[4], strTable);
                                    string itemPS = Utils.oxCellStr(cells[5], strTable);
                                    if( !(String.IsNullOrEmpty(itemKind) || String.IsNullOrEmpty(itemName) || String.IsNullOrEmpty(itemUnit) ) )
                                    {
                                        CarbonEmissionMaterialModel m = new CarbonEmissionMaterialModel() {
                                            itemNo = inx, Seq = -1, 
                                            Kind = itemKind, NameSpec = itemName, Unit = itemUnit, KgCo2e = kgCO2e, Memo = itemPS
                                        };
                                        items.Add(m);
                                    } else
                                    {
                                        System.Diagnostics.Debug.Print(String.Format("err {0}", inx));
                                    }
                                }
                                else
                                {
                                    System.Diagnostics.Debug.Print(String.Format("kgCO2e err {0}: {1}", inx, Utils.oxCellStr(cells[3], strTable)));
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
        //清單
        public JsonResult GetMaterialRecords(int pageRecordCount, int pageIndex, string keyWord)
        {
            List<CarbonEmissionMaterialModel> lists = new List<CarbonEmissionMaterialModel>();
            int total = materialService.GetListCount(keyWord);
            if (total > 0)
            {
                lists = materialService.GetList<CarbonEmissionMaterialModel>(pageRecordCount, pageIndex, keyWord);
            }
            List<DateTimeVModel> lastDT = materialService.GetLastDateTime();
            return Json(new
            {
                result = 0,
                pTotal = total,
                items = lists,
                lastUpdate = (lastDT.Count==0 ? "" : lastDT[0].dateTimeStr)
            });
        }
        //更新
        public JsonResult UpdateMaterialRecord(CarbonEmissionMaterialModel m)
        {
            if (m.KgCo2e != null && !String.IsNullOrEmpty(m.Kind) && !String.IsNullOrEmpty(m.NameSpec) && !String.IsNullOrEmpty(m.Unit))
            {
                int state;
                if (m.Seq == -1)
                    state = materialService.AddRecord(m);
                else
                    state = materialService.UpdateRecord(m);
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
        //刪除
        public JsonResult DelMaterialRecord(int id)
        {
            int state = materialService.DelRecord(id);
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
        public JsonResult NewMaterialRecord()
        {
            return Json(new
            {
                result = 0,
                item = new CarbonEmissionMaterialModel() { Seq = -1, Kind = "", NameSpec = "" }
            });
        }
        //碳排表-材料
        public ActionResult dnMaterial()
        {
            string filename = Utils.CopyTemplateFile("材料機具燃料維護-材料.xlsx", ".xlsx");
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
                Microsoft.Office.Interop.Excel.Worksheet sheet = dict["材料"];
                List<CarbonEmissionMaterialModel> items = materialService.GetList<CarbonEmissionMaterialModel>(99999, 1, "");
                int row = 3, bkChange=0, inx = 1;
                //string execUnitName = "";
                Microsoft.Office.Interop.Excel.Range excelRange;
                for(int i=1; i<items.Count; i++)
                {
                    sheet.Rows[4].Insert();
                }
                foreach (CarbonEmissionMaterialModel m in items)
                {
                    sheet.Cells[row, 1] = inx;                   
                    sheet.Cells[row, 2] = m.Kind;
                    sheet.Cells[row, 3] = m.NameSpec;
                    sheet.Cells[row, 4] = m.KgCo2e;
                    sheet.Cells[row, 5] = m.Unit;
                    sheet.Cells[row, 6] = m.Memo;

                    excelRange = sheet.Range[sheet.Cells[row, 1], sheet.Cells[row, 6]];
                    excelRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    excelRange.Borders.ColorIndex = 1;
                    /*excelRange = sheet.Range[sheet.Cells[row, 1], sheet.Cells[row, 7]];
                    excelRange.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlMedium;
                    excelRange.Borders.ColorIndex = 1; */
                         inx++;
                    row++;
                }
                
                workbook.Save();
                workbook.Close();
                appExcel.Quit();

                Stream iStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", String.Format("經濟部水利署-材料單位碳排量-[{0}].xlsx", DateTime.Now.ToString("yyyy-MM-dd")));
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

        // ** 機具 維護 **
        //匯入 機具excel
        public JsonResult MachineUpload()
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                string fileName;
                try
                {
                    fileName = SaveFile(file);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("上傳檔案儲存失敗: " + e.Message);
                    return Json(new
                    {
                        result = -1,
                        message = "上傳檔案儲存失敗"
                    });
                }
                List<CarbonEmissionMachineModel> items = new List<CarbonEmissionMachineModel>();
                try
                {
                    readMachineExcelData(fileName, items);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Excel解析發生錯誤: " + e.Message);
                    return Json(new
                    {
                        result = -1,
                        message = "Excel解析發生錯誤"
                    });
                }

                int iCnt = 0, uCnt = 0;
                string errCnt = "";
                machineService.ImportData(items, ref iCnt, ref uCnt, ref errCnt);
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
        private void readMachineExcelData(string fileName, List<CarbonEmissionMachineModel> items)
        {
            int inx;
            decimal kgCO2e;
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fileName, false))
            {
                var workbookPart = doc.WorkbookPart;
                string relationshipId = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name.Equals("機具"))?.Id;
                Worksheet sheet = ((WorksheetPart)workbookPart.GetPartById(relationshipId)).Worksheet;
                if (sheet != null)
                {
                    SharedStringTable strTable = doc.WorkbookPart.SharedStringTablePart.SharedStringTable;//取得共用字串表

                    List<Row> rows = sheet.Descendants<Row>().ToList();
                    for (int i = 2; i < rows.Count; i++)
                    {
                        List<Cell> cells = rows[i].Descendants<Cell>().ToList();
                        if (cells.Count >= 11)
                        {
                            string str = Utils.oxCellStr(cells[0], strTable);
                            if (int.TryParse(str, out inx))
                            {
                                if (decimal.TryParse(Utils.oxCellStr(cells[8], strTable), out kgCO2e))
                                {
                                    string itemKind = Utils.oxCellStr(cells[1], strTable);
                                    string itemName = Utils.oxCellStr(cells[2], strTable);
                                    string itemUnit = Utils.oxCellStr(cells[9], strTable);
                                    string itemPS = Utils.oxCellStr(cells[10], strTable);
                                    if (!(String.IsNullOrEmpty(itemKind) || String.IsNullOrEmpty(itemName) || String.IsNullOrEmpty(itemUnit)))
                                    {
                                        CarbonEmissionMachineModel m = new CarbonEmissionMachineModel()
                                        {
                                            itemNo = inx,
                                            Seq = -1,
                                            Kind = itemKind,
                                            NameSpec = itemName,
                                            Unit = itemUnit,
                                            KgCo2e = kgCO2e,
                                            Memo = itemPS
                                        };
                                        if (decimal.TryParse(Utils.oxCellStr(cells[3], strTable), out kgCO2e))
                                        {
                                            m.ConsumptionRate = kgCO2e;
                                            m.ConsumptionRateUnit = Utils.oxCellStr(cells[4], strTable);
                                        }
                                        if (decimal.TryParse(Utils.oxCellStr(cells[6], strTable), out kgCO2e))
                                        {
                                            m.FuelKind = Utils.oxCellStr(cells[5], strTable);
                                            m.FuelKgCo2e = kgCO2e;
                                            m.FuelUnit = Utils.oxCellStr(cells[7], strTable);
                                        }
                                        items.Add(m);
                                    }
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
        //清單
        public JsonResult GetMachineRecords(int pageRecordCount, int pageIndex, string keyWord)
        {
            List<CarbonEmissionMachineModel> lists = new List<CarbonEmissionMachineModel>();
            int total = machineService.GetListCount(keyWord);
            if (total > 0)
            {
                lists = machineService.GetList<CarbonEmissionMachineModel>(pageRecordCount, pageIndex, keyWord);
            }
            List<DateTimeVModel> lastDT = machineService.GetLastDateTime();
            return Json(new
            {
                result = 0,
                pTotal = total,
                items = lists,
                lastUpdate = (lastDT.Count == 0 ? "" : lastDT[0].dateTimeStr)
            });
        }
        //更新
        public JsonResult UpdateMachineRecord(CarbonEmissionMachineModel m)
        {
            if (m.KgCo2e != null && !String.IsNullOrEmpty(m.Kind) && !String.IsNullOrEmpty(m.NameSpec) && !String.IsNullOrEmpty(m.Unit))
            {
                int state;
                if (m.Seq == -1)
                    state = machineService.AddRecord(m);
                else
                    state = machineService.UpdateRecord(m);
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
        //刪除
        public JsonResult DelMachineRecord(int id)
        {
            int state = machineService.DelRecord(id);
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
        public JsonResult NewMachineRecord()
        {
            return Json(new
            {
                result = 0,
                item = new CarbonEmissionMachineModel() { Seq = -1, Kind = "", NameSpec = "" }
            });
        }
        //碳排表-機具
        public ActionResult dnMachine()
        {
            string filename = Utils.CopyTemplateFile("材料機具燃料維護-機具.xlsx", ".xlsx");
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
                Microsoft.Office.Interop.Excel.Worksheet sheet = dict["機具"];
                List<CarbonEmissionMachineModel> items = machineService.GetList<CarbonEmissionMachineModel>(99999, 1, "");
                int row = 3, bkChange = 0, inx = 1;
                //string execUnitName = "";
                Microsoft.Office.Interop.Excel.Range excelRange;
                for (int i = 1; i < items.Count; i++)
                {
                    sheet.Rows[4].Insert();
                }
                foreach (CarbonEmissionMachineModel m in items)
                {
                    sheet.Cells[row, 1] = inx;
                    sheet.Cells[row, 2] = m.Kind;
                    sheet.Cells[row, 3] = m.NameSpec;
                    sheet.Cells[row, 4] = m.ConsumptionRate;
                    sheet.Cells[row, 5] = m.ConsumptionRateUnit;
                    sheet.Cells[row, 6] = m.FuelKind;
                    sheet.Cells[row, 7] = m.FuelKgCo2e;
                    sheet.Cells[row, 8] = m.FuelUnit;
                    sheet.Cells[row, 9] = m.KgCo2e;
                    sheet.Cells[row, 10] = m.Unit;
                    sheet.Cells[row, 11] = m.Memo;

                    excelRange = sheet.Range[sheet.Cells[row, 1], sheet.Cells[row, 11]];
                    excelRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    excelRange.Borders.ColorIndex = 1;
                    /*excelRange = sheet.Range[sheet.Cells[row, 1], sheet.Cells[row, 7]];
                    excelRange.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlMedium;
                    excelRange.Borders.ColorIndex = 1; */
                    inx++;
                    row++;
                }

                workbook.Save();
                workbook.Close();
                appExcel.Quit();

                Stream iStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", String.Format("經濟部水利署-機具單位碳排量-[{0}].xlsx", DateTime.Now.ToString("yyyy-MM-dd")));
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

        // ** 燃料 維護 **
        //匯入 燃料excel
        public JsonResult FuelUpload()
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                string fileName;
                try
                {
                    fileName = SaveFile(file);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("上傳檔案儲存失敗: " + e.Message);
                    return Json(new
                    {
                        result = -1,
                        message = "上傳檔案儲存失敗"
                    });
                }
                List<CarbonEmissionFuelModel> items = new List<CarbonEmissionFuelModel>();
                try
                {
                    readFuelExcelData(fileName, items);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("Excel解析發生錯誤: " + e.Message);
                    return Json(new
                    {
                        result = -1,
                        message = "Excel解析發生錯誤"
                    });
                }

                int iCnt = 0, uCnt = 0;
                string errCnt = "";
                fuelService.ImportData(items, ref iCnt, ref uCnt, ref errCnt);
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
        private void readFuelExcelData(string fileName, List<CarbonEmissionFuelModel> items)
        {
            int inx;
            decimal kgCO2e;
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fileName, false))
            {
                var workbookPart = doc.WorkbookPart;
                string relationshipId = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name.Equals("燃料"))?.Id;
                Worksheet sheet = ((WorksheetPart)workbookPart.GetPartById(relationshipId)).Worksheet;
                if (sheet != null)
                {
                    SharedStringTable strTable = doc.WorkbookPart.SharedStringTablePart.SharedStringTable;//取得共用字串表

                    List<Row> rows = sheet.Descendants<Row>().ToList();
                    for (int i = 2; i < rows.Count; i++)
                    {
                        List<Cell> cells = rows[i].Descendants<Cell>().ToList();
                        if (cells.Count >= 5)
                        {
                            string str = Utils.oxCellStr(cells[0], strTable);
                            if (int.TryParse(str, out inx))
                            {
                                if (decimal.TryParse(Utils.oxCellStr(cells[2], strTable), out kgCO2e))
                                {
                                    string itemName = Utils.oxCellStr(cells[1], strTable);
                                    string itemUnit = Utils.oxCellStr(cells[3], strTable);
                                    string itemPS = Utils.oxCellStr(cells[4], strTable);
                                    if (!(String.IsNullOrEmpty(itemName) || String.IsNullOrEmpty(itemUnit)))
                                    {
                                        CarbonEmissionFuelModel m = new CarbonEmissionFuelModel()
                                        {
                                            itemNo = inx,
                                            Seq = -1,
                                            NameSpec = itemName,
                                            Unit = itemUnit,
                                            KgCo2e = kgCO2e,
                                            Memo = itemPS
                                        };
                                        items.Add(m);
                                    }
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
        //清單
        public JsonResult GetFuelRecords(int pageRecordCount, int pageIndex, string keyWord)
        {
            List<CarbonEmissionFuelModel> lists = new List<CarbonEmissionFuelModel>();
            int total = fuelService.GetListCount(keyWord);
            if (total > 0)
            {
                lists = fuelService.GetList<CarbonEmissionFuelModel>(pageRecordCount, pageIndex, keyWord);
            }
            List<DateTimeVModel> lastDT = fuelService.GetLastDateTime();
            return Json(new
            {
                result = 0,
                pTotal = total,
                items = lists,
                lastUpdate = (lastDT.Count == 0 ? "" : lastDT[0].dateTimeStr)
            });
        }
        //更新
        public JsonResult UpdateFuelRecord(CarbonEmissionFuelModel m)
        {
            if (m.KgCo2e != null && !String.IsNullOrEmpty(m.NameSpec) && !String.IsNullOrEmpty(m.Unit))
            {
                int state;
                if (m.Seq == -1)
                    state = fuelService.AddRecord(m);
                else
                    state = fuelService.UpdateRecord(m);
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
        //刪除
        public JsonResult DelFuelRecord(int id)
        {
            int state = fuelService.DelRecord(id);
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
        public JsonResult NewFuelRecord()
        {
            return Json(new
            {
                result = 0,
                item = new CarbonEmissionFuelModel() { Seq = -1, NameSpec = "" }
            });
        }
        //碳排表-燃料
        public ActionResult dnFuel()
        {
            string filename = Utils.CopyTemplateFile("材料機具燃料維護-燃料.xlsx", ".xlsx");
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
                Microsoft.Office.Interop.Excel.Worksheet sheet = dict["燃料"];
                List<CarbonEmissionFuelModel> items = fuelService.GetList<CarbonEmissionFuelModel>(99999, 1, "");
                int row = 3, bkChange = 0, inx = 1;
                //string execUnitName = "";
                Microsoft.Office.Interop.Excel.Range excelRange;
                for (int i = 1; i < items.Count; i++)
                {
                    sheet.Rows[4].Insert();
                }
                foreach (CarbonEmissionFuelModel m in items)
                {
                    sheet.Cells[row, 1] = inx;
                    sheet.Cells[row, 2] = m.NameSpec;
                    sheet.Cells[row, 3] = m.KgCo2e;
                    sheet.Cells[row, 4] = m.Unit;
                    sheet.Cells[row, 5] = m.Memo;

                    excelRange = sheet.Range[sheet.Cells[row, 1], sheet.Cells[row, 5]];
                    excelRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                    excelRange.Borders.ColorIndex = 1;
                    /*excelRange = sheet.Range[sheet.Cells[row, 1], sheet.Cells[row, 7]];
                    excelRange.Borders.Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlMedium;
                    excelRange.Borders.ColorIndex = 1; */
                    inx++;
                    row++;
                }

                workbook.Save();
                workbook.Close();
                appExcel.Quit();

                Stream iStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", String.Format("經濟部水利署-燃料單位碳排量-[{0}].xlsx", DateTime.Now.ToString("yyyy-MM-dd")));
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

        /*private void setBK(Microsoft.Office.Interop.Excel.Worksheet sheet, int row)
        {
                for (int i = 1; i < 14; i++) sheet.Cells[row, i].Interior.Color = System.Drawing.Color.Linen;
        }*/

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