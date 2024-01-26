using EQC.Models;
using EQC.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace EQC.Services
{
    public class ECPriceAdjustmentService : BaseService
    {//工程變更-物價調整款
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
        public int UpdateEngAwardDate(ECPriceAdjustmentVModel m)
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
                log.Info("ECPriceAdjustmentService.UpdateEngAwardDate: " + e.Message);
                return -1;
            }
}
        //
        public bool UpdateEngPriceAdjLockWorkItem(List<EC_EngPriceAdjLockWorkItemModel> items)
        {
            SqlCommand cmd;
            string sql = @"
                    update EC_EngPriceAdjLockWorkItem set
                        PriceIndexKindId=@PriceIndexKindId,
                        ModifyTime=GetDate(),
                        ModifyUserSeq=@ModifyUserSeq
                    where Seq=@Seq
                 ";
            db.BeginTransaction();
            try
            {
                foreach (EC_EngPriceAdjLockWorkItemModel m in items)
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
                log.Info("ECPriceAdjustmentService.UpdateEngPriceAdjLockWorkItem: " + e.Message);
                return false;
            }
        }
        //所有M Item
        public List<T> GetEngPriceAdjLockWorkItem<T>(int engMainSeq)
        {
            string sql = @"
                select
                    a.Seq,
                    a.EC_SchEngProgressWorkItemSeq,
                    a.Kind,
                    a1.PayItem,
                    a1.Description,
                    b.ItemCode,
                    a.Weights,
                    a.Amount,
                    a.PriceIndexKindId
                from EC_EngPriceAdjLockWorkItem a
                inner join EC_SchEngProgressWorkItem b on(b.Seq=a.EC_SchEngProgressWorkItemSeq)
                inner join EC_SchEngProgressPayItem a1 on(a1.Seq=b.EC_SchEngProgressPayItemSeq)
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
        public bool UpdateEngPriceAdjWorkItem(ECEngPriceAdjWorkItemGroupVModel gModel)
        {
            SqlCommand cmd;
            string sql = @"
                    Update EC_EngPriceAdjWorkItem set
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
                List<ECEngPriceAdjWorkItemVModel> allItem = new List<ECEngPriceAdjWorkItemVModel>();
                foreach (ECEngPriceAdjWorkItemVModel m in gModel.M03310) { allItem.Add(m); }
                foreach (ECEngPriceAdjWorkItemVModel m in gModel.M03210) { allItem.Add(m); }
                foreach (ECEngPriceAdjWorkItemVModel m in gModel.M02742) { allItem.Add(m); }
                foreach (ECEngPriceAdjWorkItemVModel m in gModel.M03) { allItem.Add(m); }
                foreach (ECEngPriceAdjWorkItemVModel m in gModel.M05) { allItem.Add(m); }
                foreach (ECEngPriceAdjWorkItemVModel m in gModel.M02319) { allItem.Add(m); }
                foreach (ECEngPriceAdjWorkItemVModel m in gModel.M027_0296) { allItem.Add(m); }
                foreach (ECEngPriceAdjWorkItemVModel m in gModel.Mxxx) { allItem.Add(m); }

                foreach (ECEngPriceAdjWorkItemVModel m in allItem)
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
                log.Info("ECPriceAdjustmentService.UpdateEngPriceAdjWorkItem: " + e.Message);
                return false;
            }
        }
        //建立 WorkItem
        public bool InitEngPriceAdjWorkItem(EC_EngPriceAdjModel item)
        {
            try
            {
                string sql = @"
                    insert into EC_EngPriceAdjWorkItem(
                        EC_EngPriceAdjSeq, EC_EngPriceAdjLockWorkItemSeq, MonthQuantity, CreateUserSeq, ModifyUserSeq
                    )
                    select
	                    b.Seq EC_EngPriceAdjSeq, c.Seq EC_EngPriceAdjLockWorkItemSeq, ISNULL(d.MonthQuantity, 0), @ModifyUserSeq, @ModifyUserSeq
                    from EC_EngPriceAdj b
                    inner join EC_EngPriceAdjLockWorkItem c on(c.EngMainSeq=b.EngMainSeq)
                    inner join EC_SchEngProgressWorkItem c1 on(c1.Seq=c.EC_SchEngProgressWorkItemSeq)
                    left outer join (
                        select
                            b.EC_SchEngProgressPayItemSeq, sum(ISNULL(b.TodayConfirm, 0)) MonthQuantity
                        from EC_SupDailyDate a
                        inner join EC_SupPlanOverview b on(b.EC_SupDailyDateSeq=a.Seq)
                        inner join EC_SchEngProgressPayItem b1 on(b1.Seq=b.EC_SchEngProgressPayItemSeq)
                        where a.EngMainSeq=@EngMainSeq and a.DataType=1
                        and a.ItemDate>=@StartDate and a.ItemDate<@EndDate
                        group by b.EC_SchEngProgressPayItemSeq --group by b1.PayItem
                    ) d on (d.EC_SchEngProgressPayItemSeq=c1.EC_SchEngProgressPayItemSeq) --) d on (d.PayItem=c.PayItem)
                    where b.Seq=@EC_EngPriceAdjSeq
                 ";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@EC_EngPriceAdjSeq", item.Seq);
                cmd.Parameters.AddWithValue("@StartDate", item.AdjMonth);
                cmd.Parameters.AddWithValue("@EndDate", item.AdjMonth.AddMonths(1));
                cmd.Parameters.AddWithValue("@EngMainSeq", item.EngMainSeq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return true;
            }
            catch (Exception e)
            {
                log.Info("ECPriceAdjustmentService.initEngPriceAdjWorkItem: " + e.Message);
                return false;
            }
        }

        //
        public List<T> GetEngPriceAdj<T>(int engMainSeq, string adjMonth)
        {
            string sql = @"
                select
                    Seq, EngMainSeq, AdjMonth
                from EC_EngPriceAdj
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
                delete EC_EngPriceAdjWorkItem
                where EC_EngPriceAdjSeq in(
                    select Seq from EC_EngPriceAdj where EngMainSeq=@EngMainSeq and AdjMonth=@AdjMonth
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
                log.Info("ECPriceAdjustmentService.DelEngPriceAdjWorkItem: " + e.Message);
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
                from EC_EngPriceAdjWorkItem a
                inner join EC_EngPriceAdj b on(b.Seq=a.EC_EngPriceAdjSeq)
                inner join EC_EngPriceAdjLockWorkItem c on(c.Seq=a.EC_EngPriceAdjLockWorkItemSeq)
                inner join EC_SchEngProgressWorkItem c1 on(c1.Seq=c.EC_SchEngProgressWorkItemSeq)
                inner join EC_SchEngProgressPayItem c2 on(c2.Seq=c1.EC_SchEngProgressPayItemSeq)
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
                from EC_EngPriceAdj a
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
            List<EC_EngPriceAdjLockWorkItemModel> wItems = GetWorkItemBaseData(engMainSeq);
            string sql;
            SqlCommand cmd;

            db.BeginTransaction();
            try
            {
                sql = @"
                    insert into EC_EngPriceAdj (
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

                sql = @"insert into EC_EngPriceAdjLockWorkItem(
                        EngMainSeq,
                        EC_SchEngProgressWorkItemSeq,
                        Kind,
                        Weights,
                        Amount,
                        CreateUserSeq
                    )values(
                        @EngMainSeq,
                        @EC_SchEngProgressWorkItemSeq,
                        @Kind,
                        @Weights,
                        @Amount,
                        @ModifyUserSeq
                    )";
                foreach (EC_EngPriceAdjLockWorkItemModel m in wItems)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                    cmd.Parameters.AddWithValue("@EC_SchEngProgressWorkItemSeq", m.EC_SchEngProgressWorkItemSeq);
                    cmd.Parameters.AddWithValue("@Kind", m.Kind);
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
                log.Info("ECPriceAdjustmentService.initDateList: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }
        //M WorkItem 清單
        public List<EC_EngPriceAdjLockWorkItemModel> GetWorkItemBaseData(int engMainSeq)
        {
            string sql = @"
                select z1.Seq EC_SchEngProgressWorkItemSeq, z1.Kind
	                ,c.PayItem, b.Description, b.ItemCode
                    ,(b.Amount) / (c.Amount) Weights
	                ,b.Amount
                from (
                    select MIN(z.kind) kind, z.Seq
                    from (
    
                        select 101 kind, a.Seq from EC_SchEngProgressWorkItem a
                        where a.EC_SchEngProgressPayItemSeq in(
                            select Seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in (
                                select Seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M03310%'

                        union all

                        select 102 kind, a.Seq from EC_SchEngProgressWorkItem a
                        where a.EC_SchEngProgressPayItemSeq in(
                            select Seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in (
                                select Seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M03210%'

                        union all

                        select 103 kind, a.Seq from EC_SchEngProgressWorkItem a
                        where a.EC_SchEngProgressPayItemSeq in(
                            select Seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in (
                                select Seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M02742%'

                        union all

                        select 201 kind, a.Seq from EC_SchEngProgressWorkItem a
                        where a.EC_SchEngProgressPayItemSeq in(
                            select Seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in (
                                select Seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M03%'

                        union all

                        select 202 kind, a.Seq from EC_SchEngProgressWorkItem a
                        where a.EC_SchEngProgressPayItemSeq in(
                            select Seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in (
                                select Seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M05%'

                        union all

                        select 203 kind, a.Seq from EC_SchEngProgressWorkItem a
                        where a.EC_SchEngProgressPayItemSeq in(
                            select Seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in (
                                select Seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M2319%'

                        union all

                        select 204 kind, a.Seq from EC_SchEngProgressWorkItem a
                        where a.EC_SchEngProgressPayItemSeq in(
                            select Seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in (
                                select Seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and (a.ItemCode like 'M027%' or a.ItemCode like 'M0296%')

                        union all

                        select 999 kind, a.Seq from EC_SchEngProgressWorkItem a
                        where a.EC_SchEngProgressPayItemSeq in(
                            select Seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in (
                                select Seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M%'
                    ) z
                    group by z.Seq
                ) z1
                inner join EC_SchEngProgressWorkItem b on(b.Seq=z1.Seq)
                inner join EC_SchEngProgressPayItem c on(c.Seq=b.EC_SchEngProgressPayItemSeq)
                order by z1.kind, c.PayItem, b.ItemCode
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<EC_EngPriceAdjLockWorkItemModel>(cmd);
        }
        //新增加的 M WorkItem 清單
        public List<EC_EngPriceAdjLockWorkItemModel> GetNewWorkItemBaseData(int engMainSeq)
        {
            string sql = @"
                select z1.Seq EC_SchEngProgressWorkItemSeq, z1.Kind
                    ,(b.Amount) / (c.Amount) Weights ,b.Amount
                from (
                    select MIN(z.kind) kind, z.Seq 
                    from (
    
                        select 101 kind, a.Seq from EC_SchEngProgressWorkItem a
                        where a.EC_SchEngProgressPayItemSeq in(
                            select Seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in (
                                select Seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M03310%'

                        union all

                        select 102 kind, a.Seq from EC_SchEngProgressWorkItem a
                        where a.EC_SchEngProgressPayItemSeq in(
                            select Seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in (
                                select Seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M03210%'

                        union all

                        select 103 kind, a.Seq from EC_SchEngProgressWorkItem a
                        where a.EC_SchEngProgressPayItemSeq in(
                            select Seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in (
                                select Seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M02742%'

                        union all

                        select 201 kind, a.Seq from EC_SchEngProgressWorkItem a
                        where a.EC_SchEngProgressPayItemSeq in(
                            select Seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in (
                                select Seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M03%'

                        union all

                        select 202 kind, a.Seq from EC_SchEngProgressWorkItem a
                        where a.EC_SchEngProgressPayItemSeq in(
                            select Seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in (
                                select Seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M05%'

                        union all

                        select 203 kind, a.Seq from EC_SchEngProgressWorkItem a
                        where a.EC_SchEngProgressPayItemSeq in(
                            select Seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in (
                                select Seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M2319%'

                        union all

                        select 204 kind, a.Seq from EC_SchEngProgressWorkItem a
                        where a.EC_SchEngProgressPayItemSeq in(
                            select Seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in (
                                select Seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and (a.ItemCode like 'M027%' or a.ItemCode like 'M0296%')

                        union all

                        select 999 kind, a.Seq from EC_SchEngProgressWorkItem a
                        where a.EC_SchEngProgressPayItemSeq in(
                            select Seq from EC_SchEngProgressPayItem where EC_SchEngProgressHeaderSeq in (
                                select Seq from EC_SchEngProgressHeader where EngMainSeq=@EngMainSeq
                            )
                        )
                        and a.ItemCode like 'M%'
                    ) z
                    group by z.Seq
                ) z1
                inner join EC_SchEngProgressWorkItem b on(b.Seq=z1.Seq)
                inner join EC_SchEngProgressPayItem c on(c.Seq=b.EC_SchEngProgressPayItemSeq)
                where z1.Seq not in (
                    select EC_SchEngProgressWorkItemSeq from EC_EngPriceAdjLockWorkItem
                    where EngMainSeq=@EngMainSeq
                )
                order by z1.kind, c.PayItem, b.ItemCode
                ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
            return db.GetDataTableWithClass<EC_EngPriceAdjLockWorkItemModel>(cmd);
        }
        //增加日期清單
        public bool AddDateList(int engMainSeq, List<DateTime> dateList, List<EC_EngPriceAdjLockWorkItemModel> wItems)
        {
            string sql;
            SqlCommand cmd;

            db.BeginTransaction();
            try
            {
                sql = @"
                    insert into EC_EngPriceAdj (
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

                sql = @"insert into EC_EngPriceAdjLockWorkItem(
                        EngMainSeq,
                        EC_SchEngProgressWorkItemSeq,
                        Kind,
                        Weights,
                        Amount,
                        CreateUserSeq
                    )values(
                        @EngMainSeq,
                        @EC_SchEngProgressWorkItemSeq,
                        @Kind,
                        @Weights,
                        @Amount,
                        @ModifyUserSeq
                    )";
                foreach (EC_EngPriceAdjLockWorkItemModel m in wItems)
                {
                    Null2Empty(m);
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@EngMainSeq", engMainSeq);
                    cmd.Parameters.AddWithValue("@EC_SchEngProgressWorkItemSeq", m.EC_SchEngProgressWorkItemSeq);
                    cmd.Parameters.AddWithValue("@Kind", m.Kind);
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
                log.Info("ECPriceAdjustmentService.AddDateList: " + e.Message);
                //log.Info(sql);
                return false;
            }
        }
        //監造日誌最大日期
        public List<T> GetMaxSupDailyDate<T>(int engMainSeq)
        {
            string sql = @"
                select * from (
                    select max(ItemDate) ItemDate from EC_SupDailyDate
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