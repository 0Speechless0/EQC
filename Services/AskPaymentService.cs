using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class AskPaymentService : BaseService
    {//估驗請款
        public List<T> GetAskPaymentList<T>(int seq)
        {
            string sql = @"
                select
	                a.OrderNo, a.PayItem, a.Description, a.Unit, a.Price, a.Amount, a.ItemType,
	                a.AccuQuantity, a.AccuAmount, a.SchProgress
                    ,(
    	                select sum(ISNULL(b.AccuAmount,0)) from AskPaymentPayItem b
                        where b.AskPaymentHeaderSeq=@Seq
                        and b.PayItem not like '%==='
                        and b.PayItem like a.PayItem+',%'
                    )  subTotalAmount
                    ,(
    	                select count(b.AccuAmount) from AskPaymentPayItem b
                        where b.AskPaymentHeaderSeq=@Seq
                        and b.PayItem not like '%==='
                        and b.PayItem like a.PayItem+',%'
                    )  subCount
                    ,(SELECT count(value) FROM STRING_SPLIT(a.PayItem, ',')) level
                from AskPaymentPayItem a
                where a.AskPaymentHeaderSeq=@Seq
                and a.PayItem not like '%==='
                order by a.OrderNo
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetAskPaymentHeader<T>(int seq)
        {
            string sql = @"
                select
                    Seq,
                    EngMainSeq,
                    APDate,
                    APState,
                    CurrentAccuAmount,
                    Period
                from AskPaymentHeader
                where Seq=@Seq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetAskPaymentHeader<T>(int engMainSeq, int period)
        {
            string sql = @"
                select
                    Seq,
                    EngMainSeq,
                    APDate,
                    APState,
                    CurrentAccuAmount,
                    Period
                from AskPaymentHeader
                where EngMainSeq=@EngMainSeq
                and Period=@Period
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@Period", period);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //日期清單
        public List<T> GetDateList<T>(int engMainSeq)
        {
            string sql = @"
                select a.Seq, a.APDate, a.APState
                from AskPaymentHeader a
                where a.EngMainSeq=@EngMainSeq
                Order by a.APDate desc
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> CheckDate<T>(int engMainSeq, DateTime date)
        {
            string sql = @"
                select a.Seq, a.APDate, a.APState
                from AskPaymentHeader a
                where a.EngMainSeq=@EngMainSeq
                and a.APDate>=@APDate
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@APDate", date);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //檢查監造日誌
        public List<T> CheckDailyDate<T>(int engMainSeq, DateTime date)
        {
            string sql = @"
                select top 1 Seq, ItemDate from SupDailyDate
                where EngMainSeq=@EngMainSeq
                and DataType=1        
                and ItemDate<@APDate
                order by ItemDate desc
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@APDate", date);
            return db.GetDataTableWithClass<T>(cmd);
        }

        //建立請款清單
        public bool AddItems(int engMainSeq, DateTime tarDate, SupDailyDateModel supDailyDate)
        {
            string sql;
            db.BeginTransaction();
            try
            {
                sql = @"
                    insert into AskPaymentHeader (
                        EngMainSeq,
                        APDate,
                        Period,
                        SupDailyDate,
                        CreateUserSeq,
                        ModifyUserSeq
                    ) values (
                        @EngMainSeq,
                        @APDate,
                        (select ISNULL(max(Period),0)+1 from AskPaymentHeader where EngMainSeq=@EngMainSeq),
                        @SupDailyDate,
                        @ModifyUserSeq,
                        @ModifyUserSeq
                    )";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                cmd.Parameters.AddWithValue("@APDate", tarDate);
                cmd.Parameters.AddWithValue("@SupDailyDate", supDailyDate.ItemDate);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('AskPaymentHeader') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dTable = db.GetDataTable(cmd);
                int askPaymentHeaderSeq = Convert.ToInt32(dTable.Rows[0]["NewSeq"].ToString());

                sql = @"
                    insert into AskPaymentPayItem(
                        AskPaymentHeaderSeq,
                        PayItem,
                        Description,
                        Unit,
                        Quantity,
                        Price,
                        Amount,
                        ItemKey,
                        ItemNo,
                        RefItemCode,
                        OrderNo,
                        ItemType,
                        AccuQuantity,
                        AccuAmount,
                        SchProgress,
                        CreateUserSeq,
                        ModifyUserSeq
                    )
                    select
                        @AskPaymentHeaderSeq,
                        z11.PayItem,
                        z11.Description,
                        z11.Unit,
                        z11.Quantity,
                        z11.Price,
                        z11.Amount,
                        z11.ItemKey,
                        z11.ItemNo,
                        z11.RefItemCode,
                        z11.OrderNo,
                        Cast(IIF(z1.DayProgress=-1, -1, 0) as int) ItemType,
                        CAST(z1.TodayConfirm + ISNULL(z2.TotalAccConfirm, 0) as decimal(20, 4)) AccuQuantity,
                        CAST((z1.TodayConfirm + ISNULL(z2.TotalAccConfirm, 0)) * z11.Price as decimal(20, 0)) AccuAmount,
                        z3.SchProgress,
                        @ModifyUserSeq,
                        @ModifyUserSeq
                    FROM SupPlanOverview z1
                    inner join SchEngProgressPayItem z11 on(z11.Seq=z1.SchEngProgressPayItemSeq)
                    inner join (
                	    SELECT b.SupDailyDateSeq, b.Seq, sum(c.TodayConfirm) TotalAccConfirm FROM SupDailyDate a
                        inner join SupPlanOverview b on(b.SupDailyDateSeq=a.Seq)
                        left outer join (
                            SELECT bb.SchEngProgressPayItemSeq, bb.TodayConfirm FROM SupDailyDate ba
                            inner join SupDailyDate ba1 on(ba1.EngMainSeq = ba.EngMainSeq AND ba1.ItemDate < ba.ItemDate AND ba1.DataType = ba.DataType)
                            inner join SupPlanOverview bb on(bb.SupDailyDateSeq=ba1.Seq)
                            where ba.Seq=@SupDailyDateSeq
                        ) c on (b.SchEngProgressPayItemSeq=c.SchEngProgressPayItemSeq)
                        where a.Seq=@SupDailyDateSeq
                        group by b.SupDailyDateSeq, b.Seq
                    ) z2 on (z2.SupDailyDateSeq=z1.SupDailyDateSeq and z2.Seq=z1.Seq)
                    inner join (
                            select DISTINCT
	                            a.SchEngProgressPayItemSeq,
                	            (
                    	            IIF(a.SchProgress=-1, 0, a.SchProgress) -  IIF(a.DayProgress=-1, 0, a.DayProgress)*DATEDIFF(Day, @SupDailyDate, a.SPDate)
                                ) SchProgress
                            from SchProgressHeader b
                            inner join SchProgressPayItem a on(a.SchProgressHeaderSeq=b.Seq)
                            where b.EngMainSeq=@EngMainSeq
                            and a.SPDate = (
                	            select top 1 z.SPDate from SchProgressPayItem z
                                where z.SchProgressHeaderSeq = b.Seq
                                and z.SPDate >= @SupDailyDate
                                order by z.SPDate
                            )
                            union ALL
                            select DISTINCT
                	            a.SchEngProgressPayItemSeq,
                	            (
                    	            IIF(a.SchProgress=-1, 0, a.SchProgress) -  IIF(a.DayProgress=-1, 0, a.DayProgressAfter)*DATEDIFF(Day, @SupDailyDate, a.SPDate)
                                ) SchProgress
                            from SchProgressHeader b
                            inner join SchProgressPayItem a on(a.SchProgressHeaderSeq=b.Seq)
                            where b.EngMainSeq=@EngMainSeq
                            and (
                                select count(z.Seq) from SchProgressPayItem z
                                where z.SchProgressHeaderSeq = b.Seq
                                and z.SPDate >= @SupDailyDate
                                ) = 0
                            and a.SPDate = (select max(z.SPDate) from SchProgressPayItem z where z.SchProgressHeaderSeq=b.Seq)
                    ) z3 on (z3.SchEngProgressPayItemSeq=z1.SchEngProgressPayItemSeq)
                ";

                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@AskPaymentHeaderSeq", askPaymentHeaderSeq);
                cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                //cmd.Parameters.AddWithValue("@APDate", tarDate);//20230424
                cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyDate.Seq);
                cmd.Parameters.AddWithValue("@SupDailyDate", supDailyDate.ItemDate);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("AskPaymentService.AddItems: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }
        //清單
        public List<T> GetList<T>(int seq)
        {
            string sql = @"
                select
                    a.Seq,
                    a.ItemType,
                    a.OrderNo,
                    a.PayItem,
                    a.Description,
                    a.Unit,
                    a.Quantity,
                    a.Price,
                    a.Amount,
                    a.ItemKey,
                    a.ItemNo,
                    a.RefItemCode,
                    a.AccuQuantity,
                    a.AccuAmount,
                    a.Memo
                from AskPaymentPayItem a
                where a.AskPaymentHeaderSeq=@AskPaymentHeaderSeq
                Order by a.OrderNo
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@AskPaymentHeaderSeq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //刪除
        public bool Del(int seq)
        {
            db.BeginTransaction();
            try
            {
                db.TransactionCommit();
                string sql = @"
                    delete from AskPaymentPayItem where AskPaymentHeaderSeq=(select Seq from AskPaymentHeader where Seq=@Seq and APState=0);

                    delete from AskPaymentHeader where Seq=@Seq and APState=0;
                ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@Seq", seq);
                int cnt = db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return (cnt > 0);
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("AskPaymentService.Del: " + e.Message);
                return false;
            }
}
        //完成請款
        public int APCompleted(int seq, int state)
        {
            string sql = @"
                update AskPaymentHeader set
                APState=@SPState,
                CurrentAccuAmount=(
                    select sum(AccuAmount) from AskPaymentPayItem where AskPaymentHeaderSeq=@Seq and ItemType>-1
                )
                where Seq=@Seq
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq",seq);
            cmd.Parameters.AddWithValue("@SPState", state);

            return db.ExecuteNonQuery(cmd);
        }
        //估驗請款更新
        public bool UpdateAccu(List<AskPaymentPayItemModel> planItems)
        {
            string sql = "";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                //依施工計畫執行按圖施工概況
                sql = @"update AskPaymentPayItem set
                        AccuQuantity=@AccuQuantity,
                        AccuAmount=@AccuAmount,
                        Memo=@Memo,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
                foreach (AskPaymentPayItemModel m in planItems)
                {
                    if (m.ItemType == -1) continue;
                    
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", m.Seq);
                    cmd.Parameters.AddWithValue("@AccuQuantity", m.AccuQuantity);
                    cmd.Parameters.AddWithValue("@AccuAmount", m.AccuAmount);
                    cmd.Parameters.AddWithValue("@Memo", m.Memo);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("SupDailyReportService.UpdateTodayConfirm:" + e.Message);
                log.Info(sql);
                return false;
            }
        }
        //
        public List<T> GetEngItem<T>(int seq)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    a.EngNo,
                    a.EngName,
                    a.PrjXMLSeq,
                    e.TenderNo,
                    e.TenderName,
                    e.Location,
                    e.ExecUnitName,
                    e.ContractorName1,
                    e.ContractNo,
                    e.BidAmount,
                    ext.BelongPrj,
                    ext.DesignChangeContractAmount
                FROM EngMain a
                inner join PrjXML e on(e.Seq=a.PrjXMLSeq)
                left outer join PrjXMLExt ext on(ext.PrjXMLSeq=a.PrjXMLSeq)
                where
                    a.Seq=@Seq
            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}