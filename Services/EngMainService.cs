using EQC.Common;
using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace EQC.Services
{
    public class EngMainService : BaseService
    {//工程主檔
        //取消工程連結標案 shioulo 20220518
        public int CancelEngLinkTender(int engMainSeq)
        {
            string sql = @"update EngMain set PrjXMlSeq=null where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", engMainSeq);
            return db.ExecuteNonQuery(cmd);
        }
        //工程連結標案 shioulo 20220518
        public int SetEngLinkTender(int engMainSeq, int prjXMLSeq)
        {
            string sql = @"SELECT Seq FROM EngMain where Seq<>@Seq and PrjXMLSeq=@PrjXMLSeq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", engMainSeq);
            cmd.Parameters.AddWithValue("@PrjXMlSeq", prjXMLSeq);
            DataTable dt = db.GetDataTable(cmd);
            if (dt.Rows.Count > 0) return -1;

            sql = @"update EngMain set PrjXMlSeq=@PrjXMlSeq where Seq=@Seq";
            cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", engMainSeq);
            cmd.Parameters.AddWithValue("@PrjXMLSeq", prjXMLSeq);
            return db.ExecuteNonQuery(cmd);
        }
        public List<object> GetEngDataForOpenNetWork()
        {
            string sql = @"
                Select 

                e.Seq, 
                e.EngNo, 
                p.TenderNo,
                p.TenderName,
				p.ActualBidReviewDate,
				p.TownName,
				p.EngOverview,
				p.SupervisionUnitName,
				p.ContractorName1,
                px.BelongPrj,
                p.ExecUnitName,
                pr.PDExecState,
                p.BidAmount,
                pr.PDAccuScheProgress,
                pr.PDAccuActualProgress,
                p.ScheStartDate,
                p.ActualStartDate,
                p.ScheCompletionDate,
                p.CoordX,
                p.CoordY,
                eco.SelfEvalFilename,
                eco.PlanDesignRecordFilename,
                eco.ConservMeasFilename,
				eco.SOCFilename,
				eco.MemberDocFilename,
                eco.DataCollectDocFilename,
                eco2.SelfEvalFilename BuildedSelfEvalFilename,
                eco2.PlanDesignRecordFilename BuildedPlanDesignRecordFilename,
                eco2.ConservMeasFilename BuildedConservMeasFilename,
				eco2.SOCFilename  BuildedSOCFilename,
				eco2.DataCollectDocFilename BuildedMemberDocFilename,
				eco2.EngDiagram,
				eco2.ChecklistFilename,
				eco2.LivePhoto,
				eco2.Other,
				eco2.Other2
                from EngMain e 
                inner join PrjXML p on e.PrjXMLSeq = p.Seq
                inner join PrjXMLExt px on p.Seq = px.PrjXMLSeq
                inner join ProgressData pr on pr.PrjXMLSeq = p.Seq
                left join EcologicalChecklist eco on (eco.EngMainSeq = e.Seq and eco.Stage = 1)
                left join EcologicalChecklist  eco2 on (eco2.EngMainSeq = e.Seq and eco2.Stage = 2)
                
            ";
            SqlCommand cmd = db.GetCommand(sql);
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

                string checkFilePath1 = Path.Combine(EcologicalCheckPath, row.Field<int>("Seq").ToString(), row.Field<string>("SelfEvalFilename") ?? "");
                string checkFilePath2 = Path.Combine(EcologicalCheckPath2, row.Field<int>("Seq").ToString(), row.Field<string>("SelfEvalFilename") ?? "");
                List<string> SelfEvalUrl = new List<string>();
                List<string> SelfEvalBuildedUrl = new List<string>();
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
                    SelfEvalBuildedUrl =
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
                    EngNo = row.Field<object>("EngNo"),
                    TenderNo = row.Field<object>("TenderNo"),
                    TenderName = row.Field<object>("TenderName"),
                    BelongPrj = row.Field<object>("BelongPrj"),
                    ExecUnitName = row.Field<object>("ExecUnitName"),
                    PDExecState = row.Field<object>("PDExecState"),
                    BidAmount = row.Field<object>("BidAmount"),
                    PDAccuScheProgress = row.Field<object>("PDAccuScheProgress"),
                    PDAccuActualProgress = row.Field<object>("PDAccuActualProgress"),
                    ScheStartDate = ROCDateStrHandler(row.Field<string>("ScheStartDate")),
                    ActualStartDate = ROCDateStrHandler(row.Field<string>("ActualStartDate")),
                    ScheCompletionDate = ROCDateStrHandler(row.Field<string>("ScheCompletionDate")),
                    CoordX = row.Field<object>("CoordX"),
                    CoordY = row.Field<object>("CoordY"),
                    SelfEvalUrl = SelfEvalUrl,
                    DataCollectDocFilenameUrl = DataCollectDocFilenameUrl,
                    PlanDesignRecordUrl = PlanDesignRecordUrl,
                    ConservMeasUrl = ConservMeasUrl,
                    SelfEvalBuildedUrl = SelfEvalBuildedUrl,
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
                    Other2Url = Other2Url

                };
            }).ToList<object>();

        }
        public List<T> GetItemBySeq<T>(int seq)
        {
            string sql = @"
                SELECT
                    a.ExecType,
                    a.SupervisorExecType,
                    a.Seq,
                    a.EngYear,
                    a.EngNo,
                    a.EngName,
                    a.OrganizerUnitSeq,
                    a.ExecUnitSeq,
                    a.DesignUnitName,
                    a.DesignManName,
                    a.DesignUnitTaxId,
                    a.DesignUnitEmail,
                    a.TotalBudget,
                    a.SubContractingBudget,
                    a.ContractAmountAfterDesignChange,
                    a.PurchaseAmount,
                    a.ProjectScope,
                    a.EngTownSeq,
                    a.EngPeriod,
                    a.StartDate,
                    a.SchCompDate,
                    a.PostCompDate,
                    a.ApproveDate,
                    a.ApproveNo,
                    a.AwardAmount,
                    a.BuildContractorName,
                    a.BuildContractorContact,
                    a.BuildContractorTaxId,
                    a.BuildContractorEmail,
                    a.IsNeedElecDevice,
                    a.SupervisorUnitName,
                    a.SupervisorDirector,
                    a.SupervisorTechnician,
                    a.SupervisorTaxid,
                    a.SupervisorContact,
                    a.SupervisorSelfPerson1,
                    a.SupervisorSelfPerson2,
                    a.SupervisorCommPerson4,
                    a.SupervisorCommPersion2,
                    a.SupervisorCommPerson3,
                    a.SupervisorCommPerson4LicenseExpires,
                    a.SupervisorCommPerson3LicenseExpires,
                    a.EngChangeStartDate,
                    a.WarrantyExpires,
                    a.ConstructionDirector,
                    a.ConstructionPerson1,
                    a.ConstructionPerson2,
                    a.OrganizerSubUnitSeq,
                    a.ExecSubUnitSeq,
                    a.OrganizerUserSeq,
                    a.AwardDate,
                    a.CarbonDemandQuantity,
                    a.ApprovedCarbonQuantity,
                    a.OfficialApprovedCarbonQuantity,
                    a.DredgingEng,
                    b.CitySeq,
                    c.Name organizerUnitName,
                    c1.Name execUnitName,
                    c2.Name execSubUnitName,
                    d.DocState,
                    a.PrjXMLSeq,
                    e.TenderNo,
                    e.TenderName,
                    e.OrganizerName tenderOrgUnitName,
                    e.ExecUnitName tenderExecUnitName,
                    e.DurationCategory,
                    e.BidAmount,
                    ISNULL(f.Seq, 0) PCCESSMainSeq
                FROM EngMain a
                left outer join Town b on(b.Seq=a.EngTownSeq)
                left outer join Unit c on(c.Seq=a.OrganizerUnitSeq)
                left outer join Unit c1 on(c1.Seq=a.ExecUnitSeq)
                left outer join Unit c2 on(c2.Seq=a.ExecSubUnitSeq)
                left outer join SupervisionProjectList d on(
                    d.EngMainSeq=a.Seq
                    and d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                )
                left outer join PrjXML e on(e.Seq=a.PrjXMLSeq)
                left outer join PCCESSMain f on(f.contractNo=a.EngNo) -- s20230829
                where
                    a.Seq=@Seq
            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //帶入預設值處理 s20220826 暫不使用
        /*public List<T> GetItemBySeq<T>(int seq)
        {
            string sql = @"
            select
                IIF(z.hideExecType=0, cast(IIF(z.execUnitName=z.DesignUnitName, 1, 2) as tinyint), z.hideExecType) ExecType,
                IIF(z.hideSupervisorExecType=0, cast(IIF(z.execUnitName=z.SupervisorUnitName, 1, 2) as tinyint), z.hideSupervisorExecType) SupervisorExecType,
                z.*
            from (
                SELECT
                    a.ExecType hideExecType,
                    a.SupervisorExecType hideSupervisorExecType,
                    a.Seq,
                    a.EngYear,
                    a.EngNo,
                    a.EngName,
                    a.OrganizerUnitSeq,
                    a.ExecUnitSeq,
                    IIF(ISNULL(a.DesignUnitName,'')='', e.DesignUnitName, a.DesignUnitName) DesignUnitName,
                    a.DesignManName,
                    a.DesignUnitTaxId,
                    a.DesignUnitEmail,
                    a.TotalBudget,
                    a.SubContractingBudget,
                    IIF(ISNULL(a.ContractAmountAfterDesignChange,-1)=-1, cast(ext.DesignChangeContractAmount as decimal(18,0)), a.ContractAmountAfterDesignChange) ContractAmountAfterDesignChange,
                    a.PurchaseAmount,
                    IIF(ISNULL(a.ProjectScope,'')='', e.EngOverview, a.ProjectScope) ProjectScope,
                    a.EngTownSeq,
                    IIF(ISNULL(a.EngPeriod,-1)=-1, e.TotalDays, a.EngPeriod) EngPeriod,
                    IIF(ISNULL(a.StartDate,'')='', dbo.ChtDate2Date(e.ActualStartDate), a.StartDate) StartDate,
                    IIF(ISNULL(a.SchCompDate,'')='', dbo.ChtDate2Date(e.ScheCompletionDate), a.SchCompDate) SchCompDate,
                    IIF(ISNULL(a.PostCompDate,'')='', dbo.ChtDate2Date(ext.ScheChangeCloseDate), a.PostCompDate) PostCompDate,
                    a.ApproveDate,
                    a.ApproveNo,
                    IIF(ISNULL(a.AwardAmount,-1)=-1, cast(e.BidAmount as decimal(18,0)), a.AwardAmount) AwardAmount,
                    IIF(ISNULL(a.BuildContractorName,'')='', e.ContractorName1, a.BuildContractorName) BuildContractorName,
                    a.BuildContractorContact,
                    a.BuildContractorTaxId,
                    a.BuildContractorEmail,
                    a.IsNeedElecDevice,
                    IIF(ISNULL(a.SupervisorUnitName,'')='', e.SupervisionUnitName, a.SupervisorUnitName) SupervisorUnitName,
                    a.SupervisorDirector,
                    a.SupervisorTechnician,
                    a.SupervisorTaxid,
                    a.SupervisorContact,
                    a.SupervisorSelfPerson1,
                    a.SupervisorSelfPerson2,
                    a.SupervisorCommPerson4,
                    a.SupervisorCommPersion2,
                    a.SupervisorCommPerson3,
                    a.SupervisorCommPerson4LicenseExpires,
                    a.SupervisorCommPerson3LicenseExpires,
                    a.EngChangeStartDate,
                    a.WarrantyExpires,
                    a.ConstructionDirector,
                    a.ConstructionPerson1,
                    a.ConstructionPerson2,
                    a.OrganizerSubUnitSeq,
                    a.ExecSubUnitSeq,
                    a.OrganizerUserSeq,
                    IIF(ISNULL(a.AwardDate,'')='', dbo.ChtDate2Date(e.ActualBidAwardDate), a.AwardDate) AwardDate,
                    b.CitySeq,
                    c.Name organizerUnitName,
                    c1.Name execUnitName,
                    c2.Name execSubUnitName,
                    d.DocState,
                    a.PrjXMLSeq,
                    e.TenderNo,
                    e.TenderName,
                    e.OrganizerName tenderOrgUnitName,
                    e.ExecUnitName tenderExecUnitName,
                    e.DurationCategory
                FROM EngMain a
                left outer join Town b on(b.Seq=a.EngTownSeq)
                left outer join Unit c on(c.Seq=a.OrganizerUnitSeq)
                left outer join Unit c1 on(c1.Seq=a.ExecUnitSeq)
                left outer join Unit c2 on(c2.Seq=a.ExecSubUnitSeq)
                left outer join SupervisionProjectList d on(
                    d.EngMainSeq=a.Seq
                    and d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                )
                left outer join PrjXML e on(e.Seq=a.PrjXMLSeq)
                left outer join PrjXMLExt ext on(ext.PrjXMLSeq=a.PrjXMLSeq)
                where
                    a.Seq=@Seq
            ) z";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }*/
        public List<T> GetEngSeqByEngNo<T>(string engNo)
        {
            string sql = @"
                SELECT Seq FROM EngMain
                where EngNo=@engNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@engNo", engNo);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //s20231103
        public List<T> CheckEngNo<T>(string engNo)
        {
            string sql = @"
                SELECT Seq FROM EngMain where EngNo=@engNo
                union all
                SELECT Seq FROM PCCESSMain where contractNo=@engNo
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@engNo", engNo);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //查詢工程 s20230527
        public List<T> SearchEngByEngNoOrName<T>(string keyword)
        {
            string sql = @"
                SELECT Seq, EngNo, EngName FROM EngMain
                where EngNo like @engNo
                or EngName like @EngName";
            keyword = "%" + keyword + "%";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@engNo", keyword.ToUpper());
            cmd.Parameters.AddWithValue("@EngName", keyword);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetItemPccesFileBySeq<T>(int seq)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    a.EngNo,
                    a.EngName,
                    a.PccesXMLFile,
                    a.PccesXMLDate
                FROM EngMain a
                where a.Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        public int Update(EngMainEditVModel m)
        {
            Null2Empty(m);
            //
            string sql = @"Select Seq from EngSupervisor 
                where UserKind = 2 and EngMainSeq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", m.Seq);
            DataTable dt = db.GetDataTable(cmd);
            bool addMode = dt.Rows.Count == 0;

            //
            db.BeginTransaction();
            try
            {
                if (!String.IsNullOrEmpty(m.EngNo))
                {//s20231114 同步變更
                    sql = @"
                        Update PCCESSMain Set
                            contractNo = @contractNo
                        where contractNo=(select EngNo from EngMain where Seq=@Seq)
                    ";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.AddWithValue("@Seq", m.Seq);
                    cmd.Parameters.AddWithValue("@contractNo", m.EngNo);
                    db.ExecuteNonQuery(cmd);
                }

                sql = @"
                update EngMain set
                    EngYear = @EngYear,
                    EngNo = @EngNo,
                    EngName = @EngName,
                    OrganizerUnitSeq = @OrganizerUnitSeq,
                    OrganizerSubUnitSeq = @OrganizerSubUnitSeq,
                    OrganizerUserSeq = @OrganizerUserSeq,
                    ExecUnitSeq = @ExecUnitSeq,
                    ExecSubUnitSeq = @ExecSubUnitSeq,
                    DesignUnitName = @DesignUnitName,
                    DesignManName = @DesignManName,
                    DesignUnitTaxId = @DesignUnitTaxId,
                    DesignUnitEmail = @DesignUnitEmail,
                    TotalBudget = @TotalBudget,
                    SubContractingBudget = @SubContractingBudget,
                    ContractAmountAfterDesignChange = @ContractAmountAfterDesignChange,
                    PurchaseAmount = @PurchaseAmount,
                    ExecType = @ExecType,
                    ProjectScope = @ProjectScope,
                    EngTownSeq = @EngTownSeq,
                    EngPeriod = @EngPeriod,
                    StartDate = @StartDate,
                    SchCompDate = @SchCompDate,
                    PostCompDate = @PostCompDate,
                    ApproveDate = @ApproveDate,
                    ApproveNo = @ApproveNo,
                    AwardAmount = @AwardAmount,
                    BuildContractorName = @BuildContractorName,
                    BuildContractorContact = @BuildContractorContact,
                    BuildContractorTaxId = @BuildContractorTaxId,
                    BuildContractorEmail = @BuildContractorEmail,
                    IsNeedElecDevice = @IsNeedElecDevice,
                    SupervisorExecType = @SupervisorExecType,
                    SupervisorUnitName = @SupervisorUnitName,
                    SupervisorDirector = @SupervisorDirector,
                    SupervisorTechnician = @SupervisorTechnician,
                    SupervisorTaxid = @SupervisorTaxid,
                    SupervisorContact = @SupervisorContact,
                    SupervisorSelfPerson1 = @SupervisorSelfPerson1,
                    SupervisorSelfPerson2 = @SupervisorSelfPerson2,
                    SupervisorCommPerson1 = @SupervisorCommPerson4+','+@SupervisorCommPerson3,
                    SupervisorCommPersion2 = @SupervisorCommPersion2,
                    SupervisorCommPerson3 = @SupervisorCommPerson3,
                    SupervisorCommPerson4 = @SupervisorCommPerson4,
                    SupervisorCommPerson4LicenseExpires = @SupervisorCommPerson4LicenseExpires,
                    SupervisorCommPerson3LicenseExpires = @SupervisorCommPerson3LicenseExpires,
                    EngChangeStartDate = @EngChangeStartDate,
                    WarrantyExpires = @WarrantyExpires,
                    ConstructionDirector = @ConstructionDirector,
                    ConstructionPerson1 = @ConstructionPerson1,
                    ConstructionPerson2 = @ConstructionPerson2,
                    AwardDate = @AwardDate,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq,
                    PrjXMLSeq=@PrjXMLSeq,
                    CarbonDemandQuantity = @CarbonDemandQuantity,
                    ApprovedCarbonQuantity = @ApprovedCarbonQuantity,
                    DredgingEng = @DredgingEng
                where Seq=@Seq";

                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngNo", m.EngNo);
                cmd.Parameters.AddWithValue("@EngYear", m.EngYear);
                cmd.Parameters.AddWithValue("@EngName", m.EngName);
                cmd.Parameters.AddWithValue("@OrganizerUnitSeq", this.NulltoDBNull(m.OrganizerUnitSeq));
                cmd.Parameters.AddWithValue("@OrganizerSubUnitSeq", this.NulltoDBNull(m.OrganizerSubUnitSeq));
                cmd.Parameters.AddWithValue("@OrganizerUserSeq", this.NulltoDBNull(m.OrganizerUserSeq));
                cmd.Parameters.AddWithValue("@ExecUnitSeq", this.NulltoDBNull(m.ExecUnitSeq));
                cmd.Parameters.AddWithValue("@ExecSubUnitSeq", this.NulltoDBNull(m.ExecSubUnitSeq));
                cmd.Parameters.AddWithValue("@DesignUnitName", m.DesignUnitName);
                cmd.Parameters.AddWithValue("@DesignManName", m.DesignManName);
                cmd.Parameters.AddWithValue("@DesignUnitTaxId", m.DesignUnitTaxId);
                cmd.Parameters.AddWithValue("@DesignUnitEmail", m.DesignUnitEmail);
                cmd.Parameters.AddWithValue("@TotalBudget", this.NulltoDBNull(m.TotalBudget));
                cmd.Parameters.AddWithValue("@SubContractingBudget", this.NulltoDBNull(m.SubContractingBudget));
                cmd.Parameters.AddWithValue("@ContractAmountAfterDesignChange", this.NulltoDBNull(m.ContractAmountAfterDesignChange));
                cmd.Parameters.AddWithValue("@PurchaseAmount", this.NulltoDBNull(m.PurchaseAmount));
                cmd.Parameters.AddWithValue("@ExecType", this.NulltoDBNull(m.ExecType));
                cmd.Parameters.AddWithValue("@ProjectScope", m.ProjectScope);
                cmd.Parameters.AddWithValue("@EngTownSeq", this.NulltoDBNull(m.EngTownSeq));
                cmd.Parameters.AddWithValue("@EngPeriod", this.NulltoDBNull(m.EngPeriod));
                cmd.Parameters.AddWithValue("@StartDate", this.NulltoDBNull(m.StartDate));
                cmd.Parameters.AddWithValue("@SchCompDate", this.NulltoDBNull(m.SchCompDate));
                cmd.Parameters.AddWithValue("@PostCompDate", this.NulltoDBNull(m.PostCompDate));
                cmd.Parameters.AddWithValue("@ApproveDate", this.NulltoDBNull(m.ApproveDate));
                cmd.Parameters.AddWithValue("@ApproveNo", m.ApproveNo);
                cmd.Parameters.AddWithValue("@AwardAmount", this.NulltoDBNull(m.AwardAmount));
                cmd.Parameters.AddWithValue("@BuildContractorName", m.BuildContractorName);
                cmd.Parameters.AddWithValue("@BuildContractorContact", m.BuildContractorContact);
                cmd.Parameters.AddWithValue("@BuildContractorTaxId", m.BuildContractorTaxId);
                cmd.Parameters.AddWithValue("@BuildContractorEmail", m.BuildContractorEmail);
                cmd.Parameters.AddWithValue("@IsNeedElecDevice", m.IsNeedElecDevice);
                cmd.Parameters.AddWithValue("@SupervisorExecType", m.SupervisorExecType);
                cmd.Parameters.AddWithValue("@SupervisorUnitName", m.SupervisorUnitName);
                cmd.Parameters.AddWithValue("@SupervisorDirector", m.SupervisorDirector);
                cmd.Parameters.AddWithValue("@SupervisorTechnician", m.SupervisorTechnician);
                cmd.Parameters.AddWithValue("@SupervisorTaxid", m.SupervisorTaxid);
                cmd.Parameters.AddWithValue("@SupervisorContact", m.SupervisorContact);
                cmd.Parameters.AddWithValue("@SupervisorSelfPerson1", m.SupervisorSelfPerson1);
                cmd.Parameters.AddWithValue("@SupervisorSelfPerson2", m.SupervisorSelfPerson2);
                cmd.Parameters.AddWithValue("@SupervisorCommPerson4", m.SupervisorCommPerson4);
                cmd.Parameters.AddWithValue("@SupervisorCommPersion2", m.SupervisorCommPersion2);
                cmd.Parameters.AddWithValue("@SupervisorCommPerson3", m.SupervisorCommPerson3);
                cmd.Parameters.AddWithValue("@SupervisorCommPerson4LicenseExpires", this.NulltoDBNull(m.SupervisorCommPerson4LicenseExpires));
                cmd.Parameters.AddWithValue("@SupervisorCommPerson3LicenseExpires", this.NulltoDBNull(m.SupervisorCommPerson3LicenseExpires));
                cmd.Parameters.AddWithValue("@EngChangeStartDate", this.NulltoDBNull(m.EngChangeStartDate));
                cmd.Parameters.AddWithValue("@WarrantyExpires", this.NulltoDBNull(m.WarrantyExpires));
                cmd.Parameters.AddWithValue("@ConstructionDirector", m.ConstructionDirector);
                cmd.Parameters.AddWithValue("@ConstructionPerson1", m.ConstructionPerson1);
                cmd.Parameters.AddWithValue("@ConstructionPerson2", m.ConstructionPerson2);
                cmd.Parameters.AddWithValue("@AwardDate", this.NulltoDBNull(m.AwardDate));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@PrjXMLSeq", this.NulltoDBNull(m.PrjXMLSeq));//shioulo 20220504
                cmd.Parameters.AddWithValue("@CarbonDemandQuantity", this.NulltoDBNull(m.CarbonDemandQuantity));//s20230418
                cmd.Parameters.AddWithValue("@ApprovedCarbonQuantity", this.NulltoDBNull(m.ApprovedCarbonQuantity));//s20230418
                cmd.Parameters.AddWithValue("@DredgingEng", m.DredgingEng);//s20231006

                db.ExecuteNonQuery(cmd);
                if (m.PrjXMLSeq.HasValue && m.AwardDate.HasValue)
                {//s20230327
                    sql = @"
                        update PrjXML set
                            ActualBidAwardDate = @ActualBidAwardDate,
                            ModifyTime = GETDATE(),
                            ModifyUserSeq = @ModifyUserSeq
                        where Seq=@Seq";

                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@ActualBidAwardDate", String.Format("{0}{1}", m.AwardDate.Value.Year - 1911, m.AwardDate.Value.ToString("MMdd")));
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    cmd.Parameters.AddWithValue("@Seq", m.PrjXMLSeq.Value);
                    db.ExecuteNonQuery(cmd);
                }
                if (addMode)
                {
                    sql = @"
                        Insert into EngSupervisor(EngMainSeq, UserKind, SubUnitSeq, UserMainSeq) 
                        values (
                            @engMainSeq,
                            @userKind,
                            @subUnitSeq,
                            @userMainSeq
                        )";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.AddWithValue("@engMainSeq", m.Seq);
                    cmd.Parameters.AddWithValue("@userKind", 2);
                    cmd.Parameters.AddWithValue("@subUnitSeq", m.ExecSubUnitSeq);
                    cmd.Parameters.AddWithValue("@userMainSeq", m.OrganizerUserSeq);
                    db.ExecuteNonQuery(cmd);
                }
                else
                {
                    sql = @"
                        Update EngSupervisor 
                        Set UserMainSeq = @userMainSeq,
                            SubUnitSeq = @subUnitSeq
                        where EngMainSeq = @engMainSeq
                        and UserKind = 2
                    ";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.AddWithValue("@engMainSeq", m.Seq);
                    cmd.Parameters.AddWithValue("@userMainSeq", m.OrganizerUserSeq);
                    cmd.Parameters.AddWithValue("@subUnitSeq", m.ExecSubUnitSeq);
                    db.ExecuteNonQuery(cmd);
                }
                //shioulo 20230130
                if (!String.IsNullOrEmpty(m.BuildContractorTaxId))
                {
                    sql = @"
                        Update UserMain Set
                            Email = @Email,
                            DisplayName = @DisplayName
                        where UserNo=@UserNo
                    ";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.AddWithValue("@UserNo", "C" + m.BuildContractorTaxId);
                    cmd.Parameters.AddWithValue("@Email", m.BuildContractorEmail);
                    cmd.Parameters.AddWithValue("@DisplayName", m.BuildContractorContact);
                    db.ExecuteNonQuery(cmd);
                }
                if (!String.IsNullOrEmpty(m.DesignUnitTaxId))
                {
                    sql = @"
                        Update UserMain Set
                            Email = @Email,
                            DisplayName = @DisplayName
                        where UserNo=@UserNo
                    ";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.AddWithValue("@UserNo", "D" + m.DesignUnitTaxId);
                    cmd.Parameters.AddWithValue("@Email", m.DesignUnitEmail);
                    cmd.Parameters.AddWithValue("@DisplayName", m.DesignManName);
                    db.ExecuteNonQuery(cmd);
                }
                if (!String.IsNullOrEmpty(m.SupervisorTaxid))
                {
                    sql = @"
                        Update UserMain Set
                            Email = @Email,
                            DisplayName = @DisplayName
                        where UserNo=@UserNo
                    ";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.AddWithValue("@UserNo", "S" + m.SupervisorTaxid);
                    cmd.Parameters.AddWithValue("@Email", m.SupervisorContact);
                    cmd.Parameters.AddWithValue("@DisplayName", m.SupervisorDirector);
                    db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return 1;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("EngMainService.Update: " + e.Message);
                return -1;
            }
        }

        //取得鄉鎮 Seq
        public int? GetEngTownSeq(string cityTown)
        {
            string sql = @"
                SELECT
                    a.seq EngTownSeq from Town a
                inner join City b on(b.Seq=a.CitySeq)
                where (b.CityName+a.TownName)=@cityTown
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@cityTown", cityTown.Replace("台", "臺"));
            DataTable dt = db.GetDataTable(cmd);
            if (dt.Rows.Count == 1)
            {
                return Convert.ToInt32(dt.Rows[0]["EngTownSeq"].ToString());
            }
            else
            {
                return null;
            }
        }
        //刪除進度管理內所有資料:<br />預計進度, 監造/施工日誌, 估驗請款, 物價調整款 shioulo20221027
        public bool DelEng1(int seq, string engNo)
        {
            int commandTimeout = 600;
            string sql = "";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                //工程變更 20230808
                sql = @"
                    --工程變更 估驗請款
                    delete EC_AskPaymentPayItem where EC_AskPaymentHeaderSeq in (select Seq from EC_AskPaymentHeader where EngMainSeq=@EngMainSeq);
                    delete EC_AskPaymentHeader where EngMainSeq=@EngMainSeq;

                    --工程變更 物價調整款
                    delete EC_EngPriceAdjWorkItem where EC_EngPriceAdjSeq in(
                        select Seq from EC_EngPriceAdj where EngMainSeq=@EngMainSeq
                    );
                    delete EC_EngPriceAdjLockWorkItem  where EngMainSeq=@EngMainSeq;
                    delete EC_EngPriceAdj where EngMainSeq=@EngMainSeq;
 
                    --工程變更 日誌
                    delete EC_SchProgressPayItem
                    where EC_SchEngProgressPayItemSeq in (
                        select Seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in(
                            select Seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                        )
                    )
                    delete EC_SupDailyReportMiscConstruction where EC_SupDailyDateSeq in (select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete EC_SupDailyReportMisc where EC_SupDailyDateSeq in (select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete EC_SupDailyReportConstructionPerson where EC_SupDailyDateSeq in (select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete EC_SupDailyReportConstructionMaterial where EC_SupDailyDateSeq in (select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete EC_SupDailyReportConstructionEquipment where EC_SupDailyDateSeq in (select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete EC_SupPlanOverview where EC_SupDailyDateSeq in (select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete EC_SupDailyDate where EngMainSeq=@EngMainSeq;

                    delete EC_SchEngProgressWorkItem where EC_SchEngProgressPayItemSeq in (
                        select seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in (
                            select seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                        )
                    );
                    delete EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in (
                        select seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                    );

                    delete EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq;
                        ";
                cmd = db.GetCommand(sql);
                cmd.CommandTimeout = commandTimeout;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", seq);
                db.ExecuteNonQuery(cmd);

                //s20230927
                sql = @"
                    DECLARE @tmp_SupDailyReportWork table (Seq int)
                    INSERT INTO @tmp_SupDailyReportWork(Seq)
                    select SupDailyReportWorkSeq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq;

                    DECLARE @tmp_SupDailyReportExtension table (Seq int)
                    INSERT INTO @tmp_SupDailyReportExtension(Seq)
                    select SupDailyReportExtensionSeq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq;

                    delete SupDailyReportWork where Seq in (select Seq from @tmp_SupDailyReportWork);
                    delete SupDailyReportExtension where Seq in (select Seq from @tmp_SupDailyReportExtension);
                    ";
                cmd = db.GetCommand(sql);
                cmd.CommandTimeout = commandTimeout;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", seq);
                db.ExecuteNonQuery(cmd);

                sql = @"
                    --估驗請款
                    delete AskPaymentPayItem where AskPaymentHeaderSeq in (select Seq from AskPaymentHeader where EngMainSeq=@EngMainSeq);
                    delete AskPaymentHeader  where EngMainSeq=@EngMainSeq;

                    --日誌
                    --delete SupDailyReportExtension where EngMainSeq=@EngMainSeq;
                    --delete SupDailyReportWork where EngMainSeq=@EngMainSeq;

                    delete SupDailyReportConstructionMaterial where SupDailyDateSeq in (select Seq from SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete SupPlanOverview where SupDailyDateSeq in (select Seq from SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete SupDailyReportMisc where SupDailyDateSeq in (select Seq from SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete SupDailyReportMiscConstruction where SupDailyDateSeq in (select Seq from SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete SupDailyReportConstructionPerson where SupDailyDateSeq in (select Seq from SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete SupDailyReportConstructionEquipment where SupDailyDateSeq in (select Seq from SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete SupDailyDate where EngMainSeq=@EngMainSeq;                    
                    
                    --物價調整款
                    delete EngPriceAdjWorkItem where EngPriceAdjSeq in(
                        select Seq from EngPriceAdj where EngMainSeq=@EngMainSeq
                    );
                    delete EngPriceAdjLockWorkItem  where EngMainSeq=@EngMainSeq;
                    delete EngPriceAdj where EngMainSeq=@EngMainSeq;

                    --預定進度
                    
                    delete SchProgressHeaderHistoryProgress where SchProgressHeaderHistorySeq in(
                        select Seq from SchProgressHeaderHistory where SchProgressHeaderSeq in (select Seq from SchProgressHeader where EngMainSeq=@EngMainSeq)
                    );

                    delete SchEngProgressWorkItemDel where SchEngProgressPayItemSeq in ( --s20230802
                        select seq from SchEngProgressPayItemDel where SchEngProgressHeaderSeq in (
                            select seq from SchEngProgressHeader where EngMainSeq=@EngMainSeq
                        )
                    );
                    delete SchEngProgressPayItemDel where SchEngProgressHeaderSeq in ( --s20230802
                        select seq from SchEngProgressHeader where EngMainSeq=@EngMainSeq
                    );

                    delete SchProgressHeaderHistory where SchProgressHeaderSeq in (select Seq from SchProgressHeader where EngMainSeq=@EngMainSeq);
                    delete SchProgressPayItem where SchProgressHeaderSeq in (select Seq from SchProgressHeader where EngMainSeq=@EngMainSeq);
                    delete SchProgressHeader where EngMainSeq=@EngMainSeq;

                    --shioulo 20231006
                    Update EngMain set
                    StartDate = null,
                    SchCompDate = null,
                    EngChangeSchCompDate = null
                    where Seq = @EngMainSeq;

                    --shioulo 20230530
                    Update SchEngProgressHeader set
                    SPState = 0
                    where EngMainSeq = @EngMainSeq;
                ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", seq);
                cmd.Parameters.AddWithValue("@EngNo", engNo);
                db.ExecuteNonQuery(cmd);

                //--前置作業 s20230818
                sql = @"
                    delete SchEngProgressSubPayItem where SchEngProgressSubSeq in (
	                    select seq from SchEngProgressSub where SchEngProgressHeaderSeq in (
    	                    select seq from SchEngProgressHeader where EngMainSeq=@EngMainSeq
                        )
                    );

                    delete SchEngProgressSub where SchEngProgressHeaderSeq in (
	                    select seq from SchEngProgressHeader where EngMainSeq=@EngMainSeq
                    );

                    delete SchEngProgressWorkItem where SchEngProgressPayItemSeq in (
                        select seq from SchEngProgressPayItem where SchEngProgressHeaderSeq in (
    	                    select seq from SchEngProgressHeader where EngMainSeq=@EngMainSeq
                        )
                    )

                    delete SchEngProgressPayItem where SchEngProgressHeaderSeq in (
	                    select seq from SchEngProgressHeader where EngMainSeq=@EngMainSeq
                    );

                    delete SchEngProgressHeader where EngMainSeq=@EngMainSeq;";
                cmd = db.GetCommand(sql);
                cmd.CommandTimeout = commandTimeout;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("EngMainService.delEng: " + e.Message);
                log.Info(sql);
                return false;
            }
        }
        //刪除工程所有資料
        public bool DelEng(int seq, string engNo)
        {
            string sql = "";
            int commandTimeout = 600;
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                sql = @"
                    -- 其它需刪除請自行加入
                    delete UploadAuditFileResult where EngMaterialDeviceTestSummarySeq in (
                        select Seq from EngMaterialDeviceTestSummary where EngMaterialDeviceListSeq in(
                            select Seq from EngMaterialDeviceList where EngMainSeq=@EngMainSeq
                        )
                    ); --s20230626
                    delete EngMaterialDeviceTestSummary where EngMaterialDeviceListSeq in(
	                    select Seq from EngMaterialDeviceList where EngMainSeq=@EngMainSeq
                    );
                    delete EngMaterialDeviceSummary where EngMaterialDeviceListSeq in(
	                    select Seq from EngMaterialDeviceList where EngMainSeq=@EngMainSeq
                    );
                    -- End

                    delete ConstCheckRecImproveFile where ConstCheckRecImproveSeq in
                    (
	                    select Seq from ConstCheckRecImprove where ConstCheckRecSeq in
                        (
                            select Seq from ConstCheckRec where EngConstructionSeq in
                            (
                                select Seq from EngConstruction where EngMainSeq=@EngMainSeq
                            )
                        )
                    );      
                    delete ConstCheckRecImprove where ConstCheckRecSeq in
                    (
                        select Seq from ConstCheckRec where EngConstructionSeq in
                        (
                            select Seq from EngConstruction where EngMainSeq=@EngMainSeq
                        )
                    ); 

                    delete ConstCheckRecResult where ConstCheckRecSeq in
                    (
                        select Seq from ConstCheckRec where EngConstructionSeq in
                        (
                            select Seq from EngConstruction where EngMainSeq=@EngMainSeq
                        )
                    );

                    delete ConstCheckRecFile where ConstCheckRecSeq in
                    (
                        select Seq from ConstCheckRec where EngConstructionSeq in
                        (
                            select Seq from EngConstruction where EngMainSeq=@EngMainSeq
                        )
                    );

                    delete NCR where ConstCheckRecSeq in
                    (
                        select Seq from ConstCheckRec where EngConstructionSeq in
                        (
                            select Seq from EngConstruction where EngMainSeq=@EngMainSeq
                        )
                    );

                    delete ConstCheckRec where EngConstructionSeq in
                    (
                         select Seq from EngConstruction where EngMainSeq=@EngMainSeq
                    );

                    DELETE PayItem where EngMainSeq=@EngMainSeq;
                    DELETE SupervisionProjectList where EngMainSeq=@EngMainSeq;
                    DELETE ChartMaintainList where EngMainSeq=@EngMainSeq;

                    delete OccuSafeHealthControlSt where OccuSafeHealthListSeq in(
	                    select Seq from OccuSafeHealthList where EngMainSeq=@EngMainSeq
                    );
                    delete EngMaterialDeviceSummary where EngMaterialDeviceListSeq in(
	                    select Seq from EngMaterialDeviceList where EngMainSeq=@EngMainSeq
                    );

                    delete OccuSafeHealthList where EngMainSeq=@EngMainSeq;

                    delete EnvirConsControlSt where EnvirConsListSeq in(
	                    select Seq from EnvirConsList where EngMainSeq=@EngMainSeq
                    );
                    delete EnvirConsList where EngMainSeq=@EngMainSeq;

                    delete ConstCheckControlSt where ConstCheckListSeq in(
	                    select Seq from ConstCheckList where EngMainSeq=@EngMainSeq
                    );
                    delete ConstCheckList where EngMainSeq=@EngMainSeq;

                    delete EquOperControlSt where EquOperTestStSeq in(
	                    select Seq from EquOperTestList where EngMainSeq=@EngMainSeq
                    );
                    delete EquOperTestList where EngMainSeq=@EngMainSeq;

                    delete EngMaterialDeviceControlSt where EngMaterialDeviceListSeq in(
	                    select Seq from EngMaterialDeviceList where EngMainSeq=@EngMainSeq
                    );
                    delete EngMaterialDeviceList where EngMainSeq=@EngMainSeq;

                    delete EngAttachment where EngMainSeq=@EngMainSeq;

                    delete EngConstruction where EngMainSeq=@EngMainSeq;

                    delete QualityProjectList where EngMainSeq=@EngMainSeq;
                 ";
                cmd = db.GetCommand(sql);
                cmd.CommandTimeout = commandTimeout;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", seq);
                cmd.Parameters.AddWithValue("@EngNo", engNo);
                db.ExecuteNonQuery(cmd);

                sql = @"
-- 20220728 start
                    delete ConstRiskEval where EngMainSeq=@EngMainSeq;

                    delete SupDailyReportConstructionMaterial where SupDailyDateSeq in (select Seq from SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete SupPlanOverview where SupDailyDateSeq in (select Seq from SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete SupDailyReportMisc where SupDailyDateSeq in (select Seq from SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete SupDailyReportMiscConstruction where SupDailyDateSeq in (select Seq from SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete SupDailyReportConstructionPerson where SupDailyDateSeq in (select Seq from SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete SupDailyReportConstructionEquipment where SupDailyDateSeq in (select Seq from SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete SupDailyDate where EngMainSeq=@EngMainSeq;
                    
                    delete EcologicalChecklist where EngMainSeq=@EngMainSeq;
                    
         -- shioulo 20221005 adj
                    delete Officer where SuperviseEngSeq in (
                        select Seq from SuperviseEng where PrjXMLSeq in( select PrjXMLSeq from EngMain where Seq=@EngMainSeq )
                    );

                    delete SuperviseFillInsideCommittee where SuperviseFillSeq in(
                        select Seq from SuperviseFill where SuperviseEngSeq in (
                            select Seq from SuperviseEng where PrjXMLSeq in( select PrjXMLSeq from EngMain where Seq=@EngMainSeq )
                        )
                    );

                    delete SuperviseFillOutCommittee where SuperviseFillSeq in(
                        select Seq from SuperviseFill where SuperviseEngSeq in (
                            select Seq from SuperviseEng where PrjXMLSeq in( select PrjXMLSeq from EngMain where Seq=@EngMainSeq )
                        )
                    );

                    delete SuperviseFill where SuperviseEngSeq in (
                            select Seq from SuperviseEng where PrjXMLSeq in( select PrjXMLSeq from EngMain where Seq=@EngMainSeq )
                        );
                    delete InsideCommittee where SuperviseEngSeq in (
                            select Seq from SuperviseEng where PrjXMLSeq in( select PrjXMLSeq from EngMain where Seq=@EngMainSeq )
                        );
                    delete OutCommittee where SuperviseEngSeq in (
                            select Seq from SuperviseEng where PrjXMLSeq in( select PrjXMLSeq from EngMain where Seq=@EngMainSeq )
                        );
                    delete SuperviseEngTHSR where SuperviseEngSeq in (
                            select Seq from SuperviseEng where PrjXMLSeq in( select PrjXMLSeq from EngMain where Seq=@EngMainSeq )
                        );

                    delete SuperviseEng where PrjXMLSeq in( select PrjXMLSeq from EngMain where Seq=@EngMainSeq );
                    
                    --估驗請款 s20221027
                    delete AskPaymentPayItem where AskPaymentHeaderSeq in (select Seq from AskPaymentHeader where EngMainSeq=@EngMainSeq);
                    delete AskPaymentHeader  where EngMainSeq=@EngMainSeq;

                    --物價調整款 s20230331
                    delete EngPriceAdjWorkItem where EngPriceAdjSeq in(
                        select Seq from EngPriceAdj where EngMainSeq=@EngMainSeq
                    );
                    delete EngPriceAdjLockWorkItem  where EngMainSeq=@EngMainSeq;
                    delete EngPriceAdj where EngMainSeq=@EngMainSeq;

                    delete CarbonEmissionWorkItem where CarbonEmissionPayItemSeq in (
                        select Seq from CarbonEmissionPayItem where CarbonEmissionHeaderSeq in (select Seq from CarbonEmissionHeader where EngMainSeq=@EngMainSeq)
                    );
        -- shioulo 20221005 end
                    --s20221025
                    delete SchProgressHeaderHistoryProgress where SchProgressHeaderHistorySeq in(
                        select Seq from SchProgressHeaderHistory where SchProgressHeaderSeq in (select Seq from SchProgressHeader where EngMainSeq=@EngMainSeq)
                    );

                    delete SchEngProgressWorkItemDel where SchEngProgressPayItemSeq in ( --s20230802
                        select seq from SchEngProgressPayItemDel where SchEngProgressHeaderSeq in (
                            select seq from SchEngProgressHeader where EngMainSeq=@EngMainSeq
                        )
                    );
                    delete SchEngProgressPayItemDel where SchEngProgressHeaderSeq in ( --s20230802
                        select seq from SchEngProgressHeader where EngMainSeq=@EngMainSeq
                    );

                    delete SchProgressHeaderHistory where SchProgressHeaderSeq in (select Seq from SchProgressHeader where EngMainSeq=@EngMainSeq);
                    delete SchProgressPayItem where SchProgressHeaderSeq in (select Seq from SchProgressHeader where EngMainSeq=@EngMainSeq);
                    delete SchProgressHeader where EngMainSeq=@EngMainSeq;

                    delete CarbonEmissionPayItem where CarbonEmissionHeaderSeq in (select Seq from CarbonEmissionHeader where EngMainSeq=@EngMainSeq);
                    delete CECheckTable where CarbonEmissionHeaderSeq in (select Seq from CarbonEmissionHeader where EngMainSeq=@EngMainSeq); --s20230315
                    delete CarbonEmissionHeader where EngMainSeq=@EngMainSeq;


                    delete SupDailyReportExtension where EngMainSeq=@EngMainSeq;
                    delete SupDailyReportNoDuration where EngMainSeq=@EngMainSeq; --s20230927
                    delete SupDailyReportWork where EngMainSeq=@EngMainSeq;
                    ";
                cmd = db.GetCommand(sql);
                cmd.CommandTimeout = commandTimeout;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", seq);
                cmd.Parameters.AddWithValue("@EngNo", engNo);
                db.ExecuteNonQuery(cmd);

                //工程變更 20230424
                sql = @"
                    --工程變更 估驗請款
                    delete EC_AskPaymentPayItem where EC_AskPaymentHeaderSeq in (select Seq from EC_AskPaymentHeader where EngMainSeq=@EngMainSeq);
                    delete EC_AskPaymentHeader where EngMainSeq=@EngMainSeq;

                    --工程變更 物價調整款
                    delete EC_EngPriceAdjWorkItem where EC_EngPriceAdjSeq in(
                        select Seq from EC_EngPriceAdj where EngMainSeq=@EngMainSeq
                    );
                    delete EC_EngPriceAdjLockWorkItem  where EngMainSeq=@EngMainSeq;
                    delete EC_EngPriceAdj where EngMainSeq=@EngMainSeq;
 
                    --工程變更 日誌
                    delete EC_SchProgressPayItem
                    where EC_SchEngProgressPayItemSeq in (
                        select Seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in(
                            select Seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                        )
                    )
                    delete EC_SupDailyReportMiscConstruction where EC_SupDailyDateSeq in (select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete EC_SupDailyReportMisc where EC_SupDailyDateSeq in (select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete EC_SupDailyReportConstructionPerson where EC_SupDailyDateSeq in (select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete EC_SupDailyReportConstructionMaterial where EC_SupDailyDateSeq in (select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete EC_SupDailyReportConstructionEquipment where EC_SupDailyDateSeq in (select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete EC_SupPlanOverview where EC_SupDailyDateSeq in (select Seq from EC_SupDailyDate where EngMainSeq=@EngMainSeq);
                    delete EC_SupDailyDate where EngMainSeq=@EngMainSeq;

                    delete EC_SchEngProgressWorkItem where EC_SchEngProgressPayItemSeq in (
                        select seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in (
                            select seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                        )
                    );
                    delete EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in (
                        select seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                    );
                    delete EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq;
                    ";
                cmd = db.GetCommand(sql);
                cmd.CommandTimeout = commandTimeout;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", seq);
                db.ExecuteNonQuery(cmd);

                //s20230413
                sql = @"
                    --前置作業 
                    delete SchEngProgressSubPayItem where SchEngProgressSubSeq in (
	                    select seq from SchEngProgressSub where SchEngProgressHeaderSeq in (
    	                    select seq from SchEngProgressHeader where EngMainSeq=@EngMainSeq
                        )
                    );

                    delete SchEngProgressSub where SchEngProgressHeaderSeq in (
	                    select seq from SchEngProgressHeader where EngMainSeq=@EngMainSeq
                    );

                    delete SchEngProgressWorkItem where SchEngProgressPayItemSeq in (
                        select seq from SchEngProgressPayItem where SchEngProgressHeaderSeq in (
    	                    select seq from SchEngProgressHeader where EngMainSeq=@EngMainSeq
                        )
                    )

                    delete SchEngProgressPayItem where SchEngProgressHeaderSeq in (
	                    select seq from SchEngProgressHeader where EngMainSeq=@EngMainSeq
                    );

                    delete SchEngProgressHeader where EngMainSeq=@EngMainSeq;";
                cmd = db.GetCommand(sql);
                cmd.CommandTimeout = commandTimeout;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", seq);
                db.ExecuteNonQuery(cmd);//s20230413

                sql = @"                    
                    delete EngSupervisor where EngMainSeq=@EngMainSeq; -- shioulo 20221011

                    delete EngMain where Seq=@EngMainSeq;

                    -- shioulo 20221005
                    delete PCCESWorkItem where PCCESPayItemSeq in (
                        select Seq from PCCESPayItem where PCCESSMainSeq in ( select Seq from PCCESSMain where contractNo=@EngNo )
                    );

                    delete PCCESPayItem where PCCESSMainSeq in
                    (
                         select Seq from PCCESSMain where contractNo=@EngNo
                    );

                    delete PCCESSMain where contractNo=@EngNo;

                    ";
                cmd = db.GetCommand(sql);
                cmd.CommandTimeout = commandTimeout;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", seq);
                cmd.Parameters.AddWithValue("@EngNo", engNo);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("EngMainService.delEng: " + e.Message);
                log.Info(sql);
                return false;
            }
        }

        //產製監造計畫書範例
        public bool CreateSupervisionProject(int seq, string importMode)
        {
            List<FlowChartFileModel> copyFileList = new List<FlowChartFileModel>();

            //監造計畫書範本 檔案
            string sql = @"select Seq, OriginFileName,UniqueFileName from SupervisionProjectTp";
            SqlCommand cmd = db.GetCommand(sql);
            List<SupervisionProjectTpModel> supervisionProjectTpModel = db.GetDataTableWithClass<SupervisionProjectTpModel>(cmd);
            foreach (SupervisionProjectTpModel m in supervisionProjectTpModel)
            {
                if (!String.IsNullOrEmpty(m.UniqueFileName))
                {
                    copyFileList.Add(new FlowChartFileModel()
                    {
                        Seq = m.Seq,
                        FlowCharOriginFileName = m.UniqueFileName,
                        FlowCharUniqueFileName = String.Format("{0}-{1}", copyFileList.Count + 1, m.UniqueFileName)
                    });
                }
            }
            //品質計畫書範本
            sql = @"select Seq, OriginFileName,UniqueFileName from QualityProjectTp ";
            cmd = db.GetCommand(sql);
            List<QualityProjectTpModel> qualityProjectTpModel = db.GetDataTableWithClass<QualityProjectTpModel>(cmd);
            foreach (QualityProjectTpModel m in qualityProjectTpModel)
            {
                if (!String.IsNullOrEmpty(m.UniqueFileName))
                {
                    copyFileList.Add(new FlowChartFileModel()
                    {
                        Seq = m.Seq,
                        FlowCharOriginFileName = m.UniqueFileName,
                        FlowCharUniqueFileName = String.Format("{0}-{1}", copyFileList.Count + 1, m.UniqueFileName)
                    });
                }
            }
            //圖表清冊 檔案
            sql = @"select Seq,OriginFileName,UniqueFileName from ChartMaintainTp;";
            cmd = db.GetCommand(sql);
            List<CMEditModel> chartMaintainTp = db.GetDataTableWithClass<CMEditModel>(cmd);
            foreach (CMEditModel m in chartMaintainTp)
            {
                if (!String.IsNullOrEmpty(m.UniqueFileName))
                {
                    copyFileList.Add(new FlowChartFileModel()
                    {
                        Seq = m.Seq,
                        FlowCharOriginFileName = m.UniqueFileName,
                        FlowCharUniqueFileName = String.Format("{0}-{1}", copyFileList.Count + 1, m.UniqueFileName)
                    });
                }
            }

            List<EngMainEditVModel> engs = GetItemBySeq<EngMainEditVModel>(seq);
            bool isNeedElecDevice = engs[0].IsNeedElecDevice;

            //5 材料設備清冊範本 EngMaterialDeviceListTp
            List<MasterEngMaterialDeviceListTpModel> mList5 = new EngMaterialDeviceListTpService().GetList<MasterEngMaterialDeviceListTpModel>();
            foreach (MasterEngMaterialDeviceListTpModel item in mList5)
            {
                item.GetDetail();
            }
            //材料設備送審管制總表
            List<MasterPCCESPayItemModel> payItemList = MasterPCCESPayItemModel.GetList<MasterPCCESPayItemModel>(db, seq);

            //6 設備運轉測試清單範本 EquOperTestListTp
            List<MasterEquOperTestListTpModel> mList6 = new EquOperTestListTpService().GetList<MasterEquOperTestListTpModel>();
            foreach (MasterEquOperTestListTpModel item in mList6)
            {
                item.GetDetail();
            }
            //701 施工抽查清單範本 ConstCheckListTp
            List<MasterConstCheckListTpModel> mList701 = new ConstCheckListTpService().GetList<MasterConstCheckListTpModel>();
            foreach (MasterConstCheckListTpModel item in mList701)
            {
                item.GetDetail();
            }
            //702 環境保育清單範本 EnvirConsListTp
            List<MasterEnvirConsListTpModel> mList702 = new EnvirConsListTpService().GetList<MasterEnvirConsListTpModel>();
            foreach (MasterEnvirConsListTpModel item in mList702)
            {
                item.GetDetail();
            }
            //703 職業安全衛生清單範本 OccuSafeHealthListTp
            List<MasterOccuSafeHealthListTpModel> mList703 = new OccuSafeHealthListTpService().GetList<MasterOccuSafeHealthListTpModel>();
            foreach (MasterOccuSafeHealthListTpModel item in mList703)
            {
                item.GetDetail();
            }

            //碳排量計算是否有資料 shioulo 20220628
            sql = @"select count(Seq) total from CarbonEmissionPayItem where CarbonEmissionHeaderSeq=(select seq from CarbonEmissionHeader where EngMainSeq=@EngMainSeq)";
            cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", seq);
            DataTable dt = db.GetDataTable(cmd);
            bool hasCarbonEmissionPayItem = Convert.ToInt32(dt.Rows[0]["total"].ToString()) > 0;

            db.BeginTransaction();
            try
            {
                //5 材料設備清冊範本 EngMaterialDeviceListTp, 材料設備送審管制總表 EngMaterialDeviceSummary
                sql = @"
                delete EngMaterialDeviceSummary where EngMaterialDeviceListSeq in(
                    select Seq from EngMaterialDeviceList where EngMainSeq=@EngMainSeq
                );

                delete EngMaterialDeviceControlSt where EngMaterialDeviceListSeq in(
                    select Seq from EngMaterialDeviceList where EngMainSeq=@EngMainSeq
                );
                delete EngMaterialDeviceList where EngMainSeq=@EngMainSeq;";

                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", seq);
                db.ExecuteNonQuery(cmd);
                /*foreach (MasterEngMaterialDeviceListTpModel item in mList5)
                {
                    Null2Empty(item);
                    item.clone(db, seq);
                }*/

                //材料設備送審管制總表
                int orderNo = 0;
                foreach (MasterPCCESPayItemModel item in payItemList)
                {
                    //僅限[壹、一], [壹、二]兩大項內的所有細項
                    //importMode=="1" 全部 s20230830 /*item.RefItemCode.Trim().Length>0 &&*/
                    if (item.RefItemCode != null && (importMode=="1" || item.PayItem.IndexOf("壹,一,") == 0 || item.PayItem.IndexOf("壹,二,") == 0))
                    {
                        //orderNo++;
                        item.SaveEngMaterialDeviceSummary(db, seq, mList5, item, ref orderNo, copyFileList);
                    }
                }

                //6 設備運轉測試清單範本 EquOperTestListTp
                sql = @"
                delete EquOperControlSt where EquOperTestStSeq in(
                    select Seq from EquOperTestList where EngMainSeq=@EngMainSeq
                );
                delete EquOperTestList where EngMainSeq=@EngMainSeq;";

                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", seq);
                db.ExecuteNonQuery(cmd);
                if (isNeedElecDevice)
                {
                    foreach (MasterEquOperTestListTpModel item in mList6)
                    {
                        Null2Empty(item);
                        item.clone(db, seq, copyFileList);
                    }
                }

                // 701 施工抽查清單範本 ConstCheckListTp
                sql = @"
                delete ConstCheckControlSt where ConstCheckListSeq in(
                    select Seq from ConstCheckList where EngMainSeq=@EngMainSeq
                );
                delete ConstCheckList where EngMainSeq=@EngMainSeq;";

                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", seq);
                db.ExecuteNonQuery(cmd);
                foreach (MasterConstCheckListTpModel item in mList701)
                {
                    Null2Empty(item);
                    item.clone(db, seq, payItemList, copyFileList);
                }

                //702 環境保育清單範本 EnvirConsListTp
                sql = @"
                delete EnvirConsControlSt where EnvirConsListSeq in(
                    select Seq from EnvirConsList where EngMainSeq=@EngMainSeq
                );
                delete EnvirConsList where EngMainSeq=@EngMainSeq;";

                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", seq);
                db.ExecuteNonQuery(cmd);
                foreach (MasterEnvirConsListTpModel item in mList702)
                {
                    Null2Empty(item);
                    item.clone(db, seq, copyFileList);
                }

                //703 職業安全衛生清單範本 OccuSafeHealthListTp
                sql = @"
                delete OccuSafeHealthControlSt where OccuSafeHealthListSeq in(
                    select Seq from OccuSafeHealthList where EngMainSeq=@EngMainSeq
                );
                delete OccuSafeHealthList where EngMainSeq=@EngMainSeq;";

                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", seq);
                db.ExecuteNonQuery(cmd);
                foreach (MasterOccuSafeHealthListTpModel item in mList703)
                {
                    Null2Empty(item);
                    item.clone(db, seq, copyFileList);
                }

                //圖表清冊
                sql = @"
                    DELETE ChartMaintainList where EngMainSeq=@EngMainSeq
                    ;

                    insert into ChartMaintainList (
                        EngMainSeq,CreateTime,CreateUserSeq,ModifyTime,ModifyUserSeq
                        ,ChapterSeq,OrderNo,ExcelNo,ChartKind,ChartName,OriginFileName,UniqueFileName
                    )
                    select
                        @EngMainSeq,GETDATE(),@ModifyUserSeq,GETDATE(),@ModifyUserSeq
                        ,ChapterSeq,OrderNo,ExcelNo,ChartKind,ChartName,OriginFileName,UniqueFileName
                    from ChartMaintainTp
                    ;
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                //監造計畫書範本
                sql = @"
                    DELETE SupervisionProjectList where EngMainSeq=@EngMainSeq
                    ;

                    insert into SupervisionProjectList (
                        EngMainSeq,CreateTime,CreateUserSeq,ModifyTime,ModifyUserSeq,RevisionNo,DocState
                        ,Name,RevisionDate,OriginFileName,UniqueFileName
                    )
                    select
                        @EngMainSeq,GETDATE(),@ModifyUserSeq,GETDATE(),@ModifyUserSeq,1,-1
                        ,Name,RevisionDate,OriginFileName,UniqueFileName
                    from SupervisionProjectTp
                    ;
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                //材料項目清單(PCCESS帶入)
                if (hasCarbonEmissionPayItem)
                {//shioulo 20220628
                    sql = @"
                    DELETE PayItem where EngMainSeq=@EngMainSeq
                    ;

                    insert into PayItem (
                        EngMainSeq
                        ,PayItem,[Description],Unit,Quantity,Price,Amount,ItemKey,ItemNo,RefItemCode
                    )
                    select
                        b.EngMainSeq
                        ,trim(a.PayItem),a.[Description],a.Unit,a.Quantity,a.Price,a.Amount,a.ItemKey,a.ItemNo,a.RefItemCode
                    from CarbonEmissionPayItem a
                    inner join CarbonEmissionHeader b on(b.EngMainSeq=@EngMainSeq and b.Seq=a.CarbonEmissionHeaderSeq)
                    ;
                    ";
                }
                else
                {
                    sql = @"
                    DELETE PayItem where EngMainSeq=@EngMainSeq
                    ;

                    insert into PayItem (
                        EngMainSeq
                        ,PayItem,[Description],Unit,Quantity,Price,Amount,ItemKey,ItemNo,RefItemCode
                    )
                    select
                        @EngMainSeq
                        ,a.PayItem,a.[Description],a.Unit,a.Quantity,a.Price,a.Amount,a.ItemKey,a.ItemNo,a.RefItemCode
                    from PCCESPayItem a
                    inner join PCCESSMain b on(
	                    b.Seq=a.PCCESSMainSeq
                        and b.contractNo=(select c.EngNo from EngMain c where c.Seq=@EngMainSeq)
                    )
                    ;
                    ";
                }
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", seq);
                db.ExecuteNonQuery(cmd);

                //品質計畫書範本
                sql = @"
                    DELETE QualityProjectList where EngMainSeq=@EngMainSeq
                    ;

                    insert into QualityProjectList (
                        EngMainSeq,CreateTime,CreateUserSeq,ModifyTime,ModifyUserSeq,RevisionNo,DocState
                        ,Name,RevisionDate,OriginFileName,UniqueFileName
                    )
                    select
                        @EngMainSeq,GETDATE(),@ModifyUserSeq,GETDATE(),@ModifyUserSeq,1,-1
                        ,Name,RevisionDate,OriginFileName,UniqueFileName
                    from QualityProjectTp
                    ;
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                //檔案複製
                string templateFolder = Utils.GetTemplateFolder();
                string engMainFolder = Utils.GetEngMainFolder(seq);
                if (!Directory.Exists(engMainFolder))
                {
                    Directory.CreateDirectory(engMainFolder);
                }
                if (Directory.Exists(templateFolder))
                {
                    foreach (FlowChartFileModel file in copyFileList)
                    {
                        string srcName = Path.Combine(templateFolder, file.FlowCharOriginFileName);
                        string destFile = Path.Combine(engMainFolder, file.FlowCharOriginFileName);
                        if (File.Exists(srcName))
                            File.Copy(srcName, destFile, true);
                    }
                    /*string[] files = Directory.GetFiles(templateFolder);

                    foreach (string s in files)
                    {
                        string fileName = Path.GetFileName(s);
                        string destFile = Path.Combine(engMainFolder, fileName);
                        File.Copy(s, destFile, true);
                    }*/
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("EngMainService.CreateSupervisionProject: " + e.Message);
                log.Info(sql);
                return false;
            }
        }

        public List<object> GetEngListByUser(string userNo)
        {
            string getRole = @" Select distinct ur.RoleSeq from UserRole ur
                inner join UserUnitPosition uu on uu.Seq = ur.UserUnitPositionSeq
                inner join UserMain u on u.Seq = uu.UserMainSeq
                where u.userNo = @userNo                
            ";
            SqlCommand cmd = db.GetCommand(getRole);
            cmd.Parameters.AddWithValue("@userNo", userNo);
            int role = Convert.ToInt32(db.ExecuteScalar(cmd));

            string sql = @"Select EngMain.Seq as Seq, EngNo,  EngName, 

                cast(  (select Name from Unit where Unit.Seq = EngMain.ExecUnitSeq ) as varchar(20)) as ExecUnitName  
                from EngMain  where PrjXMLSeq is not null " + Utils.getAuthoritySql("", userNo) + @"
                and EngYear >= @Year
        
            ";

            cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@userNo", userNo);
            cmd.Parameters.AddWithValue("@Year", DateTime.Now.Year - 1912);

            return db.GetDataTable(cmd).Rows.Cast<DataRow>().Select(row => new
            {
                Seq = row.Field<object>("Seq"),
                EngNo = row.Field<string>("EngNo"),
                EngName = row.Field<string>("EngName"),
                ExecUnitName = row.Field<string>("ExecUnitName")
            }).ToList<object>();

        }

        //自辦監造人員 shioulo 20221007
        //新增監造人員
        public int SupervisorUserAdd(int engMainSeq, int userKind, int subUnitSeq, int userMainSeq)
        {
            try
            {
                string sql = @"
				    INSERT INTO EngSupervisor (
                        EngMainSeq, UserKind, SubUnitSeq, UserMainSeq
                    )values(
                        @EngMainSeq, @UserKind, @SubUnitSeq, @UserMainSeq
                    )";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                cmd.Parameters.AddWithValue("@UserKind", userKind);
                cmd.Parameters.AddWithValue("@SubUnitSeq", subUnitSeq);
                cmd.Parameters.AddWithValue("@UserMainSeq", userMainSeq);
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                //db.TransactionRollback();
                log.Info("EngMainService.SupervisorUserAdd" + e.Message);
                return -1;
            }
        }
        //刪除監造人員
        public int SupervisorUserDel(int Seq)
        {
            try
            {
                string sql = @"delete EngSupervisor where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", Seq);
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                //db.TransactionRollback();
                log.Info("EngMainService.SupervisorUserDel" + e.Message);
                return -1;
            }
        }
        //自辦監造人員清單
        public List<T> SupervisorUserList<T>(int engMainSeq)
        {
            string sql = @"select
	            a.Seq,
                a.UserKind,
                c.Name SubUnitName,
                b.DisplayName UserName
            from EngSupervisor a
            inner join UserMain b on(b.Seq=a.UserMainSeq)
            inner join Unit c on(c.Seq=a.SubUnitSeq)
            where a.EngMainSeq=@EngMainSeq
            order by a.UserKind, b.DisplayName";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //s20230624
        public List<T> SupervisorUserAllList<T>(int engMainSeq)
        {
            string sql = @"select
	            a.Seq,
                a.EngMainSeq,
                a.UserKind,
                a.SubUnitSeq,
                a.UserMainSeq
            from EngSupervisor a
            where a.EngMainSeq=@EngMainSeq
            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //複製工程 s20230624
        public bool CopyEng(int srcEngMainSeq, string newEngNo, EngMainCopyModel eng)
        {
            int userSeq = getUserSeq();
            SqlCommand cmd;
            DataTable dt;
            string sql;
            db.BeginTransaction();
            try
            {
                //工程主檔
                sql = @"
                insert into EngMain (
                    EngYear,
                    EngNo,
                    EngName,
                    OrganizerUnitCode,
                    OrganizerUnitSeq,
                    ExecUnitSeq,
                    DesignUnitName,
                    DesignManName,
                    TotalBudget,
                    SubContractingBudget,
                    --ContractAmountAfterDesignChange,
                    PurchaseAmount,
                    ExecType,
                    ProjectScope,
                    EngTownSeq,
                    EngPeriod,
                    StartDate,
                    SchCompDate,
                    PostCompDate,
                    ApproveDate,
                    ApproveNo,
                    AwardAmount,
                    BuildContractorName,
                    BuildContractorContact,
                    BuildContractorTaxId,
                    BuildContractorEmail,
                    IsNeedElecDevice ,
                    SupervisorUnitName,
                    SupervisorDirector,
                    SupervisorTechnician,
                    SupervisorTaxid,
                    SupervisorContact,
                    SupervisorSelfPerson1,
                    SupervisorSelfPerson2,
                    SupervisorCommPerson1,
                    SupervisorCommPersion2,
                    OrganizerSubUnitSeq,
                    ExecSubUnitSeq,
                    OrganizerUserSeq,
                    ConstructionDirector,
                    ConstructionPerson1,
                    ConstructionPerson2,
                    --PrjXMLSeq,
                    AwardDate,
                    DesignUnitEmail,
                    DesignUnitTaxId,
                    SupervisorExecType,
                    --PccesXMLFile,
                    --PccesXMLDate,
                    SupervisorCommPerson3,
                    SupervisorCommPerson3LicenseExpires,
                    SupervisorCommPerson4LicenseExpires,
                    EngChangeStartDate,
                    WarrantyExpires,
                    SupervisorCommPerson4,
                    --EngChangeSchCompDate,
                    CarbonDemandQuantity,
                    --ApprovedCarbonQuantity,
                    --OfficialApprovedCarbonQuantity,
                    CarbonDesignQuantity,
                    --CarbonTradedQuantity,
                    --CarbonConstructionQuantity,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                )
                select
                    EngYear,
                    @EngNo,
                    EngName,
                    OrganizerUnitCode,
                    OrganizerUnitSeq,
                    ExecUnitSeq,
                    DesignUnitName,
                    DesignManName,
                    TotalBudget,
                    SubContractingBudget,
                    --ContractAmountAfterDesignChange,
                    PurchaseAmount,
                    ExecType,
                    ProjectScope,
                    EngTownSeq,
                    EngPeriod,
                    StartDate,
                    SchCompDate,
                    PostCompDate,
                    ApproveDate,
                    ApproveNo,
                    AwardAmount,
                    BuildContractorName,
                    BuildContractorContact,
                    BuildContractorTaxId,
                    BuildContractorEmail,
                    IsNeedElecDevice ,
                    SupervisorUnitName,
                    SupervisorDirector,
                    SupervisorTechnician,
                    SupervisorTaxid,
                    SupervisorContact,
                    SupervisorSelfPerson1,
                    SupervisorSelfPerson2,
                    SupervisorCommPerson1,
                    SupervisorCommPersion2,
                    OrganizerSubUnitSeq,
                    ExecSubUnitSeq,
                    OrganizerUserSeq,
                    ConstructionDirector,
                    ConstructionPerson1,
                    ConstructionPerson2,
                    --PrjXMLSeq,
                    AwardDate,
                    DesignUnitEmail,
                    DesignUnitTaxId,
                    SupervisorExecType,
                    --PccesXMLFile,
                    --PccesXMLDate,
                    SupervisorCommPerson3,
                    SupervisorCommPerson3LicenseExpires,
                    SupervisorCommPerson4LicenseExpires,
                    EngChangeStartDate,
                    WarrantyExpires,
                    SupervisorCommPerson4,
                    --EngChangeSchCompDate,
                    CarbonDemandQuantity,
                    --ApprovedCarbonQuantity,
                    --OfficialApprovedCarbonQuantity,
                    CarbonDesignQuantity,
                    --CarbonTradedQuantity,
                    --CarbonConstructionQuantity,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                from EngMain
                where Seq=@EngMainSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", srcEngMainSeq);
                cmd.Parameters.AddWithValue("@EngNo", newEngNo);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq); ;
                db.ExecuteNonQuery(cmd);

                sql = @"SELECT IDENT_CURRENT('EngMain') AS NewSeq";
                cmd = db.GetCommand(sql);
                dt = db.GetDataTable(cmd);
                int newEngMainSeq = Convert.ToInt16(dt.Rows[0]["NewSeq"].ToString());

                string srcEngFilePath = Utils.GetEngMainFolder(srcEngMainSeq);
                string newEngFilePath = Utils.GetEngMainFolder(newEngMainSeq);
                if (!Directory.Exists(newEngFilePath))
                {
                    Directory.CreateDirectory(newEngFilePath);
                }

                //監造計畫書
                sql = @"
                    insert into SupervisionProjectList (
                        EngMainSeq,
                        DocState,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EngMainSeq,
                        -1,
                        GETDATE(),
                        @ModifyUserSeq,
                        GETDATE(),
                        @ModifyUserSeq
                    )
                    ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", newEngMainSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                db.ExecuteNonQuery(cmd);
                
                //工程主要施工項目及數量 清單
                sql = @"
                    insert into EngConstruction (
                        EngMainSeq,
                        ItemNo,
                        ItemName,
                        ItemQty,
                        ItemUnit,
                        OrderNo,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EngMainSeq,
                        @ItemNo,
                        @ItemName,
                        @ItemQty,
                        @ItemUnit,
                        @OrderNo,
                        GETDATE(),
                        @ModifyUserSeq,
                        GETDATE(),
                        @ModifyUserSeq
                    )
                ";
                foreach (EngConstructionModel m in eng.engConstructionItems)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EngMainSeq", newEngMainSeq);
                    cmd.Parameters.AddWithValue("@ItemNo", m.ItemNo);
                    cmd.Parameters.AddWithValue("@ItemName", m.ItemName);
                    cmd.Parameters.AddWithValue("@ItemQty", m.ItemQty);
                    cmd.Parameters.AddWithValue("@ItemUnit", m.ItemUnit);
                    cmd.Parameters.AddWithValue("@OrderNo", m.OrderNo);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                    db.ExecuteNonQuery(cmd);
                }

                //自辦監造人員清單
                sql = @"
				    INSERT INTO EngSupervisor (
                        EngMainSeq, UserKind, SubUnitSeq, UserMainSeq
                    )values(
                        @EngMainSeq, @UserKind, @SubUnitSeq, @UserMainSeq
                    )";
                foreach (EngSupervisorModel m in eng.engSupervisorItems)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EngMainSeq", newEngMainSeq);
                    cmd.Parameters.AddWithValue("@UserKind", m.UserKind);
                    cmd.Parameters.AddWithValue("@SubUnitSeq", m.SubUnitSeq);
                    cmd.Parameters.AddWithValue("@UserMainSeq", m.UserMainSeq);
                    db.ExecuteNonQuery(cmd);
                }

                //監造計畫附件 圖/表
                sql = @"
                insert into EngAttachment (
                    EngMainSeq,
                    OriginFileName,
                    UniqueFileName,
                    Chapter,
                    FileType,
                    Description,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @EngMainSeq,
                    @OriginFileName,
                    @UniqueFileName,
                    @Chapter,
                    @FileType,
                    @Description,
                    GETDATE(),
                    @ModifyUserSeq,
                    GETDATE(),
                    @ModifyUserSeq
                )";
                foreach (EngAttachmentModel m in eng.engAttachmentItems)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EngMainSeq", newEngMainSeq);
                    cmd.Parameters.AddWithValue("@Chapter", m.Chapter);
                    cmd.Parameters.AddWithValue("@FileType", m.FileType);
                    cmd.Parameters.AddWithValue("@Description", m.Description);
                    cmd.Parameters.AddWithValue("@OriginFileName", m.OriginFileName);
                    cmd.Parameters.AddWithValue("@UniqueFileName", m.UniqueFileName);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                    db.ExecuteNonQuery(cmd);
                    copyFile(srcEngFilePath, newEngFilePath, m.UniqueFileName);
                }
                
                //碳排量清單
                sql = @"
                    insert into CarbonEmissionHeader (
                        EngMainSeq,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @EngMainSeq,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", newEngMainSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('CarbonEmissionHeader') AS NewSeq";
                cmd = db.GetCommand(sql1);
                dt = db.GetDataTable(cmd);
                int carbonEmissionHeaderSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                foreach (CarbonEmissionPayItemV2Model item in eng.carbonEmissionPayItems)
                {
                    Null2Empty(item);
                    sql = @"
                        insert into CarbonEmissionPayItem (
                            CarbonEmissionHeaderSeq
                            ,PayItem,[Description],Unit,Quantity,Price,Amount,ItemKey,ItemNo,RefItemCode
                            ,RStatusCode,KgCo2e,ItemKgCo2e,Memo,RStatus,GreenFundingSeq,GreenFundingMemo
                            ,CreateTime,CreateUserSeq,ModifyTime,ModifyUserSeq
                        ) values (
                            @CarbonEmissionHeaderSeq
                            ,@PayItem,@Description,@Unit,@Quantity,@Price,@Amount,@ItemKey,@ItemNo,@RefItemCode
                            ,@RStatusCode,@KgCo2e,@ItemKgCo2e,@Memo,@RStatus,@GreenFundingSeq,@GreenFundingMemo
                            ,GetDate(),@ModifyUserSeq,GetDate(),@ModifyUserSeq
                        )
                        ";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@CarbonEmissionHeaderSeq", carbonEmissionHeaderSeq);
                    cmd.Parameters.AddWithValue("@PayItem", item.PayItem);
                    cmd.Parameters.AddWithValue("@Description", item.Description);
                    cmd.Parameters.AddWithValue("@Unit", item.Unit);
                    cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                    cmd.Parameters.AddWithValue("@Price", item.Price);
                    cmd.Parameters.AddWithValue("@Amount", item.Amount);
                    cmd.Parameters.AddWithValue("@ItemKey", item.ItemKey);
                    cmd.Parameters.AddWithValue("@ItemNo", item.ItemNo);
                    cmd.Parameters.AddWithValue("@RefItemCode", item.RefItemCode);
                    cmd.Parameters.AddWithValue("@RStatusCode", item.RStatusCode);
                    cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(item.KgCo2e));
                    cmd.Parameters.AddWithValue("@ItemKgCo2e", this.NulltoDBNull(item.ItemKgCo2e));
                    cmd.Parameters.AddWithValue("@Memo", item.Memo);
                    cmd.Parameters.AddWithValue("@RStatus", item.RStatus);
                    cmd.Parameters.AddWithValue("@GreenFundingSeq", this.NulltoDBNull(item.GreenFundingSeq));
                    cmd.Parameters.AddWithValue("@GreenFundingMemo", item.GreenFundingMemo);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);

                    cmd.Parameters.Clear();
                    sql1 = @" SELECT IDENT_CURRENT('CarbonEmissionPayItem') AS NewSeq";
                    cmd = db.GetCommand(sql1);
                    dt = db.GetDataTable(cmd);
                    int payItemSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
                    if (item.WorkItemCnt > 0)
                    {
                        sql = @"
				        INSERT INTO CarbonEmissionWorkItem (
                            CarbonEmissionPayItemSeq, WorkItemQuantity, ItemCode, ItemKind, Description, Unit, Quantity,
                            Price, Amount, Remark, LabourRatio, EquipmentRatio, MaterialRatio, MiscellaneaRatio
                        )values(
                            @CarbonEmissionPayItemSeq, @WorkItemQuantity, @ItemCode, @ItemKind, @Description, @Unit, @Quantity,
                            @Price, @Amount, @Remark, @LabourRatio, @EquipmentRatio, @MaterialRatio, @MiscellaneaRatio
                        )";
                        foreach (WorkItemModel wi in item.workItems)
                        {
                            Null2Empty(wi);
                            cmd = db.GetCommand(sql);
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@CarbonEmissionPayItemSeq", payItemSeq);
                            cmd.Parameters.AddWithValue("@WorkItemQuantity", wi.WorkItemQuantity);
                            cmd.Parameters.AddWithValue("@ItemCode", wi.ItemCode);
                            cmd.Parameters.AddWithValue("@ItemKind", wi.ItemKind);
                            cmd.Parameters.AddWithValue("@Description", wi.Description);
                            cmd.Parameters.AddWithValue("@Unit", wi.Unit);
                            cmd.Parameters.AddWithValue("@Quantity", wi.Quantity);
                            cmd.Parameters.AddWithValue("@Price", wi.Price);
                            cmd.Parameters.AddWithValue("@Amount", wi.Amount);
                            cmd.Parameters.AddWithValue("@Remark", wi.Remark);
                            cmd.Parameters.AddWithValue("@LabourRatio", wi.LabourRatio);
                            cmd.Parameters.AddWithValue("@EquipmentRatio", wi.EquipmentRatio);
                            cmd.Parameters.AddWithValue("@MaterialRatio", wi.MaterialRatio);
                            cmd.Parameters.AddWithValue("@MiscellaneaRatio", wi.MiscellaneaRatio);
                            db.ExecuteNonQuery(cmd);
                        }
                    }
                }
                //第五章 材料設備送審清冊 
                foreach (EngMaterialDeviceList2VModel m in eng.engMaterialDeviceItems)
                {
                    Null2Empty(m);
                    sql = @"
                        insert into EngMaterialDeviceList (
                            OrderNo,
                            EngMainSeq,
                            ParentSeq,
                            ItemNo,
                            MDName,
                            ExcelNo,
                            FlowCharOriginFileName,
                            FlowCharUniqueFileName,
                            DataKeep,
                            DataType,
                            IsAuditVendor,
                            IsAuditCatalog,
                            IsAuditReport,
                            IsAuditSample,
                            OtherAudit,
                            CreateTime,
                            CreateUserSeq,
                            ModifyTime,
                            ModifyUserSeq
                        ) values (
                            @OrderNo,
                            @EngMainSeq,
                            @ParentSeq,
                            @ItemNo,
                            @MDName,
                            @ExcelNo,
                            @FlowCharOriginFileName,
                            @FlowCharUniqueFileName,
                            @DataKeep,
                            @DataType,
                            @IsAuditVendor,
                            @IsAuditCatalog,
                            @IsAuditReport,
                            @IsAuditSample,
                            @OtherAudit,
                            GETDATE(),
                            @ModifyUserSeq,
                            GETDATE(),
                            @ModifyUserSeq
                        )";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EngMainSeq", newEngMainSeq);
                    cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
                    cmd.Parameters.AddWithValue("@ParentSeq", this.NulltoDBNull(m.ParentSeq));
                    cmd.Parameters.AddWithValue("@ItemNo", m.ItemNo);
                    cmd.Parameters.AddWithValue("@MDName", m.MDName);
                    cmd.Parameters.AddWithValue("@ExcelNo", m.ExcelNo);
                    cmd.Parameters.AddWithValue("@FlowCharOriginFileName", m.FlowCharOriginFileName);
                    cmd.Parameters.AddWithValue("@FlowCharUniqueFileName", m.FlowCharUniqueFileName);
                    cmd.Parameters.AddWithValue("@DataKeep", m.DataKeep);
                    cmd.Parameters.AddWithValue("@DataType", m.DataType);
                    cmd.Parameters.AddWithValue("@IsAuditVendor",m.IsAuditVendor);
                    cmd.Parameters.AddWithValue("@IsAuditCatalog", m.IsAuditCatalog);
                    cmd.Parameters.AddWithValue("@IsAuditReport", m.IsAuditReport);
                    cmd.Parameters.AddWithValue("@IsAuditSample", m.IsAuditSample);
                    cmd.Parameters.AddWithValue("@OtherAudit", m.OtherAudit);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                    db.ExecuteNonQuery(cmd);

                    cmd.Parameters.Clear();
                    sql1 = @"SELECT IDENT_CURRENT('EngMaterialDeviceList') AS NewSeq";
                    cmd = db.GetCommand(sql1);
                    dt = db.GetDataTable(cmd);
                    int engMaterialDeviceListSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
                    copyFile(srcEngFilePath, newEngFilePath, m.FlowCharUniqueFileName);

                    //材料設備送審管制總表
                    sql = @"
                        insert into EngMaterialDeviceSummary(
                            EngMaterialDeviceListSeq,
                            OrderNo,
                            ItemNo,
                            MDName,
                            ContactQty,
                            ContactUnit,
                            IsSampleTest,
                            CreateTime,
                            CreateUserSeq,
                            ModifyTime,
                            ModifyUserSeq
                        ) values (
                            @EngMaterialDeviceListSeq,
                            @OrderNo,
                            @ItemNo,
                            @MDName,
                            @ContactQty,
                            @ContactUnit,
                            1,
                            GETDATE(),
                            @ModifyUserSeq,
                            GETDATE(),
                            @ModifyUserSeq
                        )";
                    foreach (EngMaterialDeviceSummaryModel m1 in m.engMaterialDeviceSummaryItems)
                    {
                        Null2Empty(m1);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", engMaterialDeviceListSeq);
                        cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m1.OrderNo));
                        cmd.Parameters.AddWithValue("@ItemNo", m1.ItemNo);
                        cmd.Parameters.AddWithValue("@MDName", m1.MDName);
                        cmd.Parameters.AddWithValue("@ContactQty", m1.ContactQty);
                        cmd.Parameters.AddWithValue("@ContactUnit", m1.ContactUnit);
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                        db.ExecuteNonQuery(cmd);
                    }
                    //材料設備抽查管理標準
                    sql = @"
                        insert into EngMaterialDeviceControlSt (
                            DataType,
                            DataKeep,
                            EngMaterialDeviceListSeq,
                            OrderNo,
                            MDTestItem,
                            MDTestStand1,
                            MDTestStand2,
                            MDTestTime,
                            MDTestMethod,
                            MDTestFeq,
                            MDIncomp,
                            MDManageRec,
                            MDMemo,
                            CreateTime,
                            CreateUserSeq,
                            ModifyTime,
                            ModifyUserSeq
                        ) values (
                            @DataType,
                            @DataKeep,
                            @EngMaterialDeviceListSeq,
                            @OrderNo,
                            @MDTestItem,
                            @MDTestStand1,
                            @MDTestStand2,
                            @MDTestTime,
                            @MDTestMethod,
                            @MDTestFeq,
                            @MDIncomp,
                            @MDManageRec,
                            @MDMemo,
                            GETDATE(),
                            @ModifyUserSeq,
                            GETDATE(),
                            @ModifyUserSeq
                        )";
                    foreach (EngMaterialDeviceControlStModel m1 in m.controlItems)
                    {
                        Null2Empty(m1);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@DataType", m1.DataType);
                        cmd.Parameters.AddWithValue("@DataKeep", m1.DataKeep);
                        cmd.Parameters.AddWithValue("@EngMaterialDeviceListSeq", engMaterialDeviceListSeq);
                        cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m1.OrderNo));
                        cmd.Parameters.AddWithValue("@MDTestItem", m1.MDTestItem);
                        cmd.Parameters.AddWithValue("@MDTestStand1", m1.MDTestStand1);
                        cmd.Parameters.AddWithValue("@MDTestStand2", m1.MDTestStand2);
                        cmd.Parameters.AddWithValue("MDTestTime", m1.MDTestTime);
                        cmd.Parameters.AddWithValue("@MDTestMethod", m1.MDTestMethod);
                        cmd.Parameters.AddWithValue("@MDTestFeq", m1.MDTestFeq);
                        cmd.Parameters.AddWithValue("@MDIncomp", m1.MDIncomp);
                        cmd.Parameters.AddWithValue("@MDManageRec", m1.MDManageRec);
                        cmd.Parameters.AddWithValue("@MDMemo", m1.MDMemo);
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                        db.ExecuteNonQuery(cmd);
                    }
                }

                //第六章 設備功能運轉測試抽驗程序及標準
                foreach (EquOperTestListV2Model m in eng.equOperTestItems)
                {
                    Null2Empty(m);
                    sql = @"
                        insert into EquOperTestList (
                            DataType,
                            DataKeep,
                            EngMainSeq,
                            EPKind,
                            ExcelNo,
                            ItemName,
                            FlowCharOriginFileName,
                            FlowCharUniqueFileName,
                            OrderNo,
                            CreateTime,
                            CreateUserSeq,
                            ModifyTime,
                            ModifyUserSeq
                        ) values (
                            @DataType,
                            @DataKeep,
                            @EngMainSeq,
                            @EPKind,
                            @ExcelNo,
                            @ItemName,
                            @FlowCharOriginFileName,
                            @FlowCharUniqueFileName,
                            @OrderNo,
                            GETDATE(),
                            @ModifyUserSeq,
                            GETDATE(),
                            @ModifyUserSeq
                        )";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@DataType", m.DataType);
                    cmd.Parameters.AddWithValue("@DataKeep", m.DataKeep);
                    cmd.Parameters.AddWithValue("@EngMainSeq", newEngMainSeq);
                    cmd.Parameters.AddWithValue("@EPKind", m.EPKind);
                    cmd.Parameters.AddWithValue("@ExcelNo", m.ExcelNo);
                    cmd.Parameters.AddWithValue("@ItemName", m.ItemName);
                    cmd.Parameters.AddWithValue("@FlowCharOriginFileName", m.FlowCharOriginFileName);
                    cmd.Parameters.AddWithValue("@FlowCharUniqueFileName", m.FlowCharUniqueFileName);
                    cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                    db.ExecuteNonQuery(cmd);

                    sql1 = @"SELECT IDENT_CURRENT('EquOperTestList') AS NewSeq";
                    cmd = db.GetCommand(sql1);
                    cmd.Parameters.Clear();
                    dt = db.GetDataTable(cmd);
                    int equOperTestStSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
                    copyFile(srcEngFilePath, newEngFilePath, m.FlowCharUniqueFileName);

                    //設備運轉抽查標準
                    sql = @"
                        insert into EquOperControlSt (
                            DataType,
                            DataKeep,
                            EquOperTestStSeq,
                            EPCheckItem1,
                            EPCheckItem2,
                            EPStand1,
                            EPStand2,
                            EPStand3,
                            EPStand4,
                            EPStand5,
                            EPCheckTiming,
                            EPCheckMethod,
                            EPCheckFeq,
                            EPIncomp,
                            EPManageRec,
                            EPType,
                            EPMemo,
                            EPCheckFields,
                            EPManageFields,
                            OrderNo,
                            CreateTime,
                            CreateUserSeq,
                            ModifyTime,
                            ModifyUserSeq
                        ) values (
                            @DataType,
                            @DataKeep,
                            @EquOperTestStSeq,
                            @EPCheckItem1,
                            @EPCheckItem2,
                            @EPStand1,
                            @EPStand2,
                            @EPStand3,
                            @EPStand4,
                            @EPStand5,
                            @EPCheckTiming,
                            @EPCheckMethod,
                            @EPCheckFeq,
                            @EPIncomp,
                            @EPManageRec,
                            @EPType,
                            @EPMemo,
                            @EPCheckFields,
                            @EPManageFields,
                            @OrderNo,
                            GETDATE(),
                            @ModifyUserSeq,
                            GETDATE(),
                            @ModifyUserSeq
                        )";
                    foreach (EquOperControlStModel m1 in m.controlItems)
                    {
                        Null2Empty(m1);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@EquOperTestStSeq", equOperTestStSeq);
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                        cmd.Parameters.AddWithValue("@EPCheckItem1", m1.EPCheckItem1);
                        cmd.Parameters.AddWithValue("@EPCheckItem2", m1.EPCheckItem2);
                        cmd.Parameters.AddWithValue("@EPStand1", m1.EPStand1);
                        cmd.Parameters.AddWithValue("@EPStand2", m1.EPStand2);
                        cmd.Parameters.AddWithValue("@EPStand3", m1.EPStand3);
                        cmd.Parameters.AddWithValue("@EPStand4", m1.EPStand4);
                        cmd.Parameters.AddWithValue("@EPStand5", m1.EPStand5);
                        cmd.Parameters.AddWithValue("@EPCheckTiming", m1.EPCheckTiming);
                        cmd.Parameters.AddWithValue("@EPCheckMethod", m1.EPCheckMethod);
                        cmd.Parameters.AddWithValue("@EPCheckFeq", m1.EPCheckFeq);
                        cmd.Parameters.AddWithValue("@EPIncomp", m1.EPIncomp);
                        cmd.Parameters.AddWithValue("@EPManageRec", m1.EPManageRec);
                        cmd.Parameters.AddWithValue("@EPType", m1.EPType);
                        cmd.Parameters.AddWithValue("@EPMemo", m1.EPMemo);
                        cmd.Parameters.AddWithValue("@EPCheckFields", this.NulltoDBNull(m1.EPCheckFields));
                        cmd.Parameters.AddWithValue("@EPManageFields", this.NulltoDBNull(m1.EPManageFields));
                        cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m1.OrderNo));
                        cmd.Parameters.AddWithValue("@DataType", m1.DataType);
                        cmd.Parameters.AddWithValue("@DataKeep", m1.DataKeep);

                        db.ExecuteNonQuery(cmd);
                    }
                    //FlowChart
                    sql = @"Insert into FlowChartTpDiagram(FlowChartTpSeq, DiagramJson, Type)
                            values (@FlowChartTpSeq, @DiagramJson, @Type)";
                    foreach (FlowChartTpDiagramModel m1 in m.flowChartItems)
                    {
                        Null2Empty(m1);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.AddWithValue("@FlowChartTpSeq", equOperTestStSeq);
                        cmd.Parameters.AddWithValue("@DiagramJson", m1.DiagramJson);
                        cmd.Parameters.AddWithValue("@Type", m1.Type);
                        db.ExecuteNonQuery(cmd);
                    }
                }

                //第七章 701 施工抽查程序及標準
                foreach (ConstCheckListV2Model m in eng.constCheckItems)
                {
                    Null2Empty(m);
                    sql = @"
                        insert into ConstCheckList (
                            DataType,
                            DataKeep,
                            EngMainSeq,
                            ExcelNo,
                            ItemName,
                            FlowCharOriginFileName,
                            FlowCharUniqueFileName,
                            OrderNo,
                            CreateTime,
                            CreateUserSeq,
                            ModifyTime,
                            ModifyUserSeq
                        ) values (
                            @DataType,
                            @DataKeep,
                            @EngMainSeq,
                            @ExcelNo,
                            @ItemName,
                            @FlowCharOriginFileName,
                            @FlowCharUniqueFileName,
                            @OrderNo,
                            GETDATE(),
                            @ModifyUserSeq,
                            GETDATE(),
                            @ModifyUserSeq
                        )";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@DataType", m.DataType);
                    cmd.Parameters.AddWithValue("@DataKeep", m.DataKeep);
                    cmd.Parameters.AddWithValue("@EngMainSeq", newEngMainSeq);
                    cmd.Parameters.AddWithValue("@ExcelNo", m.ExcelNo);
                    cmd.Parameters.AddWithValue("@ItemName", m.ItemName);
                    cmd.Parameters.AddWithValue("@FlowCharOriginFileName", m.FlowCharOriginFileName);
                    cmd.Parameters.AddWithValue("@FlowCharUniqueFileName", m.FlowCharUniqueFileName);
                    cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                    db.ExecuteNonQuery(cmd);
                    
                    sql1 = @"SELECT IDENT_CURRENT('ConstCheckList') AS NewSeq";
                    cmd = db.GetCommand(sql1);
                    cmd.Parameters.Clear();
                    dt = db.GetDataTable(cmd);
                    int constCheckListSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
                    copyFile(srcEngFilePath, newEngFilePath, m.FlowCharUniqueFileName);

                    //施工抽查標準
                    foreach (ConstCheckControlStModel m1 in m.controlItems)
                    {
                        Null2Empty(m1);
                        sql = @"
                            insert into ConstCheckControlSt (
                                DataType,
                                DataKeep,
                                ConstCheckListSeq,
                                CCFlow1,
                                CCFlow2,
                                CCManageItem1,
                                CCManageItem2,
                                CCCheckStand1,
                                CCCheckStand2,
                                CCCheckTiming,
                                CCCheckMethod,
                                CCCheckFeq,
                                CCIncomp,
                                CCManageRec,
                                CCType,
                                CCMemo,
                                CCCheckFields,
                                CCManageFields,
                                OrderNo,
                                CreateTime,
                                CreateUserSeq,
                                ModifyTime,
                                ModifyUserSeq
                            ) values (
                                @DataType,
                                @DataKeep,
                                @ConstCheckListSeq,
                                @CCFlow1,
                                @CCFlow2,
                                @CCManageItem1,
                                @CCManageItem2,
                                @CCCheckStand1,
                                @CCCheckStand2,
                                @CCCheckTiming,
                                @CCCheckMethod,
                                @CCCheckFeq,
                                @CCIncomp,
                                @CCManageRec,
                                @CCType,
                                @CCMemo,
                                @CCCheckFields,
                                @CCManageFields,
                                @OrderNo,
                                GETDATE(),
                                @ModifyUserSeq,
                                GETDATE(),
                                @ModifyUserSeq
                            )";
                        cmd = db.GetCommand(sql);

                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@ConstCheckListSeq", constCheckListSeq);
                        cmd.Parameters.AddWithValue("@CCFlow1", m1.CCFlow1);
                        cmd.Parameters.AddWithValue("@CCFlow2", m1.CCFlow2);
                        cmd.Parameters.AddWithValue("@CCManageItem1", m1.CCManageItem1);
                        cmd.Parameters.AddWithValue("@CCManageItem2", m1.CCManageItem2);
                        cmd.Parameters.AddWithValue("@CCCheckStand1", m1.CCCheckStand1);
                        cmd.Parameters.AddWithValue("@CCCheckStand2", m1.CCCheckStand2);
                        cmd.Parameters.AddWithValue("@CCCheckTiming", m1.CCCheckTiming);
                        cmd.Parameters.AddWithValue("@CCCheckMethod", m1.CCCheckMethod);
                        cmd.Parameters.AddWithValue("@CCCheckFeq", m1.CCCheckFeq);
                        cmd.Parameters.AddWithValue("@CCIncomp", m1.CCIncomp);
                        cmd.Parameters.AddWithValue("@CCManageRec", m1.CCManageRec);
                        cmd.Parameters.AddWithValue("@CCType", this.NulltoDBNull(m1.CCType)); //s20230829
                        cmd.Parameters.AddWithValue("@CCMemo", m1.CCMemo);
                        cmd.Parameters.AddWithValue("@CCCheckFields", this.NulltoDBNull(m1.CCCheckFields)); //s20230829
                        cmd.Parameters.AddWithValue("@CCManageFields", this.NulltoDBNull(m1.CCManageFields)); //s20230829
                        cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m1.OrderNo));
                        cmd.Parameters.AddWithValue("@DataType", m1.DataType);
                        cmd.Parameters.AddWithValue("@DataKeep", m1.DataKeep);
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);

                        int result = db.ExecuteNonQuery(cmd);
                    }
                    //FlowChart
                    sql = @"Insert into FlowChartTpDiagram(FlowChartTpSeq, DiagramJson, Type)
                            values (@FlowChartTpSeq, @DiagramJson, @Type)";
                    foreach (FlowChartTpDiagramModel m1 in m.flowChartItems)
                    {
                        Null2Empty(m1);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.AddWithValue("@FlowChartTpSeq", constCheckListSeq);
                        cmd.Parameters.AddWithValue("@DiagramJson", m1.DiagramJson);
                        cmd.Parameters.AddWithValue("@Type", m1.Type);
                        db.ExecuteNonQuery(cmd);
                    }
                }

                //第七章 702 環境保育抽查標準
                foreach (EnvirConsListV2Model m in eng.envirConsItems)
                {
                    Null2Empty(m);
                    sql = @"
                        insert into EnvirConsList (
                            DataType,
                            DataKeep,
                            EngMainSeq,
                            ExcelNo,
                            ItemName,
                            FlowCharOriginFileName,
                            FlowCharUniqueFileName,
                            OrderNo,
                            CreateTime,
                            CreateUserSeq,
                            ModifyTime,
                            ModifyUserSeq
                        ) values (
                            @DataType,
                            @DataKeep,
                            @EngMainSeq,
                            @ExcelNo,
                            @ItemName,
                            @FlowCharOriginFileName,
                            @FlowCharUniqueFileName,
                            @OrderNo,
                            GETDATE(),
                            @ModifyUserSeq,
                            GETDATE(),
                            @ModifyUserSeq
                        )";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@DataType", m.DataType);
                    cmd.Parameters.AddWithValue("@DataKeep", m.DataKeep);
                    cmd.Parameters.AddWithValue("@EngMainSeq", newEngMainSeq);
                    cmd.Parameters.AddWithValue("@ExcelNo", m.ExcelNo);
                    cmd.Parameters.AddWithValue("@ItemName", m.ItemName);
                    cmd.Parameters.AddWithValue("@FlowCharOriginFileName", m.FlowCharOriginFileName);
                    cmd.Parameters.AddWithValue("@FlowCharUniqueFileName", m.FlowCharUniqueFileName);
                    cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                    db.ExecuteNonQuery(cmd);
                    
                    sql1 = @"SELECT IDENT_CURRENT('EnvirConsList') AS NewSeq";
                    cmd = db.GetCommand(sql1);
                    cmd.Parameters.Clear();
                    dt = db.GetDataTable(cmd);
                    int envirConsListSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
                    copyFile(srcEngFilePath, newEngFilePath, m.FlowCharUniqueFileName);

                    //環境保育抽查標準
                    sql = @"
                        insert into EnvirConsControlSt (
                            DataType,
                            DataKeep,
                            EnvirConsListSeq,
                            ECCFlow1,
                            ECCCheckItem1,
                            ECCCheckItem2,
                            ECCStand1,
                            ECCStand2,
                            ECCStand3,
                            ECCStand4,
                            ECCStand5,
                            ECCCheckTiming,
                            ECCCheckMethod,
                            ECCCheckFeq,
                            ECCIncomp,
                            ECCManageRec,
                            ECCType,
                            ECCMemo,
                            ECCCheckFields,
                            ECCManageFields,
                            OrderNo,
                            CreateTime,
                            CreateUserSeq,
                            ModifyTime,
                            ModifyUserSeq
                        ) values (
                            @DataType,
                            @DataKeep,
                            @EnvirConsListSeq,
                            @ECCFlow1,
                            @ECCCheckItem1,
                            @ECCCheckItem2,
                            @ECCStand1,
                            @ECCStand2,
                            @ECCStand3,
                            @ECCStand4,
                            @ECCStand5,
                            @ECCCheckTiming,
                            @ECCCheckMethod,
                            @ECCCheckFeq,
                            @ECCIncomp,
                            @ECCManageRec,
                            @ECCType,
                            @ECCMemo,
                            @ECCCheckFields,
                            @ECCManageFields,
                            @OrderNo,
                            GETDATE(),
                            @ModifyUserSeq,
                            GETDATE(),
                            @ModifyUserSeq
                        )";
                    foreach (EnvirConsControlStModel m1 in m.controlItems)
                    {
                        Null2Empty(m1);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@ECCFlow1", m1.ECCFlow1);
                        cmd.Parameters.AddWithValue("@ECCCheckItem1", m1.ECCCheckItem1);
                        cmd.Parameters.AddWithValue("@ECCCheckItem2", m1.ECCCheckItem2);
                        cmd.Parameters.AddWithValue("@ECCStand1", m1.ECCStand1);
                        cmd.Parameters.AddWithValue("@ECCStand2", m1.ECCStand2);
                        cmd.Parameters.AddWithValue("@ECCStand3", m1.ECCStand3);
                        cmd.Parameters.AddWithValue("@ECCStand4", m1.ECCStand4);
                        cmd.Parameters.AddWithValue("@ECCStand5", m1.ECCStand5);
                        cmd.Parameters.AddWithValue("@ECCCheckTiming", m1.ECCCheckTiming);
                        cmd.Parameters.AddWithValue("@ECCCheckMethod", m1.ECCCheckMethod);
                        cmd.Parameters.AddWithValue("@ECCCheckFeq", m1.ECCCheckFeq);
                        cmd.Parameters.AddWithValue("@ECCIncomp", m1.ECCIncomp);
                        cmd.Parameters.AddWithValue("@ECCManageRec", m1.ECCManageRec);
                        cmd.Parameters.AddWithValue("@ECCType", this.NulltoDBNull(m1.ECCType)); //s20230830
                        cmd.Parameters.AddWithValue("@ECCMemo", m1.ECCMemo);
                        cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m1.OrderNo));
                        cmd.Parameters.AddWithValue("@ECCCheckFields", this.NulltoDBNull(m1.ECCCheckFields)); //s20230830
                        cmd.Parameters.AddWithValue("@ECCManageFields", this.NulltoDBNull(m1.ECCManageFields)); //s20230830
                        cmd.Parameters.AddWithValue("@EnvirConsListSeq", envirConsListSeq);
                        cmd.Parameters.AddWithValue("@DataType", m1.DataType);
                        cmd.Parameters.AddWithValue("@DataKeep", m1.DataKeep);
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);

                        int result = db.ExecuteNonQuery(cmd);
                    }
                    //FlowChart
                    sql = @"Insert into FlowChartTpDiagram(FlowChartTpSeq, DiagramJson, Type)
                            values (@FlowChartTpSeq, @DiagramJson, @Type)";
                    foreach (FlowChartTpDiagramModel m1 in m.flowChartItems)
                    {
                        Null2Empty(m1);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.AddWithValue("@FlowChartTpSeq", envirConsListSeq);
                        cmd.Parameters.AddWithValue("@DiagramJson", m1.DiagramJson);
                        cmd.Parameters.AddWithValue("@Type", m1.Type);
                        db.ExecuteNonQuery(cmd);
                    }
                }

                //第七章 703 職業安全衛生抽查標準
                foreach (OccuSafeHealthListV2Model m in eng.occuSafeHealthItems)
                {
                    Null2Empty(m);
                    sql = @"
                    insert into OccuSafeHealthList (
                        DataType,
                        DataKeep,
                        EngMainSeq,
                        ExcelNo,
                        ItemName,
                        FlowCharOriginFileName,
                        FlowCharUniqueFileName,
                        OrderNo,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    ) values (
                        @DataType,
                        @DataKeep,
                        @EngMainSeq,
                        @ExcelNo,
                        @ItemName,
                        @FlowCharOriginFileName,
                        @FlowCharUniqueFileName,
                        @OrderNo,
                        GETDATE(),
                        @ModifyUserSeq,
                        GETDATE(),
                        @ModifyUserSeq
                    )";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@DataType", m.DataType);
                    cmd.Parameters.AddWithValue("@DataKeep", m.DataKeep);
                    cmd.Parameters.AddWithValue("@EngMainSeq", newEngMainSeq);
                    cmd.Parameters.AddWithValue("@ExcelNo", m.ExcelNo);
                    cmd.Parameters.AddWithValue("@ItemName", m.ItemName);
                    cmd.Parameters.AddWithValue("@FlowCharOriginFileName", m.FlowCharOriginFileName);
                    cmd.Parameters.AddWithValue("@FlowCharUniqueFileName", m.FlowCharUniqueFileName);
                    cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m.OrderNo));
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                    db.ExecuteNonQuery(cmd);

                    sql1 = @"SELECT IDENT_CURRENT('OccuSafeHealthList') AS NewSeq";
                    cmd = db.GetCommand(sql1);
                    cmd.Parameters.Clear();
                    dt = db.GetDataTable(cmd);
                    int occuSafeHealthListSeq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
                    copyFile(srcEngFilePath, newEngFilePath, m.FlowCharUniqueFileName);

                    //職業安全衛生抽查標準
                    sql = @"
                        insert into OccuSafeHealthControlSt (
                            DataType,
                            DataKeep,
                            OccuSafeHealthListSeq,
                            OSCheckItem1,
                            OSCheckItem2,
                            OSStand1,
                            OSStand2,
                            OSStand3,
                            OSStand4,
                            OSStand5,
                            OSCheckTiming,
                            OSCheckMethod,
                            OSCheckFeq,
                            OSIncomp,
                            OSManageRec,
                            OSType,
                            OSMemo,
                            OSCheckFields,
                            OSManageFields,
                            OrderNo,
                            CreateTime,
                            CreateUserSeq,
                            ModifyTime,
                            ModifyUserSeq
                        ) values (
                            @DataType,
                            @DataKeep,
                            @OccuSafeHealthListSeq,
                            @OSCheckItem1,
                            @OSCheckItem2,
                            @OSStand1,
                            @OSStand2,
                            @OSStand3,
                            @OSStand4,
                            @OSStand5,
                            @OSCheckTiming,
                            @OSCheckMethod,
                            @OSCheckFeq,
                            @OSIncomp,
                            @OSManageRec,
                            @OSType,
                            @OSMemo,
                            @OSCheckFields,
                            @OSManageFields,
                            @OrderNo,
                            GETDATE(),
                            @ModifyUserSeq,
                            GETDATE(),
                            @ModifyUserSeq
                        )";
                    foreach (OccuSafeHealthControlStModel m1 in m.controlItems)
                    {
                        Null2Empty(m1);
                        
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@OccuSafeHealthListSeq", occuSafeHealthListSeq);
                        cmd.Parameters.AddWithValue("@OSCheckItem1", m1.OSCheckItem1);
                        cmd.Parameters.AddWithValue("@OSCheckItem2", m1.OSCheckItem2);
                        cmd.Parameters.AddWithValue("@OSStand1", m1.OSStand1);
                        cmd.Parameters.AddWithValue("@OSStand2", m1.OSStand2);
                        cmd.Parameters.AddWithValue("@OSStand3", m1.OSStand3);
                        cmd.Parameters.AddWithValue("@OSStand4", m1.OSStand4);
                        cmd.Parameters.AddWithValue("@OSStand5", m1.OSStand5);
                        cmd.Parameters.AddWithValue("@OSCheckTiming", m1.OSCheckTiming);
                        cmd.Parameters.AddWithValue("@OSCheckMethod", m1.OSCheckMethod);
                        cmd.Parameters.AddWithValue("@OSCheckFeq", m1.OSCheckFeq);
                        cmd.Parameters.AddWithValue("@OSIncomp", m1.OSIncomp);
                        cmd.Parameters.AddWithValue("@OSManageRec", m1.OSManageRec);
                        cmd.Parameters.AddWithValue("@OSType", this.NulltoDBNull(m1.OSType)); //s20230830
                        cmd.Parameters.AddWithValue("@OSMemo", m1.OSMemo);
                        cmd.Parameters.AddWithValue("@OrderNo", this.NulltoDBNull(m1.OrderNo));
                        cmd.Parameters.AddWithValue("@OSCheckFields", this.NulltoDBNull(m1.OSCheckFields)); //s20230829
                        cmd.Parameters.AddWithValue("@OSManageFields", this.NulltoDBNull(m1.OSManageFields)); //s20230829
                        cmd.Parameters.AddWithValue("@ModifyUserSeq", userSeq);
                        cmd.Parameters.AddWithValue("@DataType", m1.DataType);
                        cmd.Parameters.AddWithValue("@DataKeep", m1.DataKeep);
                        db.ExecuteNonQuery(cmd);
                    }
                    //FlowChart
                    sql = @"Insert into FlowChartTpDiagram(FlowChartTpSeq, DiagramJson, Type)
                            values (@FlowChartTpSeq, @DiagramJson, @Type)";
                    foreach (FlowChartTpDiagramModel m1 in m.flowChartItems)
                    {
                        Null2Empty(m1);
                        cmd = db.GetCommand(sql);
                        cmd.Parameters.AddWithValue("@FlowChartTpSeq", occuSafeHealthListSeq);
                        cmd.Parameters.AddWithValue("@DiagramJson", m1.DiagramJson);
                        cmd.Parameters.AddWithValue("@Type", m1.Type);
                        db.ExecuteNonQuery(cmd);
                    }
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("EngMainService.CopyEng" + e.Message);
                return false;
            }
        }
        private void copyFile(string srcPath, string tarPath, string uName)
        {
            if (!String.IsNullOrEmpty(uName))
            {
                if (System.IO.File.Exists(Path.Combine(srcPath, uName)))
                {
                    System.IO.File.Copy(Path.Combine(srcPath, uName), Path.Combine(tarPath, uName));
                }
            }
        }
    }
}