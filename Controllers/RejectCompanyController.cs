using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using EQC.Common;
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
    public class RejectCompanyController : Controller
    {//拒絕往來廠商
        RVFileService iService = new RVFileService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View();
        }
       
        //清單
        public JsonResult GetRecords()
        {
            List<RejectCompanyVModel> lists = iService.GetList<RejectCompanyVModel>();
            DateTime lastUpdate = iService.getLastUpdateTime("RVFile");
            return Json(new
            {
                result = 0,
                items = lists,
                lastUpdate = lastUpdate
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
                List<RVFileModel> items = new List<RVFileModel>();
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
                if (errCnt != "") errCnt = "失敗:" + errCnt;
                return Json(new
                {
                    result = 0,
                    message = String.Format("讀入{0}筆, 新增:{1} {2}", items.Count, iCnt, errCnt)
                });                
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }
        private void readExcelData(string fileName, List<RVFileModel> items)
        {
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
                            if (!String.IsNullOrEmpty(str))
                            {
                                RVFileModel m = new RVFileModel() { Seq = -1 };
                                m.Corporation_Number = str;
                                m.Case_no = Utils.oxCellStr(cells[1], strTable);
                                m.Corporation_Name = Utils.oxCellStr(cells[2], strTable);
                                m.Case_Name = Utils.oxCellStr(cells[10], strTable);
                                m.Effective_Date = Utils.StringChsDateToDateTime(Utils.oxCellStr(cells[14], strTable));
                                m.Expire_Date = Utils.StringChsDateToDateTime(Utils.oxCellStr(cells[16], strTable));
                                items.Add(m);
                            }
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