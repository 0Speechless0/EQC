using EQC.Common;
using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class ConstCheckRecImproveApproveService : BaseService
    {//抽驗缺失改善

        //工程年分清單
        public List<EngYearVModel> GetEngYearList()
        {
            string sql = @"
                SELECT DISTINCT
                    cast(a.EngYear as integer) EngYear
                FROM EngMain a
                inner join EngConstruction y ON(y.EngMainSeq=a.Seq)
                inner join ConstCheckRec y1 on(y1.EngConstructionSeq=y.Seq)
                inner join ConstCheckRecImprove y2 on(y2.ConstCheckRecSeq=y1.Seq and y2.FormConfirm>0)
                where 1=1"
                + Utils.getAuthoritySql("a.") //a.CreateUserSeq=@CreateUserSeq
                + @" order by EngYear DESC";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<EngYearVModel>(cmd);
        }
        //工程執行機關清單
        public List<EngExecUnitsVModel> GetEngExecUnitList(string engYear)
        {
            string sql = @"
                SELECT DISTINCT
                    b.OrderNo,
                    a.ExecUnitSeq UnitSeq,
                    b.Name UnitName
                FROM EngMain a
                inner join EngConstruction y ON(y.EngMainSeq=a.Seq)
                inner join ConstCheckRec y1 on(y1.EngConstructionSeq=y.Seq)
                inner join ConstCheckRecImprove y2 on(y2.ConstCheckRecSeq=y1.Seq and y2.FormConfirm>0)
                inner join Unit b on(b.Seq=a.ExecUnitSeq and b.parentSeq is null)
                where a.EngYear=@EngYear"
                + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @" order by b.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngYear", engYear);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<EngExecUnitsVModel>(cmd);
        }
        //工程執行單位清單
        public List<EngExecUnitsVModel> GetEngExecSubUnitList(string engYear, int parentSeq)
        {
            string sql = @"
                SELECT DISTINCT
                    b.OrderNo,
                    a.ExecSubUnitSeq UnitSeq,
                    b.Name UnitName
                FROM EngMain a
                inner join EngConstruction y ON(y.EngMainSeq=a.Seq)
                inner join ConstCheckRec y1 on(y1.EngConstructionSeq=y.Seq)
                inner join ConstCheckRecImprove y2 on(y2.ConstCheckRecSeq=y1.Seq and y2.FormConfirm>0)
                inner join Unit b on(b.Seq=a.ExecSubUnitSeq and @ParentSeq=b.parentSeq)
                where a.EngYear=@EngYear"
                + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @" order by b.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngYear", engYear);
            cmd.Parameters.AddWithValue("@ParentSeq", parentSeq);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<EngExecUnitsVModel>(cmd);
        }
        
        //工程名稱清單
        public List<T> GetEngCreatedNameList<T>(string year, int unitSeq, int subUnitSeq)
        {
            string sql = @"";
            if (subUnitSeq == -1)
            {
                sql = @"
                    SELECT DISTINCT a.EngNo,
                        a.Seq,
                        a.EngName
                    FROM EngMain a
                    inner join EngConstruction y ON(y.EngMainSeq=a.Seq)
                    inner join ConstCheckRec y1 on(y1.EngConstructionSeq=y.Seq)
                    inner join ConstCheckRecImprove y2 on(y2.ConstCheckRecSeq=y1.Seq and y2.FormConfirm>0)
                    where a.EngYear=@EngYear
                    and a.ExecUnitSeq=@ExecUnitSeq"
                    + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                    + @" order by EngNo DESC
                    ";
            }
            else
            {
                sql = @"
                    SELECT DISTINCT a.EngNo,
                        a.Seq,
                        a.EngName
                    FROM EngMain a
                    inner join EngConstruction y ON(y.EngMainSeq=a.Seq)
                    inner join ConstCheckRec y1 on(y1.EngConstructionSeq=y.Seq)
                    inner join ConstCheckRecImprove y2 on(y2.ConstCheckRecSeq=y1.Seq and y2.FormConfirm>0)
                    where a.EngYear=@EngYear
                    and a.ExecSubUnitSeq=@ExecSubUnitSeq"
                    + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                    + @" order by EngNo DESC
                    ";
            }
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngYear", year);
            cmd.Parameters.AddWithValue("@ExecUnitSeq", unitSeq);
            cmd.Parameters.AddWithValue("@ExecSubUnitSeq", subUnitSeq);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<T>(cmd);
        }

        internal List<T> GetEngCreatedList<T>(int engMain, int subEngMain, int pageRecordCount, int pageIndex)
        {
            string sql = @"
                    select * from (
                        SELECT
                            a.Seq,
                            a.EngNo,
                            a.EngName,
                            b.Name ExecUnit, 
                            c.Name ExecSubUnit,
                            a.SupervisorUnitName,
                            a.ApproveDate,
                            f.ItemName subEngName,
                            f.Seq subEngNameSeq,
                            cast((
                                select COUNT(z2.Seq)  from ConstCheckRec z1
                                inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq=z1.Seq and z2.CCRCheckResult=2)
                                where z1.EngConstructionSeq=f.Seq
						    )as int) missingCount,
                            COALESCE((
                                select max(w2.FormConfirm) from ConstCheckRec w1
                                inner join ConstCheckRecImprove w2 on(w2.ConstCheckRecSeq=w1.Seq and w2.FormConfirm=1)
                                where w1.EngConstructionSeq=f.Seq
                            ),0) hasUnderReview,
                            COALESCE((
                                select max(x2.FormConfirm) from ConstCheckRec x1
                                inner join ConstCheckRecImprove x2 on(x2.ConstCheckRecSeq=x1.Seq and x2.FormConfirm=2)
                                where x1.EngConstructionSeq=f.Seq
                            ),0) hasApproved
                        FROM EngMain a
                        inner join EngConstruction f on(f.EngMainSeq=a.Seq and( (-1=@subEngMain)or(f.Seq=@subEngMain)) )
                        --inner join ConstCheckRec y1 on(y1.EngConstructionSeq=f.Seq)
                        --inner join ConstCheckRecImprove y2 on(y2.ConstCheckRecSeq=y1.Seq and y2.FormConfirm>0)
                        inner join Unit b on(b.Seq=a.ExecUnitSeq)
                        left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)
                    
                        where a.Seq=@Seq
                        "
                                    + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                                    + @" and exists (
                            select y1.seq from ConstCheckRec y1
                            inner join ConstCheckRecImprove y2 on(y2.ConstCheckRecSeq=y1.Seq and y2.FormConfirm>0)
                            where y1.EngConstructionSeq=f.Seq
                        )
                    ) zz
                    where missingCount > 0
                    order by EngNo DESC
                    OFFSET @pageIndex ROWS
				    FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", engMain);
            cmd.Parameters.AddWithValue("@subEngMain", subEngMain);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<T>(cmd);
        }

        //工程主要施工項目
        public List<T> GetSubEngList<T>(int engMain)
        {
            string sql = @"
                SELECT DISTINCT a.OrderNo,
                    a.Seq,
                    a.ItemName
                FROM EngConstruction a
                inner join ConstCheckRec y1 on(y1.EngConstructionSeq=a.Seq)
                inner join ConstCheckRecImprove y2 on(y2.ConstCheckRecSeq=y1.Seq and y2.FormConfirm>0)
                where a.EngMainSeq=@EngMainSeq
                order by a.OrderNo
				";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMain);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public int GetEngCreatedListCount(int engMain, int subEngMain)
        {
            string sql = @"
                SELECT
                    count(a.Seq) total
                FROM EngMain a
                inner join EngConstruction f on(f.EngMainSeq=a.Seq and( (-1=@subEngMain)or(f.Seq=@subEngMain)) )
                inner join Unit b on(b.Seq=a.ExecUnitSeq)
                where a.Seq=@Seq
                "
                + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @" and exists (
                    select y1.seq from ConstCheckRec y1
                    inner join ConstCheckRecImprove y2 on(y2.ConstCheckRecSeq=y1.Seq and y2.FormConfirm>0)
                    where y1.EngConstructionSeq=f.Seq
                )";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", engMain);
            cmd.Parameters.AddWithValue("@subEngMain", subEngMain);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());

            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());

        }
            public int GetEngCreatedListCount(string year, int unitSeq, int subUnitSeq, int engMain, int subEngMain)
        {
            string sql = @"";
            if (subUnitSeq == -1)
            {
                sql = @"
                SELECT
                    count(a.Seq) total
                FROM EngMain a
                inner join EngConstruction f on(f.EngMainSeq=a.Seq and( (-1=@subEngMain)or(f.Seq=@subEngMain)) )
                inner join Unit b on(b.Seq=a.ExecUnitSeq)
                where a.Seq=@Seq
                and a.EngYear=@EngYear
                and a.ExecUnitSeq=@ExecUnitSeq"
                + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @" and exists (
                    select y1.seq from ConstCheckRec y1
                    inner join ConstCheckRecImprove y2 on(y2.ConstCheckRecSeq=y1.Seq and y2.FormConfirm>0)
                    where y1.EngConstructionSeq=f.Seq
                )";
            }
            else
            {
                sql = @"
                SELECT
                    count(a.Seq) total
                FROM EngMain a
                inner join EngConstruction f on(f.EngMainSeq=a.Seq and( (-1=@subEngMain)or(f.Seq=@subEngMain)) )
                inner join Unit b on(b.Seq=a.ExecSubUnitSeq and a.ExecUnitSeq=@ExecUnitSeq)
                where a.Seq=@Seq
                and a.EngYear=@EngYear
                and a.ExecSubUnitSeq=@ExecSubUnitSeq"
                + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @" and exists (
                    select y1.seq from ConstCheckRec y1
                    inner join ConstCheckRecImprove y2 on(y2.ConstCheckRecSeq=y1.Seq and y2.FormConfirm>0)
                    where y1.EngConstructionSeq=f.Seq
                )";
            }
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", engMain);
            cmd.Parameters.AddWithValue("@EngYear", year);
            cmd.Parameters.AddWithValue("@ExecUnitSeq", unitSeq);
            cmd.Parameters.AddWithValue("@ExecSubUnitSeq", subUnitSeq);
            cmd.Parameters.AddWithValue("@subEngMain", subEngMain);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());

            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetEngCreatedList<T>(string year, int unitSeq, int subUnitSeq, int engMain, int subEngMain, int pageRecordCount, int pageIndex)
        {
            string sql = @"";
            if (subUnitSeq == -1)
            {
                sql = @"
                    select * from (
                        SELECT
                            a.Seq,
                            a.EngNo,
                            a.EngName,
                            b.Name ExecUnit, 
                            c.Name ExecSubUnit,
                            a.SupervisorUnitName,
                            a.ApproveDate,
                            f.ItemName subEngName,
                            f.Seq subEngNameSeq,
                            cast((
                                select COUNT(z2.Seq)  from ConstCheckRec z1
                                inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq=z1.Seq and z2.CCRCheckResult=2)
                                where z1.EngConstructionSeq=f.Seq
						    )as int) missingCount,
                            COALESCE((
                                select max(w2.FormConfirm) from ConstCheckRec w1
                                inner join ConstCheckRecImprove w2 on(w2.ConstCheckRecSeq=w1.Seq and w2.FormConfirm=1)
                                where w1.EngConstructionSeq=f.Seq
                            ),0) hasUnderReview,
                            COALESCE((
                                select max(x2.FormConfirm) from ConstCheckRec x1
                                inner join ConstCheckRecImprove x2 on(x2.ConstCheckRecSeq=x1.Seq and x2.FormConfirm=2)
                                where x1.EngConstructionSeq=f.Seq
                            ),0) hasApproved
                        FROM EngMain a
                        inner join EngConstruction f on(f.EngMainSeq=a.Seq and( (-1=@subEngMain)or(f.Seq=@subEngMain)) )
                        --inner join ConstCheckRec y1 on(y1.EngConstructionSeq=f.Seq)
                        --inner join ConstCheckRecImprove y2 on(y2.ConstCheckRecSeq=y1.Seq and y2.FormConfirm>0)
                        inner join Unit b on(b.Seq=a.ExecUnitSeq)
                        left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)
                    
                        where a.Seq=@Seq
                        and a.EngYear=@EngYear
                        and a.ExecUnitSeq=@ExecUnitSeq"
                        + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                        + @" and exists (
                            select y1.seq from ConstCheckRec y1
                            inner join ConstCheckRecImprove y2 on(y2.ConstCheckRecSeq=y1.Seq and y2.FormConfirm>0)
                            where y1.EngConstructionSeq=f.Seq
                        )
                    ) zz
                    where missingCount > 0
                    order by EngNo DESC
                    OFFSET @pageIndex ROWS
				    FETCH FIRST @pageRecordCount ROWS ONLY";
            }
            else
            {
                sql = @"
                    select * from (
                        SELECT
                            a.Seq,
                            a.EngNo,
                            a.EngName,
                            b.Name ExecUnit, 
                            c.Name ExecSubUnit,
                            a.SupervisorUnitName,
                            a.ApproveDate,
                            f.ItemName subEngName,
                            f.Seq subEngNameSeq,
                            cast((
                                select COUNT(z2.Seq)  from ConstCheckRec z1
                                inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq=z1.Seq and z2.CCRCheckResult=2)
                                where z1.EngConstructionSeq=f.Seq
						    )as int) missingCount,
                            COALESCE((
                                select max(w2.FormConfirm) from ConstCheckRec w1
                                inner join ConstCheckRecImprove w2 on(w2.ConstCheckRecSeq=w1.Seq and w2.FormConfirm=1)
                                where w1.EngConstructionSeq=f.Seq
                            ),0) hasUnderReview,
                            COALESCE((
                                select max(x2.FormConfirm) from ConstCheckRec x1
                                inner join ConstCheckRecImprove x2 on(x2.ConstCheckRecSeq=x1.Seq and x2.FormConfirm=2)
                                where x1.EngConstructionSeq=f.Seq
                            ),0) hasApproved
                        FROM EngMain a
                        inner join EngConstruction f on(f.EngMainSeq=a.Seq and( (-1=@subEngMain)or(f.Seq=@subEngMain)) )
                        --inner join ConstCheckRec y1 on(y1.EngConstructionSeq=f.Seq)
                        --inner join ConstCheckRecImprove y2 on(y2.ConstCheckRecSeq=y1.Seq and y2.FormConfirm>0)
                        inner join Unit b on(b.Seq=a.ExecUnitSeq and a.ExecUnitSeq=@ExecUnitSeq)
                        left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)
                        where a.Seq=@Seq
                        and a.EngYear=@EngYear
                        and a.ExecSubUnitSeq=@ExecSubUnitSeq"
                        + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                        + @" and exists (
                                select y1.seq from ConstCheckRec y1
                                inner join ConstCheckRecImprove y2 on(y2.ConstCheckRecSeq=y1.Seq and y2.FormConfirm>0)
                                where y1.EngConstructionSeq=f.Seq
                            )
                    ) zz
                    where missingCount > 0
                    order by EngNo DESC
                    OFFSET @pageIndex ROWS
				    FETCH FIRST @pageRecordCount ROWS ONLY";
            }
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngYear", year);
            cmd.Parameters.AddWithValue("@ExecUnitSeq", unitSeq);
            cmd.Parameters.AddWithValue("@ExecSubUnitSeq", subUnitSeq);
            cmd.Parameters.AddWithValue("@Seq", engMain);
            cmd.Parameters.AddWithValue("@subEngMain", subEngMain);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<T>(cmd);
        }

        //已有檢驗單之檢驗項目
        public List<T> GetRecCheckTypeOption<T>(int constructionSeq)
        {
            string sql = @"
                SELECT distinct a.CCRCheckType1 Value from ConstCheckRec a
                inner join ConstCheckRecImprove y2 on(y2.ConstCheckRecSeq=a.Seq and y2.FormConfirm>0)
                where a.EngConstructionSeq=@EngConstructionSeq 
                --and (
                        --select COUNT(z2.Seq) from ConstCheckRecResult z2
                        --where z2.ConstCheckRecSeq=a.Seq
                        --and z2.CCRCheckResult=2
                        
				--    )>0
                order by a.CCRCheckType1";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngConstructionSeq", constructionSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //檢驗單清單 單一項目
        public List<T> GetListByCheckType<T>(int engConstructionSeq, int checkType)
        {
            string sql = @"SELECT
                        a.Seq,
                        a.EngConstructionSeq,
                        a.CCRCheckType1,
                        a.ItemSeq,
                        a.CCRCheckFlow,
                        a.CCRCheckDate,
                        a.CCRPosLati,
                        a.CCRPosLong,
                        a.CCRPosDesc
                        FROM ConstCheckRec a
                        inner join ConstCheckRecImprove y2 on(y2.ConstCheckRecSeq=a.Seq and y2.FormConfirm>0)
                        where a.EngConstructionSeq=@EngConstructionSeq
                        and a.CCRCheckType1=@CCRCheckType1
                        and (
                            select COUNT(z2.Seq) from ConstCheckRecResult z2
                            where z2.ConstCheckRecSeq=a.Seq
                            and z2.CCRCheckResult=2
				        )>0
                        order by a.CCRCheckDate DESC
                        ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngConstructionSeq", engConstructionSeq);
            cmd.Parameters.AddWithValue("@CCRCheckType1", checkType);

            return db.GetDataTableWithClass<T>(cmd);
        }

        //審核更新
        public int Update(ConstCheckRecImproveVModel report)
        {
            Null2Empty(report);
            report.FormConfirm = (byte)(report.ImproveAuditResult.Value == 1 ? 2 : 0);
            string sql = @"
                update ConstCheckRecImprove set
                        ImproveAuditResult=@ImproveAuditResult,
                        ProcessTrackDate=@ProcessTrackDate,
                        TrackCont=@TrackCont,
                        CanClose=@CanClose,
                        --CloseMemo=@CloseMemo,
                        ApproveUserSeq=@ModifyUserSeq,
                        ApproveDate=GetDate(),
                        FormConfirm=@FormConfirm
                        --ModifyTime=GetDate(),
                        --ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", report.Seq);
            cmd.Parameters.AddWithValue("@ImproveAuditResult", report.ImproveAuditResult);
            cmd.Parameters.AddWithValue("@FormConfirm", report.FormConfirm);
            cmd.Parameters.AddWithValue("@ProcessTrackDate", this.NulltoDBNull(report.ProcessTrackDate));
            cmd.Parameters.AddWithValue("@TrackCont", report.TrackCont);
            cmd.Parameters.AddWithValue("@CanClose", report.CanClose);
            //cmd.Parameters.AddWithValue("@CloseMemo", report.CloseMemo);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
            return db.ExecuteNonQuery(cmd);
        }
        /*
        //由 工程主要施工項目 取得工程資訊
        public List<T> GetEngMainByEngConstructionSeq<T>(int seq)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    a.EngNo,
                    a.EngName,
                    a.ApproveDate,
                    a.ApproveNo,
                    a.SupervisorUnitName,
                    c.Name organizerUnitName,
                    c1.Name execUnitName,
                    c2.Name execSubUnitName,
                    d.DocState,
                    e.ItemName subEngName,
                    e.Seq subEngNameSeq
                FROM EngConstruction e
                inner join EngMain a on(a.Seq=e.EngMainSeq)
                left outer join Unit c on(c.Seq=a.OrganizerUnitSeq)
                left outer join Unit c1 on(c1.Seq=a.ExecUnitSeq)
                left outer join Unit c2 on(c2.Seq=a.ExecSubUnitSeq)
                left outer join SupervisionProjectList d on(
                    d.EngMainSeq=a.Seq
                    and d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                )
                where e.Seq=@Seq"
                + Utils.getAuthoritySql("a.");
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        public List<T> GetItemByConstCheckRecSeq<T>(int constCheckRecSeq)
        {
            string sql = @"SELECT
                        Seq,
                        ConstCheckRecSeq,
                        CheckItemKind,
                        IncompKind,
                        CheckerKind,
                        ImproveDeadline,
                        CauseAnalysis,
                        Improvement,
                        ProcessResult,
                        ImproveAuditResult,
                        ProcessTrackDate,
                        TrackCont,
                        CanClose,
                        CloseMemo,
                        FormConfirm
                    FROM ConstCheckRecImprove
                    where ConstCheckRecSeq=@ConstCheckRecSeq
                        ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@ConstCheckRecSeq", constCheckRecSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        //表單確認
        public int ReportConfirm(int seq, int state)
        {
            string sql = sql = @"update ConstCheckRecImprove set
                            FormConfirm=@FormConfirm,
                            ModifyTime=GetDate(),
                            ModifyUserSeq=@ModifyUserSeq
                        where Seq=@Seq";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            cmd.Parameters.AddWithValue("@FormConfirm", state);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
            return db.ExecuteNonQuery(cmd);
        }

        //施工抽查 照片群組清單
        public List<T> GetConstCheckPhotoGroupOption<T>(int recSeq)
        {
            string sql = @"
                select * from (
                        select
                            (c.CCManageItem1+c.CCManageItem2) [Text], c.Seq [Value], c.OrderNo
                        from ConstCheckRec a
                        inner join ConstCheckRecFile b on(b.ConstCheckRecSeq=a.Seq)
                        inner join ConstCheckControlSt c on(c.ConstCheckListSeq=a.ItemSeq and c.Seq=b.ControllStSeq )
                        where a.Seq=@Seq
                        union
                        select
                            (c.CCManageItem1+c.CCManageItem2) [Text], c.Seq [Value], c.OrderNo
                        from ConstCheckRec a
                        inner join ConstCheckRecImprove b on(b.ConstCheckRecSeq=a.Seq)
                        inner join ConstCheckRecImproveFile b1 on(b1.ConstCheckRecImproveSeq=b.Seq)
                        inner join ConstCheckControlSt c on(c.ConstCheckListSeq=a.ItemSeq and c.Seq=b1.ControllStSeq )
                        where a.Seq=@Seq
                ) z
                order by z.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", recSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //設備運轉 照片群組清單
        public List<T> GetEquOperPhotoGroupOption<T>(int recSeq)
        {
            string sql = @"
                select * from (
                        select
                            (c.EPCheckItem1+c.EPCheckItem2) [Text], c.Seq [Value], c.OrderNo
                        from ConstCheckRec a
                        inner join ConstCheckRecFile b on(b.ConstCheckRecSeq=a.Seq)
                        inner join EquOperControlSt c on(c.EquOperTestStSeq=a.ItemSeq and c.Seq=b.ControllStSeq )
                        where a.Seq=@Seq
                        union
                        select
                            (c.EPCheckItem1+c.EPCheckItem2) [Text], c.Seq [Value], c.OrderNo
                        from ConstCheckRec a
                        inner join ConstCheckRecImprove b on(b.ConstCheckRecSeq=a.Seq)
                        inner join ConstCheckRecImproveFile b1 on(b1.ConstCheckRecImproveSeq=b.Seq)
                        inner join EquOperControlSt c on(c.EquOperTestStSeq=a.ItemSeq and c.Seq=b1.ControllStSeq )
                        where a.Seq=@Seq
                ) z
                order by z.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", recSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //職業安全衛生 照片群組清單
        public List<T> GetOccuSafeHealthGroupOption<T>(int recSeq)
        {
            string sql = @"
                select * from (
                        select
                            DISTINCT (c.OSCheckItem1+c.OSCheckItem2) [Text], c.Seq [Value], c.OrderNo
                        from ConstCheckRec a
                        inner join ConstCheckRecFile b on(b.ConstCheckRecSeq=a.Seq)
                        inner join OccuSafeHealthControlSt c on(c.OccuSafeHealthListSeq=a.ItemSeq and c.Seq=b.ControllStSeq )
                        where a.Seq=@Seq
                        union
                        select
                            (c.OSCheckItem1+c.OSCheckItem2) [Text], c.Seq [Value], c.OrderNo
                        from ConstCheckRec a
                        inner join ConstCheckRecImprove b on(b.ConstCheckRecSeq=a.Seq)
                        inner join ConstCheckRecImproveFile b1 on(b1.ConstCheckRecImproveSeq=b.Seq)
                        inner join OccuSafeHealthControlSt c on(c.OccuSafeHealthListSeq=a.ItemSeq and c.Seq=b1.ControllStSeq )
                        where a.Seq=@Seq
                ) z
                order by z.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", recSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //環境保育 照片群組清單
        public List<T> GetEnvirConsGroupOption<T>(int recSeq)
        {
            string sql = @"
                select * from (
                        select
                            DISTINCT (c.ECCCheckItem1+c.ECCCheckItem2) [Text], c.Seq [Value], c.OrderNo
                        from ConstCheckRec a
                        inner join ConstCheckRecFile b on(b.ConstCheckRecSeq=a.Seq)
                        inner join EnvirConsControlSt c on(c.EnvirConsListSeq=a.ItemSeq and c.Seq=b.ControllStSeq )
                        where a.Seq=@Seq
                        union
                        select
                            (c.ECCCheckItem1+c.ECCCheckItem2) [Text], c.Seq [Value], c.OrderNo
                        from ConstCheckRec a
                        inner join ConstCheckRecImprove b on(b.ConstCheckRecSeq=a.Seq)
                        inner join ConstCheckRecImproveFile b1 on(b1.ConstCheckRecImproveSeq=b.Seq)
                        inner join EnvirConsControlSt c on(c.EnvirConsListSeq=a.ItemSeq and c.Seq=b1.ControllStSeq )
                        where a.Seq=@Seq
                ) z
                order by z.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", recSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //不符合事項報告清單
        public List<T> GetNgReoprtList<T>(int constructionSeq)
        {
            string sql = @"
                        select
                            b.*,
                            a.CCRCheckType1,
                            a.CCRPosDesc
                        FROM ConstCheckRec a
                        inner join ConstCheckRecImprove b on(b.ConstCheckRecSeq=a.Seq)
                        where EngConstructionSeq=@EngConstructionSeq
                        --and CCRCheckType1=@CCRCheckType1
                        and (
                            select COUNT(z2.Seq) from ConstCheckRecResult z2
                            where z2.ConstCheckRecSeq=a.Seq
                            and z2.CCRCheckResult=2
				        )>0
                        order by CCRCheckDate";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngConstructionSeq", constructionSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //NCR程序追蹤改善表
        public List<T> GetNCRList<T>(int constructionSeq)
        {
            string sql = @"
                        select
                            b.*,
                        a.CCRPosDesc
                        FROM ConstCheckRec a
                        inner join NCR b on(b.ConstCheckRecSeq=a.Seq)
                        where EngConstructionSeq=@EngConstructionSeq
                        --and CCRCheckType1=@CCRCheckType1
                        and (
                            select COUNT(z2.Seq) from ConstCheckRecResult z2
                            where z2.ConstCheckRecSeq=a.Seq
                            and z2.CCRCheckResult=2
				        )>0
                        order by CCRCheckDate";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngConstructionSeq", constructionSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
*/
    }
}