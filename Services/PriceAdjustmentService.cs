using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class PriceAdjustmentService : BaseService
    {//物價調整款
        //工程決標日期 shioulo20221228
        public List<T> GetEngMainBySeq<T>(int seq)
        {
            string sql = @"
                SELECT
                    a.Seq,
                    ISNULL(a.AwardDate, dbo.ChtDate2Date(e.ActualBidAwardDate)) AwardDate,
                    a.AwardDate srcAwardDate
                FROM EngMain a
                inner join PrjXML e on(e.Seq=a.PrjXMLSeq)
                where a.Seq=@Seq
            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Seq", seq);
            return db.GetDataTableWithClass<T>(cmd);
        }
        //更新工程決標日期 shioulo20221228
        public int UpdateEngAwardDate(EPCPriceAdjustmentVModel m)
        {
            Null2Empty(m);
            string sql = @"
                update EngMain set
                    AwardDate = @AwardDate,
                    ModifyTime = GETDATE(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq
            ";
            try { 
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@AwardDate", this.NulltoDBNull(m.AwardDate));
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                return db.ExecuteNonQuery(cmd);
            }
            catch (Exception e)
            {
                log.Info("PriceAdjustmentService.UpdateEngAwardDate: " + e.Message);
                return -1;
            }
}
        //
        public bool UpdateEngPriceAdjLockWorkItem(List<EngPriceAdjLockWorkItemModel> items)
        {
            SqlCommand cmd;
            string sql = @"
                    update EngPriceAdjLockWorkItem set
                        PriceIndexKindId=@PriceIndexKindId,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq
                 ";
            db.BeginTransaction();
            try
            {
                foreach (EngPriceAdjLockWorkItemModel m in items)
                {
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", m.Seq);
                    cmd.Parameters.AddWithValue("@PriceIndexKindId", m.PriceIndexKindId);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("PriceAdjustmentService.UpdateEngPriceAdjLockWorkItem: " + e.Message);
                return false;
            }
        }
        //所有M Item
        public List<T> GetEngPriceAdjLockWorkItem<T>(int engMainSeq)
        {
            string sql = @"
                select
                    a.Seq,
                    a.SchEngProgressWorkItemSeq,
                    a.Kind,
                    a1.PayItem,
                    a1.Description,
                    b.ItemCode,
                    a.Weights,
                    a.Amount,
                    a.PriceIndexKindId
                from EngPriceAdjLockWorkItem a
                inner join SchEngProgressWorkItem b on(b.Seq=a.SchEngProgressWorkItemSeq)
                inner join SchEngProgressPayItem a1 on(a1.Seq=b.SchEngProgressPayItemSeq)
                where a.EngMainSeq <= @EngMainSeq
                order by a1.OrderNo
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //
        public List<T> GetPriceIndex<T>(int priceIndexKindId, string tarDate)
        {
            string sql = @"
                select top 2 a.PIDate, a.PriceIndex
                from PriceIndexItems a
                where a.PriceIndexKindId=@PriceIndexKindId
                and a.PIDate <= @TarDate
                order by a.PIDate desc
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@PriceIndexKindId", priceIndexKindId);
            cmd.Parameters.AddWithValue("@TarDate", tarDate);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //建立 WorkItem
        public bool UpdateEngPriceAdjWorkItem(EngPriceAdjWorkItemGroupVModel gModel)
        {
            SqlCommand cmd;
            string sql = @"
                    Update EngPriceAdjWorkItem set
                        PriceIndex=@PriceIndex,
                        PriceAdjustment=@PriceAdjustment,
                        AdjKind=@AdjKind,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq
                 ";
            db.BeginTransaction();
            try
            {
                List<EngPriceAdjWorkItemVModel> allItem = new List<EngPriceAdjWorkItemVModel>();
                foreach (EngPriceAdjWorkItemVModel m in gModel.M03310) { allItem.Add(m); }
                foreach (EngPriceAdjWorkItemVModel m in gModel.M03210) { allItem.Add(m); }
                foreach (EngPriceAdjWorkItemVModel m in gModel.M02742) { allItem.Add(m); }
                foreach (EngPriceAdjWorkItemVModel m in gModel.M03) { allItem.Add(m); }
                foreach (EngPriceAdjWorkItemVModel m in gModel.M05) { allItem.Add(m); }
                foreach (EngPriceAdjWorkItemVModel m in gModel.M02319) { allItem.Add(m); }
                foreach (EngPriceAdjWorkItemVModel m in gModel.M027_0296) { allItem.Add(m); }
                foreach (EngPriceAdjWorkItemVModel m in gModel.Mxxx) { allItem.Add(m); }

                foreach (EngPriceAdjWorkItemVModel m in allItem)
                {
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@Seq", m.Seq);
                    cmd.Parameters.AddWithValue("@PriceIndex", m.PriceIndex);
                    cmd.Parameters.AddWithValue("@PriceAdjustment", m.PriceAdjustment);
                    cmd.Parameters.AddWithValue("@AdjKind", m.AdjKind);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("PriceAdjustmentService.UpdateEngPriceAdjWorkItem: " + e.Message);
                return false;
            }
        }
        //建立 WorkItem
        public bool InitEngPriceAdjWorkItem(EngPriceAdjModel item)
        {
            try
            {
                string sql = @"
                    insert into EngPriceAdjWorkItem(
                        EngPriceAdjSeq, EngPriceAdjLockWorkItemSeq, MonthQuantity, CreateUserSeq, ModifyUserSeq
                    )
                    select
	                    b.Seq EngPriceAdjSeq, c.Seq EngPriceAdjLockWorkItemSeq, ISNULL(d.MonthQuantity, 0), @ModifyUserSeq, @ModifyUserSeq
                    from EngPriceAdj b
                    inner join EngPriceAdjLockWorkItem c on(c.EngMainSeq=b.EngMainSeq)
                    inner join SchEngProgressWorkItem c1 on(c1.Seq=c.SchEngProgressWorkItemSeq)
                    left outer join (
                        select
                            b.SchEngProgressPayItemSeq, sum(ISNULL(b.TodayConfirm, 0)) MonthQuantity
                        from SupDailyDate a
                        inner join SupPlanOverview b on(b.SupDailyDateSeq=a.Seq)
                        inner join SchEngProgressPayItem b1 on(b1.Seq=b.SchEngProgressPayItemSeq)
                        where a.EngMainSeq=@EngMainSeq and a.DataType=1
                        and a.ItemDate>=@StartDate and a.ItemDate<@EndDate
                        group by b.SchEngProgressPayItemSeq --group by b1.PayItem
                    ) d on (d.SchEngProgressPayItemSeq=c1.SchEngProgressPayItemSeq) --) d on (d.PayItem=c.PayItem)
                    where b.Seq=@EngPriceAdjSeq
                 ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngPriceAdjSeq", item.Seq);
                cmd.Parameters.AddWithValue("@StartDate", item.AdjMonth);
                cmd.Parameters.AddWithValue("@EndDate", item.AdjMonth.AddMonths(1));
                cmd.Parameters.AddWithValue("@EngMainSeq", item.EngMainSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return true;
            }
            catch (Exception e)
            {
                log.Info("PriceAdjustmentService.initEngPriceAdjWorkItem: " + e.Message);
                return false;
            }
        }

        //
        public List<T> GetEngPriceAdj<T>(int engMainSeq, string adjMonth)
        {
            string sql = @"
                select
                    Seq, EngMainSeq, AdjMonth
                from EngPriceAdj
                where EngMainSeq=@EngMainSeq
                and AdjMonth=@AdjMonth
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@AdjMonth", adjMonth);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //刪除該月物調款資料
        public bool DelEngPriceAdjWorkItem(int engMainSeq, string adjMonth)
        {
            string sql = @"
                delete EngPriceAdjWorkItem
                where EngPriceAdjSeq in(
                    select Seq from EngPriceAdj where EngMainSeq=@EngMainSeq and AdjMonth=@AdjMonth
                )
                ";
            try { 
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                cmd.Parameters.AddWithValue("@AdjMonth", adjMonth);
                db.ExecuteNonQuery(cmd);

                return true;
            }
            catch (Exception e)
            {
                log.Info("PriceAdjustmentService.DelEngPriceAdjWorkItem: " + e.Message);
                //log.Info(sql);
                return false;
            }
}
        //物價調整款清單
        public List<T> GetList<T>(int engMainSeq, string adjMonth)
        {
            string sql = @"
                select
	                c.Kind, a.Seq, a.AdjKind,
                    c2.PayItem, c2.Description, c1.ItemCode
                    ,c.Amount, c.Weights, c.PriceIndexKindId,
                    a.MonthQuantity,
                    (a.MonthQuantity*c.Amount) WorkAmount,
                    (a.MonthQuantity*c.Amount*c.Weights) WeightsAmount,
                    a.PriceIndex, a.PriceAdjustment
                from EngPriceAdjWorkItem a
                inner join EngPriceAdj b on(b.Seq=a.EngPriceAdjSeq)
                inner join EngPriceAdjLockWorkItem c on(c.Seq=a.EngPriceAdjLockWorkItemSeq)
                inner join SchEngProgressWorkItem c1 on(c1.Seq=c.SchEngProgressWorkItemSeq)
                inner join SchEngProgressPayItem c2 on(c2.Seq=c1.SchEngProgressPayItemSeq)
                where b.EngMainSeq=@EngMainSeq
                and b.AdjMonth=@AdjMonth
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            cmd.Parameters.AddWithValue("@AdjMonth", adjMonth);

            return db.GetDataTableWithClass<T>(cmd);
        }

        //日期清單
        public List<T> GetDateList<T>(int engMainSeq)
        {
            string sql = @"
                select a.AdjMonth
                from EngPriceAdj a
                where a.EngMainSeq=@EngMainSeq
                Order by a.AdjMonth
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //建立初始清單
        public bool initDateList(int engMainSeq, List<DateTime> dateList)
        {
            List<EngPriceAdjLockWorkItemModel> wItems = GetWorkItemBaseData(engMainSeq);
            string sql;
            SqlCommand cmd;

            db.BeginTransaction();
            try
            {
                sql = @"
                    insert into EngPriceAdj (
                        EngMainSeq,
                        AdjMonth,
                        CreateUserSeq
                    ) values (
                        @EngMainSeq,
                        @AdjMonth,
                        @ModifyUserSeq
                    )";
                foreach (DateTime dt in dateList)
                {
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                    cmd.Parameters.AddWithValue("@AdjMonth", dt);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                sql = @"insert into EngPriceAdjLockWorkItem(
                        EngMainSeq,
                        SchEngProgressWorkItemSeq,
                        Kind,
                        /*PayItem,
                        Description,
                        ItemCode,*/
                        Weights,
                        Amount,
                        CreateUserSeq
                    )values(
                        @EngMainSeq,
                        @SchEngProgressWorkItemSeq,
                        @Kind,
                        /*@PayItem,
                        @Description,
                        @ItemCode,*/
                        @Weights,
                        @Amount,
                        @ModifyUserSeq
                    )";
                foreach (EngPriceAdjLockWorkItemModel m in wItems)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                    cmd.Parameters.AddWithValue("@SchEngProgressWorkItemSeq", m.SchEngProgressWorkItemSeq);
                    cmd.Parameters.AddWithValue("@Kind", m.Kind);
                    /*cmd.Parameters.AddWithValue("@PayItem", m.PayItem);
                    cmd.Parameters.AddWithValue("@Description", m.Description);
                    cmd.Parameters.AddWithValue("@ItemCode", m.ItemCode);*/
                    cmd.Parameters.AddWithValue("@Weights", m.Weights);
                    cmd.Parameters.AddWithValue("@Amount", m.Amount);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", this.getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("PriceAdjustmentService.initDateList: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }
        //M WorkItem 清單
        public List<EngPriceAdjLockWorkItemModel> GetWorkItemBaseData(int engMainSeq)
        {
            string sql = @"
                select z1.Seq SchEngProgressWorkItemSeq, z1.Kind
	                ,c.PayItem, b.Description, b.ItemCode
                    ,(b.Amount) / (c.Amount) Weights
	                ,b.Amount
                from (
                    select MIN(z.kind) kind, z.Seq
                    from (
    
                        select 101 kind, a.Seq from SchEngProgressWorkItem a
                        where a.SchEngProgressPayItemSeq in(
                            select Seq from SchEngProgressPayItem where SchEngProgressHeaderSeq in (
                                select Seq from SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M03310%'

                        union all

                        select 102 kind, a.Seq from SchEngProgressWorkItem a
                        where a.SchEngProgressPayItemSeq in(
                            select Seq from SchEngProgressPayItem where SchEngProgressHeaderSeq in (
                                select Seq from SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M03210%'

                        union all

                        select 103 kind, a.Seq from SchEngProgressWorkItem a
                        where a.SchEngProgressPayItemSeq in(
                            select Seq from SchEngProgressPayItem where SchEngProgressHeaderSeq in (
                                select Seq from SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M02742%'

                        union all

                        select 201 kind, a.Seq from SchEngProgressWorkItem a
                        where a.SchEngProgressPayItemSeq in(
                            select Seq from SchEngProgressPayItem where SchEngProgressHeaderSeq in (
                                select Seq from SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M03%'

                        union all

                        select 202 kind, a.Seq from SchEngProgressWorkItem a
                        where a.SchEngProgressPayItemSeq in(
                            select Seq from SchEngProgressPayItem where SchEngProgressHeaderSeq in (
                                select Seq from SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M05%'

                        union all

                        select 203 kind, a.Seq from SchEngProgressWorkItem a
                        where a.SchEngProgressPayItemSeq in(
                            select Seq from SchEngProgressPayItem where SchEngProgressHeaderSeq in (
                                select Seq from SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M2319%'

                        union all

                        select 204 kind, a.Seq from SchEngProgressWorkItem a
                        where a.SchEngProgressPayItemSeq in(
                            select Seq from SchEngProgressPayItem where SchEngProgressHeaderSeq in (
                                select Seq from SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and (a.ItemCode like 'M027%' or a.ItemCode like 'M0296%')

                        union all

                        select 999 kind, a.Seq from SchEngProgressWorkItem a
                        where a.SchEngProgressPayItemSeq in(
                            select Seq from SchEngProgressPayItem where SchEngProgressHeaderSeq in (
                                select Seq from SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M%'
                    ) z
                    group by z.Seq
                ) z1
                inner join SchEngProgressWorkItem b on(b.Seq=z1.Seq)
                inner join SchEngProgressPayItem c on(c.Seq=b.SchEngProgressPayItemSeq)
                order by z1.kind, c.PayItem, b.ItemCode
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<EngPriceAdjLockWorkItemModel>(cmd);
        }
        //增加日期清單
        public bool AddDateList(int engMainSeq, List<DateTime> dateList)
        {
            string sql;
            SqlCommand cmd;

            db.BeginTransaction();
            try
            {
                sql = @"
                    insert into EngPriceAdj (
                        EngMainSeq,
                        AdjMonth,
                        CreateUserSeq
                    ) values (
                        @EngMainSeq,
                        @AdjMonth,
                        @ModifyUserSeq
                    )";
                foreach (DateTime dt in dateList)
                {
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                    cmd.Parameters.AddWithValue("@AdjMonth", dt);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                db.TransactionCommit();
                return true;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("PriceAdjustmentService.AddDateList: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }
        //監造日誌最大日期
        public List<T> GetMaxSupDailyDate<T>(int engMainSeq)
        {
            string sql = @"
                select * from (
                    select max(ItemDate) ItemDate from SupDailyDate
                    where EngMainSeq=@EngMainSeq and DataType=1
                ) z
                where z.ItemDate is not null
             ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
    }
}