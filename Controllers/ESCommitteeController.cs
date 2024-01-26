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
    public class ESCommitteeController : Controller
    {//工程督導 - 委員督導(工程督導)
        SuperviseCommitteeService iServce = new SuperviseCommitteeService();
        public ActionResult Index()
        {
            Utils.setUserClass(this, 1);
            return View("Index");
        }

        //抽查項目 編輯
        public ActionResult ViewChapterPhoto(int id)
        {
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ViewCP", "ESCommittee");
            string tarUrl = redirectUrl.ToString() + "?id=" + id;
            return Json(new { Url = tarUrl });
        }
        public ActionResult ViewCP(int id)
        {
            Utils.setUserClass(this, 1);
            return View("ViewCP");
        }

        //抽查紀錄編輯
        public ActionResult SamplingInspectionRecEdit(int id)
        {
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("SIREdit", "ESCommittee");
            string tarUrl = redirectUrl.ToString() + "?id=" + id;
            return Json(new { Url = tarUrl });
        }
        public ActionResult SIREdit(int id)
        {
            Utils.setUserClass(this, 1);
            return View("SIREdit");
        }

        //工程設計圖
        public JsonResult GetChapterPhoto(int id)
        {
            List<ESCommitteePhotoVModel> photoItems = iServce.GetChapterPhotos<ESCommitteePhotoVModel>(id);
            return Json(new
            {
                result = 0,
                items = photoItems,
                rPath = Utils.GetEngMainFolder(id).Replace(System.Web.HttpContext.Current.Server.MapPath("~"), "/")
            });
        }
        //
        public JsonResult SavePhoto(ESCommitteePhotoVModel p)
        {
            if(iServce.UpdateCheckPhoto(p))
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
        //監造日誌下載
        public ActionResult DownloadSDaily(int id, string tarDate)
        {
            SupDailyReportService supDailyReportService = new SupDailyReportService();
            List<EPCSupDailyDateVModel> dateList = supDailyReportService.GetSupDailyDate<EPCSupDailyDateVModel>(SupDailyReportService._Supervise, id, DateTime.Parse(tarDate));
            if (dateList.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "今日查無資料"
                }, JsonRequestBehavior.AllowGet);
            }
            EPCSupDailyDateVModel supDailyItem = dateList[0];
            return new EPCProgressManageController().DownloadSDailyDoc(supDailyItem.Seq, 2);
        }
        //施工日誌下載
        public ActionResult DownloadCDaily(int id, string tarDate)
        {
            SupDailyReportService supDailyReportService = new SupDailyReportService();
            List<EPCSupDailyDateVModel> dateList = supDailyReportService.GetSupDailyDate<EPCSupDailyDateVModel>(SupDailyReportService._Construction, id, DateTime.Parse(tarDate));
            if (dateList.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "今日查無資料"
                }, JsonRequestBehavior.AllowGet);
            }
            EPCSupDailyDateVModel supDailyItem = dateList[0];
            return new EPCProgressManageController().DownloadCDailyDoc(supDailyItem.Seq, 2);
        }
        //工程標案 by EngMain.Seq
        public JsonResult GetTender(int id)
        {
            List<EPCTendeVModel> tender = iServce.GetEngLinkTenderBySeq<EPCTendeVModel>(id);
            //
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
                List<ESCommitteeCheckStatisticsVModel> constructionChecks = iServce.ConstructionCheck<ESCommitteeCheckStatisticsVModel>(id);
                List<ESCommitteeCheckStatisticsVModel> otherChecks = iServce.OtherCheck<ESCommitteeCheckStatisticsVModel>(id);
                List<ESCommitteePhotoVModel> photoItems = iServce.GetCheckPhotos<ESCommitteePhotoVModel>(id);

                return Json(new
                {
                    result = 0,
                    item = tender[0],
                    cItems = constructionChecks,
                    oItems = otherChecks,
                    pItems = photoItems,
                    date = DateTime.Now.ToString("yyyy-M-d"),
                    rPath = Utils.GetEngMainFolder(id).Replace(System.Web.HttpContext.Current.Server.MapPath("~"), "/")
                });
            }
        }
        //標案
        public JsonResult GetTenderOptions(string date, string unit)
        {
            List<SelectIntOptionModel> options = iServce.GetTenderOptions(date, unit);
            return Json(options);
        }
        //單位
        public JsonResult GetUnitOptions(string date)
        {
            List<SelectOptionModel> options = iServce.GetExecUnitOptions(date);
            return Json(options);
        }
        //督導日期
        public JsonResult GetSuperviseDateOptions()
        {
            List<SelectOptionModel> options = iServce.GetSuperviseDateOptions();
            return Json(options);
        }
        public JsonResult GetEngItem(int id)
        {
            List<ESCommitteeTenderVModel> tender = iServce.GetEngLinkTenderBySeq<ESCommitteeTenderVModel>(id);
            if (tender.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                });

            }
            return Json(new
            {
                result = 0,
                item = tender[0],
            });
        }

        /*public ActionResult EditEng(int seq)
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

        //期別查詢
        public JsonResult SearchPhase(string keyWord)
        {
            List<SupervisePhaseModel> list = iServce.GetPhaseCode(keyWord);
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
            int total = iServce.GetPhaseEngList1Count(id);

            if (total > 0)
            {
                engList = iServce.GetPhaseEngList1<SuperviseEng1VModel>(id, pageRecordCount, pageIndex);
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
            List<SuperviseEngSuperviseFillVModel> engList = iServce.GetEngForSuperviseFill<SuperviseEngSuperviseFillVModel>(id);
            
            if (engList.Count == 1)
            {
                SuperviseEngSuperviseFillVModel m = engList[0];
                m.committees = iServce.GetEngCommittes<SuperviseFillCommitteeVModel>(m.Seq);
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
            List<SuperviseFillVModel> lists = iServce.GetRecords<SuperviseFillVModel>(id);

            decimal q = 60, w = 20, p = 20;
            foreach(SuperviseFillVModel item in lists)
            {
                item.committeeList = iServce.GetRecordCommittes(item.SuperviseEngSeq.Value, item.Seq);
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
            if(m.Seq == -1)
                state = iServce.AddRecords(m);
            else
                state = iServce.UpdateRecords(m);
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
            int state = iServce.DelRecord(id);
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
        }*/
    }
}