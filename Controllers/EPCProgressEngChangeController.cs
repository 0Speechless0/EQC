using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EPCProgressEngChangeController : Controller
    {//進度管理 - 工程變更
        SchEngChangeService iService = new SchEngChangeService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }
        //刪除工程變更 s20230927
        public JsonResult DelEngChange(EPCProgressEngChangeListVModel ec)
        {
            if (iService.DelEngChange(ec))
            {
                return Json(new
                {
                    result = 0,
                    msg = "刪除成功"
                });
            } else
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程變更刪除失敗"
                });
            }
        }

        //停權廠商
        public JsonResult GetRejectCompany(int id)
        {
            List<RejectCompanyVModel> lists = iService.GetRejectCompany<RejectCompanyVModel>(id);
            return Json(new
            {
                result = 0,
                items = lists
            });
        }

        //單價分析清單WorkItem
        public JsonResult GetWList(int id)
        {
            return Json(new
            {
                result = 0,
                items = iService.GetWorkItemList(id)
            });
        }
        //新 WorkItem
        public JsonResult GetNewWItem(int id)
        {
            return Json(new
            {
                result = 0,
                item = new EC_SchEngProgressWorkItemModel() { Seq=-1, EC_SchEngProgressPayItemSeq=id}
            });
        }
        //更新 WorkItem
        public JsonResult UpdateWItem(EC_SchEngProgressWorkItemModel m)
        {
            if(iService.UpdateWorkItem(m)>0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "儲存完成"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }
        //刪除 WorkItem
        public JsonResult DelWItem(int id)
        {
            if (iService.DelWorkItem(id) == 0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "刪除完成"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "刪除失敗"
            });
        }

        //填報完成
        public JsonResult FillCompleted(int id)
        {
            List<EC_SchEngProgressHeaderModel> ecList = iService.GetEngChange<EC_SchEngProgressHeaderModel>(id);
            if(ecList.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程變更資料錯誤"
                });
            }
            EC_SchEngProgressHeaderModel engChange = ecList[0];
            if (engChange.SPState != 0)
            {
                return Json(new
                {
                    result = -2,
                    msg = "工程變更 非許可狀態, 請更新畫面"
                });
            }
            if(engChange.Version == 1)
            {
                return ExecEngChangeDate(engChange);
            } else
            {
                return ExecEngChangeDate2(engChange);
            }
            return Json(new
            {
                result = -1,
                msg = "更新失敗"
            });
        }
        //工程變更執行(初次)
        public JsonResult ExecEngChangeDate(EC_SchEngProgressHeaderModel engChange)
        {
            SchProgressPayItemService schProgressPayItemService = new SchProgressPayItemService();

            List<EPCSchProgressHeaderVModel> spHeaders = schProgressPayItemService.GetHeaderList<EPCSchProgressHeaderVModel>(engChange.EngMainSeq);
            if (spHeaders.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "預定進度資料錯誤[0]"
                });
            }
            EPCSchProgressHeaderVModel spHeader = spHeaders[0];
            if (spHeader.SPState == 1 || spHeader.EngChangeState == 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "預定進度非許可狀態"
                });
            }
            spHeader.EngChangeStartDate = engChange.StartDate;// .EngChangeStartDate;
            spHeader.EngChangeSchCompDate = engChange.SchCompDate;// item.ScheChangeCloseDate;*/

            //工程預定進度
            List<SchProgressHeaderHistoryProgressModel> engSchProgress = new List<SchProgressHeaderHistoryProgressModel>();
            if (spHeader.EngChangeCount == 0)
            {//首次工變才紀錄, 最初始每日工程預定進度
                List<EPCProgressRSchVModel> schProgress = new EPCProgressReportService().GetSchProgress<EPCProgressRSchVModel>(engChange.EngMainSeq);
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
            List<DateTime> dateList = DateReportCal(engChange.StartDate.Value, engChange.SchCompDate.Value);
            DateTime schProgressLastDate = engChange.StartDate.Value.AddDays(-1);
            List<SchProgressPayItemModel> ceList = new SchProgressPayItemService().GetPayItemSchProgress<SchProgressPayItemModel>(engChange.EngMainSeq, schProgressLastDate);
            //工程變更PayItem
            List<EC_SchEngProgressPayItemModel> sepPayItems = iService.GetList<EC_SchEngProgressPayItemModel>(engChange.Seq);
            if (sepPayItems.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程變更資料錯誤"
                });
            }
            int orderNo = 1;
            foreach (EC_SchEngProgressPayItemModel m in sepPayItems)
            {
                m.OrderNo = orderNo++;
            }

            int result = iService.EngChangeAddItems(spHeader, ceList, dateList, engSchProgress, engChange, sepPayItems);
            if (result == -1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程變更資料調整失敗"
                });
            } else if (result == -2) {//s20231014
                return Json(new
                {
                    result = -1,
                    msg = "工程會資料錯誤"
                });
            }
            if (engChange.ChangeType == SchEngChangeService._cyStopWork)
            {
                return Json(new
                {
                    result = 0,
                    msg = "工程變更-停工 完成."
                });
            } else if (engChange.ChangeType == SchEngChangeService._cyTerminateContract)
            {
                return Json(new
                {
                    result = 0,
                    msg = "工程變更-契約終止及解除 完成."
                });
            }
            return Json(new
            {
                result = 0,
                msg = "工程變更完成, 請進行預定進度設定"
            });
        }
        //工程變更執行
        public JsonResult ExecEngChangeDate2(EC_SchEngProgressHeaderModel engChange)
        {
            SchProgressPayItemService schProgressPayItemService = new SchProgressPayItemService();

            List<EPCSchProgressHeaderVModel> spHeaders = schProgressPayItemService.GetHeaderList<EPCSchProgressHeaderVModel>(engChange.EngMainSeq);
            if (spHeaders.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "預定進度資料錯誤[0]"
                });
            }
            EPCSchProgressHeaderVModel spHeader = spHeaders[0];
            if (spHeader.SPState == 1 || spHeader.EngChangeState == 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "預定進度非許可狀態"
                });
            }
            spHeader.EngChangeStartDate = engChange.StartDate;// .EngChangeStartDate;
            spHeader.EngChangeSchCompDate = engChange.SchCompDate;// item.ScheChangeCloseDate;*/

            //工程預定進度
            //List<SchProgressHeaderHistoryProgressModel> engSchProgress = new List<SchProgressHeaderHistoryProgressModel>();


            //計算需填寫的日期
            List<DateTime> dateList = DateReportCal(engChange.StartDate.Value, engChange.SchCompDate.Value);
            DateTime schProgressLastDate = engChange.StartDate.Value.AddDays(-1);
            List<EC_SchProgressPayItemModel> ceList = iService.GetPayItemSchProgress<EC_SchProgressPayItemModel>(engChange.EngMainSeq, schProgressLastDate);
            //工程變更PayItem
            List<EC_SchEngProgressPayItemModel> sepPayItems = iService.GetList<EC_SchEngProgressPayItemModel>(engChange.Seq);
            if (sepPayItems.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程變更資料錯誤"
                });
            }
            /*return Json(new
            {
                result = -1,
                msg = "待調整..."
            });*/
            int orderNo = 1;
            foreach (EC_SchEngProgressPayItemModel m in sepPayItems)
            {
                m.OrderNo = orderNo++;
            }
            int result = iService.EngChangeAddItems2(spHeader, ceList, dateList, engChange, sepPayItems);
            if (result == -1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程變更資料調整失敗"
                });
            } else if (result == -2)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程會資料錯誤"
                });
            }
            if (engChange.ChangeType== SchEngChangeService._cyStopWork)
            {
                return Json(new
                {
                    result = 0,
                    msg = "工程變更-停工 完成."
                });
            }
            else if (engChange.ChangeType == SchEngChangeService._cyTerminateContract)
            {
                return Json(new
                {
                    result = 0,
                    msg = "工程變更-契約終止及解除 完成."
                });
            }
            return Json(new
            {
                result = 0,
                msg = "工程變更完成, 請進行預定進度設定"
            });
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


        //新增 PayItem 資料
        public JsonResult AddRecord(int id, EC_SchEngProgressPayItemModel m)
        {
            if (iService.AddPayItem(id, m))
            {
                return Json(new
                {
                    result = 0,
                    msg = "新增完成"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "新增失敗"
            });
        }
        //更新 PayItem 資料
        public JsonResult UpdateRecord(EC_SchEngProgressPayItemModel m)
        {
            if (iService.UpdatePayItem(m) > 0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "更新完成"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "更新失敗"
            });
        }
        //刪除 PayItem 資料
        public JsonResult DelRecord(int id)
        {
            if (iService.DelRecord(id) == 0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "刪除完成"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "刪除失敗"
            });
        }
        //刪除 PayItem 資料 s20230530
        public JsonResult DelRecords(List<int> ids)
        {
            if (iService.DelRecords(ids) == 0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "刪除完成"
                });
            }
            return Json(new
            {
                result = -1,
                msg = "刪除失敗"
            });
        }
        //更新工程變更日期
        public JsonResult UpdateEngDate(EPCProgressEngChangeListVModel eng, int id, int mode)
        {
            eng.updateDate();
            if (!eng.StartDate.HasValue || !eng.SchCompDate.HasValue)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程變更日期資料錯誤"
                });
            }
            if (eng.SchCompDate.Value.Subtract(eng.StartDate.Value).Days <= 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程變更日期範圍錯誤"
                });
            }
            List<EPCProgressEngChangeListVModel> list = iService.GetEngChangeList<EPCProgressEngChangeListVModel>(eng.Seq);

            if (list.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "查無工程變更資料"
                });
            }
            else
            {
                foreach (EPCProgressEngChangeListVModel m in list)
                {
                    if (m.Seq == id)
                    {
                        eng.SupDailyReportExtensionSeq = m.SupDailyReportExtensionSeq;
                        eng.SupDailyReportWorkSeq = m.SupDailyReportWorkSeq;
                    } else
                    {
                        if (eng.StartDate.Value.Subtract(m.StartDate.Value).Days <= 0)
                        {
                            return Json(new
                            {
                                result = -1,
                                msg = "工程變更起始日期錯誤, 不得小於先前變更起始日期"
                            });
                        }
                        if (eng.StartDate.Value.Subtract(m.StartDate.Value).Days <= 0)
                        {
                            return Json(new
                            {
                                result = -1,
                                msg = "工程變更起始日期錯誤, 不得大於於先前變更的預定完工日期"
                            });
                        }
                    }
                    
                }
            }
            
            List<EPCTendeVModel> tenders = new EPCProgressManageService().GetEngLinkTenderBySeq<EPCTendeVModel>(eng.Seq);
            if (tenders.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }
            else if (eng.StartDate.Value.Subtract(tenders[0].StartDate.Value).Days <= 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程變更起始日期錯誤, 不得小於開工日期"
                });
            }

            int seq = iService.UpdateEngDate(eng, id, mode);
            if (seq > 0)
            {
                return Json(new
                {
                    result = 0,
                    msg = "更新完成"
                });
            }
            else if (seq == -2)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程會資料異常, 更新失敗"
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
        //新增工程變更
        public JsonResult AddEngChange(EPCProgressEngChangeListVModel eng)
        {
            eng.updateDate();
            if (!eng.StartDate.HasValue || !eng.SchCompDate.HasValue)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程變更日期資料錯誤"
                });
            }
            if (eng.SchCompDate.Value.Subtract(eng.StartDate.Value).Days <= 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程變更日期範圍錯誤"
                });
            }
            int version = 0;
            List<EPCProgressEngChangeListVModel> list = iService.GetEngChangeList<EPCProgressEngChangeListVModel>(eng.Seq);
            if (list.Count > 0)
            {
                foreach (EPCProgressEngChangeListVModel m in list)
                {
                    if (m.SPState != 1)
                    {
                        return Json(new
                        {
                            result = -1,
                            msg = "已有工程變更作業中, 無法再新增"
                        });
                    }
                    if (eng.StartDate.Value.Subtract(m.StartDate.Value).Days <= 0)
                    {
                        return Json(new
                        {
                            result = -1,
                            msg = "工程變更起始日期錯誤, 不得小於先前變更起始日期"
                        });
                    }
                    if (eng.StartDate.Value.Subtract(m.StartDate.Value).Days <= 0)
                    {
                        return Json(new
                        {
                            result = -1,
                            msg = "工程變更起始日期錯誤, 不得大於於先前變更的預定完工日期"
                        });
                    }
                    if (m.Version > version) version = m.Version;
                }
            }
            else
            {
                List<EPCTendeVModel> tenders = new EPCProgressManageService().GetEngLinkTenderBySeq<EPCTendeVModel>(eng.Seq);
                if(tenders.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "工程資料錯誤"
                    });
                } else if (eng.StartDate.Value.Subtract(tenders[0].StartDate.Value).Days <= 0) {
                    return Json(new
                    {
                        result = -1,
                        msg = "工程變更起始日期錯誤, 不得小於開工日期"
                    });
                }
            }

            eng.Version = version + 1;
            int seq = iService.AddEngChange(eng);
            if (seq > 0)
            {
                return Json(new
                {
                    result = 0,
                    id = seq,
                    msg = "新增完成"
                });
            } else if(seq == -2)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程會資料異常, 新增失敗"
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
        //新增工程變更-延展工期 s20230523
        public JsonResult AddEngChangeByExtension(EPCProgressEngChangeListVModel eng, SupDailyReportExtensionVModel extension)
        {
            eng.updateDate();
            extension.updateDate();
            if (!eng.StartDate.HasValue || !eng.SchCompDate.HasValue)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程變更日期資料錯誤"
                });
            }
            if (eng.SchCompDate.Value.Subtract(eng.StartDate.Value).Days <= 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程變更日期範圍錯誤"
                });
            }
            int version = 0;
            List<EPCProgressEngChangeListVModel> list = iService.GetEngChangeList<EPCProgressEngChangeListVModel>(eng.Seq);
            if (list.Count > 0)
            {
                foreach (EPCProgressEngChangeListVModel m in list)
                {
                    if (m.SPState != 1)
                    {
                        return Json(new
                        {
                            result = -1,
                            msg = "已有工程變更作業中, 無法再新增"
                        });
                    }
                    if (eng.StartDate.Value.Subtract(m.StartDate.Value).Days <= 0)
                    {
                        return Json(new
                        {
                            result = -1,
                            msg = "工程變更起始日期錯誤, 不得小於先前變更起始日期"
                        });
                    }
                    if (eng.StartDate.Value.Subtract(m.StartDate.Value).Days <= 0)
                    {
                        return Json(new
                        {
                            result = -1,
                            msg = "工程變更起始日期錯誤, 不得大於於先前變更的預定完工日期"
                        });
                    }
                    if (m.Version > version) version = m.Version;
                }
            }
            else
            {
                List<EPCTendeVModel> tenders = new EPCProgressManageService().GetEngLinkTenderBySeq<EPCTendeVModel>(eng.Seq);
                if (tenders.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "工程資料錯誤"
                    });
                }
                else if (eng.StartDate.Value.Subtract(tenders[0].StartDate.Value).Days <= 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "工程變更起始日期錯誤, 不得小於開工日期"
                    });
                }
            }

            eng.Version = version + 1;
            int seq = iService.AddEngChange(eng, extension);
            if (seq > 0)
            {
                return Json(new
                {
                    result = 0,
                    id = seq,
                    msg = "新增完成"
                });
            }
            else if (seq == -2)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程會資料異常, 新增失敗"
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
        //新增工程變更-設定停工 s20230525 
        public JsonResult AddEngChangeByWorkStop(string engJSON, SupDailyReportWorkVModel work)
        {
            work.updateDate();
            EPCProgressEngChangeListVModel eng = JsonConvert.DeserializeObject<EPCProgressEngChangeListVModel>(engJSON);
            eng.updateDate();
            if (!eng.StartDate.HasValue || !eng.SchCompDate.HasValue)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程變更日期資料錯誤"
                });
            }
            if (eng.SchCompDate.Value.Subtract(eng.StartDate.Value).Days <= 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程變更日期範圍錯誤"
                });
            }
            int version = 0;
            List<EPCProgressEngChangeListVModel> list = iService.GetEngChangeList<EPCProgressEngChangeListVModel>(eng.Seq);
            if (list.Count > 0)
            {
                foreach (EPCProgressEngChangeListVModel m in list)
                {
                    if (m.SPState != 1)
                    {
                        return Json(new
                        {
                            result = -1,
                            msg = "已有工程變更作業中, 無法再新增"
                        });
                    }
                    if (eng.StartDate.Value.Subtract(m.StartDate.Value).Days <= 0)
                    {
                        return Json(new
                        {
                            result = -1,
                            msg = "工程變更起始日期錯誤, 不得小於先前變更起始日期"
                        });
                    }
                    if (eng.StartDate.Value.Subtract(m.StartDate.Value).Days <= 0)
                    {
                        return Json(new
                        {
                            result = -1,
                            msg = "工程變更起始日期錯誤, 不得大於於先前變更的預定完工日期"
                        });
                    }
                    if (m.Version > version) version = m.Version;
                }
            }
            else
            {
                List<EPCTendeVModel> tenders = new EPCProgressManageService().GetEngLinkTenderBySeq<EPCTendeVModel>(eng.Seq);
                if (tenders.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "工程資料錯誤"
                    });
                }
                else if (eng.StartDate.Value.Subtract(tenders[0].StartDate.Value).Days <= 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "工程變更起始日期錯誤, 不得小於開工日期"
                    });
                }
            }
            if(ReportWorkFile(work)<0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "上傳檔案失敗"
                });
            }

            eng.Version = version + 1;
            int seq = iService.AddEngChange(eng, work);
            if (seq > 0)
            {
                return Json(new
                {
                    result = 0,
                    id = seq,
                    msg = "新增完成"
                });
            }
            else if (seq == -2)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程會資料異常, 新增失敗"
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
        private int ReportWorkFile(SupDailyReportWorkModel item)
        {
            if (Request.Files.Count == 0) return 0;

            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];
                int mode = -1;
                if (fileName == "StopWorkFile") mode = 1;
                else if (fileName == "BackWorkFile") mode = 2;
                if (mode > 0)
                {
                    if (file.ContentLength > 0)
                    {
                        try
                        {
                            if (mode == 1)
                            {
                                item.StopWorkApprovalFile = SaveFile(file, item.EngMainSeq, "ApproveStop-");
                            }
                            else if (mode == 2)
                            {
                                item.BackWorkApprovalFile = SaveFile(file, item.EngMainSeq, "ApproveBack-");
                            }
                        }
                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                            return -1;
                        }
                    }
                }
            }
            return 0;
            /*if (item.Seq == -1)
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
            }
            else
            {
                List<SupDailyReportWorkModel> workItems = iService.GetWorkBySeq<SupDailyReportWorkModel>(item.Seq);
                if (workItems.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "儲存失敗"
                    });
                }
                SupDailyReportWorkModel workItem = workItems[0];
                if (!stopWorkApprovalFile) item.StopWorkApprovalFile = workItem.StopWorkApprovalFile;
                if (!backWorkApprovalFile) item.BackWorkApprovalFile = workItem.BackWorkApprovalFile;

                if (iService.WorkUpdate(item))
                {
                    if (stopWorkApprovalFile) DelFile(workItem.EngMainSeq, workItem.StopWorkApprovalFile);
                    if (backWorkApprovalFile) DelFile(workItem.EngMainSeq, workItem.BackWorkApprovalFile);
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
            }*/
        }
        private string SaveFile(HttpPostedFileBase file, int engMainSeq, string fileHeader)
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
                string filePath = Utils.GetEngMainFolder(engMainSeq);
                if (Directory.Exists(filePath))
                {
                    if (fileName != null && fileName.Length > 0)
                    {
                        System.IO.File.Delete(Path.Combine(filePath, fileName));
                    }
                }
            }
            catch { }
        }

        //檢查工程變更清單
        public JsonResult CheckECState(int id)
        {
            List<EPCProgressEngChangeListVModel> list = iService.GetEngChangeList<EPCProgressEngChangeListVModel>(id);
            bool addFlag = true;
            foreach (EPCProgressEngChangeListVModel m in list)
            {
                if (m.SPState != 1)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "已有工程變更作業中, 無法再新增"
                    });
                    break;
                }
            }
            return Json(new
            {
                result = 0
            });
        }
        //工程變更履歷清單
        public JsonResult GetECList(int id)
        {
            List<EPCProgressEngChangeListVModel> list = iService.GetEngChangeList<EPCProgressEngChangeListVModel>(id);
            bool addFlag = iService.CheckEngState(id);//進度作業已經點選"填報完成"，且監造報表已有資料
            foreach (EPCProgressEngChangeListVModel m in list)
            {
                if(m.SPState != 1)
                {
                    addFlag = false;
                    break;
                }
            }

            return Json(new
            {
                result = 0,
                addFlag = addFlag,
                items = list
            });
        }
        //工程 Payitem
        public JsonResult GetList(int id, int eng, int perPage, int pageIndex)
        {
            int resultCode = 0;
            int total = iService.GetListTotal(id);
            if (total == 0)
            {//初始化資料
                List<EPCProgressEngChangeListVModel> eclists = iService.GetEngChangeList<EPCProgressEngChangeListVModel>(eng);
                if (eclists.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "工程變更主檔資料不存在"
                    });
                }
                EPCProgressEngChangeListVModel engChange = eclists[eclists.Count - 1];
                if (engChange.Seq==id && engChange.SPState == 0)
                {
                    if (engChange.Version == 1)
                    {
                        SchEngProgressService schEngProgressService = new SchEngProgressService();
                        List<SchEngProgressPayItem2Model> sepList = schEngProgressService.GetPayItemList<SchEngProgressPayItem2Model>(eng);
                        if (sepList.Count == 0)
                        {
                            return Json(new
                            {
                                result = -1,
                                msg = "進度管理-前置作業 無資料"
                            });
                        }
                        schEngProgressService.GetWorkItemList(sepList);
                        if (!iService.InitFromSchEngProgress(id, sepList))
                        {
                            return Json(new
                            {
                                result = -1,
                                msg = "資料初始化失敗"
                            });
                        }
                    }
                    else
                    {
                        List<EC_SchEngProgressPayItem2Model> verList = iService.GetListByVer<EC_SchEngProgressPayItem2Model>(engChange.EngMainSeq, engChange.Version-1);
                        if (verList.Count == 0)
                        {
                            return Json(new
                            {
                                result = -1,
                                msg = "無法取得前次工程變更資料"
                            });
                        }
                        iService.GetWorkItemListFromPayItem(verList);
                        if (!iService.InitFromEngChange(id, verList))
                        {
                            return Json(new
                            {
                                result = -1,
                                msg = "資料初始化失敗"
                            });
                        }
                    }
                    total = iService.GetListTotal(id);
                }
            }
            List<EC_SchEngProgressPayItemModel> ceList = iService.GetList<EC_SchEngProgressPayItemModel>(id, perPage, pageIndex);

            //s20230528 碳排量計算
            List<EngMainEditVModel> items = new EngMainService().GetItemBySeq<EngMainEditVModel>(eng);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }
            EngMainEditVModel engMain = items[0];
            decimal? co2Total = null;
            decimal? co2ItemTotal = null;
            decimal? dismantlingRate = null;
            //s20230528
            decimal? co2TotalDesign = null;
            decimal? greenFunding = null;
            decimal? greenFundingRate = null;
            new CarbonEmissionPayItemService().CalCarbonTotal(eng, ref co2TotalDesign, ref co2ItemTotal, ref greenFunding);

            co2Total = null;
            co2ItemTotal = null;
            dismantlingRate = null;
            greenFunding = null;
            greenFundingRate = null;
            if (total > 0)
            {
                iService.CalCarbonTotal(eng, ref co2Total, ref co2ItemTotal, ref greenFunding);
                if (engMain.SubContractingBudget.HasValue && engMain.SubContractingBudget.Value > 0)
                {
                    dismantlingRate = Math.Round(co2ItemTotal.Value * 100 / engMain.SubContractingBudget.Value);
                    greenFundingRate = Math.Round(greenFunding.Value * 100 / engMain.SubContractingBudget.Value);
                }
            }
            return Json(new
            {
                result = resultCode,
                totalRows = total,
                items = ceList,
                co2Total = co2Total,
                dismantlingRate = dismantlingRate,
                co2TotalDesign = co2TotalDesign,
                greenFunding = greenFunding,
                greenFundingRate = greenFundingRate,
            });
        }
        //下載工程範本(excel)
        public ActionResult DnSchProgress(int id)
        {
            List<EC_SchEngProgressHeaderModel> engChanges = iService.GetEngChange<EC_SchEngProgressHeaderModel>(id);
            if (engChanges.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程變更資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EC_SchEngProgressHeaderModel engChangeHeader = engChanges[0];

            List<EngMainEditVModel> items = new EngMainService().GetItemBySeq<EngMainEditVModel>(engChangeHeader.EngMainSeq);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EngMainEditVModel eng = items[0];

            List<EPCSchProgressVModel> dateList = iService.GetDateList<EPCSchProgressVModel>(id);
            if (dateList.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "查無日期資料"
                }, JsonRequestBehavior.AllowGet);
            }
            List<EC_SchProgressPayItem1Model> spList;
            if(engChangeHeader.Version == 1)
                spList = iService.GetSchProgressPayItemsAndDayBefore<EC_SchProgressPayItem1Model>(engChangeHeader.Seq, dateList[0].ItemDate);
            else
                spList = iService.GetSchProgressPayItemsAndDayBefore2<EC_SchProgressPayItem1Model>(engChangeHeader.Seq, dateList[0].ItemDate);
            if (spList.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "查無資料"
                }, JsonRequestBehavior.AllowGet);
            }
            return CreateExcel(eng, spList, dateList, engChangeHeader);
        }
        private ActionResult CreateExcel(EngMainEditVModel eng, List<EC_SchProgressPayItem1Model> spList, List<EPCSchProgressVModel> dateList, EC_SchEngProgressHeaderModel engChangeHeader)
        {
            string filename = Utils.CopyTemplateFile("進度管理-工程範本-工變.xlsx", ".xlsx");
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
                DateSheet(sheet, eng, dateList, engChangeHeader, spList);
                sheet.Protect(String.Format("{0}-{1}", eng.EngNo, eng.Seq));//shioulo20221213

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
                    message = "Excel 製表失敗\n" + e.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        //日期,預計數量
        private void DateSheet(Worksheet sheet, EngMainEditVModel eng, List<EPCSchProgressVModel> dateList, EC_SchEngProgressHeaderModel engChangeHeader, List<EC_SchProgressPayItem1Model> spList)
        {
            if (sheet == null) return;

            sheet.Cells[1, 8] = engChangeHeader.Seq;
            
            sheet.Cells[4, 9].NumberFormat = "M/d";
            sheet.Cells[4, 9].Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            sheet.Cells[4, 9].VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
            sheet.Cells[4, 9] = engChangeHeader.StartDate.Value.AddDays(-1).ToString("M/d");

            int col = 10;
            foreach (EPCSchProgressVModel m in dateList)
            {
                int row = 4;
                sheet.Cells[row, col].NumberFormat = "M/d";
                sheet.Cells[row, col].Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                sheet.Cells[row, col].VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                sheet.Cells[row, col] = m.ItemDate;
                row++;
                sheet.Cells[row, col] = "累計預定進度%";
                sheet.Cells[row, col].WrapText = true;
                List<EC_SchProgressPayItemModel> payitems = iService.GetSchProgressPayItemList<EC_SchProgressPayItemModel>(engChangeHeader.Seq, m.ItemDate);
                int inx = 0;
                foreach (EC_SchProgressPayItemModel p in payitems)
                {
                    EC_SchProgressPayItem1Model dayBeforeItem = null;
                    foreach (EC_SchProgressPayItem1Model item in spList)
                    {
                        if(item.EC_SchEngProgressPayItemSeq == p.EC_SchEngProgressPayItemSeq)
                        {
                            dayBeforeItem = item;
                            break;
                        }
                    }

                    row++;
                    if (dayBeforeItem == null)
                        sheet.Cells[row, col] = p.SchProgress;
                    else
                    {
                        if (dayBeforeItem.SchProgressDayBefore == -1)
                        {
                            sheet.Cells[row, col] = -1;
                            sheet.Cells[row, col].Locked = true;
                            sheet.Cells[row, col].Interior.Color = sheet.Cells[1, 1].Interior.Color;
                        } else if (dayBeforeItem.SchProgressDayBefore == 100)
                            sheet.Cells[row, col] = 100;
                        else
                            sheet.Cells[row, col] = p.SchProgress;
                    }
                    sheet.Cells[row, col].Style.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                }
                col++;
            }
        }
        //payitem
        private void PayitemSheet(Worksheet sheet, EngMainEditVModel eng, List<EC_SchProgressPayItem1Model> spList)
        {
            if (sheet == null) return;

            sheet.Cells[2, 3] = eng.EngName;// "@工程名稱";
            sheet.Cells[3, 3] = eng.EngNo;// "@標案編號";

            int row = 6;
            foreach (EC_SchProgressPayItem1Model m in spList)
            {
                sheet.Cells[row, 1] = m.OrderNo;
                sheet.Cells[row, 2] = m.PayItem.Trim();// === 會與 Excel 通用格式衝突";
                sheet.Cells[row, 3] = m.Description;// 項目及說明
                sheet.Cells[row, 4] = m.Unit;
                sheet.Cells[row, 5] = m.Quantity;
                sheet.Cells[row, 6] = m.Price;
                sheet.Cells[row, 7] = m.Amount;
                sheet.Cells[row, 8] = m.EC_SchEngProgressPayItemSeq;
                sheet.Cells[row, 9] = m.SchProgressDayBefore.HasValue ? m.SchProgressDayBefore.Value : 0;
                row++;
            }
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