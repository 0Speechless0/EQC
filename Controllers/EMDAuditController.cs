using EQC.Common;
using EQC.EDMXModel;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Xml;

using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EMDAuditController : Controller
    {
        private EMDAuditService emdAuditService = new EMDAuditService();
        private EngMainService engMainService = new EngMainService();

        //工程座標 s20230503
        public ActionResult GetEngLatLng(int id)
        {
            List<EngLatLngVModel> engs = emdAuditService.GetEngLatLng<EngLatLngVModel>(id);
            if (engs.Count == 1)
            {
                return Json(new
                {
                    result = 0,
                    item = engs[0]
                });
            } else
            {
                return Json(new
                {
                    result = -1,
                    msg = "工程資料錯誤"
                });
            }
        }

        #region === 材料送審管制清單 ===
        public ActionResult EMDAuditList()
        {
            ViewBag.Title = "材料送審管制";
            return View("EMDAuditList", "_LayoutFrontDesk");
        }
        //取得使用者單位資訊
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
        //工程年分
        public JsonResult GetYearOptions()
        {
            List<EngYearVModel> years = emdAuditService.GetEngYearList();

            return Json(years);
        }
        //依年分取執行機關
        public JsonResult GetUnitOptions(string year)
        {
            List<EngExecUnitsVModel> years = emdAuditService.GetEngExecUnitList(year);
            return Json(years);
        }
        //依年分,機關取執行單位
        public JsonResult GetSubUnitOptions(string year, int parentSeq)
        {
            List<EngExecUnitsVModel> items = emdAuditService.GetEngExecSubUnitList(year, parentSeq);
            EngExecUnitsVModel m = new EngExecUnitsVModel();
            m.UnitSeq = -1;
            m.UnitName = "全部單位";
            items.Insert(0, m);
            return Json(items);
        }
        //依年分,機關執行單位取工程
        public JsonResult GetEngNameOptions(string year, int unit, int subUnit, int engMain)
        {
            List<EngNameOptionsVModel> items = new List<EngNameOptionsVModel>();
            int total = emdAuditService.GetEngListCount(year, unit, subUnit, engMain);
            if (total > 0)
            {
                items = emdAuditService.GetEngCreatedList<EngNameOptionsVModel>(year, unit, subUnit, engMain, 9999, 1);
                items.Sort((x, y) => x.CompareTo(y));
                EngNameOptionsVModel m = new EngNameOptionsVModel();
                m.Seq = -1;
                m.EngName = "全部工程";
                items.Insert(0, m);
            }
            return Json(items);
        }

        //工程清單
        public JsonResult GetList(string year, int unit, int subUnit, int engMain, int pageRecordCount, int pageIndex)
        {
            List<EngMainVModel> engList = new List<EngMainVModel>();
            List<EngNameOptionsVModel> engNames = new List<EngNameOptionsVModel>();
            int total = emdAuditService.GetEngListCount(year, unit, subUnit, engMain);
            if (total > 0)
            {
                engList = emdAuditService.GetEngList(year, unit, subUnit, engMain, pageRecordCount, pageIndex);
                engNames = emdAuditService.GetEngCreatedList<EngNameOptionsVModel>(year, unit, subUnit, engMain, 9999, 1);
                engNames.Sort((x, y) => x.CompareTo(y));
                EngNameOptionsVModel m = new EngNameOptionsVModel();
                m.Seq = -1;
                m.EngName = "全部工程";
                engNames.Insert(0, m);
            }
            return Json(new
            {
                pTotal = total,
                items = engList,
                engNameItems = engNames
            });
        }

        #endregion

        public ActionResult EMDAuditEdit()
        {
            Utils.setUserClass(this);
            ViewBag.Title = "材料送審管制";
            return View("EMDAuditEdit"); //shioulo 20220519
            //return View("EMDAuditEdit", "_LayoutFrontDesk");
        }        
        
        //工程基本資料
        public JsonResult GetEngItem(int id)
        {
            List<EngMainVModel> items = emdAuditService.GetItemBySeq<EngMainVModel>(id);
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
        //材料設備送審管制總表
        public JsonResult GetEMDSummaryList(int id, int pageIndex, int perPage)
        {
            List<EngMaterialDeviceSummaryVModel> EMDSummaryList = new List<EngMaterialDeviceSummaryVModel>();
            int total = emdAuditService.GetEMDSummaryListCount(id);
            if (total > 0)
            {
                EMDSummaryList = emdAuditService.GetEMDSummaryList<EngMaterialDeviceSummaryVModel>(id, pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = EMDSummaryList
            });
        }

        //材料設備檢(試)驗管制總表
        public JsonResult GetEMDTestSummaryList(int id, int pageIndex, int perPage)
        {
            List<EngMaterialDeviceTestSummaryVModel> EMDTestSummaryList = new List<EngMaterialDeviceTestSummaryVModel>();
            int total = emdAuditService.GetEMDTestSummaryListCount(id);
            if (total > 0)
            {
                EMDTestSummaryList = emdAuditService.GetEMDTestSummaryList<EngMaterialDeviceTestSummaryVModel>(id, pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = EMDTestSummaryList
            });
        }
        //s20230308
        public JsonResult AddEMDSummary(EngMaterialDeviceSummaryVModel item)
        {
            if (emdAuditService.CopyEMDSummary(item))
            {
                return Json(new
                {
                    result = 0,
                    message = "新增完成",
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "新增失敗"
                });
            }
        }
        //s20230308
        public JsonResult DelEMDSummary(int id)
        {
            if (emdAuditService.DelEMDSummary(id))
            {
                return Json(new
                {
                    result = 0,
                    message = "刪除完成",
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "刪除失敗"
                });
            }
        }
        public JsonResult UpdateEMDSummary(EngMaterialDeviceSummaryVModel item)
        {
            if (emdAuditService.UpdateEMDSummary(item))
            {
                string QRCodeImageURL = emdAuditService.GetEMDSummaryListBySeq(item.Seq);
                return Json(new
                {
                    qrcodeImageURL = QRCodeImageURL,
                    result = 0,
                    message = "資料儲存完成",
                });
            }
            else
            {
                return Json(new
                {
                    qrcodeImageURL = "",
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
        }

        public JsonResult UpdateEMDSummary_1(EngMaterialDeviceSummaryVModel item)
        {
            if (emdAuditService.UpdateEMDSummary_1(item))
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成",
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
        }

        public JsonResult UpdateEMDSummary_2(EngMaterialDeviceSummaryVModel item)
        {
            if (emdAuditService.UpdateEMDSummary_2(item))
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成",
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
        }
        //s20230308
        public JsonResult AddEMDTestSummary(EngMaterialDeviceTestSummaryVModel item)
        {
            if (emdAuditService.CopyEMDTestSummary(item))
            {
                return Json(new
                {
                    result = 0,
                    message = "新增完成",
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "新增失敗"
                });
            }
        }
        //s20230308
        public JsonResult DelEMDTestSummary(int id)
        {
            if (emdAuditService.DelEMDTestSummary(id))
            {
                return Json(new
                {
                    result = 0,
                    message = "刪除完成",
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "刪除失敗"
                });
            }
        }
        public JsonResult UpdateEMDTestSummary(EngMaterialDeviceTestSummaryVModel item)
        {
            if (emdAuditService.UpdateEMDTestSummary(item))
            {
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成",
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
        }

        //已上傳送審檔案
        public JsonResult GetUploadAuditFileList(int seq, int fileType)
        {
            List<UploadAuditFileVModel> itemList = emdAuditService.UploadAuditFileListBySeq<UploadAuditFileVModel>(seq, fileType);
            return Json(new
            {
                pTotal = itemList.Count,
                items = itemList
            });
        }
        //上傳送審檔案 FileType:檔案種類(1:型錄, 2:相關試驗報告, 3:樣品)
        public JsonResult Upload_FileType(int engMain, int EMDSummarySeq, int fileType)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                UploadAuditFileVModel model = new UploadAuditFileVModel();
                model.EngMainSeq = engMain;
                model.EngMaterialDeviceSummarySeq = EMDSummarySeq;
                model.FileType = fileType;
                try
                {
                    if (SaveFile(file, model, "Audit-"))
                    {
                        EMDAuditService service = new EMDAuditService();
                        if (service.AddUploadAuditFile(model) == 0)
                        {
                            return Json(new
                            {
                                result = -1,
                                message = "新增失敗"
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                result = 0,
                                message = "新增成功",
                            });

                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                }
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }
        private bool SaveFile(HttpPostedFileBase file, UploadAuditFileVModel m, string fileHeader)
        {
            try
            {
                string filePath = Utils.GetEngMainFolder(m.EngMainSeq);// GetFlowChartPath();
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                m.OriginFileName = file.FileName.ToString().Trim();

                int inx = m.OriginFileName.LastIndexOf(".");
                m.UniqueFileName = String.Format("{0}{1}{2}", fileHeader, Guid.NewGuid(), m.OriginFileName.Substring(inx));
                string fullPath = Path.Combine(filePath, m.UniqueFileName);
                file.SaveAs(fullPath);

                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                return false;
            }
        }
        //
        //已上傳送審結果檔案 s20230626
        public JsonResult GetUploadAuditFileResultList(int seq, int fileType)
        {
            List<UploadAuditFileResultVModel> itemList = emdAuditService.GetAuditFileResultList<UploadAuditFileResultVModel>(seq, fileType);
            return Json(new
            {
                items = itemList
            });
        }
        //上傳送審結果檔案 FileType:檔案種類(1:相關試驗報告)
        public JsonResult UploadResult_FileType(int engMain, int id, int fileType)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                UploadAuditFileVModel model = new UploadAuditFileVModel();
                model.EngMainSeq = engMain;
                model.EngMaterialDeviceSummarySeq = id;
                model.FileType = fileType;
                try
                {
                    if (SaveFile(file, model, "AuditResult-"))
                    {
                        EMDAuditService service = new EMDAuditService();
                        if (service.AddUploadAuditFileResult(model) == 0)
                        {
                            return Json(new
                            {
                                result = -1,
                                message = "新增失敗"
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                result = 0,
                                message = "新增成功",
                            });

                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                }
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }
        //已上傳履歷
        public JsonResult GetResumeList(int seq)
        {
            List<ResumeVModel> sesumeList = new EMDAuditService().ResumeListBySeq<ResumeVModel>(seq);
            return Json(new
            {
                pTotal = sesumeList.Count,
                items = sesumeList
            });
        }
        //上傳履歷-網址轉QRCode
        public JsonResult UpdateResume_1(int engMain, int EMDSummarySeq,string ResumeUrl)
        {
            ResumeVModel model = new ResumeVModel();
            model.EngMaterialDeviceSummarySeq = EMDSummarySeq;
            model.ResumeType = 1;
            //model.ResumeUrl = ResumeUrl;
            model.OriginFileName = "";
            model.UniqueFileName = "";

            #region 產生QRCode
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = 174,
                    Width = 174,
                }
            };

            string filePath = Utils.GetEngMainFolder(engMain);// GetFlowChartPath();
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            model.OriginFileName = String.Format("{0}{1}.jpg", "Resume-", Guid.NewGuid());
            model.UniqueFileName = model.OriginFileName;
            model.ResumeUrl = model.OriginFileName;
            string fullPath = Path.Combine(filePath, model.UniqueFileName);

            //產生QRcode
            var img = writer.Write(ResumeUrl);
            string FileName = "ithome";
            Bitmap myBitmap = new Bitmap(img);

            //string filePath = string.Format("D:\\{0}.jpg", FileName);
            ViewBag.filePath = fullPath;

            myBitmap.Save(fullPath, System.Drawing.Imaging.ImageFormat.Jpeg);
            #endregion 

            if (emdAuditService.AddResume(model) == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "新增失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    message = "新增成功",
                });
            }
        }
        //上傳 履歷
        public JsonResult Upload_Resume(int engMain, int EMDSummarySeq, int resumeType)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                ResumeVModel model = new ResumeVModel();
                model.EngMainSeq = engMain;
                model.EngMaterialDeviceSummarySeq = EMDSummarySeq;
                model.ResumeType = resumeType;
                model.ResumeUrl = "";
                try
                {
                    if (SaveFile_Resume(file, model, "Resume-"))
                    {
                        EMDAuditService service = new EMDAuditService();
                        if (service.AddResume(model) == 0)
                        {
                            return Json(new
                            {
                                result = -1,
                                message = "新增失敗"
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                result = 0,
                                message = "新增成功",
                            });
                        }
                    }
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                }
            }

            return Json(new
            {
                result = -1,
                message = "上傳檔案失敗"
            });
        }
        private bool SaveFile_Resume(HttpPostedFileBase file, ResumeVModel m, string fileHeader)
        {
            try
            {
                string filePath = Utils.GetEngMainFolder(m.EngMainSeq);// GetFlowChartPath();
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                m.OriginFileName = file.FileName.ToString().Trim();

                int inx = m.OriginFileName.LastIndexOf(".");
                m.UniqueFileName = String.Format("{0}{1}{2}", fileHeader, Guid.NewGuid(), m.OriginFileName.Substring(inx));
                string fullPath = Path.Combine(filePath, m.UniqueFileName);
                file.SaveAs(fullPath);

                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                return false;
            }
        }
        //刪除 送審檔案
        public JsonResult DelUploadAuditFile(int seq)
        {
            if (emdAuditService.DelUploadAuditFile(seq) == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "刪除失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    message = "刪除成功",
                });

            }
        }
        //刪除 送審結果檔案 s20230626
        public JsonResult DelUploadAuditFileResult(int seq, int eId)
        {
            List<UploadAuditFileResultModel> items = emdAuditService.GetUploadAuditFileResultItem(seq);
            if(items.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "刪除失敗"
                });
            }
            if (emdAuditService.DelUploadAuditFileResult(seq) == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "刪除失敗"
                });
            }
            else
            {
                UploadAuditFileResultModel m = items[0];
                string uniqueFileName = m.UniqueFileName;
                if (uniqueFileName != null && uniqueFileName.Length > 0)
                {
                    string filePath = Utils.GetEngMainFolder(eId);
                    string fullPath = Path.Combine(filePath, uniqueFileName);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                return Json(new
                {
                    result = 0,
                    message = "刪除成功",
                });

            }
        }
        //刪除 履歷
        public JsonResult DelResume(int Seq)
        {
            if (emdAuditService.DelResume(Seq) == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "刪除失敗"
                });
            }
            else
            {
                return Json(new
                {
                    result = 0,
                    message = "刪除成功",
                });

            }
        }
        //下載
        public ActionResult AuditFileDownload(int seq)
        {
            List<UploadAuditFileVModel> items = emdAuditService.GetUploadAuditFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].UniqueFileName == null || items[0].UniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            return DownloadAuditFile(items[0]);
        }
        private ActionResult DownloadAuditFile(UploadAuditFileVModel m)
        {
            string filePath = Utils.GetEngMainFolder(m.EngMainSeq);

            string uniqueFileName = m.UniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
                if (System.IO.File.Exists(fullPath))
                {
                    Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", m.OriginFileName);
                }
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }
        //下載 s20230626
        public ActionResult AuditFileResultDownload(int seq, int eId)
        {
            List<UploadAuditFileResultModel> items = emdAuditService.GetUploadAuditFileResultItem(seq);
            if (items.Count == 0 || items[0].UniqueFileName == null || items[0].UniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            string filePath = Utils.GetEngMainFolder(eId);

            UploadAuditFileResultModel m = items[0];
            string uniqueFileName = m.UniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
                if (System.IO.File.Exists(fullPath))
                {
                    Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", m.OriginFileName);
                }
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }
        //下載
        public ActionResult ResumeFileDownload(int seq)
        {
            List<ResumeVModel> items = emdAuditService.GetResumeInfoBySeq(seq);
            if (items.Count == 0 || items[0].UniqueFileName == null || items[0].UniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            return DownloadAuditFile(items[0]);
        }

        private ActionResult DownloadAuditFile(ResumeVModel m)
        {
            string filePath = Utils.GetEngMainFolder(m.EngMainSeq);

            string uniqueFileName = m.UniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
                if (System.IO.File.Exists(fullPath))
                {
                    Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", m.OriginFileName);
                }
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }

        #region G-Word
        public ActionResult DownloadReport(int engMain, DownloadArgExtension downloadArg = null)
        {
            try
            {
                string uuid = Guid.NewGuid().ToString("B").ToUpper();
                string folder = Path.Combine(Path.GetTempPath(), uuid);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                string engNo;
                using(var context = new EQC_NEW_Entities())
                {
                    engNo = context.EngMain.Find(engMain)?.EngNo;
                }
                List<EngMaterialDeviceSummaryVModel> items51 = emdAuditService.GetEMDSummaryListByEngMainSeq<EngMaterialDeviceSummaryVModel>(engMain);
                if (items51.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "沒有材料設備送審管制總表資料"
                    }, JsonRequestBehavior.AllowGet);
                }

                List<EngMaterialDeviceTestSummaryVModel> items52 = emdAuditService.GetEMDTestSummaryListByEngMainSeq<EngMaterialDeviceTestSummaryVModel>(engMain);
                if (items52.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "沒有材料設備檢(試)驗管制總表"
                    }, JsonRequestBehavior.AllowGet);
                }

                string dName = engNo + "-材料送審管制.zip";
                string outputFilePath = "";
                outputFilePath  =Report51(engMain, items51, folder);
                downloadArg?.targetPathSetting(outputFilePath);
                outputFilePath = Report52(engMain, items52, folder);
                downloadArg?.targetPathSetting(outputFilePath);

                if(downloadArg?.DistFilePath == null)
                {
                    string path = Path.Combine(Path.GetTempPath(), uuid);
                    string zipFile = Path.Combine(Path.GetTempPath(), uuid + ".zip");
                    ZipFile.CreateFromDirectory(path, zipFile);// AddFiles(files, "ProjectX");

                    Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);

                    return File(iStream, "application/blob", dName);
                }
                else
                {
                    return null;
                }

            }
            catch (Exception e)
            {
                return Json(new
                {
                    result = -1,
                    message = "產製錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public string Report51(int engMain, List<EngMaterialDeviceSummaryVModel> items, string folder)
        {
            string tempFile = CopyTemplateFile("表5-1材料設備送審管制總表.docx");
            DateTime dt = DateTime.Now;
            string fn = String.Format("{0}-{1}{2:00}{3:00}-{4}.docx", "表5-1材料設備送審管制總表", dt.Year - 1911, dt.Month, dt.Day, engMain);
            string outFile = Path.Combine(folder, fn);

            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            XWPFTable table = doc.Tables[0];
            //插入 row
            for (int i = 1; i < (items.Count * 2) - 1; i++)
            {
                Utils.insertRow(table, 3, 4);
            }
            int rowInx = 0;
            int iCount = 1;
            foreach (EngMaterialDeviceSummaryVModel m in items)
            {
                new BaseService().Null2Empty(m);
                table.GetRow(3 + rowInx).GetCell(0).SetText((iCount).ToString());
                table.GetRow(3 + rowInx).GetCell(1).SetText(m.ItemNo);
                table.GetRow(3 + rowInx).GetCell(2).SetText(m.ContactQty.ToString());
                table.GetRow(3 + rowInx).GetCell(3).SetText(m.IsSampleTest ? "是" : "否");
                table.GetRow(3 + rowInx).GetCell(4).SetText(m.chsSchAuditDate);
                table.GetRow(3 + rowInx).GetCell(5).SetText(m.IsFactoryInsp ? "是" : "否");
                table.GetRow(3 + rowInx).GetCell(6).SetText(m.IsAuditVendor ? "V" : "");
                table.GetRow(3 + rowInx).GetCell(7).SetText(m.IsAuditCatalog ? "V" : "");
                table.GetRow(3 + rowInx).GetCell(8).SetText(m.IsAuditReport ? "V" : "");
                table.GetRow(3 + rowInx).GetCell(9).SetText(m.IsAuditSample ? "V" : "");
                table.GetRow(3 + rowInx).GetCell(10).SetText(m.OtherAudit);
                table.GetRow(3 + rowInx).GetCell(11).SetText(m.chsAuditDate);
                table.GetRow(3 + rowInx).GetCell(12).SetText(m.ArchiveNo);

                table.GetRow(4 + rowInx).GetCell(1).SetText(m.MDName);
                table.GetRow(4 + rowInx).GetCell(4).SetText(m.chsRealAutitDate);
                table.GetRow(4 + rowInx).GetCell(5).SetText(m.chsFactoryInspDate);
                if (m.AuditResult == 1)
                    table.GetRow(4 + rowInx).GetCell(11).SetText("通過");
                if (m.AuditResult == 2)
                    table.GetRow(4 + rowInx).GetCell(11).SetText("否決");

                Utils.rowMergeStart(table.GetRow(3 + rowInx).GetCell(0));
                Utils.rowMergeContinue(table.GetRow(4 + rowInx).GetCell(0));

                Utils.rowMergeStart(table.GetRow(3 + rowInx).GetCell(2));
                Utils.rowMergeContinue(table.GetRow(4 + rowInx).GetCell(2));

                Utils.rowMergeStart(table.GetRow(3 + rowInx).GetCell(3));
                Utils.rowMergeContinue(table.GetRow(4 + rowInx).GetCell(3));

                Utils.rowMergeStart(table.GetRow(3 + rowInx).GetCell(6));
                Utils.rowMergeContinue(table.GetRow(4 + rowInx).GetCell(6));

                Utils.rowMergeStart(table.GetRow(3 + rowInx).GetCell(7));
                Utils.rowMergeContinue(table.GetRow(4 + rowInx).GetCell(7));

                Utils.rowMergeStart(table.GetRow(3 + rowInx).GetCell(8));
                Utils.rowMergeContinue(table.GetRow(4 + rowInx).GetCell(8));

                Utils.rowMergeStart(table.GetRow(3 + rowInx).GetCell(9));
                Utils.rowMergeContinue(table.GetRow(4 + rowInx).GetCell(9));

                Utils.rowMergeStart(table.GetRow(3 + rowInx).GetCell(10));
                Utils.rowMergeContinue(table.GetRow(4 + rowInx).GetCell(10));

                Utils.rowMergeStart(table.GetRow(3 + rowInx).GetCell(12));
                Utils.rowMergeContinue(table.GetRow(4 + rowInx).GetCell(12));

                rowInx++;
                rowInx++;
                iCount++;
            }

            doc.Write(fsOut);
            fsOut.Close();
            doc.Close();
            fs.Close();
            return outFile;
        }

        public string Report52(int engMain, List<EngMaterialDeviceTestSummaryVModel> items, string folder)
        {
            string tempFile = CopyTemplateFile("表5-2材料設備檢(試)驗管制總表.docx");
            DateTime dt = DateTime.Now;
            string fn = String.Format("{0}-{1}{2:00}{3:00}-{4}.docx", "表5-2材料設備檢(試)驗管制總表", dt.Year - 1911, dt.Month, dt.Day, engMain);
            string outFile = Path.Combine(folder, fn);

            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            XWPFTable table = doc.Tables[0];
            //插入 row
            for (int i = 1; i < (items.Count * 2) - 1; i++)
            {
                Utils.insertRow(table, 2, 3);
            }
            int rowInx = 0;
            int iCount = 1;
            foreach (EngMaterialDeviceTestSummaryVModel m in items)
            {
                new BaseService().Null2Empty(m);
                table.GetRow(2 + rowInx).GetCell(0).SetText((iCount).ToString());
                table.GetRow(2 + rowInx).GetCell(1).SetText(m.ItemNo);
                table.GetRow(2 + rowInx).GetCell(2).SetText(m.chsSchTestDate);
                table.GetRow(2 + rowInx).GetCell(3).SetText(m.TestQty.ToString());
                table.GetRow(2 + rowInx).GetCell(4).SetText(m.chsSampleDate);
                table.GetRow(2 + rowInx).GetCell(5).SetText(m.SampleFeq);
                table.GetRow(2 + rowInx).GetCell(6).SetText(m.AccTestQty.ToString());
                if (m.TestResult == 1)
                    table.GetRow(2 + rowInx).GetCell(7).SetText("通過");
                if (m.TestResult == 2)
                    table.GetRow(2 + rowInx).GetCell(7).SetText("否決");
                table.GetRow(2 + rowInx).GetCell(8).SetText(m.Coworkers);
                table.GetRow(2 + rowInx).GetCell(9).SetText(m.ArchiveNo);

                table.GetRow(3 + rowInx).GetCell(1).SetText(m.MDName);
                table.GetRow(3 + rowInx).GetCell(2).SetText(m.chsRealTestDate);
                table.GetRow(3 + rowInx).GetCell(4).SetText(m.SampleQty.ToString());
                table.GetRow(3 + rowInx).GetCell(6).SetText(m.AccSampleQty.ToString());

                Utils.rowMergeStart(table.GetRow(2 + rowInx).GetCell(0));
                Utils.rowMergeContinue(table.GetRow(3 + rowInx).GetCell(0));

                Utils.rowMergeStart(table.GetRow(2 + rowInx).GetCell(3));
                Utils.rowMergeContinue(table.GetRow(3 + rowInx).GetCell(3));

                Utils.rowMergeStart(table.GetRow(2 + rowInx).GetCell(5));
                Utils.rowMergeContinue(table.GetRow(3 + rowInx).GetCell(5));

                Utils.rowMergeStart(table.GetRow(2 + rowInx).GetCell(7));
                Utils.rowMergeContinue(table.GetRow(3 + rowInx).GetCell(7));

                Utils.rowMergeStart(table.GetRow(2 + rowInx).GetCell(8));
                Utils.rowMergeContinue(table.GetRow(3 + rowInx).GetCell(8));

                Utils.rowMergeStart(table.GetRow(2 + rowInx).GetCell(9));
                Utils.rowMergeContinue(table.GetRow(3 + rowInx).GetCell(9));

                rowInx++;
                rowInx++;
                iCount++;
            }

            doc.Write(fsOut);
            fsOut.Close();
            doc.Close();
            fs.Close();
            return outFile;
        }

        private string CopyTemplateFile(string filename)
        {
            string tempFile = GetTempFile();
            string srcFile = Path.Combine(Utils.GetTemplateFilePath(), filename);
            System.IO.File.Copy(srcFile, tempFile);
            return tempFile;
        }

        private string GetTempFile()
        {
            string uuid = Guid.NewGuid().ToString("B").ToUpper();
            string tempPath = Path.GetTempPath();
            return Path.Combine(tempPath, uuid + ".docx");
        }

        #endregion 

    }
}