using EQC.Common;
using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class SuperviseFillService : BaseService
    {//督導填報
        //督導紀錄表 20230217
        public List<T> GetSuperviseRecordSheet<T>(int superviseEngSeq)
        {
            string sql = @"
                SELECT
	                b.PhaseCode,
                    a1.SuperviseMode,
                    a.TenderNo,
                    a.TenderName,
                    a.OrganizerName,
                    (
    	                select DisplayName from UserMain where Seq=d.CreateUserSeq
                    ) ContactName,
                    a1.SuperviseDate,
                    a1.SuperviseEndDate, --s20230316
                    a.Location,
                    a.SupervisionUnitName,
                    d.SupervisorSelfPerson1,
                    d.SupervisorCommPerson1,
                    a.ActualStartDate,
                    a.ScheCompletionDate,
                    a.ContractorName1,
                    a.EngOverview,
                    ISNULL(c.DesignChangeContractAmount, a.BidAmount) BidAmount,
                    (
                        SELECT STUFF(
                        (SELECT ',' + z.cName
                        FROM (
                            select DisplayName cName, 0 OrderNo from UserMain where Seq = a1.LeaderSeq --s20230325
                            union all

                            select zb.ECName cName, za.OrderNo from OutCommittee za
                            inner join ExpertCommittee zb on(zb.Seq=za.ExpertCommitteeSeq)
                            where za.SuperviseEngSeq=a1.Seq
                            union all
                            select zb.DisplayName cName, za.OrderNo from InsideCommittee za
                            inner join UserMain zb on(zb.Seq=za.UserMainSeq)
                            where za.SuperviseEngSeq=a1.Seq
                            /*union all 幹事不列
                            select top 1 zb.DisplayName cName, za.OrderNo from Officer za
        	                inner join UserMain zb on(zb.Seq=za.UserMainSeq)
        	                where za.SuperviseEngSeq=a1.Seq*/
                        ) z
                        FOR XML PATH('')) ,1,1,'')
                    ) AS CommitteeList,
                    a1.CommitteeAverageScore,
                    a1.Inspect,
                    (
    	                select sum(DeductPoint) from SuperviseFill where SuperviseEngSeq=a1.Seq
                    ) DeductPoints,
                    e.ProjectNo,
                    (
    	                select top 1 UnitCode from SuperviseUnitCode where UnitName=a.ExecUnitName or UnitName=SUBSTRING(a.ExecUnitName,1,3)
                    ) ExecUnitCode,
                    f.PDAccuActualProgress,
                    f.PDAccuScheProgress,
                    f.DiffProgress,
                    g.ImproveDeadline
                from SuperviseEng a1
                inner join SupervisePhase b on(b.Seq=a1.SupervisePhaseSeq)
                inner join PrjXML a on(a.Seq=a1.PrjXMLSeq)
                left outer join PrjXMLExt c on(c.PrjXMLSeq=a.Seq)
                left outer join EngMain d on(d.PrjXMLSeq=a.Seq)
                left outer join wraControlPlanNo e on(e.ProjectName=ISNULL(a.ManualBelongPrj, c.BelongPrj))
                left join ProgressData f on (
                    f.Seq = ( select top 1 Seq from ProgressData where PrjXMLSeq=a.Seq order by (PDYear*100+PDMonth) desc )
                )
                left join BackwardData g on (
                    g.Seq = ( select top 1 Seq from BackwardData where PrjXMLSeq=a.Seq order by (BDYear*100+BDMonth) desc )
                )
                where a1.Seq=@SuperviseEngSeq
				";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SuperviseEngSeq", superviseEngSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //抽驗項目清單 20230215
        public List<T> GetSamplingList<T>(int superviseEngSeq)
        {
            string sql = @"
                select
                    Seq,
                    SuperviseEngSeq,
                    SamplingName,
                    Location,
                    Quantity
                from SuperviseFillSampling
                where SuperviseEngSeq=@SuperviseEngSeq
				";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SuperviseEngSeq", superviseEngSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //更新抽驗項目 20230215
        public int UpdateSamplingItem(SuperviseFillSamplingModel m)
        {
            Null2Empty(m);
            string sql = @"
                update SuperviseFillSampling set
                    SamplingName = @SamplingName,
                    Location = @Location,
                    Quantity = @Quantity,
                    ModifyTime = GetDate(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@SamplingName", m.SamplingName);
                cmd.Parameters.AddWithValue("@Location", m.Location);
                cmd.Parameters.AddWithValue("@Quantity", m.Quantity);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("SupervisePhaseService.UpdateSamplingItem: " + e.Message);
                return -1;
            }
        }
        //新增抽驗項目 20230215
        public int AddSamplingItem(SuperviseFillSamplingModel m)
        {
            Null2Empty(m);
            string sql = @"
                insert into SuperviseFillSampling(
                    SuperviseEngSeq,
                    SamplingName,
                    Location,
                    Quantity,
                    CreateTime,
                    CreateUserSeq,
                    ModifyTime,
                    ModifyUserSeq
                ) values (
                    @SuperviseEngSeq,
                    @SamplingName,
                    @Location,
                    @Quantity,
                    GetDate(),
                    @ModifyUserSeq,
                    GetDate(),
                    @ModifyUserSeq
                )";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@SuperviseEngSeq", m.SuperviseEngSeq);
                cmd.Parameters.AddWithValue("@SamplingName", m.SamplingName);
                cmd.Parameters.AddWithValue("@Location", m.Location);
                cmd.Parameters.AddWithValue("@Quantity", m.Quantity);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("SupervisePhaseService.AddSamplingItem: " + e.Message);
                return -1;
            }
        }
        //刪除抽驗項目 20230215
        public int DelSamplingItem(int seq)
        {
            string sql = @"Delete from SuperviseFillSampling where Seq=@Seq";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("SupervisePhaseService.DelSamplingItem: " + e.Message);
                return -1;
            }
        }
        //儲存內外聘委員分數 20230215
        public bool SaveInspect(int superviseEngSeq, string inspect)
        {
            string sql = @"
            update SuperviseEng set
            Inspect=@Inspect
            where Seq=@Seq ";
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Inspect", inspect);
                cmd.Parameters.AddWithValue("@Seq", superviseEngSeq);
                db.ExecuteNonQuery(cmd);

                return true;
            }
            catch (Exception e)
            {
                log.Info("SupervisePhaseService.SaveInspect: " + e.Message);
                return false;
            }
        }
        //工程內外聘委員分數清單 20230215
        public List<T> GetEngCommittesScore<T>(int superviseEngSeq)
        {
            string sql = @"
                SELECT z.mode, z.Seq, z.CName, z.Score, z.OrderNo
                from (
                    select Cast(0 as int) mode,
                        a.OrderNo,
                        a.Seq,
                        a.Score,
	                    b.DisplayName CName
                    from InsideCommittee a
                    inner join UserMain b on(b.Seq=a.UserMainSeq)
                    where a.SuperviseEngSeq=@SuperviseEngSeq
                
                    union all
                    select Cast(1 as int) mode,
                        a.OrderNo,
                        a.Seq,
                        a.Score,
	                    b.ECName CName
                    from OutCommittee a
                    inner join ExpertCommittee b on(b.Seq=a.ExpertCommitteeSeq)
                    where a.SuperviseEngSeq=@SuperviseEngSeq               

                    union all
                    select Cast(2 as int) mode,
                        1 OrderNo,
                        a.Seq,
                        a.LeaderScore Score,
	                    b.DisplayName CName
                    from SuperviseEng a
                    inner join UserMain b on(b.Seq=a.LeaderSeq)
                    where a.Seq=@SuperviseEngSeq   
                ) z
                order by z.mode,z.OrderNo
				";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SuperviseEngSeq", superviseEngSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //儲存內外聘委員分數
        public bool SaveEngCommittesScore(int superviseEngSeq, List<SuperviseFillCommitteeScoreVModel> items)
        {
            string sql;
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                decimal committeeScore = 0;
                int committeeCnt = 0;
                foreach (SuperviseFillCommitteeScoreVModel m in items) {
                    Null2Empty(m);

                    if (m.mode == 0)
                    {
                        sql = @"
                        update InsideCommittee set
                        Score=@Score
                        where Seq=@Seq ";
                    } else if (m.mode == 1)
                    {
                        sql = @"
                        update OutCommittee set
                        Score=@Score
                        where Seq=@Seq ";
                    }
                    else
                    {
                        sql = @"
                        update SuperviseEng set
                        LeaderScore=@Score
                        where Seq=@Seq ";
                    }
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.AddWithValue("@Score", m.Score);
                    cmd.Parameters.AddWithValue("@Seq", m.Seq);
                    db.ExecuteNonQuery(cmd);
                    if (m.Score > 0)
                    {
                        committeeScore += m.Score;
                        committeeCnt++;
                    }
                }

                sql = @"
                    update SuperviseEng set
                    CommitteeAverageScore=@CommitteeAverageScore
                    where Seq=@Seq ";
                cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", superviseEngSeq);
                cmd.Parameters.AddWithValue("@CommitteeAverageScore", committeeCnt == 0 ? 0 : committeeScore / committeeCnt);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupervisePhaseService.SaveEngCommittesScore: " + e.Message);
                return false;
            }
        }

        //取得工程填報紀錄
        public List<T> GetRecords<T>(int superviseEngSeq)
        {
            string sql = @"SELECT
                    a.Seq,
                    a.SuperviseEngSeq,
                    a.MissingNo,
                    a.MissingLoc,
                    a.DeductPointStr,
                    a.DeductPoint,
                    a.SuperviseMemo,
                    b.Content missingContent
                FROM SuperviseFill a
                left outer join QualityDeductionPoints b on(b.MissingNo=a.MissingNo)
                where a.SuperviseEngSeq=@SuperviseEngSeq
                order by a.MissingNo
				";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SuperviseEngSeq", superviseEngSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //填報內外聘委員清單
        public List<string> GetRecordCommittes(int superviseEngSeq, int superviseFillSeq)
        {
            string sql = @"SELECT Cast(z.mode as varchar)+'-'+Cast(z.Seq as varchar) value ---z.mode, z.Seq, z.CName, z.OrderNo
                from (
                select Cast(0 as int) mode,  
                    a.OrderNo,
                    a.Seq,
	                b.DisplayName CName
                from InsideCommittee a
                inner join SuperviseFillInsideCommittee a1 on(a1.SuperviseFillSeq=@SuperviseFillSeq and a1.InsideCommitteeSeq=a.Seq)
                inner join UserMain b on(b.Seq=a.UserMainSeq)
                where a.SuperviseEngSeq=@SuperviseEngSeq
                union all

                select Cast(1 as int) mode,
                    a.OrderNo,
                    a.Seq,
	                b.ECName CName
                from OutCommittee a
                inner join SuperviseFillOutCommittee a1 on(a1.SuperviseFillSeq=@SuperviseFillSeq and a1.OutCommitteeSeq=a.Seq)
                inner join ExpertCommittee b on(b.Seq=a.ExpertCommitteeSeq)
                where a.SuperviseEngSeq=@SuperviseEngSeq

                union all
                select Cast(2 as int) mode,  
                    0 OrderNo,
                    b.Seq,
	                b.DisplayName CName
                from SuperviseFillInsideCommittee a1 
                inner join UserMain b on(b.Seq= -a1.InsideCommitteeSeq)
                inner join SuperviseFill s on (s.Seq = @SuperviseFillSeq)
                where s.SuperviseEngSeq = @SuperviseEngSeq
                ) z
                order by z.mode,z.OrderNo
				";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SuperviseEngSeq", superviseEngSeq);
            cmd.Parameters.AddWithValue("@SuperviseFillSeq", superviseFillSeq);
            DataTable dt = db.GetDataTable(cmd);

            List<string> items = new List<string>();
            for(int i=0; i<dt.Rows.Count; i++)
            {
                items.Add(dt.Rows[i]["value"].ToString());
            }
            return items;
        }
        //新增督導紀錄
        public int AddRecords(SuperviseFillVModel m)
        {
            Null2Empty(m);
            string sql = @"
            insert into SuperviseFill (
                SuperviseEngSeq,
                MissingNo,
                MissingLoc,
                DeductPointStr,
                DeductPoint,
                SuperviseMemo,
                CreateTime,
                CreateUserSeq,
                ModifyTime,
                ModifyUserSeq
            )values(
                @SuperviseEngSeq,
                @MissingNo,
                @MissingLoc,
                @DeductPointStr,
                @DeductPoint,
                @SuperviseMemo,
                GetDate(),
                @ModifyUserSeq,
                GetDate(),
                @ModifyUserSeq
            )";

            db.BeginTransaction();
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@SuperviseEngSeq", m.SuperviseEngSeq);
                cmd.Parameters.AddWithValue("@MissingNo", m.MissingNo);
                cmd.Parameters.AddWithValue("@MissingLoc", m.MissingLoc);
                cmd.Parameters.AddWithValue("@DeductPointStr", m.DeductPointStr);
                cmd.Parameters.AddWithValue("@DeductPoint", m.DeductPoint);
                cmd.Parameters.AddWithValue("@SuperviseMemo", m.SuperviseMemo);
                
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('SuperviseFill') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);

                m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());
                int state = AddRecordCommittes(m);
                if (state == -1)
                {
                    db.TransactionRollback();
                    return -1;
                }

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupervisePhaseService.AddRecords: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
        //更新督導紀錄
        public int UpdateRecords(SuperviseFillVModel m)
        {
            Null2Empty(m);
            string sql = @"
            update SuperviseFill set 
                MissingNo = @MissingNo,
                MissingLoc = @MissingLoc,
                DeductPointStr = @DeductPointStr,
                DeductPoint = @DeductPoint,
                SuperviseMemo = @SuperviseMemo,
                ModifyTime = GetDate(),
                ModifyUserSeq = @ModifyUserSeq
            where Seq=@Seq";

            db.BeginTransaction();
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@MissingNo", m.MissingNo);
                cmd.Parameters.AddWithValue("@MissingLoc", m.MissingLoc);
                cmd.Parameters.AddWithValue("@DeductPointStr", m.DeductPointStr);
                cmd.Parameters.AddWithValue("@DeductPoint", m.DeductPoint);
                cmd.Parameters.AddWithValue("@SuperviseMemo", m.SuperviseMemo);

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                sql = @"delete from SuperviseFillInsideCommittee where SuperviseFillSeq=@SuperviseFillSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@SuperviseFillSeq", m.Seq);
                db.ExecuteNonQuery(cmd);

                sql = @"delete from SuperviseFillOutCommittee where SuperviseFillSeq=@SuperviseFillSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@SuperviseFillSeq", m.Seq);
                db.ExecuteNonQuery(cmd);

                int state = AddRecordCommittes(m);
                if(state ==-1)
                {
                    db.TransactionRollback();
                    return -1;
                }

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupervisePhaseService.UpdateRecords: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
        //加入督導委員
        public int AddRecordCommittes(SuperviseFillVModel m)
        {
            foreach(String item in m.committeeList)
            {
                string[] kv = item.Split('-');
                if(kv.Length==2)
                {
                    string sql = null;
                    if (kv[0] == "0" )
                    {
                        sql = @"
                        insert into SuperviseFillInsideCommittee(
                            SuperviseFillSeq,
                            InsideCommitteeSeq,
                            CreateTime,
                            CreateUserSeq,
                            ModifyTime,
                            ModifyUserSeq
                        )values(
                            @SuperviseFillSeq,
                            @CommitteeSeq,
                            GetDate(),
                            @ModifyUserSeq,
                            GetDate(),
                            @ModifyUserSeq
                        )
                        ";
                    }
                    else if (kv[0] == "1")
                    {
                        sql = @"
                        insert into SuperviseFillOutCommittee(
                            SuperviseFillSeq,
                            OutCommitteeSeq,
                            CreateTime,
                            CreateUserSeq,
                            ModifyTime,
                            ModifyUserSeq
                        )values(
                            @SuperviseFillSeq,
                            @CommitteeSeq,
                            GetDate(),
                            @ModifyUserSeq,
                            GetDate(),
                            @ModifyUserSeq
                        )
                        ";
                    }
                    else if (kv[0] == "2")
                    {
                        kv[1] = "-" + kv[1];
                        sql = @"
                        insert into SuperviseFillInsideCommittee(
                            SuperviseFillSeq,
                            InsideCommitteeSeq,
                            CreateTime,
                            CreateUserSeq,
                            ModifyTime,
                            ModifyUserSeq
                        )values(
                            @SuperviseFillSeq,
                            @CommitteeSeq,
                            GetDate(),
                            @ModifyUserSeq,
                            GetDate(),
                            @ModifyUserSeq
                        )
                        ";
                    }
                    if (sql == null) return -1;
                    try
                    {
                        SqlCommand cmd = db.GetCommand(sql);
                        cmd.Parameters.AddWithValue("@SuperviseFillSeq", m.Seq);
                        cmd.Parameters.AddWithValue("@CommitteeSeq", kv[1]);

                        cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                        db.ExecuteNonQuery(cmd);
                    }
                    catch (Exception e)
                    {
                        log.Info("SupervisePhaseService.AddRecordCommittes: " + e.Message);
                        log.Info(sql);
                        return -1;
                    }
                }
            }
            return 0;
        }
        //刪除督導紀錄
        public int DelRecord(int seq)
        {
            string sql="";
            db.BeginTransaction();
            try
            {
                sql = @"delete from SuperviseFillInsideCommittee where SuperviseFillSeq=@SuperviseFillSeq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@SuperviseFillSeq", seq);
                db.ExecuteNonQuery(cmd);

                sql = @"delete from SuperviseFillOutCommittee where SuperviseFillSeq=@SuperviseFillSeq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@SuperviseFillSeq", seq);
                db.ExecuteNonQuery(cmd);

                sql = @"delete from SuperviseFill where Seq=@Seq";
                cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupervisePhaseService.DelDelRecords: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
        //工程內外聘委員清單
        public List<T> GetEngCommittes<T>(int superviseEngSeq)
        {
            string sql = @"SELECT z.mode, z.Seq, z.CName, z.OrderNo
                from (
                select Cast(0 as int) mode,
                    a.OrderNo,
                    a.Seq,
	                b.DisplayName CName
                from InsideCommittee a
                inner join UserMain b on(b.Seq=a.UserMainSeq)
                where a.SuperviseEngSeq=@SuperviseEngSeq
                union all

                select Cast(1 as int) mode,
                    a.OrderNo,
                    a.Seq,
	                b.ECName CName
                from OutCommittee a
                inner join ExpertCommittee b on(b.Seq=a.ExpertCommitteeSeq)
                where a.SuperviseEngSeq=@SuperviseEngSeq    
                union all
                select Cast(2 as int) mode,
                    0 OrderNo,
                    b.Seq,
	                b.DisplayName CName
                from SuperviseEng a
                inner join UserMain b on(b.Seq=a.LeaderSeq)
                where a.Seq=@SuperviseEngSeq   
                ) z
                order by z.mode,z.OrderNo
				";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SuperviseEngSeq", superviseEngSeq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //取得工程
        public List<T> GetEngForSuperviseFill<T>(int seq)
        {
            var userInfo = Utils.getUserInfo();
            string sql = @"SELECT
                    b.Seq,
                    a.TenderNo EngNo,
                    a.TenderName EngName,
                    c.PhaseCode,
                    b.CommitteeAverageScore,
                    b.Inspect
                FROM SuperviseEng b
                inner join PrjXML a on(a.Seq=b.PrjXMLSeq)
                inner join SupervisePhase c on(c.Seq=b.SupervisePhaseSeq)
                left join UserMain d on(d.Seq=b.LeaderSeq)
                where b.Seq=@Seq"
                + Utils.getAuthoritySql("a.")
                + @"
                    or 
                    ( b.Seq=@Seq and @userSeq = d.Seq )
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@Seq", seq);
            cmd.Parameters.AddWithValue("@userSeq", userInfo.Seq);

            return db.GetDataTableWithClass<T>(cmd);
        }

        //查詢督導期別
        public List<SupervisePhaseModel> GetPhaseCode(string phaseCode)
        {
            string sql = @"
                SELECT Seq, PhaseCode FROM SupervisePhase
                where PhaseCode=@PhaseCode ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@PhaseCode", phaseCode);
            return db.GetDataTableWithClass<SupervisePhaseModel>(cmd);
        }
        //期別工程清單
        public int GetPhaseEngList1Count(int supervisePhaseSeq)
        {
            string sql = @"SELECT
                    count(a1.Seq) total
                FROM SuperviseEng a1
                inner join PrjXML a on(a.Seq=a1.PrjXMLSeq)
                where a1.SupervisePhaseSeq=@SupervisePhaseSeq"
                ;
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SupervisePhaseSeq", supervisePhaseSeq);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());

            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetPhaseEngList1<T>(int supervisePhaseSeq, int pageRecordCount, int pageIndex)
        {
            string sql = @"SELECT
                    a1.Seq,
                    a1.PrjXMLSeq,
                    a.TenderNo EngNo,
                    a.TenderName EngName,
                    a.SupervisionUnitName,
                    a.DesignUnitName,
                    a.ExecUnitName ExecUnit,
                    NULLIF(
                        (select top 1 b.PDExecState from ProgressData b
                        where PrjXMLSeq=a.Seq
                        order by b.PDYear DESC, b.PDMonth DESC), '') ExecState -- 執行進度
                FROM SuperviseEng a1
                inner join PrjXML a on(a.Seq=a1.PrjXMLSeq)
                where a1.SupervisePhaseSeq=@SupervisePhaseSeq
                order by a.TenderNo Desc
				OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@SupervisePhaseSeq", supervisePhaseSeq);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            cmd.Parameters.AddWithValue("@CreateUserSeq", getUserSeq());

            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}