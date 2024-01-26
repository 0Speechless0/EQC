using EQC.Common;
using EQC.EDMXModel;
using EQC.Models;
using EQC.ProposalV2;
using EQC.Services;
using EQC.ViewModel;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class SupervisionPlanController : Controller
    {
        private SupervisionProjectListService supervisionProjectListService = new SupervisionProjectListService();
        private EngMainService engMainService = new EngMainService();
        public ActionResult Index()
        {
            //ViewBag.Title = "監造計畫";
            Utils.setUserClass(this);
            return View("SupervisionPlanList");
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

        public ActionResult EditEng(int seq)
        {
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Edit", "SupervisionPlan");
            string tarUrl = redirectUrl.ToString() + "?id=" + seq;
            return Json(new { Url = tarUrl });
        }
        public ActionResult Edit()
        {
            Utils.setUserClass(this);
            //ViewBag.Title = "監造計畫-編輯";
            List<VMenu> menu = new List<VMenu>();
            var link = new UrlHelper(Request.RequestContext).Action("Index", "SupervisionPlan");
            menu.Add(new VMenu() { Name = "監造計畫", Url = link.ToString() });
            menu.Add(new VMenu() { Name = "監造計畫-編輯", Url = "" });
            ViewBag.breadcrumb = menu;

            return View("SupervisionPlanEdit");
        }

        public ActionResult QCEdit(string chapter, int engMain, int seq)
        {
            //ViewBag.Title = "抽查管理標準";
            List<VMenu> menu = new List<VMenu>();
            var link = new UrlHelper(Request.RequestContext).Action("Index", "SupervisionPlan");
            menu.Add(new VMenu() { Name = "監造計畫", Url = link.ToString() });
            link = new UrlHelper(Request.RequestContext).Action("Edit", "SupervisionPlan");
            menu.Add(new VMenu() { Name = "監造計畫-編輯", Url = link.ToString() + "?id=" + engMain });
            string chapterName = "";
            if (chapter == "703")
            {
                chapterName = "職業安全衛生標準-";
            }
            else if (chapter == "702")
            {
                chapterName = "環境保育標準-";
            }
            else if (chapter == "701")
            {
                chapterName = "施工抽查標準-";
            }
            else if (chapter == "6")
            {
                chapterName = "設備運轉測試標準-";
            }
            else if (chapter == "5")
            {
                chapterName = "材料設備送審管制標準-";
            }
            menu.Add(new VMenu() { Name = chapterName + "抽查管理標準", Url = "" });
            ViewBag.breadcrumb = menu;

            return View("QCStdEdit");
        }

        public ActionResult FlowChartEdit(string chapter, int engMain, int seq)
        {
            //ViewBag.Title = "抽查管理標準";
            List<VMenu> menu = new List<VMenu>();
            var link = new UrlHelper(Request.RequestContext).Action("Index", "SupervisionPlan");
            menu.Add(new VMenu() { Name = "監造計畫", Url = link.ToString() });
            link = new UrlHelper(Request.RequestContext).Action("Edit", "SupervisionPlan");
            menu.Add(new VMenu() { Name = "監造計畫-編輯", Url = link.ToString() + "?id=" + engMain });
            string chapterName = "";
            if (chapter == "703")
            {
                chapterName = "職業安全衛生標準-";
            }
            else if (chapter == "702")
            {
                chapterName = "環境保育標準-";
            }
            else if (chapter == "701")
            {
                chapterName = "施工抽查標準-";
            }
            else if (chapter == "6")
            {
                chapterName = "設備運轉測試標準-";
            }
            else if (chapter == "5")
            {
                chapterName = "材料設備送審管制標準-";
            }
            menu.Add(new VMenu() { Name = chapterName + "流程圖", Url = "" });
            ViewBag.breadcrumb = menu;

            return View("FlowChartEdit");
        }

        //s20230527
        public ActionResult SearchEng(string k)
        {
            List<EngMain1VModel> items = engMainService.SearchEngByEngNoOrName<EngMain1VModel>(k);
            return Json(items);
        }
        //標案年分
        public JsonResult GetYearOptions()
        {
            List<EngYearVModel> years = supervisionProjectListService.GetEngYearList();
            return Json(years);
        }
        //依年分取執行機關
        public JsonResult GetUnitOptions(string year)
        {
            List<EngExecUnitsVModel> items = supervisionProjectListService.GetEngExecUnitList(year);
            return Json(items);
        }
        //依年分,機關取執行單位
        public JsonResult GetSubUnitOptions(string year, int parentSeq)
        {
            List<EngExecUnitsVModel> items = supervisionProjectListService.GetEngExecSubUnitList(year, parentSeq);
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
            int total = supervisionProjectListService.GetEngCreatedListCount(year, unit, subUnit, engMain);
            if (total > 0)
            {
                items = supervisionProjectListService.GetEngCreatedList<EngNameOptionsVModel>(year, unit, subUnit, engMain, 9999, 1);
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
            int total = supervisionProjectListService.GetEngCreatedListCount(year, unit, subUnit, engMain);
            if (total > 0)
            {
                engList = supervisionProjectListService.GetEngCreatedList<EngMainVModel>(year, unit, subUnit, engMain, pageRecordCount, pageIndex);
                engNames = supervisionProjectListService.GetEngCreatedList<EngNameOptionsVModel>(year, unit, subUnit, engMain, 9999, 1);
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

        //已上傳計畫書 版次清單 
        public JsonResult RevisionList(int seq)
        {
            List<EngUploadHistoryVModel> items = new EngUploadHistoryService().ListByEngMainSeq<EngUploadHistoryVModel>(seq);
            return Json(items);
        }
        //計畫書 新版次儲存
        public JsonResult RevisionSave(EngUploadHistoryVModel item)
        {
            EngUploadHistoryService service = new EngUploadHistoryService();
            int result = service.UpdateName(item);
            if (result == 1)
            {
                List<EngUploadHistoryVModel> items = service.GetItemBySeq<EngUploadHistoryVModel>(item.Seq);
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
        //定稿計畫書 上傳
        public JsonResult RevisionUpload(int engMain, string m, string aNo)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                EngUploadHistoryModel model = new EngUploadHistoryModel();
                model.EngMainSeq = engMain;
                model.Memo = m;
                model.ApproveNo = aNo;
                try
                {
                    if (SaveFile(file, model, "SP-"))
                    {
                        if (new EngUploadHistoryService().AddRevision(model) == 0)
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
        private bool SaveFile(HttpPostedFileBase file, EngUploadHistoryModel m, string fileHeader)
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
        //定稿計畫書 下載
        public ActionResult RevisionDownload(int seq)
        {
            EngUploadHistoryService service = new EngUploadHistoryService();
            List<EngUploadHistoryModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].UniqueFileName == null || items[0].UniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            return DownloadFile(items[0]);
        }
        private ActionResult DownloadFile(EngUploadHistoryModel m)
        {
            string filePath = Utils.GetEngMainFolder(m.EngMainSeq);

            string uniqueFileName = m.UniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
                if (System.IO.File.Exists(fullPath))
                {
                    string exoprtPdfPath = fullPath;
                    if (!fullPath.ToLower().Contains(".pdf")) //s20230524
                        exoprtPdfPath = fullPath.CreatePDF();

                    Stream iStream = new FileStream(exoprtPdfPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", Path.ChangeExtension(m.OriginFileName, ".pdf"));
                }
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }

        //解鎖工程
        public JsonResult UnlockEng(SupervisionProjectListVModel item)
        {
            int result = supervisionProjectListService.UpdateDocState(item.Seq, SupervisionProjectListService.docstate_Edit);
            if (result == 1)
            {
                List<SupervisionProjectListVModel> items = supervisionProjectListService.GetLastItemByEngMain<SupervisionProjectListVModel>(item.Seq);
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
        public ActionResult getFileStatus(int engMain)
        {
            string docFilePath = Utils.GetSupervisionPlanFolder(engMain);
            if (!Directory.Exists(docFilePath))
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

            string lastestDocFile = Path.GetFileNameWithoutExtension(docFiles.OrderByDescending(row => row).FirstOrDefault() );
            string lastestPdfFile = Path.GetFileNameWithoutExtension(pdfFiles.OrderByDescending(row => row).FirstOrDefault() );
            string lastestOdtFile =Path.GetFileNameWithoutExtension(odtFiles.OrderByDescending(row => row).FirstOrDefault());
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
        //監造計畫書 下載
        public ActionResult PlanDownload(int seq, int type)
        {
            try
            {
                switch (type)
                {
                    case 1: return PlanDownloadExt(seq, "pdf");
                    case 2: return PlanDownloadExt(seq, "odt");
                    default: return PlanDownloadExt(seq, "docx");
                }
            }
            catch(Exception e)
            {
                BaseService.log.Info("PlanDownload Err: " + e.Message);
            }
            return null;
        
        }
        public ActionResult PlanDownloadExt(int seq, string ext)
        {
            List<EngMainEditVModel> items = engMainService.GetItemBySeq<EngMainEditVModel>(seq);
            if (items.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }

            string filePath = Utils.GetSupervisionPlanFolder(seq);
            if (!Directory.Exists(filePath))
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                }, JsonRequestBehavior.AllowGet);
            }
            string[] files = Directory.GetFiles(filePath, "*.docx");

            if (files.Length == 0 )
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
            switch(ext)
            {
                case "pdf": iStream = fileName.getFileStreamWithConvert(fileName.CreatePDF, ext);break;
                case "odt": iStream = fileName.getFileStreamWithConvert(fileName.CreateODT, ext);break;
                default : iStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);break;
            }

            return File(iStream, "application/blob", items[0].EngNo + "_監造計畫書."+ext);
        }



        //for edit ===========================================================
        public JsonResult GetEngItem(int id)
        {
            List<EngMainEditVModel> items = engMainService.GetItemBySeq<EngMainEditVModel>(id);
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

        //第五章 材料設備清冊
        public JsonResult Chapter5(int engMain, int pageIndex, int perPage)
        {
            List<EngMaterialDeviceListVModel> result = new List<EngMaterialDeviceListVModel>();
            EngMaterialDeviceListService service = new EngMaterialDeviceListService();
            int total = service.GetListCount(engMain);
            if (total > 0)
            {
                result = service.GetList<EngMaterialDeviceListVModel>(engMain, pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        public JsonResult Chapter5Item(int seq)
        {
            List<EngMaterialDeviceListVModel> result = new List<EngMaterialDeviceListVModel>();
            EngMaterialDeviceListService service = new EngMaterialDeviceListService();
            result = service.GetItemBySeq<EngMaterialDeviceListVModel>(seq);
            if (result.Count > 0)
                return Json(result[0]);
            else
                return Json(new { });
        }

        public JsonResult Chapter5NewItem(int engMain)
        {
            EngMaterialDeviceListService service = new EngMaterialDeviceListService();
            EngMaterialDeviceListModel item = new EngMaterialDeviceListModel();
            item.EngMainSeq = engMain;
            item.MDName = "未輸入";
            item.DataKeep = true;
            item.DataType = 1;//使用者新增
  
            if (service.Add(item))
            {
                List<EngMaterialDeviceListVModel> items = service.GetItemBySeq<EngMaterialDeviceListVModel>(item.Seq);
                return Json(new
                {
                    result = 0,
                    message = "新增完成",
                    item = Json(items[0])
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

        public JsonResult Chapter5Save(EngMaterialDeviceListVModel item)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + item.MDTestItem);
            EngMaterialDeviceListService service = new EngMaterialDeviceListService();
            if (service.Update(item))
            {
                //List<EngMaterialDeviceListVModel> items = service.GetItemBySeq<EngMaterialDeviceListVModel>(item.Seq);
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成",
                    //item = Json(items[0])
                });
            }
            else
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗，或資料不可變更"
                });
            }
        }
        public JsonResult Chapter5SaveKeep(List<EngMaterialDeviceListVModel> items)
        {
            EngMaterialDeviceListService service = new EngMaterialDeviceListService();
            if (service.UpdateKeep(items))
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

        public JsonResult Chapter5Del(int seq)
        {
            EngMaterialDeviceListService service = new EngMaterialDeviceListService();
            FlowChartDiagramService fServuce = new FlowChartDiagramService();
            fServuce.DeleteFlowChart(seq, "Chapter5Addition");
            List<EngMaterialDeviceListVModel> items = service.GetItemFileInfoBySeq(seq);
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
                if (!service.Delete(seq))
                {
                    return Json(new
                    {
                        result = -1,
                        message = "資料刪除失敗"
                    });
                }
                else
                {
                    //刪除已儲存檔案
                    DelFile(items[0].EngMainSeq, items[0]);
                    return Json(new
                    {
                        result = 0,
                        message = "資料刪除完成",
                    });
                }
            }
        }

        public JsonResult getFlowChartTpDiagramJson(int id, string type)
        {
            try
            {
                EngMaterialDeviceListTpService service = new EngMaterialDeviceListTpService();
                FlowChartDiagramService flowChartHandler = new FlowChartDiagramService();
                int tpId = flowChartHandler.getFlowChartTpId(id, type);
                string result;
                if (tpId == -1)
                {
                    type += "Addition";
                    result = flowChartHandler.getFlowChartTpDiagramJson(id, type);
                }
                else
                {
                    result = flowChartHandler.getFlowChartTpDiagramJson(tpId, type);
                }

                return Json(new { status = "success", jsonStr = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { status = "failed" }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult storeFlowChartTpDiagramJson(FormCollection value)
        {
            try
            {
                EngMaterialDeviceListTpService service = new EngMaterialDeviceListTpService();
                FlowChartDiagramService flowChartHandler = new FlowChartDiagramService();
                Int32.TryParse(value["ItemId"], out int flowChartSeq);
                int tpId = flowChartHandler.getFlowChartTpId(flowChartSeq, value.Get("Type"));
                value.Set("Type", value.Get("Type") + "Addition");
                value.Set("Seq", value.Get("ItemId"));
                //if (tpId == -1)
                //{
                //    value.Set("Type", value.Get("Type") + "Addition");
                //    value.Set("Seq", value.Get("ItemId"));
                //}
                //else
                //{
                //    value.Set("Seq", tpId.ToString());
                //}
                flowChartHandler.storeFlowChartTpDiagramJson(value);
                return Json(new { status = "success" });
            }
            catch (Exception e)
            {
                return Json(new { status = "failed" });
            }
        }
        public JsonResult Chapter5Upload(int engMain, int seq)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                try
                {
                    EngMaterialDeviceListService service = new EngMaterialDeviceListService();
                    List<EngMaterialDeviceListVModel> items = service.GetItemFileInfoBySeq(seq);
                    if (items.Count == 0)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "資料錯誤無法上傳檔案"
                        });
                    }

                    EngMaterialDeviceListVModel m = items[0];
                    if (SaveFile(file, engMain, m, "Flow-U5-"))
                    {
                        string originFileName = file.FileName.ToString().Trim();
                        if (service.UpdateUploadFile(seq, m.FlowCharOriginFileName, m.FlowCharUniqueFileName) == 0)
                        {

                            return Json(new
                            {
                                result = -1,
                                message = "上傳檔案失敗"
                            });
                        }
                        else
                        {

                            items = service.GetItemBySeq<EngMaterialDeviceListVModel>(seq);
                            return Json(new
                            {
                                result = 0,
                                message = "上傳檔案完成",
                                item = Json(items[0])
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
        //流程圖
        public ActionResult Chapter5DownloadFlowChart(int engMain, int seq)
        {
            EngMaterialDeviceListService service = new EngMaterialDeviceListService();
            List<EngMaterialDeviceListVModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                });
            }
            return DownloadFile(engMain, items[0]);
        }

        public ActionResult Chapter5ShowFlowChart(int engMain, int seq)
        {
            EngMaterialDeviceListService service = new EngMaterialDeviceListService();
            List<EngMaterialDeviceListVModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                });
            }
            //string filePath = Utils.GetEngMainFolder(engMain);
            string filePath = String.Format("/FileUploads/Eng/{0}", engMain);

            string uniqueFileName = items[0].FlowCharUniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
                return Json(new
                {
                    url = fullPath
                });
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Chapter5DelFlowChart(int engMain, int seq)
        {
            EngMaterialDeviceListService service = new EngMaterialDeviceListService();
            List<EngMaterialDeviceListVModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0)
            {
                return Json(new
                {
                    result = 0,
                    message = "刪除檔案完成"
                });
            }
            EngMaterialDeviceListVModel m = items[0];
            if (DelFile(engMain, m))
            {
                if (service.UpdateUploadFile(seq, "", "") == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "刪除檔案紀錄失敗"
                    });
                }
                else
                {
                    items = service.GetItemBySeq<EngMaterialDeviceListVModel>(seq);
                    return Json(new
                    {
                        result = 0,
                        message = "刪除檔案完成",
                        item = Json(items[0])
                    });

                }
            }
            return Json(new
            {
                result = -1,
                message = "刪除檔案失敗"
            });
        }
        //抽查紀錄表
        public ActionResult Chapter5DownloadCheckSheet(int engMain, int seq)
        {
            List<EngMaterialDeviceControlStModel> items = new EngMaterialDeviceControlStService().GetList<EngMaterialDeviceControlStModel>(seq);

            if (items.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "沒有抽檢明細資料"
                }, JsonRequestBehavior.AllowGet);
            }
            List<EngMainModel> engItems = new EngMainService().GetItemBySeq<EngMainModel>(engMain);
            if (engItems.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "工程資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            List<EngMaterialDeviceListVModel> masterItems = new EngMaterialDeviceListService().GetItemBySeq<EngMaterialDeviceListVModel>(seq);
            if (masterItems.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "項目資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }

            string tempFile = CopyTemplateFile("抽查紀錄表維護-5-材料品質抽驗紀錄表.docx");
            string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            XWPFTable table = doc.Tables[0];
            table.GetRow(0).GetCell(1).SetText(engItems[0].EngName);//工程名稱
            table.GetRow(1).GetCell(1).SetText(masterItems[0].MDName);//分項工程名稱
            //插入 row
            for (int i = 1; i < items.Count; i++)
            {
                Utils.insertRow(table, 3, 4);
            }
            int rowInx = 0;
            foreach (EngMaterialDeviceControlStModel m in items)
            {
                table.GetRow(3 + rowInx).GetCell(0).SetText(m.MDTestItem);
                table.GetRow(3 + rowInx).GetCell(1).SetText(m.MDTestStand1);// + " " + m.MDTestStand2);
                rowInx++;
            }
            doc.Paragraphs.Where(r => r.Text.StartsWith("表5-3")).FirstOrDefault()?.ReplaceText("#[ItemName]", masterItems[0].MDName);
            doc.Write(fsOut);
            fsOut.Close();
            doc.Close();
            fs.Close();

            Stream iStream = new FileStream(outFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, "application/blob", $"{masterItems[0].MDName}抽驗紀錄表.docx");
        }

        //第六章 設備功能運轉測試抽驗程序及標準
        //s20230302
        public JsonResult Chapter6SearchEngNo(string engNo)
        {
            return Json(new
            {
                result = 0,
                items = new EquOperTestListService().GetListByEngNo<ConstCheckListVModel>(engNo)
            });
        }
        //複製流程圖,檢驗標準 s20230302
        public JsonResult Chapter6CopyItem(int srcId, int tarId)
        {
            if (new EquOperTestListService().CopyFlowAndStItem(srcId, tarId))
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
        public JsonResult Chapter6(int engMain, int pageIndex, int perPage)
        {
            List<EquOperTestListVModel> result = new List<EquOperTestListVModel>();
            EquOperTestListService service = new EquOperTestListService();
            int total = service.GetListCount(engMain);
            if (total > 0)
            {
                result = service.GetList<EquOperTestListVModel>(engMain, pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        public JsonResult Chapter6Item(int seq)
        {
            List<EquOperTestListVModel> result = new List<EquOperTestListVModel>();
            EquOperTestListService service = new EquOperTestListService();
            result = service.GetItemBySeq<EquOperTestListVModel>(seq);
            if (result.Count > 0)
                return Json(result[0]);
            else
                return Json(new { });
        }

        public JsonResult Chapter6NewItem(int engMain)
        {
            EquOperTestListService service = new EquOperTestListService();
            EquOperTestListModel item = new EquOperTestListModel();
            item.EngMainSeq = engMain;
            item.EPKind = 1;
            item.ItemName = "未輸入";
            item.DataKeep = true;
            item.DataType = 1;//使用者新增

            int newSeq = service.Add(item);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "新增失敗"
                });
            }
            else
            {
                List<EquOperTestListVModel> items = service.GetItemBySeq<EquOperTestListVModel>(newSeq);
                return Json(new
                {
                    result = 0,
                    message = "新增完成",
                    item = Json(items[0])
                });
            }
        }

        public JsonResult Chapter6Save(EquOperTestListVModel item)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + item.MDTestItem);

            using (var context = new EQC_NEW_Entities())
            {
                if (context.ConstCheckRec.Where(rec => rec.ItemSeq == item.Seq && rec.CCRCheckType1 == 2).Count() > 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "已有抽查，無法刪除",
                    });
                }
            }
                        EquOperTestListService service = new EquOperTestListService();
            if (service.Update(item.Seq, item.OrderNo.Value, item.EPKind.Value, item.ItemName, item.DataKeep) == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
            else
            {
                List<EquOperTestListVModel> items = service.GetItemBySeq<EquOperTestListVModel>(item.Seq);
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成",
                    item = Json(items[0])
                });
            }
        }
        public JsonResult Chapter6SaveKeep(List<EquOperTestListVModel> items)
        {
            EquOperTestListService service = new EquOperTestListService();
            if (service.UpdateKeep(items))
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

        public JsonResult Chapter6Del(int seq)
        {
            EquOperTestListService service = new EquOperTestListService();
            List<EquOperTestListVModel> items = service.GetItemFileInfoBySeq(seq);
            FlowChartDiagramService fServuce = new FlowChartDiagramService();
            fServuce.DeleteFlowChart(seq, "Chapter6Addition");
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
                int result = 0;
                try
                {
                    result = service.Delete(seq);
                }
                catch (Exception e)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "已有抽驗標準項目, 不可刪除"
                    });
                }
                if (result == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "資料刪除失敗"
                    });
                }
                else
                {
                    //刪除已儲存檔案
                    DelFile(items[0].EngMainSeq, items[0]);

                    return Json(new
                    {
                        result = 0,
                        message = "資料刪除完成",
                    });
                }
            }
        }

        public JsonResult Chapter6Upload(int engMain, int seq)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                try
                {
                    EquOperTestListService service = new EquOperTestListService();
                    List<EquOperTestListVModel> items = service.GetItemFileInfoBySeq(seq);
                    if (items.Count == 0)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "資料錯誤無法上傳檔案"
                        });
                    }

                    EquOperTestListVModel m = items[0];
                    if (SaveFile(file, engMain, m, "Flow-U6-"))
                    {
                        string originFileName = file.FileName.ToString().Trim(); ;
                        if (service.UpdateUploadFile(seq, m.FlowCharOriginFileName, m.FlowCharUniqueFileName) == 0)
                        {

                            return Json(new
                            {
                                result = -1,
                                message = "上傳檔案失敗"
                            });
                        }
                        else
                        {

                            items = service.GetItemBySeq<EquOperTestListVModel>(seq);
                            return Json(new
                            {
                                result = 0,
                                message = "上傳檔案完成",
                                item = Json(items[0])
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
        //流程圖
        public ActionResult Chapter6DownloadFlowChart(int engMain, int seq)
        {
            EquOperTestListService service = new EquOperTestListService();
            List<EquOperTestListVModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                });
            }
            return DownloadFile(engMain, items[0]);
        }

        public ActionResult Chapter6ShowFlowChart(int engMain, int seq)
        {
            EquOperTestListService service = new EquOperTestListService();
            List<EquOperTestListVModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                });
            }
            //string filePath = Utils.GetEngMainFolder(engMain);
            string filePath = String.Format("/FileUploads/Eng/{0}", engMain);

            string uniqueFileName = items[0].FlowCharUniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
                return Json(new
                {
                    url = fullPath
                });
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Chapter6DelFlowChart(int engMain, int seq)
        {
            EquOperTestListService service = new EquOperTestListService();
            List<EquOperTestListVModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0)
            {
                return Json(new
                {
                    result = 0,
                    message = "刪除檔案完成"
                });
            }
            EquOperTestListVModel m = items[0];
            if (DelFile(engMain, m))
            {
                if (service.UpdateUploadFile(seq, "", "") == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "刪除檔案紀錄失敗"
                    });
                }
                else
                {
                    items = service.GetItemBySeq<EquOperTestListVModel>(seq);
                    return Json(new
                    {
                        result = 0,
                        message = "刪除檔案完成",
                        item = Json(items[0])
                    });

                }
            }
            return Json(new
            {
                result = -1,
                message = "刪除檔案失敗"
            });
        }
        //抽查紀錄表
        public ActionResult Chapter6DownloadCheckSheet(int engMain, int seq)
        {
            List<EquOperControlStModel> items = new EquOperControlStService().GetList<EquOperControlStModel>(seq);

            if (items.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "沒有抽檢明細資料"
                }, JsonRequestBehavior.AllowGet);
            }
            List<EngMainModel> engItems = new EngMainService().GetItemBySeq<EngMainModel>(engMain);
            if (engItems.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "工程資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            List<EquOperTestListVModel> masterItems = new EquOperTestListService().GetItemBySeq<EquOperTestListVModel>(seq);
            if (masterItems.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "項目資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }

            string tempFile = CopyTemplateFile("抽查紀錄表維護-6-設備運轉測試抽驗紀錄表.docx");
            string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            XWPFTable table = doc.Tables[0];
            table.GetRow(0).GetCell(1).SetText(engItems[0].EngName);//工程名稱
            table.GetRow(1).GetCell(1).SetText(masterItems[0].ItemName);//分項工程名稱
            doc.Paragraphs.Where(r => r.Text.StartsWith("表5-3")).FirstOrDefault()?.ReplaceText("#[ItemName]", masterItems[0].ItemName);
            //插入 row
            for (int i = 1; i < items.Count; i++)
            {
                Utils.insertRow(table, 6, 7);
            }
            int rowInx = 0;
            foreach (EquOperControlStModel m in items)
            {
                table.GetRow(6 + rowInx).GetCell(0).SetText(m.EPCheckItem1);// + m.EPCheckItem2);
                table.GetRow(6 + rowInx).GetCell(1).SetText(m.EPStand1);// + m.EPStand2 + m.EPStand3 + " " + m.EPStand4 + m.EPStand5);
                rowInx++;
            }

            doc.Write(fsOut);
            fsOut.Close();
            doc.Close();
            fs.Close();

            Stream iStream = new FileStream(outFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, "application/blob", "施設備運轉測試抽驗紀錄表.docx");
        }

        //第七章 701 施工抽查程序及標準
        //s20230301
        public JsonResult Chapter701SearchEngNo(string engNo)
        {
            return Json(new
            {
                result = 0,
                items = new ConstCheckListService().GetListByEngNo<ConstCheckListVModel>(engNo)
            });
        }
        //複製流程圖,檢驗標準 s20230301
        public JsonResult Chapter701CopyItem(int srcId, int tarId)
        {
            if (new ConstCheckListService().CopyFlowAndStItem(srcId, tarId))
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
        public JsonResult Chapter701(int engMain, int pageIndex, int perPage)
        {
            List<ConstCheckListVModel> result = new List<ConstCheckListVModel>();
            ConstCheckListService service = new ConstCheckListService();
            int total = service.GetListCount(engMain);
            if (total > 0)
            {
                result = service.GetList<ConstCheckListVModel>(engMain, pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        public JsonResult Chapter701Item(int seq)
        {
            List<ConstCheckListVModel> result = new List<ConstCheckListVModel>();
            ConstCheckListService service = new ConstCheckListService();
            result = service.GetItemBySeq<ConstCheckListVModel>(seq);
            if (result.Count > 0)
                return Json(result[0]);
            else
                return Json(new { });
        }

        public JsonResult Chapter701NewItem(int engMain)
        {
            ConstCheckListService service = new ConstCheckListService();
            ConstCheckListModel item = new ConstCheckListModel();
            item.EngMainSeq = engMain;
            item.ItemName = "未輸入";
            item.DataKeep = true;
            item.DataType = 1;//使用者新增
            

            //var file = Request.Files[0];


            int newSeq = service.Add(item);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "新增失敗"
                });
            }
            else
            {
                List<ConstCheckListVModel> items = service.GetItemBySeq<ConstCheckListVModel>(newSeq);


                //ConstCheckListVModel m = items[0];
                //SaveFile(file, engMain, m, "Flow-U701-");
                return Json(new
                {
                    result = 0,
                    message = "新增完成",
                    item = Json(items[0])
                });
            }
        }

        public JsonResult Chapter701Save(ConstCheckListVModel item)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + item.MDTestItem);

            using (var context = new EQC_NEW_Entities())
            {
                if(context.ConstCheckRec.Where(rec => rec.ItemSeq == item.Seq && rec.CCRCheckType1 == 1).Count() > 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "已有抽查，無法刪除",
                    });
                }
            }
            ConstCheckListService service = new ConstCheckListService();

            if (service.Update(item.Seq, item.OrderNo.Value, item.ItemName, item.DataKeep) == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
            else
            {


                List<ConstCheckListVModel> items = service.GetItemBySeq<ConstCheckListVModel>(item.Seq);
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成",
                    item = Json(items[0])
                });
            }
        }
        public JsonResult Chapter701SaveKeep(List<ConstCheckListVModel> items)
        {
            ConstCheckListService service = new ConstCheckListService();
            if (service.UpdateKeep(items))
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

        public JsonResult Chapter701Del(int seq)
        {
            ConstCheckListService service = new ConstCheckListService();
            List<ConstCheckListVModel> items = service.GetItemFileInfoBySeq(seq);
            FlowChartDiagramService fServuce = new FlowChartDiagramService();
            fServuce.DeleteFlowChart(seq, "Chapter701Addition");
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
                int result = 0;
                try
                {
                    result = service.Delete(seq);
                }
                catch (Exception e)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "已有抽驗標準項目, 不可刪除"
                    });
                }
                if (result == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "資料刪除失敗"
                    });
                }
                else
                {
                    //刪除已儲存檔案
                    DelFile(items[0].EngMainSeq, items[0]);
                    FlowChartDiagramService flowChartHandler = new FlowChartDiagramService();
                    flowChartHandler.DeleteFlowChart(seq, "Chapter701");
                    return Json(new
                    {
                        result = 0,
                        message = "資料刪除完成",
                    });
                }
            }
        }

        public JsonResult Chapter701Upload(int engMain, int seq)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                try
                {
                    ConstCheckListService service = new ConstCheckListService();
                    List<ConstCheckListVModel> items = service.GetItemFileInfoBySeq(seq);
                    if (items.Count == 0)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "資料錯誤無法上傳檔案"
                        });
                    }

                    ConstCheckListVModel m = items[0];
                    if (SaveFile(file, engMain, m, "Flow-U701-"))
                    {
                        string originFileName = file.FileName.ToString().Trim();
                        if (service.UpdateUploadFile(seq, m.FlowCharOriginFileName, m.FlowCharUniqueFileName) == 0)
                        {
                            return Json(new
                            {
                                result = -1,
                                message = "上傳檔案失敗"
                            });
                        }
                        else
                            {
                            items = service.GetItemBySeq<ConstCheckListVModel>(seq);
                            return Json(new
                            {
                                result = 0,
                                message = "上傳檔案完成",
                                item = Json(items[0])
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
        //流程圖
        public ActionResult Chapter701DownloadFlowChart(int engMain, int seq)
        {
            ConstCheckListService service = new ConstCheckListService();
            List<ConstCheckListVModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                });
            }
            return DownloadFile(engMain, items[0]);
        }

        public ActionResult Chapter701ShowFlowChart(int engMain, int seq)
        {
            ConstCheckListService service = new ConstCheckListService();
            List<ConstCheckListVModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                });
            }
            //string filePath = Utils.GetEngMainFolder(engMain);
            string filePath = String.Format("/FileUploads/Eng/{0}", engMain);

            string uniqueFileName = items[0].FlowCharUniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
                return Json(new
                {
                    url = fullPath
                });
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Chapter701DelFlowChart(int engMain, int seq)
        {
            ConstCheckListService service = new ConstCheckListService();
            List<ConstCheckListVModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0)
            {
                return Json(new
                {
                    result = 0,
                    message = "刪除檔案完成"
                });
            }
            ConstCheckListVModel m = items[0];
            if (DelFile(engMain, m))
            {
                if (service.UpdateUploadFile(seq, "", "") == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "刪除檔案紀錄失敗"
                    });
                }
                else
                {
                    items = service.GetItemBySeq<ConstCheckListVModel>(seq);
                    return Json(new
                    {
                        result = 0,
                        message = "刪除檔案完成",
                        item = Json(items[0])
                    });

                }
            }
            return Json(new
            {
                result = -1,
                message = "刪除檔案失敗"
            });
        }
        //抽查紀錄表
        public ActionResult Chapter701DownloadCheckSheet(int engMain, int seq)
        {
            List<ConstCheckControlStModel> items = new ConstCheckControlStService().GetList<ConstCheckControlStModel>(seq);

            if (items.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "沒有抽檢明細資料"
                }, JsonRequestBehavior.AllowGet);
            }
            List<EngMainModel> engItems = new EngMainService().GetItemBySeq<EngMainModel>(engMain);
            if (engItems.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "工程資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            List<ConstCheckListVModel> masterItems = new ConstCheckListService().GetItemBySeq<ConstCheckListVModel>(seq);
            if (masterItems.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "項目資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }

            string tempFile = CopyTemplateFile("抽查紀錄表維護-701-施工抽查紀錄表.docx");
            string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);

            doc.Paragraphs.Where(r => r.Text.StartsWith("表附四-18")).FirstOrDefault()?.ReplaceText("#[ItemName]", masterItems[0].ItemName.Replace("施工抽查", ""));
            XWPFTable table = doc.Tables[0];
            table.GetRow(0).GetCell(1).SetText(engItems[0].EngName);//工程名稱
            table.GetRow(1).GetCell(1).SetText(masterItems[0].ItemName);//分項工程名稱
            //插入 row
            for (int i = 1; i < items.Count; i++)
            {
                Utils.insertRow(table, 6, 7);
            }
            int rowInx = 0;
            foreach (ConstCheckControlStModel m in items)
            {
                string flowName = "施工後";
                if (m.CCFlow1 == 1)
                {
                    flowName = "施工前";
                }
                else if (m.CCFlow1 == 2)
                {
                    flowName = "施工中";
                }
                table.GetRow(6 + rowInx).GetCell(0).SetText(flowName + " " + m.CCFlow2);
                table.GetRow(6 + rowInx).GetCell(1).SetText(m.CCManageItem1);// + " " + m.CCManageItem2);
                table.GetRow(6 + rowInx).GetCell(2).SetText(m.CCCheckStand1);// + " " + m.CCCheckStand2);
                rowInx++;
            }

            doc.Write(fsOut);
            fsOut.Close();
            doc.Close();
            fs.Close();

            Stream iStream = new FileStream(outFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, "application/blob", $"{masterItems[0].ItemName}紀錄表.docx");
        }

        //第七章 702 環境保育抽查標準
        //s20230302
        public JsonResult Chapter702SearchEngNo(string engNo)
        {
            return Json(new
            {
                result = 0,
                items = new EnvirConsListService().GetListByEngNo<ConstCheckListVModel>(engNo)
            });
        }
        //複製流程圖,檢驗標準 s20230302
        public JsonResult Chapter702CopyItem(int srcId, int tarId)
        {
            if (new EnvirConsListService().CopyFlowAndStItem(srcId, tarId))
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
        public JsonResult Chapter702(int engMain, int pageIndex, int perPage)
        {
            List<EnvirConsListVModel> result = new List<EnvirConsListVModel>();
            EnvirConsListService service = new EnvirConsListService();
            int total = service.GetListCount(engMain);
            if (total > 0)
            {
                result = service.GetList<EnvirConsListVModel>(engMain, pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        public JsonResult Chapter702Item(int seq)
        {
            List<EnvirConsListVModel> result = new List<EnvirConsListVModel>();
            EnvirConsListService service = new EnvirConsListService();
            result = service.GetItemBySeq<EnvirConsListVModel>(seq);
            if (result.Count > 0)
                return Json(result[0]);
            else
                return Json(new { });
        }

        public JsonResult Chapter702NewItem(int engMain)
        {
            EnvirConsListService service = new EnvirConsListService();
            EnvirConsListModel item = new EnvirConsListModel();
            item.EngMainSeq = engMain;
            item.ItemName = "未輸入";
            item.DataKeep = true;
            item.DataType = 1;//使用者新增

            int newSeq = service.Add(item);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "新增失敗"
                });
            }
            else
            {
                List<EnvirConsListVModel> items = service.GetItemBySeq<EnvirConsListVModel>(newSeq);
                return Json(new
                {
                    result = 0,
                    message = "新增完成",
                    item = Json(items[0])
                });
            }
        }

        public JsonResult Chapter702Save(EnvirConsListVModel item)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + item.MDTestItem);
            using (var context = new EQC_NEW_Entities())
            {
                if (context.ConstCheckRec.Where(rec => rec.ItemSeq == item.Seq && rec.CCRCheckType1 == 4).Count() > 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "已有抽查，無法刪除",
                    });
                }
            }
            EnvirConsListService service = new EnvirConsListService();
            if (service.Update(item.Seq, item.OrderNo.Value, item.ItemName, item.DataKeep) == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
            else
            {
                List<EnvirConsListVModel> items = service.GetItemBySeq<EnvirConsListVModel>(item.Seq);
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成",
                    item = Json(items[0])
                });
            }
        }
        public JsonResult Chapter702SaveKeep(List<EnvirConsListVModel> items)
        {
            EnvirConsListService service = new EnvirConsListService();
            if (service.UpdateKeep(items))
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

        public JsonResult Chapter702Del(int seq)
        {
            EnvirConsListService service = new EnvirConsListService();
            List<EnvirConsListVModel> items = service.GetItemFileInfoBySeq(seq);
            FlowChartDiagramService fServuce = new FlowChartDiagramService();
            fServuce.DeleteFlowChart(seq, "Chapter702Addition");
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
                int result = 0;
                try
                {
                    result = service.Delete(seq);
                }
                catch (Exception e)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "已有抽驗標準項目, 不可刪除"
                    });
                }
                if (result == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "資料刪除失敗"
                    });
                }
                else
                {
                    //刪除已儲存檔案
                    DelFile(items[0].EngMainSeq, items[0]);
                    return Json(new
                    {
                        result = 0,
                        message = "資料刪除完成",
                    });
                }
            }
        }

        public JsonResult Chapter702Upload(int engMain, int seq)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                try
                {
                    EnvirConsListService service = new EnvirConsListService();
                    List<EnvirConsListVModel> items = service.GetItemFileInfoBySeq(seq);
                    if (items.Count == 0)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "資料錯誤無法上傳檔案"
                        });
                    }

                    EnvirConsListVModel m = items[0];
                    if (SaveFile(file, engMain, m, "Flow-U702-"))
                    {
                        string originFileName = file.FileName.ToString().Trim();
                        if (service.UpdateUploadFile(seq, m.FlowCharOriginFileName, m.FlowCharUniqueFileName) == 0)
                        {
                            return Json(new
                            {
                                result = -1,
                                message = "上傳檔案失敗"
                            });
                        }
                        else
                        {
                            items = service.GetItemBySeq<EnvirConsListVModel>(seq);
                            return Json(new
                            {
                                result = 0,
                                message = "上傳檔案完成",
                                item = Json(items[0])
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
        //流程圖
        public ActionResult Chapter702DownloadFlowChart(int engMain, int seq)
        {
            EnvirConsListService service = new EnvirConsListService();
            List<EnvirConsListVModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                });
            }
            return DownloadFile(engMain, items[0]);
        }

        public ActionResult Chapter702ShowFlowChart(int engMain, int seq)
        {
            EnvirConsListService service = new EnvirConsListService();
            List<EnvirConsListVModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                });
            }
            //string filePath = Utils.GetEngMainFolder(engMain);
            string filePath = String.Format("/FileUploads/Eng/{0}", engMain);

            string uniqueFileName = items[0].FlowCharUniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
                return Json(new
                {
                    url = fullPath
                });
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Chapter702DelFlowChart(int engMain, int seq)
        {
            EnvirConsListService service = new EnvirConsListService();
            List<EnvirConsListVModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0)
            {
                return Json(new
                {
                    result = 0,
                    message = "刪除檔案完成"
                });
            }
            EnvirConsListVModel m = items[0];
            if (DelFile(engMain, m))
            {
                if (service.UpdateUploadFile(seq, "", "") == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "刪除檔案紀錄失敗"
                    });
                }
                else
                {
                    items = service.GetItemBySeq<EnvirConsListVModel>(seq);
                    return Json(new
                    {
                        result = 0,
                        message = "刪除檔案完成",
                        item = Json(items[0])
                    });

                }
            }
            return Json(new
            {
                result = -1,
                message = "刪除檔案失敗"
            });
        }
        //抽查紀錄表
        public ActionResult Chapter702DownloadCheckSheet(int engMain, int seq)
        {
            List<EnvirConsControlStModel> items = new EnvirConsControlStService().GetList<EnvirConsControlStModel>(seq);

            if (items.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "沒有抽檢明細資料"
                }, JsonRequestBehavior.AllowGet);
            }
            List<EngMainModel> engItems = new EngMainService().GetItemBySeq<EngMainModel>(engMain);
            if (engItems.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "工程資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            List<EnvirConsListVModel> masterItems = new EnvirConsListService().GetItemBySeq<EnvirConsListVModel>(seq);
            if (masterItems.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "項目資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }

            string tempFile = CopyTemplateFile("抽查紀錄表維護-702-生態保育措施抽查紀錄表.docx");
            string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            XWPFTable table = doc.Tables[0];
            doc.Paragraphs.Where(r => r.Text.StartsWith("表7- 43")).FirstOrDefault()?.ReplaceText("#[ItemName]", masterItems[0].ItemName);
            table.GetRow(0).GetCell(1).SetText(engItems[0].EngName);//工程名稱
            table.GetRow(1).GetCell(1).SetText(masterItems[0].ItemName);//分項工程名稱

            //插入 row
            for (int i = 1; i < items.Count; i++)
            {
                Utils.insertRow(table, 5, 6);
            }
            int rowInx = 0;
            int oldRow = -1;
            byte? oldECCFlow1 = 0;

            foreach (EnvirConsControlStModel m in items)
            {
                if (oldECCFlow1 != m.ECCFlow1)
                {
                    oldECCFlow1 = m.ECCFlow1;

                    XWPFTableCell cellFirstofThird = table.GetRow(5 + rowInx).GetCell(0);
                    CT_Tc cttcFirstofThird = cellFirstofThird.GetCTTc();
                    CT_TcPr ctPrFirstofThird = cttcFirstofThird.AddNewTcPr();
                    ctPrFirstofThird.AddNewVMerge().val = ST_Merge.restart;//開始合併
                    ctPrFirstofThird.AddNewVAlign().val = ST_VerticalJc.center;//垂直置中
                    cttcFirstofThird.GetPList()[0].AddNewPPr().AddNewJc().val = ST_Jc.center;

                    string flowName = "施工後";
                    if (m.ECCFlow1 == 1)
                    {
                        flowName = "施工前";
                    }
                    else if (m.ECCFlow1 == 2)
                    {
                        flowName = "施工中";
                    }
                    table.GetRow(5 + rowInx).GetCell(0).SetText(flowName);
                }
                else
                {
                    XWPFTableCell cellfirstofRow = table.GetRow(5 + rowInx).GetCell(0);
                    CT_Tc cttcfirstofRow = cellfirstofRow.GetCTTc();
                    CT_TcPr ctPrfirstofRow = cttcfirstofRow.AddNewTcPr();
                    ctPrfirstofRow.AddNewVMerge().val = ST_Merge.@continue;//續合併
                    ctPrfirstofRow.AddNewVAlign().val = ST_VerticalJc.center;//垂直置中
                }
                table.GetRow(5 + rowInx).GetCell(1).SetText(m.ECCCheckItem1);// + " " + m.ECCCheckItem2);
                rowInx++;
            }
            doc.Write(fsOut);
            fsOut.Close();
            doc.Close();
            fs.Close();

            Stream iStream = new FileStream(outFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, "application/blob", $"{masterItems[0].ItemName}紀錄表.docx");
        }

        //第七章 703 職業安全衛生抽查標準
        //s20230302
        public JsonResult Chapter703SearchEngNo(string engNo)
        {
            return Json(new
            {
                result = 0,
                items = new EnvirConsListService().GetListByEngNo<ConstCheckListVModel>(engNo)
            });
        }
        //複製流程圖,檢驗標準 s20230302
        public JsonResult Chapter703CopyItem(int srcId, int tarId)
        {
            if (new OccuSafeHealthListService().CopyFlowAndStItem(srcId, tarId))
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
        public JsonResult Chapter703(int engMain, int pageIndex, int perPage)
        {
            List<OccuSafeHealthListVModel> result = new List<OccuSafeHealthListVModel>();
            OccuSafeHealthListService service = new OccuSafeHealthListService();
            int total = service.GetListCount(engMain);
            if (total > 0)
            {
                result = service.GetList<OccuSafeHealthListVModel>(engMain, pageIndex, perPage);
            }
            return Json(new
            {
                pTotal = total,
                items = result
            });
        }
        public JsonResult Chapter703Item(int seq)
        {
            List<OccuSafeHealthListVModel> result = new List<OccuSafeHealthListVModel>();
            OccuSafeHealthListService service = new OccuSafeHealthListService();
            result = service.GetItemBySeq<OccuSafeHealthListVModel>(seq);
            if (result.Count > 0)
                return Json(result[0]);
            else
                return Json(new { });
        }

        public JsonResult Chapter703NewItem(int engMain)
        {
            OccuSafeHealthListService service = new OccuSafeHealthListService();
            OccuSafeHealthListModel item = new OccuSafeHealthListModel();
            item.EngMainSeq = engMain;
            item.ItemName = "未輸入";
            item.DataKeep = true;
            item.DataType = 1;//使用者新增
            int newSeq = service.Add(item);
            if (newSeq == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "新增失敗"
                });
            }
            else
            {
                List<OccuSafeHealthListVModel> items = service.GetItemBySeq<OccuSafeHealthListVModel>(newSeq);
                return Json(new
                {
                    result = 0,
                    message = "新增完成",
                    item = Json(items[0])
                });
            }
        }

        public JsonResult Chapter703Save(OccuSafeHealthListVModel item)
        {
            //System.Diagnostics.Debug.WriteLine("op1: " + item.MDTestItem);
            using (var context = new EQC_NEW_Entities())
            {
                if (context.ConstCheckRec.Where(rec => rec.ItemSeq == item.Seq && rec.CCRCheckType1 == 3).Count() > 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "已有抽查，無法刪除",
                    });
                }
            }
            OccuSafeHealthListService service = new OccuSafeHealthListService();
            if (service.Update(item.Seq, item.OrderNo.Value, item.ItemName, item.DataKeep) == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "資料儲存失敗"
                });
            }
            else
            {
                List<OccuSafeHealthListVModel> items = service.GetItemBySeq<OccuSafeHealthListVModel>(item.Seq);
                return Json(new
                {
                    result = 0,
                    message = "資料儲存完成",
                    item = Json(items[0])
                });
            }
        }
        public JsonResult Chapter703SaveKeep(List<OccuSafeHealthListVModel> items)
        {
            OccuSafeHealthListService service = new OccuSafeHealthListService();
            if (service.UpdateKeep(items))
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

        public JsonResult Chapter703Del(int seq)
        {
            OccuSafeHealthListService service = new OccuSafeHealthListService();
            List<OccuSafeHealthListVModel> items = service.GetItemFileInfoBySeq(seq);
            FlowChartDiagramService fServuce = new FlowChartDiagramService();
            fServuce.DeleteFlowChart(seq, "Chapter703Addition");
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
                int result = 0;
                try
                {
                    result = service.Delete(seq);
                }
                catch (Exception e)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "已有抽驗標準項目, 不可刪除"
                    });
                }
                if (result == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "資料刪除失敗"
                    });
                }
                else
                {
                    //刪除已儲存檔案
                    DelFile(items[0].EngMainSeq, items[0]);
                    return Json(new
                    {
                        result = 0,
                        message = "資料刪除完成",
                    });
                }
            }
        }

        public JsonResult Chapter703Upload(int engMain, int seq)
        {
            var file = Request.Files[0];
            if (file.ContentLength > 0)
            {
                try
                {
                    OccuSafeHealthListService service = new OccuSafeHealthListService();
                    List<OccuSafeHealthListVModel> items = service.GetItemFileInfoBySeq(seq);
                    if (items.Count == 0)
                    {
                        return Json(new
                        {
                            result = -1,
                            message = "資料錯誤無法上傳檔案"
                        });
                    }

                    OccuSafeHealthListVModel m = items[0];
                    if (SaveFile(file, engMain, m, "Flow-U703-"))
                    {
                        string originFileName = file.FileName.ToString().Trim();
                        if (service.UpdateUploadFile(seq, m.FlowCharOriginFileName, m.FlowCharUniqueFileName) == 0)
                        {
                            return Json(new
                            {
                                result = -1,
                                message = "上傳檔案失敗"
                            });
                        }
                        else
                        {
                            items = service.GetItemBySeq<OccuSafeHealthListVModel>(seq);
                            return Json(new
                            {
                                result = 0,
                                message = "上傳檔案完成",
                                item = Json(items[0])
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
        //流程圖
        public ActionResult Chapter703DownloadFlowChart(int engMain, int seq)
        {
            OccuSafeHealthListService service = new OccuSafeHealthListService();
            List<OccuSafeHealthListVModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                });
            }
            return DownloadFile(engMain, items[0]);
        }

        public ActionResult Chapter703ShowFlowChart(int engMain, int seq)
        {
            OccuSafeHealthListService service = new OccuSafeHealthListService();
            List<OccuSafeHealthListVModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0 || items[0].FlowCharUniqueFileName == null || items[0].FlowCharUniqueFileName.Length == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "無檔案資料"
                });
            }
            //string filePath = Utils.GetEngMainFolder(engMain);
            string filePath = String.Format("/FileUploads/Eng/{0}", engMain);

            string uniqueFileName = items[0].FlowCharUniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
                return Json(new
                {
                    url = fullPath
                });
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Chapter703DelFlowChart(int engMain, int seq)
        {
            OccuSafeHealthListService service = new OccuSafeHealthListService();
            List<OccuSafeHealthListVModel> items = service.GetItemFileInfoBySeq(seq);
            if (items.Count == 0)
            {
                return Json(new
                {
                    result = 0,
                    message = "刪除檔案完成"
                });
            }
            OccuSafeHealthListVModel m = items[0];
            if (DelFile(engMain, m))
            {
                if (service.UpdateUploadFile(seq, "", "") == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        message = "刪除檔案紀錄失敗"
                    });
                }
                else
                {
                    items = service.GetItemBySeq<OccuSafeHealthListVModel>(seq);
                    return Json(new
                    {
                        result = 0,
                        message = "刪除檔案完成",
                        item = Json(items[0])
                    });

                }
            }
            return Json(new
            {
                result = -1,
                message = "刪除檔案失敗"
            });
        }
        //抽查紀錄表
        public ActionResult Chapter703DownloadCheckSheet(int engMain, int seq)
        {
            List<OSHCTpModel> items = new OccuSafeHealthControlStService().GetList<OSHCTpModel>(seq);

            if (items.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "沒有抽檢明細資料"
                }, JsonRequestBehavior.AllowGet);
            }
            List<EngMainModel> engItems = new EngMainService().GetItemBySeq<EngMainModel>(engMain);
            if (engItems.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "工程資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }
            List<OccuSafeHealthListVModel> masterItems = new OccuSafeHealthListService().GetItemBySeq<OccuSafeHealthListVModel>(seq);
            if (masterItems.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "項目資料錯誤"
                }, JsonRequestBehavior.AllowGet);
            }

            string tempFile = CopyTemplateFile("抽查紀錄表維護-703-汛期工地防災減災抽查紀錄表.docx");
            string outFile = GetTempFile();
            FileStream fs = new FileStream(tempFile, FileMode.Open, FileAccess.ReadWrite);
            FileStream fsOut = new FileStream(outFile, FileMode.Create, FileAccess.Write);
            XWPFDocument doc = new XWPFDocument(fs);
            XWPFTable table = doc.Tables[0];

            table.GetRow(0).GetCell(1).SetText(engItems[0].EngName);//工程名稱
            table.GetRow(2).GetCell(1).SetText(masterItems[0].ItemName);//抽查地點
            doc.Paragraphs.Where(r => r.Text.StartsWith("表7- 40")).FirstOrDefault()?.ReplaceText("#[ItemName]", masterItems[0].ItemName);
            //插入 row
            for (int i = 1; i < items.Count; i++)
            {
                Utils.insertRow(table, 4, 5);
            }
            int rowInx = 0;
            foreach (OSHCTpModel m in items)
            {
                table.GetRow(4 + rowInx).GetCell(0).SetText(m.OSCheckItem1);// + " " + m.OSCheckItem2);
                table.GetRow(4 + rowInx).GetCell(1).SetText(m.OSStand1);// + " " + m.OSStand2 + " " + m.OSStand3 + " " + m.OSStand4 + " " + m.OSStand5);
                rowInx++;
            }

            doc.Write(fsOut);
            fsOut.Close();
            doc.Close();
            fs.Close();

            Stream iStream = new FileStream(outFile, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(iStream, "application/blob", $"{masterItems[0].ItemName}紀錄表.docx");
        }

        //勾選與設置完成
        public JsonResult SettingComplete(int engMain)
        {
            SupervisionProjectListService service = new SupervisionProjectListService();
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
        //產製監造計畫書
        public JsonResult CreatePlan(int engMain)
        {

            List<SupervisionProjectListVModel> items = supervisionProjectListService.ListByEngMainSeq<SupervisionProjectListVModel>(engMain);
            EcologicalChecklistService ecologicalChecklistService = new EcologicalChecklistService();
            var  _EcologicalItems = ecologicalChecklistService.GetItemByEngMain<EcologicalChecklistModel>(engMain) ;
            EcologicalChecklistModel EcologicalItem = _EcologicalItems.Count > 0 ? _EcologicalItems.First() : new EcologicalChecklistModel();
            string filePath = Utils.GetEcologicalCheckRemoteFolder(engMain, "FileUploads/EcologicalCheck"); //施工階段
            string filePath2 = Utils.GetEcologicalCheckRemoteFolder(engMain, "FileUploads/EcologicalCheck2"); //設計階段
            string successMessage = "計畫書背景產製中，預估需要2 - 5分鐘，請可繼續操作...";


            if (!System.IO.File.Exists(Path.Combine(filePath, EcologicalItem.SelfEvalFilename ?? "")) &&
                !System.IO.File.Exists(Path.Combine(filePath, EcologicalItem.ConservMeasFilename ?? "")) &&
                EcologicalItem.ToDoChecklit <= 2 )
            {
                return Json(new
                {
                    result = -1,
                    message = "產製監造計畫書前，請先上傳設計階段之生態檢核自評表及生態保育措施資料，請至功能選單【生態檢核(設計階段)上傳】功能上傳相關檔案"
                });
            }
            if (!System.IO.File.Exists(Path.Combine(filePath2, EcologicalItem.SelfEvalFilename ?? "")) ||
                !System.IO.File.Exists(Path.Combine(filePath2, EcologicalItem.PlanDesignRecordFilename ?? "")) ||
                !System.IO.File.Exists(Path.Combine(filePath2, EcologicalItem.ConservMeasFilename ?? "")) && EcologicalItem.ToDoChecklit <= 2)
            {
                successMessage += "， 提醒: 需上傳生態自我檢核表、施工說明會議記錄、生態保育措施自主檢查表";
            }
            if (items.Count == 0)
            {
                return Json(new
                {
                    result = -1,
                    message = "計畫書產製失敗,無範本"
                });
            }
            SupervisionProjectListService service = new SupervisionProjectListService();
            if (service.UpdateDocState(engMain, SupervisionProjectListService.docstate_PlanMaking) == 1)
            {
                Task.Run( async() => await Export.ExportWord(items[0].Seq, HttpContext));


                return Json(new
                {
                    result = 0,
                    state = SupervisionProjectListService.docstate_PlanMaking,
                    message = successMessage
                }) ; 
            }
            else
                return Json(new
                {
                    result = -1,
                    message = "計畫書產製失敗"
                });
        }

        // 共用 =============================
        private bool SaveFile(HttpPostedFileBase file, int engMainSeq, FlowChartFileModel m, string fileHeader)
        {
            try
            {
                //string filePath = Utils.GetEngMainFolder(engMainSeq);
                string filePath = Utils.GetTemplateFolder();
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                //刪除已儲存原始檔案
                string uniqueFileName = m.FlowCharUniqueFileName;
                if (uniqueFileName != null && uniqueFileName.Length > 0)
                {
                    System.IO.File.Delete(Path.Combine(filePath, uniqueFileName));
                }


                string originFileName = file.FileName.ToString().Trim();
                int inx = originFileName.LastIndexOf(".");

                //配合流程圖編輯的更動
                uniqueFileName = uniqueFileNameFomater(fileHeader)+ originFileName.Substring(inx);

                string fullPath = Path.Combine(filePath, uniqueFileName);
                file.SaveAs(fullPath);

                m.FlowCharOriginFileName = originFileName;
                m.FlowCharUniqueFileName = uniqueFileName; 
                return true;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("上傳檔案失敗: " + e.Message);
                return false;
            }
        }

        private string uniqueFileNameFomater(string header)
        {
            return String.Format("{0}{1}", header, DateTime.Now.ToFileTime());
        }

        private ActionResult DownloadFile(int engMainSeq, FlowChartFileModel m)
        {
            string filePath = Utils.GetTemplateFolder();  //配合流程圖編輯的變更

            string uniqueFileName = m.FlowCharUniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
                if (System.IO.File.Exists(fullPath))
                {
                    Stream iStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    return File(iStream, "application/blob", m.FlowCharOriginFileName);
                }
            }

            return Json(new
            {
                result = -1,
                message = "下載失敗"
            }, JsonRequestBehavior.AllowGet);
        }

        private bool DelFile(int engMainSeq, FlowChartFileModel m)
        {
            string filePath = Utils.GetEngMainFolder(engMainSeq);

            string uniqueFileName = m.FlowCharUniqueFileName;
            if (uniqueFileName != null && uniqueFileName.Length > 0)
            {
                string fullPath = Path.Combine(filePath, uniqueFileName);
                if (System.IO.File.Exists(fullPath))
                {
                    try
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }
            }

            return true;
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
    }
}
