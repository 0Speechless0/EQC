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
    public class CarbonEmissionSettingController : Controller
    {//碳排量設定
        CarbonEmissionSettingService iService = new CarbonEmissionSettingService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }
        //單位清單 s20230502
        public JsonResult GetUnitList()
        {
            return Json(new
            {
                result = 0,
                items = iService.GetUnitList<SelectIntOptionModel>()
            });
        }
        //清單
        public JsonResult GetList(int year)
        {
            if (year == -1)
            {
                return Json(new
                {
                    result = 0,
                    items = iService.GetListAll<CarbonEmissionSettingVModel>()
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    items = iService.GetList<CarbonEmissionSettingVModel>(year)
                });
            }
        }

        public JsonResult NewRecord()
        {
            return Json(new
            {
                result = 0,
                item = new CarbonEmissionSettingModel() { Seq = -1 }
            });
        }
        //更新
        public JsonResult UpdateRecord(CarbonEmissionSettingModel m)
        {
            int state = -1;
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
        //刪除
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
        public JsonResult Upload(int year, int uId)
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
                List<CarbonEmissionSettingImportModel> items = new List<CarbonEmissionSettingImportModel>();
                if (!readExcelData(fileName, items, year, uId)) {
                    return Json(new
                    {
                        result = -1,
                        message = "Excel解析發生錯誤"
                    });
                }
                if(items.Count==0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "無資料"
                    });
                }

                return Json(new
                {
                    result = 0,
                    items = items
                });
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }
        private bool readExcelData(string filename, List<CarbonEmissionSettingImportModel> items, int engYear, int engUnitSeq)
        {
            Microsoft.Office.Interop.Excel.Application appExcel = null;
            Workbook workbook = null;
            //開啟 Excel 檔案
            try
            {
                appExcel = new Microsoft.Office.Interop.Excel.Application();
                workbook = appExcel.Workbooks.Open(filename);

                Worksheet sheet = null;
                Dictionary<string, Worksheet> dict = new Dictionary<string, Worksheet>();
                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    sheet = worksheet;
                    break;
                }
                if(sheet == null)
                {
                    workbook.Close();
                    appExcel.Quit();

                    return false;
                }

                int row = 2;
                bool read = true;
                while (read)
                {
                    if (sheet.Cells[row, 1].Value == null || sheet.Cells[row, 2].Value == null || sheet.Cells[row, 1] == null)
                    {
                        read = false;
                    }
                    else if(String.IsNullOrEmpty(sheet.Cells[row, 1].Value.ToString()) || String.IsNullOrEmpty(sheet.Cells[row, 2].Value.ToString()) || String.IsNullOrEmpty(sheet.Cells[row, 3].Value.ToString()))
                    {
                        read = false;
                    }
                    else
                    {
                        items.Add(new CarbonEmissionSettingImportModel()
                        {
                            Seq = -1,
                            EngYear = engYear,
                            EngUnitSeq = engUnitSeq,
                            EngNo = sheet.Cells[row, 1].Value.ToString(),
                            CarbonDemandQuantity = int.Parse(sheet.Cells[row, 2].Value.ToString()),
                            ApprovedCarbonQuantity = int.Parse(sheet.Cells[row, 3].Value.ToString())
                        });
                    }
                    row++;
                }
                workbook.Close();
                appExcel.Quit();

                return true;
            }
            catch (Exception e)
            {
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
        //儲存匯入資料
        public JsonResult UpdateImport(List<CarbonEmissionSettingImportModel> items)
        {
            if(iService.UpdateImport(items))
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
    }
}