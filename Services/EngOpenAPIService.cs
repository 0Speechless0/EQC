using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using EQC.Common;
using EQC.EDMXModel;

namespace EQC.Services
{
    public class EngOpenAPIService :BaseService
    {

        
        public List<object> GetEngDataForOpenNetWork(DateTime? startTime, DateTime? endTime)
        {
            string sql = @"
                Select ee.*  from (
                    Select 

                    e.Seq, 
                    e.EngNo, 
                    e.EngName,
                    e.EngYear,
                    p.ExecUnitCd,
                    p.TenderNo,
                    p.TenderName,
				    p.ActualBidReviewDate,
                    p.ActualBidAwardDate,
				    ISNULL(p.TownName, '') TownName,
				    p.EngOverview,
				    p.SupervisionUnitName,
				    p.ContractorName1,
                    px.ActualCompletionDate,
                    px.BelongPrj,
                    u.Name ExecUnitName,
                    pr.PDExecState,
                    p.BidAmount,
				    pr.PDAccuScheProgress,
				    pr.PDAccuActualProgress,
                    [PDAccuScheProgressRow] = ROW_NUMBER() over(partition by e.EngNo order by pr.CreateTime desc )   ,
                    [PDAccuActualProgressRow] = ROW_NUMBER() over(partition by e.EngNo order by pr.CreateTime desc) ,
                    p.ScheStartDate,
                    p.ActualStartDate,
                    p.ScheCompletionDate,
                    p.CoordX,
                    p.CoordY,
                    eco.Stage Stage1,
                    eco2.Stage Stage2,
                    eco.ToDoChecklit ToDoChecklit1,
                    eco2.ToDoChecklit ToDoChecklit2,
                    eco.PlanDesignRecordFilename,
                    eco.ConservMeasFilename,
				    eco.SOCFilename,
				    eco.MemberDocFilename,
                    eco.DataCollectDocFilename,
                    eco.EngCreatureNameList,
                    eco2.PlanDesignRecordFilename BuildedPlanDesignRecordFilename,
                    eco2.ConservMeasFilename BuildedConservMeasFilename,
				    eco2.SOCFilename  BuildedSOCFilename,
				    eco2.DataCollectDocFilename BuildedMemberDocFilename,
				    eco2.EngDiagram,
				    eco2.ChecklistFilename,
				    eco2.LivePhoto,
				    eco2.Other,
				    eco2.Other2,
                    eco.ModifyTime ModifyTime1,
                    eco2.ModifyTime ModifyTime2
                    from EngMain e 
                    left join Unit u on (u.Seq = e.ExecUnitSeq and u.ParentSeq is null)
                    left join PrjXML p on e.PrjXMLSeq = p.Seq  
                    left join PrjXMLExt px on p.Seq = px.PrjXMLSeq
                    left join ProgressData pr on pr.PrjXMLSeq = p.Seq
                    left join EcologicalChecklist eco on (eco.EngMainSeq = e.Seq and eco.Stage = 1)
                    left join EcologicalChecklist  eco2 on (eco2.EngMainSeq = e.Seq and eco2.Stage = 2)
                    where ( eco2.Stage is null and ( eco.ModifyTime >= @startDate and eco.ModifyTime <= @endDate )  ) or (eco2.Stage is not null  and ( eco2.ModifyTime >= @startDate and eco2.ModifyTime <= @endDate ) )
                ) ee where ee.PDAccuActualProgressRow = 1 and ee.PDAccuScheProgressRow = 1
                and ee.EngName is not null and ee.EngYear >= 111
                order by ee.ModifyTime1 desc, ee.ModifyTime2 desc
                
            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@startDate", startTime ?? DateTime.MinValue.AddYears(1912));
            cmd.Parameters.AddWithValue("@endDate", endTime ?? DateTime.MaxValue);
            string baseUrl = HttpContext.Current.Request.Url.Scheme + "://"
                + (ConfigurationManager.AppSettings["SelfEvalHost"]?.ToString() ?? HttpContext.Current.Request.Url.Authority);

            string remotePath;
            string EcologicalCheckPath;
            string EcologicalCheckPath2;
            if (ConfigurationManager.AppSettings["SelfEvalPath"]?.ToString() == null)
            {
                remotePath = HttpContext.Current.Server.MapPath("~");
                EcologicalCheckPath = Path.Combine(remotePath, "FileUploads/EcologicalCheck");
                EcologicalCheckPath2 = Path.Combine(remotePath, "FileUploads/EcologicalCheck2");
            }
            else
            {
                remotePath = Path.Combine(HttpContext.Current.Server.MapPath("~"), ConfigurationManager.AppSettings["SelfEvalPath"]?.ToString());

                EcologicalCheckPath = Path.Combine(remotePath, "FileUploads/EcologicalCheck");
                EcologicalCheckPath2 = Path.Combine(remotePath, "FileUploads/EcologicalCheck2");
            }

            string fullUrl = baseUrl + "/" + "FileUploads/";



            return db.GetDataTable(cmd).Rows.Cast<DataRow>().Select(row => {

                string checkFilePath1 = "";
                string checkFilePath2 = "";
                List<string> SelfEvalUrl = new List<string>();
                List<string> BuildedSelfEvalUrl = new List<string>();
                string fileName;

                //設計階段 : 公共工程
                if (Directory.Exists(EcologicalCheckPath + "/" + row.Field<object>("Seq") + "/SelfEvalFiles"))
                {
                    SelfEvalUrl =
                        Directory.GetFiles(EcologicalCheckPath + "/" + row.Field<object>("Seq") + "/SelfEvalFiles")
                        .Select(row2 => fullUrl + "EcologicalCheck/" + row.Field<object>("Seq") + "/SelfEvalFiles/" + Uri.EscapeDataString(Path.GetFileName(row2)))
                        .ToList();
                }

                //施工階段 : 公共工程
                if (Directory.Exists(EcologicalCheckPath2 + "/" + row.Field<object>("Seq") + "/SelfEvalFiles"))
                {
                    BuildedSelfEvalUrl =
                        Directory.GetFiles(EcologicalCheckPath2 + "/" + row.Field<object>("Seq") + "/SelfEvalFiles")
                        .Select(row2 => fullUrl + "EcologicalCheck2/" + row.Field<object>("Seq") + "/SelfEvalFiles/" + Uri.EscapeDataString(Path.GetFileName(row2)))
                        .ToList();
                }


                string PlanDesignRecordUrl = null;
                string PlanDesignRecordBuildedUrl = null;

                //設計階段 : 民眾參與
                fileName = row.Field<string>("PlanDesignRecordFilename") ?? "";
                checkFilePath1 = Path.Combine(EcologicalCheckPath, row.Field<int>("Seq").ToString(), fileName);
                if (File.Exists(checkFilePath1))
                {
                    string _fileName = Uri.EscapeDataString(fileName);
                    PlanDesignRecordUrl = fullUrl + "EcologicalCheck/" + row.Field<object>("Seq") + "/" + _fileName;
                }

                //施工階段: 民眾參與
                fileName = row.Field<string>("BuildedPlanDesignRecordFilename") ?? "";
                checkFilePath2 = Path.Combine(EcologicalCheckPath2, row.Field<int>("Seq").ToString(), fileName);
                if (File.Exists(checkFilePath2))
                {
                    string _fileName = Uri.EscapeDataString(fileName);
                    PlanDesignRecordBuildedUrl = fullUrl + "EcologicalCheck2/" + row.Field<object>("Seq") + "/" + _fileName;
                }



                string ConservMeasUrl = null;
                string ConservMeasBuildedUrl = null;

                //設計階段: 生態保育措施研擬
                fileName = row.Field<string>("ConservMeasFilename") ?? "";
                checkFilePath1 = Path.Combine(EcologicalCheckPath, row.Field<int>("Seq").ToString(), fileName);
                if (File.Exists(checkFilePath1))
                {
                    string _fileName = Uri.EscapeDataString(fileName);
                    ConservMeasUrl = fullUrl + "EcologicalCheck/" + row.Field<object>("Seq") + "/" + _fileName;
                }

                //設計階段: 工程生態背景資料表
                string DataCollectDocFilenameUrl = null;
                fileName = row.Field<string>("DataCollectDocFilename") ?? "";
                checkFilePath1 = Path.Combine(EcologicalCheckPath, row.Field<int>("Seq").ToString(), fileName);
                if (File.Exists(checkFilePath1))
                {
                    string _fileName = Uri.EscapeDataString(fileName);
                    DataCollectDocFilenameUrl = fullUrl + "EcologicalCheck/" + row.Field<object>("Seq") + "/" + _fileName;
                }
                //施工階段 : 生態調查評析
                fileName = row.Field<string>("BuildedConservMeasFilename") ?? "";
                checkFilePath2 = Path.Combine(EcologicalCheckPath2, row.Field<int>("Seq").ToString(), fileName);
                if (File.Exists(checkFilePath2))
                {
                    string _fileName = Uri.EscapeDataString(fileName);
                    ConservMeasBuildedUrl = fullUrl + "EcologicalCheck2/" + row.Field<object>("Seq") + "/" + _fileName;
                }

                string SOCFilenameUrl = null;
                string SOCFilenameBuildedUrl = null;

                //設計階段 : 生態調查評析表
                fileName = row.Field<string>("SOCFilename") ?? "";
                checkFilePath1 = Path.Combine(EcologicalCheckPath, row.Field<int>("Seq").ToString(), fileName);
                if (File.Exists(checkFilePath1))
                {
                    string _fileName = Uri.EscapeDataString(fileName);
                    SOCFilenameUrl = fullUrl + "EcologicalCheck/" + row.Field<object>("Seq") + "/" + _fileName;
                }

                //施工階段 : 生態保育措施抽查表
                fileName = row.Field<string>("BuildedSOCFilename") ?? "";
                checkFilePath2 = Path.Combine(EcologicalCheckPath2, row.Field<int>("Seq").ToString(), fileName);
                if (File.Exists(checkFilePath2))
                {
                    string _fileName = Uri.EscapeDataString(fileName);
                    SOCFilenameBuildedUrl = fullUrl + "EcologicalCheck2/" + row.Field<object>("Seq") + "/" + _fileName;
                }

                string MemberDocFilenameUrl = null;
                string MemberDocFilenameBuildedUrl = null;


                //設計階段 :現場勘查
                fileName = row.Field<string>("MemberDocFilename") ?? "";
                checkFilePath1 = Path.Combine(EcologicalCheckPath, row.Field<int>("Seq").ToString(), fileName);
                if (File.Exists(checkFilePath1))
                {
                    string _fileName = Uri.EscapeDataString(fileName);
                    MemberDocFilenameUrl = fullUrl + "EcologicalCheck/" + row.Field<object>("Seq") + "/" + _fileName;
                }
                //設計階段:工程範圍生物名錄
                string EngCreatureNameListUrl = null;
                fileName = row.Field<string>("EngCreatureNameList") ?? "";
                checkFilePath1 = Path.Combine(EcologicalCheckPath, row.Field<int>("Seq").ToString(), fileName);
                if (File.Exists(checkFilePath1))
                {
                    string _fileName = Uri.EscapeDataString(fileName);
                    EngCreatureNameListUrl = fullUrl + "EcologicalCheck1/" + row.Field<object>("Seq") + "/" + _fileName;
                }

                //施工階段:現場勘查
                fileName = row.Field<string>("BuildedMemberDocFilename") ?? "";
                checkFilePath2 = Path.Combine(EcologicalCheckPath2, row.Field<int>("Seq").ToString(), fileName);
                if (File.Exists(checkFilePath2))
                {
                    string _fileName = Uri.EscapeDataString(fileName);
                    MemberDocFilenameBuildedUrl = fullUrl + "EcologicalCheck2/" + row.Field<object>("Seq") + "/" + _fileName;
                }

                //施工階段:前置作業
                string EngDiagramUrl = null;
                fileName = row.Field<string>("EngDiagram") ?? "";
                checkFilePath2 = Path.Combine(EcologicalCheckPath2, row.Field<int>("Seq").ToString(), fileName);
                if (File.Exists(checkFilePath2))
                {
                    string _fileName = Uri.EscapeDataString(fileName);
                    EngDiagramUrl = fullUrl + "EcologicalCheck2/" + row.Field<object>("Seq") + "/" + _fileName;
                }

                //施工階段: 生態保育措施自主檢查表
                string ChecklistFilenameUrl = null;
                fileName = row.Field<string>("ChecklistFilename") ?? "";
                checkFilePath2 = Path.Combine(EcologicalCheckPath2, row.Field<int>("Seq").ToString(), fileName);
                if (File.Exists(checkFilePath2))
                {
                    string _fileName = Uri.EscapeDataString(fileName);
                    ChecklistFilenameUrl = fullUrl + "EcologicalCheck2/" + row.Field<object>("Seq") + "/" + _fileName;
                }

                //施工階段:環境狀態異常
                string LivePhotoUrl = null;
                fileName = row.Field<string>("LivePhoto") ?? "";
                checkFilePath2 = Path.Combine(EcologicalCheckPath2, row.Field<int>("Seq").ToString(), fileName);
                if (File.Exists(checkFilePath2))
                {
                    string _fileName = Uri.EscapeDataString(fileName);
                    LivePhotoUrl = fullUrl + "EcologicalCheck2/" + row.Field<object>("Seq") + "/" + _fileName;
                }

                //施工階段:事項報告表
                string OtherUrl = null;
                fileName = row.Field<string>("Other") ?? "";
                checkFilePath2 = Path.Combine(EcologicalCheckPath2, row.Field<int>("Seq").ToString(), fileName);
                if (File.Exists(checkFilePath2))
                {
                    string _fileName = Uri.EscapeDataString(fileName);
                    OtherUrl = fullUrl + "EcologicalCheck2/" + row.Field<object>("Seq") + "/" + _fileName;
                }

                //施工階段:事項彙整表
                string Other2Url = null;
                fileName = row.Field<string>("Other2") ?? "";
                checkFilePath2 = Path.Combine(EcologicalCheckPath2, row.Field<int>("Seq").ToString(), fileName);
                if (File.Exists(checkFilePath2))
                {
                    string _fileName = Uri.EscapeDataString(fileName);
                    Other2Url = fullUrl + "EcologicalCheck2/" + row.Field<object>("Seq") + "/" + _fileName;
                }

            return new
                {
                    Seq = row.Field<object>("Seq"),
                    EngYear = row.Field<object>("EngYear"),
                    TenderNo = row.Field<object>("EngNo"),
                    TenderName = row.Field<object>("EngName"),
                    TownName = row.Field<object>("TownName"),
                    SupervisionUnitName = row.Field<object>("SupervisionUnitName"),
                    ContractorName1 = row.Field<object>("ContractorName1"),
                    ActualBidReviewDate = ROCDateStrHandler(row.Field<string>("ActualBidReviewDate")),
                    ActualCompletionDate = ROCDateStrHandler(row.Field<string>("ActualCompletionDate")),
                    EngOverview = row.Field<object>("EngOverview"),
                    BelongPrj = row.Field<object>("BelongPrj"),
                    //ExecUnitCd = row.Field<object>("ExecUnitCd"),
                    ExecUnitName = row.Field<object>("ExecUnitName"),
                    PDExecState = row.Field<object>("PDExecState"),
                    BidAmount = row.Field<object>("BidAmount"),
                    PDAccuScheProgress = row.Field<object>("PDAccuScheProgress"),
                    PDAccuActualProgress = row.Field<object>("PDAccuActualProgress"),
                    ScheStartDate = ROCDateStrHandler(row.Field<string>("ScheStartDate")),
                    ActualStartDate = ROCDateStrHandler(row.Field<string>("ActualStartDate")),
                    ActualBidAwardYear = row.Field<string>("ActualBidAwardDate")?.Substring(0, row.Field<string>("ActualBidAwardDate").Count() >=  3 ? 3: 0 ),
                    ToDoCheckList = row.Field<byte?>("ToDoChecklit1"),
                    //BuildedToDoCheckList = row.Field<byte?>("ToDoChecklit2"),
                    ScheCompletionDate = ROCDateStrHandler(row.Field<string>("ScheCompletionDate")),
                    CoordX = row.Field<object>("CoordX"),
                    CoordY = row.Field<object>("CoordY"),
                    SelfEvalUrl = SelfEvalUrl,
                    DataCollectDocFilenameUrl = DataCollectDocFilenameUrl,
                    PlanDesignRecordUrl = PlanDesignRecordUrl,
                    ConservMeasUrl = ConservMeasUrl,
                    BuildedSelfEvalUrl = BuildedSelfEvalUrl,
                    PlanDesignRecordBuildedUrl = PlanDesignRecordBuildedUrl,
                    ConservMeasBuildedUrl = ConservMeasBuildedUrl,
                    SOCFilenameUrl = SOCFilenameUrl,
                    SOCFilenameBuildedUrl = SOCFilenameBuildedUrl,
                    MemberDocFilenameUrl = MemberDocFilenameUrl,
                    MemberDocFilenameBuildedUrl = MemberDocFilenameBuildedUrl,
                    EngDiagramUrl = EngDiagramUrl,
                    ChecklistFilenameUrl = ChecklistFilenameUrl,
                    LivePhotoUrl = LivePhotoUrl,
                    OtherUrl = OtherUrl,
                    Other2Url = Other2Url,
                    EngCreatureNameListUrl = EngCreatureNameListUrl,
                    EcoState = ( row.Field<byte?>("Stage2") ?? ( row.Field<byte?>("Stage1" ) ??  0)  ) +1 ,
                    DesignModifyTime = EngDateTime(row.Field<DateTime?>("ModifyTime1")),
                    BuildedModifyTime = EngDateTime( row.Field<DateTime?>("ModifyTime2"))

                };
            }).ToList<object>();

        }

        

