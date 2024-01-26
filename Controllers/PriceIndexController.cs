using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class PriceIndexController : Controller
    {//物價指數維護
        PriceIndexService iService = new PriceIndexService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Edit");
        }

        //指數類別清單
        public JsonResult GetKindList()
        {
            List<PriceIndexKindModel> lists = iService.GetKindList<PriceIndexKindModel>();
            return Json(new
            {
                result = 0,
                items = lists
            });
        }

        //指數清單
        public JsonResult GetRecords(int kind)
        {
            List<PriceIndexItemsVModel> lists = iService.GetList<PriceIndexItemsVModel>(kind);
            return Json(new
            {
                result = 0,
                items = lists
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
                List<PriceIndexListVModel> items = new List<PriceIndexListVModel>();
                if(readExcelData(fileName, items) == -1) {
                    return Json(new
                    {
                        result = -1,
                        message = "Excel解析發生錯誤"
                    });
                }
                iService.ImportData(items);
                return Json(new
                {
                    result = 0,
                    message = String.Format("匯入完成", items.Count)
                });
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }
        private int readExcelData(string filename, List<PriceIndexListVModel> items)
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
                Worksheet sheet = dict["物價指數"];

                int colInx = 2;
                int rowInx;
                int id;
                while (sheet.Cells[1, colInx].Value != null && !String.IsNullOrEmpty(sheet.Cells[1, colInx].Value.ToString()) )
                {
                    if (int.TryParse(sheet.Cells[2, colInx].Value.ToString(), out id)) {
                        string mCode = sheet.Cells[1, colInx].Value.ToString();
                        List<PriceIndexListVModel> kinds = iService.GetKindItem<PriceIndexListVModel>(id, mCode);
                        if(kinds.Count == 1)
                        {
                            PriceIndexListVModel kind = kinds[0];
                            rowInx = 4;
                            while (sheet.Cells[rowInx, 1].Value != null && !String.IsNullOrEmpty(sheet.Cells[rowInx, 1].Value.ToString()))
                            {
                                PriceIndexItemModel m = new PriceIndexItemModel();
                                //string cht = ;
                                string[] cDate = sheet.Cells[rowInx, 1].Value.ToString().Split('/');
                                if (cDate.Length == 2)
                                {
                                    m.PIDate = DateTime.Parse(String.Format("{0}/{1}/1", int.Parse(cDate[0])+1911, cDate[1]));
                                    m.PriceIndex = Decimal.Parse(sheet.Cells[rowInx, colInx].Value.ToString());

                                    kind.items.Add(m);
                                }
                                rowInx++;
                            }
                            if(kind.items.Count > 0) items.Add(kind);
                        }
                    }
                    colInx++;
                }

                workbook.Close();
                appExcel.Quit();

                return 0;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Excel解析發生錯誤: " + e.Message);
                if (workbook != null) workbook.Close(false);
                if (appExcel != null) appExcel.Quit();
                return -1;
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

        //下載工程範本(excel)
        public ActionResult Download()
        {
            string filename = Utils.CopyTemplateFile("物價指數清單.xlsx", ".xlsx");
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
                    dict.Add(worksheet.Name, worksheet);
                }
                Worksheet sheet = dict["物價指數"];

                List<PriceIndexKindModel> kinds = iService.GetKindList<PriceIndexKindModel>();

                int col = 2;
                foreach(PriceIndexKindModel m in kinds)
                {
                    sheet.Cells[1, col] = m.MCode;
                    sheet.Cells[2, col] = m.Id;
                    sheet.Cells[3, col] = m.PS;
                    col++;
                }

                workbook.Save();
                workbook.Close();
                appExcel.Quit();

                return DownloadFile(filename);
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
        private ActionResult DownloadFile(string fullPath)
        {
            if (System.IO.File.Exists(fullPath))
            {
                Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", "物價指數清單程範本.xlsx");
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }
    }
}