using EQC.Common;
using EQC.EDMXModel;
using EQC.Models;
using EQC.Services;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EQC.Controllers
{
    [SessionFilter]
    public class EADPlaneWeaknessController : MyController
    {//品質管制弱面追蹤與分析
        EADPlaneWeaknessService iService = new EADPlaneWeaknessService();
        public ActionResult Index()
        {
            Utils.setUserClass(this);
            return View("Index");
        }

        public void ExportExcel(string units, int sYear, int eYear, string places, bool isSupervise, string minBid, string maxBid)
        {

            List<string> placesList = places.Split(',').ToList();
            var unitsSqlStr = units.Contains(",") ?
                units.Split(',').Select(r => $"'{r}'").Aggregate("", (a, c) => a + "," + c).Substring(1) : "";
            var excelProcess = new ExcelProcesser(Path.Combine(Utils.GetTemplateFilePath(), "工程弱面.xlsx"), 0);
            excelProcess.setStartRow(2);
            List<EADPlaneWeaknessEngVModel> engList = iService.GetList<EADPlaneWeaknessEngVModel>(unitsSqlStr, sYear, eYear, placesList, isSupervise, null, minBid, maxBid);
            engList.Where(e => e.PlaneWeakness.Length > 0)
                .ToList().ForEach(e => e.PlaneWeakness = "," + e.PlaneWeakness);
            string uuid = Guid.NewGuid().ToString("B").ToUpper();
            string folder = Path.Combine(Utils.GetTempFolderForUser(), uuid);
            Detection.DownloadTaskDetection.AddTaskQueneToRun(() => {

                using (var context = new EQC_NEW_Entities())
                {
                    var exportEngList = engList.Join(
                        context.PrjXML, r1 => r1.Seq, r2 => r2.Seq,
                        (r1, r2) => new EADPlaneWeaknessEngExcelVModel
                        {
                            EADPlaneWeaknessEng = r1,
                            prjXML = r2,
                            progressData = r2.ProgressData.LastOrDefault()
                        });
                    excelProcess.copyOutSideRowStyle(0, 2, exportEngList.Count());
                    int i = 1;
                    var farRegions = context.FarRegion.Select(r => r.TownCity).ToHashSet();
                    excelProcess.insertOneCol(exportEngList.Select(r => i++), 0);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.CompetentAuthority), 1);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.OrganizerName), 1);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.ExecUnitName), 3);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.TenderNo), 4);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.TenderName), 5);
                    excelProcess.insertOneCol(exportEngList.Select(r => Utils.ChsDateFormat(r.prjXML.ActualBidAwardDate)), 6);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.FundingSourceName), 7);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.TotalEngBudget), 8);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.OutsourcingBudget), 9);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.RendBasePrice), 10);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.PrjXMLExt.DesignChangeContractAmount ?? r.prjXML.BidAmount), 11);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.PrjXMLExt.DesignChangeContractAmount), 12);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.EADPlaneWeaknessEng.PlaneWeaknessNumArr.Contains(1) ? "V" : ""), 15);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.EADPlaneWeaknessEng.PlaneWeaknessNumArr.Contains(2) ? "V" : ""), 16);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.EADPlaneWeaknessEng.PlaneWeaknessNumArr.Contains(3) ? "V" : ""), 17);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.EADPlaneWeaknessEng.PlaneWeaknessNumArr.Contains(4) ? "V" : ""), 18);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.EADPlaneWeaknessEng.PlaneWeaknessNumArr.Contains(5) ? "V" : ""), 19);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.EADPlaneWeaknessEng.PlaneWeaknessNumArr.Contains(6) ? "V" : ""), 20);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.EADPlaneWeaknessEng.PlaneWeaknessNumArr.Contains(7) ? "V" : ""), 21);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.EADPlaneWeaknessEng.PlaneWeaknessNumArr.Contains(8) ? "V" : ""), 22);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.EADPlaneWeaknessEng.PlaneWeaknessNumArr.Contains(9) ? "V" : ""), 23);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.EADPlaneWeaknessEng.PlaneWeaknessNumArr.Contains(10) ? "V" : ""), 24);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.EADPlaneWeaknessEng.PlaneWeaknessNumArr.Contains(11) ? "V" : ""), 25);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.EADPlaneWeaknessEng.PlaneWeaknessNumArr.Contains(12) ? "V" : ""), 26);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.EADPlaneWeaknessEng.PlaneWeaknessNumArr.Contains(13) ? "V" : ""), 27);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.EADPlaneWeaknessEng.PlaneWeaknessNumArr.Contains(14) ? "V" : ""), 28);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.EADPlaneWeaknessEng.PlaneWeaknessNumArr.Count ), 29);
                    excelProcess.insertOneCol(exportEngList.Select(r => farRegions.Contains(r.prjXML.TownName) ? "V" : "" ), 30);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.PrjXMLExt.AuditDate != null ? "V" : ""), 31);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.PrjXMLExt.Score), 32);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.TenderName.Contains("疏濬")  ? "V" : "" ), 36);
                    excelProcess.insertOneCol(exportEngList.Select(r => (r.prjXML.TenderName.Contains("開口契約") || r.prjXML.TenderName.Contains("開口合約"))  ? "V" : ""), 37);
                    excelProcess.insertOneCol(exportEngList.Select(r => (r.prjXML.TenderName.Contains("搶險") || r.prjXML.TenderName.Contains("搶修") )  ? "V" : "" )  , 38);
                    excelProcess.insertOneCol(exportEngList.Select(r => (r.prjXML.TenderName.Contains("土石採售") || r.prjXML.TenderName.Contains("土石標售")  ? "V" : "") ), 39);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.TenderName.Contains("河道整理")  ? "V" : "" ), 40);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.ContractorName1), 41);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.PlanningUnitName), 42);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.DesignUnitName), 43);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.SupervisionUnitName), 44);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.PrjXMLExt.PrjManageUnit), 45);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.EngType), 46);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.EngOverview), 47);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.TownName), 48);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.Location), 49);
                    excelProcess.insertOneCol(exportEngList.Select(r => Utils.ChsDateFormat(r.prjXML.ScheStartDate)), 50);
                    excelProcess.insertOneCol(exportEngList.Select(r => Utils.ChsDateFormat(r.prjXML.ActualStartDate)), 51);
                    excelProcess.insertOneCol(exportEngList.Select(r => Utils.ChsDateFormat(r.prjXML.ScheCompletionDate)), 52);
                    excelProcess.insertOneCol(exportEngList.Select(r => Utils.ChsDateFormat(r.prjXML.PrjXMLExt.ScheChangeCloseDate)), 53);
                    excelProcess.insertOneCol(exportEngList.Select(r => $"{r.progressData.PDYear}/{r.progressData.PDMonth}"), 54);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.progressData.PDAccuScheProgress), 55);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.progressData.PDAccuActualProgress), 56);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.progressData.DiffProgress), 57);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.progressData.PDExecState), 58);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.progressData.PDAccuScheCloseAmount), 59);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.progressData.PDAccuActualCloseAmount), 60);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.progressData.PDAccuEstValueAmount), 61);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.progressData.PDAccountPayable), 62);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.progressData.LatePaymentReason), 63);
                    excelProcess.insertOneCol(exportEngList.Select(r => Utils.ChsDateFormat(r.prjXML.PrjXMLExt.ActualCompletionDate)), 64);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.ContactName), 65);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.ContactPhone), 66);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.ContactEmail), 67);
                    excelProcess.insertOneCol(exportEngList.Select(r => r.prjXML.PrjXMLExt.BelongPrj), 68);
                    //Response.AddHeader("Content-Disposition", $"attachment; filename={HttpUtility.UrlEncode($"工程弱面{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}.xlsx") }");
                    //Response.BinaryWrite(excelProcess.getTemplateStream().ToArray());
                    excelProcess.SaveToFile(folder, $"工程弱面{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.xlsx");
                    excelProcess.Disopse();
                }

            }, Utils.getUserSeq());

            ResponseJson(new { message = "已背景執行中..." });

           

        }
        //工程清單
        public virtual JsonResult GetList(List<string> units, int sYear, int eYear, List<string> places, bool isSupervise, string pw, string minBid, string maxBid)
        {
            string unitList = "";
            if (units != null)
            {
                string sp = "";
                foreach (string item in units)
                {
                    unitList += String.Format("{0}'{1}'", sp, item);
                    sp = ",";
                }
            }

            List<EADPlaneWeaknessEngVModel> engList = iService.GetList<EADPlaneWeaknessEngVModel>(unitList, sYear, eYear, places, isSupervise, pw, minBid, maxBid);
            return Json(new
            {
                items = engList
            });
        }
        //標案年分
        public JsonResult GetYearOptions()
        {
            List<EngYearVModel> items = iService.GetEngYearList();//s20230726
            return Json(items);
        }
    }
}