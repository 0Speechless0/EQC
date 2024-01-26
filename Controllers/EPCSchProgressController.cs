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
    public class EPCSchProgressController : Controller
    {//工程管理 - 預定進度
        SchProgressPayItemService iService = new SchProgressPayItemService();
        //s20230720
        public JsonResult UpdateProgesss(SchProgressPayItemModel m, int id, string tarDate, int ver)
        {
            int dateInx = 0;
            List<EPCSchProgressVModel> dateList = iService.GetDateListVer<EPCSchProgressVModel>(id);
            foreach(EPCSchProgressVModel item in dateList)
            {
                if (item.ItemDate == tarDate)
                {
                    break;
                }
                else
                    dateInx++;
            }
            //
            SchProgressPayItemModel previousProgress = null;
            SchProgressPayItemModel editProgress = null;
            SchProgressPayItemModel nextProgress = null;

            List<SchProgressPayItemModel> pList = iService.GetProgressList<SchProgressPayItemModel>(id, tarDate, ver);
            int progressInx = 0;
            foreach (SchProgressPayItemModel item in pList)
            {
                if (item.Seq == m.Seq)
                {
                    editProgress = item;
                    break;
                }
                else
                    progressInx++;
            }
            if(editProgress == null)
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料讀取異常, 無法更新"
                });
            }
            editProgress.SchProgress = m.SchProgress;
            editProgress.DayProgress = 0;

            if (dateInx > 0)
            {
                pList = iService.GetProgressList<SchProgressPayItemModel>(id, dateList[dateInx-1].ItemDate, ver);
                if(pList.Count>0) previousProgress = pList[progressInx];

            }
            if ((dateInx+1) < dateList.Count)
            {
                pList = iService.GetProgressList<SchProgressPayItemModel>(id, dateList[dateInx + 1].ItemDate, ver);
                if (pList.Count > 0) nextProgress = pList[progressInx];
            }
            if (previousProgress != null)
            {
                if (previousProgress.Days > 0)
                    previousProgress.DayProgress = (editProgress.SchProgress - previousProgress.SchProgress) / previousProgress.Days;
                else
                    previousProgress.DayProgress = 0;
            }
            if (nextProgress != null)
            {
                if (editProgress.Days > 0)
                    editProgress.DayProgress = (nextProgress.SchProgress - editProgress.SchProgress) / editProgress.Days;
                    
            } 

            if(iService.UpdateProgress(ver, previousProgress, editProgress))
            {
                return Json(new
                {
                    result = 0,
                    msg = "更新成功"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "更新失敗"
            });
        }
        // shioulo20221216
        public JsonResult GetEngItem(int id)
        {
            List<EPCSchProgressV1Model> items = iService.GetEngMainBySeq<EPCSchProgressV1Model>(id);
            if (items.Count == 1)
            {
                return Json(new
                {
                    result = 0,
                    item = items[0]
                });
            }
            else
            {
                return Json(new
                {
                    result = 1,
                    message = "讀取資料錯誤"
                });
            }
        }
        // shioulo20221216
        public JsonResult UpdateEngDates(EPCSchProgressV1Model engMain)
        {
            engMain.updateDate();

            int result = iService.UpdateEngDates(engMain);
            if (result >= 1)
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存成功"
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "儲存失敗"
                });
            }
        }
        //刪除進度資料
        public JsonResult DelProgress(int id)
        {
            List<EPCSchProgressHeaderVModel> spHeaders = iService.GetHeaderList<EPCSchProgressHeaderVModel>(id);
            if (spHeaders.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "預定進度資料錯誤"
                });
            }
            EPCSchProgressHeaderVModel spHeader = spHeaders[0];
            if (spHeader.SPState != 0 || spHeader.EngChangeCount > 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "進度非許可狀態或已工變過不能刪除"
                });
            }

            List<EPCSupDailyDate1VModel> supDailyDateList = iService.GetSupDailyDateCount<EPCSupDailyDate1VModel>(id);
            if (supDailyDateList.Count > 0 && supDailyDateList[0].dailyCount > 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "已有日誌資料無法刪除"
                });
            }
            if (iService.DelProgress(id))
            {
                return Json(new
                {
                    result = 0,
                    msg = "刪除完成"
                });
            } else
            {
                return Json(new
                {
                    result = -1,
                    msg = "刪除失敗"
                });
            }
        }

        //工程變更
        //工程變更執行
        public JsonResult ExecEngChangeDate(int id)
        {
            EPCSchProgressChangeDateVModel item = new EPCSchProgressChangeDateVModel();
            if (CheckEngChangeData(id, ref item) < 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程變更資料錯誤"
                });
            }
            List<EPCSchProgressHeaderVModel> spHeaders = iService.GetHeaderList<EPCSchProgressHeaderVModel>(id);
            if (spHeaders.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "預定進度資料錯誤[0]"
                });
            }
            EPCSchProgressHeaderVModel spHeader = spHeaders[0];
            if(spHeader.SPState == 0 || spHeader.EngChangeState == 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "預定進度非許可狀態"
                });
            }
            spHeader.EngChangeStartDate = item.EngChangeStartDate;
            spHeader.EngChangeSchCompDate = item.ScheChangeCloseDate;

            //工程預定進度
            List<SchProgressHeaderHistoryProgressModel> engSchProgress = new List<SchProgressHeaderHistoryProgressModel>();
            
            if (spHeader.EngChangeCount == 0)
            {//首次工變才紀錄, 工程預定進度
                List<EPCProgressRSchVModel> schProgress = new EPCProgressReportService().GetSchProgress<EPCProgressRSchVModel>(id);
                if (schProgress.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "預定進度資料錯誤[1]"
                    });
                }
                engSchProgress = EngSchProcessCal(schProgress);
            }

            //計算需填寫的日期
            List<DateTime> dateList = DateReportCal(spHeader.EngChangeStartDate.Value, spHeader.EngChangeSchCompDate.Value);
            List<SchProgressPayItemModel> ceList = iService.GetList<SchProgressPayItemModel>(id, spHeader.EngChangeStartDate.Value.ToString("yyyy-MM-dd"));
            if(ceList.Count > 0 )
            {//表示與現有日期重疊
                dateList.Remove(spHeader.EngChangeStartDate.Value);
            } else
            {
                ceList = iService.GetListMinDate<SchProgressPayItemModel>(id);
            }
            if(ceList.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "預定進度資料錯誤[2]"
                });
            }
            if (!iService.EngChangeAddItems(spHeader, ceList, dateList, engSchProgress))
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程變更資料調整失敗"
                });
            }

            return Json(new
            {
                result = 0,
                msg = "工程變更完成, 請進行預定進度設定"
            });
        }
        //半月報日期
        private List<DateTime> DateReportCal(DateTime startDate, DateTime endDate)
        {
            List<DateTime> dateList = new List<DateTime>();

            int year = startDate.Year;
            int month = startDate.Month;
            DateTime day15 = new DateTime(year, month, 15);
            DateTime dayLast = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
            if (startDate.Day < 15 || (startDate.Day > 15 && startDate.Day < dayLast.Day))
            {//預定開工日期
                dateList.Add(startDate);
            }
            while (day15 <= endDate || dayLast <= endDate)
            {
                if (day15 >= startDate && day15 <= endDate)
                {
                    //System.Diagnostics.Debug.WriteLine(day15.ToString("yyyy-MM-dd"));
                    dateList.Add(day15);
                }
                if (dayLast >= startDate && dayLast <= endDate)
                {
                    //System.Diagnostics.Debug.WriteLine(dayLast.ToString("yyyy-MM-dd"));
                    dateList.Add(dayLast);
                }
                month++;
                if (month > 12)
                {
                    month = 1;
                    year++;
                }
                day15 = new DateTime(year, month, 15);
                dayLast = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
                if (day15 > endDate && dayLast > endDate) break;
            }
            //if (endDate.Day < 15 || (endDate.Day > 15 && endDate.Day < dayLast.Day))
            if (endDate > dateList[dateList.Count - 1])
            {//預定完工日期
                dateList.Add(endDate);
            }

            return dateList;
        }
        //工程預定進度
        private List<SchProgressHeaderHistoryProgressModel> EngSchProcessCal(List<EPCProgressRSchVModel> schProgress)
        {
            List<SchProgressHeaderHistoryProgressModel> engSchProgress = new List<SchProgressHeaderHistoryProgressModel>();

            int days;
            decimal todayProgress = 0;
            schProgress[0].dayProgress = schProgress[0].rate;
            for (int i = 1; i < schProgress.Count; i++)
            {
                EPCProgressRSchVModel preM = schProgress[i - 1];
                EPCProgressRSchVModel m = schProgress[i];
                days = m.itemDate.Subtract(preM.itemDate).Days;
                m.dayProgress = (m.rate - preM.rate) / (days);
            }

            DateTime sDate = schProgress[0].itemDate;
            decimal dayProgress = schProgress[0].dayProgress;
            DateTime maxDate = schProgress[schProgress.Count - 1].itemDate;
            days = maxDate.Subtract(sDate).Days + 1;
            for (int i = 0; i < days; i++)
            {
                SchProgressHeaderHistoryProgressModel m = new SchProgressHeaderHistoryProgressModel();
                m.ProgressDate = sDate;
                //預定累計完成
                todayProgress += dayProgress;
                m.SchProgress = todayProgress;
                if (schProgress.Count > 0 && sDate == schProgress[0].itemDate)
                {
                    schProgress.RemoveAt(0);
                    if (schProgress.Count > 0)
                        dayProgress = schProgress[0].dayProgress;
                    else
                        dayProgress = 0;
                }
                engSchProgress.Add(m);

                sDate = sDate.AddDays(1);
            }

            return engSchProgress;
        }
        //工程變更日期檢查
        public JsonResult CheckEngChangeDate(int id, EPCSchProgressV1Model engMain)
        {
            engMain.updateDate();
            iService.UpdateEngChangeStartDate(engMain); //shioulo 20221228

            EPCSchProgressChangeDateVModel item = new EPCSchProgressChangeDateVModel();
            int result = CheckEngChangeData(id, ref item);
            string msg = "";
            switch (result)
            {
                case -1:
                    msg = "資料錯誤";
                    break;
                case -2:
                    msg = "請輸入工程變更日期<br>與工程會標案管理系統輸入變更後預定完工日期";
                    break;
                case -3:
                    msg = "工程變更日期<br>已過期無法作業";
                    break;
                case -4:
                    msg = "工程變更:預定完工日期錯誤<br>無法作業";
                    break;
            }
            if (result < 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = msg
                });
            }
            
            msg = String.Format(@"
                            <div style='text-align:left'>工程變更日期: <font color='red'>{0}</font></div>
                            <div style='text-align:left'>預定完工日期: <font color='red'>{1}</font></div>
                            <div style='text-align:left'>1. 需要重新匯入pcces, 若單價,金額有變更</div>
                            <div style='text-align:left'>2. 所有 {0}(含) 之後的預定進度及所有估驗請款 全部會被刪除.</div>
                            ",
                item.EngChangeStartDateStr, item.ScheChangeCloseDateStr);
            return Json(new
            {
                result = 0,
                msg = msg
            });
        }
        private int CheckEngChangeData(int id, ref EPCSchProgressChangeDateVModel item)
        {
            List<EPCSchProgressChangeDateVModel> items = iService.GetEngChangeDate<EPCSchProgressChangeDateVModel>(id);
            if (items.Count == 0)
            {
                return -1; //資料錯誤
            }
            item = items[0];
            if (!item.EngChangeStartDate.HasValue || !item.ScheChangeCloseDate.HasValue)
            {
                return -2; //請至管考頁面輸入工程變更日期<br>至工程會標案管理系統輸入變更後預定完工日期
            }
            /* shioulo 20230201 因補資料先行取消
            if (item.EngChangeStartDate.Value.Subtract(DateTime.Now).Days < 0)
            {
                return -3; //工程變更日期<br>已過期無法作業
            }*/
            if (item.ScheChangeCloseDate.Value.Subtract(item.EngChangeStartDate.Value).Days <= 0)
            {
                return -4; //工程變更:預定完工日期錯誤<br>無法作業
            }

            return 0;
        }

        //填報完成
        public JsonResult FillCompleted(int id)
        {
            List<EPCProgressEngChangeListVModel> engChanges = new SchEngChangeService().GetEngChangeList<EPCProgressEngChangeListVModel>(id);
            if (engChanges.Count == 0)
            {
                if (!iService.CheckLastProgress(id))
                {//s20230227
                    return Json(new
                    {
                        result = -1,
                        msg = "請修正進度\n工作項目分配之累計進度值須達到100% (-1 工作項目除外)"
                    });
                }
            } else
            {
                if (!iService.CheckLastProgressForEngChange(engChanges[engChanges.Count-1].Seq))
                {//s20230412
                    return Json(new
                    {
                        result = -1,
                        msg = "請修正 工程變更 進度\n工作項目分配之累計進度值須達到100% (-1 工作項目除外)"
                    });
                }
            }
            if (iService.SetState(id, 1, 0)>0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "填報已完成"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "變更失敗"
            });
        }
        //日期清單
        public JsonResult GetDateList(int id)
        {
            List<EPCSchProgressVModel> dList = iService.GetDateListVer<EPCSchProgressVModel>(id);
            if(dList.Count == 0)
            {
                List<EPCTendeVModel> engItems = new EPCProgressManageService().GetEngLinkTenderBySeq<EPCTendeVModel>(id);
                if (engItems.Count != 1)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "工程資料錯誤"
                    });
                }
                List<SchEngProgressPayItemModel> payItemList = new SchEngProgressService().GetList<SchEngProgressPayItemModel>(id, 9999, 1, "", "");
                if (payItemList.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "碳排量計算 查無資料"
                    });
                }
                EPCTendeVModel eng = engItems[0];
                if(!eng.StartDate.HasValue || !eng.SchCompDate.HasValue)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "工程期間資料錯誤"
                    });
                }
                //計算需填寫的日期
                List<DateTime> dateList = new List<DateTime>();
                int year = eng.StartDate.Value.Year;
                int month = eng.StartDate.Value.Month;
                DateTime day15 = new DateTime(year, month, 15);
                DateTime dayLast = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
                if (eng.StartDate.Value.Day < 15 || (eng.StartDate.Value.Day > 15 && eng.StartDate.Value.Day < dayLast.Day))
                {//預定開工日期
                    dateList.Add(eng.StartDate.Value);
                }
                while (day15 <= eng.SchCompDate.Value || dayLast <= eng.SchCompDate.Value)
                {
                    if (day15 >= eng.StartDate.Value && day15 <= eng.SchCompDate.Value)
                    {
                        //System.Diagnostics.Debug.WriteLine(day15.ToString("yyyy-MM-dd"));
                        dateList.Add(day15);
                    }
                    if (dayLast >= eng.StartDate.Value && dayLast <= eng.SchCompDate.Value)
                    {
                        //System.Diagnostics.Debug.WriteLine(dayLast.ToString("yyyy-MM-dd"));
                        dateList.Add(dayLast);
                    }
                    month++;
                    if(month>12)
                    {
                        month = 1;
                        year++;
                    }
                    day15 = new DateTime(year, month, 15);
                    dayLast = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
                    if (day15 > eng.SchCompDate.Value && dayLast > eng.SchCompDate.Value) break;
                }
                //if (eng.SchCompDate.Value.Day < 15 || (eng.SchCompDate.Value.Day > 15 && eng.SchCompDate.Value.Day < dayLast.Day))
                if (eng.SchCompDate.Value > dateList[dateList.Count-1])
                {//預定完工日期
                    dateList.Add(eng.SchCompDate.Value);
                }

                if (!iService.AddItems(id, payItemList, dateList))
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "資料初始化失敗"
                    });
                }
                dList = iService.GetDateListVer<EPCSchProgressVModel>(id);
            }
            if (dList.Count > 0)
            {
                return Json(new
                {
                    result = 0,
                    items = dList
                });
            }

            return Json(new
            {
                result = -1,
                msg = "資料讀取錯誤"
            });
        }
        /// <summary>
        /// 該日期項目清單
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tarDate"></param>
        /// <param name="ver">版本</param> s20230411
        /// <returns></returns>
        public JsonResult GetList(int id, string tarDate, int ver)
        {
            bool canEdit = false; //s20230720
            List<EPCProgressEngChangeListVModel> engChanges = new SchEngChangeService().GetEngChangeList<EPCProgressEngChangeListVModel>(id);
            if (engChanges.Count == 0)
            {
                canEdit = true;
            } else
            {
                EPCProgressEngChangeListVModel engChange = engChanges[engChanges.Count-1];
                DateTime tD = DateTime.Parse(tarDate).Date;
                if(engChange.StartDate.HasValue)
                {
                    canEdit = (tD.Subtract(engChange.StartDate.Value).Days >=0);
                }
            }

            List<SchProgressPayItemModel> ceList = iService.GetListVer<SchProgressPayItemModel>(id, tarDate, ver);
            return Json(new
            {
                result = 0,
                items = ceList,
                canEdit = canEdit
            });
        }
        public JsonResult GetSPHeader(int id)
        {
            List<EPCSchProgressHeaderVModel> list = iService.GetHeaderList<EPCSchProgressHeaderVModel>(id);
            if (list.Count == 1)
            {
                return Json(new
                {
                    result = 0,
                    item = list[0]
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "查無資料"
                });
            }
        }
        //工程
        public virtual JsonResult GetEngMain(int id)
        {
            List<EngMainEditVModel> items = new EngMainService().GetItemBySeq<EngMainEditVModel>(id);
            if (items.Count == 1)
            {
                return Json(new
                {
                    result = 0,
                    item = items[0]
                });
            }
            else if (items.Count > 1)
            {
                return Json(new
                {
                    result = 1,
                    msg = "資料錯誤: 對應到多個工程資料"
                });
            }
            else
            {
                return Json(new
                {
                    result = 2,
                    msg = "讀取資料錯誤"
                });
            }
        }

        //下載工程範本(excel)
        public ActionResult Download(int id)
        {
            List<EngMainEditVModel> items = new EngMainService().GetItemBySeq<EngMainEditVModel>(id);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EngMainEditVModel eng = items[0];

            List<EPCSchProgressHeaderVModel> spHeaders = iService.GetHeaderList<EPCSchProgressHeaderVModel>(id);
            if (spHeaders.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "預定進度資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EPCSchProgressHeaderVModel spHeader = spHeaders[0];

            List<EPCSchProgressVModel> dateList = iService.GetDateList<EPCSchProgressVModel>(id);
            if (dateList.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "查無日期資料"
                }, JsonRequestBehavior.AllowGet);
            }
            List<SchProgressPayItemModel> spList = iService.GetList<SchProgressPayItemModel>(id, dateList[0].ItemDate);
            if (spList.Count==0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "查無資料"
                }, JsonRequestBehavior.AllowGet);
            }
            //工程變更清單 s20230411
            List<EPCProgressEngChangeListVModel> secList = new SchEngChangeService().GetEngChangeList<EPCProgressEngChangeListVModel>(id);

            return CreateExcel(eng, spList, dateList, spHeader, secList);
        }
        private ActionResult CreateExcel(EngMainEditVModel eng, List<SchProgressPayItemModel> spList, List<EPCSchProgressVModel> dateList, EPCSchProgressHeaderVModel spHeader, List<EPCProgressEngChangeListVModel> secList)
        {
            string filename = Utils.CopyTemplateFile("進度管理-工程範本.xlsx", ".xlsx");
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
                Worksheet sheet = dict["預定進度"];
                PayitemSheet(sheet, eng, spList);
                DateSheet(sheet, eng, dateList, spHeader, secList);
                sheet.Protect(String.Format("{0}-{1}",eng.EngNo,eng.Seq));//shioulo20221213

                workbook.Save();
                workbook.Close();
                appExcel.Quit();

                return DownloadFile(filename, eng.EngNo);
            }
            catch (Exception e)
            {
                if (workbook != null) workbook.Close(false);
                if (appExcel != null) appExcel.Quit();
                return Json(new
                {
                    result = -1,
                    message = "Excel 製表失敗\n"+e.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //日期,預計數量
        private void DateSheet(Worksheet sheet, EngMainEditVModel eng, List<EPCSchProgressVModel> dateList, EPCSchProgressHeaderVModel spHeader, List<EPCProgressEngChangeListVModel> secList)
        {
            if (sheet == null) return;

            int col = 8;
            foreach (EPCSchProgressVModel m in dateList)
            {
                if (spHeader.EngChangeState == 1)
                {//工變起始日期前之進度不變更 20220903
                    sheet.Cells[3, 9] = String.Format("{0} 日期以前 累計預定進度 不能修改(無效)", spHeader.EngChangeStartDate.Value.ToString("yyyy-MM-dd"));
                    //sheet.Cells[3, 7].Interior.Color = sheet.Cells[1, 1].Interior.Color;
                }
                int row = 4;
                sheet.Cells[row, col].NumberFormat = "M/d";
                sheet.Cells[row, col].Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                sheet.Cells[row, col].VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                sheet.Cells[row, col] = m.ItemDate;
                row++;
                sheet.Cells[row, col] = "累計預定進度%";
                sheet.Cells[row, col].WrapText = true;
                foreach (SchProgressPayItemModel p in iService.GetList<SchProgressPayItemModel>(eng.Seq, m.ItemDate))
                {
                    row++;
                    sheet.Cells[row, col] = p.SchProgress;
                    sheet.Cells[row, col].Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                    if ( (spHeader.EngChangeState == 1 && m.SPDate.Subtract(spHeader.EngChangeStartDate.Value).Days < 0) || secList.Count>0)
                    {
                        sheet.Cells[row, col].Interior.Color = sheet.Cells[1, 1].Interior.Color;
                    }
                }
                col++;
            }
        }
        //payitem
        private void PayitemSheet(Worksheet sheet, EngMainEditVModel eng, List<SchProgressPayItemModel> spList)
        {
            if (sheet == null) return;

            sheet.Cells[2, 3] = eng.EngName;// "@工程名稱";
            sheet.Cells[3, 3] = eng.EngNo;// "@標案編號";

            int row = 6;
            foreach(SchProgressPayItemModel m in spList)
            {
                sheet.Cells[row, 1] = m.OrderNo;
                sheet.Cells[row, 2] = m.PayItem.Trim();// === 會與 Excel 通用格式衝突";
                sheet.Cells[row, 3] = m.Description;// 項目及說明
                sheet.Cells[row, 4] = m.Unit;
                sheet.Cells[row, 5] = m.Quantity;
                sheet.Cells[row, 6] = m.Price;
                sheet.Cells[row, 7] = m.Amount;
                row++;
            }
        }

        //匯入 更新excel
        public JsonResult UploadSchProgress(int id)
        {
            List<EngMainEditVModel> engs = new EngMainService().GetItemBySeq<EngMainEditVModel>(id);
            if (engs.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "工程資料錯誤"
                });
            }
            EngMainEditVModel eng = engs[0];

            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                string fileName;
                try
                {
                    fileName = SaveFile(file);
                } catch {
                    return Json(new
                    {
                        result = -1,
                        message = "上傳檔案儲存失敗"
                    });
                }
                List<EPCSchProgressHeaderVModel> spHeaders = iService.GetHeaderList<EPCSchProgressHeaderVModel>(id);
                if (spHeaders.Count != 1)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "預定進度資料錯誤"
                    });
                }
                EPCSchProgressHeaderVModel spHeader = spHeaders[0];

                //s20230411
                List<EC_SchEngProgressHeaderModel> engChanges = new SchEngChangeService().GetEngChangeList<EC_SchEngProgressHeaderModel>(id);
                int result;
                if (engChanges.Count == 0)
                {
                    //List<SchProgressPayItemModel> items = new List<SchProgressPayItemModel>();
                    result = readExcelData(fileName, eng, /*items,*/ spHeader);
                } else {//工程變更
                    result = readExcelDataForEngChange(fileName, eng, spHeader, engChanges[engChanges.Count-1]); 
                }
                if(result < 0)
                {
                    string msg;
                    switch(result)
                    {
                        case -1: msg = "Excel解析發生錯誤"; break;
                        case -2: msg = "工程名稱/編號 資料錯誤"; break;
                        case -3: msg = "施工項目 資料錯誤"; break;
                        case -4: msg = "日期項目 資料錯誤"; break;
                        case -5: msg = "更新資料錯誤"; break;
                        default:
                            msg = "未知錯誤:"+result.ToString(); break;
                    }
                    return Json(new
                    {
                        result = -1,
                        message = msg
                    });
                }

                return Json(new
                {
                    result = 0,
                    message = "更新完成"
                }); ;

            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }
        //工程變更 - 工程範本 s20230411
        private int readExcelDataForEngChange(string filename, EngMainEditVModel eng, EPCSchProgressHeaderVModel spHeader, EC_SchEngProgressHeaderModel engChangeHeader)
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
                Worksheet sheet = dict["預定進度"];

                if (sheet.Cells[2, 3].Value.ToString() != eng.EngName || sheet.Cells[3, 3].Value.ToString() != eng.EngNo || sheet.Cells[1, 8].Value.ToString() != engChangeHeader.Seq.ToString())
                {
                    workbook.Close();
                    appExcel.Quit();
                    return -2;
                }

                SchEngChangeService schEngChangeService = new SchEngChangeService();

                //檢查 日期 是否一致
                List<EPCSchProgressVModel> dateList = schEngChangeService.GetDateList<EPCSchProgressVModel>(engChangeHeader.Seq);
                int row = 4;
                int col = 9;
                foreach (EPCSchProgressVModel m in dateList)
                {
                    col++;
                    DateTime d = DateTime.Parse(sheet.Cells[row, col].Value.ToString());
                    //System.Diagnostics.Debug.WriteLine(d.ToString("yyyy/M/d") + " "+ m.ItemDate);
                    if (d.ToString("yyyy/M/d") != m.ItemDate)
                    {
                        workbook.Close();
                        appExcel.Quit();
                        return -4;
                    }
                }

                //檢查 PayItem 是否一致
                List<EC_SchProgressPayItem1Model> payItems = schEngChangeService.GetSchProgressPayItemsAndDayBefore<EC_SchProgressPayItem1Model>(engChangeHeader.Seq, dateList[0].ItemDate);
                row = 6;
                foreach (EC_SchProgressPayItem1Model m in payItems)
                {
                    if(m.EC_SchEngProgressPayItemSeq != int.Parse(sheet.Cells[row, 8].Value.ToString()))
                    {
                        workbook.Close();
                        appExcel.Quit();
                        return -3;
                    }
                    m.SPDate = engChangeHeader.StartDate.Value;
                    m.SchProgress = m.SchProgressDayBefore.HasValue ? m.SchProgressDayBefore.Value : 0;
                    m.DayProgress = 0;
                    row++;
                }
                //
                col = 9;
                int dayCountLast = 0;
                bool fLast = false;
                List<EC_SchProgressPayItemModel> updateItems = new List<EC_SchProgressPayItemModel>();
                for (int mi = 0; mi < dateList.Count; mi++)
                {
                    EPCSchProgressVModel m = dateList[mi];
                    col++;
                    int inx = 0;
                    EC_SchProgressPayItemModel payItem = payItems[inx];

                    /*if (spHeader.EngChangeState == 1)
                    {//工變起始日期前之進度不變更 20220903
                        if (m.SPDate.Subtract(spHeader.EngChangeStartDate.Value).Days < 0)
                        {
                            payItem.SPDate = m.SPDate;
                            continue;
                        }
                    }*/

                    TimeSpan ts = m.SPDate.Subtract(payItem.SPDate); //兩時間天數相減
                    int dayCount = (int)ts.Days; //相距天數 + 1; //相距天數
                    if (dayCount <= 0) dayCount = 1;
                    //System.Diagnostics.Debug.WriteLine(m.SPDate.ToString("yyyy-MM-dd") + " " + payItem.SPDate.ToString("yyyy-MM-dd") + " d:" + dayCount.ToString());
                    payItem.SPDate = m.SPDate;
                    if ((mi + 1) == dateList.Count)
                    {
                        fLast = true;
                        ts = eng.SchCompDate.Value.Subtract(m.SPDate);
                        dayCountLast = (int)ts.Days; //相距天數 + 1;
                        if (dayCountLast <= 0) dayCountLast = 1;
                        //System.Diagnostics.Debug.WriteLine("dayCountLast:" + dayCountLast.ToString());
                    }

                    List<EC_SchProgressPayItemModel> schProgresses = schEngChangeService.GetSchProgressPayItemList<EC_SchProgressPayItemModel>(engChangeHeader.Seq, m.ItemDate);
                    //iService.GetList<SchProgressPayItemModel>(eng.Seq, m.ItemDate);
                    for (int pi = 0; pi < schProgresses.Count; pi++)
                    {
                        EC_SchProgressPayItemModel p = schProgresses[pi];
                        payItem = payItems[inx];
                        decimal schProgress;
                        if (Decimal.TryParse(sheet.Cells[p.OrderNo + 5, col].Value.ToString(), out schProgress))
                        {
                            p.Days = dayCount;
                            p.SchProgress = schProgress;
                            //計算每日進度
                            if (p.SchProgress < 0)
                                p.DayProgress = -1;
                            //else if(mi == 0)
                            //    p.DayProgress = 0;//工程變更第一天均已0帶入, 暫目前不抓取前一天因 Payitem 對應非一定
                            else
                                p.DayProgress = (p.SchProgress - payItem.SchProgress) / dayCount;
                            //System.Diagnostics.Debug.WriteLine(p.OrderNo.ToString() + " " + p.SchProgress.ToString() + " " + payItem.SchProgress.ToString() + " r:" + p.DayProgress.ToString());
                            payItem.SchProgress = p.SchProgress;

                            if (fLast)
                            {//計算到預定完工日期每日進度
                                if (dayCountLast == 0 || p.SchProgress <= 0 || p.SchProgress == 100)
                                    p.DayProgressAfter = 0;
                                else
                                {
                                    p.DayProgressAfter = (100 - p.SchProgress) / dayCountLast;
                                }
                            }
                            updateItems.Add(p);
                        }
                        else
                        {
                            //System.Diagnostics.Debug.WriteLine(m.ItemDate+" orderno:"+p.OrderNo + " v:" + sheet.Cells[row, col].Value.ToString());
                        }
                        inx++;
                    }

                }
                workbook.Close();
                appExcel.Quit();

                if (iService.UpdateSchProgressForEngChange(updateItems))
                    return 1;
                else
                    return -5;
            }
            catch (Exception e)
            {
                if (workbook != null) workbook.Close(false);
                if (appExcel != null) appExcel.Quit();
                return -1;
            }
        }
        //初始預定進度
        private int readExcelData(string filename, EngMainEditVModel eng, /*List<SchProgressPayItemModel> items, */EPCSchProgressHeaderVModel spHeader)
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
                Worksheet sheet = dict["預定進度"];

                if (sheet.Cells[2, 3].Value.ToString() != eng.EngName || sheet.Cells[3, 3].Value.ToString() != eng.EngNo)
                {
                    workbook.Close();
                    appExcel.Quit();//s20230411
                    return -2;
                }

                //檢查 PayItem 是否一致
                List<SchProgressPayItemModel> payItems = iService.GetPayitemList<SchProgressPayItemModel>(eng.Seq);
                int row = 6;
                foreach (SchProgressPayItemModel m in payItems)
                {
                    if (sheet.Cells[row, 1].Value.ToString() != m.OrderNo.ToString() || sheet.Cells[row, 2].Value.ToString() != m.PayItem
                        || sheet.Cells[row, 3].Value.ToString() != m.Description || sheet.Cells[row, 4].Value.ToString() != m.Unit)
                    {
                        workbook.Close();
                        appExcel.Quit();//s20230411
                        return -3;
                    }
                    m.SPDate = eng.StartDate.Value;
                    m.SchProgress = 0;
                    m.DayProgress = 0;
                    row++;
                }
                //檢查 日期 是否一致
                List<EPCSchProgressVModel> dateList = iService.GetDateList<EPCSchProgressVModel>(eng.Seq);
                row = 4;
                int col = 7;
                foreach (EPCSchProgressVModel m in dateList)
                {
                    col++;
                    DateTime d = DateTime.Parse(sheet.Cells[row, col].Value.ToString());
                    //System.Diagnostics.Debug.WriteLine(d.ToString("yyyy/M/d") + " "+ m.ItemDate);
                    if (d.ToString("yyyy/M/d") != m.ItemDate)
                    {
                        workbook.Close();
                        appExcel.Quit(); //s20230411
                        return -4;
                    }
                }
                //
                col = 7;
                int dayCountLast = 0;
                bool fLast = false;
                List<SchProgressPayItemModel> updateItems = new List<SchProgressPayItemModel>();
                for(int mi = 0; mi< dateList.Count; mi++ )
                {
                    EPCSchProgressVModel m = dateList[mi];
                    col++;
                    int inx = 0;
                    SchProgressPayItemModel payItem = payItems[inx];

                    if (spHeader.EngChangeState == 1)
                    {//工變起始日期前之進度不變更 20220903
                        if (m.SPDate.Subtract(spHeader.EngChangeStartDate.Value).Days < 0)
                        {
                            payItem.SPDate = m.SPDate;
                            continue;
                        }
                    }
                    
                    TimeSpan ts = m.SPDate.Subtract(payItem.SPDate); //兩時間天數相減
                    int dayCount = (int)ts.Days; //相距天數 + 1; //相距天數
                    if (dayCount <= 0) dayCount = 1;
                    //System.Diagnostics.Debug.WriteLine(m.SPDate.ToString("yyyy-MM-dd") + " " + payItem.SPDate.ToString("yyyy-MM-dd") + " d:" + dayCount.ToString());
                    payItem.SPDate = m.SPDate;
                    if( (mi+1) == dateList.Count)
                    {
                        fLast = true;
                        ts = eng.SchCompDate.Value.Subtract(m.SPDate); 
                        dayCountLast = (int)ts.Days; //相距天數 + 1;
                        if (dayCountLast <= 0) dayCountLast = 1;
                        //System.Diagnostics.Debug.WriteLine("dayCountLast:" + dayCountLast.ToString());
                    }

                    List<SchProgressPayItemModel> schProgresses = iService.GetList<SchProgressPayItemModel>(eng.Seq, m.ItemDate);
                    for(int pi=0; pi<schProgresses.Count; pi++)
                    {
                        SchProgressPayItemModel p = schProgresses[pi];
                        payItem = payItems[inx];
                        decimal schProgress;
                        //if (Decimal.TryParse(sheet.Cells[p.OrderNo + 5, col].Value.ToString(), out schProgress))
                        if (Decimal.TryParse(sheet.Cells[pi + 6, col].Value.ToString(), out schProgress)) //s20230818
                        {
                            p.Days = dayCount;
                            p.SchProgress = schProgress;
                            //計算每日進度
                            if (p.SchProgress < 0)
                                p.DayProgress = -1;
                            else
                                p.DayProgress = (p.SchProgress - payItem.SchProgress) / dayCount;
                            //System.Diagnostics.Debug.WriteLine(p.OrderNo.ToString() + " " + p.SchProgress.ToString() + " " + payItem.SchProgress.ToString() + " r:" + p.DayProgress.ToString());
                            payItem.SchProgress = p.SchProgress;

                            if (fLast)
                            {//計算到預定完工日期每日進度
                                if (dayCountLast == 0 || p.SchProgress <= 0 || p.SchProgress == 100)
                                    p.DayProgressAfter = 0;
                                else
                                {
                                    p.DayProgressAfter = (100 - p.SchProgress) / dayCountLast;
                                }
                            }
                            updateItems.Add(p);
                        } else
                        {
                            //System.Diagnostics.Debug.WriteLine(m.ItemDate+" orderno:"+p.OrderNo + " v:" + sheet.Cells[row, col].Value.ToString());
                        }
                        inx++;
                    }
                    
                }
                workbook.Close();
                appExcel.Quit();

                if (iService.UpdateSchProgress(updateItems))
                    return 1;
                else
                    return -5;
            }
            catch (Exception e)
            {
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

        //下載 PccesXML shioulo 20220712
        public ActionResult DownloadPccesXML(int id)
        {
            List<EPCSchProgressHeaderVModel> items = iService.GetHeaderList<EPCSchProgressHeaderVModel>(id);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EPCSchProgressHeaderVModel m = items[0];
            string fName = Path.Combine(Utils.GetEngMainFolder(m.EngMainSeq), m.PccesXMLFile);
            if (!System.IO.File.Exists(fName))
            {
                return Json(new
                {
                    result = -1,
                    message = "未發現檔案"
                }, JsonRequestBehavior.AllowGet);
            }

            Stream iStream = new FileStream(fName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, "application/blob", m.PccesXMLFile);
        }

        //上傳 PCCESS xml
        public JsonResult UploadXML(int id)
        {
            var file = Request.Files[0];
            string errMsg = "";
            if (file.ContentLength > 0)
            {
                try
                {
                    //string fullPath = Utils.GetTempFile(".xml");// Path.Combine(tempPath, uuid + ".xml");
                    string tempPath = Path.GetTempPath();
                    string fullPath = Path.Combine(tempPath, "SchProgress-" + file.FileName);
                    file.SaveAs(fullPath);
                    List<EngMainEditVModel> items = new EngMainService().GetItemBySeq<EngMainEditVModel>(id);
                    if (items.Count != 1)
                    {
                        return Json(new
                        {
                            result = -1,
                            msg = "工程資料讀取錯誤"
                        });
                    }
                    List<SchProgressHeaderModel> hList = iService.GetHeaderList<SchProgressHeaderModel>(id);
                    if (hList.Count != 1)
                    {
                        return Json(new
                        {
                            result = -1,
                            msg = "主檔資料讀取錯誤"
                        });
                    }
                    SchProgressHeaderModel headerModel = hList[0];
                    headerModel.PccesXMLFile = "SchProgress-" + file.FileName;

                    int result = processXML(fullPath, items[0], headerModel, ref errMsg);
                    if (result > 0)
                    {
                        string dir = Utils.GetEngMainFolder(headerModel.EngMainSeq);
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                        System.IO.File.Copy(fullPath, Path.Combine(dir, headerModel.PccesXMLFile), true);
                        return Json(new
                        {
                            result = 0,
                            message = "上傳檔案更新完成",
                            item = result
                        });
                    }
                    else if (result == -1)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "工程編號/名稱 錯誤"
                        });
                    }
                    else if (result == -2)
                    {//shioulo20221208
                        return Json(new
                        {
                            result = -1,
                            message = "PCCES 項目資料與系統不一致\n" + errMsg
                        });
                    }
                    else if (result == -3)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "更新失敗: " + errMsg
                        });
                    }
                }
                catch (Exception e)
                {
                    //System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                    errMsg = e.Message;
                }
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗: "+ errMsg
            });
        }
        private String nullTrim(String value)
        {
            return value == null ? "" : value.Trim();
        }
        private int processXML(string fileName, EngMainEditVModel eng, SchProgressHeaderModel headerModel, ref string errMsg)
        {
            PccseXML pccseXML = new PccseXML(fileName);
            EngMainModel engMainModel = pccseXML.CreateEngMainModel(ref errMsg);
            if (engMainModel == null || eng.EngNo != engMainModel.EngNo || eng.EngName != engMainModel.EngName) return -1;

            List<SchProgressPayItemModel> payItems = iService.GetPayitemList<SchProgressPayItemModel>(eng.Seq);
            //檢查 PayItem 是否一致
            if (pccseXML.payItems.Count != payItems.Count)
            {
                errMsg = "\nPayItem 與系統的數量不一致\n若為首次匯入請先按 [刪除進度] 後再進行匯入作業";
                return -2;
            }
            int inx = 0;
            foreach (PCCESPayItemModel item in pccseXML.payItems)
            {
                SchProgressPayItemModel m = payItems[inx];
                //增加空白濾除
                if (item.PayItem != m.PayItem || nullTrim(item.Description) != nullTrim(m.Description) || nullTrim(item.Unit) != nullTrim(m.Unit)
                    || nullTrim(item.RefItemCode) != nullTrim(m.RefItemCode) )
                //if (item.PayItem != m.PayItem || item.Description != m.Description || item.Unit != m.Unit || item.RefItemCode != m.RefItemCode)
                    {
                    //shioulo20221208
                    errMsg = String.Format("\nPayItem:[{0}] / [{1}]\n項目:[{2}] / [{3}]\n單位:[{4}] / [{5}]\nRefItemCode:[{6}] / [{7}]",
                        item.PayItem, m.PayItem,
                        item.Description, m.Description,
                        item.Unit, m.Unit,
                        item.RefItemCode, m.RefItemCode);
                    /*System.Diagnostics.Debug.WriteLine("檢查 PayItem 不一致");
                    System.Diagnostics.Debug.WriteLine(item.PayItem +" / " + m.PayItem);
                    System.Diagnostics.Debug.WriteLine(item.Description + " / " + m.Description);
                    System.Diagnostics.Debug.WriteLine(item.Unit + " / " + m.Unit);
                    System.Diagnostics.Debug.WriteLine(item.RefItemCode + " / " + m.RefItemCode);*/
                    
                    return -2;
                }
                m.Price = item.Price;
                m.Amount = item.Amount;
                m.Quantity = item.Quantity;
                inx++;
            }
            return iService.UpdatePCCES(payItems, headerModel, ref errMsg);
        }

        private ActionResult DownloadFile(string fullPath, string engNo)
        {
            if (System.IO.File.Exists(fullPath))
            {
                Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", String.Format("{0} 預定進度工程範本.xlsx", engNo));
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }
        
    }
}