        public List<object> GetEngReportEC(DateTime? startTime, DateTime? endTime)
        {
            using(var context =new EQC_NEW_Entities())
            {
                string baseUrl = HttpContext.Current.Request.Url.Scheme + "://"
                    +  HttpContext.Current.Request.Url.Authority;
                string urlTemplate = baseUrl + "/FileUploads/EngReport/{0}/{1}" ;
                return context.EngReportList
                    .Where(r => r.RptYear >= 111)
                    .Where(r => r.CreateTime >= startTime || startTime == null)
                    .Where(r => r.CreateTime <= endTime || endTime == null)
                    .ToList()
                    .Select(r =>
                        new
                        {
                            EngName = r.RptName,
                            EngNo = r.EngNo,
                            EngYear = r.RptYear,
                            EngReportECD01 = r.EcologicalConservationD01 != null ? String.Format(urlTemplate, r.Seq, Uri.EscapeDataString(r.EcologicalConservationD01) ) : null,
                            EngReportECD02 = r.EcologicalConservationD02 != null ? String.Format(urlTemplate, r.Seq, Uri.EscapeDataString(r.EcologicalConservationD02)) : null,
                            EngReportECD03 = r.EcologicalConservationD03 != null ? String.Format(urlTemplate, r.Seq, Uri.EscapeDataString(r.EcologicalConservationD03)) : null,
                            EngReportECD04 = r.EcologicalConservationD04 != null ? String.Format(urlTemplate, r.Seq, Uri.EscapeDataString(r.EcologicalConservationD04)) : null,
                            EngReportECD05 = r.EcologicalConservationD05 != null ? String.Format(urlTemplate, r.Seq, Uri.EscapeDataString(r.EcologicalConservationD05)) : null,
                            EngReportECD06 = r.EcologicalConservationD06 != null ? String.Format(urlTemplate, r.Seq, Uri.EscapeDataString(r.EcologicalConservationD06)) : null

                        }).Cast<object>().ToList();
            }
        }
        public List<object> GetGIS()
        {
            using (var context = new EQC_NEW_Entities())
            {
                var list = context.PrjXML
                    .ToList()
                    .Where(r => r.EngMain.FirstOrDefault()?.EngYear >= 110 && r.ProgressData.LastOrDefault()?.PDAccuActualProgress < 100);
                return list
                .GroupJoin(context.ProgressData, 
                    prj => prj.Seq, 
                    progress => progress.PrjXMLSeq,
                    (prj, progress) => new
                    {

                        TenderNo = prj.TenderNo,
                        TenderName = prj.TenderName,
                        ExecUnitName = prj.ExecUnitName,
                        BidAmount = prj.BidAmount,
                        ScheCompletionDate = prj.ScheCompletionDate,
                        ActualStartDate = prj.ActualStartDate,
                        CoordX = prj.CoordX,
                        CoordY = prj.CoordY,
                        PDExecState = progress.Count() > 0 ?  progress.Last().PDExecState : null,
                        PDAccuScheProgress = progress.Count() > 0 ? progress.Last().PDAccuScheProgress : null,
                        PDAccuActualProgress = progress.Count() > 0 ?  progress.Last().PDAccuActualProgress : null

                    }
                
                    )
                .ToList<object>();
            }
        }
    }
}