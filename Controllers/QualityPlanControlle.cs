using EQC.Common;
using EQC.Models;
using EQC.Services;
using EQC.ProposalV2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace EQC.Controllers
{
    [SessionFilter]
    public class QualityPlanController : Controller
    {
        private QualityProjectListService qualityProjectListService = new QualityProjectListService();

        public ActionResult Index()
        {
            Utils.setUserClass(this, 2);
            //ViewBag.Title = "品質計畫";
            return View();
        }
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

        //標案年分
        public JsonResult GetYearOptions()
        {
            List<EngYearVModel> years = qualityProjectListService.GetEngYearList();
            return Json(years);
        }
        //依年分取執行機關
        public JsonResult GetUnitOptions(string year)
        {
            List<EngExecUnitsVModel> items = qualityProjectListService.GetEngExecUnitList(year);
            return Json(items);
        }
        //依年分,機關取執行單位
        public JsonResult GetSubUnitOptions(string year, int parentSeq)
        {
            List<EngExecUnitsVModel> items = qualityProjectListService.GetEngExecSubUnitList(year, parentSeq);
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
            int total = qualityProjectListService.GetEngCreatedListCount(year, unit, subUnit, engMain);
            if (total > 0)
            {
                items = qualityProjectListService.GetEngCreatedList<EngNameOptionsVModel>(year, unit, subUnit, engMain, 9999, 1);
                items.Sort((x, y) => x.CompareTo(y));
                EngNameOptionsVModel m = new EngNameOptionsVModel();
                m.Seq = -1;
                m.EngName = "全部工程";
                items.Insert(0, m);
            }
            return Json(items);
        }

        //工程清單

        //public JsonResult GetList(string year, int unit, int subUnit, int engMain, int pageRecordCount, int pageIndex)
        //{
        //    List<EngMainVModel> engList = new List<EngMainVModel>();
        //    List<EngNameOptionsVModel> engNames = new List<EngNameOptionsVModel>();
        //    int total = qualityProjectListService.GetEngCreatedListCount(year, unit, subUnit, engMain);
        //    if (total > 0)
        //    {
        //        engList = qualityProjectListService.GetEngCreatedList<EngMainVModel>(year, unit, subUnit, engMain, pageRecordCount, pageIndex);
        //        engNames = qualityProjectListService.GetEngCreatedList<EngNameOptionsVModel>(year, unit, subUnit, engMain, 9999, 1);
        //        engNames.Sort((x, y) => x.CompareTo(y));
        //        EngNameOptionsVModel m = new EngNameOptionsVModel();
        //        m.Seq = -1;
        //        m.EngName = "全部工程";
        //        engNames.Insert(0, m);
        //    }
        //    return Json(new
        //    {
        //        pTotal = total,
        //        items = engList,
        //        engNameItems = engNames
        //    });
        //}
        public JsonResult GetEng(int engMain)
        {
            List<EngMainVModel> engList = new List<EngMainVModel>();
            List<EngNameOptionsVModel> engNames = new List<EngNameOptionsVModel>();
            engList = qualityProjectListService.getEng(engMain);

            return Json(new {
            
                items = engList
            });
        }

        //已上傳計畫書 版次清單 
        public JsonResult RevisionList(int seq)
        {
            List<QualityProjectListVModel> items = qualityProjectListService.ListByEngMainSeq<QualityProjectListVModel>(seq);
            return Json(items);
        }
        //計畫書 新版次儲存
        public JsonResult RevisionSave(QualityProjectListVModel item)
        {
            int result = qualityProjectListService.UpdateName(item);
            if (result == 1)
            {
                List<QualityProjectListVModel> items = qualityProjectListService.GetItemBySeq<QualityProjectListVModel>(item.Seq);
                return Json(new
                {
                    result = 0,
                    message = "儲存成功",
                    item = items[0]
                });
            }
            else
            {
                return Json(new
                {
                    result = 1,
                    message = "儲存錯誤"
                });
            }
        }
        //解鎖品質計畫
        public JsonResult UnlockEng(SupervisionProjectListVModel item)
        {
            int result = qualityProjectListService.UpdateDocState(item.Seq, QualityProjectListService.docstate_Edit);
            if (result == 1)
            {
                List<QualityProjectListVModel> items = qualityProjectListService.GetLastItemByEngMain<QualityProjectListVModel>(item.Seq);
                return Json(new
                {
                    result = 0,
                    message = "解鎖成功",
                    item = items[0]
                });
            }
            else
            {
                return Json(new
                {
                    result = 1,
                    message = "該筆資料,解鎖失敗"
                });
            }
        }
        //計畫書 上傳
        public JsonResult RevisionUpload(int engMain, string name)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                QualityProjectListModel model = new QualityProjectListModel();
                model.EngMainSeq = engMain;
                model.Name = name;
                try
                {
                    if (SaveFile(file, model, "QP-"))
                    {
                        if (qualityProjectListService.AddRevision(model) == 0)
                        {
                            return Json(new
                            {
                                result = -1,
                                message = "新增計劃書失敗"
                            });
                        }
                        else
                        {
                            return Json(new
                            {
                                result = 0,
                                message = "新增計劃成功",
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
        private bool SaveFile(HttpPostedFileBase file, QualityProjectListModel m, string fileHeader)
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
        //計畫書 下載
        public ActionResult RevisionDownload(int seq, int type)
        {
            List<QualityProjectListModel> items = qualityProjectListService.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].UniqueFileName == null || items[0].UniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            switch (type)
            {
                case 1: return DownloadFile(items[0], "pdf");
                case 2: return DownloadFile(items[0], "odt");
                default: return DownloadFile(items[0], "docx");
            }

        }

        private ActionResult DownloadFile(QualityProjectListModel m, string ext)
        {
            string filePath = Utils.GetEngMainFolder(m.EngMainSeq);

            string uniqueFileName = m.UniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
                if (System.IO.File.Exists(fullPath))
                {
                    string pdfExportPath;
                    Stream iStream;
                    switch (ext)
                    {
                        case "pdf": iStream =fullPath.getFileStreamWithConvert(fullPath.CreatePDF, ext); break;
                        case "odt": iStream = fullPath.getFileStreamWithConvert(fullPath.CreateODT, ext); break;
                        default: iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read); break;
                    }
                    return File(iStream, "application/blob", Path.ChangeExtension(m.OriginFileName, "."+ext));
                }
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }

        //品質計畫範例
        public ActionResult DownloadQualityPlan(int seq, int type)
        {
            switch (type)
            {
                case 1: return DownloadQualityPlanExt(seq, "pdf");
                case 2: return DownloadQualityPlanExt(seq, "odt");
                default: return DownloadQualityPlanExt(seq, "docx");
            }

        }
        public ActionResult DownloadQualityPlanExt(int seq, string ext)
        {
            List<EngMainEditVModel> engItems = new EngMainService().GetItemBySeq<EngMainEditVModel>(seq);
            if (engItems.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "工程資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            string filePath = Utils.GetQualityPlanFolder(seq);
            if (!Directory.Exists(filePath))
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            string[] files = Directory.GetFiles(filePath, "*.docx");
            if (files.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            string fileName = "";
            foreach (string fName in files)
            {//取字串排序最大的檔案
                if (String.Compare(fName, fileName, StringComparison.InvariantCulture) > 0)
                {
                    fileName = fName;
                }
            }
            if (fileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            Stream iStream;
            switch (ext)
            {
                case "pdf": iStream = fileName.getFileStreamWithConvert(fileName.CreatePDF, ext); break;
                case "odt": iStream = fileName.getFileStreamWithConvert(fileName.CreateODT, ext); break;
                default: iStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read); break;
            }

            return File(iStream, "application/blob", engItems[0].EngNo + "_品質計畫書."+ext);
        }
        public ActionResult getFileStatus(int engMain)
        {
            string docFilePath = Utils.GetQualityPlanFolder(engMain);

            if(!Directory.Exists(docFilePath))
            {
                return Json(new
                {
                    result = 0,
                    fileStatuts = new
                    {
                        PdfExist = false,
                        OdtExist = false
                    }

                });
            }
            string[] docFiles = Directory.GetFiles(docFilePath, "*.docx");
            string[] pdfFiles = Directory.GetFiles(docFilePath, "*.pdf");
            string[] odtFiles = Directory.GetFiles(docFilePath, "*.odt");

            string lastestDocFile = Path.GetFileNameWithoutExtension(docFiles.OrderByDescending(row => row).FirstOrDefault());
            string lastestPdfFile = Path.GetFileNameWithoutExtension(pdfFiles.OrderByDescending(row => row).FirstOrDefault());
            string lastestOdtFile = Path.GetFileNameWithoutExtension(odtFiles.OrderByDescending(row => row).FirstOrDefault());
            return Json(new
            {
                result = 0,
                fileStatus = new
                {
                    PdfExist = lastestDocFile == lastestPdfFile,
                    OdtExist = lastestDocFile == lastestOdtFile
                }

            });
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

        //產製監造計畫書
        public JsonResult CreatePlan(int engMain)
        {
            List<QualityProjectListVModel> items = qualityProjectListService.ListByEngMainSeq<QualityProjectListVModel>(engMain);
            if (items.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "計畫書產製失敗,無範本"
                });
            }
            QualityProjectListService service = new QualityProjectListService();
            if (service.UpdateDocState(engMain, QualityProjectListService.docstate_PlanMaking) == 1)
            {
                Task.Run(async () => await Export2.ExportWord2(items[0].Seq, HttpContext));
                return Json(new
                {
                    result = 0,
                    state = QualityProjectListService.docstate_PlanMaking,
                    message = "背景產製中請稍後..."
                });
            }
            else
                return Json(new
                {
                    result = -1,
                    message = "計畫書產製失敗"
                });
        }
       
        //勾選與設置完成
        public JsonResult SettingComplete(int engMain)
        {
            QualityProjectListService service = new QualityProjectListService();
            if (service.SettingComplateDocState(engMain))
                return Json(new
                {
                    result = 0,
                    message = "設置完成"
                });
            else
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
      
      
        }
    } 
 }