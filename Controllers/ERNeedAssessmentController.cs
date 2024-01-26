using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;

namespace EQC.Controllers
{
    [SessionFilter]
    public class ERNeedAssessmentController : Controller
    {//工程提報 - 需求評估

        EngReportService engReportService = new EngReportService();

        public ActionResult Index()
        {
            Utils.setUserClass(this, 1);
            return View();
        }

        public JsonResult NewRecord()
        {
            string unitSubSeq = "";
            string unitSeq = "";
            string unitSubN = "";
            string unitN = "";
            Utils.GetUserUnit(ref unitSeq, ref unitN, ref unitSubSeq, ref unitSubN);
            return Json(new
            {
                result = 0,
                item = new EngReportVModel() { Seq = -1, RptYear = 0, RptName = ""
                                                , ExecUnitSeq = Convert.ToInt16(unitSeq), ExecUnit = unitN
                                                , ExecSubUnitSeq = Convert.ToInt16(unitSubSeq), ExecSubUnit = unitSubN
                                                , ExecUser = Utils.getUserInfo().DisplayName, CreateUserSeq = Utils.getUserSeq() }
            });
        }

        //工程清單
        public JsonResult GetList(int year, int unit, int subUnit, int rptType, int pageRecordCount, int pageIndex)
        {
            List<EngReportVModel> engList = new List<EngReportVModel>();
            int total = engReportService.GetEngListCount(year, unit, subUnit, rptType, 1);
            if (total > 0)
            {
                engList = engReportService.GetEngList<EngReportVModel>(year, unit, subUnit, rptType, pageRecordCount, pageIndex, 1);
            }
            return Json(new
            {
                pTotal = total,
                items = engList
            });
        }

        //工程清單-待核清單
        public JsonResult GetListB(int year, int unit, int subUnit, int rptType, int pageRecordCount, int pageIndex)
        {
            subUnit = -1;
            rptType = 0;
            List<EngReportVModel> engList = new List<EngReportVModel>();
            int total = engReportService.GetEngListCount(year, unit, subUnit, rptType, 11);
            if (total > 0)
            {
                engList = engReportService.GetEngList<EngReportVModel>(year, unit, subUnit, rptType, pageRecordCount, pageIndex, 11);
            }
            return Json(new
            {
                pTotal = total,
                items = engList
            });
        }

        //更新 工程
        public ActionResult UpdateEngReport(EngReportVModel m)
        {
            if (!String.IsNullOrEmpty(m.RptYear.ToString()) && !String.IsNullOrEmpty(m.RptName))
            {
                int state;
                if (m.Seq == -1)
                    state = engReportService.AddEngReport(m);
                else
                    state = engReportService.UpdateEngReport(m);
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

        //更新工程提報-需求評估-填報
        public ActionResult UpdateEngReportForNA(EngReportVModel m, int naType)
        {
            int state;
            state = engReportService.UpdateEngReportForNA(m, naType);
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

        //更新工程提報-需求評估-送簽或簽核
        public ActionResult UpdateEngReportForNAApproval(EngReportVModel m, EngReportApproveVModel era)
        {
            if (String.IsNullOrEmpty(m.OriginAndScope) || String.IsNullOrEmpty(m.RelatedReportResults)
                 || String.IsNullOrEmpty(m.FacilityManagement) || String.IsNullOrEmpty(m.ProposalScopeLand)
                  || String.IsNullOrEmpty(m.LocationMapFileName) 
                  || String.IsNullOrEmpty(m.AerialPhotographyFileName)
                  || String.IsNullOrEmpty(m.ScenePhotoFileName) 
                )
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料不完整!!"
                });
            }

            int state;
            era.EngReportSeq = m.Seq;
            //更新簽核
            state = new EngReportApproveService().Update(m,era);
            if (state == 0)
            {
                state = engReportService.UpdateEngReportForNAApproval(m);

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

        //刪除工程
        public ActionResult DelEngReport(int id)
        {
            if (engReportService.DelEngReport(id) == -1)
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
                    //items = engMainService.SupervisorUserList<EngSupervisorVModel>(eng),
                    msg = "刪除完成"
                });
            }
        }

        public ActionResult EditEng(int seq)
        {
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Edit", "ERNeedAssessment");
            string tarUrl = redirectUrl.ToString() + "?id=" + seq;
            return Json(new { Url = tarUrl });
        }

        public ActionResult Edit()
        {
            Utils.setUserClass(this, 2);
            //ViewBag.Title = "需求評估-填報";
            List<VMenu> menu = new List<VMenu>();
            var link = new UrlHelper(Request.RequestContext).Action("Index", "ERNeedAssessment");
            menu.Add(new VMenu() { Name = "需求評估", Url = link.ToString() });
            menu.Add(new VMenu() { Name = "需求評估", Url = "" });
            ViewBag.breadcrumb = menu;
            return View("Edit");
        }

