using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class ESQCPlaneWeaknessController : Controller
    {//工程督導 - 品質管制弱面篩選
        SupervisePhaseService iService = new SupervisePhaseService();
        public ActionResult Index()
        {
            Utils.setUserClass(this, 1);
            return View("Index");
        }

        //當年度期別
        public JsonResult GetPhaseOptions()
        {
            return Json(new
            {
                result = 0,
                items = iService.GetPhaseOptions(DateTime.Now.Year - 1911)
            });
        }
        //期別查詢
        public JsonResult SearchPhase(string keyWord)
        {
            List<SupervisePhaseModel> list = iService.GetPhaseCode(keyWord);
            if(list.Count==1)
            {
                return Json(new
                {
                    result = 0,
                    item = list[0]
                });
            }
            return Json(new
            {
                result = -1,
                msg = "查無此期別"
            });
        }
        //新增期別查詢
        public JsonResult NewPhase(string keyWord)
        {
            List<SupervisePhaseModel> list = iService.GetPhaseCode(keyWord);
            if (list.Count == 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = keyWord+"期別資料已存在"
                });
            }
            int seq = iService.AddPhaseCode(keyWord);
            if (seq>0)
            {
                return Json(new {
                    result = 0,
                    item = new SupervisePhaseModel()
                    {
                        Seq = seq,
                        PhaseCode = keyWord
                    }
                });
            }
            return Json(new
            {
                result = -1,
                msg = "新增期別失敗"
            });
        }
        //刪除期別
        public JsonResult DelPhase(int id)
        {
            int state = iService.DelPhase(id);
            if (state == 0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "刪除成功"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "刪除期別失敗, 可能相關資料已建立"
            });
        }
        //工程加入到期別
        public JsonResult AddEng(int phaseSeq, int engSeq)
        {
            int seq = iService.AddEng(phaseSeq, engSeq);
            if (seq > 0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "工程加入成功"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "工程加入失敗"
            });
        }
        //工程加入到期別
        public JsonResult DelEng(int id)
        {
            int state = iService.DelEng(id);
            if (state > -1)
            {
                return Json(new
                {
                    result = 0,
                    msg = "工程刪除成功"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "工程刪除失敗"
            });
        }
        //期間工程清單
        public JsonResult GetPhaseEngList(int id)
        {
            List<SuperviseEngVModel> engList = iService.GetPhaseEngList<SuperviseEngVModel>(id);

            return Json(new
            {
                items = engList
            });
        }
        //工程清單 by PrjXML shioulo 20220713
        //public JsonResult GetEngList(string year, string unit, string engMain, int pageRecordCount, int pageIndex, ESQEngFilterVModel filterItem )
        public JsonResult GetEngList(int mode, int pageRecordCount, int pageIndex, string fName, string fUnit)
        {
            //PrjXMLService pService = new PrjXMLService();
            List<ESQCPlaneWeaknessVModel> engList = new List<ESQCPlaneWeaknessVModel>();
            int total = iService.GetTenderListCount(mode, fName, fUnit);
            if (total > 0)
            {
                engList = iService.GetTenderList<ESQCPlaneWeaknessVModel>(mode, pageRecordCount, pageIndex, fName, fUnit);
            }
            return Json(new
            {
                pTotal = total,
                items = engList
            });
        }
        //工程單位清單 s20230310
        public JsonResult GetEngUnitList(int mode)
        {
            return Json(new
            {
                result = 0,
                items = iService.GetTenderUnitList(mode)
            });
        }

        public ActionResult GetUserUnit()
        {
            string unitSubSeq = "";
            string unitSeq = "";
            Utils.GetUserUnit(ref unitSeq, ref unitSubSeq);
            return Json(new
            {
                result = 0,
                unit = unitSeq,
                unitSub = unitSubSeq
            });
        }
        //標案年分
        public JsonResult GetYearOptions()
        {
            //List<EngYearVModel> years = iService.GetEngYearList();
            List <SelectIntOptionModel> years = new PrjXMLService().GetTenderYearList();
            return Json(years);
        }
        //依年分取執行機關
        public JsonResult GetUnitOptions(string year)
        {
            //List<EngExecUnitsVModel> items = iService.GetEngExecUnitList(year);
            List<SelectOptionModel> items = new PrjXMLService().GetTenderExecUnitList(year);
            return Json(items);
        }
        //依年分,機關執行單位取工程
        public JsonResult GetEngNameOptions(string year, string unit, string engMain)
        {
            List<ESQEngNameOptionsVModel> items = new List<ESQEngNameOptionsVModel>();
            items = new PrjXMLService().GetPrjXMLPlaneWeaknessList<ESQEngNameOptionsVModel>(year, unit);
            
            if (items.Count > 0)
            {
                items.Sort((x, y) => x.CompareTo(y));
                ESQEngNameOptionsVModel m = new ESQEngNameOptionsVModel();
                m.Seq = -1;
                m.EngName = "全部工程";
                items.Insert(0, m);
            }
            return Json(items);
        }
        //品質管制弱面檔案
        public ActionResult dnQCPlaneWeakness(int mode)
        {
            string filename = Utils.CopyTemplateFile("品質管制弱面報表.xlsx", ".xlsx");
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
                Microsoft.Office.Interop.Excel.Worksheet sheet = dict["管制弱面"];
                List<ESQCPlaneWeakness1VModel> items = iService.QCPlaneWeaknessList<ESQCPlaneWeakness1VModel>(mode);
                int row = 3, inx = 1;
                Microsoft.Office.Interop.Excel.Range excelRange;
                for (int i = 1; i < items.Count; i++)
                {
                    sheet.Rows[4].Insert();
                }
                foreach (ESQCPlaneWeakness1VModel m in items)
                {
                    sheet.Cells[row, 1] = inx;
                    sheet.Cells[row, 2] = m.ExecUnitName;
                    sheet.Cells[row, 3] = m.TenderName;
                    sheet.Cells[row, 4] = m.TownName;
                    sheet.Cells[row, 5] = m.ActualStartDate;
                    sheet.Cells[row, 6] = m.ScheCompletionDate;
                    if (m.ActualProgress.HasValue) sheet.Cells[row, 7] = m.ActualProgress.Value;
                    sheet.Cells[row, 8] = m.W1==1 ? "V" : "";
                    sheet.Cells[row, 9] = m.W2 == 1 ? "V" : "";
                    sheet.Cells[row, 10] = m.W3 == 1 ? "V" : "";
                    sheet.Cells[row, 11] = m.W4 == 1 ? "V" : "";
                    sheet.Cells[row, 12] = m.W5 == 1 ? "V" : "";
                    sheet.Cells[row, 13] = m.W6 == 1 ? "V" : "";
                    sheet.Cells[row, 14] = m.W7 == 1 ? "V" : "";
                    sheet.Cells[row, 15] = m.W8 == 1 ? "V" : "";
                    sheet.Cells[row, 16] = m.W9 == 1 ? "V" : "";
                    sheet.Cells[row, 17] = m.W10 == 1 ? "V" : "";
                    sheet.Cells[row, 18] = m.W11 == 1 ? "V" : "";
                    sheet.Cells[row, 19] = m.W12 == 1 ? "V" : "";
                    sheet.Cells[row, 20] = m.W13 == 1 ? "V" : "";
                    sheet.Cells[row, 21] = m.W14 == 1 ? "V" : "";
                    sheet.Cells[row, 22] = m.W1 + m.W2 + m.W3 + m.W4 + m.W5 + m.W6 + m.W7 + m.W8 + m.W9 + m.W10 + m.W11 + m.W12 + m.W13 + m.W14;
                    sheet.Cells[row, 23] = String.IsNullOrEmpty(m.AuditDate) ? "" : "V";
                    sheet.Cells[row, 24] = m.AuditDate;
                    sheet.Cells[row, 25] = m.RecDate;
                    sheet.Cells[row, 26] = m.GravelField;
                    sheet.Cells[row, 27] = m.NearTender;
                    if (m.BidAmount.HasValue) sheet.Cells[row, 28] = m.BidAmount.Value;
                    sheet.Cells[row, 29] = m.TenderName.IndexOf("疏濬") > 0 ? "V" : "";
                    sheet.Cells[row, 30] = m.TenderName.IndexOf("開工合約") > 0 ? "V" : "";
                    inx++;
                    row++;
                }

                excelRange = sheet.Range[sheet.Cells[4, 1], sheet.Cells[row - 1, 30]];
                excelRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                excelRange.Borders.ColorIndex = 1;

                workbook.Save();
                workbook.Close();
                appExcel.Quit();

                Stream iStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", String.Format("品質管制弱面報表-[{0}].xlsx", DateTime.Now.ToString("yyyy-MM-dd")));
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
    }
}