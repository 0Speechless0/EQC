using EQC.Common;
using EQC.EDMXModel;
using EQC.Models;
using EQC.ViewModel;
using EQC.ViewModel.Common;
using NPOI.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class EngReportService : BaseService
    {
        private UnitService unitService = new UnitService();

        private string getAuthoritySql(string alias,  int rptType = 0)
        {
            string sql = " and 1=0";
            System.Web.SessionState.HttpSessionState _session = System.Web.HttpContext.Current.Session;

            int roleSeq; string roleSeqId; UserInfo userInfo = Utils.getUserInfo() ; VUserMain user = new VUserMain() ;

            roleSeq = userInfo.RoleSeq;

            if (roleSeq == 1)
            {
                sql = " and 1=1";
            }

            if (roleSeq == 2) //署內管理者
            {
                sql = " ";
                /*sql = String.Format(@" and (
                        {0}OrganizerUnitSeq={1}
                        or
                        {0}ExecUnitSeq={1}
                    ) ", alias, userInfo.UnitSeq1);*/
            }
            else if (roleSeq == 3) //各局管理者
            {
                //if (userInfo.UnitSeq2 == null)
                //{
                sql = String.Format(@" and (
                        {0}ExecUnitSeq={1}
                        or (
                        {0}Seq in (select Seq from EngReportList where CreateUserSeq={2})
                        )
                    ) ", alias, userInfo.UnitSeq1,  getUserSeq());
                //}
                //else
                //{
                //    sql = String.Format(@" and (
                //        ( {0}ExecUnitSeq={1} and {0}ExecSubUnitSeq = {2} )
                //        or
                //        ( {0}ExecUnitSeq={1} ) 
                //        or (
                //        {0}Seq in (select Seq from EngReportList where CreateUserSeq={3})
                //        )
                //    ) ", alias, userInfo.UnitSeq1, userInfo.UnitSeq2, userNo != null ? user.Seq : getUserSeq());
                //}
            }
            else if (roleSeq == 20) //署內執行者
            {
                sql = String.Format(@" and (
                        {0}Seq in (
                            select Seq from EngReportList 
                                where  CreateUserSeq={1} 
                        )
                    ) ", alias,  getUserSeq());

            }

            return sql;
        }

        #region === 下拉選單

        //工程年度清單
        public List<EngYearVModel> GetEngYearList()
        {
            string sql;
            sql = @"
                SELECT DISTINCT
                    cast(a.[RptYear] as integer) EngYear
                FROM [EngReportList] a

                where 1=1 "//CreateUserSeq=@CreateUserSeq"
                + getAuthoritySql("a.")
                + @" 
                order by [EngYear] DESC";

            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<EngYearVModel>(cmd);
        }

        //工程執行機關清單
        public List<EngExecUnitsVModel> GetEngExecUnitList(string engYear)
        {
            int userSeq = new SessionManager().GetUser().Seq;
            string sql;
            int year = Int32.Parse(engYear);

            sql = @"
                SELECT DISTINCT
                    b.OrderNo,
                    b.Seq UnitSeq,
                    b.Name UnitName
                FROM [EngReportList] a
                inner join Unit b on(b.Seq=a.ExecUnitSeq and b.parentSeq is null)

                where 1=1
                " + ((year == -1) ? "" : " and a.RptYear=@EngYear ") + @"
                "
                + getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @" order by b.OrderNo";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngYear", engYear);
            return db.GetDataTableWithClass<EngExecUnitsVModel>(cmd);
        }

        //工程執行單位清單
        public List<EngExecUnitsVModel> GetEngExecSubUnitList(string engYear, int parentSeq)
        {
            int userSeq = new SessionManager().GetUser().Seq;
            string sql;
            int year = Int32.Parse(engYear);
            sql = @"
                SELECT DISTINCT
                    b.OrderNo,
                    a.ExecSubUnitSeq UnitSeq,
                    b.Name UnitName
                FROM [EngReportList] a

                left join Unit b on(b.Seq=a.ExecSubUnitSeq and @ParentSeq=b.parentSeq)
                where 1=1
                " + ((year == -1) ? "" : " and a.RptYear=@RptYear ")
                + ((parentSeq == -1) ? "" : " and b.Seq is not null ")
                + getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @" order by b.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@RptYear", engYear);
            cmd.Parameters.AddWithValue("@ParentSeq", parentSeq);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<EngExecUnitsVModel>(cmd);
        }

        //工程狀態清單
        public List<SelectVM> GetEngRptTypeList(int funcNo)
        {
            string sql;
            sql = @"
                SELECT Cast(Seq as varchar(10)) as Value,
                    TypeName as Text
                FROM [EngReportType]
                WHERE IsEnabled = 1 "
            //+ ((funcNo == 2) ? " and Seq in (1,2,3) " : "")
            //+ ((funcNo == 3) ? " and Seq in (2,4,5,6) " : "")
            + ((funcNo == 4) ? " and Seq in (5,7) " : "")
            + ((funcNo == 5) ? " and Seq in (7,8) " : "")
            + @" order by Seq ";

            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<SelectVM>(cmd);
        }

        //類別
        public List<SelectVM> GetProposalReviewTypeList()
        {
            string sql;
            sql = @"
                SELECT Cast(Seq as varchar(10)) as Value,
                    TypeName as Text
                FROM [ProposalReviewType]
                WHERE IsEnabled = 1 "
            + @" order by Seq ";

            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<SelectVM>(cmd);
        }

        //屬性
        public List<SelectVM> GetProposalReviewAttributesList()
        {
            string sql;
            sql = @"
                SELECT Cast(Seq as varchar(10)) as Value,
                    [AttributesName] as Text
                FROM [ProposalReviewAttributes]
                WHERE IsEnabled = 1 "
            + @" order by [OrderNo] ";

            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<SelectVM>(cmd);
        }

        //河川
        public List<SelectVM> GetRiverList(string Unit,int ParentSeq)
        {
            string sql;
            sql = @"
                SELECT Cast(Seq as varchar(10)) as Value,
                    [Name] as Text
                FROM [RiverList]
                WHERE [Unit] = @Unit AND ISNULL([ParentSeq],0) = @ParentSeq "
            + @" order by [OrderNo] ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Unit", Unit);
            cmd.Parameters.AddWithValue("@ParentSeq", ParentSeq);
            return db.GetDataTableWithClass<SelectVM>(cmd);
        }

        //排水
        public List<SelectVM> GetDrainList()
        {
            string sql;
            sql = @"
                SELECT Cast(Seq as varchar(10)) as Value,
                    [Name] as Text
                FROM [DrainList]"
            + @" order by [OrderNo] ";

            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<SelectVM>(cmd);
        }

        //主要工作內容
        public List<SelectVM> GetReportJobDescriptionList()
        {
            string sql;
            sql = @"
                SELECT Cast(Seq as varchar(10)) as Value,
                    [Name] as Text
                FROM [ReportJobDescriptionList]
                WHERE IsEnabled = 1 "
            + @" order by [OrderNo] ";

            SqlCommand cmd = db.GetCommand(sql);
            return db.GetDataTableWithClass<SelectVM>(cmd);
        }

        //覆核人員
        public List<SelectVM> GetReviewUserList(string UnitSeq,string UnitSubName)
        {
            string sql;
            sql = @"
                SELECT CAST(FN1.[Seq] AS VARCHAR(10)) AS [Value], FN1.[DisplayName] AS [Text] 
                FROM [dbo].[UserMain] FN1 
	                INNER JOIN [UserUnitPosition] FN2 ON FN1.Seq = FN2.UserMainSeq 
	                INNER JOIN [Unit] FN3 ON FN2.UnitSeq = FN3.Seq 
                WHERE FN3.[ParentSeq] = @UnitSeq AND FN3.[Name] = @UnitSubName or @UnitSeq = '-1' 
            ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@UnitSeq", UnitSeq);
            cmd.Parameters.AddWithValue("@UnitSubName", UnitSubName);
            return db.GetDataTableWithClass<SelectVM>(cmd);
        }

        #endregion 

        //工程清單-總筆數
        public int GetEngListCount(int year, int unitSeq, int subUnitSeq, int rptTypeSeq,int funcType, string keyWord = null)
        {
            string sql = @"";
            int user = new SessionManager().GetUser().Seq;

            string subSQL = "";
            switch (funcType)
            {
                case 14:
                    subSQL += " and isnull(a.ProposalAuditOpinion, 2) != 2 ";
                    break;
                //case 2:
                //    subSQL += " and a.RptTypeSeq in (1,2,3) and a.NeedAssessmenApproval = 2 ";
                //    break;
                //case 3:
                //    subSQL += " and a.RptTypeSeq in (2,4,5)  ";
                //    break;
                case 4:
                    subSQL += " and a.RptTypeSeq in (5,7) and a.ProposalAuditOpinion = 1 AND isnull(a.ReviewSort,0)>0 ";
                    break;
                case 5:
                    subSQL += " and a.RptTypeSeq in (7,8) and a.ResolutionAuditOpinion = 1 AND isnull(a.ReviewSort,0)>0 ";
                    break;
                case 11://待覆核或簽核清單
                    subSQL += " and a.RptTypeSeq in (1,3) ";
                    subSQL += "  and _erp.Seq = erp.Seq";
                    subSQL += " and ( (  a.RelatedReportResultsAssignReviewUserSeq = @RelatedReportResultsAssignReviewUserSeq ) ";
                    subSQL += " or    (  a.FacilityManagementAssignReviewUserSeq = @FacilityManagementAssignReviewUserSeq ) ";
                    subSQL += " or    (  a.ProposalScopeLandAssignReviewUserSeq = @ProposalScopeLandAssignReviewUserSeq ) ";
                    subSQL += @" or   ( a.Seq in (SELECT T1.EngReportSeq 
					                                FROM [dbo].[EngReportApprove] T1
						                                INNER JOIN (SELECT MIN(FN1.Seq) AS Seq FROM [dbo].[EngReportApprove] FN1 WHERE ISNULL(FN1.ApproveUserSeq,0)=0 GROUP BY FN1.EngReportSeq) T2 ON T1.Seq=T2.Seq
						                                INNER JOIN [dbo].[EngReportApprovePosition] T3 ON T1.EngReportSeq = T3.EngReportSeq AND T1.GroupId=T3.GroupId AND T1.ApprovalModuleListSeq =T3.ApprovalModuleListSeq 
						                                INNER JOIN [dbo].[UserUnitPosition] T4 ON (T3.SubUnitSeq = T4.UnitSeq  or T3.SubUnitSeq is null) AND T3.PositionSeq = T4.PositionSeq 
					                                WHERE T4.UserMainSeq = @UserSeq) ) ) ";
                    break;
                case 12://提案審查清單
                    subSQL += " and a.RptTypeSeq in (2) ";
                    subSQL += " and isnull(a.IsProposalReview,0) = 1 ";
                    subSQL += " and isnull(a.ProposalAuditOpinion,0) = 0 ";
                    break;
            }

            sql = @"
                    SELECT 						
		            _erp.EngReportSeq Seq
                    FROM [dbo].[EngReportList] a
                
                    " + (@"

                    left join EngReportApprove _erp  on (a.Seq = _erp.EngReportSeq)

                    left join
                    ( select
		                    erp.EngReportSeq , 
		                    Max(erp.GroupId)　maxGroup,
                            Min(erp.Seq) Seq
                    from EngReportApprove erp 
                    where erp.ApproveTime is null 
                    group by erp.EngReportSeq) erp 
                    on (_erp.EngReportSeq = erp.EngReportSeq and _erp.GroupId = erp.maxGroup)

                    ") + @"
                    where (a.ExecUnitSeq=@ExecUnitSeq or  @ExecUnitSeq = -1 )
                    "
                    + ((year == -1) ? "" : " and a.RptYear=" + year)
                    + ((subUnitSeq == -1) ? "" : " and a.ExecSubUnitSeq=" + subUnitSeq)
                    + ((rptTypeSeq == 0) ? "" : "  and a.RptTypeSeq=" + rptTypeSeq)

                    ////+ ((funcType == 2) ? "  and a.RptTypeSeq in (1,2,3) and a.NeedAssessmenApproval = 2 " : "")
                    ////+ ((funcType == 3) ? "  and a.RptTypeSeq in (2,4,5) " : "")
                    //+ ((funcType == 4) ? "  and a.RptTypeSeq in (5,7) and a.ProposalAuditOpinion = 1 " : "")
                    //+ ((funcType == 5) ? "  and a.RptTypeSeq in (7,8) and a.ResolutionAuditOpinion = 1 " : "")
                    + subSQL

                + ((funcType == 11 || funcType == 12) ? "" : getAuthoritySql("a."))
                ;

            string sql2 = @"
                     SELECT Count(*) "
               
                  + @" FROM [dbo].[view_EngReportList] aa
                    inner join EngReportList a on a.Seq = aa.Seq
                    where aa.Seq in (" + sql + @") and a.RptName Like '%'+@keyWord+'%' ";

            SqlCommand cmd = db.GetCommand(sql2);
            cmd.Parameters.AddWithValue("@ExecUnitSeq", unitSeq);
            cmd.Parameters.AddWithValue("@keyWord", keyWord ?? "");
            cmd.Parameters.AddWithValue("@ExecSubUnitSeq", subUnitSeq);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@RelatedReportResultsAssignReviewUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@FacilityManagementAssignReviewUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@ProposalScopeLandAssignReviewUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@UserSeq", getUserSeq());
            return (int)db.ExecuteScalar(cmd);
        }

        //工程清單
        public List<T> GetEngList<T>(int year, int unitSeq, int subUnitSeq, int rptTypeSeq, int pageRecordCount, int pageIndex, int funcType, string keyWord = null)
        {
            string sql = @"";
            int user= new SessionManager().GetUser().Seq;

            string subSQL = "";
            switch (funcType)
            {
                //case 13:
                //    subSQL += " and isnull(a.ProposalAuditOpinion,0) = 1 ";
                //    break;
                case 14:
                    subSQL += " and isnull(a.ProposalAuditOpinion, 2) != 2 ";
                    break;
                //case 2:
                //    subSQL += " and a.RptTypeSeq in (1,2,3) and a.NeedAssessmenApproval = 2 ";
                //    break;
                //case 3:
                //    subSQL += " and a.RptTypeSeq in (2,4,5)  ";
                //    break;
                case 4:
                    subSQL += " and a.RptTypeSeq in (5,7) and a.ProposalAuditOpinion = 1 AND isnull(a.ReviewSort,0)>0 ";
                    break;
                case 5:
                    subSQL += " and a.RptTypeSeq in (7,8) and a.ResolutionAuditOpinion = 1 AND isnull(a.ReviewSort,0)>0 ";
                    break;
                case 11://待覆核或簽核清單
                    subSQL +=  " and a.RptTypeSeq in (1,3) ";
                    subSQL += "  and _erp.Seq = erp.Seq";
                    subSQL +=  " and ( (  a.RelatedReportResultsAssignReviewUserSeq = @RelatedReportResultsAssignReviewUserSeq ) ";
                    subSQL +=  " or    (  a.FacilityManagementAssignReviewUserSeq = @FacilityManagementAssignReviewUserSeq ) ";
                    subSQL +=  " or    (  a.ProposalScopeLandAssignReviewUserSeq = @ProposalScopeLandAssignReviewUserSeq ) ";
                    subSQL += @" or   ( a.Seq in (SELECT T1.EngReportSeq 
					                                FROM [dbo].[EngReportApprove] T1
						                                INNER JOIN (SELECT MIN(FN1.Seq) AS Seq FROM [dbo].[EngReportApprove] FN1 WHERE ISNULL(FN1.ApproveUserSeq,0)=0 GROUP BY FN1.EngReportSeq) T2 ON T1.Seq=T2.Seq
						                                INNER JOIN [dbo].[EngReportApprovePosition] T3 ON T1.EngReportSeq = T3.EngReportSeq AND T1.GroupId=T3.GroupId AND T1.ApprovalModuleListSeq =T3.ApprovalModuleListSeq 
						                                INNER JOIN [dbo].[UserUnitPosition] T4 ON (T3.SubUnitSeq = T4.UnitSeq  or T3.SubUnitSeq is null) AND T3.PositionSeq = T4.PositionSeq 
					                                WHERE T4.UserMainSeq = @UserSeq) ) ) ";
                    break;
                case 12://提案審查清單
                    subSQL += " and a.RptTypeSeq in (2) ";
                    subSQL += " and isnull(a.IsProposalReview,0) = 1 ";
                    subSQL += " and isnull(a.ProposalAuditOpinion,0) = 0 ";
                    break;
            }

            sql = @"
                    SELECT 						
		            _erp.EngReportSeq Seq
                    FROM [dbo].[EngReportList] a
                
                    " + (@"

                    left join EngReportApprove _erp  on (a.Seq = _erp.EngReportSeq)

                    left join
                    ( select
		                    erp.EngReportSeq , 
		                    Max(erp.GroupId)　maxGroup,
                            Min(erp.Seq) Seq
                    from EngReportApprove erp 
                    where erp.ApproveTime is null 
                    group by erp.EngReportSeq) erp 
                    on (_erp.EngReportSeq = erp.EngReportSeq and _erp.GroupId = erp.maxGroup)

                    ") + @"
                    where (a.ExecUnitSeq=@ExecUnitSeq  or  @ExecUnitSeq = -1)
                    "
                    + ((year == -1) ? "" : " and a.RptYear=" + year)
                    + ((subUnitSeq == -1) ? "" : " and a.ExecSubUnitSeq=" + subUnitSeq)
                    + ((rptTypeSeq == 0) ? "" : "  and a.RptTypeSeq=" + rptTypeSeq)

                    ////+ ((funcType == 2) ? "  and a.RptTypeSeq in (1,2,3) and a.NeedAssessmenApproval = 2 " : "")
                    ////+ ((funcType == 3) ? "  and a.RptTypeSeq in (2,4,5) " : "")
                    //+ ((funcType == 4) ? "  and a.RptTypeSeq in (5,7) and a.ProposalAuditOpinion = 1 " : "")
                    //+ ((funcType == 5) ? "  and a.RptTypeSeq in (7,8) and a.ResolutionAuditOpinion = 1 " : "")
                    + subSQL

                + ((funcType == 11|| funcType == 12) ? "" : getAuthoritySql("a."))
                ;

            string sql2 = @"
                SELECT aa.*, a.ProposalReviewEngReportSeq "
