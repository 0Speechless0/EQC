using EQC.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class ECAskPaymentService : BaseService
    {//估驗請款
        public List<T> GetAskPaymentList<T>(int seq)
        {
            string sql = @"
                select
	                a.OrderNo, a.PayItem, a.Description, a.Unit, a.Price, a.Amount, a.ItemType,
	                a.AccuQuantity, a.AccuAmount, a.SchProgress
                    ,(
    	                select sum(ISNULL(b.AccuAmount,0)) from EC_AskPaymentPayItem b
                        where b.EC_AskPaymentHeaderSeq=@Seq
                        and b.PayItem not like '%==='
                        and b.PayItem like a.PayItem+',%'
                    )  subTotalAmount
                    ,(
    	                select count(b.AccuAmount) from EC_AskPaymentPayItem b
                        where b.EC_AskPaymentHeaderSeq=@Seq
                        and b.PayItem not like '%==='
                        and b.PayItem like a.PayItem+',%'
                    )  subCount
                    ,(SELECT count(value) FROM STRING_SPLIT(a.PayItem, ',')) level
                from EC_AskPaymentPayItem a
                where a.EC_AskPaymentHeaderSeq=@Seq
                and a.PayItem not like '%==='
                order by a.OrderNo
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        public List<T> GetProphaseAskPaymentList<T>(int engMainSeq, int seq)
        {
            string sql = @"
                select
	                ISNULL(c.AccuQuantity,0) AccuQuantity, ISNULL(c.AccuAmount,0)AccuAmount,
                    a.OrderNo, a.PayItem, a.Description, a.Unit, a.Price, a.Amount, a.ItemType, a.SchProgress
                    --a.AccuQuantity, a.AccuAmount, 
                    ,(
                        select sum(ISNULL(b.AccuAmount,0)) from EC_AskPaymentPayItem b
                        where b.EC_AskPaymentHeaderSeq=@Seq
                        and b.PayItem not like '%==='
                        and b.PayItem like a.PayItem+',%'
                    )  subTotalAmount
                    ,(
                        select count(b.AccuAmount) from EC_AskPaymentPayItem b
                        where b.EC_AskPaymentHeaderSeq=@Seq
                        and b.PayItem not like '%==='
                        and b.PayItem like a.PayItem+',%'
                    )  subCount
                    ,(SELECT count(value) FROM STRING_SPLIT(a.PayItem, ',')) level
                from EC_AskPaymentPayItem a
                inner join EC_SchEngProgressPayItem b on(b.Seq=a.RootSeq)
                left outer join (
                  select
                      zb.PayItem, zb.Description, zb.Unit, zb.Price, zb.AccuQuantity, zb.AccuAmount
                  from AskPaymentHeader za
                  inner join AskPaymentPayItem zb on(zb.AskPaymentHeaderSeq=za.Seq and zb.ItemType=0)
                  where za.EngMainSeq=@EngMainSeq
                  and za.APDate=(select max(APDate) from AskPaymentHeader where EngMainSeq=@EngMainSeq)
                ) c ON (
	                c.PayItem=b.PayItem and c.Description=b.Description and c.Unit=b.Unit and c.Price=b.Price
                )
                where a.EC_AskPaymentHeaderSeq=@Seq
                and a.PayItem not like '%==='
                order by a.OrderNo
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

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
                from EC_AskPaymentHeader
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
                from EC_AskPaymentHeader
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
                from EC_AskPaymentHeader a
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
                from EC_AskPaymentHeader a
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
                select top 1 Seq, ItemDate from EC_SupDailyDate
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
        public bool AddItems(int engMainSeq, DateTime tarDate, EC_SupDailyDateModel supDailyDate)
        {
            string sql;
            db.BeginTransaction();
            try
            {
                sql = @"
                    insert into EC_AskPaymentHeader (
                        EngMainSeq,
                        APDate,
                        Period,
                        SupDailyDate,
                        CreateUserSeq,
                        ModifyUserSeq
                    ) values (
                        @EngMainSeq,
                        @APDate,
                        (select ISNULL(max(Period),0)+1 from EC_AskPaymentHeader where EngMainSeq=@EngMainSeq),
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
                string sql1 = @"SELECT IDENT_CURRENT('EC_AskPaymentHeader') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dTable = db.GetDataTable(cmd);
                int EC_AskPaymentHeaderSeq = Convert.ToInt32(dTable.Rows[0]["NewSeq"].ToString());

                sql = @"
                    insert into EC_AskPaymentPayItem(
                        EC_AskPaymentHeaderSeq,
                        RootSeq,
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
                        @EC_AskPaymentHeaderSeq,
	                    zz.RootSeq,
                        zz.PayItem,
                        zz.Description,
                        zz.Unit,
                        zz.Quantity,
                        zz.Price,
                        zz.Amount,
                        zz.ItemKey,
                        zz.ItemNo,
                        zz.RefItemCode,
                        zz.OrderNo,
                        zz.ItemType,
                        zz.AccuQuantity,
                        CAST(zz.AccuQuantity * zz.Price as decimal(20, 0)) AccuAmount,
                        zz.SchProgress,
                        @ModifyUserSeq,
                        @ModifyUserSeq
                    from (
                    select
	                    b1.RootSeq, b1.PayItem, b1.Description, b1.Unit, b1.Quantity, b1.Price, b1.Amount, 
                        b1.ItemKey, b1.ItemNo, b1.RefItemCode, b1.OrderNo,
                        Cast(IIF(a.DayProgress=-1, -1, 0) as int) ItemType,
                        (
                            ISNULL((
                                SELECT sum(zb.TodayConfirm) FROM EC_SupDailyDate za
                                inner join EC_SupPlanOverview zb on(zb.EC_SupDailyDateSeq=za.Seq and zb.TodayConfirm>0 )
                                inner join EC_SchEngProgressPayItem zb1 on(zb1.Seq=zb.EC_SchEngProgressPayItemSeq and zb1.RootSeq=b1.RootSeq)
                                where za.EngMainSeq=b.EngMainSeq
                                and za.DataType=1
                                and za.ItemDate<=@tarDate
                            ),0)
                             +
                            ISNULL((
                                SELECT sum(zb.TodayConfirm) FROM SupDailyDate za
                                inner join SupPlanOverview zb on(zb.SupDailyDateSeq=za.Seq and zb.TodayConfirm>0 )
                                inner join SchEngProgressPayItem zb1 on(zb1.Seq=zb.SchEngProgressPayItemSeq and zb1.Seq=b1.ParentSchEngProgressPayItemSeq)
                                where za.EngMainSeq=b.EngMainSeq
                                and za.DataType=1
                            ),0) 
                        ) AccuQuantity, 
                        (
                            a.SchProgress - a.DayProgress * DATEDIFF(Day, IIF(@tarDate>a.SPDate, a.SPDate, @tarDate), a.SPDate)
                        ) SchProgress --預定進度 %
                    from EC_SchEngProgressHeader b
                    inner join EC_SchEngProgressPayItem b1 on(b1.EC_SchEngProgressHeaderSeq=b.Seq)
                    inner join EC_SchProgressPayItem a on(a.EC_SchEngProgressPayItemSeq=b1.Seq)
                    where b.EngMainSeq=@engMainSeq
                    and (b.StartDate<=@tarDate and (b.EndDate is null or b.EndDate>=@tarDate)) --s20230527
                    and a.SPDate = (
                        select top 1 z1.SPDate from (
                          select top 1 z.SPDate from EC_SchEngProgressHeader z2
                          inner join EC_SchEngProgressPayItem z1 on(z1.EC_SchEngProgressHeaderSeq=z2.Seq)
                          inner join EC_SchProgressPayItem z on(z.EC_SchEngProgressPayItemSeq=z1.Seq)
                          where z2.EngMainSeq=@EngMainSeq
                          and z.SPDate >= @tarDate
                          order by z.SPDate
          
                          union all
          
                          select top 1 z.SPDate from EC_SchEngProgressHeader z2
                          inner join EC_SchEngProgressPayItem z1 on(z1.EC_SchEngProgressHeaderSeq=z2.Seq)
                          inner join EC_SchProgressPayItem z on(z.EC_SchEngProgressPayItemSeq=z1.Seq)
                          where z2.EngMainSeq=@EngMainSeq
                          and z.SPDate < @tarDate
                          order by z.SPDate desc
                        ) z1
                        order by z1.SPDate desc
                    )
                    ) zz
                    order by zz.OrderNo
                ";

                cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EC_AskPaymentHeaderSeq", EC_AskPaymentHeaderSeq);
                cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                cmd.Parameters.AddWithValue("@tarDate", tarDate);
                //cmd.Parameters.AddWithValue("@SupDailyDateSeq", supDailyDate.Seq);
                //cmd.Parameters.AddWithValue("@SupDailyDate", supDailyDate.ItemDate);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("ECAskPaymentService.AddItems: " + e.Message);
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
                from EC_AskPaymentPayItem a
                where a.EC_AskPaymentHeaderSeq=@EC_AskPaymentHeaderSeq
                Order by a.OrderNo
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EC_AskPaymentHeaderSeq", seq);

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
                    delete from EC_AskPaymentPayItem where EC_AskPaymentHeaderSeq=(select Seq from EC_AskPaymentHeader where Seq=@Seq and APState=0);

                    delete from EC_AskPaymentHeader where Seq=@Seq and APState=0;
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
                log.Info("ECAskPaymentService.Del: " + e.Message);
                return false;
            }
}
        //完成請款
        public int APCompleted(int seq, int state)
        {
            string sql = @"
                update EC_AskPaymentHeader set
                APState=@SPState,
                CurrentAccuAmount=(
                    select sum(AccuAmount) from EC_AskPaymentPayItem where EC_AskPaymentHeaderSeq=@Seq and ItemType>-1
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
        public bool UpdateAccu(List<EC_AskPaymentPayItemModel> planItems)
        {
            string sql = "";
            SqlCommand cmd;
            db.BeginTransaction();
            try
            {
                //依施工計畫執行按圖施工概況
                sql = @"update EC_AskPaymentPayItem set
                        AccuQuantity=@AccuQuantity,
                        AccuAmount=@AccuAmount,
                        Memo=@Memo,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq";
                foreach (EC_AskPaymentPayItemModel m in planItems)
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
                log.Info("ECAskPaymentService.Accu:" + e.Message);
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