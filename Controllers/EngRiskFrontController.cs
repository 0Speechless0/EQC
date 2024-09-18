
using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using EQC.ViewModel.Common;
using System;
using System.Collections.Generic;

using System.IO;
using EQC.ProposalV2;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Linq;
using System.Web;
using Org.BouncyCastle.Ocsp;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EngRiskFrontController : Controller
    {//工程品管 - 施工風險評估報告產製
        EngRiskFrontService iService = new EngRiskFrontService();


        EngRiskFrontManagementService iServiceM = new EngRiskFrontManagementService();

        public ActionResult Index()
        {
            Utils.setUserClass(this, 2);
            return View("Index");
        }

        //標案 EngMain
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
            else
            {
                return Json(new
                {
                    result = 2,
                    msg = "讀取資料錯誤"
                });
            }
        }

        //施工風險
        public virtual JsonResult GetEngRiskMain(int id)
        {
            List<EngRiskFrontListVModel> items = iService.GetItem<EngRiskFrontListVModel>(id);
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
                iService.AddEngRiskMain(id);
                items = iService.GetItem<EngRiskFrontListVModel>(id);
                iServiceM.GoJsSubProjectJsonSetting(items.FirstOrDefault()?.Seq);
                if (items.Count == 1)
                {
                    return Json(new
                    {
                        result = 0,
                        item = items[0]
                    });
                }
                return Json(new
                {
                    result = 2,
                    msg = "讀取資料錯誤"
                });
            }
        }

        //更新施工風險
        public ActionResult UpdateEngRiskMain(EngRiskFrontListVModel m)
        {
            int state;
            state = iService.UpdateEngRiskMain(m);
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

        //讀取子表
        public JsonResult GetSubList(int Type, int Seq)
        {
            object newRecord = new object();
            switch (Type)
            {
                case 1:
                case 4:
                    var listA = iService.GetEngRiskFrontProjectOutlineFunction<EngRiskFrontProjectOutlineFunctionVModel>(Seq);
                    newRecord = new { items = listA };
                    break;
                case 2:
                case 5:
                    var listB = iService.GetEngRiskFrontProjectOutlineSiteEnvironment<EngRiskFrontProjectOutlineSiteEnvironmentVModel>(Seq);
                    newRecord = new { items = listB };
                    break;
                case 3:
                    var listC = iService.GetEngRiskFrontEvaluationTeam<EngRiskFrontEvaluationTeamVModel>(Seq);
                    newRecord = new { items = listC };
                    break;
                case 6:
                    var listF = iService.GetEngRiskFrontProjectSelection<EngRiskFrontProjectSelectionVModel>(Seq);
                    newRecord = new { items = listF };
                    break;
                case 7:
                    var listG = iService.GetEngRiskFrontSubProjectList<EngRiskFrontSubProjectListModel>(Seq);
                    newRecord = new { items = listG };
                    break;
                case 8:
                    var listH = iService.GetEngRiskFrontSubProjectDetail<EngRiskFrontSubProjectDetailModel>(Seq);
                    newRecord = new { items = listH };
                    break;
            }
            return Json(newRecord);
        }

        //刪除
        public ActionResult DelRecord(int Type, int id)
        {
            int iResoult = 0;
            switch (Type)
            {
                case 1:
                case 4:
                    iResoult = iService.DelEngRiskFrontProjectOutlineFunction(id);
                    break;
                case 2:
                case 5:
                    iResoult = iService.DelEngRiskFrontProjectOutlineSiteEnvironment(id);
                    break;
                case 3:
                    iResoult = iService.DelEngRiskFrontEvaluationTeam(id);
                    break;
                case 6:
                    iResoult = iService.DelEngRiskFrontProjectSelection(id);
                    break;
            }

            if(iResoult==-1)
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

        //讀取對策處置人員
        public JsonResult GetUserPrincipalList(int Seq)
        {
            List<SelectVM> list = iService.GetUserPrincipalList(Seq);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //上傳附件
        public JsonResult UploadAttachment(int Seq, string fileType)
        {
            List<EngRiskFrontListVModel> items = iService.GetItem<EngRiskFrontListVModel>(Seq);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "施工風險資料錯誤"
                });
            }
            EngRiskFrontListVModel m = items[0];
            string folder = Utils.GetEngRiskFrontFolder(Seq);
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
                        case "A1": m.PlanScopeFile = GUID + file.FileName; fullPath = Path.Combine(folder, m.PlanScopeFile); break;
                        case "A2": m.DesignConceptFile = GUID + file.FileName; fullPath = Path.Combine(folder, m.DesignConceptFile); break;
                        case "A3": m.DesignSelectionFile = GUID + file.FileName; fullPath = Path.Combine(folder, m.DesignSelectionFile); break;
                        case "A4": m.DesignStageRiskResultFile = GUID + file.FileName; fullPath = Path.Combine(folder, m.DesignStageRiskResultFile); break;
                        case "A5": m.RiskTrackingFile = GUID + file.FileName; fullPath = Path.Combine(folder, m.RiskTrackingFile); break;
                        case "A6": m.ConclusionFile = GUID + file.FileName; fullPath = Path.Combine(folder, m.ConclusionFile); break;
                        case "A7": m.FinishFile = GUID + file.FileName; fullPath = Path.Combine(folder, m.FinishFile); break;
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
            state = iService.UpdateEngRiskMain(m, fileType);

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

        public ActionResult Download(int id, string fileNo, DownloadArgExtension downloadArg = null)
        {
            List<EngRiskFrontListVModel> items = iService.GetDownloadFile<EngRiskFrontListVModel>(id, fileNo);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EngRiskFrontListVModel m = items[0];
            string fName = Path.Combine(Utils.GetEngRiskFrontFolder(id), m.FileName);
            if (!System.IO.File.Exists(fName))
            {
                return Json(new
                {
                    result = -1,
                    message = "未發現檔案"
                }, JsonRequestBehavior.AllowGet);
            }
            m.FileName = items[0].FileName.Substring(items[0].FileName.LastIndexOf('}') + 1);
            FileStream iStream = new FileStream(fName, FileMode.Open, FileAccess.Read, FileShare.Read);
            downloadArg?.targetPathSetting(iStream.Name);
            return File(iStream, "application/blob", m.FileName);
        }

        public JsonResult DelAttachment(int Seq, string fileNo)
        {
            List<EngRiskFrontListVModel> items = iService.GetItem<EngRiskFrontListVModel>(Seq);
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
                EngRiskFrontListVModel m = items[0];
                int result = iService.UpdateEngRiskMain(m, fileNo);
                if (result == 1)
                {
                    //刪除已儲存檔案
                    string uniqueFileName = "";
                    switch (fileNo)
                    {
                        case "A1": uniqueFileName = m.PlanScopeFile; break;
                        case "A2": uniqueFileName = m.DesignConceptFile; break;
                        case "A3": uniqueFileName = m.DesignSelectionFile; break;
                        case "A4": uniqueFileName = m.DesignStageRiskResultFile; break;
                        case "A5": uniqueFileName = m.RiskTrackingFile; break;
                        case "A6": uniqueFileName = m.ConclusionFile; break;
                        case "A7": uniqueFileName = m.FinishFile; break;
                    }
                    if (uniqueFileName != null && uniqueFileName.Length > 0)
                    {
                        System.IO.File.Delete(Path.Combine(Utils.GetEngRiskFrontFolder(items[0].Seq), uniqueFileName));
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

        #region 工程計畫概要 - 工程功能需求
        //更新
        public ActionResult UpdateRecordA(EngRiskFrontProjectOutlineFunctionVModel m)
        {
            if (!String.IsNullOrEmpty(m.EngFunction) && !String.IsNullOrEmpty(m.EngMemo))
            {
                int state;
                if (m.Seq == -1) 
                {
                    m.PotentialHazard = "";
                    m.HazardCountermeasures = "";
                    state = iService.AddEngRiskFrontProjectOutlineFunction(m);
                }
                else
                    state = iService.UpdateEngRiskFrontProjectOutlineFunction(m);
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
        #endregion 

        #region 工程計畫概要 - 工址環境現況
        //更新
        public ActionResult UpdateRecordB(EngRiskFrontProjectOutlineSiteEnvironmentVModel m)
        {
            if (!String.IsNullOrEmpty(m.SiteEnvironment) && !String.IsNullOrEmpty(m.EngMemo))
            {
                int state;
                if (m.Seq == -1)
                {
                    m.PotentialHazard = "";
                    m.HazardCountermeasures = "";
                    state = iService.AddEngRiskFrontProjectOutlineSiteEnvironment(m);
                }
                else
                    state = iService.UpdateEngRiskFrontProjectOutlineSiteEnvironment(m);
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
        #endregion 

        #region 準備作業 - 施工風險評估小組
        //更新
        public ActionResult UpdateRecordC(EngRiskFrontEvaluationTeamModel m)
        {
            if (!String.IsNullOrEmpty(m.OrganizerUnitSeq.ToString()) && 
                !String.IsNullOrEmpty(m.UnitSeq.ToString()) &&
                !String.IsNullOrEmpty(m.PrincipalSeq.ToString())
                )
            {
                int state;
                if (m.Seq == -1)
                    state = iService.AddEngRiskFrontEvaluationTeam(m);
                else
                    state = iService.UpdateEngRiskFrontEvaluationTeam(m);
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
        #endregion 

        #region 準備作業 - 工程功能需求
        //更新
        public ActionResult UpdateRecordD(EngRiskFrontProjectOutlineFunctionVModel m)
        {
            if (!String.IsNullOrEmpty(m.EngFunction.ToString()) &&
                !String.IsNullOrEmpty(m.PotentialHazard.ToString()) &&
                !String.IsNullOrEmpty(m.HazardCountermeasures.ToString()) &&
                !String.IsNullOrEmpty(m.PrincipalSeq.ToString())
                )
            {
                int state;
                if (m.Seq == -1)
                    state = iService.AddEngRiskFrontProjectOutlineFunction(m);
                else
                    state = iService.UpdateEngRiskFrontProjectOutlineFunction(m);
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
        #endregion 

        #region 準備作業 - 工址環境現況
        //更新
        public ActionResult UpdateRecordE(EngRiskFrontProjectOutlineSiteEnvironmentVModel m)
        {
            if (!String.IsNullOrEmpty(m.SiteEnvironment.ToString()) &&
                !String.IsNullOrEmpty(m.PotentialHazard.ToString()) &&
                !String.IsNullOrEmpty(m.HazardCountermeasures.ToString()) &&
                !String.IsNullOrEmpty(m.PrincipalSeq.ToString())
                )
            {
                int state;
                if (m.Seq == -1)
                    state = iService.AddEngRiskFrontProjectOutlineSiteEnvironment(m);
                else
                    state = iService.UpdateEngRiskFrontProjectOutlineSiteEnvironment(m);
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
        #endregion 

        #region 設計方案評選 - 方案項目權重
        //更新
        public ActionResult UpdateRecordF(EngRiskFrontProjectSelectionVModel m)
        {
            if (!String.IsNullOrEmpty(m.Weight1) &&
                !String.IsNullOrEmpty(m.Weight2) &&
                !String.IsNullOrEmpty(m.Weight3) &&
                !String.IsNullOrEmpty(m.Weight4) &&
                !String.IsNullOrEmpty(m.Weight5) &&
                !String.IsNullOrEmpty(m.Weight6) &&
                !String.IsNullOrEmpty(m.Weight7)
                )
            {
                if (m.PSType == 1)
                {
                    m.TWeight = "";
                }
                else if (m.PSType == 2)
                {
                    if ((Convert.ToInt16(m.Weight1) + Convert.ToInt16(m.Weight2) + Convert.ToInt16(m.Weight3) + Convert.ToInt16(m.Weight4) + Convert.ToInt16(m.Weight5) + Convert.ToInt16(m.Weight6) + Convert.ToInt16(m.Weight7)) != 100) 
                    {
                        return Json(new
                        {
                            result = -1,
                            msg = "資料錯誤，權重加總須等於100！"
                        });
                    }
                    m.TWeight = "";
                }
                
                int state;
                if (m.Seq == -1)
                    state = iService.AddEngRiskFrontProjectSelection(m);
                else
                    state = iService.UpdateEngRiskFrontProjectSelection(m);
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
        #endregion

        #region 檔案上傳(1.設計方案評選，2.設計階段施工風險評估成果之運用，3.風險資訊傳遞及風險追蹤管理，4.結論)

        //清單
        public JsonResult GetEngRiskFrontFileList(int Seq,int ERFType)
        {
            object newRecord = new object();
            var ERListB = iService.GetEngRiskFrontFileList<EngRiskFrontFileVModel>(Seq, ERFType);
            newRecord = new { items = ERListB };
            return Json(newRecord);
        }

        //新增
        public ActionResult AddEngRiskFrontFile(EngRiskFrontFileVModel m)
        {
            if (!String.IsNullOrEmpty(m.EngRiskFrontSeq.ToString()))
            {
                string folder = Utils.GetEngRiskFrontFolder(m.EngRiskFrontSeq);
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
                //if (m.Seq == -1)
                    state = iService.AddEngRiskFrontFile(m);
                //else
                //    state = iService.UpdateEngRiskFrontFile(m);
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

        //刪除
        public ActionResult DelEngRiskFrontFile(int id)
        {
            if (iService.DelEngRiskFrontFile(id) == -1)
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

        //下載附件
        public ActionResult DownloadEngRiskFrontFile(int id)
        {
            List<EngRiskFrontFileVModel> items = iService.DownloadEngRiskFrontFile<EngRiskFrontFileVModel>(id);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            EngRiskFrontFileVModel m = items[0];
            string fName = Path.Combine(Utils.GetEngRiskFrontFolder(m.EngRiskFrontSeq), m.FileName);
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

        #endregion 

        #region 施工風險內容及施作順序
        #endregion

        #region 施工風險評估 - 明細
        #endregion

        public JsonResult UnLock(int Seq,int LockState) 
        {
            List<EngRiskFrontListVModel> items = iService.GetItem<EngRiskFrontListVModel>(Seq);
            if (items.Count != 1)
            {
                return Json(new
                {
                    result = -1,
                    msg = "施工風險資料錯誤"
                });
            }
            EngRiskFrontListVModel m = items[0];
            int state = -1;
            m.LockState = LockState;
            state = iService.UpdateEngRiskMainByLock(m);

            return Json(new
            {
                status = 1,
                message = "已解除鎖定"
            });
        }

        #region 評估報報產製
        public JsonResult CreateDocument(int id)
        {
            var ExportInstance = new EngRiskExportInstance(id);
            EngRiskFrontListVModel engRisk = iService.GetItem<EngRiskFrontListVModel>(id).FirstOrDefault();

            var eng = new EngMainService().GetItemBySeq<EngMain>(id).First();
            var attachmentFolder = Utils.GetEngRiskFrontFolder(id);
            var targetFilePath = @"RiskManagement\DocumentTp".GetFiles()
                .Select(file => new FileInfo(file))
                .OrderByDescending(file => file.LastWriteTime)
                .FirstOrDefault()?.FullName;
            var dir = HttpContext.Server.MapPath("/");

            if(targetFilePath == null)
            {
                return Json(new
                {
                    status = 2,
                    message = "範本未上傳，請至後台計畫書維護頁面上傳施工風險評估範本..."
                });
            }
            var task = Task.Run(() => {
                ExportInstance.Export(
                Path.Combine(dir, $"FileUploads/Eng/{id}/EngRiskDocument/{eng?.EngName}-施工風險評估-{DateTime.Now.ToString("yyyyMMddHHmmss")}.docx"),
                targetFilePath,
                attachmentFolder,
                Path.Combine(dir, @".\LogFiles"),
                Path.Combine(dir, @".\FileUploads\RiskManagement\Tp"));


            }) ;

            return Json(new
            {
                status = 1,
                message ="風險評估背景產製中，請稍後重整此頁下載檔案..."
            });
        }

        public JsonResult CheckDocument(int id)
        {
            var dir = HttpContext.Server.MapPath("/");
            var fileDir = Path.Combine(dir, $"FileUploads/Eng/{id}/EngRiskDocument");
            if (!Directory.Exists(fileDir)) Directory.CreateDirectory(fileDir);
            var engRisk = iService.GetItem<EngRiskFrontListVModel>(id).FirstOrDefault();

            if (engRisk?.IsFinish == -1)
                return Json(-1);
            return Json(Directory.GetFiles(fileDir).Length > 0 ? 2 : engRisk?.IsFinish ?? 0 );

        }



        #endregion
        public void DownloadDocument(int id, DownloadArgExtension downloadArgExtension)
        {


            var fileDir = $"Eng/{id}/EngRiskDocument".getUploadDir();
            var file = Directory.GetFiles(fileDir)
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTime)
                .First();
            Stream iStream = $"Eng/{id}/EngRiskDocument".GetFileStream(file.Name);
            downloadArgExtension?.targetPathSetting(Path.Combine(fileDir, file.Name));
            var ms = new MemoryStream();
            iStream.CopyTo(ms);
            Response?.AddHeader("Content-Disposition", $"attachment; filename={file.Name}");
            Response?.BinaryWrite(ms.ToArray());

        }


    }
}