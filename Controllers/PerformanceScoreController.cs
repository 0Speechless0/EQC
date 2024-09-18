using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EQC.Controllers.InterFaceForFrontEnd;
using EQC.EDMXModel;
using EQC.Controllers.Common;
using NPOI.SS.UserModel;

namespace EQC.Controllers
{
    public class PerformanceScoreController : ExcelImportController<PerformanceScore>
    {

        private static string[,] excelHeaderMap = new string[,]
        {
            { "PrjXMLSeq", "序號" },
            { "PSTotalScore", "履約計分總分" },
            { "PSIssueDate", "發文日期" },
            { "PSIssueNo", "發文文號" },
            { "PSCompleteAhead", "kb01提前竣工加分" },
            { "PSOverdueCompletion", "kb02逾期竣工減分" },
            { "PSAlterPlan", "kc01替代方案加分" },
            { "PSLiquidDamageAmount", "kc02逾期違約金" },
            { "PSAcceptanceBonus", "kd01驗收加分" },
            { "PSAcceptanceMinus", "kd02驗收減分" },
            { "PSConstWork", "kd03施工作業" },
            { "PSAuditBonus", "kd04查核加分" },
            { "PSAuditMinus", "kd05查核減分" },
            { "PSFullTimeEngineerBonus", "kd06專任工程人員加分" },
            { "PSFullTimeEngineerMinus", "kd07專任工程人員減分" },
            { "PSTechnicianBonus", "kd08技術士加分" },
            { "PSQualityAwardBonus", "kd09品質獲獎" },
            { "PSLaborSafetyAwardBonus", "ke01勞安獲獎加分" },
            { "PSNoneOccupDisaBonus", "ke02無職業災害加分" },
            { "PSOccupDisaMinus", "ke03發生職災減分" },
            { "PSEnvirProtectBonus", "ke04環保加分" },
            { "PSPublicBulletinBonus", "kf01民眾通報加分" },
            { "PSSuspensionMinus", "kf02停權減分" },
        };


        public ActionResult Index()
        {
            return View();
        }
        private static Dictionary<string ,int>  prjNameDic = new Dictionary<string, int>();
        public PerformanceScoreController() :
        base(
            "PerformanceScore", 
            "PrjXMLSeq",
            excelHeaderMap,
            (context, id) => context.PerformanceScore.Find(id),
            (performanceScore) => performanceScore.Seq ,
            (context) => context.PerformanceScore.ToList() ,
            (performanceScore, str) => performanceScore.PSIssueNo.Contains(str),
            (process) =>
            {
                using (var context = new EQC_NEW_Entities())
                {
                    prjNameDic = context.PrjXML.ToDictionary(r => r.TenderNo + r.ExecUnitCd, r => r.Seq);
                    foreach (IRow row in process.sheet)
                    {
                        var seqCell = row.GetCell(0);
                        if (!Int32.TryParse(seqCell.ToString(), out int result)) continue;
                        var orgC = row.GetCell(4).ToString() + row.GetCell(2).ToString();
                        if (prjNameDic.TryGetValue(orgC, out int prjSeq))
                        {
                            seqCell.SetCellValue(prjSeq);
                        }
                        else
                        {
                            seqCell.SetCellValue(0);

                        }
                    }

                }
            })
        {

        }


    }
}