        //取得單筆工程
        public virtual JsonResult GetEngReport(int id)
        {
            List<EngReportVModel> items = engReportService.GetEegReportBySeq<EngReportVModel>(id);
            var userInfo = Utils.getUserInfo();
            var userRole = userInfo.RoleSeq;
            if (items.Count == 1)
            {
                string unitSubSeq = "";
                string unitSeq = "";
                string unitSubN = "";
                string unitN = "";
                Utils.GetUserUnit(ref unitSeq, ref unitN, ref unitSubSeq, ref unitSubN);

                items[0].IsEditA = 0;
                items[0].IsEditB = 0;
                items[0].IsEditC = 0;
                items[0].IsEditD = 0;
                items[0].IsEditF = 0;
                items[0].IsEditFile = 0;
                items[0].IsSavaApproval =

                       (!items[0].OriginAndScopeReviewState || items[0].OriginAndScopeUpdateReviewUserName == items[0].OriginAndScopeAssignReviewUserName) &&
                       ( !items[0].RelatedReportResultsReviewState || items[0].RelatedReportResultsAssignReviewUserName == items[0].RelatedReportResultsUpdateReviewUserName ) &&
                       ( !items[0].FacilityManagementReviewState || items[0].FacilityManagementAssignReviewUserName == items[0].FacilityManagementUpdateReviewUserName )&&
                       (!items[0].ProposalScopeLandReviewState || items[0].ProposalScopeLandAssignReviewUserName == items[0].ProposalScopeLandUpdateReviewUserName );
                
                items[0].LocationMapFileName = items[0].LocationMapFileName != null ? items[0].LocationMapFileName : "";
                items[0].AerialPhotographyFileName = items[0].AerialPhotographyFileName != null ? items[0].AerialPhotographyFileName : "";
                items[0].ScenePhotoFileName = items[0].ScenePhotoFileName != null ? items[0].ScenePhotoFileName : "";
                items[0].BaseMapFileName = items[0].BaseMapFileName != null ? items[0].BaseMapFileName : "";
                items[0].EngPlaneLayoutFileName = items[0].EngPlaneLayoutFileName != null ? items[0].EngPlaneLayoutFileName : "";
                items[0].LongitudinalSectionFileName = items[0].LongitudinalSectionFileName != null ? items[0].LongitudinalSectionFileName : "";
                items[0].StandardSectionFileName = items[0].StandardSectionFileName != null ? items[0].StandardSectionFileName : "";
                if (items[0].ExecUnitSeq.ToString() == unitSeq || userRole == 1 ||userRole == 2 )
                {
                    //取得單筆簽核資料
                    EngReportApproveService engReportApproveService = new EngReportApproveService();
                    List<EngReportApproveVModel> list = engReportApproveService.GetEngReportApprove<EngReportApproveVModel>(id);

                    if (userRole == 1 || userRole == 2 || userRole == 3)
                    {
                        //系統管理者
                        items[0].IsEditA = 1;
                        items[0].IsEditB = 1;
                        items[0].IsEditC = 1;
                        items[0].IsEditD = 1;
                        items[0].IsEditF = 1;
                        items[0].IsEditFile = 1;
 
                    }
                    else if (list.Count > 0) 
                    {
                        //建立者或簽核者
                        items[0].IsEditA = 1;
                        items[0].IsEditB = 1;
                        items[0].IsEditC = 1;
                        items[0].IsEditD = 1;
                        items[0].IsEditF = 1;
                        items[0].IsEditFile = 1;
                    }
                    else
                    {
                        items[0].IsSavaApproval = false;
                        if ((items[0].OriginAndScopeReviewState && items[0].OriginAndScopeAssignReviewUserSeq == Utils.getUserSeq())
                            || (items[0].RelatedReportResultsReviewState && items[0].RelatedReportResultsAssignReviewUserSeq == Utils.getUserSeq())
                            || (items[0].FacilityManagementReviewState && items[0].FacilityManagementAssignReviewUserSeq == Utils.getUserSeq())
                            || (items[0].ProposalScopeLandReviewState && items[0].ProposalScopeLandAssignReviewUserSeq == Utils.getUserSeq()))
                        {
                            //覆核者
                            items[0].IsEditFile = 1;
                            if (items[0].RelatedReportResultsReviewState && items[0].RelatedReportResultsAssignReviewUserSeq == Utils.getUserSeq()) { items[0].IsEditB = 1; }
                            else if (items[0].FacilityManagementReviewState && items[0].FacilityManagementAssignReviewUserSeq == Utils.getUserSeq()) { items[0].IsEditC = 1; }
                            else if (items[0].ProposalScopeLandReviewState && items[0].ProposalScopeLandAssignReviewUserSeq == Utils.getUserSeq()) { items[0].IsEditD = 1; }
                        }
                    }
                }
                items[0].IsEditFile = !(items[0].CreateUserSeq != userInfo.Seq && userInfo.RoleSeq == 20) ? 1 : 0;
                return Json(new
                {
                    result = 0,
                    item = items[0],

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

        //取得工程簽核流程清單
        public JsonResult GetEngReportApproveList(int engReportSeq)
        {
            EngReportApproveService engReportApproveService = new EngReportApproveService();

            List<EngReportApproveVModel> engApproveList = new List<EngReportApproveVModel>();

            engApproveList = engReportApproveService.GetEngReportApproveList<EngReportApproveVModel>(engReportSeq);

            return Json(new
            {
                result = 0,
                items = engApproveList
            });
        }

        //取得單筆工程簽核流程
        public JsonResult GetEngReportApprove(int engReportSeq)
        {
            EngReportApproveService engReportApproveService = new EngReportApproveService();

            EngReportApproveVModel model;
            List<EngReportApproveVModel> list = engReportApproveService.GetEngReportApprove<EngReportApproveVModel>(engReportSeq);

            if (list.Count > 0)
            {
                model = list[0];
            }
            else
            {
                model = new EngReportApproveVModel() { Seq = -1 };
            }

            //if (String.IsNullOrEmpty(model.Signature) && (model.ApprovalMethod == 2 || model.ApprovalMethod == 3))
            //{
            //    List<SignatureFileVM> signatureFileVMList = new UserService().GetSignatureFileByUserSeq(Utils.getUserSeq());
            //    if (signatureFileVMList.Count > 0)
            //    {
            //        SignatureFileVM signatureFileVM = signatureFileVMList[0];
            //        string fullPath = Path.Combine(Server.MapPath(signatureFileVM.FilePath), signatureFileVM.FileName);
            //        //model.Signature = @"data:image/png;base64," + Convert.ToBase64String(System.IO.File.ReadAllBytes(fullPath));
            //        Image im = Utils.ResizeImage(400, 200, fullPath);
            //        using (MemoryStream ms = new MemoryStream())
            //        {
            //            im.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
            //            byte[] imageBytes = ms.ToArray();

            //            model.Signature = @"data:image/png;base64," + Convert.ToBase64String(imageBytes);
            //        }
            //    }
            //}

            if (String.IsNullOrEmpty(model.Signature)) model.Signature = "";

            return Json(new
            {
                result = 0,
                item = model
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
                    msg = "工程提報資料錯誤"
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
                        case "A1": m.LocationMap = GUID + file.FileName; fullPath = Path.Combine(folder, m.LocationMap); break;
                        case "A2": m.AerialPhotography = GUID + file.FileName; fullPath = Path.Combine(folder, m.AerialPhotography); break;
                        case "A3": m.ScenePhoto = GUID + file.FileName; fullPath = Path.Combine(folder, m.ScenePhoto); break;
                        case "A4": m.BaseMap = GUID + file.FileName; fullPath = Path.Combine(folder, m.BaseMap); break;
                        case "A5": m.EngPlaneLayout = GUID + file.FileName; fullPath = Path.Combine(folder, m.EngPlaneLayout); break;
                        case "A6": m.LongitudinalSection = GUID + file.FileName; fullPath = Path.Combine(folder, m.LongitudinalSection); break;
                        case "A7": m.StandardSection = GUID + file.FileName; fullPath = Path.Combine(folder, m.StandardSection); break;
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
                        case "A1": uniqueFileName = m.LocationMap; break;
                        case "A2": uniqueFileName = m.AerialPhotography; break;
                        case "A3": uniqueFileName = m.ScenePhoto; break;
                        case "A4": uniqueFileName = m.BaseMap; break;
                        case "A5": uniqueFileName = m.EngPlaneLayout; break;
                        case "A6": uniqueFileName = m.LongitudinalSection; break;
                        case "A7": uniqueFileName = m.StandardSection; break;
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

        public ActionResult ViewEng(int seq)
        {
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("EView", "ERNeedAssessment");
            string tarUrl = redirectUrl.ToString() + "?id=" + seq;
            return Json(new { Url = tarUrl });
        }

        public ActionResult EView()
        {
            Utils.setUserClass(this, 2);
            List<VMenu> menu = new List<VMenu>();
            var link = new UrlHelper(Request.RequestContext).Action("Index", "ERNeedAssessment");
            menu.Add(new VMenu() { Name = "需求評估", Url = link.ToString() });
            menu.Add(new VMenu() { Name = "需求評估", Url = "" });
            ViewBag.breadcrumb = menu;
            return View("EView");
        }


        /// <summary> 取得人員列表 </summary>
        /// <param name="page"> 頁數 </param>
        /// <param name="per_page"> 跳頁 </param>
        /// <param name="unitSeq"> 單位序號 </param>
        /// <returns></returns>
        public JsonResult GetUserList(int page, int per_page, int unitSeq, string nameSearch)
        {
            SessionManager sessionManager = new SessionManager();
            List<VUserMain> list = new List<VUserMain>();
            list = new UserService().GetList(unitSeq, "%", 0, 9999);
            return Json(new
            {
                l = list,
            }, JsonRequestBehavior.AllowGet);
        }
    }
}