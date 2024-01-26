using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using EQC.ViewModel.Common;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class ERProposalReviewController : Controller
    {//工程提報 - 提案審查

        EngReportService engReportService = new EngReportService();

        public ActionResult Index()
        {
            Utils.setUserClass(this, 1);
            return View();
        }

        //工程清單A - 全部案件清單
        public JsonResult GetListA(int year, int unit, int subUnit, int rptType, int pageRecordCount, int pageIndex)
        {
            List<EngReportVModel> engList = new List<EngReportVModel>();
            int total = engReportService.GetEngListCount(year, unit, subUnit, rptType, 3);
            if (total > 0)
            {
                engList = engReportService.GetEngList<EngReportVModel>(year, unit, subUnit, rptType, pageRecordCount, pageIndex, 3);
            }
            return Json(new
            {
                pTotal = total,
                items = engList
            });
        }

        //工程清單B - 提案審查清單
        public JsonResult GetListB(int year, int unit, int subUnit, int rptType, int pageRecordCount, int pageIndex)
        {
            List<EngReportVModel> engList = new List<EngReportVModel>();
            int total = engReportService.GetEngListCount(year, unit, subUnit, 0, 12);
            if (total > 0)
            {
                engList = engReportService.GetEngList<EngReportVModel>(year, unit, subUnit, 0, pageRecordCount, pageIndex, 12);
                int iCount = 0;
                foreach (EngReportVModel vm in engList)
                {
                    engList[iCount].IsShow = true;

                    iCount++;
                }
            }
            return Json(new
            {
                pTotal = total,
                items = engList
            });
        }

        //工程清單C - 核可清單
        public JsonResult GetListC(int year, int unit, int subUnit, int rptType, int pageRecordCount, int pageIndex)
        {
            List<EngReportVModel> engList = new List<EngReportVModel>();
            int total = engReportService.GetEngListCount(year, unit, subUnit, 5, 0);
            if (total > 0)
            {
                engList = engReportService.GetEngList<EngReportVModel>(year, unit, subUnit, 5, pageRecordCount, pageIndex, 0);
                int iCount = 0;
                foreach (EngReportVModel vm in engList)
                {
                    engList[iCount].IsShow = true;
                    engList[iCount].IsCheck = false;

                    iCount++;
                }
            }
            return Json(new
            {
                pTotal = total,
                items = engList
            });
        }

        //工程清單D - 暫緩清單
        public JsonResult GetListD(int year, int unit, int subUnit, int rptType, int pageRecordCount, int pageIndex)
        {
            List<EngReportVModel> engList = new List<EngReportVModel>();
            int total = engReportService.GetEngListCount(year, unit, subUnit, 6, 0);
            if (total > 0)
            {
                engList = engReportService.GetEngList<EngReportVModel>(year, unit, subUnit, 6, pageRecordCount, pageIndex, 0);
                int iCount = 0;
                foreach (EngReportVModel vm in engList)
                {
                    engList[iCount].IsShow = true;

                    iCount++;
                }
            }
            return Json(new
            {
                pTotal = total,
                items = engList
            });
        }

        public ActionResult UpdateEngReportSort(List<EngReportVModel> list) 
        {
            try 
            {
                int state = -1;
                foreach (var item in list)
                {
                    if(item.IsCheck) 
                    {
                        state = engReportService.UpdateEngReportForReviewSort(item);
                        if (state < 0)
                        {
                            return Json(new
                            {
                                result = -1,
                                msg = "儲存失敗"
                            });
                        }
                    }
                }
            }
            catch(Exception ex) 
            {
                return Json(new
                {
                    result = -1,
                    msg = "儲存失敗"
                });
            }

            return Json(new
            {
                result = 0,
                msg = "儲存成功"
            });
        }

        public ActionResult EditEng(int seq)
        {
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Edit", "ERProposalReview");
            string tarUrl = redirectUrl.ToString() + "?id=" + seq;
            return Json(new { Url = tarUrl });
        }

        public ActionResult Edit()
        {
            Utils.setUserClass(this, 2);
            //ViewBag.Title = "提案填報及審查-提案審查";
            List<VMenu> menu = new List<VMenu>();
            var link = new UrlHelper(Request.RequestContext).Action("Index", "ERProposalReview");
            menu.Add(new VMenu() { Name = "提案審查", Url = link.ToString() });
            menu.Add(new VMenu() { Name = "提案審查", Url = "" });
            ViewBag.breadcrumb = menu;
            return View("Edit");
        }

        //取得單筆工程
        public virtual JsonResult GetEngReport(int id)
        {
            List<EngReportVModel> items = engReportService.GetEegReportBySeq<EngReportVModel>(id);
            if (items.Count == 1)
            {
                //items[0].OriginAndScopeTWDT = Utils.ChsDate(items[0].OriginAndScopeUpdateTime);
                //items[0].RelatedReportResultsTWDT = Utils.ChsDate(items[0].RelatedReportResultsUpdateTime);
                //items[0].FacilityManagementTWDT = Utils.ChsDate(items[0].FacilityManagementUpdateTime);
                //items[0].ProposalScopeLandTWDT = Utils.ChsDate(items[0].ProposalScopeLandUpdateTime);

                items[0].HistoricalCatastrophe = items[0].HistoricalCatastrophe == null ? 2 : items[0].HistoricalCatastrophe;

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
                    result = 2,
                    msg = "讀取資料錯誤"
                });
            }
        }

        //更新工程 FOR 提案審查
        public ActionResult UpdateTempEngReport(EngReportVModel m)
        {
            
            if (!String.IsNullOrEmpty(m.RptYear.ToString()) && !String.IsNullOrEmpty(m.RptName))
            {
                if (Convert.ToInt16(m.ProposalReviewTypeSeq) >= 2 && !String.IsNullOrEmpty(m.DrainName)) 
                {
                    List<SelectVM> items = engReportService.GetDrainList();
                    var item = items.Where(p => p.Text == m.DrainName)
                        .Select(row => new
                        {
                            DrainSeq = row.Value

                        }).FirstOrDefault();
                    m.DrainSeq = Convert.ToInt32(item.DrainSeq);
                }

                if (m.RptTypeSeq == 2) 
                {
                    //提案審查填報
                    m.RptTypeSeq = 4;
                }

                int state;
                state = engReportService.UpdateEngReportForPR(m);
                if (state == 0)
                {
                    if(m.ProposalAuditOpinion!=null)
                        engReportService.UpdateEngReportForPRProposalAudit(m);

                    return Json(new
                    {
                        result = 0,
                        msg = "儲存成功"
                    });
                }
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                });
            }

            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }
        
        //更新工程 FOR 提案審查
        public ActionResult UpdateEngReport(EngReportVModel m, int isSend)
        {
            if (String.IsNullOrEmpty(m.RptName)  //工程名稱
                || m.ProposalReviewTypeSeq == null  //類別
                || m.ProposalReviewAttributesSeq == null  //屬性
                || (Convert.ToInt16(m.ProposalReviewTypeSeq) == 1 && !m.RiverSeq3.HasValue)  //河川
                || (Convert.ToInt16(m.ProposalReviewTypeSeq) == 2 && String.IsNullOrEmpty(m.DrainName))  //排水
                || (Convert.ToInt16(m.ProposalReviewTypeSeq) == 3 && String.IsNullOrEmpty(m.Coastal))  //海岸
                || (Convert.ToInt16(m.ProposalReviewTypeSeq) > 3 && !m.RiverSeq3.HasValue && String.IsNullOrEmpty(m.DrainName))  //河川及排水
                || String.IsNullOrEmpty(m.LargeSectionChainage)  //大斷面樁號 
                || !m.CitySeq.HasValue  //所在行政區域
                || !m.TownSeq.HasValue  //所在行政區域
                || !m.CoordX.HasValue   //座標(X)
                || !m.CoordY.HasValue   //座標(Y)
                //|| String.IsNullOrEmpty(m.EngineeringScale)  //工程規模
                || String.IsNullOrEmpty(m.ProcessReason)  //辦理緣由
                //|| String.IsNullOrEmpty(m.EngineeringScaleMemo)  //工程規模說明
                || String.IsNullOrEmpty(m.RelatedReportContent)  //相關報告內容概述
                || (Convert.ToInt16(m.HistoricalCatastrophe) == 1 && String.IsNullOrEmpty(m.HistoricalCatastropheMemo))  //歷史災害描述
                //|| String.IsNullOrEmpty(m.ProtectionTarget)  //保護標的
                || (Convert.ToInt16(m.ProposalReviewAttributesSeq)==3 && String.IsNullOrEmpty(m.SetConditions))  //設計條件
                || (m.ProposalReviewAttributesSeq == 3 && String.IsNullOrEmpty(m.D01FileName))  //公共工程生態檢核自評表
                || (m.ProposalReviewAttributesSeq == 3 && String.IsNullOrEmpty(m.D02FileName))  //P-01 提案階段工程生態背景資料表
                || (m.ProposalReviewAttributesSeq == 3 && String.IsNullOrEmpty(m.D03FileName))  //P-02 提案階段現場勘查紀錄表
                || (m.ProposalReviewAttributesSeq == 3 && String.IsNullOrEmpty(m.D04FileName))  //P-03 提案階段民眾參與紀錄表
                || (m.ProposalReviewAttributesSeq == 3 && String.IsNullOrEmpty(m.D05FileName))  //P-04 提案階段生態保育原則研擬紀錄表
                || (m.ProposalReviewAttributesSeq == 3 && String.IsNullOrEmpty(m.D06FileName))  //P-05 提案工程生態檢核作業事項確認表
                || (m.ProposalReviewAttributesSeq == 3 && !m.DemandCarbonEmissions.HasValue)   //需求碳排量
                )
            {
                string sMsg = "";
                if (String.IsNullOrEmpty(m.RptName)) sMsg += $"\n「工程名稱」";
                if (m.ProposalReviewTypeSeq == null) sMsg += $"\n「類別」";
                if (m.ProposalReviewAttributesSeq == null) sMsg += $"\n「屬性」";
                if (Convert.ToInt16(m.ProposalReviewTypeSeq) == 1 && !m.RiverSeq3.HasValue) sMsg += $"\n「河川」";
                if (Convert.ToInt16(m.ProposalReviewTypeSeq) == 2 && String.IsNullOrEmpty(m.DrainName)) sMsg += $"\n「排水」";
                if (Convert.ToInt16(m.ProposalReviewTypeSeq) == 3 && String.IsNullOrEmpty(m.Coastal)) sMsg += $"\n「海岸」";
                if (Convert.ToInt16(m.ProposalReviewTypeSeq) > 3 && (!m.RiverSeq3.HasValue || String.IsNullOrEmpty(m.DrainName) || String.IsNullOrEmpty(m.Coastal))) sMsg += $"\n「河川或排水或海岸」";
                if (String.IsNullOrEmpty(m.LargeSectionChainage)) sMsg += $"\n「大斷面樁號」";
                if (!m.CitySeq.HasValue || !m.TownSeq.HasValue) sMsg += $"\n「所在行政區域」";
                if (!m.CoordX.HasValue || !m.CoordY.HasValue) sMsg += $"\n「經緯座標」";
                //if (String.IsNullOrEmpty(m.EngineeringScale)) sMsg += $"\n「工程規模」";
                if (String.IsNullOrEmpty(m.ProcessReason)) sMsg += $"\n「辦理緣由」";
                //if (String.IsNullOrEmpty(m.EngineeringScaleMemo)) sMsg += $"\n「工程規模說明」";
                if (String.IsNullOrEmpty(m.RelatedReportContent)) sMsg += $"\n「相關報告內容概述」";
                if (Convert.ToInt16(m.HistoricalCatastrophe) == 1 && String.IsNullOrEmpty(m.HistoricalCatastropheMemo)) sMsg += $"\n「歷史災害描述」";
                //if (String.IsNullOrEmpty(m.ProtectionTarget)) sMsg += $"\n「保護標的」";
                if (Convert.ToInt16(m.ProposalReviewAttributesSeq) == 3) 
                {
                    if (String.IsNullOrEmpty(m.SetConditions)) sMsg += $"\n「設計條件」";
                    if (String.IsNullOrEmpty(m.D01FileName) || String.IsNullOrEmpty(m.D02FileName) 
                        || String.IsNullOrEmpty(m.D03FileName) || String.IsNullOrEmpty(m.D04FileName) 
                        || String.IsNullOrEmpty(m.D05FileName) || String.IsNullOrEmpty(m.D06FileName)) sMsg += $"\n「生態保育原則」";
                    if (!m.DemandCarbonEmissions.HasValue) sMsg += $"\n「需求碳排量(頓)」";
                }
                return Json(new
                {
                    result = -1,
                    msg = "資料不完整，以下為必填!!"+sMsg
                });
            }

            if (!String.IsNullOrEmpty(m.RptYear.ToString()) && !String.IsNullOrEmpty(m.RptName))
            {
                if (Convert.ToInt16(m.ProposalReviewTypeSeq) >= 2 && !String.IsNullOrEmpty(m.DrainName))
                {
                    List<SelectVM> items = engReportService.GetDrainList();
                    var item = items.Where(p => p.Text == m.DrainName)
                        .Select(row => new
                        {
                            DrainSeq = row.Value

                        }).FirstOrDefault();
                    m.DrainSeq = Convert.ToInt32(item.DrainSeq);
                }

                if (m.RptTypeSeq == 2)
                {
                    //提案審查填報
                    m.RptTypeSeq = 4;
                }

                //送簽
                if (isSend == 1 && m.IsProposalReview == 0)
                {
                    m.IsProposalReview = 1;
                }

                int state;
                state = engReportService.UpdateEngReportForPR(m);
                if (state == 0)
                {
                    if (m.ProposalAuditOpinion != null)
                        engReportService.UpdateEngReportForPRProposalAudit(m);

                    return Json(new
                    {
                        result = 0,
                        msg = "儲存成功"
                    });
                }
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                });
            }

            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }

        //上傳附件
        public JsonResult UploadAttachment(int Seq, string fileType)
        {
            List<EngReportVModel> items = engReportService.GetEegReportBySeq<EngReportVModel>(Seq);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程提案資料錯誤"
                });
            }
            EngReportVModel m = items[0];
            string folder = Utils.GetEngReportFolder(m.Seq);
            int fCount = Request.Files.Count;
            if (fCount > 0 && Request.Files[0].ContentLength > 0)
            {
                try
                {
                    var file = Request.Files[0];
                    string GUID = Guid.NewGuid().ToString("B").ToUpper();
                    string fullPath = "";
                    switch (fileType)
                    {
                        case "D1": m.EcologicalConservationD01 = GUID + file.FileName; fullPath = Path.Combine(folder, m.EcologicalConservationD01); break;
                        case "D2": m.EcologicalConservationD02 = GUID + file.FileName; fullPath = Path.Combine(folder, m.EcologicalConservationD02); break;
                        case "D3": m.EcologicalConservationD03 = GUID + file.FileName; fullPath = Path.Combine(folder, m.EcologicalConservationD03); break;
                        case "D4": m.EcologicalConservationD04 = GUID + file.FileName; fullPath = Path.Combine(folder, m.EcologicalConservationD04); break;
                        case "D5": m.EcologicalConservationD05 = GUID + file.FileName; fullPath = Path.Combine(folder, m.EcologicalConservationD05); break;
                        case "D6": m.EcologicalConservationD06 = GUID + file.FileName; fullPath = Path.Combine(folder, m.EcologicalConservationD06); break;
                    }

                    file.SaveAs(fullPath);
                }
                catch (Exception e)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "上傳檔案失敗"
                    });
                }
            }
            else
                m.FileName = "";
            int state = -1;
            state = engReportService.UpdateEngReportForUA(m, fileType);
            //if (fCount > 0 && state > 0) System.IO.File.Delete(Path.Combine(folder, items[0].FileName));

            if (state == -1)
            {
                return Json(new
                {
                    result = -1,
                    message = "儲存失敗"
                });
            }
            return Json(new
            {
                result = 0,
                message = "儲存完成"
            });
        }

        //下載附件
        public ActionResult Download(int id, string fileNo)
        {
            List<EngReportVModel> items = engReportService.GetDownloadFile<EngReportVModel>(id, fileNo);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EngReportVModel m = items[0];
            string fName = Path.Combine(Utils.GetEngReportFolder(m.Seq), m.FileName);
            if (!System.IO.File.Exists(fName))
            {
                return Json(new
                {
                    result = -1,
                    message = "未發現檔案"
                }, JsonRequestBehavior.AllowGet);
            }
            m.FileName = items[0].FileName.Substring(items[0].FileName.LastIndexOf('}') + 1);
            Stream iStream = new FileStream(fName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, "application/blob", m.FileName);
        }

        //下載附件-在地溝通辦理情形
        public ActionResult DownloadLC(int id)
        {
            List<EngReportLocalCommunicationVModel> items = engReportService.GetDownloadFileLC<EngReportLocalCommunicationVModel>(id);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EngReportLocalCommunicationVModel m = items[0];
            string fName = Path.Combine(Utils.GetEngReportFolder(m.EngReportSeq), m.FileName);
            if (!System.IO.File.Exists(fName))
            {
                return Json(new
                {
                    result = -1,
                    message = "未發現檔案"
                }, JsonRequestBehavior.AllowGet);
            }
            m.FileName = items[0].FileName.Substring(items[0].FileName.LastIndexOf('}') + 1);
            Stream iStream = new FileStream(fName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, "application/blob", m.FileName);
        }

        //下載附件-在地諮詢辦理情形
        public ActionResult DownloadSC(int id)
        {
            List<EngReportOnSiteConsultationVModel> items = engReportService.GetDownloadFileSC<EngReportOnSiteConsultationVModel>(id);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EngReportOnSiteConsultationVModel m = items[0];
            string fName = Path.Combine(Utils.GetEngReportFolder(m.EngReportSeq), m.FileName);
            if (!System.IO.File.Exists(fName))
            {
                return Json(new
                {
                    result = -1,
                    message = "未發現檔案"
                }, JsonRequestBehavior.AllowGet);
            }
            m.FileName = items[0].FileName.Substring(items[0].FileName.LastIndexOf('}') + 1);
            Stream iStream = new FileStream(fName, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, "application/blob", m.FileName);
        }

        //刪除附件
        public JsonResult DelAttachment(int Seq, string fileNo)
        {
            List<EngReportVModel> items = engReportService.GetEegReportBySeq<EngReportVModel>(Seq);
            if (items.Count == 0)
            {
                return Json(new
                {
                    result = 0,
                    message = "資料刪除完成"
                });
            }
            else
            {
                EngReportVModel m = items[0];
                int result = engReportService.UpdateEngReportForDA(m, fileNo);
                if (result == 1)
                {
                    //刪除已儲存檔案
                    string uniqueFileName = "";
                    switch (fileNo)
                    {
                        case "D1": uniqueFileName = m.EcologicalConservationD01; break;
                        case "D2": uniqueFileName = m.EcologicalConservationD02; break;
                        case "D3": uniqueFileName = m.EcologicalConservationD03; break;
                        case "D4": uniqueFileName = m.EcologicalConservationD04; break;
                        case "D5": uniqueFileName = m.EcologicalConservationD05; break;
                        case "D6": uniqueFileName = m.EcologicalConservationD06; break;
                    }
                    if (uniqueFileName != null && uniqueFileName.Length > 0)
                    {
                        System.IO.File.Delete(Path.Combine(Utils.GetEngReportFolder(items[0].Seq), uniqueFileName));
                    }
                    return Json(new
                    {
                        result = 0,
                        message = "刪除成功",
                    });
                }
                else
                {
                    return Json(new
                    {
                        result = 1,
                        message = "刪除錯誤"
                    });
                }
            }
        }

        //工程提案子表 - 新增用格式
        public JsonResult NewRecord(int Seq, int id)
        {
            object newRecord = new object();
            switch (id) 
            {
                case 1:
                    newRecord = new
                        {
                            result = 0,
                            item = new EngReportEstimatedCostVModel() { Seq = -1, EngReportSeq = Seq, edit = false }
                        };
                    break;
                case 2:
                    newRecord = new
                    {
                        result = 0,
                        item = new EngReportLocalCommunicationVModel() { Seq = -1, EngReportSeq = Seq, edit = false }
                    };
                    break;
                case 3:
                    newRecord = new
                    {
                        result = 0,
                        item = new EngReportOnSiteConsultationVModel() { Seq = -1, EngReportSeq = Seq, edit = false }
                    };
                    break;
                case 4:
                    newRecord = new
                    {
                        result = 0,
                        item = new EngReportMainJobDescriptionVModel() { Seq = -1, EngReportSeq = Seq, edit = false }
                    };
                    break;
            }
            return Json(newRecord);
        }

        //工程提案子表 - 清單
        public JsonResult GetSubList(int Seq, int id)
        {
            object newRecord = new object();
            switch (id)
            {
                case 1:
                    var ERListA = engReportService.GetEngReportEstimatedCostList<EngReportEstimatedCostVModel>(Seq);
                    newRecord = new { items = ERListA };
                    break;
                case 2:
                    var ERListB = engReportService.GetEngReportLocalCommunicationList<EngReportLocalCommunicationVModel>(Seq);
                    newRecord = new { items = ERListB };
                    break;
                case 3:
                    var ERListC = engReportService.GetEngReportOnSiteConsultatioList<EngReportOnSiteConsultationVModel>(Seq);
                    newRecord = new { items = ERListC };
                    break;
                case 4:
                    var ERListD = engReportService.GetEngReportMainJobDescriptionList<EngReportMainJobDescriptionVModel>(Seq);
                    newRecord = new { items = ERListD };
                    break;
            }
            return Json(newRecord);
        }

        //更新 提案審查-概估經費
        public ActionResult UpdateEngReportEstimatedCost(EngReportEstimatedCostVModel m)
        {
            if (!String.IsNullOrEmpty(m.EngReportSeq.ToString()) && 
                !String.IsNullOrEmpty(m.Year.ToString()) && 
                !String.IsNullOrEmpty(m.Price.ToString()) && 
                !String.IsNullOrEmpty(m.AttributesSeq.ToString()))
            {
                int state;
                if (m.Seq == -1)
                    state = engReportService.AddEngReportEstimatedCost(m);
                else
                    state = engReportService.UpdateEngReportEstimatedCost(m);
                if (state == 0)
                {
                    return Json(new
                    {
                        result = 0,
                        msg = "儲存成功"
                    });
                }
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                });
            }

            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }

        //新增 提案審查-在地溝通辦理情形
        public ActionResult AddEngReportLocalCommunication(EngReportLocalCommunicationVModel m)
        {
            if (!String.IsNullOrEmpty(m.EngReportSeq.ToString()) &&
                !String.IsNullOrEmpty(m.Date.ToString()) &&
                !String.IsNullOrEmpty(m.FileNumber.ToString())
               )
            {
                string folder = Utils.GetEngReportFolder(m.EngReportSeq);
                int fCount = Request.Files.Count;
                if (fCount > 0 && Request.Files[0].ContentLength > 0)
                {
                    try
                    {
                        var file = Request.Files[0];
                        string GUID = Guid.NewGuid().ToString("B").ToUpper();
                        string fullPath = "";
                        m.FilePath = GUID + file.FileName; fullPath = Path.Combine(folder, m.FilePath);

                        file.SaveAs(fullPath);
                    }
                    catch (Exception e)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "上傳檔案失敗:" + e.Message +";" +e.StackTrace
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "未取得上傳檔案"
                    });
                }

                int state;
                if (m.Seq == -1)
                    state = engReportService.AddEngReportLocalCommunication(m);
                else
                    state = engReportService.UpdateEngReportLocalCommunication(m);
                if (state == 0)
                {
                    return Json(new
                    {
                        result = 0,
                        msg = "儲存成功"
                    });
                }
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                });
            }

            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }

        //更新 提案審查-在地溝通辦理情形
        public ActionResult UpdateEngReportLocalCommunication(EngReportLocalCommunicationVModel m)
        {
            if (!String.IsNullOrEmpty(m.EngReportSeq.ToString()) &&
                !String.IsNullOrEmpty(m.Date.ToString()) &&
                !String.IsNullOrEmpty(m.FileNumber.ToString())
               )
            {
                m.Date = Convert.ToDateTime(m.DateStr);
                int state;
                state = engReportService.UpdateEngReportLocalCommunication(m);
                if (state == 0)
                {
                    return Json(new
                    {
                        result = 0,
                        msg = "儲存成功"
                    });
                }
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                });
            }

            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }

        //新增 提案審查-在地諮詢辦理情形
        public ActionResult AddEngReportOnSiteConsultation(EngReportOnSiteConsultationVModel m)
        {
            if (!String.IsNullOrEmpty(m.EngReportSeq.ToString()) &&
                !String.IsNullOrEmpty(m.Date.ToString()) &&
                !String.IsNullOrEmpty(m.FileNumber.ToString())
               )
            {
                string folder = Utils.GetEngReportFolder(m.EngReportSeq);
                int fCount = Request.Files.Count;
                if (fCount > 0 && Request.Files[0].ContentLength > 0)
                {
                    try
                    {
                        var file = Request.Files[0];
                        string GUID = Guid.NewGuid().ToString("B").ToUpper();
                        string fullPath = "";
                        m.FilePath = GUID + file.FileName; fullPath = Path.Combine(folder, m.FilePath);

                        file.SaveAs(fullPath);
                    }
                    catch (Exception e)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "上傳檔案失敗"
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "未取得上傳檔案"
                    });
                }

                int state;
                if (m.Seq == -1)
                    state = engReportService.AddEngReportOnSiteConsultation(m);
                else
                    state = engReportService.UpdateEngReportOnSiteConsultation(m);
                if (state == 0)
                {
                    return Json(new
                    {
                        result = 0,
                        msg = "儲存成功"
                    });
                }
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                });
            }

            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }

        //更新 提案審查-在地諮詢辦理情形
        public ActionResult UpdateEngReportOnSiteConsultation(EngReportOnSiteConsultationVModel m)
        {
            if (!String.IsNullOrEmpty(m.EngReportSeq.ToString()) &&
                !String.IsNullOrEmpty(m.Date.ToString()) &&
                !String.IsNullOrEmpty(m.FileNumber.ToString())
               )
            {
                m.Date = Convert.ToDateTime(m.DateStr);
                int state;
                state = engReportService.UpdateEngReportOnSiteConsultation(m);
                if (state == 0)
                {
                    return Json(new
                    {
                        result = 0,
                        msg = "儲存成功"
                    });
                }
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                });
            }

            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }

        //更新 提案審查-主要工作內容
        public ActionResult UpdateEngReportMainJobDescription(EngReportMainJobDescriptionVModel m)
        {
            if (!String.IsNullOrEmpty(m.EngReportSeq.ToString()) &&
                !String.IsNullOrEmpty(m.RptJobDescriptionSeq.ToString()) &&
                //!String.IsNullOrEmpty(m.OtherJobDescription.ToString()) &&
                !String.IsNullOrEmpty(m.Cost.ToString()))// &&
                //!String.IsNullOrEmpty(m.Memo.ToString()))
            {
                int state;
                if (m.Seq == -1)
                    state = engReportService.AddEngReportMainJobDescription(m);
                else
                    state = engReportService.UpdateEngReportMainJobDescription(m);
                if (state == 0)
                {
                    return Json(new
                    {
                        result = 0,
                        msg = "儲存成功"
                    });
                }
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                });
            }

            return Json(new
            {
                result = -1,
                msg = "儲存失敗"
            });
        }

        //刪除 提案審查-概估經費
        public ActionResult DelEngReportEstimatedCost(int id)
        {
            if (engReportService.DelEngReportEstimatedCost(id) == -1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "刪除失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    msg = "刪除完成"
                });
            }
        }

        //刪除 提案審查-在地溝通辦理情形
        public ActionResult DelEngReportLocalCommunication(int id)
        {
            if (engReportService.DelEngReportLocalCommunication(id) == -1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "刪除失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    msg = "刪除完成"
                });
            }
        }

        //刪除 提案審查-在地諮詢辦理情形
        public ActionResult DelEngReportOnSiteConsultation(int id)
        {
            if (engReportService.DelEngReportOnSiteConsultation(id) == -1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "刪除失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    msg = "刪除完成"
                });
            }
        }

        //刪除 提案審查-主要工作內容
        public ActionResult DelEngReportMainJobDescription(int id)
        {
            if (engReportService.DelEngReportMainJobDescription(id) == -1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "刪除失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    msg = "刪除完成"
                });
            }
        }

        public ActionResult ViewEng(int seq)
        {
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("EView", "ERProposalReview");
            string tarUrl = redirectUrl.ToString() + "?id=" + seq;
            return Json(new { Url = tarUrl });
        }

        public ActionResult EView()
        {
            Utils.setUserClass(this, 2);
            List<VMenu> menu = new List<VMenu>();
            var link = new UrlHelper(Request.RequestContext).Action("Index", "ERProposalReview");
            menu.Add(new VMenu() { Name = "提案審查", Url = link.ToString() });
            menu.Add(new VMenu() { Name = "提案審查", Url = "" });
            ViewBag.breadcrumb = menu;
            return View("EView");
        }

        //更新工程 FOR 提案審查
        public ActionResult UpdateEngReportForPRO(EngReportVModel m)
        {
            int state;
            state = engReportService.UpdateEngReportForPR1(m);
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
    }
}