+                 ((funcType == 4) ? "  ,a.RefCarbonEmission " : "")
                  //+ ((funcType == 4) ? "  ,aa.ApprovedFund ,aa.ApprovedCarbonEmissions ,aa.Expenditure ,aa.Resolution ,aa.ResolutionAuditOpinion ,aa.ResolutionAuditOpinionName " : "")
                  + ((funcType == 14) ? "  ,a.EstimatedExpenditureCurrentYear, a.ExpensesSubsequentYears, a.BookingProcess_SY, a.BookingProcess_SM, a.BookingProcess_EY, a.BookingProcess_EM   " : "")
                  + ((funcType == 5) ? "  ,aa.EngNo ,aa.IsTransfer " : "")
                  //+ ((funcType == 9) ? "  ,aa.[OriginAndScope] ,aa.[RelatedReportResults] ,aa.[FacilityManagement] ,aa.[ProposalScopeLand] " : "")
                  //+ ((funcType == 9) ? "  ,aa.[EvaluationResult] ,aa.[ER1_1] ,aa.[ER1_2] ,aa.[ER2_1] ,aa.[ER2_2] ,aa.[ER3] ,aa.[ER4] ,aa.[ER6] " : "")
                  //+ ((funcType == 9) ? "  ,aa.[OriginAndScopeUpdateReviewUserName] ,aa.[OriginAndScopeReviewTime] " : "")
                  //+ ((funcType == 9) ? "  ,aa.[RelatedReportResultsUpdateReviewUserName] ,aa.[RelatedReportResultsReviewTime] " : "")
                  //+ ((funcType == 9) ? "  ,aa.[FacilityManagementUpdateReviewUserName] ,aa.[FacilityManagementReviewTime] " : "")
                  //+ ((funcType == 9) ? "  ,aa.[ProposalScopeLandUpdateReviewUserName] ,aa.[ProposalScopeLandReviewTime] " : "")
                  //+ ((funcType == 9) ? "  ,aa.[LocationMap] ,aa.[AerialPhotography] ,aa.[ScenePhoto] ,aa.[BaseMap] ,aa.[EngPlaneLayout] ,aa.[LongitudinalSection] ,aa.[StandardSection] " : "")
                  //+ ((funcType == 9) ? "  ,aa.[LocationMapFileName] ,aa.[AerialPhotographyFileName] ,aa.[ScenePhotoFileName] ,aa.[BaseMapFileName] ,aa.[EngPlaneLayoutFileName] ,aa.[LongitudinalSectionFileName] ,aa.[StandardSectionFileName] " : "")
                  + @" FROM [dbo].[view_EngReportList] aa
                    inner join EngReportList a on a.Seq = aa.Seq
                    where aa.Seq in (" + sql + @") and a.RptName Like '%'+@keyWord+'%' 
       
                    order by aa.Seq DESC
                    OFFSET @pageIndex ROWS
                    FETCH FIRST @pageRecordCount ROWS ONLY";

            SqlCommand cmd = db.GetCommand(sql2);
            cmd.Parameters.AddWithValue("@ExecUnitSeq", unitSeq);
            cmd.Parameters.AddWithValue("@keyWord", keyWord ?? "");
            cmd.Parameters.AddWithValue("@ExecSubUnitSeq", subUnitSeq);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@RelatedReportResultsAssignReviewUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@FacilityManagementAssignReviewUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@ProposalScopeLandAssignReviewUserSeq", getUserSeq());
            cmd.Parameters.AddWithValue("@UserSeq", getUserSeq());
            return db.GetDataTableWithClass<T>(cmd);
        }

        //工程清單-依Seq
        public List<T> GetEegReportBySeq<T>(int seq)
        {
            string sql = @"
                SELECT v.*, e.RefCarbonEmission _RefCarbonEmission
                FROM [dbo].[view_EngReportList] v
                inner join EngReportList e on v.Seq = e.Seq
                WHERE v.Seq = @Seq
            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //新增工程
        public int AddEngReport(EngReportVModel m)
        {
            Null2Empty(m);
            string unitSubSeq = "";
            string unitSeq = "";
            Utils.GetUserUnit(ref unitSeq, ref unitSubSeq);
            try
            {
                string sql = @"
                    DECLARE @EngReportSeq INT;
				    INSERT INTO EngReportList (
                        RptYear, RptName, RptTypeSeq, ExecUnitSeq, ExecSubUnitSeq,
                        CreateTime ,CreateUserSeq ,ModifyTime ,ModifyUserSeq
                    )values(
                        @RptYear, @RptName, 1, @ExecUnitSeq, @ExecSubUnitSeq,
                        GETDATE() ,@CreateUserSeq ,GETDATE() ,@ModifyUserSeq
                    );
                    SET @EngReportSeq = @@IDENTITY;

                    SELECT @EngReportSeq AS EngReportSeq;                  
                    ";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@RptYear", m.RptYear);
                cmd.Parameters.AddWithValue("@RptName", m.RptName);
                cmd.Parameters.AddWithValue("@ExecUnitSeq", m.ExecUnitSeq);
                cmd.Parameters.AddWithValue("@ExecSubUnitSeq", m.ExecSubUnitSeq );
                cmd.Parameters.AddWithValue("@CreateUserSeq", m.CreateUserSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                //db.ExecuteNonQuery(cmd);
                int engReportSeq = (int)db.ExecuteScalar(cmd);
                m.Seq = engReportSeq;
                AddEngReportApprove(m);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.AddEngReport " + e.Message);
                return -1;
            }
        }

        //更新工程
        public int UpdateEngReport(EngReportVModel m)
        {
            Null2Empty(m);
            string sql = @"
            update EngReportList set 
                RptYear = @RptYear,
                RptName = @RptName,
                ModifyTime = GetDate(),
                ModifyUserSeq = @ModifyUserSeq
            where Seq=@Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@RptYear", m.RptYear);
                cmd.Parameters.AddWithValue("@RptName", m.RptName);

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.UpdateEngReport: " + e.Message);
                return -1;
            }
        }

        //更新工程 FOR 需求評估填報
        public int UpdateEngReportForNA(EngReportVModel m, int naType)
        {
            Null2Empty(m);
            var usrInfo = Utils.getUserInfo();
            string strSubSQL = "";
            switch (naType) 
            {
                case 1:
                    strSubSQL += ", OriginAndScope = '" + m.OriginAndScope + "'";
                    strSubSQL += ", OriginAndScopeUserSeq = " + getUserSeq();
                    strSubSQL += ", OriginAndScopeUpdateTime = GETDATE()";
                    strSubSQL += ", OriginAndScopeReviewState = 1";
                    strSubSQL += ", OriginAndScopeAssignReviewUserSeq = " + getUserSeq();
                    strSubSQL += ", OriginAndScopeUpdateReviewUserSeq = " + getUserSeq();
                    strSubSQL += ", OriginAndScopeReviewTime = GETDATE()";
                    break;
                case 2: 
                    strSubSQL += ", RelatedReportResults='" + m.RelatedReportResults + "'";
                    if (Utils.getUserInfo().IsAdmin || m.CreateUserSeq == getUserSeq() || usrInfo.RoleSeq <= 3) 
                    {
                        strSubSQL += ", RelatedReportResultsUserSeq=" + getUserSeq();
                        strSubSQL += ", RelatedReportResultsUpdateTime=GETDATE()";
                        if (!m.RelatedReportResultsReviewState)
                        {
                            //免覆核且未更新覆核資料
                            strSubSQL += ", RelatedReportResultsReviewState = 0";
                            strSubSQL += ", RelatedReportResultsAssignReviewUserSeq = 0";
                            //strSubSQL += ", RelatedReportResultsAssignReviewUserSeq = " + getUserSeq();
                            strSubSQL += ", RelatedReportResultsUpdateReviewUserSeq = " + getUserSeq();
                            strSubSQL += ", RelatedReportResultsReviewTime = GETDATE()";
                        }
                        else
                        {
                            //指定覆核
                            //if (m.RelatedReportResultsAssignReviewUserSeq.HasValue && !m.RelatedReportResultsUpdateReviewUserSeq.HasValue)
                            //{
                                strSubSQL += ", RelatedReportResultsReviewState = 1";
                                strSubSQL += ", RelatedReportResultsAssignReviewUserSeq = ";
                                strSubSQL += m.RelatedReportResultsAssignReviewUserSeq == null ? 0 : m.RelatedReportResultsAssignReviewUserSeq;
                                //strSubSQL += ", RelatedReportResultsUpdateReviewUserSeq = null";
                                //strSubSQL += ", RelatedReportResultsReviewTime = null";
                            //}
                        }
                    }
                    else if (m.RelatedReportResultsAssignReviewUserSeq == getUserSeq())
                    {
                        //strSubSQL += ", RelatedReportResultsReviewState = 1";
                        strSubSQL += ", RelatedReportResultsUpdateReviewUserSeq = " + getUserSeq();
                        strSubSQL += ", RelatedReportResultsReviewTime = GETDATE()";
                    }
                    break;
                case 3:
                    strSubSQL += ", FacilityManagement='" + m.FacilityManagement + "'";
                    if (Utils.getUserInfo().IsAdmin || m.CreateUserSeq == getUserSeq() || usrInfo.RoleSeq <= 3)
                    {
                        strSubSQL += ", FacilityManagementUserSeq=" + getUserSeq();
                        strSubSQL += ", FacilityManagementUpdateTime=GETDATE()";
                        if (!m.FacilityManagementReviewState)
                        {
                            //免覆核且未更新覆核資料
                         
                            strSubSQL += ", FacilityManagementReviewState = 0";
                            strSubSQL += ", FacilityManagementAssignReviewUserSeq = 0";
                            //strSubSQL += ", FacilityManagementAssignReviewUserSeq = " + getUserSeq();
                            strSubSQL += ", FacilityManagementUpdateReviewUserSeq = " + getUserSeq();
                            strSubSQL += ", FacilityManagementReviewTime = GETDATE()";
                            
                        }
                        else
                        {
                            //指定覆核
                            //if (m.FacilityManagementAssignReviewUserSeq.HasValue && !m.FacilityManagementUpdateReviewUserSeq.HasValue)
                            //{
                                strSubSQL += ", FacilityManagementReviewState = 1";
                                strSubSQL += ", FacilityManagementAssignReviewUserSeq = ";
                                strSubSQL += m.FacilityManagementAssignReviewUserSeq == null ? 0 : m.FacilityManagementAssignReviewUserSeq;
                                //strSubSQL += ", FacilityManagementUpdateReviewUserSeq = null";
                                //strSubSQL += ", FacilityManagementReviewTime = null";
                            //}
                        }
                    }
                    else if (m.FacilityManagementAssignReviewUserSeq == getUserSeq())
                    {
                        //strSubSQL += ", FacilityManagementReviewState = 1";
                        strSubSQL += ", FacilityManagementUpdateReviewUserSeq = " + getUserSeq();
                        strSubSQL += ", FacilityManagementReviewTime = GETDATE()";
                    }
                    break;
                case 4:
                    strSubSQL += ", ProposalScopeLand='" + m.ProposalScopeLand + "'";
                    if (Utils.getUserInfo().IsAdmin || m.CreateUserSeq == getUserSeq() || usrInfo.RoleSeq <= 3)
                    {
                        strSubSQL += ", ProposalScopeLandUserSeq=" + getUserSeq();
                        strSubSQL += ", ProposalScopeLandUpdateTime=GETDATE()";
                        //strSubSQL += ", ProposalScopeLandReviewState="+ m.ProposalScopeLandReviewState;

                        if (!m.ProposalScopeLandReviewState) 
                        {
                            //免覆核且未更新覆核資料
                            strSubSQL += ", ProposalScopeLandReviewState = 0";
                            strSubSQL += ", ProposalScopeLandAssignReviewUserSeq = 0";
                            //strSubSQL += ", ProposalScopeLandAssignReviewUserSeq = " + getUserSeq();
                            strSubSQL += ", ProposalScopeLandUpdateReviewUserSeq = " + getUserSeq();
                            strSubSQL += ", ProposalScopeLandReviewTime = GETDATE()";
                        }
                        else 
                        {
                            //指定覆核
                            //if (m.ProposalScopeLandAssignReviewUserSeq.HasValue && !m.ProposalScopeLandUpdateReviewUserSeq.HasValue)
                            //{
                                strSubSQL += ", ProposalScopeLandReviewState = 1";
                                strSubSQL += ", ProposalScopeLandAssignReviewUserSeq = ";
                                strSubSQL += m.ProposalScopeLandAssignReviewUserSeq == null ? 0 : m.ProposalScopeLandAssignReviewUserSeq;
                                //strSubSQL += ", ProposalScopeLandUpdateReviewUserSeq = null";
                                //strSubSQL += ", ProposalScopeLandReviewTime = null";
                            //}
                        }
                    }
                    else if (m.ProposalScopeLandAssignReviewUserSeq == getUserSeq())
                    {
                        //strSubSQL += ", ProposalScopeLandReviewState = 1";
                        strSubSQL += ", ProposalScopeLandUpdateReviewUserSeq = " + getUserSeq();
                        strSubSQL += ", ProposalScopeLandReviewTime = GETDATE()";
                    }
                    break;
                case 5:
                    strSubSQL += ", RptName = '" + m.RptName + "'";
                    break;
                case 6:
                    strSubSQL += ", RptYear = '" + m.RptYear + "'";
                    break;
            }

            string sql = @"
            update EngReportList set 
                ModifyTime = GetDate(),
                ModifyUserSeq = @ModifyUserSeq"
                + strSubSQL
                + " where Seq=@Seq;";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.UpdateEngReportForNA: " + e.Message);
                return -1;
            }
        }

        //更新工程 FOR 需求評估填報 - 送簽或簽核
        public int UpdateEngReportForNAApproval(EngReportVModel m)
        {
            Null2Empty(m);
            string sql = @"
            DECLARE @IsFinish INT = ISNULL((SELECT COUNT(*) AS Seq FROM [dbo].[EngReportApprove] WHERE ISNULL([ApproveTime],'')='' AND [EngReportSeq] = @Seq ),0)

            update EngReportList set 
                ModifyTime = GetDate()
                ,ModifyUserSeq = @ModifyUserSeq
                ,NeedAssessmenApproval = CASE WHEN @IsFinish = 0 THEN 2 ELSE NeedAssessmenApproval END
            where Seq = @Seq; ";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.UpdateEngReportForNAApproval: " + e.Message);
                return -1;
            }
        }

        //更新工程-上傳附件 Upload Attachment = UA
        public int UpdateEngReportForUA(EngReportVModel m, string fileType)
        {
            Null2Empty(m);
            string sql = @"
            update EngReportList set 
                ModifyTime = GetDate(),
                ModifyUserSeq = @ModifyUserSeq"
                + ((fileType == "A1") ? " , LocationMap='" + m.LocationMap + "'" : "")
                + ((fileType == "A2") ? " , AerialPhotography='" + m.AerialPhotography + "'" : "")
                + ((fileType == "A3") ? " , ScenePhoto='" + m.ScenePhoto + "'" : "")
                + ((fileType == "A4") ? " , BaseMap='" + m.BaseMap + "'" : "")
                + ((fileType == "A5") ? " , EngPlaneLayout='" + m.EngPlaneLayout + "'" : "")
                + ((fileType == "A6") ? " , LongitudinalSection='" + m.LongitudinalSection + "'" : "")
                + ((fileType == "A7") ? " , StandardSection='" + m.StandardSection + "'" : "")

                + ((fileType == "D1") ? " , EcologicalConservationD01='" + m.EcologicalConservationD01 + "'" : "")
                + ((fileType == "D2") ? " , EcologicalConservationD02='" + m.EcologicalConservationD02 + "'" : "")
                + ((fileType == "D3") ? " , EcologicalConservationD03='" + m.EcologicalConservationD03 + "'" : "")
                + ((fileType == "D4") ? " , EcologicalConservationD04='" + m.EcologicalConservationD04 + "'" : "")
                + ((fileType == "D5") ? " , EcologicalConservationD05='" + m.EcologicalConservationD05 + "'" : "")
                + ((fileType == "D6") ? " , EcologicalConservationD06='" + m.EcologicalConservationD06 + "'" : "")
            + " where Seq=@Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.UpdateEngReportForUA: " + e.Message);
                return -1;
            }
        }

        //更新工程-刪除上傳附件Delete Attachment = DA
        public int UpdateEngReportForDA(EngReportVModel m, string fileType)
        {
            Null2Empty(m);
            string sql = @"
            update EngReportList set 
                ModifyTime = GetDate(),
                ModifyUserSeq = @ModifyUserSeq"
                + ((fileType == "A1") ? " , LocationMap=''" : "")
                + ((fileType == "A2") ? " , AerialPhotography=''" : "")
                + ((fileType == "A3") ? " , ScenePhoto=''" : "")
                + ((fileType == "A4") ? " , BaseMap=''" : "")
                + ((fileType == "A5") ? " , EngPlaneLayout=''" : "")
                + ((fileType == "A6") ? " , LongitudinalSection=''" : "")
                + ((fileType == "A7") ? " , StandardSection=''" : "")
                + ((fileType == "D1") ? " , EcologicalConservationD01=''" : "")
                + ((fileType == "D2") ? " , EcologicalConservationD02=''" : "")
                + ((fileType == "D3") ? " , EcologicalConservationD03=''" : "")
                + ((fileType == "D4") ? " , EcologicalConservationD04=''" : "")
                + ((fileType == "D5") ? " , EcologicalConservationD05=''" : "")
                + ((fileType == "D6") ? " , EcologicalConservationD06=''" : "")
            + " where Seq=@Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("EngReportService.UpdateEngReportForDA: " + e.Message);
                return -1;
            }
        }

        //更新工程 FOR 提案工作會議
        public int UpdateEngReportForPW(EngReportVModel m)
        {
            Null2Empty(m);
            string unitSubSeq = "";
            string unitSeq = "";
            Utils.GetUserUnit(ref unitSeq, ref unitSubSeq);
            string sql = @"
            update EngReportList set 
                ModifyTime = GetDate()
                ,ModifyUserSeq = @ModifyUserSeq
                ,RptTypeSeq = @RptTypeSeq
                ,NeedAssessmenApproval = CASE WHEN @EvaluationResult > 2 THEN 0 ELSE NeedAssessmenApproval END
                ,EvaluationResult = @EvaluationResult
                ,ER1_1 = @ER1_1
                ,ER1_2 = @ER1_2
                ,ER2_1 = @ER2_1
                ,ER2_2 = @ER2_2
                ,ER3 = @ER3
                ,ER4 = @ER4
                ,ER6 = @ER6 
            where Seq = @Seq;
            ";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@RptTypeSeq", m.RptTypeSeq);
                cmd.Parameters.AddWithValue("@EvaluationResult", m.EvaluationResult);
                cmd.Parameters.AddWithValue("@ER1_1", m.ER1_1);
                cmd.Parameters.AddWithValue("@ER1_2", m.ER1_2);
                cmd.Parameters.AddWithValue("@ER2_1", m.ER2_1);
                cmd.Parameters.AddWithValue("@ER2_2", m.ER2_2);
                cmd.Parameters.AddWithValue("@ER3", m.ER3);
                cmd.Parameters.AddWithValue("@ER4", m.ER4);
                cmd.Parameters.AddWithValue("@ER6", m.ER6);
                cmd.Parameters.AddWithValue("@ExecUnitSeq", unitSeq);
                cmd.Parameters.AddWithValue("@ExecSubUnitSeq", unitSubSeq);
                cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                if(m.EvaluationResult>2)
                    AddEngReportApprove(m);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.UpdateEngReportForPW: " + e.Message);
                return -1;
            }
        }

        //更新工程 FOR 提案審查
        public int UpdateEngReportForPR(EngReportVModel m)
        {
            Null2Empty(m);
            string RiverSeq = "";
            string DrainSeq = "";
            if (m.RiverSeq3.HasValue) RiverSeq = ",[RiverSeq] = @RiverSeq";
            if (m.DrainSeq.HasValue) DrainSeq = ",[DrainSeq] = @DrainSeq";

            string sql = @"
            update EngReportList set 
                ModifyTime = GetDate()
                ,ModifyUserSeq = @ModifyUserSeq
                ,[RefCarbonEmission] = @RefCarbonEmission
                ,[RptTypeSeq] = @RptTypeSeq
                ,[RptName] = @RptName
                ,[ProposalReviewTypeSeq] = @ProposalReviewTypeSeq
                ,[ProposalReviewAttributesSeq] = @ProposalReviewAttributesSeq
                " + RiverSeq + @"
                " + DrainSeq + @"
                ,[Coastal] = @Coastal
                ,[LargeSectionChainage] = @LargeSectionChainage
                ,[CitySeq] = @CitySeq
                ,[TownSeq] = @TownSeq
                ,[CoordX] = @CoordX
                ,[CoordY] = @CoordY
                ,[EngineeringScale] = @EngineeringScale
                ,[ProcessReason] = @ProcessReason
                ,[EngineeringScaleMemo] = @EngineeringScaleMemo
                ,[RelatedReportContent] = @RelatedReportContent
                ,[HistoricalCatastrophe] = @HistoricalCatastrophe
                ,[HistoricalCatastropheMemo] = @HistoricalCatastropheMemo
                ,[ProtectionTarget] = @ProtectionTarget
                ,[SetConditions] = @SetConditions
                ,[DemandCarbonEmissions] = @DemandCarbonEmissions
                ,[DemandCarbonEmissionsMemo] = @DemandCarbonEmissionsMemo
                ,[IsProposalReview] = @IsProposalReview
                --,[ReviewSort] = @ReviewSort
                --,[ApprovedFund] = @ApprovedFund
                --,[ApprovedCarbonEmissions] = @ApprovedCarbonEmissions
                --,[Expenditure] = @Expenditure
                --,[Resolution] = @Resolution
                --,[ResolutionAuditOpinion] = @ResolutionAuditOpinion
                ,[EstimatedLandAcquisitionCosts] = @EstimatedLandAcquisitionCosts
                ,[ManagementPlanningLayoutSituation] = @ManagementPlanningLayoutSituation
                ,[IsFloodControlRecords] = @IsFloodControlRecords
            WHERE Seq=@Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@RptTypeSeq", m.RptTypeSeq);
                cmd.Parameters.AddWithValue("@RptName", m.RptName);
                cmd.Parameters.AddWithValue("@ProposalReviewTypeSeq", m.ProposalReviewTypeSeq.HasValue ? m.ProposalReviewTypeSeq : 0);
                cmd.Parameters.AddWithValue("@ProposalReviewAttributesSeq", m.ProposalReviewAttributesSeq.HasValue ? m.ProposalReviewAttributesSeq : 0);
                if (m.RiverSeq3.HasValue)
                    cmd.Parameters.AddWithValue("@RiverSeq", m.RiverSeq3);
                if(m.DrainSeq.HasValue)
                    cmd.Parameters.AddWithValue("@DrainSeq", m.DrainSeq);
                cmd.Parameters.AddWithValue("@Coastal", String.IsNullOrEmpty(m.Coastal)?"":m.Coastal);
                cmd.Parameters.AddWithValue("@LargeSectionChainage", m.LargeSectionChainage);
                cmd.Parameters.AddWithValue("@CitySeq", m.CitySeq.HasValue ? m.CitySeq : 0);
                cmd.Parameters.AddWithValue("@TownSeq", m.TownSeq.HasValue ? m.TownSeq : 0);
                cmd.Parameters.AddWithValue("@CoordX", m.CoordX.HasValue ? m.CoordX : 0);
                cmd.Parameters.AddWithValue("@CoordY", m.CoordY.HasValue ? m.CoordY : 0);
                cmd.Parameters.AddWithValue("@EngineeringScale", m.EngineeringScale);
                cmd.Parameters.AddWithValue("@ProcessReason", m.ProcessReason);
                cmd.Parameters.AddWithValue("@EngineeringScaleMemo", m.EngineeringScaleMemo);
                cmd.Parameters.AddWithValue("@RelatedReportContent", m.RelatedReportContent);
                cmd.Parameters.AddWithValue("@HistoricalCatastrophe", m.HistoricalCatastrophe);
                cmd.Parameters.AddWithValue("@HistoricalCatastropheMemo", Utils.NulltoDBNull(m.HistoricalCatastropheMemo) );
                cmd.Parameters.AddWithValue("@ProtectionTarget", m.ProtectionTarget);
                cmd.Parameters.AddWithValue("@SetConditions", m.SetConditions);
                cmd.Parameters.AddWithValue("@DemandCarbonEmissions", m.DemandCarbonEmissions.HasValue ? m.DemandCarbonEmissions : 0);
                cmd.Parameters.AddWithValue("@DemandCarbonEmissionsMemo", m.DemandCarbonEmissionsMemo);
                cmd.Parameters.AddWithValue("@IsProposalReview", m.IsProposalReview);
                //cmd.Parameters.AddWithValue("@ProposalAuditOpinion", m.ProposalAuditOpinion);
                //cmd.Parameters.AddWithValue("@ReviewSort", m.ReviewSort);
                //cmd.Parameters.AddWithValue("@ApprovedFund", m.ApprovedFund);
                //cmd.Parameters.AddWithValue("@ApprovedCarbonEmissions", m.ApprovedCarbonEmissions);
                //cmd.Parameters.AddWithValue("@Expenditure", m.Expenditure);
                //cmd.Parameters.AddWithValue("@Resolution", m.Resolution);
                //cmd.Parameters.AddWithValue("@ResolutionAuditOpinion", m.ResolutionAuditOpinion);
                cmd.Parameters.AddWithValue("@EstimatedLandAcquisitionCosts ", Utils.NulltoDBNull( m.EstimatedLandAcquisitionCosts));
                cmd.Parameters.AddWithValue("@ManagementPlanningLayoutSituation", m.ManagementPlanningLayoutSituation);
                cmd.Parameters.AddWithValue("@IsFloodControlRecords", m.IsFloodControlRecords.HasValue? m.IsFloodControlRecords:0);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@RefCarbonEmission", m.RefCarbonEmission);
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.UpdateEngReportForPR: " + e.Message);
                return -1;
            }
        }

        //更新工程 FOR 提案審查
        public int UpdateEngReportForPR1(EngReportVModel m)
        {
            Null2Empty(m);
            string sql = @"
            update EngReportList set 
                ModifyTime = GetDate()
                ,ModifyUserSeq = @ModifyUserSeq
                ,[EstimatedExpenditureCurrentYear] = @EstimatedExpenditureCurrentYear
                ,[ExpensesSubsequentYears] = @ExpensesSubsequentYears
                ,[BookingProcess_SY] = @BookingProcess_SY
                ,[BookingProcess_SM] = @BookingProcess_SM
                ,[BookingProcess_EY] = @BookingProcess_EY
                ,[BookingProcess_EM] = @BookingProcess_EM
                ,[Remark] = @Remark
            WHERE Seq=@Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@EstimatedExpenditureCurrentYear ", m.EstimatedExpenditureCurrentYear);
                cmd.Parameters.AddWithValue("@ExpensesSubsequentYears", m.ExpensesSubsequentYears);
                cmd.Parameters.AddWithValue("@BookingProcess_SY", m.BookingProcess_SY);
                cmd.Parameters.AddWithValue("@BookingProcess_SM", m.BookingProcess_SM);
                cmd.Parameters.AddWithValue("@BookingProcess_EY", m.BookingProcess_EY);
                cmd.Parameters.AddWithValue("@BookingProcess_EM", m.BookingProcess_EM);
                cmd.Parameters.AddWithValue("@Remark", m.Remark);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.UpdateEngReportForPR1: " + e.Message);
                return -1;
            }
        }

        //更新工程 FOR 提案審查 - 順序
        public int UpdateEngReportForReviewSort(EngReportVModel m)
        {
            Null2Empty(m);
            string sql = @"
            update EngReportList set 
                ModifyTime = GetDate()
                ,ModifyUserSeq = @ModifyUserSeq
                ,[ReviewSort] = @ReviewSort
            WHERE Seq=@Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@ReviewSort", m.ReviewSort);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.UpdateEngReportForReviewSort: " + e.Message);
                return -1;
            }
        }

        //更新工程 FOR 提案審查
        public int UpdateEngReportForPRProposalAudit(EngReportVModel m)
        {
            Null2Empty(m);
            string sql = @"
            update EngReportList set 
                ModifyTime = GetDate()
                ,ModifyUserSeq = @ModifyUserSeq

                ,[ProposalAuditOpinion] = @ProposalAuditOpinion
                ,[RptTypeSeq] = CASE WHEN @ProposalAuditOpinion = 1 THEN 5 ELSE 6 END
            WHERE Seq=@Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@ProposalAuditOpinion", m.ProposalAuditOpinion);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.UpdateEngReportForPRProposalAudit: " + e.Message);
                return -1;
            }
        }

        //更新工程 FOR 年度經費檢討會議
        public EngReportVModel UpdateEngReportForAF(EngReportVModel m)
        {
            Null2Empty(m);

            EngReportVModel.GetRefCarbonEmission(m);

            string sql = @"
            UPDATE EngReportList set 
                ModifyTime = GetDate()
                ,ModifyUserSeq = @ModifyUserSeq
                ,[RefCarbonEmission] = @RefCarbonEmission
                ,[ApprovedFund] = @ApprovedFund
                ,[ApprovedCarbonEmissions] = @ApprovedCarbonEmissions
                ,[Expenditure] = @Expenditure
                ,[Resolution] = @Resolution "
                + ((m.ResolutionAuditOpinion != null) ? "  ,[ResolutionAuditOpinion] = @ResolutionAuditOpinion " : "")
                + ((m.ResolutionAuditOpinion != null) ? "  ,[RptTypeSeq] = CASE WHEN @ResolutionAuditOpinion = 1 THEN 7 ELSE [RptTypeSeq] END " : "")
            + @" WHERE Seq=@Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@ApprovedFund", m.ApprovedFund *1000);
                cmd.Parameters.AddWithValue("@RefCarbonEmission", decimal.Round(m.RefCarbonEmission.Value, 4));
                cmd.Parameters.AddWithValue("@ApprovedCarbonEmissions", m.ApprovedCarbonEmissions);
                cmd.Parameters.AddWithValue("@Expenditure", m.Expenditure*1000);
                cmd.Parameters.AddWithValue("@Resolution", m.Resolution);
                if (m.ResolutionAuditOpinion != null)
                    cmd.Parameters.AddWithValue("@ResolutionAuditOpinion", m.ResolutionAuditOpinion);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                if (db.ExecuteNonQuery(cmd) > 0)
                    return m;
                else return null;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.UpdateEngReportForAF: " + e.Message);
                return null;
            }
        }

        //更新工程 FOR 轉入核定案件
        public int UpdateEngReportForAC(EngReportVModel m)
        {
            Null2Empty(m);
            string sql = @"
            update EngReportList set 
                ModifyTime = GetDate()
                ,ModifyUserSeq = @ModifyUserSeq

                ,[EngNo] = @EngNo
            WHERE Seq=@Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@EngNo", m.EngNo);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.UpdateEngReportForPRProposalAudit: " + e.Message);
                return -1;
            }
        }

        //更新工程 FOR 轉入核定案件 - 轉入建立案件
        public int UpdateEngReportForACT(int Seq)
        {
            string sql = @"
            update EngReportList set 
                ModifyTime = GetDate()
                ,ModifyUserSeq = @ModifyUserSeq

                ,[IsTransfer] = 1
                ,[RptTypeSeq] = 8
            WHERE [Seq] = @Seq and [RptTypeSeq] = 7";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", Seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                using(var context = new EQC_NEW_Entities())
                {
                    var target = context.EngReportList.Find(Seq);
                    if(target.RptTypeSeq == 8 )
                    {
                        context.EngApprovalImport.Add(new EngApprovalImport
                        {
                            EngYear = target.RptYear,
                            EngNo = target.EngNo,
                            EngName = target.RptName,
                            TotalBudget = target.ApprovedFund,
                            ApprovedCarbonQuantity = (int) target.ApprovedCarbonEmissions*1000,
                            CarbonDemandQuantity = (int)target.DemandCarbonEmissions*1000,
                            SubContractingBudget = 0,
                            CreateTime = DateTime.Now,
                            ModifyTime = DateTime.Now

                        });
                        context.SaveChanges();
                    }

                }

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.UpdateEngReportForACT: " + e.Message);
                return -1;
            }
        }


        //刪除工程(全部關聯)
        public int DelEngReportAll(int Seq)
        {
            try
            {
                string sql = @"
                    delete from EngReportApprove where
                    EngReportSeq = @Seq;
                    delete from EngReportEstimatedCost where
                    EngReportSeq = @Seq;
                    delete from EngReportLocalCommunication where
                    EngReportSeq = @Seq;
                    delete from EngReportMainJobDescription where
                    EngReportSeq = @Seq;
                    delete from EngReportOnSiteConsultation where
                    EngReportSeq = @Seq;

                    delete from EngReportList where Seq = @Seq;";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", Seq);
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                //db.TransactionRollback();
                log.Info("EngReportService.DelEngReport" + e.Message);
                return -1;
            }
        }
        //刪除工程
        public int DelEngReport(int Seq)
        {
            try
            {
                string sql = @"
                    --刪除簽核流程
                    DELETE FROM [dbo].[EngReportApprove] WHERE [EngReportSeq] = @Seq;
                    DELETE FROM [dbo].[EngReportList] where Seq = @Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", Seq);
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                //db.TransactionRollback();
                log.Info("EngReportService.DelEngReport" + e.Message);
                return -1;
            }
        }

        //下載
        public List<T> GetDownloadFile<T>(int seq,string fileNo)
        {
            string col = "";
            switch (fileNo) 
            {
                case "A1": col = ",LocationMap as FileName"; break;
                case "A2": col = ",AerialPhotography as FileName"; break;
                case "A3": col = ",ScenePhoto as FileName"; break;
                case "A4": col = ",BaseMap as FileName"; break;
                case "A5": col = ",EngPlaneLayout as FileName"; break;
                case "A6": col = ",LongitudinalSection as FileName"; break;
                case "A7": col = ",StandardSection as FileName"; break;
                case "D1": col = ",EcologicalConservationD01 as FileName"; break;
                case "D2": col = ",EcologicalConservationD02 as FileName"; break;
                case "D3": col = ",EcologicalConservationD03 as FileName"; break;
                case "D4": col = ",EcologicalConservationD04 as FileName"; break;
                case "D5": col = ",EcologicalConservationD05 as FileName"; break;
                case "D6": col = ",EcologicalConservationD06 as FileName"; break;
                default: col = ""; break;
            }
            string sql = @"
                select
                    Seq " + col + @"
                from EngReportList
                where Seq=@Seq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        #region 提案審查-概估經費

        //提案審查-概估經費清單
        public List<T> GetEngReportEstimatedCostList<T>(int EngReportSeq)
        {
            string sql = @"
SELECT FN1.[Seq]
      ,FN1.[EngReportSeq]
      ,FN1.[Year]
      ,FN1.[AttributesSeq]
      ,FN1.[Price]
      ,FN1.[CreateTime]
      ,FN1.[CreateUserSeq]
      ,FN1.[ModifyTime]
      ,FN1.[ModifyUserSeq]
	  ,ISNULL(FN2.AttributesName,'') AS AttributesName
	  ,ISNULL(FN3.DisplayName,'') AS CreateUser
	  ,ISNULL(FN4.DisplayName,'') AS ModifyUser
FROM [dbo].[EngReportEstimatedCost] FN1
	LEFT OUTER JOIN [dbo].[ProposalReviewAttributes] FN2 ON FN2.Seq = FN1.[AttributesSeq]
	LEFT OUTER JOIN [dbo].[UserMain] FN3 ON FN3.Seq = FN1.[CreateUserSeq]
	LEFT OUTER JOIN [dbo].[UserMain] FN4 ON FN4.Seq = FN1.[ModifyUserSeq]
WHERE FN1.[EngReportSeq] = @EngReportSeq";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngReportSeq", EngReportSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //提案審查-概估經費-依Seq
        public List<T> GetEngReportEstimatedCostBySeq<T>(int Seq)
        {
            string sql = @"
SELECT FN1.[Seq]
      ,FN1.[EngReportSeq]
      ,FN1.[Year]
      ,FN1.[AttributesSeq]
      ,FN1.[Price]
      ,FN1.[CreateTime]
      ,FN1.[CreateUserSeq]
      ,FN1.[ModifyTime]
      ,FN1.[ModifyUserSeq]
	  ,ISNULL(FN2.AttributesName,'') AS AttributesName
	  ,ISNULL(FN3.DisplayName,'') AS CreateUser
	  ,ISNULL(FN4.DisplayName,'') AS ModifyUser
FROM [dbo].[EngReportEstimatedCost] FN1
	LEFT OUTER JOIN [dbo].[ProposalReviewAttributes] FN2 ON FN2.Seq = FN1.[AttributesSeq]
	LEFT OUTER JOIN [dbo].[UserMain] FN3 ON FN3.Seq = FN1.[CreateUserSeq]
	LEFT OUTER JOIN [dbo].[UserMain] FN4 ON FN4.Seq = FN1.[ModifyUserSeq]
WHERE FN1.[Seq] = @Seq";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", Seq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //提案審查-概估經費-依EngReportSeq及RptYear
        public List<T> GetEngReportEstimatedCost<T>(int EngReportSeq,int RptYear)
        {
            string sql = @"
SELECT FN1.[EngReportSeq]
      ,FN1.[Year]
      ,SUM(FN1.[Price]) AS [Price]
FROM [dbo].[EngReportEstimatedCost] FN1
	LEFT OUTER JOIN [dbo].[ProposalReviewAttributes] FN2 ON FN2.Seq = FN1.[AttributesSeq]
WHERE FN1.[EngReportSeq] = @EngReportSeq AND FN1.[Year] = @RptYear 
GROUP BY FN1.[EngReportSeq],FN1.[Year] ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngReportSeq", EngReportSeq);
            cmd.Parameters.AddWithValue("@RptYear", RptYear);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //新增 提案審查-概估經費
        public int AddEngReportEstimatedCost(EngReportEstimatedCostVModel m)
        {
            Null2Empty(m);
            try
            {
                string sql = @"
INSERT INTO [dbo].[EngReportEstimatedCost]
           ([EngReportSeq],[Year],[AttributesSeq],[Price]
           ,[CreateTime],[CreateUserSeq],[ModifyTime],[ModifyUserSeq])
     VALUES
           (@EngReportSeq,@Year,@AttributesSeq,@Price
           ,GETDATE(),@CreateUserSeq,GETDATE(),@ModifyUserSeq)";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngReportSeq", m.EngReportSeq);
                cmd.Parameters.AddWithValue("@Year", m.Year);
                cmd.Parameters.AddWithValue("@AttributesSeq", m.AttributesSeq);
                cmd.Parameters.AddWithValue("@Price", m.Price);
                cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.AddEngReportEstimatedCost " + e.Message);
                return -1;
            }
        }

        //更新 提案審查-概估經費
        public int UpdateEngReportEstimatedCost(EngReportEstimatedCostVModel m)
        {
            Null2Empty(m);
            string sql = @"
UPDATE [dbo].[EngReportEstimatedCost]
   SET [Year] = @Year
      ,[AttributesSeq] = @AttributesSeq
      ,[Price] = @Price
      ,[ModifyTime] = GetDate()
      ,[ModifyUserSeq] = @ModifyUserSeq
 WHERE [Seq] = @Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@Year", m.Year);
                cmd.Parameters.AddWithValue("@AttributesSeq", m.AttributesSeq);
                cmd.Parameters.AddWithValue("@Price", m.Price);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.UpdateEngReportEstimatedCost: " + e.Message);
                return -1;
            }
        }

        //刪除 提案審查-概估經費
        public int DelEngReportEstimatedCost(int Seq)
        {
            try
            {
                string sql = @"DELETE FROM [dbo].[EngReportEstimatedCost] WHERE [Seq] = @Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", Seq);
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                //db.TransactionRollback();
                log.Info("EngReportService.DelEngReportEstimatedCost" + e.Message);
                return -1;
            }
        }

        #endregion 

        #region 提案審查-在地溝通辦理情形

        //提案審查-在地溝通辦理情形清單
        public List<T> GetEngReportLocalCommunicationList<T>(int Seq)
        {
            int userSeq = new SessionManager().GetUser().Seq;

            string sql = @"
SELECT FN1.[Seq]
      ,FN1.[EngReportSeq]
      ,FN1.[Date]
      ,FN1.[FileNumber]
      ,FN1.[FilePath]
      ,FN1.[CreateTime]
      ,FN1.[CreateUserSeq]
      ,FN1.[ModifyTime]
      ,FN1.[ModifyUserSeq]
	  ,ISNULL(FN2.DisplayName,'') AS CreateUser
	  ,ISNULL(FN3.DisplayName,'') AS ModifyUser
      ,dbo.DelGuidStr(FN1.[FilePath]) FileName
FROM [dbo].[EngReportLocalCommunication] FN1
	LEFT OUTER JOIN [dbo].[UserMain] FN2 ON FN2.Seq = FN1.[CreateUserSeq]
	LEFT OUTER JOIN [dbo].[UserMain] FN3 ON FN3.Seq = FN1.[ModifyUserSeq]
WHERE FN1.[EngReportSeq] = @Seq";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", Seq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //提案審查-在地溝通辦理情形
        public List<T> GetEngReportLocalCommunicationBySeq<T>(int Seq)
        {
            int userSeq = new SessionManager().GetUser().Seq;

            string sql = @"
SELECT FN1.[Seq]
      ,FN1.[EngReportSeq]
      ,FN1.[Date]
      ,FN1.[FileNumber]
      ,FN1.[FilePath]
      ,FN1.[CreateTime]
      ,FN1.[CreateUserSeq]
      ,FN1.[ModifyTime]
      ,FN1.[ModifyUserSeq]
	  ,ISNULL(FN2.DisplayName,'') AS CreateUser
	  ,ISNULL(FN3.DisplayName,'') AS ModifyUser
      ,dbo.DelGuidStr(FN1.[FilePath]) FileName
FROM [dbo].[EngReportLocalCommunication] FN1
	LEFT OUTER JOIN [dbo].[UserMain] FN2 ON FN2.Seq = FN1.[CreateUserSeq]
	LEFT OUTER JOIN [dbo].[UserMain] FN3 ON FN3.Seq = FN1.[ModifyUserSeq]
WHERE FN1.[Seq] = @Seq";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", Seq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //新增 提案審查-在地溝通辦理情形
        public int AddEngReportLocalCommunication(EngReportLocalCommunicationVModel m)
        {
            Null2Empty(m);
            try
            {
                string sql = @"
INSERT INTO [dbo].[EngReportLocalCommunication]
           ([EngReportSeq],[Date],[FileNumber],[FilePath]
           ,[CreateTime],[CreateUserSeq],[ModifyTime],[ModifyUserSeq])
     VALUES
           (@EngReportSeq,@Date,@FileNumber,@FilePath
           ,GETDATE(),@CreateUserSeq,GETDATE(),@ModifyUserSeq)";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngReportSeq", m.EngReportSeq);
                cmd.Parameters.AddWithValue("@Date", m.Date);
                cmd.Parameters.AddWithValue("@FileNumber", m.FileNumber);
                cmd.Parameters.AddWithValue("@FilePath", m.FilePath);
                cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.AddEngReportLocalCommunication " + e.Message);
                return -1;
            }
        }

        //更新 提案審查-在地溝通辦理情形
        public int UpdateEngReportLocalCommunication(EngReportLocalCommunicationVModel m)
        {
            Null2Empty(m);
            string sql = @"
UPDATE [dbo].[EngReportLocalCommunication]
   SET [Date] = @Date
      ,[FileNumber] = @FileNumber
      ,[FilePath] = @FilePath
      ,[ModifyTime] = GetDate()
      ,[ModifyUserSeq] = @ModifyUserSeq
 WHERE [Seq] = @Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@Date", m.Date);
                cmd.Parameters.AddWithValue("@FileNumber", m.FileNumber);
                cmd.Parameters.AddWithValue("@FilePath", m.FilePath);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.UpdateEngReportLocalCommunication: " + e.Message);
                return -1;
            }
        }

        //刪除 提案審查-在地溝通辦理情形
        public int DelEngReportLocalCommunication(int Seq)
        {
            try
            {
                string sql = @"DELETE FROM [dbo].[EngReportLocalCommunication] WHERE [Seq] = @Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", Seq);
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                //db.TransactionRollback();
                log.Info("EngReportService.DelEngReportLocalCommunication" + e.Message);
                return -1;
            }
        }

        //下載-在地溝通辦理情形
        public List<T> GetDownloadFileLC<T>(int seq)
        {
            string sql = @"
                select
                    Seq ,EngReportSeq ,FilePath as FileName
                from EngReportLocalCommunication
                where Seq=@Seq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        #endregion 

        #region 提案審查-在地諮詢辦理情形

        //提案審查-在地諮詢辦理情形清單
        public List<T> GetEngReportOnSiteConsultatioList<T>(int Seq)
        {
            int userSeq = new SessionManager().GetUser().Seq;

            string sql = @"
SELECT FN1.[Seq]
      ,FN1.[EngReportSeq]
      ,FN1.[Date]
      ,FN1.[FileNumber]
      ,FN1.[FilePath]
      ,FN1.[CreateTime]
      ,FN1.[CreateUserSeq]
      ,FN1.[ModifyTime]
      ,FN1.[ModifyUserSeq]
	  ,ISNULL(FN2.DisplayName,'') AS CreateUser
	  ,ISNULL(FN3.DisplayName,'') AS ModifyUser
      ,dbo.DelGuidStr(FN1.[FilePath]) FileName
  FROM [dbo].[EngReportOnSiteConsultation] FN1
	LEFT OUTER JOIN [dbo].[UserMain] FN2 ON FN2.Seq = FN1.[CreateUserSeq]
	LEFT OUTER JOIN [dbo].[UserMain] FN3 ON FN3.Seq = FN1.[ModifyUserSeq]
WHERE FN1.[EngReportSeq] = @Seq";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", Seq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //提案審查-在地諮詢辦理情形
        public List<T> GetEngReportOnSiteConsultatioBySeq<T>(int Seq)
        {
            int userSeq = new SessionManager().GetUser().Seq;

            string sql = @"
SELECT FN1.[Seq]
      ,FN1.[EngReportSeq]
      ,FN1.[Date]
      ,FN1.[FileNumber]
      ,FN1.[FilePath]
      ,FN1.[CreateTime]
      ,FN1.[CreateUserSeq]
      ,FN1.[ModifyTime]
      ,FN1.[ModifyUserSeq]
	  ,ISNULL(FN2.DisplayName,'') AS CreateUser
	  ,ISNULL(FN3.DisplayName,'') AS ModifyUser
      ,dbo.DelGuidStr(FN1.[FilePath]) FileName
  FROM [dbo].[EngReportOnSiteConsultation] FN1
	LEFT OUTER JOIN [dbo].[UserMain] FN2 ON FN2.Seq = FN1.[CreateUserSeq]
	LEFT OUTER JOIN [dbo].[UserMain] FN3 ON FN3.Seq = FN1.[ModifyUserSeq]
WHERE FN1.[Seq] = @Seq";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", Seq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //新增 提案審查-在地諮詢辦理情形
        public int AddEngReportOnSiteConsultation(EngReportOnSiteConsultationVModel m)
        {
            Null2Empty(m);
            try
            {
                string sql = @"
INSERT INTO [dbo].[EngReportOnSiteConsultation]
           ([EngReportSeq],[Date],[FileNumber],[FilePath]
           ,[CreateTime],[CreateUserSeq],[ModifyTime],[ModifyUserSeq])
     VALUES
           (@EngReportSeq,@Date,@FileNumber,@FilePath
           ,GETDATE(),@CreateUserSeq,GETDATE(),@ModifyUserSeq)";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngReportSeq", m.EngReportSeq);
                cmd.Parameters.AddWithValue("@Date", m.Date);
                cmd.Parameters.AddWithValue("@FileNumber", m.FileNumber);
                cmd.Parameters.AddWithValue("@FilePath", m.FilePath);
                cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.AddEngReportOnSiteConsultation " + e.Message);
                return -1;
            }
        }

        //更新 提案審查-在地諮詢辦理情形
        public int UpdateEngReportOnSiteConsultation(EngReportOnSiteConsultationVModel m)
        {
            Null2Empty(m);
            string sql = @"
UPDATE [dbo].[EngReportOnSiteConsultation]
   SET [Date] = @Date
      ,[FileNumber] = @FileNumber
      --,[FilePath] = @FilePath
      ,[ModifyTime] = GetDate()
      ,[ModifyUserSeq] = @ModifyUserSeq
 WHERE [Seq] = @Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@Date", m.Date);
                cmd.Parameters.AddWithValue("@FileNumber", m.FileNumber);
                //cmd.Parameters.AddWithValue("@FilePath", m.FilePath);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.UpdateEngReportOnSiteConsultation: " + e.Message);
                return -1;
            }
        }

        //刪除 提案審查-在地諮詢辦理情形
        public int DelEngReportOnSiteConsultation(int Seq)
        {
            try
            {
                string sql = @"DELETE FROM [dbo].[EngReportOnSiteConsultation] WHERE [Seq] = @Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", Seq);
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                //db.TransactionRollback();
                log.Info("EngReportService.DelEngReportOnSiteConsultation" + e.Message);
                return -1;
            }
        }

        //下載-在地諮詢辦理情形
        public List<T> GetDownloadFileSC<T>(int seq)
        {
            string sql = @"
                select
                    Seq ,EngReportSeq ,FilePath as FileName
                from EngReportOnSiteConsultation
                where Seq=@Seq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        #endregion 

        #region 提案審查-主要工作內容

        //提案審查-主要工作內容清單
        public List<T> GetEngReportMainJobDescriptionList<T>(int Seq)
        {
            int userSeq = new SessionManager().GetUser().Seq;

            string sql = @"
SELECT FN1. [Seq]
      ,FN1.[EngReportSeq]
      ,FN1.[RptJobDescriptionSeq]
      ,FN1.[OtherJobDescription]
      ,FN1.[Num]
      ,FN1.[Cost]
      ,FN1.[Memo]
      ,FN1.[CreateTime]
      ,FN1.[CreateUserSeq]
      ,FN1.[ModifyTime]
      ,FN1.[ModifyUserSeq]
	  ,ISNULL(FN2.DisplayName,'') AS CreateUser
	  ,ISNULL(FN3.DisplayName,'') AS ModifyUser
	  ,ISNULL(FN4.Name ,'') AS RptJobDescriptionName
FROM [dbo].[EngReportMainJobDescription] FN1
	LEFT OUTER JOIN [dbo].[UserMain] FN2 ON FN2.Seq = FN1.[CreateUserSeq]
	LEFT OUTER JOIN [dbo].[UserMain] FN3 ON FN3.Seq = FN1.[ModifyUserSeq]
	LEFT OUTER JOIN [dbo].[ReportJobDescriptionList] FN4 ON FN4.Seq = FN1.[RptJobDescriptionSeq]
WHERE FN1.[EngReportSeq] = @Seq";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", Seq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //提案審查-主要工作內容
        public List<T> GetEngReportMainJobDescriptionBySeq<T>(int Seq)
        {
            int userSeq = new SessionManager().GetUser().Seq;

            string sql = @"
SELECT FN1. [Seq]
      ,FN1.[EngReportSeq]
      ,FN1.[RptJobDescriptionSeq]
      ,FN1.[OtherJobDescription]
      ,FN1.[Num]
      ,FN1.[Cost]
      ,FN1.[Memo]
      ,FN1.[CreateTime]
      ,FN1.[CreateUserSeq]
      ,FN1.[ModifyTime]
      ,FN1.[ModifyUserSeq]
	  ,ISNULL(FN2.DisplayName,'') AS CreateUser
	  ,ISNULL(FN3.DisplayName,'') AS ModifyUser
	  ,ISNULL(FN4.Name ,'') AS RptJobDescriptionName
FROM [dbo].[EngReportMainJobDescription] FN1
	LEFT OUTER JOIN [dbo].[UserMain] FN2 ON FN2.Seq = FN1.[CreateUserSeq]
	LEFT OUTER JOIN [dbo].[UserMain] FN3 ON FN3.Seq = FN1.[ModifyUserSeq]
	LEFT OUTER JOIN [dbo].[ReportJobDescriptionList] FN4 ON FN4.Seq = FN1.[RptJobDescriptionSeq]
WHERE FN1.[Seq] = @Seq";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", Seq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //新增 提案審查-主要工作內容清單
        public int AddEngReportMainJobDescription(EngReportMainJobDescriptionVModel m)
        {
            Null2Empty(m);
            try
            {
                string sql = @"
INSERT INTO [dbo].[EngReportMainJobDescription]
           ([EngReportSeq],[RptJobDescriptionSeq],[OtherJobDescription],[Num],[Cost],[Memo]
           ,[CreateTime],[CreateUserSeq],[ModifyTime],[ModifyUserSeq])
     VALUES
           (@EngReportSeq,@RptJobDescriptionSeq,@OtherJobDescription,@Num,@Cost,@Memo
           ,GETDATE(),@CreateUserSeq,GETDATE(),@ModifyUserSeq)";

                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngReportSeq", m.EngReportSeq);
                cmd.Parameters.AddWithValue("@RptJobDescriptionSeq", m.RptJobDescriptionSeq);
                cmd.Parameters.AddWithValue("@OtherJobDescription", m.OtherJobDescription);
                cmd.Parameters.AddWithValue("@Num", m.Num);
                cmd.Parameters.AddWithValue("@Cost", m.Cost);
                cmd.Parameters.AddWithValue("@Memo", m.Memo);
                cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.AddEngReportMainJobDescription " + e.Message);
                return -1;
            }
        }

        //更新 提案審查-主要工作內容清單
        public int UpdateEngReportMainJobDescription(EngReportMainJobDescriptionVModel m)
        {
            Null2Empty(m);
            string sql = @"
UPDATE [dbo].[EngReportMainJobDescription]
   SET [RptJobDescriptionSeq] = @RptJobDescriptionSeq
      ,[OtherJobDescription] = @OtherJobDescription
      ,[Num] = @Num
      ,[Cost] = @Cost
      ,[Memo] = @Memo
      ,[ModifyTime] = GetDate()
      ,[ModifyUserSeq] = @ModifyUserSeq
 WHERE [Seq] = @Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@RptJobDescriptionSeq", m.RptJobDescriptionSeq);
                cmd.Parameters.AddWithValue("@OtherJobDescription", m.OtherJobDescription);
                cmd.Parameters.AddWithValue("@Num", m.Num);
                cmd.Parameters.AddWithValue("@Cost", m.Cost);
                cmd.Parameters.AddWithValue("@Memo", m.Memo);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.UpdateEngReportMainJobDescription: " + e.Message);
                return -1;
            }
        }

        //刪除 提案審查-主要工作內容清單
        public int DelEngReportMainJobDescription(int Seq)
        {
            try
            {
                string sql = @"DELETE FROM [dbo].[EngReportMainJobDescription] WHERE [Seq] = @Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", Seq);
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                //db.TransactionRollback();
                log.Info("EngReportService.DelEngReportMainJobDescription" + e.Message);
                return -1;
            }
        }

        #endregion

        #region 

        //新增簽核流程
        public int AddEngReportApprove(EngReportVModel m)
        {
            Null2Empty(m);
            string unitSubSeq = "";
            string unitSeq = "";
            Utils.GetUserUnit(ref unitSeq, ref unitSubSeq);
            try
            {
                string sql = @"
                    --新增簽核流程
                    DECLARE @MAX INT = ISNULL((SELECT MAX([GroupId]) FROM [dbo].[EngReportApprove] WHERE [EngReportSeq] = @EngReportSeq ),0)+1;

                    INSERT INTO [dbo].[EngReportApprove]
                               ([EngReportSeq],[GroupId],[ApprovalModuleListSeq],[Signature]
                               ,[UnitSeq],[SubUnitSeq],[PositionSeq],[UserMainSeq]
                               ,CreateTime ,CreateUserSeq ,ModifyTime ,ModifyUserSeq)
                    SELECT @EngReportSeq, @MAX, FN1.[Seq], ''
	                    ,CASE WHEN FN2.[Name] = '不限'  THEN NULL  ELSE @ExecUnitSeq END
                        ,CASE WHEN FN2.[Name] IN ('申請人','提案單位主管') THEN @ExecSubUnitSeq ELSE FN3.Seq END
                        ,CASE WHEN FN2.[Name]='申請人' THEN 0 ELSE FN4.[PositionSeq] END 
                        ,CASE WHEN FN2.[Name]='申請人' THEN @CreateUserSeq ELSE 0 END 
                        ,GETDATE() ,@CreateUserSeq ,GETDATE() ,@ModifyUserSeq
                    FROM [dbo].[ApprovalModuleList] FN1
	                    INNER JOIN [dbo].[ApproverList] FN2 ON FN1.[Approver] = FN2.[Seq]
	                    LEFT OUTER JOIN (SELECT * FROM [dbo].[Unit] WHERE [ParentSeq]=@ExecUnitSeq) FN3 ON FN2.[Name] = FN3.[Name]
                        LEFT OUTER JOIN (SELECT [ApproverListSeq],MIN([PositionSeq]) AS [PositionSeq] FROM [dbo].[ApprovalPosition] GROUP BY [ApproverListSeq]) FN4 ON FN2.[Seq]=FN4.[ApproverListSeq]
                    WHERE FN1.[FormCode]='A01';

                    --SELECT @EngReportSeq, @MAX, [Seq], ''
                    --      , @ExecUnitSeq, @ExecSubUnitSeq, 0 ,@CreateUserSeq 
                    --      ,GETDATE() ,@CreateUserSeq ,GETDATE() ,@ModifyUserSeq
                    --FROM [dbo].[ApprovalModuleList] 
                    --WHERE [FormCode]='A01';

                    INSERT INTO [dbo].[EngReportApprovePosition]
                               ([EngReportSeq],[GroupId],[ApprovalModuleListSeq],[UnitSeq],[SubUnitSeq],[PositionSeq],[UserMainSeq])
                    SELECT @EngReportSeq, @MAX, FN1.[Seq], @ExecUnitSeq, @ExecSubUnitSeq, 0 ,@CreateUserSeq 
                    FROM [dbo].[ApprovalModuleList] FN1 
	                    INNER JOIN [dbo].[ApproverList] FN2 ON FN1.[Approver] = FN2.[Seq]
                    WHERE FN1.[FormCode]='A01' AND FN2.[Name]='申請人'
                    UNION ALL
                    SELECT @EngReportSeq, @MAX, FN1.[Seq], @ExecUnitSeq, CASE WHEN FN2.[Name] IN ('申請人','提案單位主管') THEN @ExecSubUnitSeq ELSE FN4.Seq END, FN3.PositionSeq ,0 
                    FROM [dbo].[ApprovalModuleList] FN1
	                    INNER JOIN [dbo].[ApproverList] FN2 ON FN1.[Approver] = FN2.[Seq]
	                    INNER JOIN [dbo].[ApprovalPosition] FN3 ON FN2.Seq = FN3.ApproverListSeq 
	                    LEFT OUTER JOIN (SELECT * FROM [dbo].[Unit] WHERE [ParentSeq]=@ExecUnitSeq) FN4 ON FN2.[Name] = FN4.[Name]
                    WHERE FN1.[FormCode]='A01' AND FN2.[Name]<>'申請人'

                    --SELECT @EngReportSeq, @MAX, FN1.[Seq], @ExecUnitSeq, @ExecSubUnitSeq, FN3.PositionSeq ,@CreateUserSeq 
                    --FROM [dbo].[ApprovalModuleList] FN1
	                --    INNER JOIN [dbo].[ApproverList] FN2 ON FN1.[Approver] = FN2.[Seq]
	                --    INNER JOIN [dbo].[ApprovalPosition] FN3 ON FN2.Seq = FN3.ApproverListSeq 
	                --    INNER JOIN [dbo].[Position] FN4 ON FN3.PositionSeq = FN4.Seq 
                    --WHERE FN1.[FormCode]='A01'

                    ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngReportSeq", m.Seq);
                cmd.Parameters.AddWithValue("@ExecUnitSeq", m.ExecUnitSeq);
                cmd.Parameters.AddWithValue("@ExecSubUnitSeq", m.ExecSubUnitSeq);
                cmd.Parameters.AddWithValue("@CreateUserSeq", m.CreateUserSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
                //int engReportSeq = (int)db.ExecuteScalar(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.AddEngReportApprove " + e.Message);
                return -1;
            }
        }

        //更新簽核流程
        public int UpdateEngReportApprove(EngReportVModel m)
        {
            Null2Empty(m);
            string unitSubSeq = "";
            string unitSeq = "";
            Utils.GetUserUnit(ref unitSeq, ref unitSubSeq);
            try
            {
                string sql = @"

                    --更新簽核單位
                    --UPDATE FN1
                    --SET FN1.[SubUnitSeq]=ISNULL(FN4.Seq,0)
                    --	,FN1.[PositionSeq]=ISNULL(FN3.[PositionSeq],0)
                    --	,FN1.[UserMainSeq]=0
                    --FROM [dbo].[EngReportApprove] FN1
                    --	INNER JOIN [dbo].[ApprovalModuleList] FN2 ON FN1.[ApprovalModuleListSeq] = FN2.[Seq] 
                    --	LEFT OUTER JOIN [dbo].[ApproverList] FN3 ON FN2.[Approver] = FN3.[Seq]
                    --	LEFT OUTER JOIN (SELECT * FROM [dbo].[Unit] WHERE [ParentSeq]=@ExecUnitSeq) FN4 ON FN3.[Name] = FN4.[Name]
                    --WHERE FN1.[EngReportSeq] = @EngReportSeq AND FN3.[Name] NOT IN ('申請人','提案單位主管');

                    UPDATE FN1
                    SET FN1.[SubUnitSeq]=ISNULL(FN5.Seq,0)
	                    --,FN1.[PositionSeq]=ISNULL(FN4.[PositionSeq],0)
	                    ,FN1.[UserMainSeq]=0
                    FROM [dbo].[EngReportApprovePosition] FN1
	                    INNER JOIN [dbo].[ApprovalModuleList] FN2 ON FN1.[ApprovalModuleListSeq] = FN2.[Seq] 
	                    LEFT OUTER JOIN [dbo].[ApproverList] FN3 ON FN2.[Approver] = FN3.[Seq]
	                    --LEFT OUTER JOIN [dbo].[ApprovalPosition] FN4 ON FN3.Seq = FN4.ApproverListSeq 
	                    --LEFT OUTER JOIN [dbo].[Unit] FN5 ON FN1.[UnitSeq] = FN5.[ParentSeq] AND FN3.[Name] = FN5.[Name]
                    WHERE FN1.[EngReportSeq] = @EngReportSeq AND FN3.[Name] NOT IN ('申請人','提案單位主管')

                    UPDATE FN1
                    SET FN1.[SubUnitSeq]=@ExecSubUnitSeq
	                    --,FN1.[PositionSeq]=ISNULL(FN4.[PositionSeq],0)
	                    ,FN1.[UserMainSeq]=0
                    FROM [dbo].[EngReportApprovePosition] FN1
	                    INNER JOIN [dbo].[ApprovalModuleList] FN2 ON FN1.[ApprovalModuleListSeq] = FN2.[Seq] 
	                    LEFT OUTER JOIN [dbo].[ApproverList] FN3 ON FN2.[Approver] = FN3.[Seq]
	                    --LEFT OUTER JOIN [dbo].[ApprovalPosition] FN4 ON FN3.Seq = FN4.ApproverListSeq 
                    WHERE FN1.[EngReportSeq] = @EngReportSeq AND RTRIM(FN3.[Name]) IN ('提案單位主管')

                    UPDATE FN1
                    SET FN1.[SubUnitSeq]=FN2.[SubUnitSeq], FN1.[PositionSeq]=FN2.[PositionSeq], FN1.[UserMainSeq]=FN2.[UserMainSeq]
                    FROM [dbo].[EngReportApprove] FN1
	                    INNER JOIN (SELECT * FROM [EngReportApprovePosition] WHERE Seq IN (SELECT MIN(Seq) FROM [EngReportApprovePosition] WHERE EngReportSeq = @EngReportSeq GROUP BY [ApprovalModuleListSeq])) FN2 
		                    ON FN1.[ApprovalModuleListSeq] = FN2.[ApprovalModuleListSeq]
                    WHERE FN1.EngReportSeq = @EngReportSeq
                    ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngReportSeq", m.Seq);
                cmd.Parameters.AddWithValue("@ExecUnitSeq", m.ExecUnitSeq);
                cmd.Parameters.AddWithValue("@ExecSubUnitSeq", m.ExecSubUnitSeq);
                cmd.Parameters.AddWithValue("@CreateUserSeq", m.CreateUserSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);
                int engReportSeq = (int)db.ExecuteScalar(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("EngReportService.UpdateEngReportApprove " + e.Message);
                return -1;
            }
        }

        #endregion 
    }
}