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
    public class ESSuperviseFillController : Controller
    {//工程督導 - 督導填報
        SuperviseFillService iService = new SuperviseFillService();
        public ActionResult Index()
        {
            Utils.setUserClass(this, 1);
            return View("Index");
        }
        public ActionResult EditEng(int seq)
        {
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Edit", "ESSuperviseFill");
            string tarUrl = redirectUrl.ToString() + "?id=" + seq;
            return Json(new { Url = tarUrl });
        }
        public ActionResult Edit()
        {
            Utils.setUserClass(this, 1);
            return View("Edit");
        }

        //刪除項目清單 20230215
        public JsonResult DelSamplingRecord(int id)
        {
            int state = iService.DelSamplingItem(id);
            if (state > -1)
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
        //檢驗項目清單 20230215
        public JsonResult GetSamplingRecords(int id)
        {
            return Json(new
            {
                result = 0,
                items = iService.GetSamplingList<SuperviseFillSamplingModel>(id)
            });
        }
        //儲存檢驗項目 20230215
        public JsonResult UpdateSamplingRecord(SuperviseFillSamplingModel m)
        {
            int result = -1;
            if (m.Seq == -1)
                result = iService.AddSamplingItem(m);
            else
                result = iService.UpdateSamplingItem(m);

            if (result == -1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "儲存失敗"
                });
            } else
            {
                return Json(new
                {
                    result = 0,
                    msg = "儲存成功"
                });
            }
        }
        public JsonResult GetNewSamplingRecord()
        {
            return Json(new
            {
                result = 0,
                item = new SuperviseFillSamplingModel() { Seq = -1 }
            });
        }
        //儲存檢驗、拆驗 20230215
        public JsonResult SaveInspect(int id, string inspect)
        {
            if (iService.SaveInspect(id, inspect))
            {
                return Json(new
                {
                    result = 0,
                    msg = "儲存成功"
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
        //委員評分清單 20230215
        public JsonResult GetCommitteeScoreList(int id)
        {
            return Json(new
            {
                result = 0,
                items = iService.GetEngCommittesScore<SuperviseFillCommitteeScoreVModel>(id)
                .SortListByMap(r => r.mode.ToString(), Order.CommitteeScoredOrder)
            });
        }
        //儲存委員評分清單 20230215
        public JsonResult SaveCommitteeScore(int id, List<SuperviseFillCommitteeScoreVModel> items)
        {
            if (iService.SaveEngCommittesScore(id, items))
            {
                List<SuperviseEngSuperviseFillVModel> engList = iService.GetEngForSuperviseFill<SuperviseEngSuperviseFillVModel>(id);
                return Json(new
                {
                    result = 0,
                    avgScore = engList[0].CommitteeAverageScore,
                    msg = "儲存成功"
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
        //期別查詢
        public JsonResult SearchPhase(string keyWord)
        {
            List<SupervisePhaseModel> list = iService.GetPhaseCode(keyWord);
            if (list.Count == 1)
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
        //期間工程清單
        public JsonResult GetPhaseEngList(int id, int pageRecordCount, int pageIndex)
        {
            List<SuperviseEng1VModel> engList = new List<SuperviseEng1VModel>();
            int total = iService.GetPhaseEngList1Count(id);

            if (total > 0)
            {
                engList = iService.GetPhaseEngList1<SuperviseEng1VModel>(id, pageRecordCount, pageIndex);
            }
            return Json(new
            {
                pTotal = total,
                items = engList
            });
        }

        //期間工程
        public JsonResult GetEng(int id)
        {
            List<SuperviseEngSuperviseFillVModel> engList = iService.GetEngForSuperviseFill<SuperviseEngSuperviseFillVModel>(id);

            if (engList.Count == 1)
            {
                SuperviseEngSuperviseFillVModel m = engList[0];
                m.committees = iService.GetEngCommittes<SuperviseFillCommitteeVModel>(m.Seq)
                    .SortListByMap(r => r.mode.ToString(), Order.CommitteeScoredOrder);
                return Json(new
                {
                    result = 0,
                    item = m
                });
            } else
            {
                return Json(new
                {
                    result = -1,
                    msg = "無法取得資料"
                });
            }

        }

        //填報內容清單
        public JsonResult GetRecords(int id)
        {
            List<SuperviseFillVModel> lists = iService.GetRecords<SuperviseFillVModel>(id);

            decimal q = 60, w = 20, p = 20;
            foreach (SuperviseFillVModel item in lists)
            {
                item.committeeList = iService.GetRecordCommittes(item.SuperviseEngSeq.Value, item.Seq);
                if (item.MissingNo.IndexOf("4.") == 0)
                    q -= item.DeductPoint;
                else if (item.MissingNo.IndexOf("5.") == 0)
                    w -= item.DeductPoint;
                else if (item.MissingNo.IndexOf("6.") == 0)
                    p -= item.DeductPoint;
            }

            decimal ts = -1;
            if (lists.Count > 0)
            {
                if (q < 0) q = 0;
                if (w < 0) w = 0;
                if (p < 0) p = 0;
                ts = q + w + p;
            }
            return Json(new
            {
                result = 0,
                totalScore = ts,
                items = lists
            });
        }
        public JsonResult NewRecord()
        {
            return Json(new
            {
                result = 0,
                item = new SuperviseFillVModel() { Seq = -1 }
            });
        }
        public JsonResult UpdateRecords(SuperviseFillVModel m)
        {
            int state;
            if (m.Seq == -1)
                state = iService.AddRecords(m);
            else
                state = iService.UpdateRecords(m);
            if (state == 0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "儲存成功"
                }); ;
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
        //督導紀錄表
        public ActionResult dnSRSheet(int id)
        {
            string filename = Utils.CopyTemplateFile("督導填報-督導紀錄表.xlsx", ".xlsx");
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
                Microsoft.Office.Interop.Excel.Worksheet sheet = dict["督導紀錄"];
                List<ESSuperviseRecordSheetVModel> items = iService.GetSuperviseRecordSheet<ESSuperviseRecordSheetVModel>(id);
                List<SuperviseFillVModel> fills = iService.GetRecords<SuperviseFillVModel>(id);
                if (items.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "資料錯誤"
                    }, JsonRequestBehavior.AllowGet);
                }

                ESSuperviseRecordSheetVModel item = items[0];
                string str = "□工程施工督導□走動式督導□專案督導 紀錄";
                string mode = "__", org = "___", superviseDate = "______", planNo = "___";
                switch (item.SuperviseMode)
                {
                    case 0: str = "■工程施工督導□走動式督導□專案督導 紀錄";
                        mode = "SD";
                        break;
                    case 1: str = "□工程施工督導■走動式督導□專案督導 紀錄";
                        mode = "SE"; 
                        break;
                    case 2: str = "□工程施工督導□走動式督導■專案督導 紀錄";
                        mode = "SG"; 
                        break;
                }
                sheet.Cells[2, 2] = str;
                if(item.SuperviseDate.HasValue)
                {
                    superviseDate = String.Format("{0}{1:00}{2:00}", item.SuperviseDate.Value.Year-1911, item.SuperviseDate.Value.Month, item.SuperviseDate.Value.Day);
                }
                sheet.Cells[3, 1] = String.Format("紀錄編號：{0}-{1}-{2}-{3}-{4}",
                    superviseDate, mode
                    , String.IsNullOrEmpty(item.ExecUnitCode) ? "_orgNo_" : item.ExecUnitCode
                    , String.IsNullOrEmpty(item.ProjectNo) ? "_planNo_" : item.ProjectNo
                    , item.TenderNo);
                sheet.Cells[4, 2] = item.TenderName;
                sheet.Cells[4, 7] = item.OrganizerName;
                sheet.Cells[5, 7] = item.ContactName;
                string sd = "";// "@督導日期" s20230316
                if (item.SuperviseEndDate.HasValue)
                {
                    sd = String.Format("{0}~{1}",
                        Utils.ChsDate(item.SuperviseDate),
                        Utils.ChsDate(item.SuperviseEndDate));
                }
                else
                {
                    sd = Utils.ChsDate(item.SuperviseDate);
                }
                sheet.Cells[6, 2] = sd;
                sheet.Cells[6, 5] = item.Location;
                sheet.Cells[6, 7] = item.SupervisionUnitName;
                if (!String.IsNullOrEmpty(item.SupervisorSelfPerson1) && !String.IsNullOrEmpty(item.SupervisorCommPerson1))
                {
                    str = item.SupervisorSelfPerson1 + "、" + item.SupervisorCommPerson1;
                } else
                {
                    str = item.SupervisorSelfPerson1 + item.SupervisorCommPerson1;
                }
                sheet.Cells[7, 7] = str;
                sheet.Cells[8, 2] = Utils.ChsDateFormat(item.ActualStartDate);
                sheet.Cells[8, 5] = Utils.ChsDateFormat(item.ScheCompletionDate);
                sheet.Cells[8, 7] = item.ContractorName1;
                sheet.Cells[9, 2] = String.Format("{0}%", item.PDAccuScheProgress.HasValue ? item.PDAccuScheProgress.Value.ToString() : "");
                sheet.Cells[9, 5] = String.Format("{0}%", item.PDAccuActualProgress.HasValue ? item.PDAccuActualProgress.Value.ToString() : "");
                sheet.Cells[9, 7] = String.Format("{0}%", item.DiffProgress.HasValue ? item.DiffProgress.Value.ToString() : "");
                sheet.Cells[9, 9] = item.ImproveDeadline;
                sheet.Cells[10, 2] = item.EngOverview;
                sheet.Cells[10, 7] = String.Format("{0}仟元", item.BidAmount.HasValue ? item.BidAmount.Value.ToString("N") : "");
                sheet.Cells[11, 2] = item.CommitteeList;
                sheet.Cells[11, 7] = item.CommitteeAverageScore.HasValue ? item.CommitteeAverageScore.Value.ToString() : "";
                sheet.Cells[30, 2] = item.Inspect;
                if (item.DeductPoints.HasValue) sheet.Cells[31, 2] = item.DeductPoints.Value;

                if(fills.Count >0)
                {
                    Microsoft.Office.Interop.Excel.Range excelRange;
                    int mergeStart = 0, mergeEnd = 0;
                    List<SuperviseFillVModel> missingNo = new List<SuperviseFillVModel>();
                    int row = 12, tarRow = 12, inx = 1;
                    GetMissingNo("0.01", fills, ref missingNo);
                    GetMissingNo("0.02", fills, ref missingNo);
                    GetMissingNo("0.03", fills, ref missingNo);
                    GetMissingNo("0.04", fills, ref missingNo);
                    GetMissingNo("0.05", fills, ref missingNo);
                    inx = WriteRow(sheet, inx, row, missingNo, -1);
                    //主辦機關：
                    inx = 1;
                    tarRow += (missingNo.Count < 3 ? 5 : (missingNo.Count < 2 ? missingNo.Count+4 : missingNo.Count + 3));//s20230320
                    mergeStart = tarRow - 3;
                    row = tarRow;
                    missingNo = new List<SuperviseFillVModel>();
                    GetMissingNo("4.01", fills, ref missingNo);
                    inx = WriteRow(sheet, inx, row, missingNo);

                    //監造單位：
                    tarRow += (missingNo.Count == 0 ? 2 : missingNo.Count + 1);
                    row = tarRow;
                    missingNo = new List<SuperviseFillVModel>();
                    GetMissingNo("4.02", fills, ref missingNo);
                    inx = WriteRow(sheet, inx, row, missingNo);
 
                    //B、承攬廠商：
                    tarRow += (missingNo.Count == 0 ? 2 : missingNo.Count + 1);
                    row = tarRow;
                    missingNo = new List<SuperviseFillVModel>();
                    GetMissingNo("4.03", fills, ref missingNo);
                    inx = WriteRow(sheet, inx, row, missingNo);

                    //二、施工品質：
                    tarRow += (missingNo.Count == 0 ? 2 : missingNo.Count + 1);
                    row = tarRow;
                    missingNo = new List<SuperviseFillVModel>();
                    GetMissingNo("5.", fills, ref missingNo);
                    inx = WriteRow(sheet, inx, row, missingNo);

                    //三、施工進度：
                    tarRow += (missingNo.Count == 0 ? 2 : missingNo.Count + 1);
                    row = tarRow;
                    missingNo = new List<SuperviseFillVModel>();
                    GetMissingNo("6.01", fills, ref missingNo);
                    inx = WriteRow(sheet, inx, row, missingNo);

                    //四、規劃設計建議：
                    tarRow += (missingNo.Count == 0 ? 2 : missingNo.Count + 1);

                    //缺點 合併
                    mergeEnd = tarRow - 2;
                    excelRange = sheet.Range[sheet.Cells[mergeStart, 1], sheet.Cells[mergeEnd, 1]];
                    excelRange.Merge(0);
                    mergeStart = tarRow - 1;

                    inx = 1;
                    row = tarRow;
                    missingNo = new List<SuperviseFillVModel>();
                    GetMissingNo("7.", fills, ref missingNo);
                    inx = WriteRow(sheet, inx, row, missingNo);

                    //五、其他建議：
                    tarRow += (missingNo.Count == 0 ? 2 : missingNo.Count + 1);
                    //規劃設計建議 合併
                    mergeEnd = tarRow - 2;
                    excelRange = sheet.Range[sheet.Cells[mergeStart, 1], sheet.Cells[mergeEnd, 1]];
                    excelRange.Merge(0);
                    mergeStart = tarRow - 1;

                    inx = 1;
                    row = tarRow;
                    missingNo = new List<SuperviseFillVModel>();
                    GetMissingNo("8.01", fills, ref missingNo);
                    inx = WriteRow(sheet, inx, row, missingNo);
                    tarRow += (missingNo.Count == 0 ? 2 : missingNo.Count + 1);
                    //其他建議 合併
                    mergeEnd = tarRow - 2;
                    excelRange = sheet.Range[sheet.Cells[mergeStart, 1], sheet.Cells[mergeEnd, 1]];
                    excelRange.Merge(0);
                }

                /*
                

                excelRange = sheet.Range[sheet.Cells[4, 1], sheet.Cells[row - 1, 30]];
                excelRange.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                excelRange.Borders.ColorIndex = 1;*/

                workbook.Save();
                workbook.Close();
                appExcel.Quit();

                Stream iStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", String.Format("{0}-{1}.xlsx", items[0].PhaseCode, items[0].TenderName));
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
        private int WriteRow(Microsoft.Office.Interop.Excel.Worksheet sheet, int inx, int row, List<SuperviseFillVModel> missingNo)
        {
            return WriteRow(sheet, inx, row, missingNo, 0);
        }
        private int WriteRow(Microsoft.Office.Interop.Excel.Worksheet sheet, int inx, int row, List<SuperviseFillVModel> missingNo, int rowAadj)
        {
            Microsoft.Office.Interop.Excel.Range excelRange;
            for (int i = 1; i < (missingNo.Count+ rowAadj); i++)
            {
                sheet.Rows[row + 1].Insert();
            }
            foreach (SuperviseFillVModel m in missingNo)
            {
                excelRange = sheet.Range[sheet.Cells[row, 3], sheet.Cells[row, 9]];
                excelRange.Merge(0);
                sheet.Cells[row, 2] = String.Format("{0}、", inx);
                sheet.Cells[row, 3] = String.Format("{0}({1})", m.SuperviseMemo, m.MissingNo);
                
                row++;
                inx++;
            }
            return inx;
        }
        private void GetMissingNo(string MissingNo, List<SuperviseFillVModel> fills, ref List<SuperviseFillVModel> missingNo)
        {
            List<SuperviseFillVModel> filters = new List<SuperviseFillVModel>();
            foreach(SuperviseFillVModel m in fills)
            {
                if (m.MissingNo.IndexOf(MissingNo) == 0)
                    missingNo.Add(m);
            }
        }
    }
}