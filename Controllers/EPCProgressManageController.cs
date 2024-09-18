using EQC.Common;
using EQC.Detection;
using EQC.EDMXModel;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace EQC.Controllers
{
    [SessionFilter]
    public class EPCProgressManageController : MyController
    {//工程管理 - 進度管理
        EPCCalendarService iService = new EPCCalendarService();
        SupDailyReportService supDailyReportService = new SupDailyReportService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }

        public JsonResult ConstructionDoneEarly(int id, DateTime tarDate)
        {
            using(var context = new EQC_NEW_Entities())
            {
                var targetEng = context.EngMain.Find(id);
                if ( tarDate ==  Utils.ChsDateStrToDate(targetEng.PrjXML?.PrjXMLExt?.ActualCompletionDate)  )
                {
                    targetEng.SchCompDate = tarDate;
                    targetEng.ProgressDoneEarly = tarDate;
                    context.SaveChanges();
                }
                else
                {
                    return Json("與公共工程雲端服務網之實際完工日期不一致");
                }

            }
            return Json(true); 
        }

        //檢查日誌日期 是否屬於工程變更範圍 s20230412
        public JsonResult CheckActiveDate(int id, DateTime tarDate)
        {
            List<EPCProgressEngChangeListVModel> engChanges = new SchEngChangeService().GetEngChangeList<EPCProgressEngChangeListVModel>(id);
            //if (tarDate.AddDays(1) > DateTime.Now)
            //{
            //    return Json(new
            //    {
            //        result = -1,
            //        msg = String.Format("請填寫日期{0}(不含)之前的日誌", DateTime.Now.ToString("yyyy-M-d"))
            //    });
            //}
            if (engChanges.Count > 0)
            {
                EPCProgressEngChangeListVModel engChange = engChanges[0];

                if (tarDate.Subtract(engChange.StartDate.Value).Days >= 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = String.Format("日期{0}(含)之後的日誌, 請至 進度管理-工程變更 頁面填寫", engChange.StartDate.Value.ToString("yyyy-M-d"))
                    });
                }
            }

            int i = 0;
            int[] OverviewCalculator = new int[2];

            using (var context = new EQC_NEW_Entities())
            {
                var target = context.EngMain.Find(id);

                
                context.SupDailyDate.Where(r => r.EngMainSeq == id && r.ItemDate == tarDate)
                    .OrderBy(r => r.DataType)
                    .GroupBy(r => r.DataType)
                    .Select(r  => r.FirstOrDefault() )
                    .ToList()
                    .ForEach(e =>
                    {
                        var overview = supDailyReportService.GetPlanOverview<EPCSupPlanOverviewVModel>(e.Seq);
                        OverviewCalculator[i++] =  (int)overview.Sum(r => r.TodayConfirm);
                });
 
                var schCompDate = target
                    .EngChangeSchCompDate ??
                    context.EngMain.Find(id)
                    .SchCompDate;


                if (
                     target
                    .StartDate?.Date > tarDate)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "日期不在預定進度範圍內，請執行工程變更"
                    });
                }
            }    

            return Json(new
            {
                result = 0,
                ConstructionDateConfirmed = OverviewCalculator[1],
                MiscDateConfirmed = OverviewCalculator[0],
                msg = ""
            });
        }

        //******************************************************************************
        //施工日誌 月曆資料
        //******************************************************************************
        public JsonResult GetConstructionInfo(int id, DateTime fromDate)
        {
            return new CalUtils().GetCalDayInfo(id, fromDate, SupDailyReportService._Construction); //
        }

        private List<EPCSupPlanOverviewVModel> GetPlanItem(int id, 
            DateTime tarDate, List<EPCSupDailyDateVModel> dateList)
        {
            EPCSupDailyDateVModel supDailyItem;
            List<EPCSupPlanOverviewVModel> planItems = null;
            if (dateList.Count == 0)
            {//該日未有紀錄

                supDailyItem = new EPCSupDailyDateVModel()
                {
                    Seq = -1,
                    DataType = SupDailyReportService._Construction,
                    EngMainSeq = id,
                    ItemDate = tarDate,
                    ItemState = 0,
                    FillinDate = DateTime.Now
                };
                using(var context = new EQC_NEW_Entities() )
                {

                    var targetEng = context.EngMain.Find(id);
                    planItems = supDailyReportService.GetPayitemList<EPCSupPlanOverviewVModel>(id, tarDate);
                    if (targetEng.SchCompDate.Value.Subtract(tarDate).Days < 0)
                    {//超出工程預定完工日期
                        foreach (SupPlanOverviewModel m in planItems)
                        {
                            m.DayProgress = 0;
                        }
                    }

                }



            }
            else
            {
                supDailyItem = dateList[0];

                planItems = supDailyReportService.GetPlanOverviewAndTotal<EPCSupPlanOverviewVModel>(supDailyItem.Seq);
            }
            return planItems;
        }
        //施工日誌
        public JsonResult GetConstructionItem(int id, DateTime tarDate)
        {
            List<EPCSchProgressHeaderVModel> scItems = new SchProgressPayItemService().GetHeaderList<EPCSchProgressHeaderVModel>(id);
            if (scItems.Count == 0 || scItems[0].SPState != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "預訂定度作業未完成, 無法作業"
                });
            }

            List<EPCTendeVModel> tenders = new EPCProgressManageService().GetEngLinkTenderBySeq<EPCTendeVModel>(id);
            if (tenders.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }
            EPCTendeVModel eng = tenders[0];
            //
            SupDailyReportMiscConstructionModel miscItem;
            EPCSupDailyDateVModel supDailyItem;
            List<EPCSupPlanOverviewVModel> planItems;
            List<EPCSupDailyDateVModel> dateList = supDailyReportService.GetSupDailyDate<EPCSupDailyDateVModel>(SupDailyReportService._Construction, id, tarDate);
            if(dateList.Count == 0)
            {//該日未有紀錄

                supDailyItem = new EPCSupDailyDateVModel() {
                    Seq = -1,
                    DataType = SupDailyReportService._Construction,
                    EngMainSeq = id,
                    ItemDate = tarDate,
                    ItemState = 0,
                    FillinDate = DateTime.Now
                };
                miscItem = new SupDailyReportMiscConstructionModel() { Seq = -1 };
                planItems = supDailyReportService.GetPayitemList<EPCSupPlanOverviewVModel>(id, tarDate);
                if(eng.SchCompDate.Value.Subtract(tarDate).Days <0)
                {//超出工程預定完工日期
                    foreach(SupPlanOverviewModel m in planItems)
                    {
                        m.DayProgress = 0;
                    }
                }
            } else
            {
                supDailyItem = dateList[0];
                List<SupDailyReportMiscConstructionModel> miscList = supDailyReportService.GetMiscConstruction<SupDailyReportMiscConstructionModel>(supDailyItem.Seq);
                miscItem = miscList[0];
                planItems = supDailyReportService.GetPlanOverviewAndTotal<EPCSupPlanOverviewVModel>(supDailyItem.Seq);
            }
            
            return Json(new
            {
                result = 0,
                supDailyItem = supDailyItem,
                miscItem = miscItem,
                planItems = planItems
            });
        }
        //日誌填報完成 s20231116
        public JsonResult DailyLogCompleted(int id)
        {
            if (supDailyReportService.DailyLogCompleted(id))
            {
                return Json(new
                {
                    result = 0,
                    msg = "設定完成"
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "設定失敗"
                });
            }
        }
        //複製前日材料,人員,機具資料 s20231116
        public JsonResult CopyConstructionMiscData(EPCSupDailyDateVModel supDailyItem)
        {
            DateTime dayBefore = DateTime.Parse(supDailyItem.ItemDateStr).AddDays(-1);
            SupDailyReportConstructionEquipmentService equipmentService = new SupDailyReportConstructionEquipmentService();
            List<SupDailyReportConstructionEquipmentModel> equipments = equipmentService.GetDayBeforeItems<SupDailyReportConstructionEquipmentModel>(supDailyItem.EngMainSeq, dayBefore);
            //
            SupDailyReportConstructionPersonService personService = new SupDailyReportConstructionPersonService();
            List<SupDailyReportConstructionPersonModel> persons = personService.GetDayBeforeItems<SupDailyReportConstructionPersonModel>(supDailyItem.EngMainSeq, dayBefore);
            //
            SupDailyReportConstructionMaterialService materialService = new SupDailyReportConstructionMaterialService();
            List<SupDailyReportConstructionMaterialModel> materials = materialService.GetDayBeforeItems<SupDailyReportConstructionMaterialModel>(supDailyItem.EngMainSeq, dayBefore);
            if (equipments.Count > 0 || persons.Count > 0 || materials.Count > 0)
            {
                if(supDailyReportService.CopyConstructionMiscData(supDailyItem, equipments, persons, materials))
                {
                    return Json(new
                    {
                        result = 0,
                        msg = "複製完成"
                    });
                } else
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "複製失敗"
                    });
                }
            }
            return Json(new
            {
                result = -1,
                msg = "無資料可複製"
            });
        }
        //更新
        public JsonResult ConstructionSave(EPCSupDailyDateVModel supDailyItem, SupDailyReportMiscConstructionModel miscItem, List<SupPlanOverviewModel> planItems)
        {
            bool result;
            if (supDailyItem.Seq == -1)
            {//預設值
                DateTime dayBefore = DateTime.Parse(supDailyItem.ItemDateStr).AddDays(-1);
                //
                SupDailyReportConstructionEquipmentService equipmentService = new SupDailyReportConstructionEquipmentService();
                List<SupDailyReportConstructionEquipmentModel> equipments = equipmentService.GetDayBeforeItems<SupDailyReportConstructionEquipmentModel>(supDailyItem.EngMainSeq, dayBefore);//s20231113
                if (equipments.Count == 0)
                {
                    equipments = equipmentService.GetDefaultItems<SupDailyReportConstructionEquipmentModel>(supDailyItem.EngMainSeq, dayBefore);
                }
                //
                SupDailyReportConstructionPersonService personService = new SupDailyReportConstructionPersonService();
                List<SupDailyReportConstructionPersonModel> persons = personService.GetDayBeforeItems<SupDailyReportConstructionPersonModel>(supDailyItem.EngMainSeq, dayBefore);//s20231113
                if (persons.Count == 0)
                {
                    persons = personService.GetDefaultItems<SupDailyReportConstructionPersonModel>(supDailyItem.EngMainSeq, dayBefore);
                }
                //
                SupDailyReportConstructionMaterialService materialService = new SupDailyReportConstructionMaterialService();
                List<SupDailyReportConstructionMaterialModel> materials = materialService.GetDayBeforeItems<SupDailyReportConstructionMaterialModel>(supDailyItem.EngMainSeq, dayBefore);//s20231113
                if(materials.Count == 0)
                {
                    materials = materialService.GetDefaultItems<SupDailyReportConstructionMaterialModel>(supDailyItem.EngMainSeq, dayBefore);
                }
                result = supDailyReportService.ConstructionAdd(supDailyItem, miscItem, planItems, equipments, persons, materials);
            }
            else
            {
                result = supDailyReportService.ConstructionUpdate(supDailyItem, miscItem, planItems);
            }
            if (result)
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
        //下載工程範本(excel)
        /// <summary>
        /// 多日期範本下載 20230223
        /// </summary>
        /// <param name="sd">起始日期</param>
        /// <param name="ed">結束日期</param>
        /// <param name="eId">EengMain.Seq</param>
        /// <returns></returns>
        public ActionResult DnMultiDate(DateTime sd, DateTime ed, int eId, DownloadArgExtension downloadArg = null)
        {
            if(sd == null || ed == null)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }

            List<EPCSchProgressHeaderVModel> scItems = new SchProgressPayItemService().GetHeaderList<EPCSchProgressHeaderVModel>(eId);
            if (scItems.Count == 0 || scItems[0].SPState != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "預訂定度作業未完成, 無法作業"
                });
            }
            List<EPCTendeVModel> tenders = new EPCProgressManageService().GetEngLinkTenderBySeq<EPCTendeVModel>(eId);
            if (tenders.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }
            EPCTendeVModel eng = tenders[0];
            try
            {
                DateTime startDate = sd;
                DateTime endDate = ed;

                //s20230412
                List<EPCProgressEngChangeListVModel> engChanges = new SchEngChangeService().GetEngChangeList<EPCProgressEngChangeListVModel>(eng.Seq);
                if (engChanges.Count > 0)
                {
                    EPCProgressEngChangeListVModel engChange = engChanges[0];
                    if (startDate.Subtract(engChange.StartDate.Value).Days >= 0 || endDate.Subtract(engChange.StartDate.Value).Days >= 0)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = String.Format("日期區間不可以跨越工程變更日期", engChange.StartDate.Value.ToString("yyyy-M-d"))
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                System.Diagnostics.Debug.WriteLine(endDate.Subtract(startDate).Days);
                if (endDate.Subtract(startDate).Days > 30)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "避免網路速度影響，僅提供下載31天日誌(報表)"
                    }, JsonRequestBehavior.AllowGet);
                }

                DateTime tarDate = startDate;
                while (endDate.Subtract(tarDate).Days >= 0)
                {
                    if(!checkConstructionItem(eng, tarDate))
                    {
                        return Json(new
                        {
                            result = -1,
                            message = String.Format("{0} 日誌建立失敗", tarDate.ToString("yyyy-M-d"))
                        }, JsonRequestBehavior.AllowGet);
                    }
                    tarDate = tarDate.AddDays(1);
                }
                var userSeq = downloadArg?.GetCreateUser() ?? Utils.getUserInfo().Seq;
                Utils.WebRootPath = Utils.WebRootPath ?? HttpContext.Server.MapPath("~");
                string tarDir = Path.Combine( Utils.GetTempFolderForUser() , Guid.NewGuid().ToString("B").ToUpper());
                Directory.CreateDirectory(tarDir);

                DownloadTaskDetection.AddTaskQueneToRun(() => {
                    var result =  CreateExcelMultiDate(eng, startDate, endDate, tarDir, downloadArg);

                } 
                , userSeq);


                return Json(new
                {
                    downloadTaskTag = true,
                    message = "已開始產製，請稍後重新整理網頁"
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e){
             
                return Json(new
                {
                    result = -1,
                    message = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        private ActionResult CreateExcelMultiDate(EPCTendeVModel eng, DateTime startDate, DateTime endDate, string tarDir = null, DownloadArgExtension downloadArg = null)
        {
            List<EPCSupDailyDateVModel> supDailyDateList = supDailyReportService.GetSupDailyDate<EPCSupDailyDateVModel>(SupDailyReportService._Construction, eng.Seq, startDate, endDate);

            string filename = Utils.CopyTemplateFile("進度管理-施工日誌.xlsx", ".xlsx");
            string altFileName = Path.Combine(tarDir, String.Format("{0} 施工日誌[{1}].xlsx", eng.EngNo, startDate.ToString("yyyy-M-d")));
            if (tarDir != null)
            {
                
                System.IO.File.Move(filename, altFileName);
            }
            Dictionary<string, Worksheet> dict = new Dictionary<string, Worksheet>();
            Microsoft.Office.Interop.Excel.Application appExcel = null;
            Workbook workbook = null;
            //開啟 Excel 檔案
            try
            {
                appExcel = new Microsoft.Office.Interop.Excel.Application();

                if (tarDir == null)
                {
                    workbook = appExcel.Workbooks.Open(filename);
                }
                else
                {
                    workbook = appExcel.Workbooks.Open(altFileName);
                }
                foreach (Worksheet worksheet in workbook.Worksheets)
                {
                    dict.Add(worksheet.Name, worksheet);
                }
                Worksheet sheet = dict["施工日誌"];
                if (supDailyDateList.Count > 0)
                {
                    PayitemSheet(sheet, eng, supDailyDateList);
                    sheet.Protect(String.Format("{0}-{1}", eng.EngNo, supDailyDateList[0].ItemDateStr));//shioulo20221214
                }

                workbook.Save();
                workbook.Close();
                appExcel.Quit();

                FileStream iStream = new FileStream(
                    tarDir != null? altFileName : filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                downloadArg?.targetPathSetting(iStream.Name);
                return File(iStream, "application/blob", String.Format("{0} 施工日誌[{1}].xlsx", eng.EngNo, startDate.ToString("yyyy-M-d")));
            }
            catch (Exception e)
            {

                if (workbook != null) workbook.Close(false);
                if (appExcel != null) appExcel.Quit();
                return Json(new
                {
                    result = -1,
                    message = "Excel 製表失敗"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //payitem
        private void PayitemSheet(Worksheet sheet, EPCTendeVModel eng, List<EPCSupDailyDateVModel> supDailyDates)
        {
            if (sheet == null) return;

            sheet.Cells[1, 8] = supDailyDates.Count;
            sheet.Cells[2, 3] = eng.EngName;// "@工程名稱";
            sheet.Cells[3, 3] = eng.EngNo;// "@標案編號";

            int col = 0, cnt = 0;
            Microsoft.Office.Interop.Excel.Range fromRange = sheet.Range[String.Format("{0}:{1}", Utils.GetExcelColumnName(8), Utils.GetExcelColumnName(9))];
            for( int i=0; i<supDailyDates.Count-1; i++)
            {
                col += 2;
                Microsoft.Office.Interop.Excel.Range toRange = sheet.Range[String.Format("{0}:{1}", Utils.GetExcelColumnName(8+col), Utils.GetExcelColumnName(9+col))]; ;
                fromRange.Copy(toRange);
            }
            col = 0;
            foreach (EPCSupDailyDateVModel supDailyDate in supDailyDates)
            {
                List<SupPlanOverviewModel> spList = supDailyReportService.GetPlanOverview<SupPlanOverviewModel>(supDailyDate.Seq);
                sheet.Cells[4, col+8] = supDailyDate.ItemDateStr;// 日期
                int row = 6;
                foreach (SupPlanOverviewModel m in spList)
                {
                    if (cnt == 0)
                    {
                        sheet.Cells[row, 1] = m.OrderNo;
                        sheet.Cells[row, 2] = m.PayItem.Trim();// === 會與 Excel 通用格式衝突";
                        sheet.Cells[row, 3] = m.Description;// 項目及說明
                        sheet.Cells[row, 4] = m.Unit;
                        sheet.Cells[row, 5] = m.Quantity;
                        sheet.Cells[row, 6] = m.Price;
                        sheet.Cells[row, 7] = m.Amount;
                    }
                    if (m.DayProgress == -1)
                    {//shioulo20221210
                        sheet.Cells[row, col+8] = 0;
                        sheet.Cells[row, col+9].Interior.Color = sheet.Cells[row, col+8].Interior.Color;
                        sheet.Cells[row, col+9] = 0;
                        sheet.Cells[row, col+9].Locked = true;//shioulo20221214
                    }
                    else
                    {
                        sheet.Cells[row, col+8] = m.Quantity * m.DayProgress / 100;
                        sheet.Cells[row, col+9] = m.TodayConfirm;
                    }

                    row++;
                }
                cnt++;
                col += 2;
            }
        }
        //匯入多日期 excel 更新施工日誌
        public JsonResult UploadMultiDailyLog(int id)
        {
            List<EngMainEditVModel> items = new EngMainService().GetItemBySeq<EngMainEditVModel>(id);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }
            EngMainEditVModel eng = items[0];

            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                string fileName;
                try
                {
                    fileName = SaveFile(file);
                }
                catch
                {
                    return Json(new
                    {
                        result = -1,
                        message = "上傳檔案儲存失敗"
                    });
                }
                string errMsg = "";
                int result = readExcelMultiData(SupDailyReportService._Construction, fileName, eng, ref errMsg);
                if (result < 0)
                {
                    string msg;
                    switch (result)
                    {
                        case -1: msg = "Excel解析發生錯誤"; break;
                        case -2: msg = "工程名稱/編號 資料錯誤"; break;
                        case -3: msg = "施工項目 資料錯誤 " + errMsg; break;
                        case -4: msg = "日期項目 資料錯誤"; break;
                        case -5: msg = "更新資料錯誤 "+errMsg; break;
                        default:
                            msg = "未知錯誤:" + result.ToString(); break;
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
        private int readExcelMultiData(int mode, string filename, EngMainEditVModel eng, ref string errMsg)
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
                Worksheet sheet = dict["施工日誌"];

                if (sheet.Cells[2, 3].Value.ToString() != eng.EngName || sheet.Cells[3, 3].Value.ToString() != eng.EngNo) return -2;

                //檢查 日期 是否一致
                int col = 0, inx = 0;
                DateTime? startDate = null, endDate = null;
                while (sheet.Cells[4, col + 8].Value != null)
                {
                    DateTime d = DateTime.Parse(sheet.Cells[4, col + 8].Value.ToString());
                    if (inx == 0) startDate = d;
                    endDate = d;
                    inx++;
                    col += 2;
                }
                if(!startDate.HasValue || !endDate.HasValue) return -4;
                int dayCount = endDate.Value.Subtract(startDate.Value).Days + 1;
                if (sheet.Cells[1, 8].Value == null && dayCount != 1)
                    return -4;
                else if (sheet.Cells[1, 8].Value != null && sheet.Cells[1, 8].Value.ToString() != dayCount.ToString())
                    return -4;


                List<EPCSupDailyDateVModel> supDailyDateList = supDailyReportService.GetSupDailyDate<EPCSupDailyDateVModel>(mode, eng.Seq, startDate.Value, endDate.Value);
                if (dayCount != supDailyDateList.Count) return -4;
                //
                col = 0;
                inx = 0;
                foreach (EPCSupDailyDateVModel supDailyDate in supDailyDateList)
                {
                    errMsg = supDailyDate.ItemDateStr;
                    List<SupPlanOverviewModel> items = supDailyReportService.GetPlanOverview<SupPlanOverviewModel>(supDailyDate.Seq);
                    if (items.Count == 0) return -3;
                    //
                    int row = 6;
                    foreach (SupPlanOverviewModel m in items)
                    {
                        if (inx == 0)
                        {//檢查 PayItem 是否一致
                            if (sheet.Cells[row, 1].Value.ToString() != m.OrderNo.ToString() || sheet.Cells[row, 2].Value.ToString() != m.PayItem
                            || sheet.Cells[row, 3].Value.ToString() != m.Description || sheet.Cells[row, 4].Value.ToString() != m.Unit) return -3;
                        }
                        decimal todayConfirm;
                        if (decimal.TryParse(sheet.Cells[row, col+9].Value.ToString(), out todayConfirm))
                        {
                            if (m.DayProgress == -1)
                                m.TodayConfirm = 0;
                            else
                                m.TodayConfirm = todayConfirm;
                        }

                        row++;
                    }
                    supDailyDate.FillinDate = DateTime.Now;
                    if (!supDailyReportService.UpdateTodayConfirm(items, supDailyDate)) return -5;
                    inx++;
                    col += 2;
                }
                //
                workbook.Close();
                appExcel.Quit();

                return 1;
            }
            catch (Exception e)
            {
                if (workbook != null) workbook.Close(false);
                if (appExcel != null) appExcel.Quit();
                return -1;
            }
        }
        //檢查是否已建立當日日誌
        private bool checkConstructionItem(EPCTendeVModel eng, DateTime tarDate)
        {
            int id = eng.Seq;
            //
            SupDailyReportMiscConstructionModel miscItem;
            EPCSupDailyDateVModel supDailyItem;
            List<SupPlanOverviewModel> planItems;
            List<EPCSupDailyDateVModel> dateList = supDailyReportService.GetSupDailyDate<EPCSupDailyDateVModel>(SupDailyReportService._Construction, id, tarDate);
            if (dateList.Count == 0)
            {//該日未有紀錄

                supDailyItem = new EPCSupDailyDateVModel()
                {
                    Seq = -1,
                    DataType = SupDailyReportService._Construction,
                    EngMainSeq = id,
                    ItemDate = tarDate,
                    ItemState = 0
                };
                miscItem = new SupDailyReportMiscConstructionModel() { Seq = -1 };
                planItems = supDailyReportService.GetPayitemList<SupPlanOverviewModel>(id, tarDate);
                if (eng.SchCompDate.Value.Subtract(tarDate).Days < 0)
                {//超出工程預定完工日期
                    foreach (SupPlanOverviewModel m in planItems)
                    {
                        m.DayProgress = 0;
                    }
                }
                //s20230831
                DateTime dayBefore = tarDate.AddDays(-1);
                List<SupDailyReportConstructionEquipmentModel> equipments = new SupDailyReportConstructionEquipmentService().GetDefaultItems<SupDailyReportConstructionEquipmentModel>(supDailyItem.EngMainSeq, dayBefore);
                List<SupDailyReportConstructionPersonModel> persons = new SupDailyReportConstructionPersonService().GetDefaultItems<SupDailyReportConstructionPersonModel>(supDailyItem.EngMainSeq, dayBefore);
                List<SupDailyReportConstructionMaterialModel> materials = new SupDailyReportConstructionMaterialService().GetDefaultItems<SupDailyReportConstructionMaterialModel>(supDailyItem.EngMainSeq, dayBefore);

                return supDailyReportService.ConstructionAdd(supDailyItem, miscItem, planItems, equipments, persons, materials, true);
            }

            return true;
        }

        //單一日期 下載工程範本(excel)
        public ActionResult Download(int id)
        {
            List<EPCSupDailyDateVModel> supDailyDateList = supDailyReportService.GetSupDailyDateBySeq<EPCSupDailyDateVModel>(id);
            if (supDailyDateList.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料錯誤"
                });
            }
            EPCSupDailyDateVModel supDailyDate = supDailyDateList[0];

            List<EPCTendeVModel> items = new EngMainService().GetItemBySeq<EPCTendeVModel>(supDailyDate.EngMainSeq);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "工程資料錯誤"
                });
            }
            EPCTendeVModel eng = items[0];
            DateTime startDate = DateTime.Parse(supDailyDate.ItemDateStr);
            return CreateExcelMultiDate(eng, startDate, startDate);

            /*List<SupPlanOverviewModel> miscList = supDailyReportService.GetPlanOverview<SupPlanOverviewModel>(supDailyDate.Seq);
            if (miscList.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "查無資料"
                });
            }
            return CreateExcel(eng, supDailyDate, miscList);*/
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
        //工地材料管理概況
        //材料清單
        public JsonResult GetMaterialOptions()
        {
            return Json(new
            {
                items = new OptionListService().GetList(OptionListService._Material)
            });
        }
        //清單
        public JsonResult GetMaterialRecords(int id)
        {
            List<SupDailyReportConstructionMaterialModel> lists = new SupDailyReportConstructionMaterialService().GetList<SupDailyReportConstructionMaterialModel>(id);

            return Json(new
            {
                result = 0,
                items = lists
            });
        }
        public JsonResult NewMaterialRecord()
        {
            return Json(new
            {
                result = 0,
                item = new SupDailyReportConstructionMaterialModel() { Seq = -1 }
            });
        }
        public JsonResult UpdateMaterialRecords(SupDailyReportConstructionMaterialModel m)
        {
            int state;
            if (m.Seq == -1)
                state = new SupDailyReportConstructionMaterialService().AddRecord(m);
            else
                state = new SupDailyReportConstructionMaterialService().UpdateRecord(m);
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
        public JsonResult DelMaterialRecord(int id)
        {
            int state = new SupDailyReportConstructionMaterialService().DelRecord(id);
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

        //工地人員概況-人員類型清單
        public JsonResult GetPersonOptions()
        {
            return Json(new
            {
                items = new OptionListService().GetList(OptionListService._Person)
            });
        }
        //清單
        public JsonResult GetPersonRecords(int id)
        {
            List<SupDailyReportConstructionPersonModel> lists = new SupDailyReportConstructionPersonService().GetList<SupDailyReportConstructionPersonModel>(id);

            return Json(new
            {
                result = 0,
                items = lists
            });
        }
        public JsonResult NewPersonRecord()
        {
            return Json(new
            {
                result = 0,
                item = new SupDailyReportConstructionPersonModel() { Seq = -1 }
            });
        }
        public JsonResult UpdatePersonRecords(SupDailyReportConstructionPersonModel m)
        {
            int state;
            if (m.Seq == -1)
                state = new SupDailyReportConstructionPersonService().AddRecord(m);
            else
                state = new SupDailyReportConstructionPersonService().UpdateRecord(m);
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
        public JsonResult DelPersonRecord(int id)
        {
            int state = new SupDailyReportConstructionPersonService().DelRecord(id);
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
        
        //工地人員概況-機具類型清單
        public JsonResult GetEquipmentOptions()
        {
            List<SelectOptionModel> op  = new CarbonEmissionMachineService().GetMachineKindList();//shioulo20230208
            op.Add(new SelectOptionModel() { Text= "非上述", Value= "非上述" });
            return Json(new
            {
                //items = new OptionListService().GetList(OptionListService._Equipment)
                items = op
            });
        }
        //機具規格清單 shioulo 20230208
        public JsonResult GetEquipmentSpecOptions(string kind)
        {
            return Json(new
            {
                items = new CarbonEmissionMachineService().GetMachineSpecList(kind)
            });
        }
        //清單
        public JsonResult GetEquipmentRecords(int id)
        {
            List<EPCSupDailyReportConstructionEquipmentModelVModel> lists = new SupDailyReportConstructionEquipmentService().GetList<EPCSupDailyReportConstructionEquipmentModelVModel>(id);

            return Json(new
            {
                result = 0,
                items = lists
            });
        }
        public JsonResult NewEquipmentRecord()
        {
            return Json(new
            {
                result = 0,
                item = new SupDailyReportConstructionEquipmentModel() { Seq = -1 }
            });
        }
        public JsonResult UpdateEquipmentRecords(SupDailyReportConstructionEquipmentModel m)
        {
            int state;
            if (m.Seq == -1)
                state = new SupDailyReportConstructionEquipmentService().AddRecord(m);
            else
                state = new SupDailyReportConstructionEquipmentService().UpdateRecord(m);
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
        public JsonResult DelEquipmentRecord(int id)
        {
            int state = new SupDailyReportConstructionEquipmentService().DelRecord(id);
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
        //施工日誌下載
        //多日期
        public ActionResult DownloadCDailyMulti(DateTime sd, DateTime ed, int eId, int eEM, DownloadArgExtension downloadArg = null)
        {
            var userSeq = downloadArg?.GetCreateUser() ?? Utils.getUserInfo().Seq;
            
            if (sd == null || ed == null )
            {
                return Json(new
                {
                    result = -1,
                    message = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            List<EngMainEditVModel> items = new EngMainService().GetItemBySeq<EngMainEditVModel>(eId);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "工程資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EngMainEditVModel eng = items[0];
            List<TenderModel> tenders = new EPCTenderService().GetItemDetail(eng.PrjXMLSeq.Value);
            if (tenders.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "工程會資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            TenderModel tender = tenders[0];

            try
            {
                DateTime startDate = sd;
                DateTime endDate = ed;
                if (endDate.Subtract(startDate).Days > 30)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "避免網路速度影響，僅提供下載31天日誌(報表)"
                    }, JsonRequestBehavior.AllowGet);
                }

                List<EPCSupDailyDate1VModel> supDailyDateList = supDailyReportService.GetSupDailyDateAndCount<EPCSupDailyDate1VModel>(SupDailyReportService._Construction, eng.Seq, startDate, endDate);
                if(supDailyDateList.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "日期區間, 無日誌資料"
                    }, JsonRequestBehavior.AllowGet);
                }

                string uuid = Guid.NewGuid().ToString("B").ToUpper();
                string folder = "";
                if (downloadArg?.DistFilePath == null)
                {

                    folder = Path.Combine(Utils.GetTempFolderForUser(), uuid);
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                }


                string srcFile = Path.Combine(Utils.GetTemplateFilePath(), "附錄四_公共工程施工日誌V2.docx");
                List<EPCSupDailyDate1VModel> supDailyDateQueue = new List<EPCSupDailyDate1VModel>();
                foreach (EPCSupDailyDate1VModel supDailyDate in supDailyDateList)
                {
                    supDailyDate.daily = new EPCDailyVModel();
                    supDailyDate.daily.miscList = supDailyReportService.GetMiscConstruction<SupDailyReportMiscConstructionModel>(supDailyDate.Seq);
                    if (supDailyDate.daily.miscList.Count == 1)
                    {

                        supDailyDate.daily.planOverviewList = supDailyReportService.GetPlanOverviewAndTotalFilter<EPCSupPlanOverviewVModel>(supDailyDate.Seq, eEM.ToString());
                        //數量檢查 s20230830
                        foreach (EPCSupPlanOverviewVModel m in supDailyDate.daily.planOverviewList)
                        {
                            if (m.TotalAccConfirm > (m.Quantity * 100))
                            {
                                return Json(new
                                {
                                    result = -1,
                                    message = String.Format("{0}\n{1}\n{2}\n日誌數量有錯,請修正", supDailyDate.ItemDateStr, m.PayItem, m.Description)
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        supDailyDate.daily.materialList = new SupDailyReportConstructionMaterialService().GetList<SupDailyReportConstructionMaterialModel>(supDailyDate.Seq);
                        supDailyDate.daily.personList = new SupDailyReportConstructionPersonService().GetList<SupDailyReportConstructionPersonModel>(supDailyDate.Seq);
                        supDailyDate.daily.equipmentList = new SupDailyReportConstructionEquipmentService().GetList<SupDailyReportConstructionEquipmentModel>(supDailyDate.Seq);
                        supDailyDateQueue.Add(supDailyDate);

                    }
                }
                DownloadTaskDetection.AddTaskQueneToRun(() =>
                {
                    var fileTempFolder = Utils.GetTempFolder();
                    //int downloadTag = new EPCProgressManageService().UpdateProgressManageTag("ConstructionDownload", 1);
                    foreach (EPCSupDailyDate1VModel supDailyDate in supDailyDateQueue)
                    {
                        if (supDailyDate.daily.miscList.Count == 1)
                        {
                            string tarfile = Path.Combine(fileTempFolder, String.Format("{0} 公共工程施工日誌[{1}].docx", eng.EngNo, supDailyDate.ItemDateStr));
                            System.IO.File.Copy(srcFile, tarfile, true);                         
                            CreateConstructionDoc(tarfile, eng, supDailyDate, supDailyDate.daily, tender, 1);
                            downloadArg?.targetPathSetting(tarfile, String.Format("{0} 公共工程施工日誌[{1}].docx", eng.EngNo, supDailyDate.ItemDateStr));
                        }
                    }
                    //downloadTag = new EPCProgressManageService().UpdateProgressManageTag("ConstructionDownload", 0);
                    if (downloadArg?.DistFilePath == null)
                    {
                        string zipFile = Path.Combine(folder, uuid + "-施工日誌.zip");
                        ZipFile.CreateFromDirectory(fileTempFolder, zipFile);// AddFiles(files, "ProjectX");
      
                    }
                    Directory.Delete(fileTempFolder, true);


                }, userSeq);

                return Json(new {           
                    result = -1,
                    downloadTaskTag = true,
                    message = "已開始產製日報，請稍後重新整理網頁"
                });
                //Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                //return File(iStream, "application/blob", String.Format("{0} 公共工程施工日誌[{1}]-施工日誌.zip", eng.EngNo, supDailyDateList[0].ItemDateStr));
            }
            catch
            {
                return Json(new
                {
                    result = -1,
                    message = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
        }        
        //單一日期
        public ActionResult DownloadCDaily(int id, string eEM)
        {
            return DownloadCDailyDoc(id, 1, eEM);
        }
        public ActionResult DownloadCDailyDoc(int id, int docMode)
        {
            return DownloadCDailyDoc(id, docMode, "0"); //s20230830
        }
        //s20230830
        public ActionResult DownloadCDailyDoc(int id, int docMode, string eEM)
        {
            List<EPCSupDailyDate1VModel> supDailyDateList = supDailyReportService.GetSupDailyDateAndCount<EPCSupDailyDate1VModel>(id, 2);
            if (supDailyDateList.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EPCSupDailyDate1VModel supDailyDate = supDailyDateList[0];

            List<EngMainEditVModel> items = new EngMainService().GetItemBySeq<EngMainEditVModel>(supDailyDate.EngMainSeq);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "工程資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EngMainEditVModel eng = items[0];
            List<TenderModel> tenders = new EPCTenderService().GetItemDetail(eng.PrjXMLSeq.Value);
            if (tenders.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "工程會資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            TenderModel tender = tenders[0];

            EPCDailyVModel daily = new EPCDailyVModel();
            daily.miscList = supDailyReportService.GetMiscConstruction<SupDailyReportMiscConstructionModel>(supDailyDate.Seq);
            if (daily.miscList.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "日誌資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            daily.planOverviewList = supDailyReportService.GetPlanOverviewAndTotalFilter<EPCSupPlanOverviewVModel>(supDailyDate.Seq, eEM);
            //數量檢查 s20230830
            foreach(EPCSupPlanOverviewVModel m in daily.planOverviewList)
            {
                if(m.TotalAccConfirm > (m.Quantity * 100))
                {
                    return Json(new
                    {
                        result = -1,
                        message = String.Format("{0}\n{1}\n日誌數量有錯,請修正", m.PayItem, m.Description)
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            daily.materialList = new SupDailyReportConstructionMaterialService().GetList<SupDailyReportConstructionMaterialModel>(supDailyDate.Seq);
            daily.personList = new SupDailyReportConstructionPersonService().GetList<SupDailyReportConstructionPersonModel>(supDailyDate.Seq);
            daily.equipmentList = new SupDailyReportConstructionEquipmentService().GetList<SupDailyReportConstructionEquipmentModel>(supDailyDate.Seq);

            string filename = String.Format("{0} 公共工程施工日誌[{1}].docx", eng.EngNo, supDailyDate.ItemDateStr);
            string tarfile = Utils.CopyTemplateFile("附錄四_公共工程施工日誌V2.docx", ".docx");
            if (CreateConstructionDoc(tarfile, eng, supDailyDate, daily, tender, docMode) )
            {
                Stream iStream = new FileStream(tarfile, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", filename);
            } else
            {
                return Json(new
                {
                    result = -1,
                    message = "日誌建立失敗"
                }, JsonRequestBehavior.AllowGet);
            }
            //return CreateConstructionDoc(eng, supDailyDate, daily, tender, docMode);
        }
        private bool CreateConstructionDoc(string tarfile, EngMainEditVModel eng, EPCSupDailyDate1VModel supDailyDate, EPCDailyVModel daily, TenderModel tender, int docMode)
        {//s20230228
            Microsoft.Office.Interop.Word.Application wordApp = null;
            Document doc = null;
            string filename = String.Format("{0} 公共工程施工日誌[{1}].docx", eng.EngNo, supDailyDate.ItemDateStr);
            try
            {
                //string tarfile = Utils.CopyTemplateFile("附錄四_公共工程施工日誌V2.docx", ".docx");
                wordApp = new Microsoft.Office.Interop.Word.Application();
                doc = wordApp.Documents.Open(tarfile);
                Table table = doc.Tables[1];
                table.Cell(1, 1).Range.Text = String.Format("報表編號: {0}", supDailyDate.OrderNo.ToString());
                table.Cell(2, 2).Range.Text = String.Format("上午：{0} 下午：{1}", supDailyDate.Weather1, supDailyDate.Weather2);
                int extensionDayCount =
                    new EPCCalendarService()
                    .GetExtensionByPrjSeq<EPCReportExtensionVModel>(supDailyDate.EngMainSeq)
                    .Sum(r => r.ExtendDays);
                if (supDailyDate.FillinDate.HasValue)
                {
                    var dtInfo = new System.Globalization.DateTimeFormatInfo();
                    dtInfo.AbbreviatedDayNames = new string[] { "日", "一", "二", "三", "四", "五", "六" };
                    table.Cell(2, 4).Range.Text = String.Format("{0}年 {1}月 {2}日(星期{3})",
                        supDailyDate.FillinDate.Value.Year-1911, supDailyDate.FillinDate.Value.Month
                        ,supDailyDate.FillinDate.Value.Day, supDailyDate.FillinDate.Value.ToString("ddd", dtInfo));
                }
                table.Cell(3, 2).Range.Text = eng.EngName;
                table.Cell(3, 4).Range.Text = tender.ContractorName1;
                table.Cell(4, 2).Range.Text = String.Format("{0}天", tender.TotalDays);
                table.Cell(4, 4).Range.Text = String.Format("{0}天", supDailyDate.OrderNo);
                table.Cell(4, 6).Range.Text = String.Format("{0}天", tender.TotalDays - supDailyDate.OrderNo);
                table.Cell(4, 8).Range.Text = String.Format("{0}天", extensionDayCount);
                table.Cell(5, 2).Range.Text = tender.ActualStartDate;
                table.Cell(5, 4).Range.Text = tender.ScheCompletionDate;
                //進度 s20230227
                List<EngProgressVModel> engProgress = new EPCProgressManageService().GetEngProgress<EngProgressVModel>(eng.Seq, SupDailyReportService._Construction, supDailyDate.ItemDate.ToString("yyyy-M-d"));
                if (engProgress.Count == 1)
                {
                    table.Cell(6, 2).Range.Text = String.Format("{0}%", engProgress[0].SchProgress);
                    table.Cell(6, 4).Range.Text = String.Format("{0}%", engProgress[0].AcualProgress);
                }
                string check = "□有 □無";
                if (daily.miscList[0].IsFollowSkill.HasValue)
                {
                    if (daily.miscList[0].IsFollowSkill.Value)
                        check = "■有 □無";
                    else
                        check = "□有 ■無";
                }
                table.Cell(19, 1).Range.Text = String.Format("四、本日施工項目是否有須依「營造業專業工程特定施工項目應置之技術士種類、比率或人數標準表」規定應設置技術士之專業工程：{0}（此項如勾選”有”，則應填寫後附「公共工程施工日誌之技術士簽章表」）", check);

                check = "□有 □無";
                if (daily.miscList[0].SafetyHygieneMatters01.HasValue)
                {
                    if (daily.miscList[0].SafetyHygieneMatters01.Value)
                        check = "■有 □無";
                    else
                        check = "□有 ■無";
                }
                table.Cell(21, 1).Range.Text = String.Format("   1.實施勤前教育(含工地預防災變及危害告知)：{0}", check);

                check = "□有 □無 □無新進勞工";
                if (daily.miscList[0].SafetyHygieneMatters02.HasValue)
                {
                    if (daily.miscList[0].SafetyHygieneMatters02.Value == 0)
                        check = "■有 □無 □無新進勞工";
                    else if (daily.miscList[0].SafetyHygieneMatters02.Value == 1)
                        check = "□有 ■無 □無新進勞工";
                    else if (daily.miscList[0].SafetyHygieneMatters02.Value == 2)
                        check = "□有 □無 ■無新進勞工";
                }
                table.Cell(22, 1).Range.Text = String.Format("   2.確認新進勞工是否提報勞工保險(或其他商業保險)資料及安全衛生教育訓練紀錄：{0}", check);

                check = "□有 □無";
                if (daily.miscList[0].SafetyHygieneMatters03.HasValue)
                {
                    if (daily.miscList[0].SafetyHygieneMatters03.Value)
                        check = "■有 □無";
                    else
                        check = "□有 ■無";
                }
                table.Cell(23, 1).Range.Text = String.Format("   3.檢查勞工個人防護具：{0}", check);
                table.Cell(25, 2).Range.Text = daily.miscList[0].SafetyHygieneMattersOther;
                table.Cell(27, 1).Range.Text = daily.miscList[0].SamplingTest;
                table.Cell(29, 1).Range.Text = daily.miscList[0].NoticeManufacturers;
                table.Cell(31, 1).Range.Text = daily.miscList[0].ImportantNotes;
                //三、工地人員及機具管理
                int row = 18;
                int itemCnt = daily.personList.Count;
                if (itemCnt < daily.equipmentList.Count)
                    itemCnt = daily.equipmentList.Count;
                Row templateRow = table.Rows[row];
                templateRow.Range.Copy();
                for (int i = 1; i < itemCnt; i++)
                {
                    templateRow.Range.Paste();
                }
                itemCnt = 0;
                foreach (SupDailyReportConstructionPersonModel m in daily.personList)
                {
                    table.Cell(row + itemCnt, 1).Range.Text = m.KindName;
                    table.Cell(row + itemCnt, 2).Range.Text = m.TodayQuantity.ToString();
                    table.Cell(row + itemCnt, 3).Range.Text = m.AccQuantity.ToString();
                    itemCnt++;
                }
                itemCnt = 0;
                foreach (SupDailyReportConstructionEquipmentModel m in daily.equipmentList)
                {
                    table.Cell(row + itemCnt, 4).Range.Text = m.EquipmentName + " " + m.EquipmentModel;
                    table.Cell(row + itemCnt, 5).Range.Text = m.TodayQuantity.ToString();
                    table.Cell(row + itemCnt, 6).Range.Text = m.AccQuantity.ToString();
                    itemCnt++;
                }
                //二、工地材料管理概況
                row = 15;
                itemCnt = daily.materialList.Count;
                templateRow = table.Rows[row];
                templateRow.Range.Copy();
                for (int i = 1; i < itemCnt; i++)
                {
                    templateRow.Range.Paste();
                }
                itemCnt = 0;
                foreach (SupDailyReportConstructionMaterialModel m in daily.materialList)
                {
                    table.Cell(row + itemCnt, 1).Range.Text = m.MaterialName;
                    table.Cell(row + itemCnt, 2).Range.Text = m.Unit;
                    table.Cell(row + itemCnt, 3).Range.Text = m.ContractQuantity.ToString();
                    table.Cell(row + itemCnt, 4).Range.Text = m.TodayQuantity.ToString();
                    table.Cell(row + itemCnt, 5).Range.Text = m.AccQuantity.ToString();
                    table.Cell(row + itemCnt, 6).Range.Text = m.Memo;
                    itemCnt++;
                }
                //一、依施工計畫書執行按圖施工概況
                row = 9;
                itemCnt = daily.planOverviewList.Count;
                templateRow = table.Rows[row];
                templateRow.Range.Copy();
                for (int i = 1; i < itemCnt; i++)
                {
                    templateRow.Range.Paste();
                }
                itemCnt = 0;
                foreach (EPCSupPlanOverviewVModel m in daily.planOverviewList)
                {
                    table.Cell(row + itemCnt, 1).Range.Text = m.Description;
                    table.Cell(row + itemCnt, 2).Range.Text = m.Unit;
                    table.Cell(row + itemCnt, 3).Range.Text = String.Format("{0:0.####}", m.Quantity);
                    table.Cell(row + itemCnt, 4).Range.Text = String.Format("{0:0.####}", m.TodayConfirm);
                    table.Cell(row + itemCnt, 5).Range.Text = String.Format("{0:0.####}", m.TodayConfirm + m.TotalAccConfirm);
                    table.Cell(row + itemCnt, 6).Range.Text = m.Memo;
                    itemCnt++;
                }
                //
                table = doc.Tables[3];
                table.Cell(1, 2).Range.Text = eng.EngName;
                table.Cell(2, 2).Range.Text = tender.ContractorName1;

                if (docMode == 2)
                {
                    filename = filename.Replace(".docx", ".pdf");
                    tarfile = tarfile.Replace(".docx", ".pdf");
                    doc.SaveAs2(tarfile, WdExportFormat.wdExportFormatPDF);

                }

                if (doc != null) doc.Close();
                if (wordApp != null) wordApp.Quit();

                //Stream iStream = new FileStream(tarfile, FileMode.Open, FileAccess.Read, FileShare.Read);
                //return File(iStream, "application/blob", filename);
                return true;
            }
            catch(Exception e)
            {

                if (doc != null) doc.Close();
                if (wordApp != null) wordApp.Quit();
                BaseService.log.Info($"EngName :{eng.EngName}\nErr:{e.Message }\nInnerError:{e.InnerException}\nStackTrace:{e.StackTrace }\n\n");
                return false;
            }

            if (doc != null) doc.Close();
            if (wordApp != null) wordApp.Quit();

            return false;
        }


        //******************************************************************************
        //監造日誌 月曆資料
        //******************************************************************************
        public JsonResult GetMiscInfo(int id, DateTime fromDate)
        {
            return new CalUtils().GetCalDayInfo(id, fromDate, SupDailyReportService._Supervise);
        }
        //監造日誌
        public JsonResult GetMiscItem(int id, DateTime tarDate)
        {
            List<EPCSchProgressHeaderVModel> scItems = new SchProgressPayItemService().GetHeaderList<EPCSchProgressHeaderVModel>(id);
            if (scItems.Count == 0 || scItems[0].SPState != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "預訂定度作業未完成, 無法作業"
                });
            }
            List<EPCTendeVModel> tenders = new EPCProgressManageService().GetEngLinkTenderBySeq<EPCTendeVModel>(id);
            if (tenders.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }
            EPCTendeVModel eng = tenders[0];
            //
            SupDailyReportMiscModel miscItem;
            EPCSupDailyDateVModel supDailyItem;
            List<EPCSupPlanOverviewVModel> planItems = null;
            List<EPCSupDailyDateVModel> dateList = supDailyReportService.GetSupDailyDate<EPCSupDailyDateVModel>(SupDailyReportService._Supervise, id, tarDate);
            List<EPCSupDailyDateVModel> dateList_construction = supDailyReportService.GetSupDailyDate<EPCSupDailyDateVModel>(SupDailyReportService._Construction, id, tarDate);
            var constructionsPlanItem = GetPlanItem(id, tarDate, dateList_construction);
            if (dateList.Count == 0)
            {//該日未有紀錄

                supDailyItem = new EPCSupDailyDateVModel()
                {
                    Seq = -1,
                    DataType = SupDailyReportService._Supervise,
                    EngMainSeq = id,
                    ItemDate = tarDate,
                    ItemState = 0,
                    FillinDate = DateTime.Now
                };
                miscItem = new SupDailyReportMiscModel() { Seq = -1 };
                //複製前一日雜項 s20230831
                List<SupDailyReportMiscModel> dayBefore = supDailyReportService.GetMiscByDate<SupDailyReportMiscModel>(id, tarDate.AddDays(-1));
                if(dayBefore.Count == 1)
                {
                    miscItem = dayBefore[0];
                }

                List<EPCSupDailyDateVModel> constructionList = supDailyReportService.GetSupDailyDate<EPCSupDailyDateVModel>(SupDailyReportService._Construction, id, tarDate);
                if(constructionList.Count > 0)
                {//複製施工日誌
                    EPCSupDailyDateVModel construction = constructionList[0];
                    planItems = supDailyReportService.GetPlanOverview<EPCSupPlanOverviewVModel>(construction.Seq);
                    foreach(SupPlanOverviewModel m in planItems)
                    {
                        m.Seq = -1;
                    }
                    //s20230408
                    supDailyItem.Weather1 = construction.Weather1;
                    supDailyItem.Weather2 = construction.Weather2;
                    if(construction.FillinDate.HasValue) supDailyItem.FillinDate = construction.FillinDate;
                }
                if (planItems == null || planItems.Count == 0)
                {
                    planItems = supDailyReportService.GetPayitemList<EPCSupPlanOverviewVModel>(id, tarDate);
                    if (eng.SchCompDate.Value.Subtract(tarDate).Days < 0)
                    {//超出工程預定完工日期
                        foreach (SupPlanOverviewModel m in planItems)
                        {
                            m.DayProgress = 0;
                        }
                    }
                }
            }
            else
            {
                supDailyItem = dateList[0];
                List<SupDailyReportMiscModel> miscList = supDailyReportService.GetMisc<SupDailyReportMiscModel>(supDailyItem.Seq);
                miscItem = miscList[0];
                planItems = supDailyReportService.GetPlanOverviewAndTotal<EPCSupPlanOverviewVModel>(supDailyItem.Seq);
            }
            planItems = planItems.Join(constructionsPlanItem, r1 => r1.PayItem + r1.Description, r2 => r2.PayItem +r2.Description, (r1, r2) =>
            {
                r1.ConstructionConfirm = r2.TodayConfirm;
                return r1;
            }).
            ToList();
            return Json(new
            {
                result = 0,
                supDailyItem = supDailyItem,
                miscItem = miscItem,
                planItems = planItems
            });
        }
        //更新
        public JsonResult MiscSave(EPCSupDailyDateVModel supDailyItem, SupDailyReportMiscModel miscItem, List<SupPlanOverviewModel> planItems)
        {
            bool result;
            if (supDailyItem.Seq == -1)
            {
                result = supDailyReportService.MiscAdd(supDailyItem, miscItem, planItems);
            }
            else
            {
                result = supDailyReportService.MiscUpdate(supDailyItem, miscItem, planItems);
            }
            if (result)
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
        //監造日誌下載
        //多日期
        public ActionResult DownloadSDailyMulti(DateTime sd, DateTime ed, int eId, int eEM, DownloadArgExtension downloadArg = null)
        {
            if ( sd == null || ed == null)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            List<EngMainEditVModel> items = new EngMainService().GetItemBySeq<EngMainEditVModel>(eId);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "工程資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EngMainEditVModel eng = items[0];
            List<TenderModel> tenders = new EPCTenderService().GetItemDetail(eng.PrjXMLSeq.Value);
            if (tenders.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "工程會資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            TenderModel tender = tenders[0];

            try
            {
                DateTime startDate = sd;
                DateTime endDate = ed;
                if (endDate.Subtract(startDate).Days > 30)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "避免網路速度影響，僅提供下載31天日誌(報表)"
                    }, JsonRequestBehavior.AllowGet);
                }

                List<EPCSupDailyDate1VModel> supDailyDateList = supDailyReportService.GetSupDailyDateAndCount<EPCSupDailyDate1VModel>(SupDailyReportService._Supervise, eng.Seq, startDate, endDate);
                if (supDailyDateList.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "日期區間, 無日誌資料"
                    }, JsonRequestBehavior.AllowGet);
                }

                string uuid = Guid.NewGuid().ToString("B").ToUpper();

                string folder = "";
                if (downloadArg?.DistFilePath == null)
                {
                    folder = Path.Combine(Utils.GetTempFolderForUser(), uuid);
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                }


                string srcFile = Path.Combine(Utils.GetTemplateFilePath(), "附錄五_公共工程監造報表V2.docx");
                List<EPCSupDailyDate1VModel> downloadQueue = new List<EPCSupDailyDate1VModel>();
                foreach (EPCSupDailyDate1VModel supDailyDate in supDailyDateList)
                {


                    supDailyDate.daily2 = new EPCDailySVModel();
                    supDailyDate.daily2.miscList = supDailyReportService.GetMisc<SupDailyReportMiscModel>(supDailyDate.Seq);
                    if (supDailyDate.daily2.miscList.Count == 1)
                    {
                        supDailyDate.daily2.planOverviewList = supDailyReportService.GetPlanOverviewAndTotalFilter<EPCSupPlanOverviewVModel>(supDailyDate.Seq, eEM.ToString());
                        downloadQueue.Add(supDailyDate);
                        //CreateMiscDoc(tarfile, eng, supDailyDate, daily, tender, 1);
                    }
                }
                var userSeq = downloadArg?.GetCreateUser() ?? Utils.getUserInfo().Seq;

                DownloadTaskDetection.AddTaskQueneToRun(() =>
                {
                    string fileTempFolder = Utils.GetTempFolder();

                    foreach (var downloadItem in downloadQueue)
                    {
                        string tarfile = Path.Combine(fileTempFolder, String.Format("{0} 公共工程監造報表[{1}].docx", eng.EngNo, downloadItem.ItemDateStr));
                        System.IO.File.Copy(srcFile, tarfile, true);
                       
                        CreateMiscDoc(tarfile, eng, downloadItem, downloadItem.daily2, tender, 1);
                        downloadArg?.targetPathSetting(tarfile, String.Format("{0} 公共工程施工日誌[{1}].docx", eng.EngNo, downloadItem.ItemDateStr));
                    }
                    //string zipFile = Path.Combine(folder, uuid, uuid + "-監造報表.zip");
                    //Directory.CreateDirectory(Path.GetDirectoryName(zipFile) );
                    //ZipFile.CreateFromDirectory(folder, zipFile);
                    if(downloadArg?.DistFilePath == null)
                    {
                        string zipFile = Path.Combine(folder, uuid + "-監造報表.zip");
                        ZipFile.CreateFromDirectory(fileTempFolder, zipFile);// AddFiles(files, "ProjectX");
                    }
                    Directory.Delete(fileTempFolder, true);


                }, userSeq);

                return Json(new
                {
                    downloadTaskTag = true,
                    message = "已開始產製日報，請稍後重新整理網頁"
                }, JsonRequestBehavior.AllowGet);
                //ZipFile.CreateFromDirectory(folder, zipFile);// AddFiles(files, "ProjectX");
                //Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                //return File(iStream, "application/blob", String.Format("{0} 公共工程監造報表[{1}].zip", eng.EngNo, supDailyDateList[0].ItemDateStr));
            }
            catch(Exception e)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //單一日期
        public ActionResult DownloadSDaily(int id, string eEM)
        {
            return DownloadSDailyDoc(id, 1, eEM);
        }
        public ActionResult DownloadSDailyDoc(int id, int docMode)
        {
            return DownloadSDailyDoc(id, docMode, "0"); //s20230831
        }
        public ActionResult DownloadSDailyDoc(int id, int docMode, string eEM)
        {
            List<EPCSupDailyDate1VModel> supDailyDateList = supDailyReportService.GetSupDailyDateAndCount<EPCSupDailyDate1VModel>(id, 1);
            if (supDailyDateList.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EPCSupDailyDate1VModel supDailyDate = supDailyDateList[0];

            List<EngMainEditVModel> items = new EngMainService().GetItemBySeq<EngMainEditVModel>(supDailyDate.EngMainSeq);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "工程資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EngMainEditVModel eng = items[0];
            List<TenderModel> tenders = new EPCTenderService().GetItemDetail(eng.PrjXMLSeq.Value);
            if (tenders.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "工程會資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            TenderModel tender = tenders[0];

            EPCDailySVModel daily = new EPCDailySVModel();
            daily.miscList = supDailyReportService.GetMisc<SupDailyReportMiscModel>(supDailyDate.Seq);
            if (daily.miscList.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "日誌資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            daily.planOverviewList = supDailyReportService.GetPlanOverviewAndTotalFilter<EPCSupPlanOverviewVModel>(supDailyDate.Seq, eEM);

            string filename = String.Format("{0} 公共工程監造報表[{1}].docx", eng.EngNo, supDailyDate.ItemDateStr);
            string tarfile = Utils.CopyTemplateFile("附錄五_公共工程監造報表V2.docx", ".docx");

            if (CreateMiscDoc(tarfile, eng, supDailyDate, daily, tender, docMode))
            {
                Stream iStream = new FileStream(tarfile, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", filename);
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "日誌建立失敗"
                }, JsonRequestBehavior.AllowGet);
            }
            //return CreateMiscDoc(eng, supDailyDate, daily, tender, docMode);
        }
        public bool CreateMiscDoc(string tarfile, EngMainEditVModel eng, EPCSupDailyDate1VModel supDailyDate, EPCDailySVModel daily, TenderModel tender, int docMode)
        {//s20230228
            Microsoft.Office.Interop.Word.Application wordApp = null;
            Document doc = null;
            string filename = String.Format("{0} 公共工程監造報表[{1}].docx", eng.EngNo, supDailyDate.ItemDateStr);
            try
            {
                //string tarfile = Utils.CopyTemplateFile("附錄五_公共工程監造報表V2.docx", ".docx");
                wordApp = new Microsoft.Office.Interop.Word.Application();
                doc = wordApp.Documents.Open(tarfile);
                Table table = doc.Tables[1];

                int contractChangeCount =  new SchEngChangeService().GetEngChangeList<EPCProgressEngChangeListVModel>(supDailyDate.EngMainSeq).Count;
                int extensionDayCount = 
                    new EPCCalendarService()
                    .GetExtensionByPrjSeq<EPCReportExtensionVModel>(supDailyDate.EngMainSeq)
                    .Sum(r => r.ExtendDays);
                table.Cell(1, 1).Range.Text = String.Format("報表編號: {0}",supDailyDate.OrderNo.ToString());
                table.Cell(2, 2).Range.Text = String.Format("上午：{0} 下午：{1}", supDailyDate.Weather1, supDailyDate.Weather2);
                if (supDailyDate.FillinDate.HasValue)
                {
                    var dtInfo = new System.Globalization.DateTimeFormatInfo();
                    dtInfo.AbbreviatedDayNames = new string[] { "日", "一", "二", "三", "四", "五", "六" };
                    table.Cell(2, 4).Range.Text = String.Format("{0}年 {1}月 {2}日(星期{3})",
                        supDailyDate.FillinDate.Value.Year - 1911, supDailyDate.FillinDate.Value.Month
                        , supDailyDate.FillinDate.Value.Day, supDailyDate.FillinDate.Value.ToString("ddd", dtInfo));
                }
                table.Cell(3, 2).Range.Text = eng.EngName;
                table.Cell(4, 2).Range.Text = String.Format("{0}天", tender.TotalDays);
                table.Cell(4, 4).Range.Text = tender.ActualStartDate;
                table.Cell(4, 6).Range.Text = tender.ScheCompletionDate;
                table.Cell(4, 8).Range.Text = tender.PrjXMLExt.ActualCompletionDate;

                table.Cell(5, 2).Range.Text = String.Format("{0}次", contractChangeCount);
                table.Cell(5, 4).Range.Text = String.Format("{0}天", extensionDayCount);
                //進度 s20230227
                List<EngProgressVModel> engProgress = new EPCProgressManageService().GetEngProgress<EngProgressVModel>(eng.Seq, SupDailyReportService._Construction, supDailyDate.ItemDate.ToString("yyyy-M-d"));
                if (engProgress.Count == 1)
                {
                    table.Cell(6, 2).Range.Text = String.Format("{0}%", engProgress[0].SchProgress);
                    table.Cell(6, 4).Range.Text = String.Format("{0}%", engProgress[0].AcualProgress);
                }

                using (var context = new EQC_NEW_Entities() )
                {
                    table.Cell(5, 6).Range.Text = (context.EngMain.Find(supDailyDate.EngMainSeq).PrjXML.BidAmount ?? 0).ToString() ;
                    table.Cell(6, 6).Range.Text = (context.EngMain.Find(supDailyDate.EngMainSeq).PrjXML.PrjXMLExt.DesignChangeContractAmount ?? 0).ToString() ;
                }
                    //table.Cell(5, 6).Range.Text = String.Format("原契約：{0}", tender.ContractNo);
                table.Cell(11, 1).Range.Text = daily.miscList[0].DesignDrawingConst;
                table.Cell(13, 1).Range.Text = daily.miscList[0].SpecAndQuality;

                string check = "□完成 □未完成";
                if (daily.miscList[0].SafetyHygieneMatters01.HasValue)
                {
                    if (daily.miscList[0].SafetyHygieneMatters01.Value)
                        check = "■完成 □未完成";
                    else
                        check = "□完成 ■未完成";
                }
                table.Cell(15, 1).Range.Text = String.Format("（一）施工廠商施工前檢查事項辦理情形：{0}", check);
                table.Cell(17, 1).Range.Text = daily.miscList[0].SafetyHygieneMattersOther;
                table.Cell(19, 1).Range.Text = daily.miscList[0].OtherMatters;
                //一、依施工計畫書執行按圖施工概況
                int row = 9;
                int itemCnt = daily.planOverviewList.Count;
                Row templateRow = table.Rows[row];
                templateRow.Range.Copy();
                for (int i = 1; i < itemCnt; i++)
                {
                    templateRow.Range.Paste();
                }
                itemCnt = 0;
                foreach (EPCSupPlanOverviewVModel m in daily.planOverviewList)
                {
                    table.Cell(row + itemCnt, 1).Range.Text = m.Description;
                    table.Cell(row + itemCnt, 2).Range.Text = m.Unit;
                    table.Cell(row + itemCnt, 3).Range.Text = String.Format("{0:0.####}", m.Quantity);
                    table.Cell(row + itemCnt, 4).Range.Text = String.Format("{0:0.####}", m.TodayConfirm);
                    table.Cell(row + itemCnt, 5).Range.Text = String.Format("{0:0.####}", m.TodayConfirm + m.TotalAccConfirm);
                    table.Cell(row + itemCnt, 6).Range.Text = m.Memo;
                    itemCnt++;
                }

                if (docMode == 2)
                {
                    filename = filename.Replace(".docx", ".pdf");
                    tarfile = tarfile.Replace(".docx", ".pdf");
                    doc.SaveAs2(tarfile, WdExportFormat.wdExportFormatPDF);

                }

                if (doc != null) doc.Close();
                if (wordApp != null) wordApp.Quit();

                return true;
            }
            catch(Exception e)
            {
                if (doc != null) doc.Close();
                if (wordApp != null) wordApp.Quit();
                BaseService.log.Info($"EngName :{eng.EngName}\nErr:{e.Message }\nInnerError:{e.InnerException}\nStackTrace:{e.StackTrace}\n\n");
                return false;
            }

            if (doc != null) doc.Close();
            if (wordApp != null) wordApp.Quit();

            return false;
        }

        //匯入 多日期 excel 更新 以施工日誌excel s20230303
        public JsonResult UploadMultiSDailyLog(int id)
        {
            List<EngMainEditVModel> items = new EngMainService().GetItemBySeq<EngMainEditVModel>(id);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }
            EngMainEditVModel eng = items[0];
            List<EPCSchProgressHeaderVModel> scItems = new SchProgressPayItemService().GetHeaderList<EPCSchProgressHeaderVModel>(id);
            if (scItems.Count == 0 || scItems[0].SPState != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "預訂定度作業未完成, 無法作業"
                });
            }

            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                string fileName;
                try
                {
                    fileName = SaveFile(file);
                }
                catch
                {
                    return Json(new
                    {
                        result = -1,
                        message = "上傳檔案儲存失敗"
                    });
                }
                string errMsg = "";
                int result = checkMiscData(fileName, eng, ref errMsg);
                if (result < 0)
                {
                    string msg;
                    switch (result)
                    {
                        case -1: msg = "Excel解析發生錯誤"; break;
                        case -2: msg = "資料建立錯誤"; break;
                        case -4: msg = "日期項目 資料錯誤"; break;
                        default:
                            msg = "未知錯誤:" + result.ToString(); break;
                    }
                    return Json(new
                    {
                        result = -1,
                        message = msg
                    });
                }
                //
                result = readExcelMultiData(SupDailyReportService._Supervise, fileName, eng, ref errMsg);
                if (result < 0)
                {
                    string msg;
                    switch (result)
                    {
                        case -1: msg = "Excel解析發生錯誤"; break;
                        case -2: msg = "工程名稱/編號 資料錯誤"; break;
                        case -3: msg = "施工項目 資料錯誤 " + errMsg; break;
                        case -4: msg = "日期項目 資料錯誤"; break;
                        case -5: msg = "更新資料錯誤 " + errMsg; break;
                        default:
                            msg = "未知錯誤:" + result.ToString(); break;
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
        //s20230303 檢查當日日誌是否建立
        private int checkMiscData(string filename, EngMainEditVModel eng, ref string errMsg)
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
                Worksheet sheet = dict["施工日誌"];

                if (sheet.Cells[2, 3].Value.ToString() != eng.EngName || sheet.Cells[3, 3].Value.ToString() != eng.EngNo) return -2;

                //檢查 日期 是否一致
                int col = 0, inx = 0;
                DateTime? startDate = null, endDate = null;
                while (sheet.Cells[4, col + 8].Value != null)
                {
                    DateTime d = DateTime.Parse(sheet.Cells[4, col + 8].Value.ToString());
                    if (inx == 0) startDate = d;
                    endDate = d;
                    inx++;
                    col += 2;
                }
                if (!startDate.HasValue || !endDate.HasValue) return -4;
                int dayCount = endDate.Value.Subtract(startDate.Value).Days + 1;
                if (sheet.Cells[1, 8].Value == null && dayCount != 1)
                    return -4;
                else if (sheet.Cells[1, 8].Value != null && sheet.Cells[1, 8].Value.ToString() != dayCount.ToString())
                    return -4;

                DateTime tarDate = startDate.Value;
                while (endDate.Value.Subtract(tarDate).Days >= 0)
                {
                    SupDailyReportMiscModel miscItem;
                    EPCSupDailyDateVModel supDailyItem;
                    List<SupPlanOverviewModel> planItems = null;
                    List<EPCSupDailyDateVModel> dateList = supDailyReportService.GetSupDailyDate<EPCSupDailyDateVModel>(SupDailyReportService._Supervise, eng.Seq, tarDate);
                    if (dateList.Count == 0)
                    {//該日未有紀錄

                        supDailyItem = new EPCSupDailyDateVModel()
                        {
                            Seq = -1,
                            DataType = SupDailyReportService._Supervise,
                            EngMainSeq = eng.Seq,
                            ItemDate = tarDate,
                            ItemState = 0
                        };
                        miscItem = new SupDailyReportMiscModel() { Seq = -1 };

                        List<EPCSupDailyDateVModel> constructionList = supDailyReportService.GetSupDailyDate<EPCSupDailyDateVModel>(SupDailyReportService._Construction, eng.Seq, tarDate);
                        if (constructionList.Count > 0)
                        {//複製施工日誌
                            planItems = supDailyReportService.GetPlanOverview<SupPlanOverviewModel>(constructionList[0].Seq);
                            foreach (SupPlanOverviewModel m in planItems)
                            {
                                m.Seq = -1;
                            }
                        }
                        if (planItems == null || planItems.Count == 0)
                        {
                            planItems = supDailyReportService.GetPayitemList<SupPlanOverviewModel>(eng.Seq, tarDate);
                            if (eng.SchCompDate.Value.Subtract(tarDate).Days < 0)
                            {//超出工程預定完工日期
                                foreach (SupPlanOverviewModel m in planItems)
                                {
                                    m.DayProgress = 0;
                                }
                            }
                        }
                        if (!supDailyReportService.MiscAdd(supDailyItem, miscItem, planItems)) return -2;
                    }

                    tarDate = tarDate.AddDays(1);
                }
                //
                workbook.Close();
                appExcel.Quit();

                return 1;
            }
            catch (Exception e)
            {
                if (workbook != null) workbook.Close(false);
                if (appExcel != null) appExcel.Quit();
                return -1;
            }
        }

        // *** 月曆設定 **************************************
        /*/月曆每日狀態資料
        private JsonResult GetCalDayInfo(int id, DateTime fromDate, int mode)
        {
            List<EPCTendeVModel> tenders = new EPCProgressManageService().GetEngLinkTenderBySeq<EPCTendeVModel>(id);
            if (tenders.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程錯誤"
                });
            }
            EPCTendeVModel tender = tenders[0];
            if (!tender.StartDate.HasValue || !tender.SchCompDate.HasValue)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程日期錯誤"
                });
            }
            //日期範圍
            DateTime startDate = tender.StartDate.Value;
            if (!String.IsNullOrEmpty(tender.ActualStartDate))//實際開工日期
            {
                DateTime? dt = Utils.StringChs2DateToDateTime(tender.ActualStartDate);
                if (dt != null) startDate = dt.Value;
            }
            if (fromDate > startDate) startDate = fromDate;

            DateTime monthEndDate = fromDate.AddMonths(1).AddDays(-1);//月底
            DateTime endDate = monthEndDate;
            if (endDate > tender.SchCompDate.Value) endDate = tender.SchCompDate.Value; //預計完工日期

            DateTime nowDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));//今日
            if (endDate >= nowDate) endDate = nowDate.AddDays(-1);

            Dictionary<string, EPCCalInfoVModel> cals = new Dictionary<string, EPCCalInfoVModel>();
            //未填寫
            DateTime bDate = startDate;
            while (bDate <= endDate)
            {
                cals.Add(bDate.ToString("yyyy-MM-dd"), new EPCCalInfoVModel() { Mode = 0, ItemDate = bDate });
                bDate = bDate.AddDays(1);
            }
            //停工
            List<EPCReportWorkVModel> stopList = iService.GetWorkByPrjSeq<EPCReportWorkVModel>(id);
            foreach (EPCReportWorkVModel m in stopList)
            {
                bDate = m.SStopWorkDate;
                DateTime eDate = m.BackWorkDate.HasValue ? m.BackWorkDate.Value : m.EStopWorkDate;//s20230526
                while (bDate <= eDate)
                {
                    string key = bDate.ToString("yyyy-MM-dd");
                    if (cals.ContainsKey(key))
                    {
                        cals[key].Mode = 1;
                    }
                    else if (bDate <= monthEndDate)
                    {
                        cals.Add(key, new EPCCalInfoVModel() { Mode = 1, ItemDate = bDate });
                    }
                    bDate = bDate.AddDays(1);
                }
            }
            //已填寫
            List<EPCCalInfoVModel> fillList = supDailyReportService.GetCalendarInfo<EPCCalInfoVModel>(id, fromDate, mode);
            foreach (EPCCalInfoVModel m in fillList)
            {
                if (cals.ContainsKey(m.DateStr))
                {
                    cals[m.DateStr].Mode = 2;
                }
                else
                {
                    cals.Add(m.ItemDate.ToString("yyyy-MM-dd"), new EPCCalInfoVModel() { Mode = 2, ItemDate = m.ItemDate });
                }
            }
            List<EPCCalInfoVModel> calList = new List<EPCCalInfoVModel>();
            foreach (string key in cals.Keys)
            {
                calList.Add(cals[key]);
            }
            UserInfo userInfo = Utils.getUserInfo();
            bool admin = userInfo.IsAdmin || userInfo.IsDepartmentAdmin || userInfo.IsDepartmentUser;//s20230417
            return Json(new
            {
                result = 0,
                items = calList,
                admin = admin
            });
        }
        */
        //設定停復工
        public JsonResult WorkSave(SupDailyReportWorkModel item)
        {
            bool stopWorkApprovalFile = false, backWorkApprovalFile = false;
            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];
                int mode = -1;
                if (fileName == "StopWorkFile") mode = 1;
                else if (fileName == "BackWorkFile") mode = 2;
                if(mode > 0)
                {
                    if (file.ContentLength > 0)
                    {
                        try
                        {
                            if (mode == 1)
                            {
                                item.StopWorkApprovalFile = new CalUtils().SaveFile(file, item.EngMainSeq, "ApproveStop-");
                                stopWorkApprovalFile = true;
                            }
                            else if (mode == 2)
                            {
                                item.BackWorkApprovalFile = new CalUtils().SaveFile(file, item.EngMainSeq, "ApproveBack-");
                                backWorkApprovalFile = true;
                            }
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                            return Json(new
                            {
                                result = -1,
                                message = "上傳檔案失敗"
                            });
                        }
                    }
                }
            }
            if (item.Seq == -1)
            {
                if (iService.WorkAdd(item))
                {
                    return Json(new
                    {
                        result = 0,
                        msg = "新增完成"
                    });

                }
                else
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "新增失敗"
                    });
                }
            } else
            {
                List<SupDailyReportWorkModel> workItems = iService.GetWorkBySeq<SupDailyReportWorkModel>(item.Seq);
                if(workItems.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "儲存失敗"
                    });
                }
                SupDailyReportWorkModel workItem = workItems[0];
                if(!stopWorkApprovalFile) item.StopWorkApprovalFile = workItem.StopWorkApprovalFile;
                if(!backWorkApprovalFile) item.BackWorkApprovalFile = workItem.BackWorkApprovalFile;

                if (iService.WorkUpdate(item))
                {
                    if (stopWorkApprovalFile) new CalUtils().DelFile(workItem.EngMainSeq, workItem.StopWorkApprovalFile);
                    if (backWorkApprovalFile) new CalUtils().DelFile(workItem.EngMainSeq, workItem.BackWorkApprovalFile);
                    //
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
        }
        /*private string SaveFile(HttpPostedFileBase file, int engMainSeq, string fileHeader)
        {
            string filePath = Utils.GetTenderFolder(engMainSeq);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string originFileName = file.FileName.ToString().Trim();
            int inx = originFileName.LastIndexOf(".");
            string uniqueFileName = String.Format("{0}{1}{2}", fileHeader, Guid.NewGuid(), originFileName.Substring(inx));

            string fullPath = Path.Combine(filePath, uniqueFileName);
            file.SaveAs(fullPath);

            return uniqueFileName;
        }
        private void DelFile(int engMainSeq, string fileName)
        {
            try
            {
                string filePath = Utils.GetTenderFolder(engMainSeq);
                if (Directory.Exists(filePath))
                {
                    if (fileName != null && fileName.Length > 0)
                    {
                        System.IO.File.Delete(Path.Combine(filePath, fileName));
                    }
                }
            } catch { }
        }*/
        public ActionResult DocDownload(int id, int mode, string fn)
        {
            return new CalUtils().DownloadFile(id, mode, fn);//s20230526
        }
        /*private ActionResult DownloadFile(int id, int mode, string fn)
        {
            string filePath = Utils.GetTenderFolder(id);

            string uniqueFileName = fn;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
                if (System.IO.File.Exists(fullPath))
                {
                    int inx = uniqueFileName.LastIndexOf(".");
                    Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", String.Format("{0}-核定公文檔案.{1}",mode==1 ? "停工" : "復工",uniqueFileName.Substring(inx)));
                }
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }*/
        public JsonResult GetWorkList(int id)
        {
            return new CalUtils().GetWorkList(id);// Json(iService.GetWorkByPrjSeq<EPCReportWorkVModel>(id));
        }

        //設定展延工期
        public JsonResult ExtensionDel(int id)
        {
            if (iService.ExtensionDel(id) == 1)
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
        public JsonResult ExtensionUpdate(SupDailyReportExtensionModel item)
        {
            if (iService.ExtensionUpdate(item))
            {
                return Json(new
                {
                    result = 0,
                    msg = "更新完成"
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "更新失敗"
                });
            }
        }
        public JsonResult ExtensionAdd(SupDailyReportExtensionModel item)
        {
            if (iService.ExtensionAdd(item))
            {
                return Json(new
                {
                    result = 0,
                    msg = "新增完成"
                });

            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "新增失敗"
                });
            }
        }
        public JsonResult GetExtensionList(int id)
        {
            return new CalUtils().GetExtensionList(id); //Json(iService.GetExtensionByPrjSeq<EPCReportExtensionVModel>(id));
        }

        //設定不計工期
        public JsonResult NoDurationDel(int id)
        {
            if (iService.NoDurationDel(id) == 1)
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
        public JsonResult NoDurationAdd(SupDailyReportNoDurationModel item)
        {
            if (iService.NoDurationAdd(item))
            {
                return Json(new
                {
                    result = 0,
                    msg = "新增完成"
                });

            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "新增失敗"
                });
            }
        }
        public JsonResult GetNoDurationList(int id)
        {
            return Json(iService.GetNoDurationByPrjSeq<EPCReportNoDurationVModel>(id));
        }

        //設定假日計工期
        public JsonResult HolidayDel(int id)
        {
            if (iService.HolidayDel(id)==1)
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
        public JsonResult HolidayAdd(SupDailyReportHolidayModel item)
        {
            if(iService.HolidayAdd(item))
            {
                return Json(new
                {
                    result = 0,
                    msg = "新增完成"
                });

            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "新增失敗"
                });
            }
        }
        public JsonResult GetHolidayList(int id)
        {
            return Json(iService.GetHolidayByPrjSeq<EPCReportHolidayVModel>(id));
        }

        //工程標案
        public JsonResult GetTrender(int id)
        {
            EPCProgressManageService srv = new EPCProgressManageService();
            //List<EPCTendeVMode> tender = new EPCTenderService().GetTrender<EPCTendeVMode>(id);
            List<EPCTendeVModel> tender = srv.GetEngLinkTenderBySeq<EPCTendeVModel>(id);//20220602
                                                                                        //
            var dList =  new SchProgressPayItemService().GetDateListVer<EPCSchProgressVModel>(id);
            var e = new Encryption();
            var s = e.encryptCode($"{Request.UserHostAddress},{id},{DateTime.Now.ToOADate()}");
            s = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(s));
            if (tender.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "查無資料"
                });

            }
            else
            {
                //shioulo 20230216 日誌權限
                bool canEditConstruction = false;
                bool canEditSupervision = false;
                /*UserInfo userInfo = Utils.getUserInfo();
                if (userInfo != null)
                {
                    if (userInfo.IsDepartmentAdmin || userInfo.IsAdmin)
                    {
                        canEditConstruction = true;
                        canEditSupervision = true;
                    }
                    else
                    {
                        if (userInfo.IsBuildContractor) canEditConstruction = true;
                        if (userInfo.IsSupervisorUnit) canEditSupervision = true;
                    }
                }*/
                //暫不檢查 20230216
                canEditConstruction = true;
                canEditSupervision = true;
                bool isContractBreaked = false;
                List<EngProgressVModel> engProgress = srv.GetEngProgressAndEngChange<EngProgressVModel>(id, SupDailyReportService._Supervise, DateTime.Now.ToString("yyyy-M-d"));//20230227
                var calUtils = new CalUtils();
                using(var context = new EQC_NEW_Entities() )
                {
                    isContractBreaked = context.SupDailyDate.Where(r => r.EngMainSeq == id)
                        .OrderByDescending(r => r.ItemDate).FirstOrDefault()?.ItemDate > dList.LastOrDefault()?.SPDate;
                }
                if(engProgress.Count>0)
                {
                    tender[0].SchProgress = engProgress[0].SchProgress;
                    tender[0].AcualProgress = engProgress[0].AcualProgress;
                }
                return Json(new
                {
                    result = 0,
                    editConstruction = canEditConstruction,
                    editSupervision = canEditSupervision,
                    item = tender[0],
                    schOrgStartDate = dList.FirstOrDefault()?.ItemDate,
                    schOrgEndDate = dList.LastOrDefault()?.ItemDate,
                    isContractBreaked = isContractBreaked,
                    ProgressDiagramAceessCode = s,
                    IsSupervision = Utils.getUserInfo().RoleSeq == 5
                });
            }
        }

        public JsonResult GetDownloadTag() {

            int downloadTag = new EPCProgressManageService().GetProgressManageTag("ConstructionDownload");
            return Json(new
            {
                result = 0,
                downloadTag = downloadTag
            });
        }
    }
}