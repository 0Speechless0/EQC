using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EngApprovalImportController : Controller
    {//工程核定資料匯入 s20231006
        EngApprovalImportService iService = new EngApprovalImportService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View();
        }
        //刪除資料
        public JsonResult DelItem(int id)
        {
            if (iService.DelItem(id))
            {
                return Json(new
                {
                    result = 0,
                    msg = "刪除完成"
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "刪除失敗"
                });
            }
        }
        //儲存
        public JsonResult UpdateUnit(EngApprovalImportModel item)
        {
            if (iService.UpdateUnit(item))
            {
                return Json(new
                {
                    result = 0,
                    msg = "儲存完成"
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "儲存失敗"
                });
            }
        }
        //清單
        public JsonResult GetList(int pageRecordCount, int pageIndex)
        {
            List<EngApprovalImportModel> lists = new List<EngApprovalImportModel>();
            int total = iService.GetListCount();
            if (total > 0)
            {
                lists = iService.GetList<EngApprovalImportModel>(pageRecordCount, pageIndex);
            }
            return Json(new
            {
                result = 0,
                pTotal = total,
                items = lists,
            });
        }
        //清單 含單位資訊
        public JsonResult GetEngList(int pageRecordCount, int pageIndex, int unit)
        {
            List<EngApprovalImportVModel> lists = new List<EngApprovalImportVModel>();
            int total = iService.GetEngListCount(unit);
            if (total > 0)
            {
                lists = iService.GetEngList<EngApprovalImportVModel>(pageRecordCount, pageIndex, unit);
            }
            return Json(new
            {
                result = 0,
                pTotal = total,
                items = lists,
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
                List<EngApprovalImportModel> items = new List<EngApprovalImportModel>();
                if(!readExcelData(fileName, items)) {
                    return Json(new
                    {
                        result = -1,
                        message = "Excel解析發生錯誤"
                    });
                }

                int iCnt = 0, uCnt = 0;
                string errCnt = "";
                iService.ImportData(items, ref iCnt, ref uCnt, ref errCnt);
                if (errCnt != "") errCnt = "\n失敗編號:" + errCnt;
                return Json(new
                {
                    result = 0,
                    message = String.Format("讀入{0}筆, 新增:{1} 更新:{2} {3}", items.Count, iCnt, uCnt, errCnt)
                });
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }
        private bool readExcelData(string filename, List<EngApprovalImportModel> items)
        {
            Microsoft.Office.Interop.Excel.Application appExcel = null;
            Workbook workbook = null;
            //開啟 Excel 檔案
            try
            {
                appExcel = new Microsoft.Office.Interop.Excel.Application();
                workbook = appExcel.Workbooks.Open(filename);

                Dictionary<string, Worksheet> dict = new Dictionary<string, Worksheet>();
                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    dict.Add(worksheet.Name, worksheet);
                }
                Worksheet sheet = dict["核定工程"];

                int row = 2;
                while (!String.IsNullOrEmpty(sheet.Cells[row, 1].Formula.ToString()) && !String.IsNullOrEmpty(sheet.Cells[row, 2].Formula.ToString()) && !String.IsNullOrEmpty(sheet.Cells[row, 3].Formula.ToString()))
                {
                    EngApprovalImportModel m = new EngApprovalImportModel() { Seq = -1 };
                    Int16 year;
                    if (Int16.TryParse(sheet.Cells[row, 1].Value.ToString(), out year))
                    {
                        m.EngYear = year;
                    }
                    m.EngNo = sheet.Cells[row, 2].Formula.ToString();
                    m.EngName = sheet.Cells[row, 3].Formula.ToString();

                    decimal dV;
                    if (decimal.TryParse(sheet.Cells[row, 4].Value.ToString(), out dV))
                    {
                        m.TotalBudget = dV;
                    }
                    if (decimal.TryParse(sheet.Cells[row, 5].Value.ToString(), out dV))
                    {
                        m.SubContractingBudget = dV;
                    }

                    int iV;
                    if (int.TryParse(sheet.Cells[row, 6].Value.ToString(), out iV))
                    {
                        m.CarbonDemandQuantity = iV;
                    }
                    if (int.TryParse(sheet.Cells[row, 7].Value.ToString(), out iV))
                    {
                        m.ApprovedCarbonQuantity = iV;
                    }
                    items.Add(m);
                    row++;
                }
                //
                workbook.Close();
                appExcel.Quit();

                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Excel解析發生錯誤: " + e.Message);
                if (workbook != null) workbook.Close(false);
                if (appExcel != null) appExcel.Quit();
                return false;
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