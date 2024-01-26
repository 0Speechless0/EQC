using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class ECProgressManageController : Controller
    {//工程變更 - 進度管理
        EPCCalendarService iService = new EPCCalendarService();
        ECSupDailyReportService supDailyReportService = new ECSupDailyReportService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }

        //檢查日誌日期 是否屬於工程變更範圍 s20230412
        public JsonResult CheckActiveDate(int id, DateTime tarDate)
        {
            List<EPCProgressEngChangeListVModel> engChanges = new SchEngChangeService().GetEngChangeList<EPCProgressEngChangeListVModel>(id);
            if (engChanges.Count > 0)
            {
                EPCProgressEngChangeListVModel engChange = engChanges[0];
                if (tarDate.Subtract(engChange.StartDate.Value).Days < 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = String.Format("日期{0}之前的日誌, 請至 進度管理 頁面填寫", engChange.StartDate.Value.ToString("yyyy-M-d"))
                    });
                }
            }
            foreach(EPCProgressEngChangeListVModel engChange in engChanges)
            {//s20230526
                if (engChange.ChangeType == SchEngChangeService._cyStopWork && tarDate.Subtract(engChange.StartDate.Value).Days >=0
                    && (!engChange.EndDate.HasValue || tarDate.Subtract(engChange.EndDate.Value).Days <= 0)) 
                {
                    return Json(new
                    {
                        result = -1,
                        msg = String.Format("該日期為 停工期間, 不能作業")
                    });
                } else if (engChange.ChangeType == SchEngChangeService._cyTerminateContract && tarDate.Subtract(engChange.StartDate.Value).Days >= 0
                    && (!engChange.EndDate.HasValue || tarDate.Subtract(engChange.EndDate.Value).Days <= 0))
                {
                    return Json(new
                    {
                        result = -1,
                        msg = String.Format(String.Format("此工程 {0} 已契約終止及解除, 不能作業", engChange.chsStartDate))
                    });
                }
            }

            return Json(new
            {
                result = 0,
                msg = ""
            });
        }

        //******************************************************************************
        //施工日誌 月曆資料
        //******************************************************************************
        public JsonResult GetConstructionInfo(int id, DateTime fromDate)
        {
            return new CalUtils().GetCalDayInfo(id, fromDate, ECSupDailyReportService._Construction);
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
            EC_SupDailyReportMiscConstructionModel miscItem;
            ECSupDailyDateVModel supDailyItem;
            List<ECSupPlanOverviewVModel> planItems;
            List<ECSupDailyDateVModel> dateList = supDailyReportService.GetSupDailyDate<ECSupDailyDateVModel>(ECSupDailyReportService._Construction, id, tarDate);
            if(dateList.Count == 0)
            {//該日未有紀錄

                supDailyItem = new ECSupDailyDateVModel() {
                    Seq = -1,
                    DataType = ECSupDailyReportService._Construction,
                    EngMainSeq = id,
                    ItemDate = tarDate,
                    ItemState = 0,
                    FillinDate = DateTime.Now
                };
                miscItem = new EC_SupDailyReportMiscConstructionModel() { Seq = -1 };
                planItems = supDailyReportService.GetPayitemList<ECSupPlanOverviewVModel>(id, tarDate);
                if(eng.SchCompDate.Value.Subtract(tarDate).Days <0)
                {//超出工程預定完工日期
                    foreach(EC_SupPlanOverviewModel m in planItems)
                    {
                        m.DayProgress = 0;
                    }
                }
            } else
            {
                supDailyItem = dateList[0];
                List<EC_SupDailyReportMiscConstructionModel> miscList = supDailyReportService.GetMiscConstruction<EC_SupDailyReportMiscConstructionModel>(supDailyItem.Seq);
                miscItem = miscList[0];
                planItems = supDailyReportService.GetPlanOverviewAndTotal<ECSupPlanOverviewVModel>(supDailyItem.Seq);
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
            if(supDailyReportService.DailyLogCompleted(id))
            {
                return Json(new
                {
                    result = 0,
                    msg = "設定完成"
                });
            } else
            {
                return Json(new
                {
                    result = -1,
                    msg = "設定失敗"
                });
            }
        }
        //複製前日材料,人員,機具資料 s20231116
        public JsonResult CopyConstructionMiscData(ECSupDailyDateVModel supDailyItem)
        {
            DateTime dayBefore = DateTime.Parse(supDailyItem.ItemDateStr).AddDays(-1);
            SupDailyReportConstructionEquipmentService equipmentService = new SupDailyReportConstructionEquipmentService();
            List<EC_SupDailyReportConstructionEquipmentModel> equipments = equipmentService.GetDayBeforeItemsEC<EC_SupDailyReportConstructionEquipmentModel>(supDailyItem.EngMainSeq, dayBefore);
            //
            SupDailyReportConstructionPersonService personService = new SupDailyReportConstructionPersonService();
            List<EC_SupDailyReportConstructionPersonModel> persons = personService.GetDayBeforeItemsEC<EC_SupDailyReportConstructionPersonModel>(supDailyItem.EngMainSeq, dayBefore);
            //
            SupDailyReportConstructionMaterialService materialService = new SupDailyReportConstructionMaterialService();
            List<EC_SupDailyReportConstructionMaterialModel> materials = materialService.GetDayBeforeItemsEC<EC_SupDailyReportConstructionMaterialModel>(supDailyItem.EngMainSeq, dayBefore);
            if (equipments.Count > 0 || persons.Count > 0 || materials.Count > 0)
            {
                if (supDailyReportService.CopyConstructionMiscData(supDailyItem, equipments, persons, materials))
                {
                    return Json(new
                    {
                        result = 0,
                        msg = "複製完成"
                    });
                }
                else
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
        public JsonResult ConstructionSave(ECSupDailyDateVModel supDailyItem, EC_SupDailyReportMiscConstructionModel miscItem, List<EC_SupPlanOverviewModel> planItems)
        {
            bool result;
            if (supDailyItem.Seq == -1)
            {//預設值
                DateTime dayBefore = DateTime.Parse(supDailyItem.ItemDateStr).AddDays(-1);
                //
                SupDailyReportConstructionEquipmentService equipmentService = new SupDailyReportConstructionEquipmentService();
                List<EC_SupDailyReportConstructionEquipmentModel> equipments = equipmentService.GetDayBeforeItemsEC<EC_SupDailyReportConstructionEquipmentModel>(supDailyItem.EngMainSeq, dayBefore);//s20231113
                if(equipments.Count == 0)
                {
                    equipments = equipmentService.GetDefaultItemsEC<EC_SupDailyReportConstructionEquipmentModel>(supDailyItem.EngMainSeq, dayBefore);
                }
                //
                SupDailyReportConstructionPersonService personService =new SupDailyReportConstructionPersonService();
                List<EC_SupDailyReportConstructionPersonModel> persons = personService.GetDayBeforeItemsEC<EC_SupDailyReportConstructionPersonModel>(supDailyItem.EngMainSeq, dayBefore);//s20231113
                if (persons.Count == 0)
                {
                    persons = personService.GetDefaultItemsEC<EC_SupDailyReportConstructionPersonModel>(supDailyItem.EngMainSeq, dayBefore);
                }
                //
                SupDailyReportConstructionMaterialService materialService = new SupDailyReportConstructionMaterialService();
                List<EC_SupDailyReportConstructionMaterialModel> materials = materialService.GetDayBeforeItemsEC<EC_SupDailyReportConstructionMaterialModel>(supDailyItem.EngMainSeq, dayBefore);//s20231113
                if(materials.Count == 0)
                {
                    materials = materialService.GetDefaultItemsEC<EC_SupDailyReportConstructionMaterialModel>(supDailyItem.EngMainSeq, dayBefore);
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
        public ActionResult DnMultiDate(string sd, string ed, int eId)
        {
            if(String.IsNullOrEmpty(sd) || String.IsNullOrEmpty(ed))
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
                }, JsonRequestBehavior.AllowGet);
            }
            List<EPCTendeVModel> tenders = new EPCProgressManageService().GetEngLinkTenderBySeq<EPCTendeVModel>(eId);
            if (tenders.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EPCTendeVModel eng = tenders[0];
            try
            {
                DateTime startDate = DateTime.Parse(sd);
                DateTime endDate = DateTime.Parse(ed);

                if (endDate.Subtract(startDate).Days < 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "日期區間設定錯誤"
                    }, JsonRequestBehavior.AllowGet);
                }

                //s20230412
                List<EPCProgressEngChangeListVModel> engChanges = new SchEngChangeService().GetEngChangeList<EPCProgressEngChangeListVModel>(eng.Seq);
                if (engChanges.Count > 0)
                {
                    EPCProgressEngChangeListVModel engChange = engChanges[0];
                    if (startDate.Subtract(engChange.StartDate.Value).Days < 0 || endDate.Subtract(engChange.StartDate.Value).Days < 0)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = String.Format("日期區間不可小於{0}工程變更日期", engChange.StartDate.Value.ToString("yyyy-M-d"))
                        }, JsonRequestBehavior.AllowGet);
                    }
                    foreach(EPCProgressEngChangeListVModel ec in engChanges)
                    {
                        DateTime eDate = ec.EndDate.HasValue ? ec.EndDate.Value : ec.SchCompDate.Value;
                        if (endDate.Subtract(eDate).Days <= 0)
                        {
                            if (startDate.Subtract(ec.StartDate.Value).Days < 0)
                            {
                                return Json(new
                                {
                                    result = -1,
                                    message = String.Format("日期可在工程變更日期區間:{0}~{1}\n或其它工程變更日期區間內, 但不可以跨區間下載", ec.StartDate.Value.ToString("yyyy-M-d"), eDate.ToString("yyyy-M-d"))
                                }, JsonRequestBehavior.AllowGet);
                            }
                            break;
                        }
                    }
                    
                    foreach (EPCProgressEngChangeListVModel ec in engChanges)
                    {//s20230528
                        if (ec.ChangeType == SchEngChangeService._cyStopWork || ec.ChangeType == SchEngChangeService._cyTerminateContract) {
                            bool fErr = (startDate.Subtract(ec.StartDate.Value).Days >= 0 && !ec.EndDate.HasValue)
                                || (startDate.Subtract(ec.StartDate.Value).Days >= 0 && startDate.Subtract(ec.EndDate.Value).Days <= 0)
                                || (endDate.Subtract(ec.StartDate.Value).Days >= 0 && endDate.Subtract(ec.EndDate.Value).Days <= 0)
                                || (startDate.Subtract(ec.StartDate.Value).Days < 0 && endDate.Subtract(ec.EndDate.Value).Days > 0);
                            if (fErr)
                            {
                                return Json(new
                                {
                                    result = -1,
                                    message = String.Format("日期範圍不能涵蓋 停工/契約終止及解除 期間")
                                }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }

                if (endDate.Subtract(startDate).Days > 29)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "避免網路速度影響，僅提供下載、上傳30天日誌(報表)"
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

                return CreateExcelMultiDate(eng, startDate, endDate);
            }
            catch {
                return Json(new
                {
                    result = -1,
                    message = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
        }
        private ActionResult CreateExcelMultiDate(EPCTendeVModel eng, DateTime startDate, DateTime endDate)
        {
            List<ECSupDailyDateVModel> supDailyDateList = supDailyReportService.GetSupDailyDate<ECSupDailyDateVModel>(ECSupDailyReportService._Construction, eng.Seq, startDate, endDate);

            string filename = Utils.CopyTemplateFile("進度管理-施工日誌.xlsx", ".xlsx");
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
                Worksheet sheet = dict["施工日誌"];
                if (supDailyDateList.Count > 0)
                {
                    PayitemSheet(sheet, eng, supDailyDateList);
                    sheet.Protect(String.Format("{0}-{1}", eng.EngNo, supDailyDateList[0].ItemDateStr));//shioulo20221214
                }

                workbook.Save();
                workbook.Close();
                appExcel.Quit();

                Stream iStream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
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
        private void PayitemSheet(Worksheet sheet, EPCTendeVModel eng, List<ECSupDailyDateVModel> supDailyDates)
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
            foreach (ECSupDailyDateVModel supDailyDate in supDailyDates)
            {
                List<EC_SupPlanOverviewModel> spList = supDailyReportService.GetPlanOverview<EC_SupPlanOverviewModel>(supDailyDate.Seq);
                sheet.Cells[4, col+8] = supDailyDate.ItemDateStr;// 日期
                int row = 6;
                foreach (EC_SupPlanOverviewModel m in spList)
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
                int result = readExcelMultiData(ECSupDailyReportService._Construction, fileName, eng, ref errMsg);
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


                List<ECSupDailyDateVModel> supDailyDateList = supDailyReportService.GetSupDailyDate<ECSupDailyDateVModel>(mode, eng.Seq, startDate.Value, endDate.Value);
                if (dayCount != supDailyDateList.Count) return -4;
                //
                col = 0;
                inx = 0;
                foreach (ECSupDailyDateVModel supDailyDate in supDailyDateList)
                {
                    errMsg = supDailyDate.ItemDateStr;
                    List<EC_SupPlanOverviewModel> items = supDailyReportService.GetPlanOverview<EC_SupPlanOverviewModel>(supDailyDate.Seq);
                    if (items.Count == 0) return -3;
                    //
                    int row = 6;
                    foreach (EC_SupPlanOverviewModel m in items)
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
            EC_SupDailyReportMiscConstructionModel miscItem;
            ECSupDailyDateVModel supDailyItem;
            List<EC_SupPlanOverviewModel> planItems;
            List<ECSupDailyDateVModel> dateList = supDailyReportService.GetSupDailyDate<ECSupDailyDateVModel>(ECSupDailyReportService._Construction, id, tarDate);
            if (dateList.Count == 0)
            {//該日未有紀錄

                supDailyItem = new ECSupDailyDateVModel()
                {
                    Seq = -1,
                    DataType = ECSupDailyReportService._Construction,
                    EngMainSeq = id,
                    ItemDate = tarDate,
                    ItemState = 0
                };
                miscItem = new EC_SupDailyReportMiscConstructionModel() { Seq = -1 };
                planItems = supDailyReportService.GetPayitemList<EC_SupPlanOverviewModel>(id, tarDate);
                if (eng.SchCompDate.Value.Subtract(tarDate).Days < 0)
                {//超出工程預定完工日期
                    foreach (EC_SupPlanOverviewModel m in planItems)
                    {
                        m.DayProgress = 0;
                    }
                }
                DateTime dayBefore = tarDate.AddDays(-1);
                List<EC_SupDailyReportConstructionEquipmentModel> equipments = new SupDailyReportConstructionEquipmentService().GetDefaultItemsEC<EC_SupDailyReportConstructionEquipmentModel>(supDailyItem.EngMainSeq, dayBefore);
                List<EC_SupDailyReportConstructionPersonModel> persons = new SupDailyReportConstructionPersonService().GetDefaultItemsEC<EC_SupDailyReportConstructionPersonModel>(supDailyItem.EngMainSeq, dayBefore);
                List<EC_SupDailyReportConstructionMaterialModel> materials = new SupDailyReportConstructionMaterialService().GetDefaultItemsEC<EC_SupDailyReportConstructionMaterialModel>(supDailyItem.EngMainSeq, dayBefore);
                return supDailyReportService.ConstructionAdd(supDailyItem, miscItem, planItems, equipments, persons, materials);
            }

            return true;
        }

        //單一日期 下載工程範本(excel)
        public ActionResult Download(int id)
        {
            List<ECSupDailyDateVModel> supDailyDateList = supDailyReportService.GetSupDailyDateBySeq<ECSupDailyDateVModel>(id);
            if (supDailyDateList.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料錯誤"
                });
            }
            ECSupDailyDateVModel supDailyDate = supDailyDateList[0];

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

            /*List<EC_SupPlanOverviewModel> miscList = supDailyReportService.GetPlanOverview<EC_SupPlanOverviewModel>(supDailyDate.Seq);
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
            List<EC_SupDailyReportConstructionMaterialModel> lists = new ECSupDailyReportConstructionMaterialService().GetList<EC_SupDailyReportConstructionMaterialModel>(id);

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
                item = new EC_SupDailyReportConstructionMaterialModel() { Seq = -1 }
            });
        }
        public JsonResult UpdateMaterialRecords(EC_SupDailyReportConstructionMaterialModel m)
        {
            int state;
            if (m.Seq == -1)
                state = new ECSupDailyReportConstructionMaterialService().AddRecord(m);
            else
                state = new ECSupDailyReportConstructionMaterialService().UpdateRecord(m);
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
            int state = new ECSupDailyReportConstructionMaterialService().DelRecord(id);
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
        //工地人員概況
        //人員類型清單
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
            List<EC_SupDailyReportConstructionPersonModel> lists = new ECSupDailyReportConstructionPersonService().GetList<EC_SupDailyReportConstructionPersonModel>(id);

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
                item = new EC_SupDailyReportConstructionPersonModel() { Seq = -1 }
            });
        }
        public JsonResult UpdatePersonRecords(EC_SupDailyReportConstructionPersonModel m)
        {
            int state;
            if (m.Seq == -1)
                state = new ECSupDailyReportConstructionPersonService().AddRecord(m);
            else
                state = new ECSupDailyReportConstructionPersonService().UpdateRecord(m);
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
            int state = new ECSupDailyReportConstructionPersonService().DelRecord(id);
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
        //工地人員概況
        //機具類型清單
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
            List<ECSupDailyReportConstructionEquipmentModelVModel> lists = new ECSupDailyReportConstructionEquipmentService().GetList<ECSupDailyReportConstructionEquipmentModelVModel>(id);

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
                item = new EC_SupDailyReportConstructionEquipmentModel() { Seq = -1 }
            });
        }
        public JsonResult UpdateEquipmentRecords(EC_SupDailyReportConstructionEquipmentModel m)
        {
            int state;
            if (m.Seq == -1)
                state = new ECSupDailyReportConstructionEquipmentService().AddRecord(m);
            else
                state = new ECSupDailyReportConstructionEquipmentService().UpdateRecord(m);
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
            int state = new ECSupDailyReportConstructionEquipmentService().DelRecord(id);
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
        public ActionResult DownloadCDailyMulti(string sd, string ed, int eId, string eEM)
        {
            if (String.IsNullOrEmpty(sd) || String.IsNullOrEmpty(ed))
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
                DateTime startDate = DateTime.Parse(sd);
                DateTime endDate = DateTime.Parse(ed);
                if (endDate.Subtract(startDate).Days > 29)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "避免網路速度影響，僅提供下載、上傳30天日誌(報表)"
                    }, JsonRequestBehavior.AllowGet);
                }

                List<ECSupDailyDate1VModel> supDailyDateList = supDailyReportService.GetSupDailyDateAndCount<ECSupDailyDate1VModel>(ECSupDailyReportService._Construction, eng.Seq, startDate, endDate);
                if(supDailyDateList.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "日期區間, 無日誌資料"
                    }, JsonRequestBehavior.AllowGet);
                }

                string uuid = Guid.NewGuid().ToString("B").ToUpper();
                string folder = Path.Combine(Path.GetTempPath(), uuid);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                string srcFile = Path.Combine(Utils.GetTemplateFilePath(), "附錄四_公共工程施工日誌V2.docx");
                foreach (ECSupDailyDate1VModel supDailyDate in supDailyDateList)
                {
                    ECDailyVModel daily = new ECDailyVModel();
                    daily.miscList = supDailyReportService.GetMiscConstruction<EC_SupDailyReportMiscConstructionModel>(supDailyDate.Seq);
                    if (daily.miscList.Count == 1)
                    {
                        string tarfile = Path.Combine(folder, String.Format("{0} 公共工程施工日誌[{1}].docx", eng.EngNo, supDailyDate.ItemDateStr));
                        System.IO.File.Copy(srcFile, tarfile);

                        daily.planOverviewList = supDailyReportService.GetPlanOverviewAndTotalFilter<ECSupPlanOverviewVModel>(supDailyDate.Seq, eEM);
                        daily.materialList = new ECSupDailyReportConstructionMaterialService().GetList<EC_SupDailyReportConstructionMaterialModel>(supDailyDate.Seq);
                        daily.personList = new ECSupDailyReportConstructionPersonService().GetList<EC_SupDailyReportConstructionPersonModel>(supDailyDate.Seq);
                        daily.equipmentList = new ECSupDailyReportConstructionEquipmentService().GetList<EC_SupDailyReportConstructionEquipmentModel>(supDailyDate.Seq);

                        CreateConstructionDoc(tarfile, eng, supDailyDate, daily, tender, 1);
                    }
                }

                string zipFile = Path.Combine(Path.GetTempPath(), uuid + "-施工日誌.zip");

                ZipFile.CreateFromDirectory(folder, zipFile);// AddFiles(files, "ProjectX");
                Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", String.Format("{0} 公共工程施工日誌[{1}]-施工日誌.zip", eng.EngNo, supDailyDateList[0].ItemDateStr));
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
        public ActionResult DownloadCDailyDoc(int id, int docMode, string eEM)
        {
            List<ECSupDailyDate1VModel> supDailyDateList = supDailyReportService.GetSupDailyDateAndCount<ECSupDailyDate1VModel>(id);
            if (supDailyDateList.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            ECSupDailyDate1VModel supDailyDate = supDailyDateList[0];

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

            ECDailyVModel daily = new ECDailyVModel();
            daily.miscList = supDailyReportService.GetMiscConstruction<EC_SupDailyReportMiscConstructionModel>(supDailyDate.Seq);
            if (daily.miscList.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "日誌資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            daily.planOverviewList = supDailyReportService.GetPlanOverviewAndTotalFilter<ECSupPlanOverviewVModel>(supDailyDate.Seq, eEM);
            daily.materialList = new ECSupDailyReportConstructionMaterialService().GetList<EC_SupDailyReportConstructionMaterialModel>(supDailyDate.Seq);
            daily.personList = new ECSupDailyReportConstructionPersonService().GetList<EC_SupDailyReportConstructionPersonModel>(supDailyDate.Seq);
            daily.equipmentList = new ECSupDailyReportConstructionEquipmentService().GetList<EC_SupDailyReportConstructionEquipmentModel>(supDailyDate.Seq);

            string filename = String.Format("{0} 公共工程施工日誌[{1}].docx", eng.EngNo, supDailyDate.ItemDateStr);
            string tarfile = Utils.CopyTemplateFile("附錄四_公共工程施工日誌V2.docx", ".docx");
            if (CreateConstructionDoc(tarfile, eng, supDailyDate, daily, tender, docMode))
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
        private bool CreateConstructionDoc(string tarfile, EngMainEditVModel eng, ECSupDailyDate1VModel supDailyDate, ECDailyVModel daily, TenderModel tender, int docMode)
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
                table.Cell(2, 2).Range.Text = String.Format("上午：{0} 下午：{1}", supDailyDate.Weather1, supDailyDate.Weather2);
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
                table.Cell(4, 4).Range.Text = String.Format("{0}天", supDailyDate.dailyCount);
                table.Cell(4, 6).Range.Text = String.Format("{0}天", tender.TotalDays - supDailyDate.dailyCount);
                table.Cell(5, 2).Range.Text = tender.ActualStartDate;
                table.Cell(5, 4).Range.Text = tender.ScheCompletionDate;
                //進度 s20230227
                List<EngProgressVModel> engProgress = supDailyReportService.GetEngProgress<EngProgressVModel>(eng.Seq, ECSupDailyReportService._Construction, supDailyDate.ItemDate.ToString("yyyy-M-d"));
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
                foreach (EC_SupDailyReportConstructionPersonModel m in daily.personList)
                {
                    table.Cell(row + itemCnt, 1).Range.Text = m.KindName;
                    table.Cell(row + itemCnt, 2).Range.Text = m.TodayQuantity.ToString();
                    table.Cell(row + itemCnt, 3).Range.Text = m.AccQuantity.ToString();
                    itemCnt++;
                }
                itemCnt = 0;
                foreach (EC_SupDailyReportConstructionEquipmentModel m in daily.equipmentList)
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
                foreach (EC_SupDailyReportConstructionMaterialModel m in daily.materialList)
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
                foreach (ECSupPlanOverviewVModel m in daily.planOverviewList)
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
            catch
            {
                if (doc != null) doc.Close();
                if (wordApp != null) wordApp.Quit();

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
            return new CalUtils().GetCalDayInfo(id, fromDate, ECSupDailyReportService._Supervise);
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
            ECSupDailyDateVModel supDailyItem;
            List<ECSupPlanOverviewVModel> planItems = null;
            List<ECSupDailyDateVModel> dateList = supDailyReportService.GetSupDailyDate<ECSupDailyDateVModel>(ECSupDailyReportService._Supervise, id, tarDate);
            if (dateList.Count == 0)
            {//該日未有紀錄

                supDailyItem = new ECSupDailyDateVModel()
                {
                    Seq = -1,
                    DataType = ECSupDailyReportService._Supervise,
                    EngMainSeq = id,
                    ItemDate = tarDate,
                    ItemState = 0,
                    FillinDate = DateTime.Now
                };
                miscItem = new SupDailyReportMiscModel() { Seq = -1 };
                //複製前一日雜項 s20230831
                List<SupDailyReportMiscModel> dayBefore = supDailyReportService.GetMiscByDate<SupDailyReportMiscModel>(id, tarDate.AddDays(-1));
                if (dayBefore.Count == 1)
                {
                    miscItem = dayBefore[0];
                }

                List<ECSupDailyDateVModel> constructionList = supDailyReportService.GetSupDailyDate<ECSupDailyDateVModel>(ECSupDailyReportService._Construction, id, tarDate);
                if(constructionList.Count > 0)
                {//複製施工日誌
                    ECSupDailyDateVModel construction = constructionList[0];
                    planItems = supDailyReportService.GetPlanOverview<ECSupPlanOverviewVModel>(construction.Seq);
                    foreach(EC_SupPlanOverviewModel m in planItems)
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
                    planItems = supDailyReportService.GetPayitemList<ECSupPlanOverviewVModel>(id, tarDate);
                    if (eng.SchCompDate.Value.Subtract(tarDate).Days < 0)
                    {//超出工程預定完工日期
                        foreach (EC_SupPlanOverviewModel m in planItems)
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
                planItems = supDailyReportService.GetPlanOverviewAndTotal<ECSupPlanOverviewVModel>(supDailyItem.Seq);
            }

            return Json(new
            {
                result = 0,
                supDailyItem = supDailyItem,
                miscItem = miscItem,
                planItems = planItems
            });
        }
        //更新
        public JsonResult MiscSave(ECSupDailyDateVModel supDailyItem, EC_SupDailyReportMiscModel miscItem, List<EC_SupPlanOverviewModel> planItems)
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
        public ActionResult DownloadSDailyMulti(string sd, string ed, int eId, string eEM)
        {
            if (String.IsNullOrEmpty(sd) || String.IsNullOrEmpty(ed))
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
                DateTime startDate = DateTime.Parse(sd);
                DateTime endDate = DateTime.Parse(ed);
                if (endDate.Subtract(startDate).Days > 29)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "避免網路速度影響，僅提供下載、上傳30天日誌(報表)"
                    }, JsonRequestBehavior.AllowGet);
                }

                List<ECSupDailyDate1VModel> supDailyDateList = supDailyReportService.GetSupDailyDateAndCount<ECSupDailyDate1VModel>(ECSupDailyReportService._Supervise, eng.Seq, startDate, endDate);
                if (supDailyDateList.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "日期區間, 無日誌資料"
                    }, JsonRequestBehavior.AllowGet);
                }

                string uuid = Guid.NewGuid().ToString("B").ToUpper();
                string folder = Path.Combine(Path.GetTempPath(), uuid);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                string srcFile = Path.Combine(Utils.GetTemplateFilePath(), "附錄五_公共工程監造報表V2.docx");
                foreach (ECSupDailyDate1VModel supDailyDate in supDailyDateList)
                {
                    string tarfile = Path.Combine(folder, String.Format("{0} 公共工程監造報表[{1}].docx", eng.EngNo, supDailyDate.ItemDateStr));
                    System.IO.File.Copy(srcFile, tarfile);

                    ECDailySVModel daily = new ECDailySVModel();
                    daily.miscList = supDailyReportService.GetMisc<EC_SupDailyReportMiscModel>(supDailyDate.Seq);
                    if (daily.miscList.Count == 1)
                    {
                        daily.planOverviewList = supDailyReportService.GetPlanOverviewAndTotalFilter<ECSupPlanOverviewVModel>(supDailyDate.Seq, eEM);
                        CreateMiscDoc(tarfile, eng, supDailyDate, daily, tender, 1);
                    }
                }

                string zipFile = Path.Combine(Path.GetTempPath(), uuid + "-監造報表.zip");

                ZipFile.CreateFromDirectory(folder, zipFile);// AddFiles(files, "ProjectX");
                Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", String.Format("{0} 公共工程監造報表[{1}].zip", eng.EngNo, supDailyDateList[0].ItemDateStr));
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
        public ActionResult DownloadSDaily(int id, string eEM)
        {
            return DownloadSDailyDoc(id, 1, eEM);
        }
        public ActionResult DownloadSDailyDoc(int id, int docMode, string eEM)
        {
            List<ECSupDailyDate1VModel> supDailyDateList = supDailyReportService.GetSupDailyDateAndCount<ECSupDailyDate1VModel>(id);
            if (supDailyDateList.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            ECSupDailyDate1VModel supDailyDate = supDailyDateList[0];

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

            ECDailySVModel daily = new ECDailySVModel();
            daily.miscList = supDailyReportService.GetMisc<EC_SupDailyReportMiscModel>(supDailyDate.Seq);
            if (daily.miscList.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    message = "日誌資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            daily.planOverviewList = supDailyReportService.GetPlanOverviewAndTotalFilter<ECSupPlanOverviewVModel>(supDailyDate.Seq, eEM);

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
        public bool CreateMiscDoc(string tarfile, EngMainEditVModel eng, ECSupDailyDate1VModel supDailyDate, ECDailySVModel daily, TenderModel tender, int docMode)
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
                //進度 s20230227
                List<EngProgressVModel> engProgress = supDailyReportService.GetEngProgress<EngProgressVModel>(eng.Seq, ECSupDailyReportService._Construction, supDailyDate.ItemDate.ToString("yyyy-M-d"));
                if (engProgress.Count == 1)
                {
                    table.Cell(6, 2).Range.Text = String.Format("{0}%", engProgress[0].SchProgress);
                    table.Cell(6, 4).Range.Text = String.Format("{0}%", engProgress[0].AcualProgress);
                }
                table.Cell(5, 6).Range.Text = String.Format("原契約：{0}", tender.ContractNo);
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
                foreach (ECSupPlanOverviewVModel m in daily.planOverviewList)
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
            catch
            {
                if (doc != null) doc.Close();
                if (wordApp != null) wordApp.Quit();

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
                result = readExcelMultiData(ECSupDailyReportService._Supervise, fileName, eng, ref errMsg);
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
                    EC_SupDailyReportMiscModel miscItem;
                    ECSupDailyDateVModel supDailyItem;
                    List<EC_SupPlanOverviewModel> planItems = null;
                    List<ECSupDailyDateVModel> dateList = supDailyReportService.GetSupDailyDate<ECSupDailyDateVModel>(ECSupDailyReportService._Supervise, eng.Seq, tarDate);
                    if (dateList.Count == 0)
                    {//該日未有紀錄

                        supDailyItem = new ECSupDailyDateVModel()
                        {
                            Seq = -1,
                            DataType = ECSupDailyReportService._Supervise,
                            EngMainSeq = eng.Seq,
                            ItemDate = tarDate,
                            ItemState = 0
                        };
                        miscItem = new EC_SupDailyReportMiscModel() { Seq = -1 };

                        List<ECSupDailyDateVModel> constructionList = supDailyReportService.GetSupDailyDate<ECSupDailyDateVModel>(ECSupDailyReportService._Construction, eng.Seq, tarDate);
                        if (constructionList.Count > 0)
                        {//複製施工日誌
                            planItems = supDailyReportService.GetPlanOverview<EC_SupPlanOverviewModel>(constructionList[0].Seq);
                            foreach (EC_SupPlanOverviewModel m in planItems)
                            {
                                m.Seq = -1;
                            }
                        }
                        if (planItems == null || planItems.Count == 0)
                        {
                            planItems = supDailyReportService.GetPayitemList<EC_SupPlanOverviewModel>(eng.Seq, tarDate);
                            if (eng.SchCompDate.Value.Subtract(tarDate).Days < 0)
                            {//超出工程預定完工日期
                                foreach (EC_SupPlanOverviewModel m in planItems)
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
            return Json(new
            {
                result = 0,
                items = calList,
                admin = Utils.getUserInfo().IsAdmin
            }) ;
        }
        
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
        private string SaveFile(HttpPostedFileBase file, int prjXMLSeq, string fileHeader)
        {
            string filePath = Utils.GetTenderFolder(prjXMLSeq);
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
        private void DelFile(int prjXMLSeq, string fileName)
        {
            try
            {
                string filePath = Utils.GetTenderFolder(prjXMLSeq);
                if (Directory.Exists(filePath))
                {
                    if (fileName != null && fileName.Length > 0)
                    {
                        System.IO.File.Delete(Path.Combine(filePath, fileName));
                    }
                }
            } catch { }
        }
        public ActionResult DocDownload(int id, int mode, string fn)
        {
            return new CalUtils().DownloadFile(id, mode, fn);
        }
        private ActionResult DownloadFile(int id, string fn)
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
                    return File(iStream, "application/blob", "核定公文檔案."+ uniqueFileName.Substring(inx));
                }
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }
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
            return Json(iService.GetExtensionByPrjSeq<EPCReportExtensionVModel>(id));
        }*/
    }
}