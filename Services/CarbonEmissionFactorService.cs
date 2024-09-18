using EQC.Common;
using EQC.EDMXModel;
using EQC.Models;
using EQC.ViewModel.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace EQC.Services
{
    public class CarbonEmissionFactorService : BaseService
    {//碳排係數維護
        //**** 碳係數指引維護 ***** shioulo 20230202
        public int GetSubListCount(int carbonEmissionFactorSeq)
        {
            string sql = @"SELECT
                    count(a.Seq) total
                FROM CarbonEmissionFactorDetail a 
                where a.CarbonEmissionFactorSeq=@CarbonEmissionFactorSeq
                and IsDel=0
                and ItemMode < 90000";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@CarbonEmissionFactorSeq", carbonEmissionFactorSeq);
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetSubList<T>(int pageRecordCount, int pageIndex, int carbonEmissionFactorSeq)
        {
            string sql = @"SELECT
                    a.Seq,
                    a.CarbonEmissionFactorSeq,
                    a.NameSpec,
                    a.Unit,
                    a.Amount,
                    a.KgCo2e,
                    a.Quantity,
                    a.Memo,
                    a.ItemMode
                FROM CarbonEmissionFactorDetail a
				where a.CarbonEmissionFactorSeq=@CarbonEmissionFactorSeq
                and IsDel=0
                and ItemMode < 90000
                order by a.ItemMode, a.NameSpec
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            cmd.Parameters.AddWithValue("@CarbonEmissionFactorSeq", carbonEmissionFactorSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //工料分析基本量
        public List<T> GetSubUnitItem<T>(int carbonEmissionFactorSeq)
        {
            string sql = @"SELECT
                    a.Seq,
                    a.CarbonEmissionFactorSeq,
                    a.NameSpec,
                    a.Unit,
                    a.Amount,
                    a.KgCo2e,
                    a.Quantity,
                    a.Memo,
                    a.ItemMode
                FROM CarbonEmissionFactorDetail a
				where a.CarbonEmissionFactorSeq=@CarbonEmissionFactorSeq
                and ItemMode = 90000";

            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@CarbonEmissionFactorSeq", carbonEmissionFactorSeq);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //新增
        public int AddSubRecord(CarbonEmissionFactorDetailModel m)
        {
            Null2Empty(m);

            string sql = @"SELECT Seq FROM CarbonEmissionFactorDetail 
                where CarbonEmissionFactorSeq=@CarbonEmissionFactorSeq
                and ItemMode<90000
                and Memo=@Memo";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@CarbonEmissionFactorSeq", m.CarbonEmissionFactorSeq);
            cmd.Parameters.AddWithValue("@Memo", m.Memo);
            DataTable dt = db.GetDataTable(cmd);
            if (dt.Rows.Count == 1)
            {
                m.Seq = Convert.ToInt32(dt.Rows[0]["Seq"].ToString());
                return UpdateSubRecord(m);
            }


            sql = @"SELECT
                    count(Seq) total
                FROM CarbonEmissionFactorDetail 
                where CarbonEmissionFactorSeq=@CarbonEmissionFactorSeq
                and ItemMode>=90000";
            cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@CarbonEmissionFactorSeq", m.CarbonEmissionFactorSeq);
            dt = db.GetDataTable(cmd);
            bool addMode = Convert.ToInt32(dt.Rows[0]["total"].ToString()) == 0;
            
            db.BeginTransaction();
            try
            {
                if(addMode)
                {
                    sql = @"
                        insert into CarbonEmissionFactorDetail (
                            CarbonEmissionFactorSeq,
                            ItemMode,
                            NameSpec,
                            KgCo2e,
                            Unit,
                            Amount,
                            Quantity,
                            Memo,
                            CreateTime,
                            CreateUserSeq,
                            ModifyTime,
                            ModifyUserSeq
                        )values(
                            @CarbonEmissionFactorSeq,
                            @ItemMode,
                            @NameSpec,
                            0,
                            (select Unit from CarbonEmissionFactor where Seq=@CarbonEmissionFactorSeq),
                            1,
                            0,
                            '',
                            GetDate(),
                            @ModifyUserSeq,
                            GetDate(),
                            @ModifyUserSeq
                        )";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.AddWithValue("@CarbonEmissionFactorSeq", m.CarbonEmissionFactorSeq);
                    cmd.Parameters.AddWithValue("@ItemMode", 90000);
                    cmd.Parameters.AddWithValue("@NameSpec", "工料分析基本量");
                    cmd.Parameters.AddWithValue("@Unit", "");
                    //cmd.Parameters.AddWithValue("@Amount", 1);
                    cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                    db.ExecuteNonQuery(cmd);
                }

                sql = @"
                    insert into CarbonEmissionFactorDetail (
                        CarbonEmissionFactorSeq,
                        ItemMode,
                        NameSpec,
                        KgCo2e,
                        Unit,
                        Amount,
                        Quantity,
                        Memo,
                        CreateTime,
                        CreateUserSeq,
                        ModifyTime,
                        ModifyUserSeq
                    )values(
                        @CarbonEmissionFactorSeq,
                        @ItemMode,
                        @NameSpec,
                        @KgCo2e,
                        @Unit,
                        @Amount,
                        @Quantity,
                        @Memo,
                        GetDate(),
                        @ModifyUserSeq,
                        GetDate(),
                        @ModifyUserSeq
                    )";
                cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@CarbonEmissionFactorSeq", m.CarbonEmissionFactorSeq);
                cmd.Parameters.AddWithValue("@ItemMode", 0);
                cmd.Parameters.AddWithValue("@NameSpec", m.NameSpec);
                cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(m.KgCo2e));
                cmd.Parameters.AddWithValue("@Unit", m.Unit);
                cmd.Parameters.AddWithValue("@Amount", m.Amount);
                cmd.Parameters.AddWithValue("@Quantity", m.Amount * m.KgCo2e);
                cmd.Parameters.AddWithValue("@Memo", m.Memo);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('CarbonEmissionMaterial') AS NewSeq";
                cmd = db.GetCommand(sql1);
                dt = db.GetDataTable(cmd);

                m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                UpdateSubTotal(m.CarbonEmissionFactorSeq);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("CarbonEmissionFactorService.AddSubRecord: " + e.Message);
                return -1;
            }
        }
        //更新
        public int UpdateSubRecord(CarbonEmissionFactorDetailModel m)
        {
            Null2Empty(m);
            string sql = @"
            update CarbonEmissionFactorDetail set 
                IsDel=0,
                NameSpec = @NameSpec,
                KgCo2e = @KgCo2e,
                Unit = @Unit,
                Amount = @Amount,
                Quantity = @Amount*KgCo2e,
                Memo = @Memo,
                ModifyTime = GetDate(),
                ModifyUserSeq = @ModifyUserSeq
            where Seq=@Seq";

            db.BeginTransaction();
            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@NameSpec", m.NameSpec);
                cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(m.KgCo2e));
                cmd.Parameters.AddWithValue("@Unit", m.Unit);
                cmd.Parameters.AddWithValue("@Amount", m.Amount);
                cmd.Parameters.AddWithValue("@Memo", m.Memo);

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                UpdateSubTotal(m.CarbonEmissionFactorSeq);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("CarbonEmissionFactorService.UpdateSubRecord: " + e.Message);
                return -1;
            }
        }
        //刪除
        public int DelSubRecord(CarbonEmissionFactorDetailModel m)
        {
            Null2Empty(m);
            string sql = "";
            db.BeginTransaction();
            try
            {
                sql = @"update CarbonEmissionFactorDetail set 
                    IsDel = 1,
                    ModifyTime = GetDate(),
                    ModifyUserSeq = @ModifyUserSeq
                where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                UpdateSubTotal(m.CarbonEmissionFactorSeq);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("CarbonEmissionFactorService.DelSubRecord: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
        //更新合計
        private void UpdateSubTotal(int carbonEmissionFactorSeq)
        {
            //合計
            string sql = @"
            update CarbonEmissionFactorDetail set 
                --Amount = (select IsNULL(sum(Amount),0) from CarbonEmissionFactorDetail where CarbonEmissionFactorSeq=@CarbonEmissionFactorSeq and IsDel=0 and ItemMode<90000),
                Quantity = (select IsNULL(sum(Amount*KgCo2e),0) from CarbonEmissionFactorDetail where CarbonEmissionFactorSeq=@CarbonEmissionFactorSeq and IsDel=0 and ItemMode<90000) / Amount,
                ModifyTime = GetDate(),
                ModifyUserSeq = @ModifyUserSeq
            where CarbonEmissionFactorSeq=@CarbonEmissionFactorSeq
            and ItemMode=90000";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@CarbonEmissionFactorSeq", carbonEmissionFactorSeq);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
            db.ExecuteNonQuery(cmd);

            //回存到 master item
            sql = @"
            update CarbonEmissionFactor set 
                KgCo2e = (select IsNULL(Quantity,0) from CarbonEmissionFactorDetail where CarbonEmissionFactorSeq=@CarbonEmissionFactorSeq and ItemMode=90000),
                ModifyTime = GetDate(),
                ModifyUserSeq = @ModifyUserSeq
            where Seq=@CarbonEmissionFactorSeq
            ";
            cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@CarbonEmissionFactorSeq", carbonEmissionFactorSeq);
            cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
            db.ExecuteNonQuery(cmd);
        }

        //**** 碳係數指引維護 *****
        //碳報表-統計
        public List<T> GetCarbonReoprtGroup<T>(int year, int unitSeq, int subUnitSeq) where T : UnitTreeJsonConvertor
        {
            string sql = @"
                select
	                m1.OrderNo, m1.execUnitName, count(m1.OrderNo) engCnt,
                    sum(m1.SubContractingBudget) TotalBudget,
                    sum(m1._SubContractingBudget) AwardTotalBudget,
                    sum(m1.CarbonDemandQuantity) CarbonDemandQuantity,
                    sum(m1.ApprovedCarbonQuantity) ApprovedCarbonQuantity,
                    sum(IIF(m1.awardStatus='是',1,0)) awardCnt, --已發包件數
                    sum(m1._Co2Total) Co2Total,
                    sum(m1._Co2TotalItem) Co2TotalItem,
                    sum(m1._GreenFunding)/1000 GreenFunding,
                    round(IIF(count(m1.OrderNo) > 0 and  sum(m1._SubContractingBudget)>0, sum(m1._GreenFunding)*100/sum(m1._SubContractingBudget), 0),1) greenFundingRate,
                    max(m1.F1206Desc) + ',' + max(m1.F1312Desc) + ',' + max(m1.F1404Desc) GreenFundingValue,
                    max(m1.Remark) Remark,
                    sum(m1._F1108Area) F1108Area,
                    sum(m1._F1109Length) F1109Length,
                    min(m1.ExecUnitSeq) ExecUnitSeq,
                    min(m1.ExecSubUnitSeq) ExecSubUnitSeq,
                    sum(m1._Co2TotalItemAll) Co2TotalItemAll,
                    round(IIF(count(m1.OrderNo) > 0 and sum(m1._Co2TotalItemAll) > 0, sum(m1._Co2TotalItem)*100/ sum(m1._Co2TotalItemAll), 0),1) co2TotalRate     
                from (
                    select
                        --可分解率
                        IIF(m.SubContractingBudget is not null and m.SubContractingBudget>0 and m.awardStatus='是', Round(m.Co2TotalItem*100/m.SubContractingBudget, 0), NULL) co2TotalRate,
                        --綠色經費比例
                        IIF(m.SubContractingBudget is not null and m.SubContractingBudget>0 and m.awardStatus='是', Round(m.GreenFunding*100/m.SubContractingBudget, 0), NULL) greenFundingRate,
                        IIF(m.awardStatus='是', m.Co2Total, 0) _Co2Total,
                        IIF(m.awardStatus='是', m.Co2TotalItem, 0) _Co2TotalItem,
                        IIF(m.awardStatus='是', m.Co2TotalItemAll, 0) _Co2TotalItemAll,
                        IIF(m.awardStatus='是', m.GreenFunding, 0) _GreenFunding,
                        IIF(m.awardStatus='是', m.F1108Area, 0) _F1108Area,
                        IIF(m.awardStatus='是', m.F1109Length, 0) _F1109Length,
                        IIF(m.awardStatus='是', m.SubContractingBudget, 0) _SubContractingBudget,
                        m.*
                    from (
                        select
                            b.OrderNo,
                            b.Name execUnitName,
                            --a.EngName,
                            a.ExecUnitSeq ,
                            a.ExecSubUnitSeq ,
                            e.OutsourcingBudget, 
                            a.CarbonDemandQuantity,
                            a.ApprovedCarbonQuantity,
                            a.SubContractingBudget,
                            d.F1108Area,
                            d.F1109Length,
                            d.F1106TreeJson,
                            d.F1107TreeJson,
                            d.F1206Desc,
                            d.F1312Desc,
                            d.F1404Desc,
                            d.Remark
                            --d.F1110Desc,
                            --e.ScheStartDate,
                            --e.ActualStartDate
                            --發包狀態
                            ,(
                                CASE ISNULL(a.AwardDate,'') WHEN '' THEN '' ELSE '是' END
                            ) awardStatus
                            --碳排量
                            ,(
                                select
                                    ROUND(sum(ISNULL(za.ItemKgCo2e,0)),0)
                                from CarbonEmissionHeader zb
                                inner join CarbonEmissionPayItem za on(za.CarbonEmissionHeaderSeq=zb.Seq)
                                where zb.EngMainSeq=a.Seq
                                and za.KgCo2e is not null and za.ItemKgCo2e is not null
                            ) Co2Total 
                            --碳排量金額
                            ,(
                                select
                                    ROUND(sum(ISNULL(za.Quantity * za.Price, 0)), 0)
                                from CarbonEmissionHeader zb
                                inner join CarbonEmissionPayItem za on(za.CarbonEmissionHeaderSeq=zb.Seq)
                                where zb.EngMainSeq=a.Seq
                                and za.KgCo2e is not null and za.RStatusCode != 300
                            ) Co2TotalItem  

                            --碳排量金額全部(不包括大項)
                            ,(
                                select
                                    ROUND(sum(ISNULL(za.Quantity * za.Price, 0)), 0)
                                from CarbonEmissionHeader zb
                                inner join CarbonEmissionPayItem za on(za.CarbonEmissionHeaderSeq=zb.Seq)
                                where zb.EngMainSeq=a.Seq
                                and za.RStatusCode != 300
                            ) Co2TotalItemAll  
                            --綠色經費
                            ,(
                                select
                                    ROUND(sum(ISNULL(za.Amount, 0)), 0)
                                from CarbonEmissionHeader zb
                                inner join CarbonEmissionPayItem za on(za.CarbonEmissionHeaderSeq=zb.Seq and za.GreenFundingSeq is not null)
                                where zb.EngMainSeq=a.Seq
                                and za.KgCo2e is not null
                            ) GreenFunding
                            ----喬木類合計
                            ,(
                                select
                                    sum(za.Quantity)
                                from CarbonEmissionHeader zb
                                inner join CarbonEmissionPayItem za on(za.CarbonEmissionHeaderSeq=zb.Seq)
                                inner join TreeList zc ON(zc.Type+zc.Code = SUBSTRING(za.RefItemCode,len(za.RefItemCode)-9, 8))
                                where zb.EngMainSeq=a.Seq
                                and len(za.RefItemCode)>9
                                and SUBSTRING(za.RefItemCode,len(za.RefItemCode)-9, 5) in ('02931')
                            ) Tree02931Total
                            ----灌木類合計
                            ,(
                                select
                                    sum(za.Quantity)
                                from CarbonEmissionHeader zb
                                inner join CarbonEmissionPayItem za on(za.CarbonEmissionHeaderSeq=zb.Seq)
                                inner join TreeList zc ON(zc.Type+zc.Code = SUBSTRING(za.RefItemCode,len(za.RefItemCode)-9, 7))
                                where zb.EngMainSeq=a.Seq
                                and len(za.RefItemCode)>9
                                and SUBSTRING(za.RefItemCode,len(za.RefItemCode)-9, 5) in ('02932')
                            ) Tree02932Total        
                        from EngMain a
                        inner join Unit b on(b.ParentSeq is null and b.Seq = a.ExecUnitSeq and b.name != '水利署')
                        left join CarbonEmissionHeader c on(c.EngMainSeq=a.Seq)
                        left outer join CECheckTable d on(d.CarbonEmissionHeaderSeq=c.Seq)
                        left outer join PrjXML e on(e.Seq= a.PrjXMLSeq)
                        where 1=1 --a.EngNo='111-KPRW-E01'
                        and((@year=-1) or a.EngYear=@year)
                        and((@unitSeq=-1) or a.ExecUnitSeq=@unitSeq)
                        and((@subUnitSeq=-1) or a.ExecSubUnitSeq=@subUnitSeq)
                    ) m
                ) m1
                group by m1.OrderNo, m1.execUnitName
                order by m1.OrderNo, m1.execUnitName
            ";
            //Utils.getAuthoritySql("a.")
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@unitSeq", unitSeq);
            cmd.Parameters.AddWithValue("@subUnitSeq", subUnitSeq);

            var list =  db.GetDataTableWithClass<T>(cmd);
            using (var context = new EQC_NEW_Entities())
            {
                var engExecUnitGroupDic =
                    context.EngMain
                        .Where(r => r.AwardDate != null && (r.EngYear == year || r.EngYear == -1) && 
                            (r.ExecUnitSeq == unitSeq || unitSeq == -1 ) && 
                            (r.ExecSubUnitSeq == subUnitSeq || subUnitSeq == -1))    
                        .GroupBy(r => r.Unit1.Name)
                        .ToDictionary(r => r.Key, r => r
                        .Select(rr => rr.CarbonEmissionHeader.FirstOrDefault()?.CECheckTable.FirstOrDefault())
                        .Where(rr => rr != null)
                        );
                foreach(var engGroup in  list)
                {
                    if (!engExecUnitGroupDic.ContainsKey(engGroup.execUnitName)) continue;
                    engGroup.Tree02931JsonList = engExecUnitGroupDic[engGroup.execUnitName].Select(r => r.F1106TreeJson).ToList();
                    engGroup.Tree02932JsonList = engExecUnitGroupDic[engGroup.execUnitName].Select(r => r.F1107TreeJson).ToList();
                }
            }
            return list;
        }
        public List<T> GetCarbonReoprtDetail<T>(int year, int unitSeq, int subUnitSeq, bool awardStatus = false)
        {
            string sql = @"
                select
	                --可分解率
	                IIF(m.SubContractingBudget is not null and m.SubContractingBudget>0, Round(m.Co2TotalItem*100/m.Co2TotalItemAll  , 0), NULL) co2TotalRate,
                    --綠色經費比例
                    IIF(m.SubContractingBudget is not null and m.SubContractingBudget>0, Round(m.GreenFunding*100000/m.SubContractingBudget, 0), NULL) greenFundingRate,
   	                m.*
                from (
                    select
                        b.OrderNo,
                        b.Name execUnitName,
                        a.EngName,
                        a.ExecUnitSeq,
                        a.AwardDate,
                        a.DredgingEng,
                        e.OutsourcingBudget, --核定經費
                        a.CarbonDemandQuantity,
                        a.ApprovedCarbonQuantity,
                        a.SubContractingBudget TotalBudget,
                        a.SubContractingBudget, 
                        d.F1108Area,
                        d.F1109Length,
                        d.F1110Desc,
                        d.F1106TreeJson Tree02931Json,
                        d.F1107TreeJson Tree02932Json,
                        e.ScheStartDate,
                        e.ActualStartDate,
                        d.F1206Desc + ',' + d.F1312Desc+ ',' + d.F1404Desc GreenFundingValue,
                        d.Remark Remark,
                        dd.Result CarbonReduction,
                        d.F1110Desc ReductionStrategy
                        --發包狀態
                        ,(
                            CASE ISNULL(a.AwardDate,'') WHEN '' THEN '否' ELSE '是' END
                        ) awardStatus
                        --碳排量
                        ,(
                            select
                                ROUND(sum(ISNULL(za.ItemKgCo2e,0)),0)
                            from CarbonEmissionHeader zb
                            inner join CarbonEmissionPayItem za on(za.CarbonEmissionHeaderSeq=zb.Seq)
                            where zb.EngMainSeq=a.Seq
                            and za.KgCo2e is not null and za.ItemKgCo2e is not null
                        ) Co2Total 
                        --碳排量金額
                        ,(
                            select
                                ROUND(sum(ISNULL(za.Quantity * za.Price, 0)), 0)
                            from CarbonEmissionHeader zb
                            inner join CarbonEmissionPayItem za on(za.CarbonEmissionHeaderSeq=zb.Seq)
                            where zb.EngMainSeq=a.Seq
                            and za.KgCo2e is not null and za.RStatusCode != 300 
                        ) Co2TotalItem  
                        --碳排量金額(全部，不包括大項)
                        ,(
                            select
                                ROUND(sum(ISNULL(za.Quantity * za.Price, 0)), 0)
                            from CarbonEmissionHeader zb
                            inner join CarbonEmissionPayItem za on(za.CarbonEmissionHeaderSeq=zb.Seq)
                            where zb.EngMainSeq=a.Seq
                            and za.RStatusCode != 300 
                        ) Co2TotalItemAll  
                        --綠色經費
                        ,(
                            select
                                ROUND(sum(ISNULL(za.Amount, 0)), 0)
                            from CarbonEmissionHeader zb
                            inner join CarbonEmissionPayItem za on(za.CarbonEmissionHeaderSeq=zb.Seq and za.GreenFundingSeq is not null)
                            where zb.EngMainSeq=a.Seq
                        )/1000 GreenFunding
                         --喬木類/株
                        /*,(
                            SELECT STUFF(
                            (SELECT ',' + z1.TreeType+'*'+CAST(CAST(round(Quantity,0) as int) as VARCHAR(10))
                            FROM (
                                select
                                     z.TreeType, sum(z.Quantity) Quantity
                                from (
                                    select
                    	
                                        SUBSTRING(zc.Name, CHARINDEX('-', zc.Name)+1, len(zc.Name)-CHARINDEX('-', zc.Name)) TreeType,
                                        za.Quantity
                                    from CarbonEmissionHeader zb
                                    inner join CarbonEmissionPayItem za on(za.CarbonEmissionHeaderSeq=zb.Seq)
                                    inner join TreeList zc ON(zc.Type+zc.Code = SUBSTRING(za.RefItemCode,len(za.RefItemCode)-9, 8))
                                    where zb.EngMainSeq=a.Seq
                                    and len(za.RefItemCode)>9
                                    and SUBSTRING(za.RefItemCode,len(za.RefItemCode)-9, 5) in ('02931')
                                ) z
                                group by z.TreeType
                            ) z1
                            FOR XML PATH('')) ,1,1,'')
                        ) Tree02931 */
                        ----喬木類合計
/*
                        ,(
                            select
                                sum(za.Quantity)
                            from CarbonEmissionHeader zb
                            inner join CarbonEmissionPayItem za on(za.CarbonEmissionHeaderSeq=zb.Seq)
                            inner join TreeList zc ON(zc.Type+zc.Code = SUBSTRING(za.RefItemCode,len(za.RefItemCode)-9, 8))
                            where zb.EngMainSeq=a.Seq
                            and len(za.RefItemCode)>9
                            and SUBSTRING(za.RefItemCode,len(za.RefItemCode)-9, 5) in ('02931')
                        )  Tree02931Total */
                        --灌木類/株
                        /*,(
                            SELECT STUFF(
                            (SELECT ',' + z1.TreeType+'*'+CAST(CAST(round(Quantity,0) as int) as VARCHAR(10))
                            FROM (
                                select
                                     z.TreeType, sum(z.Quantity) Quantity
                                from (
                                    select
                    	
                                        SUBSTRING(zc.Name, CHARINDEX('-', zc.Name)+1, len(zc.Name)-CHARINDEX('-', zc.Name)) TreeType,
                                        za.Quantity
                                    from CarbonEmissionHeader zb
                                    inner join CarbonEmissionPayItem za on(za.CarbonEmissionHeaderSeq=zb.Seq)
                                    inner join TreeList zc ON(zc.Type+zc.Code = SUBSTRING(za.RefItemCode,len(za.RefItemCode)-9, 7))
                                    where zb.EngMainSeq=a.Seq
                                    and len(za.RefItemCode)>9
                                    and SUBSTRING(za.RefItemCode,len(za.RefItemCode)-9, 5) in ('02932')
                                ) z
                                group by z.TreeType
                            ) z1
                            FOR XML PATH('')) ,1,1,'')
                        ) Tree02932 */
                        ----灌木類合計
/*
                        ,(
                            select
                                sum(za.Quantity)
                            from CarbonEmissionHeader zb
                            inner join CarbonEmissionPayItem za on(za.CarbonEmissionHeaderSeq=zb.Seq)
                            inner join TreeList zc ON(zc.Type+zc.Code = SUBSTRING(za.RefItemCode,len(za.RefItemCode)-9, 7))
                            where zb.EngMainSeq=a.Seq
                            and len(za.RefItemCode)>9
                            and SUBSTRING(za.RefItemCode,len(za.RefItemCode)-9, 5) in ('02932')
                        ) Tree02932Total */
        
                    from EngMain a
                    inner join Unit b on(b.ParentSeq is null and b.Seq = a.ExecUnitSeq and b.name != '水利署')
                    left join CarbonEmissionHeader c on(c.EngMainSeq=a.Seq)
                    left outer join CarbonReductionCalResult dd on(dd.EngMainSeq=a.Seq)
                    left outer join CECheckTable d on(d.CarbonEmissionHeaderSeq=c.Seq)
                    left outer join PrjXML e on(e.Seq= a.PrjXMLSeq)
                    where ((@year=-1) or a.EngYear=@year)
                    and((@unitSeq=-1) or a.ExecUnitSeq=@unitSeq)
                    and((@subUnitSeq=-1) or a.ExecSubUnitSeq=@subUnitSeq)
                ) m
                where m.awardStatus = @awardStatus or  @awardStatus = '否'
                order by m.AwardDate
            ";
            //Utils.getAuthoritySql("a.")
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@year", year);
            cmd.Parameters.AddWithValue("@unitSeq", unitSeq);
            cmd.Parameters.AddWithValue("@subUnitSeq", subUnitSeq);
            cmd.Parameters.AddWithValue("@awardStatus", awardStatus ? "是" : "否");

            return db.GetDataTableWithClass<T>(cmd);
        }
        //水利署碳排管制表-清單
        public List<T> GetCarbonControlTable<T>(int year, int unitSeq)
        {
            string sql = @"
                select
	                b.Name execUnitName
                    ,a.EngNo
                    ,a.EngYear
                    ,a.EngName
                    ,a.ExecUnitSeq
                    ,c.TenderNo
                    ,c.TenderName
                    ,a.AwardDate
                    --發包狀態
                    --,a.AwardDate
                    ,(
    	                CASE ISNULL(a.AwardDate,'') WHEN '' THEN 0 ELSE 1 END
                    ) awardStatus
                    --建立標案
                    ,1 createEng    
                    --匯入PCCES建立標案,有勾稽工程會是1
                    --,a.PrjXMLSeq 
                    ,(
    	                CASE ISNULL(a.PrjXMLSeq,'') WHEN '' THEN 0 ELSE 1 END
                    ) pccesXML    
                    --碳排量估算及檢核,顯示碳排量的可拆解率
                    ,(
                        select cast(
	                        ROUND(sum(ISNULL(x1.Quantity * x1.Price, 0)), 0)*100 / x2.SubContractingBudget as decimal(15,2)) Co2Rate
                        from CarbonEmissionHeader x
                        inner join CarbonEmissionPayItem x1 on(x1.CarbonEmissionHeaderSeq=x.Seq)
                        inner join EngMain x2 on(x2.Seq=x.EngMainSeq and x2.SubContractingBudget is not null and x2.SubContractingBudget>0)
                        where x.EngMainSeq=a.Seq
                        and x1.KgCo2e is not null and x1.ItemKgCo2e is not null
                        group by x2.SubContractingBudget --20230719
                        --(and x1.RStatusCode>50 and x1.RStatusCode<200 or x1.RStatusCode=201)
                    ) detachableRate
                    --材料設備送審管制總表(新增過資料就是1) 及 檢試驗管制總表填列('預定送審日期'有無填寫即可,暫不判斷)
                    ,(SELECT count(x.Seq) FROM EngMaterialDeviceList x where x.EngMainSeq = a.Seq ) engMaterialDeviceCount
                    ,(SELECT count(x.Seq) FROM EngMaterialDeviceList x
                            inner join EngMaterialDeviceSummary x1 on(x1.EngMaterialDeviceListSeq=x.Seq and x1.SchAuditDate is not null)
                            where x.EngMainSeq = a.Seq
                    ) engMaterialDeviceSummaryCount
                    -- 監造報表及施工日誌登載,新增過就是1
                    --,(select count(Seq) from SupDailyDate x1 where x1.EngMainSeq = a.Seq ) supDailyCount
                    ,(
                        CASE(select count(Seq) from SupDailyDate x1 where x1.EngMainSeq = a.Seq ) WHEN 0 THEN 0 ELSE 1 END
                    ) supDaily
                    --施工抽查表填報及照片上傳,上傳過照片就是1
                    /*,(
    	                select count(x3.Seq) from EngConstruction x1
                        inner join ConstCheckRec x2 on(x2.EngConstructionSeq=x1.Seq)
                        inner join ConstCheckRecFile x3 on(x3.ConstCheckRecSeq=x2.Seq)
                        where x1.EngMainSeq=a.Seq
	                ) checkRecCount*/
                    ,(
                        CASE(
                            select count(x3.Seq) from EngConstruction x1
                            inner join ConstCheckRec x2 on (x2.EngConstructionSeq = x1.Seq)
                            inner join ConstCheckRecFile x3 on (x3.ConstCheckRecSeq = x2.Seq)
                            where x1.EngMainSeq = a.Seq
                        ) WHEN 0 THEN 0 ELSE 1 END
                    ) checkRec
                from EngMain a
                inner join Unit b on(b.ParentSeq is null and b.Seq = a.ExecUnitSeq and b.name != '水利署')
                left outer join PrjXML c on(c.Seq= a.PrjXMLSeq)
                where a.AwardDate is not null
                and((@year=-1) or (YEAR(a.AwardDate)-1911)=@year)
                and((@unitSeq=-1) or a.ExecUnitSeq=@unitSeq)
                "
                + Utils.getAuthoritySql("a.")
                + @"
                order by b.OrderNo, a.EngNo
            ";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@year", year);//s20230506
            cmd.Parameters.AddWithValue("@unitSeq", unitSeq);//s20230506

            return db.GetDataTableWithClass<T>(cmd);
        }
        //水利署碳排管制表-統計
        public List<T> GetCarbonControlTableSta<T>()
        {
            string sql = @"
                select 
	                z.execUnitName
                    ,COUNT(z.OrderNo) createEng
                    ,sum(z.pccesXML) pccesXML
                    ,sum(z.detachableRate) detachableRateCnt
                    ,sum(z.engMaterialDevice) engMaterialDevice
                    ,sum(z.supDaily) supDaily
                    ,sum(z.checkRec) checkRec
                from (
                    select
	                    b.OrderNo,
	                    b.Name execUnitName
                        --匯入PCCES建立標案,有勾稽工程會是1
                        ,(
    	                    CASE ISNULL(a.PrjXMLSeq,'') WHEN '' THEN 0 ELSE 1 END
                        ) pccesXML    
                        --碳排量估算及檢核,顯示碳排量的可拆解率
                        ,IIF(
                            (
                                select cast(
                                    ROUND(sum(ISNULL(x1.Quantity * x1.Price, 0)), 0)*100 / (select TotalBudget from EngMain where Seq=a.Seq)
                                as decimal(15,2)) Co2Rate
                                from CarbonEmissionHeader x
                                inner join CarbonEmissionPayItem x1 on(x1.CarbonEmissionHeaderSeq=x.Seq)
                                where x.EngMainSeq=a.Seq
                                and x1.KgCo2e is not null and x1.ItemKgCo2e is not null --s20230310
                                --and (x1.RStatusCode>50 and x1.RStatusCode<200 or x1.RStatusCode<201)
                            ) >=50
                        ,1, 0) detachableRate
                        --材料設備送審管制總表
                        ,(
                            CASE(SELECT count(x.Seq) FROM EngMaterialDeviceList x where x.EngMainSeq = a.Seq) WHEN 0 THEN 0 ELSE 1 END
                        ) engMaterialDevice
                        -- 監造報表及施工日誌登載
                        ,(
                            CASE(select count(Seq) from SupDailyDate x1 where x1.EngMainSeq = a.Seq ) WHEN 0 THEN 0 ELSE 1 END
                        ) supDaily
                        --施工抽查表填報及照片上傳
                        ,(
                            CASE(
                                select count(x3.Seq) from EngConstruction x1
                                inner join ConstCheckRec x2 on (x2.EngConstructionSeq = x1.Seq)
                                inner join ConstCheckRecFile x3 on (x3.ConstCheckRecSeq = x2.Seq)
                                where x1.EngMainSeq = a.Seq
                            ) WHEN 0 THEN 0 ELSE 1 END
                        ) checkRec
                    from EngMain a
                    inner join Unit b on(b.ParentSeq is null and b.Seq = a.ExecUnitSeq and b.name != '水利署')
                    left outer join PrjXML c on(c.Seq= a.PrjXMLSeq)
                    where 1=1"
                    + Utils.getAuthoritySql("a.")
                    + @"
                ) z
                GROUP by z.OrderNo,z.execUnitName
                order by z.OrderNo
            ";
            SqlCommand cmd = db.GetCommand(sql);

            return db.GetDataTableWithClass<T>(cmd);
        }
        //清單
        public int GetListCount(string keyWord)
        {
            string sql = @"SELECT
                    count(a.Seq) total
                FROM CarbonEmissionFactor a 
                where a.IsEnabled=1
                and a.Code Like @keyWord";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@keyWord", "%"+keyWord+"%");
            DataTable dt = db.GetDataTable(cmd);
            return Convert.ToInt32(dt.Rows[0]["total"].ToString());
        }
        public List<T> GetList<T>(int pageRecordCount, int pageIndex, string keyWord)
        {
            string sql = @"SELECT
                    a.Seq,
                    a.Code,
                    a.Item,
                    a.KgCo2e,
                    a.Unit,
                    a.Kind,
                    a.IsEnabled,
                    a.SubCode,
                    a.KeyCode2,
                    a.Memo,
                    a.Green
                FROM CarbonEmissionFactor a
				where a.IsEnabled=1
                and a.Code Like @keyWord
                order by a.Code,a.SubCode
                OFFSET @pageIndex ROWS
				FETCH FIRST @pageRecordCount ROWS ONLY";
            SqlCommand cmd = db.GetCommand(sql);
            cmd.Parameters.AddWithValue("@pageIndex", pageRecordCount * (pageIndex - 1));
            cmd.Parameters.AddWithValue("@pageRecordCount", pageRecordCount);
            cmd.Parameters.AddWithValue("@keyWord", "%" + keyWord + "%");

            return db.GetDataTableWithClass<T>(cmd);
        }
        //新增
        public int AddRecord(CarbonEmissionFactorModel m)
        {
            Null2Empty(m);
            string sql = @"
            insert into CarbonEmissionFactor (
                Code,
                Item,
                KgCo2e,
                Unit,
                Kind,
                IsEnabled,
                SubCode,
                Memo,
                KeyCode1,
                KeyCode2,
                KeyCode3,
                CreateTime,
                CreateUserSeq,
                ModifyTime,
                ModifyUserSeq
            )values(
                @Code,
                @Item,
                @KgCo2e,
                @Unit,
                @Kind,
                @IsEnabled,
                @SubCode,
                @Memo,
                @KeyCode1,
                @KeyCode2,
                @KeyCode3,
                GetDate(),
                @ModifyUserSeq,
                GetDate(),
                @ModifyUserSeq
            )";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Code", m.Code);
                cmd.Parameters.AddWithValue("@Item", m.Item);
                cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(m.KgCo2e));
                cmd.Parameters.AddWithValue("@Unit", m.Unit);
                cmd.Parameters.AddWithValue("@Kind", m.Kind);
                cmd.Parameters.AddWithValue("@IsEnabled", m.IsEnabled);
                cmd.Parameters.AddWithValue("@SubCode", m.SubCode);
                cmd.Parameters.AddWithValue("@Memo", m.Memo);
                cmd.Parameters.AddWithValue("@KeyCode1", m.KeyCode1);
                cmd.Parameters.AddWithValue("@KeyCode2", m.KeyCode2);
                cmd.Parameters.AddWithValue("@KeyCode3", m.KeyCode3);

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                cmd.Parameters.Clear();
                string sql1 = @"SELECT IDENT_CURRENT('CarbonEmissionFactor') AS NewSeq";
                cmd = db.GetCommand(sql1);
                DataTable dt = db.GetDataTable(cmd);

                m.Seq = Convert.ToInt32(dt.Rows[0]["NewSeq"].ToString());

                return 0;
            }
            catch (Exception e)
            {
                log.Info("CarbonEmissionFactorService.AddRecords: " + e.Message);
                log.Info(m.Code);
                return -1;
            }
        }
        //更新
        public int UpdateRecord(CarbonEmissionFactorModel m)
        {
            Null2Empty(m);
            string sql = @"
            update CarbonEmissionFactor set 
                Code = @Code,
                Item = @Item,
                KgCo2e = @KgCo2e,
                Unit = @Unit,
                Kind = @Kind,
                IsEnabled = @IsEnabled,
                SubCode = @SubCode,
                Memo = @Memo,
                KeyCode1 = @KeyCode1,
                KeyCode2 = @KeyCode2,
                KeyCode3 = @KeyCode3,
                ModifyTime = GetDate(),
                ModifyUserSeq = @ModifyUserSeq,
                Green = @Green
            where Seq=@Seq";

            try
            {
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", m.Seq);
                cmd.Parameters.AddWithValue("@Code", m.Code);
                cmd.Parameters.AddWithValue("@Item", m.Item);
                cmd.Parameters.AddWithValue("@KgCo2e", this.NulltoDBNull(m.KgCo2e));
                cmd.Parameters.AddWithValue("@Unit", m.Unit);
                cmd.Parameters.AddWithValue("@Kind", m.Kind);
                cmd.Parameters.AddWithValue("@IsEnabled", m.IsEnabled);
                cmd.Parameters.AddWithValue("@SubCode", m.SubCode);
                cmd.Parameters.AddWithValue("@Memo", m.Memo);
                cmd.Parameters.AddWithValue("@KeyCode1", m.KeyCode1);
                cmd.Parameters.AddWithValue("@KeyCode2", m.KeyCode2);
                cmd.Parameters.AddWithValue("@KeyCode3", m.KeyCode3);
                cmd.Parameters.AddWithValue("@Green", m.Green);

                cmd.Parameters.AddWithValue("@ModifyUserSeq", getUserSeq());
                db.ExecuteNonQuery(cmd);

                return 0;
            }
            catch (Exception e)
            {
                log.Info("CarbonEmissionFactorService.UpdateRecords: " + e.Message);
                log.Info(m.Code);
                return -1;
            }
        }
        //刪除
        public int DelRecord(int seq)
        {
            string sql = "";
            db.BeginTransaction();
            try
            {
                sql = @"delete from CarbonEmissionFactor where Seq=@Seq";
                SqlCommand cmd = db.GetCommand(sql);
                cmd.Parameters.AddWithValue("@Seq", seq);
                db.ExecuteNonQuery(cmd);

                db.TransactionCommit();
                return 0;
            }
            catch (Exception e)
            {
                db.TransactionRollback();
                log.Info("CarbonEmissionFactorService.DelDelRecords: " + e.Message);
                log.Info(sql);
                return -1;
            }
        }
        //批次處裡
        public void ImportData(List<CarbonEmissionFactorModel> items, ref int iCnt, ref int uCnt, ref string errCnt)
        {
            SqlCommand cmd;
            string sql;
            foreach (CarbonEmissionFactorModel m in items)
            {
                Null2Empty(m);
                if (!String.IsNullOrEmpty(m.Code)) //shioulo 20220713 去除長度限制: && m.Code.Length>9)
                {
                    sql = @"SELECT Seq FROM CarbonEmissionFactor where Code=@Code";
                    cmd = db.GetCommand(sql);
                    cmd.Parameters.AddWithValue("@Code", m.Code);
                    DataTable dt = db.GetDataTable(cmd);
                    if (dt.Rows.Count == 1)
                    {
                        m.Seq = Convert.ToInt32(dt.Rows[0]["Seq"].ToString());
                        if (UpdateRecord(m) == -1)
                            errCnt += m.Code + ",";
                        else
                            uCnt++;
                    }
                    else
                    {
                        if (AddRecord(m) == -1)
                        {
                            m.KeyCode1 = m.Code;
                            m.KeyCode2 = "-1";
                            if (AddRecord(m) == -1)
                                errCnt += m.Code + ",";
                        }
                        else
                            iCnt++;
                    }
                } else
                    errCnt += m.Code + ",";
            }
        }
    }
}