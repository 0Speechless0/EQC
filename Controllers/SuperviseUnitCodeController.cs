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
{//UnitCodes Machine Fuel Maintenance
    [SessionFilter]
    public class SuperviseUnitCodeController : Controller
    {//督導紀錄機關編碼
        SuperviseUnitCodeService UnitCodeService = new SuperviseUnitCodeService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }

        //匯入 excel
        public JsonResult UnitCodeUpload()
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
                List<SuperviseUnitCodeModel> items = new List<SuperviseUnitCodeModel>();
                try
                {
                    readUnitCodeExcelData(fileName, items);
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
                UnitCodeService.ImportData(items, ref iCnt, ref uCnt, ref errCnt);
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
        private void readUnitCodeExcelData(string fileName, List<SuperviseUnitCodeModel> items)
        {
            int inx;
            decimal kgCO2e;
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fileName, false))
            {
                var workbookPart = doc.WorkbookPart;
                string relationshipId = workbookPart.Workbook.Descendants<Sheet>().FirstOrDefault(s => s.Name.Equals("機關編碼"))?.Id;
                Worksheet sheet = ((WorksheetPart)workbookPart.GetPartById(relationshipId)).Worksheet;
                if (sheet != null)
                {
                    SharedStringTable strTable = doc.WorkbookPart.SharedStringTablePart.SharedStringTable;//取得共用字串表
                    
                    List<Row> rows = sheet.Descendants<Row>().ToList();
                    for (int i = 2; i < rows.Count; i++)
                    {
                        List<Cell> cells = rows[i].Descendants<Cell>().ToList();
                        if (cells.Count >= 2)
                        {
                            string unitName = Utils.oxCellStr(cells[0], strTable);
                            string unitCode = Utils.oxCellStr(cells[1], strTable);
                            if (!String.IsNullOrEmpty(unitName) && !String.IsNullOrEmpty(unitCode))
                            {
                                SuperviseUnitCodeModel m = new SuperviseUnitCodeModel() {
                                    Seq = -1,
                                    UnitName = unitName,
                                    UnitCode = unitCode
                                };
                                items.Add(m);
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
        public JsonResult GetUnitCodeRecords(int pageRecordCount, int pageIndex, string keyWord)
        {
            List<SuperviseUnitCodeModel> lists = new List<SuperviseUnitCodeModel>();
            int total = UnitCodeService.GetListCount(keyWord);
            if (total > 0)
            {
                lists = UnitCodeService.GetList<SuperviseUnitCodeModel>(pageRecordCount, pageIndex, keyWord);
            }
            List<DateTimeVModel> lastDT = UnitCodeService.GetLastDateTime();
            return Json(new
            {
                result = 0,
                pTotal = total,
                items = lists,
                lastUpdate = (lastDT.Count==0 ? "" : lastDT[0].dateTimeStr)
            });
        }
        //更新
        public JsonResult UpdateUnitCodeRecord(SuperviseUnitCodeModel m)
        {
            if (!String.IsNullOrEmpty(m.UnitName) && !String.IsNullOrEmpty(m.UnitCode) )
            {
                int state;
                if (m.Seq == -1)
                    state = UnitCodeService.AddRecord(m);
                else
                    state = UnitCodeService.UpdateRecord(m);
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
        public JsonResult DelUnitCodeRecord(int id)
        {
            int state = UnitCodeService.DelRecord(id);
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
        public JsonResult NewUnitCodeRecord()
        {
            return Json(new
            {
                result = 0,
                item = new SuperviseUnitCodeModel() { Seq = -1, UnitName = "", UnitCode = "" }
            });
        }
        //碳排表-材料
        public ActionResult dnUnitCode()
        {
            string filename = Utils.CopyTemplateFile("督導紀錄機關編碼.xlsx", ".xlsx");
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
                Microsoft.Office.Interop.Excel.Worksheet sheet = dict["機關編碼"];
                List<SuperviseUnitCodeModel> items = UnitCodeService.GetList<SuperviseUnitCodeModel>(99999, 1, "");
                int row = 2;
                Microsoft.Office.Interop.Excel.Range excelRange;
                for(int i=1; i<items.Count; i++)
                {
                    sheet.Rows[2].Insert();
                }
                foreach (SuperviseUnitCodeModel m in items)
                {
                    sheet.Cells[row, 1] = m.UnitName;
                    sheet.Cells[row, 2] = m.UnitCode;
                    row++;
                }

                excelRange = sheet.Range[sheet.Cells[1, 1], sheet.Cells[row-1, 2]];
                excelRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                excelRange.Borders.ColorIndex = 1;

                workbook.Save();
                workbook.Close();
                appExcel.Quit();

                Stream iStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", String.Format("工程督導紀錄-機關編碼-[{0}].xlsx", DateTime.Now.ToString("yyyy-MM-dd")));
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