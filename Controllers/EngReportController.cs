using Antlr.Runtime.Misc;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using EQC.Common;
using EQC.EDMXModel;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using EQC.ViewModel.Common;
using Microsoft.SqlServer.Server;
using NPOI.HSSF.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Web.Mvc;
using ZXing;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EngReportController : Controller
    {//工程提報
        EngReportService engReportService = new EngReportService();
        EngReportApproveService engReportApproveService = new EngReportApproveService();

        public ActionResult Index()
        {
            Utils.setUserClass(this, 1);
            return View("Index");
        }

        public ActionResult GetUserUnit()
        {
            string unitSubSeq = "";
            string unitSeq = "";
            string unitSubN = "";
            string unitN = "";
            Utils.GetUserUnit(ref unitSeq, ref unitN, ref unitSubSeq, ref unitSubN);
            return Json(new
            {
                result = 0,
                unit = unitSeq,
                unitSub = unitSubSeq,
                unitName = unitN,
                unitSubName = unitSubN,
                userName = Utils.getUserInfo().DisplayName
            });
        }

        //工程年度
        public JsonResult GetYearOptions()
        {
            List<EngYearVModel> items = engReportService.GetEngYearList();
            if (items.Count == 0)
            {
                EngYearVModel m = new EngYearVModel();
                m.EngYear = DateTime.Now.Year - 1911;
                items.Insert(0, m);
            }
            return Json(items);
        }

        //依年分取執行機關
        public JsonResult GetUnitOptions(string year)
        {
            UserInfo userInfo = Utils.getUserInfo();
            List<EngExecUnitsVModel> items = engReportService.GetEngExecUnitList(year);
            EngExecUnitsVModel m = new EngExecUnitsVModel();
            if (items.Count == 0)
            {
                m.UnitSeq = Convert.ToInt32(userInfo.UnitSeq1);
                m.UnitName = userInfo.UnitName1;
                items.Insert(0, m);
            }
            return Json(items);
        }

        //依年分,機關取執行單位
        public JsonResult GetSubUnitOptions(string year, int parentSeq)
        {
            string unitSubSeq = "";
            string unitSeq = "";
            string unitSubN = "";
            string unitN = "";
            Utils.GetUserUnit(ref unitSeq, ref unitN, ref unitSubSeq, ref unitSubN);
            UserInfo userInfo = Utils.getUserInfo();
            List<EngExecUnitsVModel> items = engReportService.GetEngExecSubUnitList(year, parentSeq);
            EngExecUnitsVModel m = new EngExecUnitsVModel();
            if (userInfo.RoleSeq == 20)
            {
                if (items.Count == 0)
                {
                    m.UnitSeq = Convert.ToInt32(unitSubSeq);
                    m.UnitName = unitSubN;
                    items.Insert(0, m);
                }
            }
            else
            {
                m.UnitSeq = -1;
                m.UnitName = "全部單位";
                items.Insert(0, m);
            }
            return Json(items);
        }

        //工程年度
        public JsonResult GetRptTypeOptions(int funcNo)
        {
            List<SelectVM> items = engReportService.GetEngRptTypeList(funcNo);
            return Json(items);
        }

        //類別
        public JsonResult GetProposalReviewTypeList()
        {
            List<SelectVM> items = engReportService.GetProposalReviewTypeList();
            return Json(items);
        }

        //屬性
        public JsonResult GetProposalReviewAttributesList()
        {
            List<SelectVM> items = engReportService.GetProposalReviewAttributesList();
            return Json(items);
        }

        //河川
        public JsonResult GetRiverList(int id, int ParentSeq)
        {
            List<EngReportVModel> engReport = engReportService.GetEegReportBySeq<EngReportVModel>(id);
            EngReportVModel item = engReport[0];
            List<SelectVM> items = engReportService.GetRiverList(item.ExecUnit, ParentSeq);
            return Json(items);
        }

        //排水
        public JsonResult GetDrainList()
        {
            List<SelectVM> items = engReportService.GetDrainList();
            return Json(items);
        }

        //主要工作內容
        public JsonResult GetReportJobDescriptionList()
        {
            List<SelectVM> items = engReportService.GetReportJobDescriptionList();
            return Json(items);
        }

        //行政區(縣市)
        public JsonResult GetCityList()
        {
            CityService s = new CityService();
            List<VCityModel> items = s.GetCityForOption<VCityModel>();
            return Json(items);
        }

        //行政區(鄉鎮)
        public JsonResult GetTownList(int id)
        {
            TownService s = new TownService();
            List<VTownModel> items = s.GetTownForOption<VTownModel>(id);
            return Json(items);
        }

        //下載-提案需求評估表
        public ActionResult DownloadNeedAssessmentVF(int year, int unit, int subUnit, int rptType)
        {
            try
            {
                List<EngReportVModel> engList = new List<EngReportVModel>();
                int total = engReportService.GetEngListCount(year, unit, subUnit, rptType, 1);
                if (total > 0)
                {
                    engList = engReportService.GetEngList<EngReportVModel>(year, unit, subUnit, rptType, total, 1, 9);
                }
                if (engList.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "工程資料錯誤"
                    });
                }
                //暫存目錄
                string uuid = Guid.NewGuid().ToString("B").ToUpper();
                string folder = Path.Combine(Path.GetTempPath(), uuid);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                string destFile = CreateNeedAssessmentVFDoc(engList, folder);
                if (destFile == "")
                {
                    return Json(new
                    {
                        result = -1,
                        message = "提案需求評估表 製表失敗"
                    }, JsonRequestBehavior.AllowGet);
                }
                //Stream iStream = new FileStream(destFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                //return File(iStream, "application/blob", "提案需求評估表.docx");

                string zipFile = Path.Combine(Path.GetTempPath(), uuid + "-提案需求評估表.zip");
                System.IO.Compression.ZipFile.CreateFromDirectory(folder, zipFile);// AddFiles(files, "ProjectX");
                Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", $"{DateTime.Now:yyyyMMddHHmmss}_提案需求評估表.zip");
            }
            catch (Exception e)
            {
                BaseService.log.Info("CreateDoc NeedAssessmentVF Err: " + e.Message);
            }

            return Json(new
            {
                result = -1,
                message = "請求錯誤"
            }, JsonRequestBehavior.AllowGet);
        }
        private void CopyFile(string folder,int EngReportSeq, string RptName, string fileName,string sFileName) 
        {
            string subFolder = Path.Combine(folder, RptName);

            if (!string.IsNullOrEmpty(fileName))
            {
                if (!Directory.Exists(subFolder)) Directory.CreateDirectory(subFolder);

                string sFile = Path.Combine(Utils.GetEngReportFolder(EngReportSeq), fileName);

                try 
                {
                    if (System.IO.File.Exists(sFile))
                    {
                        string dFile = Path.Combine(subFolder, sFileName);
                        System.IO.File.Copy(sFile, dFile);
                    }
                }
                catch { }
            }
        }
        private string CreateNeedAssessmentVFDoc(List<EngReportVModel> eng, string folder) 
        {
            bool result = false;

            List<EngReportApproveVModel> engApproveList = new List<EngReportApproveVModel>();

            string destFile = Path.Combine(folder, $"提案需求評估表-{DateTime.Now:HHmmss}.docx");
            string srcFile = Path.Combine(Utils.GetTemplateFilePath(), "提案需求評估表vf.docx");
            //System.IO.File.Copy(srcFile, filename);

            byte[] templat = System.IO.File.ReadAllBytes(srcFile);

            List<string> files = new List<string>();
            byte[] docxBytes = null;
            Dictionary<string, string> data = null;
            string MergeFilePath = "";

            var engA = eng.OrderBy(r => r.CreateTime);

            string FileName1, FileName2, FileName3, FileName4, FileName5, FileName6, FileName7;
            string ApprovalNameA = "", ApprovalNameB = "", ApprovalNameC = "", ApprovalNameD = "", ApprovalNameE = "";
            string sFileName = "";
            foreach (EngReportVModel item in engA) 
            {
                engApproveList = engReportApproveService.GetEngReportApproveList<EngReportApproveVModel>(item.Seq);

                FileName1 = string.IsNullOrEmpty(item.LocationMap) ? "" : $"位置圖_{item.LocationMapFileName}";
                CopyFile(folder, item.Seq, item.RptName, item.LocationMap, FileName1);
                FileName2 = string.IsNullOrEmpty(item.AerialPhotography) ? "" : $"空拍照_{item.AerialPhotographyFileName}";
                CopyFile(folder, item.Seq, item.RptName, item.AerialPhotography, FileName2);
                FileName3 = string.IsNullOrEmpty(item.ScenePhoto) ? "" : $"現場照片_{item.ScenePhotoFileName}";
                CopyFile(folder, item.Seq, item.RptName, item.ScenePhoto, FileName3);
                FileName4 = string.IsNullOrEmpty(item.BaseMap) ? "" : $"基地地藉圖_{item.BaseMapFileName}";
                CopyFile(folder, item.Seq, item.RptName, item.BaseMap, FileName4);
                FileName5 = string.IsNullOrEmpty(item.EngPlaneLayout) ? "" : $"工程平面配置圖_{item.EngPlaneLayoutFileName}";
                CopyFile(folder, item.Seq, item.RptName, item.EngPlaneLayout, FileName5);
                FileName6 = string.IsNullOrEmpty(item.LongitudinalSection) ? "" : $"縱斷面圖_{item.LongitudinalSectionFileName}";
                CopyFile(folder, item.Seq, item.RptName, item.LongitudinalSection, FileName6);
                FileName7 = string.IsNullOrEmpty(item.StandardSection) ? "" : $"標準斷面圖_{item.StandardSectionFileName}";
                CopyFile(folder, item.Seq, item.RptName, item.StandardSection, FileName7);

                ApprovalNameA = ""; 
                ApprovalNameB = ""; 
                ApprovalNameC = ""; 
                ApprovalNameD = ""; 
                ApprovalNameE = "";
                foreach (EngReportApproveVModel itemA in engApproveList)
                {
                    if (!string.IsNullOrEmpty(itemA.ApproveUser)) 
                    {
                        if (itemA.ApproverName == "工務課") ApprovalNameA = $"{itemA.ApproveUser}(數位簽章){Utils.ChsDateTime(itemA.ApproveTime)}";
                        if (itemA.ApproverName == "規劃課") ApprovalNameB = $"{itemA.ApproveUser}(數位簽章){Utils.ChsDateTime(itemA.ApproveTime)}";
                        if (itemA.ApproverName == "管理課") ApprovalNameC = $"{itemA.ApproveUser}(數位簽章){Utils.ChsDateTime(itemA.ApproveTime)}";
                        if (itemA.ApproverName == "資產課") ApprovalNameD = $"{itemA.ApproveUser}(數位簽章){Utils.ChsDateTime(itemA.ApproveTime)}";
                        if (itemA.ApproverName == "局長室") ApprovalNameE = $"{itemA.ApproveUser}(數位簽章){Utils.ChsDateTime(itemA.ApproveTime)}";
                    }
                }

                data = new Dictionary<string, string>()
                {
                    ["ExecUnit"] = item.ExecUnit,
                    ["RptName"] = item.RptName,
                    ["OriginAndScope"] = item.OriginAndScope == null ? "" : item.OriginAndScope,
                    ["RelatedReportResults"] = item.RelatedReportResults == null ? "" : item.RelatedReportResults,
                    ["FacilityManagement"] = item.FacilityManagement == null ? "" : item.FacilityManagement,
                    ["ProposalScopeLand"] = item.ProposalScopeLand == null ? "" : item.ProposalScopeLand,
                    ["E1"] = item.EvaluationResult == 1 ? "■" : "□",
                    ["E2"] = item.EvaluationResult == 2 ? "■" : "□",
                    ["E3"] = item.EvaluationResult == 3 ? "■" : "□",
                    ["E4"] = item.EvaluationResult == 4 ? "■" : "□",
                    ["E5"] = item.EvaluationResult == 5 ? "■" : "□",
                    ["E6"] = item.EvaluationResult == 6 ? "■" : "□",
                    ["ER1_1"] = item.ER1_1 == null ? "" : item.ER1_1,
                    ["ER1_2"] = item.ER1_2 == null ? "" : item.ER1_2,
                    ["ER2_1"] = item.ER2_1 == null ? "" : item.ER2_1,
                    ["ER2_2"] = item.ER2_2 == null ? "" : item.ER2_2,
                    ["ER3"] = item.ER3 == null ? "" : item.ER3,
                    ["ER4"] = item.ER4 == null ? "" : item.ER4,
                    ["ER6"] = item.ER6 == null ? "" : item.ER6,

                    ["ReviewNameA"] = string.IsNullOrEmpty(item.OriginAndScopeUpdateReviewUserName) ? "" : $"{item.OriginAndScopeUpdateReviewUserName}(數位簽章){Utils.ChsDateTime(item.OriginAndScopeReviewTime)}",
                    ["ReviewNameB"] = string.IsNullOrEmpty(item.RelatedReportResultsUpdateReviewUserName) ? "" : $"{item.RelatedReportResultsUpdateReviewUserName}(數位簽章){Utils.ChsDateTime(item.RelatedReportResultsReviewTime)}",
                    ["ReviewNameC"] = string.IsNullOrEmpty(item.FacilityManagementUpdateReviewUserName) ? "" : $"{item.FacilityManagementUpdateReviewUserName}(數位簽章){Utils.ChsDateTime(item.FacilityManagementReviewTime)}",
                    ["ReviewNameD"] = string.IsNullOrEmpty(item.ProposalScopeLandUpdateReviewUserName) ? "" : $"{item.ProposalScopeLandUpdateReviewUserName}(數位簽章){Utils.ChsDateTime(item.ProposalScopeLandReviewTime)}",

                    ["ApprovalNameA"] = ApprovalNameA,
                    ["ApprovalNameB"] = ApprovalNameB,
                    ["ApprovalNameC"] = ApprovalNameC,
                    ["ApprovalNameD"] = ApprovalNameD,
                    ["ApprovalNameE"] = ApprovalNameE,

                    ["FileName1"] = FileName1,
                    ["FileName2"] = FileName2,
                    ["FileName3"] = FileName3,
                    ["FileName4"] = FileName4,
                    ["FileName5"] = FileName5,
                    ["FileName6"] = FileName6,
                    ["FileName7"] = FileName7,
                };
                docxBytes = WordUtility.GenerateDocx(templat, data);
                MergeFilePath = Path.Combine(folder, $"提案需求評估表-{DateTime.Now:HHmmss}.docx");
                System.IO.File.WriteAllBytes(MergeFilePath, docxBytes);
                Thread.Sleep(1000);
                files.Add(MergeFilePath);
            }

            String[] filesArray = files.OrderBy(i => i).ToArray();

            string sDestFile = Path.Combine(folder, $"提案需求評估表.docx");
            string sEmptyFile = Path.Combine(Utils.GetTemplateFilePath(), $"提案需求評估表vf_Temp.docx");
            if (!System.IO.File.Exists(sDestFile))
            {
                System.IO.File.Copy(sEmptyFile, sDestFile);
            }
            if (filesArray.Length > 0)
            {
                //合併檔案
                result = WordUtility.CombineDocx(filesArray, sDestFile);
                Thread.Sleep(500);
                if (result)
                {
                    //刪除合併前來源檔案
                    foreach (string file in filesArray)
                    {
                        if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
                    }

                    //下載
                    //Download(sDestFile);

                    //刪除暫存資料夾
                    //Directory.Delete(DestFolderPath, true);

                    return sDestFile;
                }
            }
            return "";
        }

        //下載-提案審查表
        public ActionResult DownloadNeedAssessment(int year, int unit, int subUnit, int rptType)
        {

            try
            {
                List<EngReportVModel> engList = new List<EngReportVModel>();
                int total = engReportService.GetEngListCount(year, unit, subUnit, rptType, 1);
                if (total > 0)
                {
                    engList = engReportService.GetEngList<EngReportVModel>(year, unit, subUnit, rptType, total, 1, 9);
                }
                if (engList.Count == 0)
                {
                    return Json(new
                    {
                        result = -1,
                        msg = "工程資料錯誤",
                        JsonRequestBehavior.AllowGet

                    });
                }
                //暫存目錄
                string uuid = Guid.NewGuid().ToString("B").ToUpper();
                string folder = Path.Combine(Path.GetTempPath(), uuid);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                string destFile = CreateNeedAssessmentDoc(engList, folder);
                if (destFile == "")
                {
                    return Json(new
                    {
                        result = -1,
                        message = "提案審查表 製表失敗"
                    }, JsonRequestBehavior.AllowGet);
                }
                //Stream iStream = new FileStream(destFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                //return File(iStream, "application/blob", "提案審查表.docx");

                string zipFile = Path.Combine(Path.GetTempPath(), uuid + "-提案審查表.zip");
                System.IO.Compression.ZipFile.CreateFromDirectory(folder, zipFile);// AddFiles(files, "ProjectX");
                Stream iStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                return File(iStream, "application/blob", $"{DateTime.Now:yyyyMMddHHmmss}提案審查表.zip");
            }
            catch (Exception e)
            {
                BaseService.log.Info("CreateDoc NeedAssessment Err: " + e.Message);
            }

            return Json(new
            {
                result = -1,
                message = "請求錯誤"
            }, JsonRequestBehavior.AllowGet);
        }

        private string getImagePart(int Seq, string FileName) 
        {
            string contents="";

            if (!string.IsNullOrEmpty(FileName)) 
            {
                string sFile = Path.Combine(Utils.GetEngReportFolder(Seq), FileName);
                if (System.IO.File.Exists(sFile))
                {
                    contents = sFile;
                }
            }
            return contents;
        }
        private string CreateNeedAssessmentDoc(List<EngReportVModel> eng, string folder)
        {
            bool result = false;

            string destFile = Path.Combine(folder, $"提案審查表-{DateTime.Now:HHmmss}.docx");
            string srcFile = Path.Combine(Utils.GetTemplateFilePath(), "提案審查表.docx");
            //System.IO.File.Copy(srcFile, filename);

            byte[] templat = System.IO.File.ReadAllBytes(srcFile);

            List<string> files = new List<string>();
            byte[] docxBytes = null;
            Dictionary<string, string> data = null;
            string MergeFilePath = "";

            string FileName1, FileName2, FileName3, FileName4, FileName5, FileName6, FileName7;
            int ECPrice1, ECPrice2, ECPrice3, ECPrice4;
            string LC, SC;
            foreach (EngReportVModel item in eng)
            {
                FileName1 = string.IsNullOrEmpty(item.LocationMap) ? "" : getImagePart(item.Seq, item.LocationMap);
                FileName2 = string.IsNullOrEmpty(item.AerialPhotography) ? "" : getImagePart(item.Seq, item.AerialPhotography);
                FileName3 = string.IsNullOrEmpty(item.ScenePhoto) ? "" : getImagePart(item.Seq, item.ScenePhoto);
                FileName4 = string.IsNullOrEmpty(item.BaseMap) ? "" : getImagePart(item.Seq, item.BaseMap);
                FileName5 = string.IsNullOrEmpty(item.EngPlaneLayout) ? "" : getImagePart(item.Seq, item.EngPlaneLayout);
                FileName6 = string.IsNullOrEmpty(item.LongitudinalSection) ? "" : getImagePart(item.Seq, item.LongitudinalSection);
                FileName7 = string.IsNullOrEmpty(item.StandardSection) ? "" : getImagePart(item.Seq, item.StandardSection);

                //概估經費清單
                ECPrice1 = 0;
                ECPrice2 = 0;
                ECPrice3 = 0;
                ECPrice4 = 0;
                var ERListA = engReportService.GetEngReportEstimatedCostList<EngReportEstimatedCostVModel>(item.Seq);
                foreach (var itemA in ERListA) 
                {
                    switch (itemA.AttributesName) 
                    {
                        case "用地先期作業": ECPrice1 += Convert.ToInt32(Convert.ToInt32(itemA.Price) * 0.0001); break;
                        case "用地取得": ECPrice2 += Convert.ToInt32(Convert.ToInt32(itemA.Price) * 0.0001); break;
                        case "工程": ECPrice3 += Convert.ToInt32(Convert.ToInt32(itemA.Price) * 0.0001); break;
                        case "委託規劃設計": ECPrice4 += Convert.ToInt32(Convert.ToInt32(itemA.Price) * 0.0001); break;
                    }
                }
                //在地溝通辦理情形
                LC = "";
                var ERListB = engReportService.GetEngReportLocalCommunicationList<EngReportLocalCommunicationVModel>(item.Seq);
                foreach (var itemB in ERListB) 
                {
                    LC += (string.IsNullOrEmpty(LC) ? "" : "；") + $"{Utils.ChsDate(itemB.Date)}、{itemB.FileNumber}";
                }
                //在地諮詢辦理情形
                SC = "";
                var ERListC = engReportService.GetEngReportOnSiteConsultatioList<EngReportOnSiteConsultationVModel>(item.Seq);
                foreach (var itemC in ERListC)
                {
                    SC += (string.IsNullOrEmpty(SC) ? "" : "；") + $"{Utils.ChsDate(itemC.Date)}、{itemC.FileNumber}";
                }

                data = new Dictionary<string, string>()
                {
                    ["ExecUnit"] = item.ExecUnit,
                    ["RptName"] = item.RptName,
                    ["ProposalReviewTypeName"] = string.IsNullOrEmpty(item.ProposalReviewTypeName) ? "" : item.ProposalReviewTypeName,
                    ["ProposalReviewAttributes1"] = item.ProposalReviewAttributesSeq == 1 ? "■" : "□",
                    ["ProposalReviewAttributes2"] = item.ProposalReviewAttributesSeq == 2 ? "■" : "□",
                    ["ProposalReviewAttributes3"] = item.ProposalReviewAttributesSeq == 3 ? "■" : "□",
                    ["ProposalReviewAttributes4"] = item.ProposalReviewAttributesSeq == 4 ? "■" : "□",

                    ["RiverName1"] = string.IsNullOrEmpty(item.RiverName1) ? "" : item.RiverName1,
                    ["RiverName2"] = string.IsNullOrEmpty(item.RiverName2) ? "" : item.RiverName2,
                    ["RiverName3"] = string.IsNullOrEmpty(item.RiverName3) ? "" : item.RiverName3,
                    ["LargeSectionChainage"] = string.IsNullOrEmpty(item.LargeSectionChainage) ? "" : item.LargeSectionChainage,
                    ["CityName"] = string.IsNullOrEmpty(item.CityName) ? "" : item.CityName,
                    ["TownName"] = string.IsNullOrEmpty(item.TownName) ? "" : item.TownName,
                    ["CoordX"] = !item.CoordX.HasValue ? "" : item.CoordX.ToString(),
                    ["CoordY"] = !item.CoordY.HasValue ? "" : item.CoordY.ToString(),
                    ["EngineeringScale"] = string.IsNullOrEmpty(item.EngineeringScale) ? "" : item.EngineeringScale,

                    ["Coastal"] = string.IsNullOrEmpty(item.Coastal) ? "" : item.Coastal,
                    ["ProcessReason"] = string.IsNullOrEmpty(item.ProcessReason) ? "" : item.ProcessReason,
                    ["EngineeringScaleMemo"] = string.IsNullOrEmpty(item.EngineeringScaleMemo) ? "" : item.EngineeringScaleMemo,

                    ["RelatedReportContent"] = string.IsNullOrEmpty(item.RelatedReportContent) ? "" : item.RelatedReportContent,
                    ["HistoricalCatastrophe1"] = item.HistoricalCatastrophe == 1 ? "■" : "□",
                    ["HistoricalCatastropheMemo"] = string.IsNullOrEmpty(item.HistoricalCatastropheMemo) ? "" : item.HistoricalCatastropheMemo,
                    ["HistoricalCatastrophe2"] = item.HistoricalCatastrophe == 2 ? "■" : "□",

                    ["ECPrice1"] = ECPrice1.ToString(),
                    ["ECPrice2"] = ECPrice2.ToString(),
                    ["ECPrice3"] = ECPrice3.ToString(),
                    ["ECPrice4"] = ECPrice4.ToString(),
                    ["ProtectionTarget"] = string.IsNullOrEmpty(item.ProtectionTarget) ? "" : item.ProtectionTarget,

                    ["SetConditions"] = string.IsNullOrEmpty(item.SetConditions) ? "" : item.SetConditions,
                    ["LocalCommunication"] = LC,  //在地溝通辦理情形-日期、文號
                    ["OnSiteConsultation"] = SC,  //在地諮詢辦理情形-日期、文號
                    ["DemandCarbonEmissionsMemo"] = string.IsNullOrEmpty(item.DemandCarbonEmissionsMemo) ? "" : item.DemandCarbonEmissionsMemo,
                    ["DemandCarbonEmissions"] = !item.DemandCarbonEmissions.HasValue ? "" : item.DemandCarbonEmissions.ToString(),

                    ["LocationMap"] = FileName1,  //LocationMap
                    ["AerialPhotography"] = FileName2,  //空拍照
                    ["ScenePhoto"] = FileName3,  //現場照片
                    ["BaseMap"] = FileName4,  //基地地籍圖
                    ["EngPlaneLayout"] = FileName5,  //工程平面配置圖
                    ["LongitudinalSection"] = FileName6,  //縱斷面圖
                    ["StandardSection"] = FileName7,  //標準斷面圖
                };
                //主要工程內容
                int ListDCount = 0;
                var ERListD = engReportService.GetEngReportMainJobDescriptionList<EngReportMainJobDescriptionVModel>(item.Seq);
                foreach (var itemD in ERListD)
                {
                    ListDCount++;
                    data.Add($"RptJobName{ListDCount}", $"{itemD.RptJobDescriptionName} {itemD.OtherJobDescription}");
                    data.Add($"Num{ListDCount}", Convert.ToInt32(itemD.Num).ToString());
                    data.Add($"Cost{ListDCount}", Convert.ToInt32(Convert.ToInt32(itemD.Cost) * 0.0001).ToString());
                    data.Add($"Memo{ListDCount}", $"{itemD.Memo}");
                }
                for (int i = (ListDCount + 1); i <= 6; i++) 
                {
                    data.Add($"RptJobName{i}", "");
                    data.Add($"Num{i}", "");
                    data.Add($"Cost{i}", "");
                    data.Add($"Memo{i}", "");
                }

                docxBytes = WordUtility.GenerateDocx(templat, data);
                MergeFilePath = Path.Combine(folder, $"提案審查表-{item.RptName}.docx");
                System.IO.File.WriteAllBytes(MergeFilePath, docxBytes);
                Thread.Sleep(1000);
                files.Add(MergeFilePath);
            }

            String[] filesArray = files.OrderBy(i => i).ToArray();

            string sDestFile = Path.Combine(folder, $"提案審查表.docx");
            //string sEmptyFile = Path.Combine(Utils.GetTemplateFilePath(), $"提案需求評估表vf_Temp.docx");
            //if (!System.IO.File.Exists(sDestFile))
            //{
            //    System.IO.File.Copy(sEmptyFile, sDestFile);
            //}
            if (filesArray.Length > 0)
            {
                //合併檔案
                //result = WordUtility.CombineDocx(filesArray, sDestFile);
                Thread.Sleep(500);
                if (result)
                {
                    //刪除合併前來源檔案
                    //foreach (string file in filesArray)
                    //{
                    //    if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
                    //}

                    //下載
                    //Download(sDestFile);

                    //刪除暫存資料夾
                    //Directory.Delete(DestFolderPath, true);

                    return sDestFile;
                }
            }
            return sDestFile;
        }

        //覆核人員
        public JsonResult GetReviewUserList(int id,string UnitSubName)
        {
            List<EngReportVModel> engReport = engReportService.GetEegReportBySeq<EngReportVModel>(id);
            EngReportVModel item = engReport[0];
            

            List<SelectVM> items = engReportService.GetReviewUserList(item.ExecUnitSeq.ToString(), UnitSubName);
            return Json(items);
        }

        public void GetCollection(int year, int unit, int subUnit, int rptType)
        {
            FileStream fs = new FileStream(Path.Combine(Utils.GetTemplateFilePath(), "112先期檢討會議附件vf_SA.xlsx"), FileMode.Open, FileAccess.Read, FileShare.Read);
            int sheet = 2;
            ExcelProcesser processer = new ExcelProcesser(fs, 2);
            List<IGrouping<int, EngReportExcelVM>> engReportSheets;
            using(var context = new EQC_NEW_Entities())
            {
                
                engReportSheets = context.EngReportList
                    .ToList()
                    .Where(row => row.RptTypeSeq == rptType || rptType == 0)
                    .Select(row => new EngReportExcelVM { _row =row })
                    .Where(row => row.ReportTypeSeq > 0)
                    .Where(row => row._row.RptYear == year || year == -1)
                    .Where(row => row._row.ExecUnitSeq == unit || unit == -1)
                    .Where(row => row._row.ExecSubUnitSeq== subUnit || subUnit == -1)
                    .OrderBy(row => row.ReportTypeSeq)
                    .GroupBy(row => row.ReportTypeSeq)
                    .ToList();

                foreach (var report in engReportSheets)
                { 

                    processer.setSheet(report.Key);
                    processer.setStartRow(3);
                    List<int> sumRowIndex = new List<int>();
                    report
                        .OrderBy(row => row.ReportAttrSeq)
                        .OrderBy(row => row._row.ExecUnitSeq)
                        .GroupBy(row => row.ExecUnitName)

                        .ToList()
                        .ForEach(unitReport =>
                        {
                            int groupRowIndex = 0;
                            //... insert column
 
                            processer.InsertRow(unitReport.Count() + 1);
                            processer.insertOneCol(
                                unitReport.Select(r => $"{++groupRowIndex}")
                                , 0
                            );
                            int lastSumRow = sumRowIndex.Count > 0 ? sumRowIndex.Last() : 3;
                            string[] sumRow = null;

                            if (report.Key == 1)
                            {
                                processer.insertOneCol(
                                    unitReport.Select(r => r.ReportAttr), 1);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.ExecUnitName), 2);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.CityName), 3);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.TownName), 4);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.RiverName), 6);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.EngName), 7);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.WorkNumArr[0]), 8);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.WorkNumArr[1]), 9);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.WorkMemo), 10);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.ProtectionTarget), 11);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.Coord), 15);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.YearCost[0]), 22);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.YearCost[1]), 23);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.YearCost[2]), 24);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.YearCostTotal), 25);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.DemandCarbonEmissions), 26);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.Resolution), 28);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.ApprovedFund), 29);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.ApprovedCarbonEmissions), 30);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.Expenditure), 31);
                                sumRow = new string[32];
                                sumRow[31] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AF");
                                sumRow[30] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AE");
                                sumRow[29] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AD");
                                sumRow[28] = unitReport.Count(r => r._row.Resolution?.Contains("同意") ?? false).ToString();
                                sumRow[2] = unitReport.Key;

                            }
                            if (report.Key == 2)
                            {
                                processer.insertOneCol(
                                    unitReport.Select(r => r.ReportAttr), 1);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.ExecUnitName), 2);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.CityName), 3);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.TownName), 4);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.DrainName), 5);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.EngName), 6);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.WorkNumArr[0]), 7);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.WorkNumArr[1]), 8);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.WorkMemo), 9);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.ProtectionTarget), 10);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.Coord), 14);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.YearCost[0]), 21);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.YearCost[1]), 22);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.YearCost[2]), 23);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.YearCostTotal), 24);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.DemandCarbonEmissions), 25);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.Resolution), 27);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.ApprovedFund), 28);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.ApprovedCarbonEmissions), 29);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.Expenditure), 30);
                                sumRow = new string[32];
                                sumRow[30] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AE");
                                sumRow[29] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AD");
                                sumRow[28] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AC");
                                sumRow[27] = unitReport.Count(r => r._row.Resolution?.Contains("同意") ?? false).ToString();
                                sumRow[2] = unitReport.Key;
                            }
                            if (report.Key == 3 || report.Key == 4 || report.Key == 5)
                            {
                                processer.insertOneCol(
                                    unitReport.Select(r => r.ReportAttr), 1);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.ExecUnitName), 2);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.CityName), 3);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.TownName), 4);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.CreoleDetail), 6);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.EngName), 7);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.WorkNumArr[0]), 8);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.WorkNumArr[1]), 9);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.WorkMemo), 10);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.ProtectionTarget), 11);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.Coord), 15);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.YearCost[0]), 22);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.YearCost[1]), 23);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.YearCost[2]), 24);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.YearCostTotal), 25);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.DemandCarbonEmissions), 26);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.Resolution), 28);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.ApprovedFund), 29);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.ApprovedCarbonEmissions), 30);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.Expenditure), 31);
                                sumRow = new string[33];
                                sumRow[31] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AF");
                                sumRow[30] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AE");
                                sumRow[29] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AD");
                                sumRow[28] = unitReport.Count(r => r._row.Resolution?.Contains("同意") ?? false).ToString();
                                sumRow[2] = unitReport.Key;

                            }
                            if (report.Key == 6)
                            {
                                processer.insertOneCol(
                                    unitReport.Select(r => r.ReportAttr), 1);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.ExecUnitName), 2);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.CityName), 3);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.TownName), 4);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.CreoleDetail), 6);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.EngName), 7);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.WorkNumArr[0]), 8);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.WorkNumArr[1]), 9);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.WorkNumArr[2]), 10);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.WorkNumArr[3]), 11);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.WorkMemo), 12);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.ProtectionTarget), 13);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.Coord), 17);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.YearCost[0]), 24);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.YearCost[1]), 25);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.YearCost[2]), 26);
                                processer.insertOneCol(
                                    unitReport.Select(r => r.YearCostTotal), 27);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.DemandCarbonEmissions), 28);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.Resolution), 29);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.ApprovedFund), 30);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.ApprovedCarbonEmissions), 31);
                                processer.insertOneCol(
                                    unitReport.Select(r => r._row.Expenditure), 32);
                                sumRow = new string[34];
                                sumRow[32] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AG");
                                sumRow[31] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AF");
                                sumRow[30] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AE");
                                sumRow[29] =
                                    unitReport.Count(r => r._row.Resolution?.Contains("同意") ?? false).ToString();
                                sumRow[2] = unitReport.Key;
                            }
                            processer.fowardStartRow(groupRowIndex);

                            sumRowIndex.Add(lastSumRow + groupRowIndex + 1);
                            processer.insertRowContentV2(
                                sumRow
                                ,
                                HSSFColor.Yellow.Index,
                                HSSFColor.Black.Index
                            );

                        });


                    //processer.copyOutSideRowStyle(1, 0, 1);
                    //processer.insertRow(
                    //    totalRow
                    //    ,
                    //    HSSFColor.LightGreen.Index,
                    //    HSSFColor.Red.Index
                    //);

                    processer.evaluateSheet(0);
                    sheet++;
                }
            }

            Response.AddHeader("Content-Disposition", $"attachment; filename=先期檢討會議_提報_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            Response.BinaryWrite(processer.getTemplateStream().ToArray());
        }

        public void GetApprovedCollection(int year, int unit, int subUnit, int rptType)
        {
            FileStream fs = new FileStream(Path.Combine(Utils.GetTemplateFilePath(), "112先期檢討報部vf_SA.xlsx"), FileMode.Open, FileAccess.Read, FileShare.Read);
            ExcelProcesser processer = new ExcelProcesser(fs, 2);
            List<IGrouping<int, EngReportExcelVM>> engReportSheets;
            using (var context = new EQC_NEW_Entities())
            {

                engReportSheets = context.EngReportList
                    .Where(row => (rptType != 0 && row.RptTypeSeq == rptType ) || ( rptType == 0 && (  row.RptTypeSeq == 8|| row.RptTypeSeq == 7 ) ) )
                    .Select(row => new EngReportExcelVM { _row = row })
                    .Where(row => row._row.RptYear == year || year == -1)
                    .Where(row => row._row.ExecUnitSeq == unit || unit == -1)
                    .Where(row => row._row.ExecSubUnitSeq == subUnit || subUnit == -1)
                    .ToList()
                    .OrderBy(row => row.ReportTypeSeq)
                    .GroupBy(row => row.ReportTypeSeq)
                    .ToList();
                foreach(var report in engReportSheets)
                {
                    processer.setSheet(report.Key-1);
                    processer.setStartRow(3);
                    report
                        .OrderBy(row => row._row.ExecUnitSeq)
                        .OrderBy(row => row.ReportAttrSeq)
                        .GroupBy(row => row.ReportAttrSeq)
                        .ToList()
                        .ForEach(AttrReport =>
                        {
                            List<int> sumRowIndex = new List<int>();
                            string[] totalRow = null;
                            AttrReport

                                .GroupBy(r => r.ExecUnitName)
                                .ToList()
                                .ForEach(unitReport =>
                                {
                                    int groupRowIndex = 0;
                                    //... insert column

                                    processer.InsertRow(unitReport.Count() + 1);
                                    processer.insertOneCol(
                                        unitReport.Select(r => $"{++groupRowIndex}")
                                        , 0
                                    );
                                    int lastSumRow = sumRowIndex.Count > 0 ? sumRowIndex.Last() : processer.getStartRow();
                                    string[] sumRow = null;
                                    if (report.Key == 1)
                                    {
                                        sumRow = new string[35];
                                        processer.insertOneCol(unitReport.Select(r => r.ReportAttr), 1);
                                        processer.insertOneCol(unitReport.Select(r => r.ExecUnitName), 2);
                                        processer.insertOneCol(unitReport.Select(r => r.CityName), 3);
                                        processer.insertOneCol(unitReport.Select(r => r.RiverName), 6);
                                        processer.insertOneCol(unitReport.Select(r => r.EngName), 7);
                                        processer.insertOneCol(unitReport.Select(r => r.WorkNumArr[0]), 8);
                                        processer.insertOneCol(unitReport.Select(r => r.WorkNumArr[1]), 9);
                                        processer.insertOneCol(unitReport.Select(r => r.WorkMemo), 10);
                                        processer.insertOneCol(unitReport.Select(r => r._row.ApprovedFund), 29);
                                        processer.insertOneCol(unitReport.Select(r => r._row.EngNo), 31);
                                        processer.insertOneCol(unitReport.Select(r => r._row.Expenditure), 32);
                                        sumRow[29] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AD");
                                        sumRow[32] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AG");
                                    }
                                    if (report.Key == 2)
                                    {
                                        sumRow = new string[34];
                                        processer.insertOneCol(unitReport.Select(r => r.ReportAttr), 1);
                                        processer.insertOneCol(unitReport.Select(r => r.ExecUnitName), 2);
                                        processer.insertOneCol(unitReport.Select(r => r.CityName), 3);
                                        processer.insertOneCol(unitReport.Select(r => r.TownName), 4);
                                        processer.insertOneCol(unitReport.Select(r => r.DrainName), 5);
                                        processer.insertOneCol(unitReport.Select(r => r.EngName), 6);
                                        processer.insertOneCol(unitReport.Select(r => r.WorkNumArr[0]), 7);
                                        processer.insertOneCol(unitReport.Select(r => r.WorkNumArr[1]), 8);
                                        processer.insertOneCol(unitReport.Select(r => r.WorkMemo), 9);
                                        processer.insertOneCol(unitReport.Select(r => r._row.ApprovedFund), 28);
                                        processer.insertOneCol(unitReport.Select(r => r._row.EngNo), 30);
                                        processer.insertOneCol(unitReport.Select(r => r._row.Expenditure), 31);
                                        sumRow[28] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AC");
                                        sumRow[31] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AF");
                                    }
                                    if(report.Key == 3)
                                    {
                                        sumRow = new string[35];
                                        processer.insertOneCol(unitReport.Select(r => r.ReportAttr), 1);
                                        processer.insertOneCol(unitReport.Select(r => r.ExecUnitName), 2);
                                        processer.insertOneCol(unitReport.Select(r => r.CityName), 3);
                                        processer.insertOneCol(unitReport.Select(r => r.TownName), 4);
                                        processer.insertOneCol(unitReport.Select(r => r.CreoleDetail), 6);
                                        processer.insertOneCol(unitReport.Select(r => r.EngName), 7);
                                        processer.insertOneCol(unitReport.Select(r => r.WorkNumArr[0]), 8);
                                        processer.insertOneCol(unitReport.Select(r => r.WorkNumArr[1]), 9);
                                        processer.insertOneCol(unitReport.Select(r => r.WorkMemo), 10);
                                        processer.insertOneCol(unitReport.Select(r => r._row.ApprovedFund), 31);
                                        processer.insertOneCol(unitReport.Select(r => r._row.EngNo), 32);
                                        processer.insertOneCol(unitReport.Select(r => r._row.Expenditure), 33);
                                        sumRow[31] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AF");
                                        sumRow[33] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AH");
                                    }
                                    if (report.Key == 4)
                                    {
                                        sumRow = new string[35];
                                        processer.insertOneCol(unitReport.Select(r => r.ReportAttr), 1);
                                        processer.insertOneCol(unitReport.Select(r => r.ExecUnitName), 2);
                                        processer.insertOneCol(unitReport.Select(r => r.CityName), 3);
                                        processer.insertOneCol(unitReport.Select(r => r.TownName), 4);
                                        processer.insertOneCol(unitReport.Select(r => r.CreoleDetail), 6);
                                        processer.insertOneCol(unitReport.Select(r => r.EngName), 7);
                                        processer.insertOneCol(unitReport.Select(r => r.WorkNumArr[0]), 8);
                                        processer.insertOneCol(unitReport.Select(r => r.WorkNumArr[1]), 9);
                                        processer.insertOneCol(unitReport.Select(r => r.WorkMemo), 10);
                                        processer.insertOneCol(unitReport.Select(r => r._row.ApprovedFund), 29);
                                        processer.insertOneCol(unitReport.Select(r => r._row.EngNo), 30);
                                        processer.insertOneCol(unitReport.Select(r => r._row.Expenditure), 32);
                                        sumRow[29] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AD");
                                        sumRow[32] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AG");
                                    }
                                    if (report.Key == 5)
                                    {
                                        sumRow = new string[35];
                                        processer.insertOneCol(unitReport.Select(r => r.ReportAttr), 1);
                                        processer.insertOneCol(unitReport.Select(r => r.ExecUnitName), 2);
                                        processer.insertOneCol(unitReport.Select(r => r.CityName), 3);

                                        processer.insertOneCol(unitReport.Select(r => r.CreoleDetail), 6);
                                        processer.insertOneCol(unitReport.Select(r => r.EngName), 7);
                                        processer.insertOneCol(unitReport.Select(r => r.WorkNumArr[0]), 8);
                                        processer.insertOneCol(unitReport.Select(r => r.WorkNumArr[1]), 9);
                                        processer.insertOneCol(unitReport.Select(r => r.WorkMemo), 10);
                                        processer.insertOneCol(unitReport.Select(r => r._row.ApprovedFund), 29);
                                        processer.insertOneCol(unitReport.Select(r => r._row.EngNo), 30);
                                        processer.insertOneCol(unitReport.Select(r => r._row.Expenditure), 32);
                                        sumRow[29] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AD");
                                        sumRow[32] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AG");
                                    }
                                    if (report.Key == 6)
                                    {
                                        sumRow = new string[35];
                                        processer.insertOneCol(unitReport.Select(r => r.ReportAttr), 1);
                                        processer.insertOneCol(unitReport.Select(r => r.ExecUnitName), 2);
                                        processer.insertOneCol(unitReport.Select(r => r.CityName), 3);

                                        processer.insertOneCol(unitReport.Select(r => r.CreoleDetail), 6);
                                        processer.insertOneCol(unitReport.Select(r => r.EngName), 7);
                                        processer.insertOneCol(unitReport.Select(r => r.WorkNumArr[0]), 8);
                                        processer.insertOneCol(unitReport.Select(r => r.WorkNumArr[1]), 9);
                                        processer.insertOneCol(unitReport.Select(r => r.WorkNumArr[2]), 10);
                                        processer.insertOneCol(unitReport.Select(r => r.WorkNumArr[3]), 11);
                                        processer.insertOneCol(unitReport.Select(r => r.WorkMemo), 12);
                                        processer.insertOneCol(unitReport.Select(r => r._row.ApprovedFund), 30);
                                        processer.insertOneCol(unitReport.Select(r => r._row.EngNo), 31);
                                        processer.insertOneCol(unitReport.Select(r => r._row.Expenditure), 33);
                                        sumRow[30] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AE");
                                        sumRow[33] = processer.getSumFormula(lastSumRow + 1, groupRowIndex, "AH");
                                    }



                                    sumRow[2] = unitReport.Key;
                                    processer.fowardStartRow(groupRowIndex);

                                    sumRowIndex.Add(lastSumRow + groupRowIndex + 1);
                                    processer.insertRowContentV2(
                                        sumRow
                                        ,
                                        HSSFColor.Yellow.Index,
                                        HSSFColor.Black.Index
                                    );

                                });

                            if(report.Key == 1)
                            {
                                totalRow = new string[35];
                                totalRow[29] = processer.getPlusFormula(sumRowIndex, "AD");
                                totalRow[32] = processer.getPlusFormula(sumRowIndex, "AG");
                            }

                            if (report.Key == 2)
                            {
                                totalRow = new string[34];
                                totalRow[28] = processer.getPlusFormula(sumRowIndex, "AC");
                                totalRow[31] = processer.getPlusFormula(sumRowIndex, "AF");
                            }

                            if (report.Key == 3)
                            {
                                totalRow = new string[35];
                                totalRow[31] = processer.getPlusFormula(sumRowIndex, "AF");
                                totalRow[33] = processer.getPlusFormula(sumRowIndex, "AH");
                            }

                            if (report.Key == 4)
                            {
                                totalRow = new string[35];
                                totalRow[29] = processer.getPlusFormula(sumRowIndex, "AD");
                                totalRow[32] = processer.getPlusFormula(sumRowIndex, "AG");
                            }

                            if (report.Key == 5)
                            {
                                totalRow = new string[35];
                                totalRow[29] = processer.getPlusFormula(sumRowIndex, "AD");
                                totalRow[32] = processer.getPlusFormula(sumRowIndex, "AG");
                            }

                            if (report.Key == 6)
                            {
                                totalRow = new string[35];
                                totalRow[30] = processer.getPlusFormula(sumRowIndex, "AE");
                                totalRow[33] = processer.getPlusFormula(sumRowIndex, "AH");
                            }
                            processer.InsertRow(1);
                            processer.insertRowContentV2(
                                totalRow,
                                HSSFColor.LightGreen.Index,
                                HSSFColor.Black.Index
                            );

                        });
                }
            }
            Response.AddHeader("Content-Disposition", $"attachment; filename=先期檢討報部vf_SA{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            Response.BinaryWrite(processer.getTemplateStream().ToArray());
        }
    }
}

