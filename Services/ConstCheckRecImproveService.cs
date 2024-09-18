using EQC.Common;
using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class ConstCheckRecImproveService : BaseService
    {//抽驗缺失改善

        //工程年分清單
        public List<EngYearVModel> GetEngYearList()
        {
            string sql = @"
                SELECT DISTINCT
                    cast(a.EngYear as integer) EngYear
                FROM EngMain a
                inner join SupervisionProjectList z on(z.EngMainSeq=a.Seq)
                where 1=1"
                + Utils.getAuthoritySql("a.") //a.CreateUserSeq=@CreateUserSeq
                + @" and exists (
                    select f.EngMainSeq from EngConstruction f
                    where f.EngMainSeq=a.Seq
                    and (
                        select COUNT(z2.Seq)  from ConstCheckRec z1
                        inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq=z1.Seq and z2.CCRCheckResult=2)
                        where z1.EngConstructionSeq=f.Seq
				    )>0
                )
                order by EngYear DESC";
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
                inner join SupervisionProjectList z on(z.EngMainSeq=a.Seq)            
                inner join Unit b on(b.Seq=a.ExecUnitSeq and b.parentSeq is null)
                where a.EngYear=@EngYear"
                + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @" and exists (
                    select f.EngMainSeq from EngConstruction f
                    where f.EngMainSeq=a.Seq
                    and (
                        select COUNT(z2.Seq)  from ConstCheckRec z1
                        inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq=z1.Seq and z2.CCRCheckResult=2)
                        where z1.EngConstructionSeq=f.Seq
				    )>0
                )
                order by b.OrderNo";
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
                inner join SupervisionProjectList z on(z.EngMainSeq=a.Seq)
                inner join Unit b on(b.Seq=a.ExecSubUnitSeq and @ParentSeq=b.parentSeq)
                where a.EngYear=@EngYear"
                + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @" and exists (
                    select f.EngMainSeq from EngConstruction f
                    where f.EngMainSeq=a.Seq
                    and (
                        select COUNT(z2.Seq)  from ConstCheckRec z1
                        inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq=z1.Seq and z2.CCRCheckResult=2)
                        where z1.EngConstructionSeq=f.Seq
				    )>0
                )
                order by b.OrderNo";
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
                    SELECT
                        a.Seq,
                        a.EngName
                    FROM EngMain a
                    inner join SupervisionProjectList d on(
                        d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                    )
                    where a.EngYear=@EngYear
                    and a.ExecUnitSeq=@ExecUnitSeq"
                    + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                    + @" and exists (
                        select f.EngMainSeq from EngConstruction f
                        where f.EngMainSeq=a.Seq
                        and (
                            select COUNT(z2.Seq)  from ConstCheckRec z1
                            inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq=z1.Seq and z2.CCRCheckResult=2)
                            where z1.EngConstructionSeq=f.Seq
				        )>0
                    )
                    order by EngNo DESC
                    ";
            }
            else
            {
                sql = @"
                    SELECT
                        a.Seq,
                        a.EngName
                    FROM EngMain a
                    inner join SupervisionProjectList d on(
                        d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                    )
                    where a.EngYear=@EngYear
                    ---and exists (select EngMainSeq from EngConstruction where EngMainSeq=a.Seq)
                    and a.ExecSubUnitSeq=@ExecSubUnitSeq"
                    + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                    + @" and exists (
                        select f.EngMainSeq from EngConstruction f
                        where f.EngMainSeq=a.Seq
                        and (
                            select COUNT(z2.Seq)  from ConstCheckRec z1
                            inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq=z1.Seq and z2.CCRCheckResult=2)
                            where z1.EngConstructionSeq=f.Seq
				        )>0
                    )
                    order by EngNo DESC
                    ";
            }
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngYear", year);
            cmd.Parameters.AddWithValue("@ExecUnitSeq", unitSeq);
            cmd.Parameters.AddWithValue("@ExecSubUnitSeq", subUnitSeq);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<T>(cmd);
        }

        //
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
                inner join SupervisionProjectList d on(
                    d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                )
                where a.Seq=@Seq
                and a.EngYear=@EngYear
                and a.ExecUnitSeq=@ExecUnitSeq"
                + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @" and (
                    select COUNT(z2.Seq)  from ConstCheckRec z1
                    inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq=z1.Seq and z2.CCRCheckResult=2)
                    where z1.EngConstructionSeq=f.Seq
				)>0
                ";
            }
            else
            {
                sql = @"
                SELECT
                    count(a.Seq) total
                FROM EngMain a
                inner join EngConstruction f on(f.EngMainSeq=a.Seq and( (-1=@subEngMain)or(f.Seq=@subEngMain)) )
                inner join Unit b on(b.Seq=a.ExecSubUnitSeq and a.ExecUnitSeq=@ExecUnitSeq)
                inner join SupervisionProjectList d on(
                    d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                )
                where a.Seq=@Seq
                and a.EngYear=@EngYear
                and a.ExecSubUnitSeq=@ExecSubUnitSeq"
                + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                + @"and (
                    select COUNT(z2.Seq)  from ConstCheckRec z1
                    inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq=z1.Seq and z2.CCRCheckResult=2)
                    where z1.EngConstructionSeq=f.Seq
				)>0
                ";
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
        public List<T> GetEngCreatedList<T>(int engMain)
        {
            string sql = @"
                select* from(
                    SELECT
                        a.Seq,
                        a.EngNo,
                        a.EngName,
                        b.Name ExecUnit,
                        c.Name ExecSubUnit,
                        a.SupervisorUnitName,
                        a.ApproveDate,
                        d.DocState,
                        f.ItemName subEngName,
                        f.Seq subEngNameSeq,
                        cast((
                            select COUNT(z2.Seq) from ConstCheckRec z1
                            inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq= z1.Seq and z2.CCRCheckResult= 2)
                            where z1.EngConstructionSeq= f.Seq
						)as int) missingCount
                    FROM EngMain a
                    inner join EngConstruction f on(f.EngMainSeq = a.Seq)
                    inner join Unit b on(b.Seq = a.ExecUnitSeq)
                    inner join SupervisionProjectList d on(
                        d.Seq = (select max(Seq) from SupervisionProjectList where EngMainSeq = a.Seq)
                    )
                    left outer join Unit c on(c.Seq= a.ExecSubUnitSeq)

                    where a.Seq= @Seq
                    "
                        + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                        + @"
                ) zz
                where missingCount > 0
                order by EngNo DESC
                ";
            /*if (subUnitSeq == -1)
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
                            d.DocState,
                            f.ItemName subEngName,
                            f.Seq subEngNameSeq,
                            cast((
                                select COUNT(z2.Seq)  from ConstCheckRec z1
                                inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq=z1.Seq and z2.CCRCheckResult=2)
                                where z1.EngConstructionSeq=f.Seq
						    )as int) missingCount
                        FROM EngMain a
                        inner join EngConstruction f on(f.EngMainSeq=a.Seq and( (-1=@subEngMain)or(f.Seq=@subEngMain)) )
                        inner join Unit b on(b.Seq=a.ExecUnitSeq)
                        inner join SupervisionProjectList d on(
                            d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                        )
                        left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)
                    
                        where a.Seq=@Seq
                        and a.EngYear=@EngYear
                        and a.ExecUnitSeq=@ExecUnitSeq"
                        + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                        + @"
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
                            d.DocState,
                            f.ItemName subEngName,
                            f.Seq subEngNameSeq,
                            cast((
                                select COUNT(z2.Seq)  from ConstCheckRec z1
                                inner join ConstCheckRecResult z2 on(z2.ConstCheckRecSeq=z1.Seq and z2.CCRCheckResult=2)
                                where z1.EngConstructionSeq=f.Seq
						    )as int) missingCount
                        FROM EngMain a
                        inner join EngConstruction f on(f.EngMainSeq=a.Seq and( (-1=@subEngMain)or(f.Seq=@subEngMain)) )
                        inner join Unit b on(b.Seq=a.ExecUnitSeq and a.ExecUnitSeq=@ExecUnitSeq)
                        inner join SupervisionProjectList d on(
                            d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.Seq)
                        )
                        left outer join Unit c on(c.Seq=a.ExecSubUnitSeq)
                        where a.Seq=@Seq
                        and a.EngYear=@EngYear
                        and a.ExecSubUnitSeq=@ExecSubUnitSeq"
                        + Utils.getAuthoritySql("a.") //and a.CreateUserSeq=@CreateUserSeq
                        + @"
                    ) zz
                    where missingCount > 0
                    order by EngNo DESC
                    OFFSET @pageIndex ROWS
				    FETCH FIRST @pageRecordCount ROWS ONLY";
            }*/
            SqlCommand cmd = db.GetCommand(sql);
            //cmd.Parameters.AddWithValue("@EngYear", year);
            //cmd.Parameters.AddWithValue("@ExecUnitSeq", unitSeq);
            //cmd.Parameters.AddWithValue("@ExecSubUnitSeq", subUnitSeq);
            cmd.Parameters.AddWithValue("@Seq", engMain);
            //cmd.Parameters.AddWithValue("@subEngMain", subEngMain);
            //cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            //cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            //cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());
            return db.GetDataTableWithClass<T>(cmd);
        }

        //工程主要施工項目
        public List<T> GetSubEngList<T>(int engMain)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    a.ItemName
                FROM EngConstruction a
                inner join SupervisionProjectList d on(
                    d.Seq=(select max(Seq) from SupervisionProjectList where EngMainSeq=a.EngMainSeq)
                )
                where a.EngMainSeq=@EngMainSeq
                order by a.OrderNo
				";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMain);

            return db.GetDataTableWithClass<T>(cmd);
        }
        
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

        //已有檢驗單之檢驗項目
        public List<T> GetRecCheckTypeOption<T>(int constructionSeq)
        {
            string sql = @"
                SELECT distinct a.CCRCheckType1 Value from ConstCheckRec a
                where a.EngConstructionSeq=@EngConstructionSeq 
                and a.CCRCheckType1=1
                and (
                    select COUNT(z2.Seq) from ConstCheckRecResult z2
                    where z2.ConstCheckRecSeq=a.Seq
                    and z2.CCRCheckResult=2
				)>0
                order by a.CCRCheckType1";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngConstructionSeq", constructionSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //檢驗單清單 單一項目
        public List<T> GetListByCheckType<T>(int engConstructionSeq, int checkType)
        {
            string sql = @"SELECT
                        Seq,
                        EngConstructionSeq,
                        CCRCheckType1,
                        ItemSeq,
                        CCRCheckFlow,
                        CCRCheckDate,
                        CCRPosLati,
                        CCRPosLong,
                        CCRPosDesc
                        FROM ConstCheckRec
                        where EngConstructionSeq=@EngConstructionSeq
                        and CCRCheckType1=@CCRCheckType1
                        and (
                            select COUNT(z2.Seq) from ConstCheckRecResult z2
                            where z2.ConstCheckRecSeq=ConstCheckRec.Seq
                            and z2.CCRCheckResult=2
				        )>0
                        order by CCRCheckDate DESC
                        ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngConstructionSeq", engConstructionSeq);
            cmd.Parameters.AddWithValue("@CCRCheckType1", checkType);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //檢驗單清單 s20230920
        public List<T> GetListByCheckType1<T>(int engMainSeq, int checkType, int itemSeq)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    a.EngConstructionSeq,
                    a.CCRCheckType1,
                    a.ItemSeq,
                    a.CCRCheckFlow,
                    a.CCRCheckDate,
                    a.CCRPosLati,
                    a.CCRPosLong,
                    a.CCRPosDesc,
                    b.ItemName
                FROM ConstCheckRec a
                inner join EngConstruction b on(b.Seq=a.EngConstructionSeq)
                where a.EngConstructionSeq in (select seq from EngConstruction where EngMainSeq=@EngMainSeq)
                and a.CCRCheckType1=@CCRCheckType1
                and a.ItemSeq=@ItemSeq
                and (
                    select COUNT(z2.Seq) from ConstCheckRecResult z2
                    where z2.ConstCheckRecSeq=a.Seq
                    and z2.CCRCheckResult=2
                )>0
                order by a.CCRCheckDate DESC
                        ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@CCRCheckType1", checkType);
            cmd.Parameters.AddWithValue("@ItemSeq", itemSeq);

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
        //更新
        public bool Update(ConstCheckRecModel recItem, ConstCheckRecImproveVModel report)
        {
            string sql = "";
            SqlCommand cmd;
            Null2Empty(recItem);
            Null2Empty(report);

            db.BeginTransaction();
            try
            {
                if (report.Seq == null)
                {
                    sql = @"insert into ConstCheckRecImprove(
                            ConstCheckRecSeq,
                            CheckItemKind,
                            IncompKind,
                            CheckerKind,
                            ImproveDeadline,
                            CauseAnalysis,
                            Improvement,
                            ProcessResult,
                            --ImproveAuditResult,
                            --ProcessTrackDate,
                            --TrackCont,
                            CanClose,
                            --CloseMemo,
                            FormConfirm,
                            CreateTime,
                            CreateUserSeq,
                            ModifyTime,
                            ModifyUserSeq
                        ) values(
                            @ConstCheckRecSeq,
                            @CheckItemKind,
                            @IncompKind,
                            @CheckerKind,
                            @ImproveDeadline,
                            @CauseAnalysis,
                            @Improvement,
                            @ProcessResult,
                            --@ImproveAuditResult,
                            --@ProcessTrackDate,
                            --@TrackCont,
                            0,
                            --@CloseMemo,
                            0,
                            GetDate(),
                            @ModifyUserSeq,
                            GetDate(),
                            @ModifyUserSeq                        
                        )";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@ConstCheckRecSeq", report.ConstCheckRecSeq);
                }
                else
                {
                    sql = @"update ConstCheckRecImprove set
                            CheckItemKind=@CheckItemKind,
                            IncompKind=@IncompKind,
                            CheckerKind=@CheckerKind,
                            ImproveDeadline=@ImproveDeadline,
                            CauseAnalysis=@CauseAnalysis,
                            Improvement=@Improvement,
                            ProcessResult=@ProcessResult,
                            --ImproveAuditResult=@ImproveAuditResult,
                            --ProcessTrackDate=@ProcessTrackDate,
                            --TrackCont=@TrackCont,
                            --CanClose=@CanClose,
                            --CloseMemo=@CloseMemo,
                            ModifyTime=GetDate(),
                            ModifyUserSeq=@ModifyUserSeq
                        where Seq=@Seq";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", report.Seq);
                }

                cmd.Parameters.AddWithValue("@CheckItemKind", this.NulltoDBNull(report.CheckItemKind));
                cmd.Parameters.AddWithValue("@IncompKind", this.NulltoDBNull(report.IncompKind));
                cmd.Parameters.AddWithValue("@CheckerKind", this.NulltoDBNull(report.CheckerKind));
                cmd.Parameters.AddWithValue("@ImproveDeadline", this.NulltoDBNull(report.ImproveDeadline));
                cmd.Parameters.AddWithValue("@CauseAnalysis", report.CauseAnalysis);
                cmd.Parameters.AddWithValue("@Improvement", report.Improvement);
                cmd.Parameters.AddWithValue("@ProcessResult", report.ProcessResult);
                //cmd.Parameters.AddWithValue("@ImproveAuditResult", this.NulltoDBNull(report.ImproveAuditResult));
                //cmd.Parameters.AddWithValue("@ProcessTrackDate", this.NulltoDBNull(report.ProcessTrackDate));
                //cmd.Parameters.AddWithValue("@TrackCont", report.TrackCont);
                //cmd.Parameters.AddWithValue("@CanClose", this.NulltoDBNull(report.CanClose));
                //cmd.Parameters.AddWithValue("@CloseMemo", report.CloseMemo);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                db.ExecuteNonQuery(cmd);

                if (report.Seq == null)
                {
                    sql = @"SELECT IDENT_CURRENT('ConstCheckRecImprove') AS NewSeq";
                    cmd = db.GetCommand(sql);
                    DataTable dt = db.GetDataTable(cmd);
                    report.Seq = Convert.ToInt16(dt.Rows[0]["NewSeq"].ToString());
                }

                db.TransactionCommit();

                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("ConstCheckRecImproveService.Update: " + e.Message);
                log.Info(sql);
                return false;
            }
        }

        //表單確認
        public int ReportConfirm(int seq, int state)
        {
            string sql = sql = @"update ConstCheckRecImprove set
                            FormConfirm=@FormConfirm,
                            ImproveUserSeq=@ModifyUserSeq,
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
            return GetNgReoprtList<T>(constructionSeq, -1);
        }
        //不符合事項報告清單 s20230522
        public List<T> GetNgReoprtList<T>(int constructionSeq, int itemSeq)
        {
            string sql = @"
                        select
                            b.*,
                            a.CCRCheckType1,
                            a.CCRPosDesc,
                            a.SupervisorUserSeq,
                            a.SupervisorDirectorSeq,
                            a.CCRCheckDate,
                            (
                            select top 1 CCRRealCheckCond from ConstCheckRecResult z
                            where z.ConstCheckRecSeq=a.Seq and z.CCRCheckResult=2
                            ) CCRRealCheckCond
                        FROM ConstCheckRec a
                        inner join ConstCheckRecImprove b on(b.ConstCheckRecSeq=a.Seq)
                        where a.EngConstructionSeq=@EngConstructionSeq
                        and (@ItemSeq=-1 or a.ItemSeq=@ItemSeq)
                        --and CCRCheckType1=@CCRCheckType1
                        and (
                            select COUNT(z2.Seq) from ConstCheckRecResult z2
                            where z2.ConstCheckRecSeq=a.Seq
                            and z2.CCRCheckResult=2
				        )>0
                        order by a.CCRCheckDate";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngConstructionSeq", constructionSeq);
            cmd.Parameters.AddWithValue("@ItemSeq", itemSeq);//s20230522
            return db.GetDataTableWithClass<T>(cmd);
        }

        //NCR程序追蹤改善表
        public List<T> GetNCRList<T>(int constructionSeq)
        {
            return GetNCRList<T>(constructionSeq, -1);
        }
        //NCR程序追蹤改善表 s20230522
        public List<T> GetNCRList<T>(int constructionSeq, int itemSeq)
        {      
        string sql = @"
                        select
                            b.*,
                            a.CCRPosDesc,
                            a.CCRCheckDate,
                            b1.IncompKind,
                            a.CCRPosDesc
                        FROM ConstCheckRec a
                        inner join NCR b on(b.ConstCheckRecSeq=a.Seq)
                        inner join ConstCheckRecImprove b1 on(b1.ConstCheckRecSeq=a.Seq)
                        where a.EngConstructionSeq=@EngConstructionSeq
                        and (@ItemSeq=-1 or a.ItemSeq=@ItemSeq)
                        --and CCRCheckType1=@CCRCheckType1
                        and (
                            select COUNT(z2.Seq) from ConstCheckRecResult z2
                            where z2.ConstCheckRecSeq=a.Seq
                            and z2.CCRCheckResult=2
				        )>0
                        order by a.CCRCheckDate";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngConstructionSeq", constructionSeq);
            cmd.Parameters.AddWithValue("@ItemSeq", itemSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //有缺失項目檢驗單之分項工程清單 s20230523
        public List<SelectIntOptionModel> GetRecEngConstruction(int mode, int engMainSeq, int constCheckListSeq)
        {
            string sql = @"
                SELECT distinct b.Seq Value, b.ItemName Text
                FROM EngMain a
                inner join EngConstruction b on(b.EngMainSeq=a.Seq)
                inner join ConstCheckRec c on(c.EngConstructionSeq=b.Seq and c.CCRCheckType1=@mode and c.ItemSeq=@ItemSeq)
                inner join ConstCheckRecResult d on(d.ConstCheckRecSeq=c.Seq and d.CCRCheckResult=2)
                where a.Seq=@EngMainSeq
                ";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@mode", mode);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@ItemSeq", constCheckListSeq);
            return db.GetDataTableWithClass<SelectIntOptionModel>(cmd);
        }
        //工程-分項工程資訊 s20230522
        public List<T> GetEngConstruction<T>(int engMainSeq, int mode, int constCheckListSeq)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    a.EngNo,
                    a.EngName,
                    a.ApproveDate,
                    a.ApproveNo,
                    a.SupervisorUnitName,
                    a.SupervisorContact,
                    c.Name organizerUnitName,
                    c1.Name execUnitName,
                    c2.Name execSubUnitName,
                    d.DocState,
                    e.ItemName subEngName,
                    e.ItemNo subEngItemNo,
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
                where e.Seq in (
                    SELECT distinct f.EngConstructionSeq FROM EngMain a
                    inner join EngConstruction e on(e.EngMainSeq=a.Seq)
                    inner join ConstCheckRec f on(f.EngConstructionSeq=e.Seq and f.CCRCheckType1=@mode and f.ItemSeq=@ItemSeq)
                    inner join ConstCheckRecImprove zd on(zd.ConstCheckRecSeq=f.Seq)
                    where a.Seq=@EngMainSeq
                )"
                + Utils.getAuthoritySql("a.");
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@mode", mode);
            cmd.Parameters.AddWithValue("@ItemSeq", constCheckListSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //1 施工抽查清單 s20230522
        public List<T> GetConstCheckList1<T>(int engMain)
        {
            string sql = @"
                select
                	z.Seq, z.OrderNo, z.ItemName ,z.missingCount
                    ,(
    	                select count(zd.Seq) from EngMain za
                        inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
                        inner join ConstCheckRec zc on(zc.EngConstructionSeq = zb.Seq and zc.ItemSeq=z.Seq)
                        inner join ConstCheckRecImprove zd on(zd.ConstCheckRecSeq=zc.Seq)
                        where za.Seq=@EngMainSeq
                    ) improveCount
                    ,(
    	                select count(zd.Seq) from EngMain za
                        inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
                        inner join ConstCheckRec zc on(zc.EngConstructionSeq = zb.Seq and zc.ItemSeq=z.Seq)
                        inner join NCR zd on(zd.ConstCheckRecSeq=zc.Seq)
                        where za.Seq=@EngMainSeq
                    ) ncrCount
                    ,(
    	                select COUNT(zz.Seq) from (
                            select zf.Seq from EngMain za
        	                inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
        	                inner join ConstCheckRec zc on(zc.EngConstructionSeq = zb.Seq and zc.ItemSeq=z.Seq)
                            inner join ConstCheckRecFile zd on(zd.ConstCheckRecSeq=zc.Seq)
                            inner join ConstCheckControlSt zf on(zf.ConstCheckListSeq=zc.ItemSeq and zf.Seq=zd.ControllStSeq )
                            where za.Seq=@EngMainSeq
                            union
                            select zf.Seq from EngMain za
        	                inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
        	                inner join ConstCheckRec zc on(zc.EngConstructionSeq = zb.Seq and zc.ItemSeq=z.Seq)
                            inner join ConstCheckRecImprove zd on(zd.ConstCheckRecSeq=zc.Seq)
                            inner join ConstCheckRecImproveFile ze on(ze.ConstCheckRecImproveSeq=zd.Seq)
                            inner join ConstCheckControlSt zf on(zf.ConstCheckListSeq=zc.ItemSeq and zf.Seq=ze.ControllStSeq )
                            where za.Seq=@EngMainSeq
      	                ) zz
                    ) photoCount
                from (
                    SELECT
                        a.Seq, a.OrderNo, a.ItemName
                        ,(
                            select COUNT(z1.Seq) from ConstCheckRec z1
                            inner join ConstCheckRecResult zc on(zc.ConstCheckRecSeq=z1.Seq and zc.CCRCheckResult=2)
                            where z1.EngConstructionSeq in(
                                select zb.Seq from EngMain za
                                inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
                                where za.Seq=@EngMainSeq
                                and z1.ItemSeq=a.Seq
                            )
                            and z1.CCRCheckType1=1
                        ) missingCount
                    FROM ConstCheckList a
                    where a.EngMainSeq=@EngMainSeq
                    and a.DataKeep=1
                ) z
                where z.missingCount>0
                order by z.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMain);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //2 設備運轉測試清單 s20230522
        public List<T> GetEquOperTestList1<T>(int engMain)
        {
            string sql = @"
                select 
	                z.Seq, z.OrderNo, z.ItemName ,z.missingCount
                    ,(
    	                select count(zd.Seq) from EngMain za
                        inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
                        inner join ConstCheckRec zc on(zc.EngConstructionSeq = zb.Seq and zc.ItemSeq=z.Seq)
                        inner join ConstCheckRecImprove zd on(zd.ConstCheckRecSeq=zc.Seq)
                        where za.Seq=@EngMainSeq
                    ) improveCount
                    ,(
    	                select count(zd.Seq) from EngMain za
                        inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
                        inner join ConstCheckRec zc on(zc.EngConstructionSeq = zb.Seq and zc.ItemSeq=z.Seq)
                        inner join NCR zd on(zd.ConstCheckRecSeq=zc.Seq)
                        where za.Seq=@EngMainSeq
                    ) ncrCount
                    ,(
    	                select COUNT(zz.Seq) from (
                            select zf.Seq from EngMain za
        	                inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
        	                inner join ConstCheckRec zc on(zc.EngConstructionSeq = zb.Seq and zc.ItemSeq=z.Seq)
                            inner join ConstCheckRecFile zd on(zd.ConstCheckRecSeq=zc.Seq)
                            inner join ConstCheckControlSt zf on(zf.ConstCheckListSeq=zc.ItemSeq and zf.Seq=zd.ControllStSeq )
                            where za.Seq=@EngMainSeq
                            union
                            select zf.Seq from EngMain za
        	                inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
        	                inner join ConstCheckRec zc on(zc.EngConstructionSeq = zb.Seq and zc.ItemSeq=z.Seq)
                            inner join ConstCheckRecImprove zd on(zd.ConstCheckRecSeq=zc.Seq)
                            inner join ConstCheckRecImproveFile ze on(ze.ConstCheckRecImproveSeq=zd.Seq)
                            inner join ConstCheckControlSt zf on(zf.ConstCheckListSeq=zc.ItemSeq and zf.Seq=ze.ControllStSeq )
                            where za.Seq=@EngMainSeq
      	                ) zz
                    ) photoCount
                from (
                    SELECT
                        a.Seq, a.OrderNo, a.ItemName
                        ,(
                            select COUNT(z1.Seq) from ConstCheckRec z1
                            inner join ConstCheckRecResult zc on(zc.ConstCheckRecSeq=z1.Seq and zc.CCRCheckResult=2)
                            where z1.EngConstructionSeq in(
                                select zb.Seq from EngMain za
                                inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
                                where za.Seq=@EngMainSeq
                                and z1.ItemSeq=a.Seq
                            )
                            and z1.CCRCheckType1=2
                        ) missingCount
                    FROM EquOperTestList a
                    where a.EngMainSeq=@EngMainSeq
                    and a.DataKeep=1
                ) z
                where z.missingCount>0
                order by z.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMain);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //3 職業安全衛生清單 s20230522
        public List<T> GetOccuSafeHealthList1<T>(int engMain)
        {
            string sql = @"
                select 
	                z.Seq, z.OrderNo, z.ItemName ,z.missingCount
                    ,(
    	                select count(zd.Seq) from EngMain za
                        inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
                        inner join ConstCheckRec zc on(zc.EngConstructionSeq = zb.Seq and zc.ItemSeq=z.Seq)
                        inner join ConstCheckRecImprove zd on(zd.ConstCheckRecSeq=zc.Seq)
                        where za.Seq=@EngMainSeq
                    ) improveCount
                    ,(
    	                select count(zd.Seq) from EngMain za
                        inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
                        inner join ConstCheckRec zc on(zc.EngConstructionSeq = zb.Seq and zc.ItemSeq=z.Seq)
                        inner join NCR zd on(zd.ConstCheckRecSeq=zc.Seq)
                        where za.Seq=@EngMainSeq
                    ) ncrCount
                    ,(
    	                select COUNT(zz.Seq) from (
                            select zf.Seq from EngMain za
        	                inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
        	                inner join ConstCheckRec zc on(zc.EngConstructionSeq = zb.Seq and zc.ItemSeq=z.Seq)
                            inner join ConstCheckRecFile zd on(zd.ConstCheckRecSeq=zc.Seq)
                            inner join ConstCheckControlSt zf on(zf.ConstCheckListSeq=zc.ItemSeq and zf.Seq=zd.ControllStSeq )
                            where za.Seq=@EngMainSeq
                            union
                            select zf.Seq from EngMain za
        	                inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
        	                inner join ConstCheckRec zc on(zc.EngConstructionSeq = zb.Seq and zc.ItemSeq=z.Seq)
                            inner join ConstCheckRecImprove zd on(zd.ConstCheckRecSeq=zc.Seq)
                            inner join ConstCheckRecImproveFile ze on(ze.ConstCheckRecImproveSeq=zd.Seq)
                            inner join ConstCheckControlSt zf on(zf.ConstCheckListSeq=zc.ItemSeq and zf.Seq=ze.ControllStSeq )
                            where za.Seq=@EngMainSeq
      	                ) zz
                    ) photoCount
                from (
                    SELECT
                        a.Seq, a.OrderNo, a.ItemName
                        ,(
                            select COUNT(z1.Seq) from ConstCheckRec z1
                            inner join ConstCheckRecResult zc on(zc.ConstCheckRecSeq=z1.Seq and zc.CCRCheckResult=2)
                            where z1.EngConstructionSeq in(
                                select zb.Seq from EngMain za
                                inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
                                where za.Seq=@EngMainSeq
                                and z1.ItemSeq=a.Seq
                            )
                            and z1.CCRCheckType1=3
                        ) missingCount
                    FROM OccuSafeHealthList a
                    where a.EngMainSeq=@EngMainSeq
                    and a.DataKeep=1
                ) z
                where z.missingCount>0
                order by z.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMain);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //4 環境保育清單 s20230522
        public List<T> GetEnvirConsList1<T>(int engMain)
        {
            string sql = @"
                select 
	                z.Seq, z.OrderNo, z.ItemName ,z.missingCount
                    ,(
    	                select count(zd.Seq) from EngMain za
                        inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
                        inner join ConstCheckRec zc on(zc.EngConstructionSeq = zb.Seq and zc.ItemSeq=z.Seq)
                        inner join ConstCheckRecImprove zd on(zd.ConstCheckRecSeq=zc.Seq)
                        where za.Seq=@EngMainSeq
                    ) improveCount
                    ,(
    	                select count(zd.Seq) from EngMain za
                        inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
                        inner join ConstCheckRec zc on(zc.EngConstructionSeq = zb.Seq and zc.ItemSeq=z.Seq)
                        inner join NCR zd on(zd.ConstCheckRecSeq=zc.Seq)
                        where za.Seq=@EngMainSeq
                    ) ncrCount
                    ,(
    	                select COUNT(zz.Seq) from (
                            select zf.Seq from EngMain za
        	                inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
        	                inner join ConstCheckRec zc on(zc.EngConstructionSeq = zb.Seq and zc.ItemSeq=z.Seq)
                            inner join ConstCheckRecFile zd on(zd.ConstCheckRecSeq=zc.Seq)
                            inner join ConstCheckControlSt zf on(zf.ConstCheckListSeq=zc.ItemSeq and zf.Seq=zd.ControllStSeq )
                            where za.Seq=@EngMainSeq
                            union
                            select zf.Seq from EngMain za
        	                inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
        	                inner join ConstCheckRec zc on(zc.EngConstructionSeq = zb.Seq and zc.ItemSeq=z.Seq)
                            inner join ConstCheckRecImprove zd on(zd.ConstCheckRecSeq=zc.Seq)
                            inner join ConstCheckRecImproveFile ze on(ze.ConstCheckRecImproveSeq=zd.Seq)
                            inner join ConstCheckControlSt zf on(zf.ConstCheckListSeq=zc.ItemSeq and zf.Seq=ze.ControllStSeq )
                            where za.Seq=@EngMainSeq
      	                ) zz
                    ) photoCount
                from (
                    SELECT
                        a.Seq, a.OrderNo, a.ItemName
                        ,(
                            select COUNT(z1.Seq) from ConstCheckRec z1
                            inner join ConstCheckRecResult zc on(zc.ConstCheckRecSeq=z1.Seq and zc.CCRCheckResult=2)
                            where z1.EngConstructionSeq in(
                                select zb.Seq from EngMain za
                                inner join EngConstruction zb on(zb.EngMainSeq=za.Seq)  
                                where za.Seq=@EngMainSeq
                                and z1.ItemSeq=a.Seq
                            )
                            and z1.CCRCheckType1=4
                        ) missingCount
                    FROM EnvirConsList a
                    where a.EngMainSeq=@EngMainSeq
                    and a.DataKeep=1
                ) z
                where z.missingCount>0
                order by z.OrderNo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMain);
            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}